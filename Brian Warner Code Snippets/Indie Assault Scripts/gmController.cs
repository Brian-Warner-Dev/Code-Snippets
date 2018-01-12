using UnityEngine;
using System.Collections;

public class gmController : MonoBehaviour 
{
	public int[] pdamage;
	public int[] psmash;
	public Transform[] characters;

	public Transform midtrack;

	public string[] spChar;
	bool resetDamage;
	public int maxDeathTimer;
	public int[] dtimer;
	gameHUD gh;
	public bool start = true;
	public bool end = false;
	public string[] sdisplay = new string[4];
	string cdisplay;
	string[] sSounds = new string[4];
	float fader = 0;

	public GUIStyle gstyle;

	void Start()
	{
		gh = GetComponent<gameHUD>();

		sdisplay[0] = "3";
		sdisplay[1] = "2";
		sdisplay[2] = "1";
		sdisplay[3] = "FIGHT!";

		sSounds[0] = "Three Sound";
		sSounds[1] = "Two Sound";
		sSounds[2] = "One Sound";
		sSounds[3] = "Fight Sound";

	}

	void FixedUpdate()
	{
		if(Input.GetButton("Start"))
		{
			Application.LoadLevel(0);
		}
		if(start)
		{
			StartCoroutine(startCR());
			start = false;
		}
		checkEnd();
		if(end)
		{
			StartCoroutine(checkRestart());
			StartCoroutine(endCR());
		}

		if(fader>0)
			fader = Mathf.MoveTowards(fader,0,Time.fixedDeltaTime);

		for(int i = 0; i<characters.Length;i++)
		{
			if(characters[i]!=null)
			{
				characters[i].BroadcastMessage("applyDamage",pdamage[i],SendMessageOptions.DontRequireReceiver);
				characters[i].BroadcastMessage("setSmash",psmash[i],SendMessageOptions.DontRequireReceiver);
				characters[i].BroadcastMessage("setPlayer",i+1,SendMessageOptions.DontRequireReceiver);

				if(i==0)
				{
					midtrack.BroadcastMessage("setTarget1",characters[i]);
				}
				else if(i==1)
				{
					midtrack.BroadcastMessage("setTarget2",characters[i]);
				}
			}
		}
		BroadcastMessage("setPforms",characters,SendMessageOptions.DontRequireReceiver);
		BroadcastMessage("setSmash",psmash,SendMessageOptions.DontRequireReceiver);
		for(int i = 0; i<4; i++)
		{
			if(dtimer[i]>0)
			{
				dtimer[i]--;
				if(dtimer[i]==0)
				{
					if(spChar[i]=="titus")
					{
						if(gh.trackStock)
						{
							if(gh.stock[i]>0)
							{
								BroadcastMessage("spawnTitus",i+1);
								BroadcastMessage("reSpawned",i+1);
							}
						}
						else
						{
							BroadcastMessage("spawnTitus",i+1);
							BroadcastMessage("reSpawned",i+1);
						}
					}
					else if(spChar[i]=="hali")
					{
						if(gh.trackStock)
						{
							if(gh.stock[i]>0)
							{
								BroadcastMessage("spawnHali",i+1);
								BroadcastMessage("reSpawned",i+1);
							}
						}
						else
						{
							BroadcastMessage("spawnHali",i+1);
							BroadcastMessage("reSpawned",i+1);
						}
					}
				}
			}
		}
	}
	void setDamage(Vector2 dmUpper)
	{
		pdamage[(int)dmUpper.x-1] += (int)dmUpper.y;
		if(pdamage[(int)dmUpper.x-1]>999)
			pdamage[(int)dmUpper.x-1] = 999;
		BroadcastMessage("hit",new Vector2(dmUpper.x,dmUpper.y));
	}
	void KOed(int player)
	{
		pdamage[player-1] = 0;
		psmash[player-1] = 0;
		dtimer[player-1] = maxDeathTimer;
	}
	void addSmash(Vector2 smasher)
	{
		int s = (int)smasher.x-1;
		int amount = (int)smasher.y;
		psmash[s]+= amount;
		if(psmash[s]>100)
		{
			psmash[s] = 100;
		}
	}
	void resetSmash(int num)
	{
		psmash[num-1] = 0;
	}

	IEnumerator startCR()
	{
		//GAME START

		//Disable Input
		for(int i = 0; i<characters.Length; i++)
		{
			if(characters[i]!=null)
				characters[i].BroadcastMessage("canInput",false,SendMessageOptions.DontRequireReceiver);
		}

		yield return new WaitForSeconds(.5f);

		//Display 3, 2, 1, FIGHT!
		for(int i = 0; i<sdisplay.Length; i++)
		{
			fader = 1f;
			cdisplay = sdisplay[i];
			GameObject.Instantiate(Resources.Load(sSounds[i]));
			while(fader>0)
			{
				yield return null;
			}
		}

		//Enable Input
		for(int i = 0; i<characters.Length; i++)
		{
			if(characters[i]!=null)
				characters[i].BroadcastMessage("canInput",true,SendMessageOptions.DontRequireReceiver);
		}

		yield break;
	}

	IEnumerator endCR()
	{
		//Disable Input
		for(int i = 0; i<characters.Length; i++)
		{
			if(characters[i]!=null)
				characters[i].BroadcastMessage("canInput",false,SendMessageOptions.DontRequireReceiver);
		}

		cdisplay = "BREAK!";
		GameObject.Instantiate(Resources.Load("Break Sound"));
		GetComponent<AudioSource>().enabled = false;
		Time.timeScale = 0;

		while(end)
		{
			fader = 1;
			yield return null;
		}

		yield break;
	}

	void OnGUI()
	{
		/*
		float hratio;
		float vratio;
		
		hratio = Screen.width/1920f;
		vratio = Screen.height/1080f;
		*/
		Rect dpos = new Rect();
		
		dpos.x = Screen.width/2;
		dpos.y = Screen.height/2;

		gstyle.fontSize = (int)(Screen.height*.3f);

		GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b,fader);

		GUI.Label(dpos,this.cdisplay,this.gstyle);
	}

	void checkEnd()
	{
		if(gh!=null)
		{
			if(gh.trackStock)
			{
				int alivePlayers = 0;
				for(int i = 0; i<gh.active.Length; i++)
				{
					if(gh.active[i])
					{
						if(gh.stock[i]>0)
							alivePlayers++;
					}
				}
				if(alivePlayers<=1)
					end = true;
			}
		}
	}
	IEnumerator checkRestart()
	{
		int resTimer = 0;
		while(true)
		{
			if(Time.timeScale==0)
			{
				resTimer++;
				if(resTimer>400)
				{
					Time.timeScale = 1;
					Application.LoadLevel(1);
				}
			}
			if(Input.GetButton("Start"))
			{
				Time.timeScale = 1;
				Application.LoadLevel(1);
			}
			yield return null;
		}
	}
}
