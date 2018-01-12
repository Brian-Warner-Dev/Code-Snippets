using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dropper : MonoBehaviour {
	public tarray[] mats = new tarray[6];
	public GameObject drip;
	public Walker master;
	//bool dropped = false;
	//Rigidbody2D rgb;
	//Vector2 velocity;
	float timer = -1;
	public bool broken;
	SpriteRenderer sr;

	[System.Serializable]
	public class tarray
	{
		public GameObject[] omats = new GameObject[4];
	}

	// Use this for initialization
	void Start () {
		sr = GetComponent<SpriteRenderer> ();
		Main m = Camera.main.GetComponent<Main> ();
		if (m.lineup [0] >= 0)
		{
			sr.enabled = false;
			if (m.boxtype >= 0)
			{
				mats [6].omats [m.boxtype].SetActive (true);
			}
			else if(m.lastMat>=0 && m.lastType >= 0)
			{
				mats [m.lastType].omats [m.lastMat].SetActive (true);
			}
		}
		//rgb = GetComponent<Rigidbody2D> ();
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		//print (velocity);
		timer = 1f;
		Camera.main.GetComponent<Main> ().currentEgg = gameObject;
		Camera.main.GetComponent<Main> ().EggCheck ();
		broken = Camera.main.GetComponent<Main> ().broken;

		if (broken)
		{
			//play egg drip;
			drip.SetActive(true);
		}
	}

	// Update is called once per frame
	void Update () {
		if (timer > 0)
		{
			timer -= Time.deltaTime;
			if (timer <= 0)
			{
				Camera.main.GetComponent<Main> ().showresults = true;
			}
		}
		if (master.drop)
		{
			transform.parent = null;
			GetComponent<Rigidbody2D> ().gravityScale = 1;
		}
	}
}
