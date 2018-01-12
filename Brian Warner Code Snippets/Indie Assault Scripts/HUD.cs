using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* For each player:
 * Draw Avatar
 * Draw Name
 * Draw Damage Percentage
 * Draw Smash Bar
 * Draw Stock	
*/

public class HUD : MonoBehaviour 
{
	//integer values can be outsourced from another script, just update the variables
	public Texture[] t_avatar = new Texture[4]; //face textures
	
	public Font f_damage; //damage percent font
	public int[] i_damage = new int[4]; //damage value
	public float[] fl_damageOffset = new float[4]; //y offset of damage text
	public float[] fl_damageAlpha = new float[4]; //alpha value of damage text
	public bool[] b_fadeOut = new bool[4]; //call fade out coroutine while this is active
	public bool[] b_fadeIn = new bool[4]; //call fade in coroutine while this is active
	bool[] b_faded = new bool[4];
	public float fl_fadeTargetOffset = .1f; //how far to move while fading
	
	//public Texture t_smashBack; //smash bar backing texture
	//public Texture t_smashFore; //smash bar filler texture
	public int[] i_smash = new int[4]; //smash bar value
	
	public Texture t_stock; //stock texture
	public int[] i_stock = new int[4]; //stock value
	
	public GUIStyle gs_gstyle = new GUIStyle(); //CSS 
	
	public int i_maxPlayers; //number of players to display information for
	public bool b_isStock; //is battle stock or timed?
	
	public string[] s_name = new string[4]; //name to display per player
	public Font f_name; //name font
	
	Texture t_pixel; //texture for drawing boxes
	public Texture t_meter; //texture for smash meter
	Color[] c_pColor; //color scheme for each player #
	float fl_xOff = Screen.width*.005f; //slight x offset used for centering hud
	
	float fl_flash = 1; //between 1 and 0 that determines alpha value. Change this over time to achieve a flashing effect
	float fl_flashSpeed = .05f; //flashing speed
	bool b_lowerFlash = true; //determines whether to increase or decrease the flash value
	
	public int[] i_fadeElement = new int[4]; //divisive integer. Set to 2 to halve the alpha of an element. Default 1. For hiding a player's HUD.
	
	public Font f_points; //font for indicating increases and decreases in score during timed matches
	public Font f_timer;  //font for timer 
	public float f_sec; //timer seconds. Start at 59, reset when less than 0 and decrease minutes.
	public int i_min; //timer minutes. Lower by 1 each time seconds hit less than 0. End game when both hit 0.
	public string[] s_score = new string[4];
	public string[] s_scorePending = new string[4];
	Queue <string>[] qs_scorePending = new Queue<string>[4];
	public bool[] b_showScore = new bool[4];
	public float fl_scoreTargetOffset = .1f;
	bool[] b_running = new bool[4];
	
	float[] fl_scoreOffset = new float[4];
	float[] fl_scoreAlpha = new float[4];

	float mpOffset = 0;
	
	
	void Start()
	{
		//set player colors
		c_pColor = new Color[4];
		c_pColor[0] = Color.red;
		c_pColor[1] = Color.blue;
		c_pColor[2] = Color.yellow;
		c_pColor[3] = Color.green;
		
		t_pixel = (Texture)Resources.Load("white"); //load pixel texture from Resources folder
		
		for(int i = 0;i < 4;i++)
		{
			fl_damageAlpha[i] = 1;
			b_fadeIn[i] = false;
			b_fadeOut[i] = false;
			i_fadeElement[i] = 1;
			s_score[i] = "+1";
			s_scorePending[i] = "";
			qs_scorePending[i] = new Queue<string>();
		}
	}

	void FixedUpdate()
	{
		flashUpdate();
		if(!b_isStock)
			timeUpdate();
	}
	
	void Update()
	{
		if(!b_isStock)
			updateScoreQueues(); 
		for(int i = 0; i<4; i++)
		{
			if(b_fadeOut[i])
			{
				StartCoroutine("fadeOut",i);
			}
			if(b_fadeIn[i])
			{
				StartCoroutine("fadeIn",i);
			}
			if(b_showScore[i] && !b_isStock)
			{
				StartCoroutine(scoreUpdate(i));
			}
		}

	}
	
	void OnGUI()
	{
		
		for(int i = 0; i < i_maxPlayers; i++)
		{
			switch(i_maxPlayers) //offset player HUD elements based on max player count
			{
			case 3:
				mpOffset = -Screen.width*i/4 + Screen.width*i/3 +Screen.width*.125f/3;;
				break;
			case 2:
				mpOffset = Screen.width/4;
				break;
			case 1:
				mpOffset = Screen.width*1.5f/4;
				break;
			default:
				mpOffset = 0;
				break;
			}
			// Avatar Display
			Rect r_facePos = new Rect(Screen.width*i/4+Screen.width/12-Screen.width*1/64+fl_xOff + mpOffset,Screen.height*6.75f/8,Screen.width/12,Screen.height/10); //avatar position
			Rect r_boxPos = new Rect(r_facePos.x,r_facePos.y-Screen.width/128,r_facePos.width,r_facePos.height+Screen.width/128);
			GUI.color = new Color(.5f+.5f*i_damage[i]/200,.5f,.5f,.5f/i_fadeElement[i]);
			GUI.DrawTexture(r_boxPos,t_pixel); //draw box
			GUI.color = new Color(1,1,1,(fl_damageAlpha[i]+.5f)/i_fadeElement[i]);
			GUI.DrawTexture(r_facePos,t_avatar[i]); //draw avatar
			
			// Name Display
			Rect r_nboxPos = new Rect(r_boxPos.x,Screen.height*7.6f/8,r_boxPos.width,Screen.height/35);
			GUI.color = c_pColor[i];
			GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, GUI.color.a*.75f/i_fadeElement[i]);
			GUI.DrawTexture(r_nboxPos,t_pixel); //draw name box
			GUI.color = Color.white;
			
			gs_gstyle.font = f_name;
			gs_gstyle.fontSize = Mathf.Min(Screen.width,Screen.height)/40;
			gs_gstyle.alignment = TextAnchor.UpperCenter;
			gs_gstyle.normal.textColor = new Color(0,0,0,(float)1/i_fadeElement[i]);
			Rect r_namePos = new Rect(Screen.width*i/4 + Screen.width/12+fl_xOff-Screen.width*.77f/128 + mpOffset,Screen.height*7.6f/8,Screen.width/16,0); //name position
			Outline(r_namePos,s_name[i],gs_gstyle); //outline text
			gs_gstyle.normal.textColor = new Color(1,1,1,(float)1/i_fadeElement[i]);
			GUI.Label(r_namePos,s_name[i],gs_gstyle); //display text
			
			// Damage Display
			gs_gstyle.font = f_damage;
			gs_gstyle.fontSize = Mathf.Min(Screen.width,Screen.height)/25;
			gs_gstyle.alignment = TextAnchor.UpperRight;
			gs_gstyle.normal.textColor = new Color(0,0,0,fl_damageAlpha[i]/i_fadeElement[i]); 
			Rect r_damPos = new Rect(Screen.width*i/4 + Screen.width/8+fl_xOff + mpOffset,Screen.height*7.3f/8+fl_damageOffset[i],Screen.width/16,0); //text position
			Outline(r_damPos,i_damage[i].ToString()+"%",gs_gstyle); //outline text
			if(i_damage[i] <= 100) //low damage, white to yellow
			{
				gs_gstyle.normal.textColor = new Color(1,1,1-(float)i_damage[i]/100,fl_damageAlpha[i]/i_fadeElement[i]);
			}
			else if(i_damage[i]<= 200) //moderate damage, yellow to red
			{
				gs_gstyle.normal.textColor = new Color(1,1-((float)i_damage[i]-100)/100,0,fl_damageAlpha[i]/i_fadeElement[i]);
			}
			else //extreme damage, red
			{
				gs_gstyle.normal.textColor = new Color(1,0,0,fl_damageAlpha[i]/i_fadeElement[i]);
			}
			GUI.Label(r_damPos,i_damage[i].ToString()+"%",gs_gstyle); //display text
			
			// Score Display
			gs_gstyle.font = f_points;
			gs_gstyle.alignment = TextAnchor.UpperLeft;
			gs_gstyle.fontSize = Mathf.Min(Screen.width,Screen.height)/35;
			gs_gstyle.normal.textColor = new Color(1,1,1,fl_scoreAlpha[i]);
			gs_gstyle.fontStyle = FontStyle.BoldAndItalic;
			Rect r_scorePos = new Rect(Screen.width*i/4 + Screen.width/8+fl_xOff + Screen.width/28 + mpOffset,Screen.height*6.4f/8+fl_scoreOffset[i],0,0); //score text position


			Outline(r_scorePos,s_score[i],gs_gstyle);
			if(s_score[i] == "+1")
				gs_gstyle.normal.textColor = new Color(1,0,0,fl_scoreAlpha[i]);
			else
				gs_gstyle.normal.textColor = new Color(0,0,1,fl_scoreAlpha[i]);
			GUI.Label(r_scorePos,s_score[i],gs_gstyle); //display score text
			gs_gstyle.fontStyle = FontStyle.Normal;
			
			
			// Stock Display
			if(b_isStock)
			{
				Color c_temp = c_pColor[i];
				c_temp.a = c_temp.a/i_fadeElement[i];
				GUI.color = c_temp;
				if(i_stock[i] <=5) //5- show stock tally
				{
					tallyStock(i);
				}
				else //6+ show stock multiplier
				{
					Rect r_stockPos = new Rect(Screen.width*i/4+Screen.width/14.9f+fl_xOff + mpOffset,Screen.height*6.65f/8,Screen.width/48,Screen.width/48); //stock icon position/size
					GUI.DrawTexture(r_stockPos,t_stock); //draw stock icon
					GUI.color = new Color(1,1,1,(float)1/i_fadeElement[i]);
					gs_gstyle.font = f_name;
					gs_gstyle.fontSize = Mathf.Min(Screen.width,Screen.height)/40;
					gs_gstyle.alignment = TextAnchor.LowerLeft;
					gs_gstyle.normal.textColor = new Color(0,0,0,(float)1/i_fadeElement[i]);
					Rect r_tallyPos = new Rect(Screen.width*i/4+Screen.width/15.5f+Screen.width*1.5f/64+fl_xOff + mpOffset,Screen.height*6.625f/8,Screen.width/48,Screen.width/48); //tally text position/size
					Outline(r_tallyPos,"x " + i_stock[i].ToString(),gs_gstyle); //set tally text outline
					gs_gstyle.normal.textColor = new Color(1,1,1,(float)1/i_fadeElement[i]);
					GUI.Label(r_tallyPos,"x " + i_stock[i].ToString(),gs_gstyle); //draw tally text
				}
				GUI.color = Color.white;
			}
			
			//Final Smash Bar Display
			Rect r_barPos = new Rect(r_facePos.x,Screen.height*6.45f/8,r_boxPos.width,Screen.height/50); //bar position/size
			GUI.color = new Color(.5f,.5f,.5f,(float)1/i_fadeElement[i]);
			GUI.DrawTexture(r_barPos,t_pixel); //draw smash bar backing
			Rect r_meterPos = new Rect(r_barPos.x+Screen.width/448,r_barPos.y+Screen.height*.2f/64,(float)100/100*(r_barPos.width-Screen.width*2/448),r_barPos.height-Screen.height*1.05f/160); //empty meter position/size
			GUI.color = new Color(1,1,1,(float)1/1);
			if(i_fadeElement[i]==1)
				GUI.DrawTexture(r_meterPos,t_meter); //draw smash meter
			if(i_smash[i] == 100) //flash red when smash bar is full
			{
				GUI.color = new Color(1,1,1,fl_flash/1);
				if(i_fadeElement[i]==1)
					GUI.DrawTexture(r_meterPos,t_pixel); //draw red flash
				
			}
			Rect r_emptyPos = new Rect(r_barPos.x+Screen.width/448,r_barPos.y+Screen.height*.2f/64,(float)100/100*(r_barPos.width-Screen.width*2/448),r_barPos.height-Screen.height*1.05f/160); //empty meter position/size
			r_emptyPos.x += (float)i_smash[i]/100*(r_barPos.width-Screen.width*2/512);
			r_emptyPos.width -= (float)i_smash[i]/100*(r_barPos.width-Screen.width*2/512);
			GUI.color = new Color(.25f,.25f,.25f,(float)1/i_fadeElement[i]);
			if(i_smash[i]<100)
					GUI.DrawTexture(r_emptyPos,t_pixel); //draw empty meter
			GUI.color = Color.white;
			
			if(!b_isStock)
				timeDisplay();
		}
	}

	// Draws a tally of stock images
	void tallyStock(int plr) 
	{
		for(int i = 0; i < i_stock[plr]; i++)
		{
			Rect r_stockPos = new Rect(Screen.width*plr/4+Screen.width/14.9f+Screen.width*i/64+fl_xOff + mpOffset,Screen.height*6.65f/8,Screen.width/48,Screen.width/48);
			GUI.DrawTexture(r_stockPos,t_stock);
		}
	}

	// Updates the flasher variable to bounce between 0 and 1, for creating a flash effect
	void flashUpdate() 
	{
		if(b_lowerFlash) //lower flash alpha
		{
			fl_flash-= fl_flashSpeed;
			if(fl_flash < 0)
			{
				fl_flash = 0;
				b_lowerFlash = false;
			}
		}
		else if(!b_lowerFlash) //increase flash alpha
		{
			fl_flash+= fl_flashSpeed;
			if(fl_flash > 1)
			{
				fl_flash = 1;
				b_lowerFlash = true;
			}
		}
	}

	// Fade out damage display, resets damage and smash meter, and removes a stock if playing stock mode
	IEnumerator fadeOut(int i) 
	{
		b_faded[i] = true;
		b_fadeOut[i] = false;

		int oriDamage = i_damage[i];
		int oriSmash = i_smash[i];
		if(b_isStock)
			i_stock[i]--;
		
		for(float f = 1f; f >= 0; f-= .05f)
		{
			fl_damageAlpha[i] = f;
			fl_damageOffset[i]=Screen.height*fl_fadeTargetOffset - Screen.height*fl_fadeTargetOffset*f;
			i_damage[i] = (int)Mathf.Floor((float)oriDamage*f);
			i_smash[i] = (int)Mathf.Floor((float)oriSmash*f);
			yield return new WaitForFixedUpdate();    
		}
		i_damage[i] = 0;
		i_smash[i] = 0;
		b_faded[i] = false;
	}

	// Fades in damage display
	IEnumerator fadeIn(int i) 
	{

		b_fadeIn[i] = false;
		if(b_faded[i])
			yield break;
		for(float f = 0; f <= 1; f+=.05f)
		{
			if(b_faded[i])
			{
				fl_damageAlpha[i] = 1;
				fl_damageOffset[i]= -Screen.height*fl_fadeTargetOffset +Screen.height*fl_fadeTargetOffset*1;
				yield break;
			}
			fl_damageAlpha[i] = f;
			fl_damageOffset[i]= -Screen.height*fl_fadeTargetOffset +Screen.height*fl_fadeTargetOffset*f;
			yield return new WaitForFixedUpdate();    
		}
	}

	// Used to outline text. Also use gui.label afterwards in the text's normal color.
	void Outline(Rect area, string content, GUIStyle gs) 
	{
		Rect[] outArea = new Rect[4];
		outArea[0].Set(area.x+1,area.y,area.width,area.height);
		outArea[1].Set(area.x,area.y+1,area.width,area.height);
		outArea[2].Set(area.x-1,area.y,area.width,area.height);
		outArea[3].Set(area.x,area.y-1,area.width,area.height);
		for(int i=0;i<4;i++)
		{
			GUI.Label(outArea[i],content,gs);
		}
	}

	// Update timer
	void timeUpdate() 
	{
		if(f_sec>0)
			f_sec-=Time.fixedDeltaTime;
		else if(i_min>0)
		{
			f_sec = 60 - Time.fixedDeltaTime + f_sec;
			i_min--;
		}
		else 
		{
			f_sec = 0;
		}
	}

	// Display timer
	void timeDisplay() 
	{
		gs_gstyle.alignment = TextAnchor.UpperCenter;
		gs_gstyle.font = f_timer;
		gs_gstyle.normal.textColor = Color.white;
		Rect r_timePos = new Rect(Screen.width/2,Screen.height/64,0,0);
		GUI.Label(r_timePos,Mathf.Floor(i_min).ToString("00")+":"+f_sec.ToString("00")+":"+f_sec.ToString("00.00").Substring(3),gs_gstyle);
	}
	
	public IEnumerator scoreUpdate(int i) //used to show score updates on-kill, death, or suicide
	{
		b_showScore[i] = false; //reset trigger
		while(b_running[i]) //wait for other instances of the coroutine to finish
			yield return null;
		if(qs_scorePending[i].Count == 0) //if queue empty, end here
			yield break;
		b_running[i] = true; //set coroutine status to active to prevent others from starting
		string update = qs_scorePending[i].Peek(); //get last saved item in queue
		qs_scorePending[i].Dequeue(); //pop the item in the queue
		s_score[i] = update; //set display string
		
		for(float f = 0; f<= 1; f+=.1f) //pop up score text and fade in
		{
			fl_scoreAlpha[i] = f;
			fl_scoreOffset[i] = Screen.height*fl_scoreTargetOffset - Screen.height*fl_scoreTargetOffset*f;
			yield return null;
		}

		yield return new WaitForSeconds(.25f); //pause
		
		for(float f = 1f; f >= 0; f-= .1f) //pop down score text and fade out
		{
			fl_scoreAlpha[i] = f;
			fl_scoreOffset[i] = Screen.height*fl_scoreTargetOffset - Screen.height*fl_scoreTargetOffset*f;
			yield return null;
		}

		fl_scoreAlpha[i] = 0; //reset alpha to 0
		b_running[i] = false; //deactivate to allow the next coroutine to take over
	}

	void updateScoreQueues() //adds items to the score update queue 
	{
		for(int i = 0; i<i_maxPlayers; i++)
		{
			if(s_scorePending[i] != "")
			{
				qs_scorePending[i].Enqueue(s_scorePending[i]);
				s_scorePending[i] = "";
			}
		}
	}

	public void addScore(string content, int i) //used to trigger score updates. 
	{
		s_scorePending[i] = content;
		b_showScore[i] = true;
	}
}
