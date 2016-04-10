using UnityEngine;
using System.Collections;

public class shoot : MonoBehaviour {
	private Rigidbody rb;
	public float missileSpeed;
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		rb.AddForce (new Vector3 (missileSpeed, 0, 0));
	}
}
