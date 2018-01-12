using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//contains the main functionality of each level for a specimen: Explore, Identify, and Rebuild

public class Level : MonoBehaviour {
	public int leveltype;
	public Texture t_all;
	public Texture t_return;
	public Texture t_backdrop;
	public Texture t_inventory;
	public Texture t_leftarrow;
	public Texture t_rightarrow;
	public Texture t_view;
	public Texture t_system;
	public Texture t_infobox;
	public Texture t_select;
	public Texture t_button;
	public Texture t_menu;
	public Texture t_filledX;
	public Texture t_emptyX;
	public Texture offradio;
	public Texture onradio;
	public Font mfont;
	public string[] instructions = new string[3];
	public bool showInstructions = true;
	public Controller master;
	int h;
	int w;
	Vector3 mp;
	float offsetInventory = 1;
	GUIStyle gstyle;
	GUIStyle descriptionstyle;
	GUIStyle infostyle;
	GUIStyle closestyle;
	GUIStyle pstyle;
	GUIStyle costyle;
	public string[] stitle;
	public Title _title;
	public MainMenu menu;
	public int mistakes;
	public int answerSelection = -1;
	int strtIndex;
	bool holdingView;
	bool holdingSystem;
	float holdtime = .2f;
	float systemholdtime = .2f;
	int vindex = 0;
	int sysindex = 0;
	float scrollcooldown;
	float stopwatch;


	// Use this for initialization
	void Start () {
		mp = Input.mousePosition;
		mp.y = h - mp.y;

		gstyle = new GUIStyle ();
		gstyle.normal.textColor = Color.white;

		gstyle.alignment = TextAnchor.MiddleCenter;
		gstyle.font = mfont;

		descriptionstyle = new GUIStyle(gstyle);
		descriptionstyle.alignment = TextAnchor.UpperLeft;

		descriptionstyle.wordWrap = true;

		infostyle = new GUIStyle(gstyle);
		infostyle.normal.textColor = Color.black;
		infostyle.wordWrap = true;
		infostyle.alignment = TextAnchor.UpperLeft;

		closestyle = new GUIStyle(descriptionstyle);

		pstyle = new GUIStyle (gstyle);
		pstyle.alignment = TextAnchor.UpperLeft;

		costyle = new GUIStyle (infostyle);
		costyle.normal.textColor = Color.white;
		costyle.alignment = TextAnchor.MiddleCenter;
	}
	
	// Update is called once per frame
	void Update () {
		if (master.state == 5 && master.pstate == master.state)
		{
			stopwatch += Time.deltaTime;
		}
		else
		{
			stopwatch = 0;
		}
		if (holdingView)
		{
			if(holdtime>0)
				holdtime -= Time.deltaTime;
		}
		else
		{
			holdtime = .2f;
		}
		if (holdingSystem)
		{
			if (systemholdtime > 0)
				systemholdtime -= Time.deltaTime;
		}
		else
			systemholdtime = .2f;
		if (scrollcooldown > 0)
		{
			scrollcooldown -= Time.deltaTime;
		}
		if (!holdingView)
			vindex = 0;
		if (!holdingSystem)
			sysindex = 0;
		//on level start code
		if (master.pstate != master.state)
		{
			strtIndex = 0;
			master.pstate = master.state;
			if (master.state == 3)
			{
				master.answerPart = null;
			}
			else if (master.state == 4)
			{
				master.answerPart = null;
				mistakes = 0;
				answerSelection = 0;
			}
			else if (master.state == 5)
			{
				for (int i = 0; i < master.allparts.Count; i++)
				{
					master.inventory.Add (master.allparts [i]);
				}
				int r = Random.Range (0, master.inventory.Count);
				master.answerPart = master.inventory [r];
				mistakes = 0;
			}
		} //end state start check

		//show or hide inventory
		if (master.state == 5)
		{
			offsetInventory = Mathf.MoveTowards (offsetInventory, 0, Time.deltaTime * 10);
		}
		else if (master.MO.dragging)
		{
			if (master.MO.cpos.y > h * .8125f)
			{
				offsetInventory = Mathf.MoveTowards (offsetInventory, 0, Time.deltaTime * 10);
			}
			else
			{
				offsetInventory = Mathf.MoveTowards (offsetInventory, 1, Time.deltaTime * 10);
			}
		}
		else
		{
			if (mp.y > h * .8125f) 
			{
				offsetInventory = Mathf.MoveTowards (offsetInventory, 0, Time.deltaTime * 10);
			}
			else
			{
				offsetInventory = Mathf.MoveTowards (offsetInventory, 1, Time.deltaTime * 10);
			} //end show or hide inventory
		}
		if (master.state == 3 && master.dragpart != null)
		{
			if (master.MO.stopdrag)
			{
				if (master.MO.cpos.y > h * .8125f)
				{
					master.inventory.Add (master.dragpart);
				}
				master.dragpart = null;
			}
			else
			{
				if (!Input.GetMouseButton (0) && !master.MO.dragging)
				{
					if (mp.y > h * .8125f && !master.MO.tapped)
					{
						master.inventory.Add (master.dragpart);
					}
					master.dragpart = null;
				}
			}
		}
		//update completion status when finished and return to menu
		if (master.state == 3 || master.state == 4)
		{
			if (master.selectedpart != null)
			{
				if (master.inventory.Contains (master.selectedpart))
				{
					master.selectedpart = null;
				}
			}
			if (master.inventory.Count == master.allparts.Count)
			{
				if (_title.completion [_title.loadindex] < master.state - 2)
				{
					_title.completion [_title.loadindex] = master.state - 2;
				}
				master.state = 2;
			}
		}
		else if (master.state == 5)
		{
			if (master.inventory.Count == 0)
			{
				if (_title.completion [_title.loadindex] < master.state - 2)
					_title.completion [_title.loadindex] = master.state - 2;
				if (stopwatch < master.specimenTimes [_title.loadindex] || master.specimenTimes [_title.loadindex] == 0)
				{
					master.specimenTimes [_title.loadindex] = stopwatch;
					PlayerPrefs.SetFloat (master.specimenNames [_title.loadindex] + "Time", stopwatch);
				}
				master.state = 2;
				stopwatch = 0;
			}
			else
			{
				if (master.answerPart == null)
				{
					int r = Random.Range (0, master.inventory.Count);
					master.answerPart = master.inventory [r];
				}
				//mouse version
				if (Input.GetMouseButtonUp (0) && master.selectedpart != null && mp.y < h * .8125f)
				{
					if (master.hoverparts.Contains (master.selectedpart) && master.answerPart == master.selectedpart)
					{
						master.inventory.Remove (master.selectedpart);
						master.selectedpart = null;
						if (master.inventory.Count > 0)
						{
							int r = Random.Range (0, master.inventory.Count);
							master.answerPart = master.inventory [r];
						}
					}
					else if (master.hoverparts.Count > 0)
					{
						mistakes++;
						master.selectedpart = null;
						if (mistakes >= 6)
						{
							master.state = 2;
							menu.MoveToLevel1 = true;
						}
					}
				}
				//touch version
				if (master.MO.stopdrag && master.selectedpart != null && master.MO.cpos.y < h * .8125f && !master.MO.dragging)
				{
					if (master.hoverparts.Contains (master.selectedpart) && master.answerPart == master.selectedpart)
					{
						master.inventory.Remove (master.selectedpart);
						master.selectedpart = null;
						if (master.inventory.Count > 0)
						{
							int r = Random.Range (0, master.inventory.Count);
							master.answerPart = master.inventory [r];
						}
					}
					else if (master.hoverparts.Count > 0)
					{
						mistakes++;
						master.selectedpart = null;
						if (mistakes >= 6)
						{
							master.state = 2;
							menu.MoveToLevel1 = true;
						}
					}
				}

				// mouse version of get hover parts
				if (master.selectedpart != null && mp.y <= Screen.height * .8125f && Input.GetMouseButton (0))
				{
					Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
					RaycastHit[] hits = Physics.RaycastAll (ray);
					List<Part> p = new List<Part> ();
					for (int i = 0; i < hits.Length; i++)
					{
						Part temp = hits [i].transform.GetComponent<Part> ();
						if (temp != null)
						{
							if (master.inventory.Contains (temp) && !p.Contains (temp))
							{
								p.Add (temp);
							}
						}
						else
						{
							Paired tpair = hits [i].transform.GetComponent<Paired> ();
							if (tpair != null)
							{
								temp = tpair.pairedPart;
								if (master.inventory.Contains (temp) && !p.Contains (temp))
								{
									p.Add (temp);
								}
							}
						}
					}
					master.hoverparts = p;
				}
				//touch version
				else if (master.selectedpart != null && master.MO.cpos.y <= Screen.height * .8125f && master.MO.dragging)
				{
					Ray ray = Camera.main.ScreenPointToRay (new Vector3(master.MO.cpos.x,Screen.height - master.MO.cpos.y));
					RaycastHit[] hits = Physics.RaycastAll (ray);
					List<Part> p = new List<Part> ();
					for (int i = 0; i < hits.Length; i++)
					{
						Part temp = hits [i].transform.GetComponent<Part> ();
						if (temp != null)
						{
							if (master.inventory.Contains (temp) && !p.Contains (temp))
							{
								p.Add (temp);
							}
						}
						else
						{
							Paired tpair = hits [i].transform.GetComponent<Paired> ();
							if (tpair != null)
							{
								temp = tpair.pairedPart;
								if (master.inventory.Contains (temp) && !p.Contains (temp))
								{
									p.Add (temp);
								}
							}
						}
					}
					master.hoverparts = p;
				}
				else
				{
					master.hoverparts = new List<Part>();
				}

				if (!Input.GetMouseButton (0) && !master.MO.dragging)
				{
					master.selectedpart = null;
				}
			}
		} //end update state 5



	} // end update

	void OnGUI()
	{
		w = Screen.width;
		h = Screen.height;
		mp = Input.mousePosition;
		mp.y = h - mp.y;
		if (master.MO.dragging)
		{
			mp.y = -100000;
			mp.x = -100000;
		}
		gstyle.fontSize = ResizeFont (h * .08f);
		descriptionstyle.fontSize = Mathf.RoundToInt(h * .03f);
		infostyle.fontSize = Mathf.RoundToInt(h * .027f);
		closestyle.fontSize = Mathf.RoundToInt(h * .05f);
		pstyle.fontSize = gstyle.fontSize / 2;
		costyle.fontSize = Mathf.RoundToInt (h*.05f);

		//on state enter, skip drawing gui
		if (master.pstate != master.state)
			return;

		Rect r_backdrop = new Rect(h*.0125f,h*.0125f,.325f*w,h*.8f);

		//draw inventory
		Rect r_inventory = new Rect(h * .0125f, r_backdrop.yMax + h * .0125f, 0, 0);
		r_inventory.xMax = Screen.width - h * .0125f;
		r_inventory.yMax = Screen.height - h * .0125f;
		r_inventory.y += offsetInventory * r_inventory.height;
		if((mp.y>h*.8125f || (master.MO.dragging && master.MO.cpos.y > h*.8125f) || (master.state==5)) && !showInstructions)
			GUI.DrawTexture(r_inventory, t_inventory);

		//view button
		Rect r_view = new Rect(0,0,h*.125f,h*.125f);
		r_view.x = w - r_view.width - h * .0125f;
		r_view.y = h*.8125f - h * .0125f - r_view.height;
		if (Input.GetMouseButtonDown (0) && !holdingView && r_view.Contains(mp))
		{
			holdingView = true;
		}
		if (master.MO.dragging && !holdingView && r_view.Contains (master.MO.dpos))
		{
			holdingView = true;
		}

		if (GUI.Button (r_view, t_view, gstyle) || (master.MO.tapped && r_view.Contains(master.MO.tpos)))
		{
			master.MO.tapped = false;
			master.view++;
			strtIndex = 0;
			if (master.view >= master.maxViews)
			{
				master.view = 0;
			}
		}//end view button

		//create drop down view list while holding down the view button
		if (holdingView && holdtime<=0)
		{
			//draw the view list, with release triggering changing to a specific view
			//mousing over the top or bottom of the list should scroll it slowly
			Rect rvitem = new Rect(r_view);
			GUIStyle vstyle = new GUIStyle (gstyle);
			vstyle.alignment = TextAnchor.UpperCenter;
			vstyle.fontSize = Mathf.RoundToInt (h * .025f);
			rvitem.height = vstyle.CalcHeight (new GUIContent ("F"), w);
			rvitem.width *= 1.7f;
			rvitem.x = r_view.x - rvitem.width - h * .0125f;
			//rvitem.y -= rvitem.height * 2;
			int rit = vindex;
			for (int i = 0; i < 5; i++)
			{
				if (rit < master.sviews.Length)
				{
					GUI.color = new Color (.2f, .2f, .2f, .7f);
					if (rvitem.Contains (mp) || ((master.MO.dragging||master.MO.stopdrag) && rvitem.Contains(master.MO.cpos)))
					{
						GUI.color = new Color (.8f, .8f, .8f, .7f);
						if (!Input.GetMouseButton (0) && !master.MO.dragging)
						{
							strtIndex = 0;
							master.view = rit;

						}
						else if (master.MO.stopdrag)
						{
							strtIndex = 0;
							master.view = rit;
						}
						else if (i == 0 && scrollcooldown<=0)
						{
							if (vindex > 0)
							{
								vindex--;
								scrollcooldown = .3f;
							}
						}
						else if (i == 4 && scrollcooldown<=0)
						{
							if (vindex < master.sviews.Length - 5)
							{
								vindex++;
								scrollcooldown = .3f;
							}
						}
					}
					GUI.DrawTexture (rvitem, _title.white);
					GUI.color = Color.white;
					GUI.Label (rvitem, master.sviews [rit], vstyle);

					rvitem.y += rvitem.height + 1;

					rit++;
				}
			}
			if (!Input.GetMouseButton (0) && !master.MO.dragging)
			{
				holdingView = false;
			}
		}

		//system button
		Rect r_system = new Rect (r_view);
		r_system.y += -r_system.height - h * .0125f;
		if (Input.GetMouseButtonDown (0) && !holdingSystem && r_system.Contains(mp))
		{
			holdingSystem = true;
		}
		if (master.MO.dragging && !holdingSystem && r_system.Contains (master.MO.dpos))
		{
			holdingSystem = true;
		}

		if (GUI.Button (r_system, t_system, gstyle) || (master.MO.tapped && r_system.Contains(master.MO.tpos)))
		{
			master.MO.tapped = false;
			//switch system
			master.system++;
			if (master.system >= master.slist.Count)
			{
				master.system = -1;
			}
		} //end system button

		//all system button
		Rect r_all = new Rect (r_system);
		r_all.width *= .7f;
		r_all.height *= .7f;
		r_all.y += -r_all.height - h * .0125f;
		r_all.x = r_system.xMax - r_all.width;
		if(GUI.Button(r_all,t_all,gstyle) || (master.MO.tapped && r_all.Contains(master.MO.tpos)))
		{
			master.MO.tapped = false;
			master.system = -1;
		} //end all button
		//holdingsystem
		if (holdingSystem && systemholdtime<=0)
		{
			//draw the view list, with release triggering changing to a specific view
			//mousing over the top or bottom of the list should scroll it slowly
			Rect rvitem = new Rect(r_system);
			GUIStyle vstyle = new GUIStyle (gstyle);
			vstyle.alignment = TextAnchor.UpperCenter;
			vstyle.fontSize = Mathf.RoundToInt (h * .025f);
			rvitem.height = vstyle.CalcHeight (new GUIContent ("F"), w);
			rvitem.width *= 1.7f;
			rvitem.x = r_view.x - rvitem.width - h * .0125f;
			//rvitem.y -= rvitem.height * 2;
			int rit = sysindex;
			for (int i = 0; i < 5; i++)
			{
				if (rit < master.slist.Count)
				{
					GUI.color = new Color (.2f, .2f, .2f, .7f);
					if (rvitem.Contains (mp) || ((master.MO.dragging||master.MO.stopdrag) && rvitem.Contains(master.MO.cpos)))
					{
						GUI.color = new Color (.8f, .8f, .8f, .7f);
						if (!Input.GetMouseButton (0) && !master.MO.dragging)
						{
							strtIndex = 0;
							master.system = rit;

						}
						else if (master.MO.stopdrag)
						{
							strtIndex = 0;
							master.system = rit;
						}
						else if (i == 0 && scrollcooldown<=0)
						{
							if (sysindex > 0)
							{
								sysindex--;
								scrollcooldown = .3f;
							}
						}
						else if (i == 4 && scrollcooldown<=0)
						{
							if (sysindex < master.slist.Count - 5)
							{
								sysindex++;
								scrollcooldown = .3f;
							}
						}
					}
					GUI.DrawTexture (rvitem, _title.white);
					GUI.color = Color.white;
					GUI.Label (rvitem, master.slist [rit], vstyle);

					rvitem.y += rvitem.height + 1;

					rit++;
				}
			}
			if (!Input.GetMouseButton (0) && !master.MO.dragging)
			{
				holdingSystem = false;
			}
		}

		//draw info box
		Rect r_infobox = new Rect();
		if (master.selectedpart != null || master.answerPart!=null)
		{
			r_infobox = new Rect(0, 0, w * .35f, h * .35f);
			if (master.state == 4)
			{
				r_infobox = new Rect (0, 0, w * .5f, h * .65f);
			}
			r_infobox.x = w - r_infobox.width - h * .0125f;
			r_infobox.y = h * .0125f;
			GUI.color = new Color(1, 1, 1, .7f);

			if(master.state==3 || master.state==4)
				GUI.DrawTexture(r_infobox, t_infobox);
			GUI.color = Color.white;


			Rect r_textinfo = new Rect(r_infobox.x + h * .035f, r_infobox.y + h * .05f, r_infobox.width - .055f * h, r_infobox.height - .05f * h);
			if (master.state == 4)
			{
				r_textinfo = new Rect (r_infobox.x + h * .045f, r_infobox.y + h * .06f, r_infobox.width - .05f * h, r_infobox.height - .05f * h);
				r_textinfo.xMax = r_infobox.xMax - w * .01f;
				GUI.Label (r_textinfo, master.selectedpart.pname + ": Please choose the correct description below.", infostyle);
				r_textinfo.xMin += w * .01f;
				float width = r_textinfo.width;
				GUIContent gc = new GUIContent (master.selectedpart.pname + ": Please choose the correct description below.");
				float yoff = infostyle.CalcHeight (gc, width) + h * .01f;
				r_textinfo.y += yoff;
				for (int i = 0; i < 4; i++)
				{
					Rect r_radio = new Rect (r_textinfo.x - h * .03f, r_textinfo.y, h * .025f, h * .025f);
					if (answerSelection == i)
						GUI.DrawTexture (r_radio, onradio);
					else if (GUI.Button (r_radio, offradio, gstyle) || (master.MO.tapped && r_radio.Contains (master.MO.tpos)))
					{
						master.MO.tapped = false;
						answerSelection = i;
					}

					GUI.Label (r_textinfo, master.selectedpart.answers [i], infostyle);
					gc = new GUIContent (master.selectedpart.answers [i]);
					yoff = infostyle.CalcHeight (gc, width) + h * .02f;
					Rect r_selbox = new Rect (r_textinfo);
					r_selbox.height = yoff;
					if (GUI.Button (r_selbox, "", infostyle) || (master.MO.tapped && r_selbox.Contains(master.MO.tpos)))
					{
						master.MO.tapped = false;
						answerSelection = i;
					}
					r_textinfo.y += yoff;
				}

				if (answerSelection >= 0)
				{
					Rect r_confirm = new Rect (0, 0, h*.25f, h * .08f);
					r_confirm.y = r_infobox.yMax - r_confirm.height - h * .05f;
					r_confirm.x = r_infobox.x + r_infobox.width * .5f - r_confirm.width * .5f;
					GUI.DrawTexture (r_confirm, t_button);
					if (GUI.Button (r_confirm, "Confirm", costyle) || (master.MO.tapped && r_confirm.Contains(master.MO.tpos)))
					{
						master.MO.tapped = false;
						if (master.selectedpart.description == master.selectedpart.answers [answerSelection])
						{
							master.inventory.Add (master.selectedpart);
							master.selectedpart = null;
							answerSelection = -1;
						}
						else
						{
							master.selectedpart = null;
							answerSelection = -1;
							mistakes++;
							if (mistakes >= 3)
							{
								master.state = 1;
								menu.MoveToLevel1 = true;
							}
						}//end answer check
					}//end confirm button
				}//end answer selection >= 0
			} //end state 4 check
			else if (master.state == 3)
				GUI.Label (r_textinfo, master.selectedpart.pname + ": " + master.selectedpart.description, infostyle);
			else if (master.state == 5 && master.answerPart!=null)
			{
				r_infobox = new Rect(0, 0, w * .35f, h * .35f);
				r_infobox.x = w - r_infobox.width - h * .0125f;
				r_infobox.y = h * .0125f;
				GUI.color = new Color(1, 1, 1, .7f);

				GUI.DrawTexture(r_infobox, t_infobox);
				GUI.color = Color.white;
				GUI.Label (r_textinfo, master.answerPart.description, infostyle);
			}
			if (master.lns.enabled && master.selectedpart!=null)
			{
				Rect r_select = new Rect (0, 0, h * .05f, h * .05f);
				r_select.x = Camera.main.WorldToScreenPoint (master.selectedpart.pivot.transform.position).x - r_select.width * .5f;
				r_select.y = h - Camera.main.WorldToScreenPoint (master.selectedpart.pivot.transform.position).y - r_select.height * .5f;
				master.linevertex = new Vector3 (r_select.x + r_select.width * .5f, r_select.y + r_select.height * .5f, 0);
				if(master.state!=5)
					GUI.DrawTexture (r_select, t_select);
			}
		} //end infobox

		if (showInstructions)
		{
			GUI.DrawTexture (r_backdrop, t_backdrop);

			Rect r_title = new Rect (r_backdrop.x + r_backdrop.width * .5f, r_backdrop.y + r_backdrop.height * .08f, 0, 0);
			if(master.state>2)
				GUI.Label (r_title, stitle[master.state-3], gstyle);

			Rect r_description = new Rect (r_backdrop.x + h * .025f, r_backdrop.y + r_backdrop.height * .15f, 0, 0);
			r_description.xMax = r_backdrop.xMax - h * .025f;
			r_description.yMax = r_backdrop.yMax - h * .125f;
			if(master.state>2)
				GUI.Label (r_description, instructions[master.state-3], descriptionstyle);

			Rect r_closebutton = new Rect (0, r_backdrop.yMax - h * .1f - h * .025f, r_backdrop.width * .5f, h * .1f);
			r_closebutton.x = r_backdrop.x + r_backdrop.width * .5f - r_closebutton.width * .5f;
			GUI.DrawTexture (r_closebutton, t_button);
			if (GUI.Button (r_closebutton, "", gstyle) || (master.MO.tapped && r_closebutton.Contains(master.MO.tpos)))
			{
				master.MO.tapped = false;
				showInstructions = false;
			}

			Rect r_closetext = new Rect (r_closebutton.x + r_closebutton.width * .5f, r_closebutton.y + r_closebutton.height * .5f, 0, 0);

			closestyle.alignment = TextAnchor.MiddleCenter;
			closestyle.fontSize = Mathf.RoundToInt (h * .05f);
			GUI.Label (r_closetext, "Close", closestyle);
		} //end show instructions
		else
		{
			//show instructions button
			Rect r_menu = new Rect(h*.0125f,h*.0125f,h*.1f,h*.1f);
			if(GUI.Button(r_menu,t_menu,gstyle) || (master.MO.tapped && r_menu.Contains(master.MO.tpos)))
			{
				master.MO.tapped = false;
				showInstructions = true;
			}
			if (master.state == 5)
			{
				Rect r_time = new Rect (r_menu);
				r_time.y += r_time.height + h * .0125f;
				GUIStyle timestyle = new GUIStyle ();
				timestyle.normal.textColor = Color.white;
				timestyle.font = gstyle.font;
				timestyle.fontSize = Mathf.RoundToInt (h * .025f);
				timestyle.alignment = TextAnchor.UpperLeft;
				string btSec = string.Format ("{0}:{1:00}", (int)master.specimenTimes [_title.loadindex] / 60, (int)master.specimenTimes [_title.loadindex] % 60);
				string swSec = string.Format ("{0}:{1:00}", (int)stopwatch / 60, (int)stopwatch % 60);
				GUI.Label (r_time, "Best Time: " + btSec + "\nCurrent Time:" + swSec, timestyle);
			}

			//return to menu button
			Rect r_exit = new Rect(r_menu);
			r_exit.x += h * .0125f + r_exit.width;
			if(GUI.Button(r_exit,t_return,gstyle) || (master.MO.tapped && r_exit.Contains(master.MO.tpos)))
			{
				master.MO.tapped = false;
				master.state = 2;
			}

			Rect r_pcount = new Rect (r_exit.xMax + w*.02f,h*.0125f,0,0);
			GUI.Label (r_pcount, "Parts Stored: " + master.GetPartCount(master.view).ToString () + "/" + master.GetPartMax(master.view).ToString (), pstyle);

			Rect r_scount = new Rect (r_pcount);
			r_scount.x += w * .35f;

			if (master.system == -1)
			{
				GUI.Label (r_scount, "System: All", pstyle);
			}
			else
			{
				GUI.Label (r_scount, "System: " + master.slist [master.system], pstyle);
			}

			if (master.state == 4)
			{
				Rect r_x = new Rect (r_pcount.x, h * .08f, h * .04f, h * .04f);
				for (int i = 0; i < 3; i++)
				{
					if (mistakes > i)
						GUI.DrawTexture (r_x, t_filledX);
					else
						GUI.DrawTexture (r_x, t_emptyX);
					r_x.x += r_x.width + h * .05f;
				}
			}
			else if (master.state == 5)
			{
				Rect r_x = new Rect (r_pcount.x, h * .08f, h * .04f, h * .04f);
				for (int i = 0; i < 6; i++)
				{
					if (mistakes > i)
						GUI.DrawTexture (r_x, t_filledX);
					else
						GUI.DrawTexture (r_x, t_emptyX);
					r_x.x += r_x.width + h * .05f;
				}
			}
		} //end when-instructions-hidden UI elements

		if (master.dragpart != null && master.state != 5)
		{
			Rect r_picon = new Rect (0, 0, h * .05f, h * .05f);
			r_picon.x = mp.x - r_picon.width * .5f;
			r_picon.y = mp.y - r_picon.height * .5f;
			if (master.MO.dragging)
			{
				r_picon.width = h * .1f;
				r_picon.height = h * .1f;
				r_picon.x = master.MO.cpos.x - r_picon.width;
				r_picon.y = master.MO.cpos.y - r_picon.height;
			}
			GUI.DrawTexture (r_picon, master.dragpart.icon);
		} //end dragpart
		else if (master.state == 5 && master.selectedpart != null)
		{
			Rect r_picon = new Rect (0, 0, h * .05f, h * .05f);
			r_picon.x = mp.x - r_picon.width * .5f;
			r_picon.y = mp.y - r_picon.height * .5f;
			if (master.MO.dragging)
			{
				r_picon.width = h * .1f;
				r_picon.height = h * .1f;
				r_picon.x = master.MO.cpos.x - r_picon.width;
				r_picon.y = master.MO.cpos.y - r_picon.height;
			}
			GUI.DrawTexture (r_picon, master.selectedpart.icon);
		}

		if ((mp.y > h * .8125f || (master.MO.dragging && master.MO.cpos.y > h*.8125f) || master.state==5) && !showInstructions)
		{
			float iconoffset = 0;
			List<Part> cinventory = new List<Part> ();
			//first get a list of all the parts in this view that are in the inventory
			for (int i = 0; i < master.inventory.Count; i++)
			{
				if (master.inventory[i].type == master.view)
				{
					cinventory.Add (master.inventory [i]);
				}
			}//end inventory check
			Rect r_icon = new Rect(r_inventory.x + iconoffset + r_inventory.width * .01f, r_inventory.y + r_inventory.height * .35f + offsetInventory*r_inventory.height, h * .09f, h * .09f);
			if (GUI.Button (r_icon, t_leftarrow, gstyle) || (master.MO.tapped && r_icon.Contains(master.MO.tpos)))
			{
				master.MO.tapped = false;
				strtIndex--;
			}
			if (strtIndex > cinventory.Count - 15)
				strtIndex = 0;
			if (strtIndex < 0)
				strtIndex = 0;
			int endIndex = cinventory.Count;
			if (endIndex > strtIndex + 14)
				endIndex = strtIndex + 14;

			iconoffset += r_icon.width + h * .005f;
			for (int i = 0; i < endIndex; i++)
			{
				r_icon = new Rect(r_inventory.x + iconoffset + r_inventory.width * .01f, r_inventory.y + r_inventory.height * .35f + offsetInventory*r_inventory.height, h * .09f, h * .09f);
				GUI.DrawTexture(r_icon, cinventory[i].icon);
				if (r_icon.Contains(mp) && Input.GetMouseButtonDown(0) && master.state == 5)
				{
					master.selectedpart = cinventory [i];
				}
				if (r_icon.Contains (master.MO.dpos) && master.MO.startdrag && master.state == 5)
				{
					master.selectedpart = cinventory [i];
				}

				if (r_icon.Contains(mp) || (master.MO.dragging && r_icon.Contains(master.MO.cpos)))
				{
					Rect r_ptext = new Rect(r_icon.center.x + h * .005f, r_icon.y - h * .025f, 0, 0);
					GUI.Label(r_ptext, cinventory[i].pname, descriptionstyle);
				}

				iconoffset += r_icon.width + h * .005f;
			}
			r_icon = new Rect (r_inventory.xMax - r_inventory.width * .01f - r_icon.width, r_icon.y, r_icon.width, r_icon.height);
			if (GUI.Button (r_icon, t_rightarrow, gstyle) || (master.MO.tapped && r_icon.Contains(master.MO.tpos)))
			{
				master.MO.tapped = false;
				strtIndex--;
			}
		} //end mouse above check
	} //end gui

	//Resize font and return integer
	int ResizeFont(float f)
	{
		return Mathf.RoundToInt (f);
	} //end resize font

	//set gui color without changing alpha
	void SetColor(Color c)
	{
		c.a = GUI.color.a;
		GUI.color = c;
	} //end setcolor

	//set gui alpha without changing color
	void SetAlpha(float a)
	{
		Color b = GUI.color;
		b.a = a;
		GUI.color = b;
	} //end setalpha


}
