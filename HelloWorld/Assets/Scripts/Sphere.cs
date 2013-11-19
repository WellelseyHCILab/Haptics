using UnityEngine;
using System.Collections;

public class Sphere : MonoBehaviour {

	Color c;

	// Use this for initialization
	void Start () {
		c = Color.yellow;
		this.gameObject.renderer.material.color = c;	
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	Vector3 getVelocity() {
		Vector3 velocity = this.gameObject.rigidbody.velocity;
		return velocity;	
	}
	
	Color getColor () {
	    return c;
	}
}
