using UnityEngine;
using System.Collections;

//Purpose:
//Used for Hali's ball projectiles for her up smash

public class ballProjectile : MonoBehaviour {
	public float speed; //projectile speed
	public float distance; //distance to travel before destroying self
	float covered = 0; //distance traveled so far
	
	// Update is called once per frame
	void FixedUpdate () {
		transform.Translate(transform.up * speed); //update movement
		covered += speed; //update distance covered
		if(covered>=this.distance) //destroy when reaching past distance
		{
			GameObject.Destroy(gameObject);
		}
		if(!particleSystem.isPlaying) //play particle system AFTER instantiating and being moved into position
			particleSystem.Play();
	}
}
