using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paired : MonoBehaviour {
	public Part pairedPart;
	public MeshCollider mc;
	Renderer r;
	// Use this for initialization
	void Start () {
		r = GetComponent<Renderer> ();
	}

	void OnMouseDown()
	{
		pairedPart.OnMouseDown ();
	}
	void OnMouseOver()
	{
		pairedPart.OnMouseOver ();
	}

	// Update is called once per frame
	void LateUpdate () {
		if (pairedPart != null)
		{
			r.enabled = pairedPart.GetComponent<Renderer> ().enabled;
			if (mc != null && pairedPart.mc != null)
			{
				mc.enabled = pairedPart.mc.enabled;

			}	
			gameObject.layer = pairedPart.gameObject.layer;
		}
	}
}
