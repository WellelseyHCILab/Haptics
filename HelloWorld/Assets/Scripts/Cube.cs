using UnityEngine;
using System.Collections;

public class Cube : MonoBehaviour {

	Color c;
	
	// Use this for initialization
	void Start () {
		c = Color.magenta;
		this.gameObject.renderer.material.color = c;	
	}
	
	// Update is called once per frame
	void Update () {	
	}
	
	Color getColor () {
	    return c;
	}
}
