using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//egg drop main code

public class Main : MonoBehaviour {
	bool hasTouched;
	float ttimer;
	Vector3 tappos;
	Vector3 touchpos;
	bool tapped;
	bool touching;
	bool stoptouch;
	float taptimer;

	float malpha;
	float tmtimer;
	string tmessage;
	bool showlog;
	public Texture t_log;
	public Texture t_print;
	string[] sblock = new string[3];
	public int boxtype;
	public int lastMat;
	public int lastType;
	public tarray[] mats = new tarray[6];
	public GameObject help;
	public Texture t_help;
	int bdrag = -1;//
	public Texture t_trash;
	bool clicked = false;
	bool dragmode = false;
	int matdrag = 0;
	public Texture t_yarrow;
	public int[] lineup = new int[4];
	public Texture[] t_emats;
	public Texture[] t_hmats;
	public Texture t_bigegg;
	int screen = 0;
	public GameObject bg1;
	public Texture[] t_bmaterials;
	public Texture t_button;
	public Texture t_test;
	public Texture t_reset;
	public Texture t_rewrap;
	public Texture t_drop;
	public Texture white;
	int block;
	public GameObject[] characters;
	Vector3[] chpos = new Vector3[3];
	public bool testing;
	public bool showresults;
	float[] impact = new float[3];
	public float defense;
	public bool broken;
	public GameObject currentChar;
	public GameObject currentEgg;
	public Texture popup;
	public Texture tcontinue;
	bool showprint;
	trialinfo[] b1_info = new trialinfo[3];
	trialinfo[] b2_info = new trialinfo[3];
	trialinfo[] b3_info = new trialinfo[3];
	string[] slmat = new string[6];

	int tnum1; //track trial count up to 3. Beyond that, start overwriting old trial log values with the 3 most recent
	int tnum2;
	int tnum3;

	[System.Serializable]
	public class tarray
	{
		public Texture[] omats = new Texture[4];
	}

	public class trialinfo
	{
		public int[] lineup = new int[4];
		public bool success = false;
		public bool show = false;

		public trialinfo()
		{
			for(int i= 0; i<4; i++)
			{
				lineup[i] = -1;
			}
			show = false;
		}
	}

	void Print()
	{
		string[] spmat = new string[8];
		spmat [0] = "Cotton   |";
		spmat [1] = "T.Paper  |";
		spmat [2] = "L.Box    |";
		spmat [3] = "C.Box    |";
		spmat [4] = "Tin      |";
		spmat [5] = "F.Peanuts|";
		spmat [6] = "-        |";
		spmat [7] = "         |";
		string[] spres = new string[3];
		spres [0] = "Success|\n";
		spres [1] = "Fail   |\n";
		spres [2] = "       |\n";
		string[] spline = new string[4];
		spline [0] = "-----------------------------------------------------------------\n";
		spline [1] = "|Blocks |Trials |Materials                              |Results|\n";
		spline [2] = "|       ---------------------------------------------------------\n";
		spline [3] = "-----------------------------------------------------------------";
		string fchart = "";
		fchart += spline [0];
		fchart += spline [1];
		fchart += spline [0];

		for (int i = 0; i < 3; i++)
		{
			if (i == 0)
				fchart += "|1 Meter|Trial " + (i + 1).ToString () + "|";
			else
				fchart += "|       |Trial " + (i + 1).ToString () + "|";
			
			if (!b1_info [i].show)
			{
				fchart += spmat [7] + spmat [7] + spmat [7] + spmat [7] + spres [2];
			}
			else
			{
				for (int j = 0; j < 4; j++)
				{
					if (b1_info [i].lineup [j] == -1)
						fchart += spmat [6];
					else
						fchart += spmat [b1_info [i].lineup [j]];
				}

				if (b1_info [i].success)
					fchart += spres [0];
				else
					fchart += spres [1];
			}
			if (i < 2)
			{
				fchart += spline [2];
			}
			else
			{
				fchart += spline [0];
			}
		}

		for (int i = 0; i < 3; i++)
		{
			if (i == 0)
				fchart += "|3 Meter|Trial " + (i + 1).ToString () + "|";
			else
				fchart += "|       |Trial " + (i + 1).ToString () + "|";

			if (!b2_info [i].show)
			{
				fchart += spmat [7] + spmat [7] + spmat [7] + spmat [7] + spres [2];
			}
			else
			{
				for (int j = 0; j < 4; j++)
				{
					if (b2_info [i].lineup [j] == -1)
						fchart += spmat [6];
					else
						fchart += spmat [b2_info [i].lineup [j]];
				}

				if (b2_info [i].success)
					fchart += spres [0];
				else
					fchart += spres [1];
			}
			if (i < 2)
			{
				fchart += spline [2];
			}
			else
			{
				fchart += spline [0];
			}
		}

		for (int i = 0; i < 3; i++)
		{
			if (i == 0)
				fchart += "|5 Meter|Trial " + (i + 1).ToString () + "|";
			else
				fchart += "|       |Trial " + (i + 1).ToString () + "|";

			if (!b3_info [i].show)
			{
				fchart += spmat [7] + spmat [7] + spmat [7] + spmat [7] + spres [2];
			}
			else
			{
				for (int j = 0; j < 4; j++)
				{
					if (b3_info [i].lineup [j] == -1)
						fchart += spmat [6];
					else
						fchart += spmat [b3_info [i].lineup [j]];
				}

				if (b3_info [i].success)
					fchart += spres [0];
				else
					fchart += spres [1];
			}
			if (i < 2)
			{
				fchart += spline [2];
			}
			else
			{
				fchart += spline [3];
			}
		}

		Application.ExternalCall("SaveChart", fchart);

		tmessage = "Chart Print Successful...";
		tmtimer = 1;

/* Example Chart
-----------------------------------------------------------------
|Blocks |Trials |Materials                              |Results|
-----------------------------------------------------------------
|1 Meter|Trial 1|         |         |         |         |       |
|       ---------------------------------------------------------
|       |Trial 2|         |         |         |         |       |
|       ---------------------------------------------------------
|       |Trial 3|         |         |         |         |       |
-----------------------------------------------------------------
|3 Meter|Trial 1|         |         |         |         |       |
|       ---------------------------------------------------------
|       |Trial 2|         |         |         |         |       |
|       ---------------------------------------------------------
|       |Trial 3|         |         |         |         |       |
-----------------------------------------------------------------
|5 Meter|Trial 1|         |         |         |         |       |
|       ---------------------------------------------------------
|       |Trial 2|         |         |         |         |       |
|       ---------------------------------------------------------
|       |Trial 3|         |         |         |         |       |
-----------------------------------------------------------------
*/

	}

	void Start()
	{
		for (int i = 0; i < 3; i++)
		{
			b1_info [i] = new trialinfo ();
			b2_info [i] = new trialinfo ();
			b3_info [i] = new trialinfo ();
		}

		slmat [0] = "Cotton";
		slmat [1] = "T.Paper";
		slmat [2] = "L.Box";
		slmat [3] = "C.Box";
		slmat [4] = "Tin";
		slmat [5] = "F.Peanuts";

		sblock[0] = "1 Meter";
		sblock[1] = "3 Meters";
		sblock[2] = "5 Meters";

		chpos [0] = new Vector3(-9.67f,-0.97f,-1f);
		chpos [1] = new Vector3(-9.47f,2.21f,-1f);
		chpos [2] = new Vector3(-10.26f,4.89f,-1f);

		impact [0] = 7;
		impact [1] = 13;
		impact [2] = 16;

		for (int i = 0; i < 4; i++)
		{
			lineup [i] = -1;
		}
	}

	// Update is called once per frame
	void Update () {
		tapped = false;
		touching = false;
		stoptouch = false;
		if (taptimer > 0)
		{
			taptimer -= Time.deltaTime;
			if (Input.touches [0].position != Vector2.zero)
			{
				tappos = Input.touches [0].position;
				tappos.y = Screen.height - tappos.y;
			}
		}
		if (Input.touchCount >= 1)
		{
			hasTouched = true;	
			if (Input.touches [0].phase == TouchPhase.Began || !touching)
			{
				tapped = true;
				if (Input.touches [0].position != Vector2.zero)
				{
					tappos = Input.touches [0].position;
					tappos.y = Screen.height - tappos.y;
				}
				ttimer = .5f;
				taptimer = .25f;
			}
			else if (ttimer > 0 && !dragmode)
			{
				ttimer -= Time.deltaTime;
				if (ttimer <= 0)
				{
					tapped = true;
					if (Input.touches [0].position != Vector2.zero && bdrag<0)
					{
						tappos = Input.touches [0].position;
						tappos.y = Screen.height - tappos.y;
					}
					ttimer = .5f;
				}
			}
			if (Input.touches [0].phase == TouchPhase.Ended || Input.touches [0].phase == TouchPhase.Canceled)
			{
				stoptouch = true;
			}
			if (Input.touches [0].position != Vector2.zero)
			{
				touchpos = Input.touches [0].position;
				touchpos.y = Screen.height - touchpos.y;
			}
			touching = true;
		}
		else
		{
			ttimer = 0;
		}

		if (tmtimer > 0)
		{
			tmtimer -= Time.deltaTime;
			malpha = 1;
		}
		else if (malpha > 0)
		{
			malpha -= Time.deltaTime;
		}
		if (screen == 0 && !bg1.activeSelf)
		{
			bg1.SetActive (true);
		}
		else if (screen == 1 && bg1.activeSelf)
		{
			bg1.SetActive (false);
		}

		clicked = Input.GetMouseButtonDown (0);
	}

	void OnGUI(){
		GUIStyle gstyle = new GUIStyle ();
		gstyle.normal.textColor = Color.white;
		int h = Screen.height;
		int w = Screen.width;
		gstyle.fontSize = Mathf.RoundToInt (h*.035f);
		gstyle.wordWrap = true;

		Vector3 mp = Input.mousePosition;
		mp.y = h - mp.y;
		Rect rflat = new Rect (0, 0, h * .025f, h * .025f);
		rflat.center = new Vector2 (tappos.x, tappos.y);
		GUI.color = new Color (1, 0, 0, .5f);
		if (hasTouched)
		{
			GUI.DrawTexture (rflat, white);
		}
		GUI.color = Color.white;

		if (malpha > 0)
		{
			Rect rmessage = new Rect (w * .5f, h * .95f, 0, 0);
			GUIStyle mstyle = new GUIStyle (gstyle);
			mstyle.alignment = TextAnchor.MiddleCenter;
			GUI.color = new Color (1, 1, 1, malpha);
			GUI.Label (rmessage, tmessage, mstyle);
			GUI.color = Color.white;
		}

		if (screen == 0)
		{
			Rect r_help = new Rect (0,0,h*.1f, h*.1f);
			GUI.DrawTexture (r_help, t_help, ScaleMode.ScaleToFit);
			if (GUI.Button (r_help, "", gstyle) || (r_help.Contains(tappos)&&taptimer>0))
			{
				if (!help.activeSelf)
				{
					taptimer = 0;
					tapped = false;
					help.GetComponent<Help> ().progression = 0;
					help.SetActive (true);
				}
			}
			bool showhelp = help.activeSelf;
			if (showhelp)
			{
				clicked = false;
			}

			Rect mbrect = new Rect (0, 0, w * .09f, h * .15f);
			mbrect.height = mbrect.width * 1.2f;
			mbrect.x = w - mbrect.width * 2 - w * .05f;
			mbrect.y = h * .175f;
			float eggboffset = mbrect.height + h * .025f;
			for (int i = 0; i <= 4; i += 2)
			{
				//column 1
				if ( (clicked && mbrect.Contains (mp)) || (taptimer>0 && mbrect.Contains (tappos) && bdrag<0) )
				{
					taptimer = 0;
					bdrag = i;
					clicked = false;
				}
				if (bdrag == i)
					GUI.DrawTexture (mbrect, t_hmats [i]);
				else
					GUI.DrawTexture (mbrect, t_bmaterials [i]);
				mbrect.y += eggboffset;
			}

			mbrect.x = w - mbrect.width - w * .04f;
			mbrect.y = h * .1f;
			for (int i = 1; i <= 5; i += 2)
			{
				//column 2
				if (clicked && mbrect.Contains (mp) || (taptimer>0 && mbrect.Contains (tappos) && bdrag < 0))
				{
					taptimer = 0;
					bdrag = i;
					clicked = false;
				}
				if (bdrag == i)
					GUI.DrawTexture (mbrect, t_hmats [i]);
				else
					GUI.DrawTexture (mbrect, t_bmaterials [i]);
				mbrect.y += eggboffset;
			}

			Rect btrect = new Rect (0, h * .065f, 0, h * .1f);
			float btxoff = w * .0075f;
			btrect.width = btrect.height * 3.43f;
			btrect.x = w * .5f - btrect.width - btxoff;
			GUI.DrawTexture (btrect, t_test);
			if ( (GUI.Button (btrect, "", gstyle) || (taptimer>0 && btrect.Contains(tappos))) && !showhelp)
			{
				taptimer = 0;
				screen = 1;
			}
			btrect.x = w * .5f + btxoff;
			GUI.DrawTexture (btrect, t_reset);
			if ( (GUI.Button (btrect,"",gstyle) || (taptimer>0 && btrect.Contains(tappos))) && !showhelp)
			{
				taptimer = 0;
				for (int i = 0; i < lineup.Length; i++)
				{
					lineup [i] = -1;
				}
			}

			//big egg
			Rect r_bigegg = new Rect(w*.5f,h*.6f,w*.2f,w*.3f);
			r_bigegg.height = r_bigegg.width * 1.2f;
			r_bigegg.x -= r_bigegg.width * .5f;
			r_bigegg.y -= r_bigegg.height * .5f;
			GUI.DrawTexture (r_bigegg, t_bigegg);
			if (bdrag >= 0 && r_bigegg.Contains (touchpos) && touching)
			{
				if (Input.touches [0].phase == TouchPhase.Ended || Input.touches [0].phase == TouchPhase.Canceled)
				{
					for (int i = 0; i < lineup.Length; i++)
					{
						if (lineup [i] == -1)
						{
							if (bdrag != 2 && bdrag != 3)
							{
								lineup [i] = bdrag;
							}
							else
							{
								bool nobox = true;
								for (int j = 0; j < lineup.Length; j++)
								{
									if (lineup [j] == 2 || lineup [j] == 3)
									{
										nobox = false;
									}
								}

								if (nobox)
								{
									lineup [i] = bdrag;
								}
							}
							break;
						}
					}
				}
			}
			else if (bdrag >= 0 && r_bigegg.Contains (mp) && !Input.GetMouseButton (0) && !touching)
			{
				for (int i = 0; i < lineup.Length; i++)
				{
					if (lineup [i] == -1)
					{
						if (bdrag != 2 && bdrag != 3)
						{
							lineup [i] = bdrag;
						}
						else
						{
							bool nobox = true;
							for (int j = 0; j < lineup.Length; j++)
							{
								if (lineup [j] == 2 || lineup [j] == 3)
								{
									nobox = false;
								}
							}

							if (nobox)
							{
								lineup [i] = bdrag;
							}
						}
						break;
					}
				}
			}

			Rect r_lineup = new Rect (w*.5f,h*.6f,w*.3f,w*.3f);
			r_lineup.x -= r_lineup.width * .5f;
			r_lineup.y -= r_lineup.height * .5f;
			lastMat = -1;
			lastType = -1;
			for (int i = 0; i < 4; i++)
			{
				if (lineup [i] >= 0)
				{
					if (lineup [i] != 2 && lineup [i] != 3)
					{
						lastType = lineup [i];
						lastMat = i;

						if (i + 1 > 3)
							GUI.DrawTexture (r_lineup, mats [lineup [i]].omats [i], ScaleMode.ScaleToFit);
						else if (lineup [i + 1] == -1)
							GUI.DrawTexture (r_lineup, mats [lineup [i]].omats [i], ScaleMode.ScaleToFit);
						else
						{
						}
					}
					else
					{
						//GUI.DrawTexture (r_lineup, mats [lineup [i]].omats [0], ScaleMode.ScaleToFit);
					}
					Vector2 center = r_lineup.center;
					r_lineup.center = center;
				}
			}
			boxtype = -1;
			Vector2 c = r_lineup.center;
			r_lineup.width *= 1.4f;
			r_lineup.height *= 1.4f;
			r_lineup.center = c;
			for (int i = 0; i < 4; i++)
			{
				if (lineup [i] == 2 || lineup [i] == 3)
				{
					if (i == 3)
					{
						boxtype = lineup [i];
						//GUI.color = new Color (1, 1, 1, .7f);
						GUI.DrawTexture (r_lineup, mats [lineup [i]].omats [0], ScaleMode.ScaleToFit);
						//GUI.color = Color.white;

					}
					else if (lineup [i + 1] != -1)
					{
						int a = 1;
						if (i + 2 < 4)
						{
							if (lineup [i + 2] != -1)
								a = 2;
						}
						if (i + 3 < 4)
						{
							if (lineup [i + 3] != -1)
								a = 3;
						}
						boxtype = lineup [i + a];
						//GUI.color = new Color (1, 1, 1, .7f);
						GUI.DrawTexture (r_lineup, mats [6].omats [lineup [i + a]], ScaleMode.ScaleToFit);
						//GUI.color = Color.white;

					}
					else
					{
						boxtype = lineup [i];
						//GUI.color = new Color (1, 1, 1, .7f);
						GUI.DrawTexture (r_lineup, mats [lineup [i]].omats [0], ScaleMode.ScaleToFit);
						//GUI.color = Color.white;

					}
				}
			}

			//yellow arrow and display material order
			Rect r_yarrow = new Rect(w*.35f,h*.225f,h*.05f,h*.05f);
			GUI.DrawTexture (r_yarrow, t_yarrow, ScaleMode.ScaleToFit);

			Rect r_trash = new Rect(0,h*.205f, h*.08f,h*.08f);
			r_trash.x = r_yarrow.x - r_trash.width - h * .02f;
			if (dragmode)
			{
				if (!touching)
				{
					if (r_trash.Contains (mp))
					{
						Rect r_backtrash = new Rect (r_trash);
						r_backtrash.yMin -= h * .02f;
						r_backtrash.width = r_backtrash.height;
						r_backtrash.center = r_trash.center;
						r_backtrash.y = h * .185f;
						GUI.color = new Color (1, 0, 0, .6f);
						GUI.DrawTexture (r_backtrash, white);
						GUI.color = Color.white;
						if (!Input.GetMouseButton (0))
						{
							lineup [matdrag] = -1;

							for (int i = matdrag; i < lineup.Length; i++)
							{
								if (lineup [i] == -1)
								{
									for (int j = i; j < lineup.Length; j++)
									{
										if (lineup [j] != -1)
										{
											lineup [i] = lineup [j];
											lineup [j] = -1;
											break;
										}
									}
								}
							}
						}
					}
				}
				else
				{
					if (r_trash.Contains (touchpos))
					{
						Rect r_backtrash = new Rect (r_trash);
						r_backtrash.yMin -= h * .02f;
						r_backtrash.width = r_backtrash.height;
						r_backtrash.center = r_trash.center;
						r_backtrash.y = h * .185f;
						GUI.color = new Color (1, 0, 0, .6f);
						GUI.DrawTexture (r_backtrash, white);
						GUI.color = Color.white;
						if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
						{
							lineup [matdrag] = -1;

							for (int i = matdrag; i < lineup.Length; i++)
							{
								if (lineup [i] == -1)
								{
									for (int j = i; j < lineup.Length; j++)
									{
										if (lineup [j] != -1)
										{
											lineup [i] = lineup [j];
											lineup [j] = -1;
											break;
										}
									}
								}
							}
						}
					}
				}
			}
			GUI.DrawTexture (r_trash, t_trash, ScaleMode.ScaleToFit);

			float omx = w * .35f + r_yarrow.width + h * .015f;
			Rect r_omats = new Rect (omx, h*.185f, h*.1f,h*.1f);
			for (int i = 0; i < lineup.Length; i++)
			{
				if (lineup [i] >= 0)
				{
					r_omats.x = omx + i*(r_omats.width + h*.01f);
					if (!dragmode)
					{
						if ( (r_omats.Contains(mp) && clicked)  || (taptimer>0 && r_omats.Contains(tappos)))
						{
							taptimer = 0;
							clicked = false;
							dragmode = true;
							matdrag = i;
						}
					}
					else
					{
						if (!touching)
						{
							if (Input.GetMouseButton (0) && !showhelp)
							{
								if (i == matdrag)
								{
									GUI.color = new Color (1, 1, 0, .6f);
									GUI.DrawTexture (r_omats, white);
									GUI.color = Color.white;
								}
								else if (r_omats.Contains (mp))
								{
									GUI.color = new Color (0, 1, 1, .6f);
									GUI.DrawTexture (r_omats, white);
									GUI.color = Color.white;
								}
							}
							else
							{
								if (r_omats.Contains (mp))
								{
									if (i != matdrag)
									{
										int temp = lineup [i];
										lineup [i] = lineup [matdrag];
										lineup [matdrag] = temp;
									}
								}
							}
						}
						else
						{
							if (Input.touches[0].phase != TouchPhase.Canceled && Input.touches[0].phase != TouchPhase.Ended && !showhelp)
							{
								if (i == matdrag)
								{
									GUI.color = new Color (1, 1, 0, .6f);
									GUI.DrawTexture (r_omats, white);
									GUI.color = Color.white;
								}
								else if (r_omats.Contains (touchpos))
								{
									GUI.color = new Color (0, 1, 1, .6f);
									GUI.DrawTexture (r_omats, white);
									GUI.color = Color.white;
								}
							}
							else
							{
								if (r_omats.Contains (touchpos))
								{
									if (i != matdrag)
									{
										int temp = lineup [i];
										lineup [i] = lineup [matdrag];
										lineup [matdrag] = temp;
									}
								}
							}
						}
					}

					GUI.DrawTexture (r_omats, t_emats [lineup [i]], ScaleMode.ScaleToFit);
				}
			}
			if (!Input.GetMouseButton (0) && !touching)
			{
				dragmode = false;
				bdrag = -1;
			}
			else if (touching)
			{
				if (stoptouch)
				{
					dragmode = false;
					bdrag = -1;
				}
			}

			if (bdrag >= 0)
			{
				Rect r_cursor = new Rect (mp.x,mp.y,h*.1f,h*.1f);
				r_cursor.x -= r_cursor.width * .6f;
				r_cursor.y -= r_cursor.height * .6f;
				if (touching)
				{
					r_cursor = new Rect (touchpos.x,touchpos.y,h*.1f,h*.1f);
					r_cursor.x -= r_cursor.width * .6f;
					r_cursor.y -= r_cursor.height * .6f;
				}

				GUI.DrawTexture (r_cursor, t_emats [bdrag], ScaleMode.ScaleToFit);
			}
		}
		if (screen == 1)
		{
			Rect btrect = new Rect (0, h * .045f, 0, h * .1f);
			btrect.width = btrect.height * 3.43f;
			btrect.x = w - btrect.width - w*.025f;
			if (showlog)
			{
				GUIStyle lstyle = new GUIStyle (gstyle);
				lstyle.fontSize = Mathf.RoundToInt (gstyle.fontSize * .8f);

				Rect rpopup = new Rect (0,0, w*.7f,h*.9f);
				rpopup.center = new Vector2 (w * .5f, h * .5f);
				GUI.DrawTexture (rpopup, popup);

				Rect rtext = new Rect (rpopup);
				rtext.x += rpopup.width * .06f;
				rtext.y += h * .036f;
				GUI.Label (rtext, "Blocks", lstyle);

				Rect trialtext = new Rect (rtext);
				trialtext.x = rpopup.x + rpopup.width * .21f;
				GUI.Label (trialtext, "Trials", lstyle);

				Rect mtitletext = new Rect (trialtext);
				mtitletext.x += w * .09f;
				GUI.Label (mtitletext, "Materials", lstyle);

				Rect[] rlogmats = new Rect[4];
				for (int i = 0; i < rlogmats.Length; i++)
				{
					rlogmats [i] = new Rect (mtitletext);
					rlogmats [i].x += w * (i) * .09f;
					//GUI.Label (rlogmats [i], "T.Paper", lstyle);
				}

				Rect rresult = new Rect (rlogmats [3]);
				rresult.x += w * .09f;
				GUI.Label (rresult, "Results", lstyle);

				Rect rcolumn = new Rect (rpopup.x + rpopup.width*.175f,rpopup.y + h*.032f,h*.005f,h*.68f);
				GUI.DrawTexture (rcolumn, white);
				rcolumn.x += w * .1f;
				GUI.DrawTexture (rcolumn, white);
				rcolumn.x += w * .09f;
				rcolumn.yMin += h * .05f;
				GUI.DrawTexture (rcolumn, white);
				rcolumn.x += w * .09f;
				GUI.DrawTexture (rcolumn, white);
				rcolumn.x += w * .09f;
				GUI.DrawTexture (rcolumn, white);
				rcolumn.x += w * .09f;
				rcolumn.yMin -= h * .05f;
				GUI.DrawTexture (rcolumn, white);

				Rect rrow = new Rect (rpopup.x + rpopup.width*.05f,rpopup.y + h*.08f,rpopup.width*.9f,h*.005f);
				GUI.DrawTexture (rrow, white);
				rrow.xMin += w * .09f;
				rrow.y += h * .07f;
				GUI.DrawTexture (rrow, white);
				rrow.y += h * .07f;
				GUI.DrawTexture (rrow, white);
				rrow.y += h * .07f;
				rrow.xMin -= w * .09f;
				GUI.DrawTexture (rrow, white);
				rrow.xMin += w * .09f;
				rrow.y += h * .07f;
				GUI.DrawTexture (rrow, white);
				rrow.y += h * .07f;
				GUI.DrawTexture (rrow, white);
				rrow.xMin -= w * .09f;
				rrow.y += h * .07f;
				GUI.DrawTexture (rrow, white);
				rrow.xMin += w * .09f;
				rrow.y += h * .07f;
				GUI.DrawTexture (rrow, white);
				rrow.y += h * .07f;
				GUI.DrawTexture (rrow, white);
				rrow.xMin -= w * .09f;
				rrow.y += h * .07f;
				GUI.DrawTexture (rrow, white);

				float yrow = rresult.y + h * .07f;
				rtext.y = yrow;
				GUI.Label (rtext, "1 Meter", lstyle);

				trialtext.y = yrow;
				GUI.Label (trialtext, "Trial 1", lstyle);
				int b = 0;
				for (int i = 0; i < 4; i++)
				{
					rlogmats [i].y = yrow;//
					if (b1_info [b] != null)
					{
						if (b1_info [b].lineup [i] >= 0 && b1_info[b].show)
						{
							GUI.Label (rlogmats [i], slmat [b1_info [b].lineup [i]], lstyle);
						}
						else if(b1_info[b].show)
						{
							GUI.Label (rlogmats [i], "-", lstyle);
						}
					}
				}

				rresult.y = yrow;
				if (b1_info [b] != null)
				{
					if (b1_info [b].show)
					{
						if (b1_info [b].success)
						{
							GUI.Label (rresult, "Success", lstyle);
						}
						else
						{
							GUI.Label (rresult, "Fail", lstyle);
						}
					}
				}

				yrow += h * .07f;
				trialtext.y = yrow;
				GUI.Label (trialtext, "Trial 2", lstyle);
				b = 1;
				for (int i = 0; i < 4; i++)
				{
					rlogmats [i].y = yrow;
					if (b1_info [b] != null)
					{
						if (b1_info [b].lineup [i] >= 0 && b1_info[b].show)
						{
							GUI.Label (rlogmats [i], slmat [b1_info [b].lineup [i]], lstyle);
						}
						else if(b1_info[b].show)
						{
							GUI.Label (rlogmats [i], "-", lstyle);
						}
					}
				}

				rresult.y = yrow;
				if (b1_info [b] != null)
				{
					if (b1_info [b].show)
					{
						if (b1_info [b].success)
						{
							GUI.Label (rresult, "Success", lstyle);
						}
						else
						{
							GUI.Label (rresult, "Fail", lstyle);
						}
					}
				}

				yrow += h * .07f;
				trialtext.y = yrow;
				GUI.Label (trialtext, "Trial 3", lstyle);
				b = 2;
				for (int i = 0; i < 4; i++)
				{
					rlogmats [i].y = yrow;
					if (b1_info [b] != null)
					{
						if (b1_info [b].lineup [i] >= 0 && b1_info[b].show)
						{
							GUI.Label (rlogmats [i], slmat [b1_info [b].lineup [i]], lstyle);
						}
						else if(b1_info[b].show)
						{
							GUI.Label (rlogmats [i], "-", lstyle);
						}
					}
				}

				rresult.y = yrow;
				if (b1_info [b] != null)
				{
					if (b1_info [b].show)
					{
						if (b1_info [b].success)
						{
							GUI.Label (rresult, "Success", lstyle);
						}
						else
						{
							GUI.Label (rresult, "Fail", lstyle);
						}
					}
				}

				yrow += h * .07f;
				rtext.y = yrow;
				GUI.Label (rtext, "3 Meter", lstyle);

				trialtext.y = yrow;
				GUI.Label (trialtext, "Trial 1", lstyle);
				b = 0;
				for (int i = 0; i < 4; i++)
				{
					rlogmats [i].y = yrow;//
					if (b2_info [b] != null)
					{
						if (b2_info [b].lineup [i] >= 0 && b2_info [b].show)
						{
							GUI.Label (rlogmats [i], slmat [b2_info [b].lineup [i]], lstyle);
						}
						else if(b2_info[b].show)
						{
							GUI.Label (rlogmats [i], "-", lstyle);
						}
					}
				}

				rresult.y = yrow;
				if (b2_info [b] != null)
				{
					if (b2_info [b].show)
					{
						if (b2_info [b].success)
						{
							GUI.Label (rresult, "Success", lstyle);
						}
						else
						{
							GUI.Label (rresult, "Fail", lstyle);
						}
					}
				}

				yrow += h * .07f;
				trialtext.y = yrow;
				GUI.Label (trialtext, "Trial 2", lstyle);
				b = 1;
				for (int i = 0; i < 4; i++)
				{
					rlogmats [i].y = yrow;
					if (b2_info [b] != null)
					{
						if (b2_info [b].lineup [i] >= 0 && b2_info[b].show)
						{
							GUI.Label (rlogmats [i], slmat [b2_info [b].lineup [i]], lstyle);
						}
						else if(b2_info[b].show)
						{
							GUI.Label (rlogmats [i], "-", lstyle);
						}
					}
				}

				rresult.y = yrow;
				if (b2_info [b] != null)
				{
					if (b2_info [b].show)
					{
						if (b2_info [b].success)
						{
							GUI.Label (rresult, "Success", lstyle);
						}
						else
						{
							GUI.Label (rresult, "Fail", lstyle);
						}
					}
				}

				yrow += h * .07f;
				trialtext.y = yrow;
				GUI.Label (trialtext, "Trial 3", lstyle);
				b = 2;
				for (int i = 0; i < 4; i++)
				{
					rlogmats [i].y = yrow;
					if (b2_info [b] != null)
					{
						if (b2_info [b].lineup [i] >= 0 && b2_info[b].show)
						{
							GUI.Label (rlogmats [i], slmat [b2_info [b].lineup [i]], lstyle);
						}
						else if(b2_info[b].show)
						{
							GUI.Label (rlogmats [i], "-", lstyle);
						}
					}
				}

				rresult.y = yrow;
				if (b2_info [b] != null)
				{
					if (b2_info [b].show)
					{
						if (b2_info [b].success)
						{
							GUI.Label (rresult, "Success", lstyle);
						}
						else
						{
							GUI.Label (rresult, "Fail", lstyle);
						}
					}
				}

				yrow += h * .07f;
				rtext.y = yrow;
				GUI.Label (rtext, "5 Meter", lstyle);

				trialtext.y = yrow;
				GUI.Label (trialtext, "Trial 1", lstyle);
				b = 0;
				for (int i = 0; i < 4; i++)
				{
					rlogmats [i].y = yrow;//
					if (b3_info [b] != null)
					{
						if (b3_info [b].lineup [i] >= 0 && b3_info[b].show)
						{
							GUI.Label (rlogmats [i], slmat [b3_info [b].lineup [i]], lstyle);
						}
						else if(b3_info[b].show)
						{
							GUI.Label (rlogmats [i], "-", lstyle);
						}
					}
				}

				rresult.y = yrow;
				if (b3_info [b] != null)
				{
					if (b3_info [b].show)
					{
						if (b3_info [b].success)
						{
							GUI.Label (rresult, "Success", lstyle);
						}
						else
						{
							GUI.Label (rresult, "Fail", lstyle);
						}
					}
				}

				yrow += h * .07f;
				trialtext.y = yrow;
				GUI.Label (trialtext, "Trial 2", lstyle);
				b = 1;
				for (int i = 0; i < 4; i++)
				{
					rlogmats [i].y = yrow;
					if (b3_info [b] != null)
					{
						if (b3_info [b].lineup [i] >= 0 && b3_info[b].show)
						{
							GUI.Label (rlogmats [i], slmat [b3_info [b].lineup [i]], lstyle);
						}
						else if(b3_info[b].show)
						{
							GUI.Label (rlogmats [i], "-", lstyle);
						}
					}
				}

				rresult.y = yrow;
				if (b3_info [b] != null)
				{
					if (b3_info [b].show)
					{
						if (b3_info [b].success)
						{
							GUI.Label (rresult, "Success", lstyle);
						}
						else
						{
							GUI.Label (rresult, "Fail", lstyle);
						}
					}
				}

				yrow += h * .07f;
				trialtext.y = yrow;
				GUI.Label (trialtext, "Trial 3", lstyle);
				b = 2;
				for (int i = 0; i < 4; i++)
				{
					rlogmats [i].y = yrow;
					if (b3_info [b] != null)
					{
						if (b3_info [b].lineup [i] >= 0 && b3_info[b].show)
						{
							GUI.Label (rlogmats [i], slmat [b3_info [b].lineup [i]], lstyle);
						}
						else if(b3_info[b].show)
						{
							GUI.Label (rlogmats [i], "-", lstyle);
						}
					}
				}

				rresult.y = yrow;
				if (b3_info [b] != null)
				{
					if (b3_info [b].show)
					{
						if (b3_info [b].success)
						{
							GUI.Label (rresult, "Success", lstyle);
						}
						else
						{
							GUI.Label (rresult, "Fail", lstyle);
						}
					}
				}

				Rect rlcontinue = new Rect (0,0,h*.343f,h*.1f);
				rlcontinue.center = new Vector2 (w * .5f, h * .5f);
				rlcontinue.y = rrow.yMax + h * .03f;
				GUI.DrawTexture (rlcontinue, tcontinue);
				if(GUI.Button(rlcontinue,"",gstyle) || (taptimer>0 && rlcontinue.Contains(tappos)))
				{
					taptimer = 0;
					showlog = false;
				}
			}
			else if (!testing)
			{
				Rect rtitle = new Rect (w*.5f, h*.04f, 0, 0);
				GUIStyle tstyle = new GUIStyle (gstyle);
				tstyle.fontSize = Mathf.RoundToInt (h*.065f);
				tstyle.alignment = TextAnchor.MiddleCenter;
				GUI.Label (rtitle, sblock[block], tstyle);
				GUI.DrawTexture (btrect, t_drop);
				if (GUI.Button (btrect, "", gstyle) || (taptimer>0 && btrect.Contains(tappos)))
				{
					taptimer = 0;
					currentChar = (GameObject)Instantiate (characters [block], chpos [block], Quaternion.identity);
					testing = true;
				}
				btrect.y += btrect.height + h * .025f;
				GUI.DrawTexture (btrect, t_rewrap);
				if (GUI.Button (btrect, "", gstyle) || (taptimer>0 && btrect.Contains(tappos)))
				{
					taptimer = 0;
					screen = 0;
				}
				btrect.y += btrect.height + h * .025f;
				GUI.DrawTexture (btrect, t_log);
				if (GUI.Button (btrect, "", gstyle) || (taptimer>0 && btrect.Contains(tappos)))
				{
					taptimer = 0;
					showlog = true;
				}
				btrect.y += btrect.height + h * .025f;
				if (showprint)
				{
					GUI.DrawTexture (btrect, t_print);
					if (GUI.Button (btrect, "", gstyle) || (taptimer>0 && btrect.Contains(tappos)))
					{
						taptimer = 0;
						Print ();
					}
				}
			}
			else if (showresults)
			{
				Rect rpopup = new Rect (0,0, w*.5f,h*.6f);
				rpopup.center = new Vector2 (w * .5f, h * .5f);
				GUI.DrawTexture (rpopup, popup);

				Rect rptext = new Rect (rpopup);
				rptext.xMin += h * .04f;
				rptext.xMax -= h * .035f;
				rptext.yMin += h * .04f;

				if (broken)
					GUI.Label (rptext, "Oop! Looks like the egg has cracked. Try rewrapping your egg to make it stronger.", gstyle);
				else if (block != 2)
					GUI.Label (rptext, "Success! The egg didn't break. The next block will be harder. You may try testing the next block now or rewrap your egg first to reinforce it. Keep it up!", gstyle);
				else
					GUI.Label (rptext, "Congratulations, you beat the final block! The print option will now be available to you. Hit print and scroll down the page, then copy it into your journal. You may continue rewrapping and testing for fun.", gstyle);

				Rect rcontinue = new Rect (0,0,w*.2f,h*.2f);
				rcontinue.center = new Vector2 (w * .5f, h * .5f);
				rcontinue.y = rpopup.yMax - rcontinue.height - h * .035f;
				GUI.DrawTexture (rcontinue, tcontinue, ScaleMode.ScaleToFit);
				if (GUI.Button (rcontinue, "", gstyle) || (taptimer>0 && rcontinue.Contains(tappos)))
				{
					taptimer = 0;
					Destroy (currentChar);
					Destroy (currentEgg);
					testing = false;
					showresults = false;
					if (!broken)
					{
						block++;
						if (block > 2)
						{
							block = 2;
							showprint = true;
						}
					}
				}
			}
		}

	}

	public void EggCheck()
	{
		broken = true;
		defense = 0;
		int box = 0;
		float brating = 0;
		int tins = 0;
		int tps = 0;
		for (int i = 3; i > -1; i--)
		{
			if (lineup [i] == 2)
			{
				box = 1;
				defense = 0;
				brating = -1.5f;
			}
			else if (lineup [i] == 3)
			{
				box = 2;
				defense = 0;
				brating = 0;
			}
			else if(lineup[i] != -1)
			{
				if (box == 1)
					brating = 1.5f;
				else if (box == 2)
					brating = 2f;

				if (lineup [i] == 0)
				{
					defense += 3f;
					if (box == 1)
					{
						defense += 1.5f;
					}
					if (box == 2)
					{
						defense += 2f;
					}
					if (tins > 0)
					{
						defense += 2 * tins;
						tins = 0;
					}
					if (tps > 0)
					{
						defense += .75f * tps;
						tps = 0;
					}
				}
				if (lineup [i] == 1)
				{
					defense += 1f;
					tins++;
				}
				if (lineup [i] == 4)
				{
					defense += 1.5f;
					tps++;
				}
				if (lineup [i] == 5)
				{
					defense += 2.5f;
					if (box == 1)
					{
						defense += 1.5f;
					}
					if (box == 2)
					{
						defense += 2f;
					}
					if (tins > 0)
					{
						defense += 2 * tins;
						tins = 0;
					}
					if (tps > 0)
					{
						defense += .75f * tps;
						tps = 0;
					}
				}
			}
		}//end for
		defense += brating;

		if (defense < impact [block])
		{
			broken = true;
		}
		else
			broken = false;

		if (block == 0)
		{
			if (tnum1 < 3)
			{
				for (int i = 0; i < 4; i++)
				{
					b1_info [tnum1].lineup [i] = lineup [i];
				}
				b1_info [tnum1].show = true;
				b1_info [tnum1].success = !broken;
			}
			else
			{
				for (int i = 0; i < 4; i++)
				{
					b1_info [0].lineup [i] = b1_info[1].lineup[i];
					b1_info [1].lineup [i] = b1_info [2].lineup [i];
				}
				b1_info [0].success = b1_info [1].success;
				b1_info [1].success = b1_info [2].success;

				for (int i = 0; i < 4; i++)
				{
					if (tnum1 < 3)
					{
						b1_info [tnum1].lineup [i] = lineup [i];
					}
					else
					{
						b1_info [tnum1-1].lineup [i] = lineup [i];
					}
				}
				b1_info [tnum1-1].show = true;
				b1_info [tnum1-1].success = !broken;
			}
			if(tnum1<3)
				tnum1++;
		}
		else if (block == 1)
		{
			if (tnum2 < 3)
			{
				for (int i = 0; i < 4; i++)
				{
					b2_info [tnum2].lineup [i] = lineup [i];
				}
				b2_info [tnum2].show = true;
				b2_info [tnum2].success = !broken;
			}
			else
			{
				for (int i = 0; i < 4; i++)
				{
					b2_info [0].lineup [i] = b2_info [1].lineup[i];
					b2_info [1].lineup [i] = b2_info [2].lineup [i];
				}
				b2_info [0].success = b2_info [1].success;
				b2_info [1].success = b2_info [2].success;

				for (int i = 0; i < 4; i++)
				{
					if(tnum2<3)
						b2_info [tnum2].lineup [i] = lineup [i];
					else
						b2_info [tnum2-1].lineup [i] = lineup [i];
				}
				b2_info [tnum2-1].show = true;
				b2_info [tnum2-1].success = !broken;
			}

			if(tnum2<3)
				tnum2++;
		}
		else if (block == 2 && !showprint)
		{
			if (tnum3 < 3)
			{
				for (int i = 0; i < 4; i++)
				{
					b3_info [tnum3].lineup [i] = lineup [i];
				}
				b3_info [tnum3].show = true;
				b3_info [tnum3].success = !broken;
			}
			else
			{
				for (int i = 0; i < 4; i++)
				{
					b3_info [0].lineup [i] = b3_info[1].lineup[i];
					b3_info [1].lineup [i] = b3_info [2].lineup [i];
				}
				b3_info [0].success = b3_info [1].success;
				b3_info [1].success = b3_info [2].success;

				for (int i = 0; i < 4; i++)
				{
					if(tnum3<3)
						b3_info [tnum3].lineup [i] = lineup [i];
					else
						b3_info [tnum3-1].lineup [i] = lineup [i];
				}
				b3_info [tnum3-1].show = true;
				b3_info [tnum3-1].success = !broken;
			}

			if(tnum3<3)
				tnum3++;
		}
	} //end function
}
