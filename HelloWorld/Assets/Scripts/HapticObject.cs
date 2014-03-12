using UnityEngine;
using System.Collections;

public class HapticObject : MonoBehaviour {
	
    public GameObject stylus;
    public GameObject core;
    public GameObject _oldHoverObject;
    public GameObject sphere;
    public GameObject cube;
    public bool isRolling;

	// Use this for initialization
	void Start () {
		stylus = GameObject.Find ("ZSStylus");
		core = GameObject.Find ("ZSCore");
		sphere = GameObject.Find ("Sphere");
		cube = GameObject.Find ("Cube");
		isRolling = false;
		_oldHoverObject = null;
		//_core = GameObject.Find("ZSCore").GetComponent<ZSCore>();
        //_stylusSelector = GameObject.Find("ZSStylusSelector").GetComponent<ZSStylusSelector>();
	}
	
	// Update is called once per frame
	void Update () {
		roll ();
		hover ();
/*
		if (this.gameObject.transform.position.x <= 0) {
			print ("reading position");
		}
		
		if (stylus.GetComponent<ZSStylusSelector>().GetButtonUp(1)){
			print ("stylus click");
		}
*/		
	} // end update
	
	void hapticSetting(int n) {
		int num = n;
		float on = 0;
		float off = 0;
		int repeat = 0;
		switch(num){
		    case 0:
		        break;
			case 1:                      // hover
			    on = 0.1f;
			    off = 0.1f;
			    repeat = 0;
			    break;
			case 2:                      // roll
			    on = 0.1f;
			    off = 0.1f;
			    repeat = -1;
			    break;
		    case 3:                      // sphere
			    on = 0.15f;
			    off = 0.15f;
			    repeat = 0;
			    break;
			case 4:                      // cube
			    on = 0.2f;
			    off = 0.2f;
			    repeat = 0;
			    break;
			case 5:
			    on = 0.3f;
			    off = 0.3f;
			    repeat = 0;
			    break;
		}
		playVibration (on,off,repeat); 		
	}
	
	void playVibration(float on, float off, int repeat) {
			core.GetComponent<ZSCore>().SetStylusVibrationOnPeriod(on);
			core.GetComponent<ZSCore>().SetStylusVibrationOffPeriod(off);
			core.GetComponent<ZSCore>().SetStylusVibrationRepeatCount(repeat);
			core.GetComponent<ZSCore>().SetStylusVibrationEnabled(true);
			core.GetComponent<ZSCore>().StartStylusVibration();
	}

// sphere object roll event	
	void roll() {
		Vector3 velocity = sphere.rigidbody.velocity;
		if((velocity[0] != 0 || velocity[2] != 0) && velocity[1] == 0){
		    //print (velocity);
		    if (isRolling == false) {
		        isRolling = true;
		        hapticSetting(2); 
		        print ("roll");
		    }		        			
		} else {
		    if (isRolling == true)
		        core.GetComponent<ZSCore>().StopStylusVibration();
		    isRolling = false;
		}
	}

// stylus hovered over object	
	void hover() {		
	   GameObject hoverObject = stylus.GetComponent<ZSStylusSelector>().hoveredObject;
	   if ((hoverObject != _oldHoverObject) && (hoverObject != null)){
	       hapticSetting (0);
	       _oldHoverObject = hoverObject;
	   }           
    }

// object collision	
	void OnCollisionEnter(Collision collision) {
		//print (this.gameObject.name);
		//print (collision.gameObject.name);
		if (this.gameObject.name.Equals("Sphere")) hapticSetting(3); 
		if (this.gameObject.name.Equals("Cube")){
		    if (collision.gameObject.name.Equals("Floor")) {
		        print (this.gameObject.rigidbody.velocity.y);
		        if (this.gameObject.rigidbody.velocity.y <= -3 || this.gameObject.rigidbody.velocity.y >= 1)
					hapticSetting(5);
				else if (this.gameObject.rigidbody.velocity.y <= -2 || this.gameObject.rigidbody.velocity.y >= 0)
				    hapticSetting(4);
				else if (this.gameObject.rigidbody.velocity.y <= 0)
				    hapticSetting(3);
			}
		}
	}
}
