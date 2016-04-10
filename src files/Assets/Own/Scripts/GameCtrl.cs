using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using LockingPolicy = Thalmic.Myo.LockingPolicy;
using Pose = Thalmic.Myo.Pose;
using UnlockType = Thalmic.Myo.UnlockType;
using VibrationType = Thalmic.Myo.VibrationType;

public class GameCtrl : MonoBehaviour {
	public GameObject ball;
	private int red = 3;
	private int blue = 3;
	private bool inGame = false;
	private bool blueRdy = false;
	private bool redRdy = false;
	public static GameCtrl instance = null;
	public Text redScore,blueScore, winner;
	public Text redRdyText, blueRdyText;

	public GameObject bluePad, redPad;
	private Pose lastBluePose = Pose.Unknown;
	private Pose lastRedPose = Pose.Unknown;
	public GameObject blueMyo,redMyo;

	private bool detWinner = false;
	public bool test;


	void Awake () 
	{
		Physics.IgnoreLayerCollision (8, 9);
		Physics.IgnoreLayerCollision (8, 10);
		Physics.IgnoreLayerCollision (9, 10);
			
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
		setup ();
	}

	void setup(){
		blueScore.text = ""+blue;
		redScore.text = ""+red;
	}


	void spawnBall(){
		Instantiate (ball, Vector3.zero, Quaternion.identity);
	}

	public void LoseLife(bool blueDeath)
	{
		inGame = false;
		if (blueDeath) {
			blue--;
			blueScore.text = ""+blue;
		} else {
			red--;
			redScore.text = ""+red;
		}
		if (red <= 0 || blue <= 0) {
			if(blue <= 0){
				detWinner = true;
				winner.color = Color.red;
				winner.text = "RED WON!";
				inGame = true;
			}else{
				detWinner = true;
				winner.color = Color.blue;
				winner.text = "BLUE WON!";
				inGame = true;
			}
		}
	}

	void Update(){
		if (!inGame) {
			if(test){
				if(Input.GetKeyDown(KeyCode.Space)){
					redRdy = true;
					blueRdy = true;
				}
			}else{
			ThalmicMyo thalmicMyoRed = redMyo.GetComponent<ThalmicMyo> ();
			if (thalmicMyoRed.pose != lastRedPose && !redRdy) {
				lastRedPose = thalmicMyoRed.pose;
				
				// Vibrate the Myo armband when a fist is made.
				if (thalmicMyoRed.pose == Pose.DoubleTap) {
					redRdy = true;
					redRdyText.text = "READY";
					redRdyText.color = Color.red;
					thalmicMyoRed.Vibrate (VibrationType.Medium);
					//ExtendUnlockAndNotifyUserAction (thalmicMyo);
				}
			}

			ThalmicMyo thalmicMyoBlue = blueMyo.GetComponent<ThalmicMyo> ();
			if (thalmicMyoBlue.pose != lastBluePose && !blueRdy) {
				lastBluePose = thalmicMyoBlue.pose;
				
				// Vibrate the Myo armband when a fist is made.
				if (thalmicMyoBlue.pose == Pose.DoubleTap) {
					blueRdy = true;
					blueRdyText.text = "READY";
					blueRdyText.color = Color.blue;
					thalmicMyoBlue.Vibrate (VibrationType.Medium);
					//ExtendUnlockAndNotifyUserAction (thalmicMyo);
				}
			}
			}
			if(blueRdy && redRdy){
				blueRdyText.text = "";
				redRdyText.text = "";
				inGame=true;
				blueRdy = false;
				redRdy = false;
				spawnBall();
			}
		}


		if (detWinner && Input.GetKeyDown(KeyCode.Space)) {
			detWinner = !detWinner;
			Vector3 padSize = new Vector3(0.5f,2f,1f);
			bluePad.transform.localScale = padSize;
			redPad.transform.localScale = padSize;
			blueRdyText.text = "Double Tap";
			blueRdyText.color = Color.black;
			redRdyText.text = "Double Tap";
			redRdyText.color = Color.black;
			winner.text = "";
			red = 3;
			blue = 3;
			blueScore.text = ""+ blue;
			redScore.text = ""+ red;
			inGame = false;
		}
	}
}
