using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//attached to each mesh object that is an interactable body part.
//if the body part contains multiple mesh objects, use "Paired" on the other objects and reference this object. 

public class Part : MonoBehaviour {
	public string pname;
	public string[] systems = new string[0];
	public string description;
	public GameObject pivot;
	public Texture icon;
	public int type;
	public string[] answers = new string[4];
	public MeshCollider mc;
	public Controller master;
	public int highlighting;
	Renderer r;
	public MatFlasher mf;
	// Use this for initialization
	void Start () {
		r = GetComponent<Renderer> ();
		mf = GetComponent<MatFlasher> ();
	}

	public void OnMouseOver()
	{
		highlighting = 2;
	}
	public void OnMouseDown()
	{
		bool istouch = false;
		if (master.MO.startdrag || master.MO.tapped)
			istouch = true;
		Vector3 mp = Input.mousePosition;
		mp.y = Screen.height - mp.y;
		if (master.MO.tapped)
		{
			mp = master.MO.tpos;
		}
		else if (master.MO.startdrag)
		{
			mp = master.MO.dpos;
		}
		if (mp.y > Screen.height * .8125f)
			return;
		if (master.view == type && master.state == 3 &&  !master.level.showInstructions)
		{
			if (master.system >= 0)
			{
				if (MatchSystem())
				{
					if (master.selectedpart != this)
						master.selectedpart = this;
					else if (!istouch)
						master.dragpart = this;
					else if (istouch && master.MO.startdrag)
					{
						master.dragpart = this;
					}
				}
			}
			else
			{
				if (master.selectedpart != this)
					master.selectedpart = this;
				else if (!istouch)
					master.dragpart = this;
				else if (istouch && master.MO.startdrag)
				{
					master.dragpart = this;
					master.MO.status = mp.ToString ();
				}
			}
		}
		else if (master.view == type && master.state == 4 && !master.level.showInstructions && master.selectedpart==null)
		{
			if (master.system >= 0)
			{
				if (MatchSystem())
					master.selectedpart = this;
			}
			else
			{
				master.selectedpart = this;
			}
		}
	}

	// Update is called once per frame
	void Update () {
		if (mf.isHighlighted)
		{
			gameObject.layer = 1;
		}
		else
		{
			gameObject.layer = 0;
		}
		if (master.state != master.pstate && master.state == 4)
		{
			RandomizeAnswers ();
		}
		if (highlighting > 0)
		{
			highlighting--;
		}
		if (master.view == type)
		{
			if (master.inventory == null)
			{
				master.inventory = new List<Part> ();
			}
			if (master.state == 5)
			{
				if (!master.inventory.Contains (this))
					mc.enabled = false;
				else
					mc.enabled = true;
			}
			else
			{
				if (master.inventory.Contains (this))
					mc.enabled = false;
				else if (!showSystem ())
					mc.enabled = false;
				else if (showSystem () && !master.inventory.Contains(this))
					mc.enabled = true;
			}
		}
		else
			mc.enabled = false;


		if (showSystem () && !master.inventory.Contains (this))
		{
			r.enabled = true;
		}
		else
		{
			r.enabled = false;
		}
		if (master.state == 5)
		{
			if (!master.inventory.Contains (this) && showSystem ())
				r.enabled = true;
			else if (!master.inventory.Contains (this) && !showSystem ())
			{
				r.enabled = false;
			}
			else if (master.hoverparts.Contains (this))
				r.enabled = true;
			else
				r.enabled = false;
		}
	}
	bool showSystem()
	{
		if (master.system < 0)
			return true;
		if (MatchSystem())
			return true;
		else
			return false;
	}
	public void Activate(bool b)
	{
		GetComponent<Renderer> ().enabled = b;
		mc.enabled = b;
	}
	void RandomizeAnswers()
	{
		answers = new string[4];
		for (int i = 0; i < 4; i++)
		{
			answers [i] = "";
		}
		int r = Random.Range (0, 4);
		answers [r] = description;

		if (master.allparts.Count < 4)
		{
			for (int i = 0; i < 4; i++)
			{
				r = Random.Range (0, master.allparts.Count);
				answers [i] = master.allparts [r].description;
			}
		}
		else
		{
			List<Part> lp = new List<Part> ();
			for (int j = 0; j < master.allparts.Count; j++)
			{
				lp.Add(master.allparts [j]);
			}
			lp.Remove (this);
			for (int i = 0; i < 4; i++)
			{
				if (answers [i] == description)
					continue;
				r = Random.Range (0, lp.Count);
				answers [i] = lp [r].description;
				lp.Remove (lp [r]);
			}
		}
		
		
	}

	bool MatchSystem ()
	{
		bool symatch = false;
		for (int i = 0; i < systems.Length; i++)
		{
			if (systems [i] == master.slist [master.system])
			{
				symatch = true;
			}
		}

		return symatch;
	}
}
