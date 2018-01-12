using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker : MonoBehaviour {
	public float speed = 1f;
	Animator anim;
	AnimatorStateInfo cstate;
	Vector3 pos;
	public float xgoal;
	float cplaytime;
	public bool drop = false;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		cstate = anim.GetCurrentAnimatorStateInfo (0);
		pos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		cstate = anim.GetCurrentAnimatorStateInfo (0);
		cplaytime = cstate.normalizedTime % 1;
		if (cstate.IsName ("Walk"))
		{
			pos.x += Time.deltaTime * speed;
			if (pos.x >= xgoal)
			{
				pos.x = xgoal;
				anim.Play ("Drop", 0, 0);
			}
		}

		if (cstate.IsName ("Drop"))
		{
			if (cplaytime >= .5f)
			{
				drop = true;
			}
		}

		transform.position = pos;
	}
}
