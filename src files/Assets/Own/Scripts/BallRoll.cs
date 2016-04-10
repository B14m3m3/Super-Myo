using UnityEngine;
using System.Collections;

public class BallRoll : MonoBehaviour {
	private Rigidbody rb;
	public float ballInitialVelocity;
	private bool once = true;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();

	}
	void Update(){
		if(once){
			once = false;
			rb.AddForce (new Vector3 (ballInitialVelocity, ballInitialVelocity, 0));
		}
	}
}
