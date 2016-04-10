using UnityEngine;
using System.Collections;

using LockingPolicy = Thalmic.Myo.LockingPolicy;
using Pose = Thalmic.Myo.Pose;
using UnlockType = Thalmic.Myo.UnlockType;
using VibrationType = Thalmic.Myo.VibrationType;

public class Move : MonoBehaviour {
	public float speed, boundarytop,boundarybot, paddleScale;
	public float movelimitup, movelimitdown;
	public GameObject myo; // gets Myo from scene
	public GameObject missile;
	private Quaternion _antiYaw = Quaternion.identity;
	private float _referenceRoll = 0.0f;
	public bool testMode, playerCol;
	public KeyCode up, down, shootButton, inverter;
	private bool canShoot = false;
	private bool inverted;

	private Pose lastPose;



	void Update(){
		if (testMode) {
			if(Input.GetKey(up) && transform.position.y < boundarytop){
				transform.Translate (Vector3.up * Time.deltaTime * speed);
			}
			//down is positive
			else if(Input.GetKey(down) && transform.position.y > boundarybot){
				transform.Translate (Vector3.down * Time.deltaTime * speed);
			}
			if(Input.GetKeyDown(shootButton)){
				spawnMissile();
			}
		} else {
			if (Input.GetKeyDown ("r")){
				_antiYaw = Quaternion.FromToRotation (
					new Vector3 (myo.transform.forward.x, 0, myo.transform.forward.z),
					new Vector3 (0, 0, 1)
					);
				/*
			Vector3 referenceZeroRoll = computeZeroRollVector (myo.transform.forward);
			_referenceRoll = rollFromZero (referenceZeroRoll, myo.transform.forward, myo.transform.up);
			*/
			}
			
			Vector3 zeroRoll = computeZeroRollVector (myo.transform.forward);
			float roll = rollFromZero (zeroRoll, myo.transform.forward, myo.transform.up);
			float relativeRoll = normalizeAngle (roll - _referenceRoll);
			Quaternion antiRoll = Quaternion.AngleAxis (relativeRoll, myo.transform.forward);
			Quaternion movement = _antiYaw * antiRoll * Quaternion.LookRotation (myo.transform.forward);

			if(Input.GetKeyDown(inverter)){
				inverted = !inverted;
			}
			if(!inverted){
				if(movement.x < movelimitup && transform.position.y < boundarytop){
					transform.Translate (Vector3.up * Time.deltaTime * speed);
				}
				else if(movement.x > movelimitdown && transform.position.y > boundarybot){
					transform.Translate (Vector3.down * Time.deltaTime * speed);
				}
			}else{
				if( movement.x > movelimitdown && transform.position.y < boundarytop){
					transform.Translate (Vector3.up * Time.deltaTime * speed);
				}
				else if(movement.x < movelimitup && transform.position.y > boundarybot){
					transform.Translate (Vector3.down * Time.deltaTime * speed);
				}
			}

			ThalmicMyo thalmicMyo = myo.GetComponent<ThalmicMyo> ();
			if (thalmicMyo.pose != lastPose) {
				lastPose = thalmicMyo.pose;
				
				// Vibrate the Myo armband when a fist is made.
				if (thalmicMyo.pose == Pose.Fist) {
					thalmicMyo.Vibrate (VibrationType.Medium);
					spawnMissile();
					//ExtendUnlockAndNotifyUserAction (thalmicMyo);
				}
			}
					
		}
	}
	void spawnMissile(){
		if (canShoot) {
			//canShoot = false;
			if (playerCol) {
				Instantiate (missile, new Vector3 (transform.position.x + 1, transform.position.y, 0), missile.transform.rotation);
			} else {
				Instantiate (missile, new Vector3 (transform.position.x - 1, transform.position.y, 0), missile.transform.rotation);

			}
		}
	}
		
		Vector3 computeZeroRollVector (Vector3 forward)
	{
		Vector3 antigravity = Vector3.up;
		Vector3 m = Vector3.Cross (myo.transform.forward, antigravity);
		Vector3 roll = Vector3.Cross (m, myo.transform.forward);
		
		return roll.normalized;
	}

	float normalizeAngle (float angle)
	{
		if (angle > 180.0f) {
			return angle - 360.0f;
		}
		if (angle < -180.0f) {
			return angle + 360.0f;
		}
		return angle;
	}
	void OnCollisionEnter(Collision col){
		if (col.gameObject.tag == "ball") {
			canShoot = true;
		}
		if (col.gameObject.layer == 9 || col.gameObject.layer == 10) {
			Destroy(col.gameObject);
			transform.localScale += new Vector3(0, paddleScale, 0);
		}
	}

	float rollFromZero (Vector3 zeroRoll, Vector3 forward, Vector3 up)
	{
		// The cosine of the angle between the up vector and the zero roll vector. Since both are
		// orthogonal to the forward vector, this tells us how far the Myo has been turned around the
		// forward axis relative to the zero roll vector, but we need to determine separately whether the
		// Myo has been rolled clockwise or counterclockwise.
		float cosine = Vector3.Dot (up, zeroRoll);
		
		// To determine the sign of the roll, we take the cross product of the up vector and the zero
		// roll vector. This cross product will either be the same or opposite direction as the forward
		// vector depending on whether up is clockwise or counter-clockwise from zero roll.
		// Thus the sign of the dot product of forward and it yields the sign of our roll value.
		Vector3 cp = Vector3.Cross (up, zeroRoll);
		float directionCosine = Vector3.Dot (forward, cp);
		float sign = directionCosine < 0.0f ? 1.0f : -1.0f;
		
		// Return the angle of roll (in degrees) from the cosine and the sign.
		return sign * Mathf.Rad2Deg * Mathf.Acos (cosine);
	}

}
