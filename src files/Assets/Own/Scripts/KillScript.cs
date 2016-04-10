using UnityEngine;
using System.Collections;

public class KillScript : MonoBehaviour {
	public bool blue;

	void OnTriggerEnter(Collider other) {
		Destroy(other.gameObject);
		if (other.gameObject.tag == "ball") {
			GameCtrl.instance.LoseLife (blue);
		}
	}
}
