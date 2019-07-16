using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {
    
	public static float ballRadius = .1f;
	public LayerMask colliderMask;

	public bool isThrowing = false;
	public bool enableRandomMove = false;
	public bool isClone;
	public bool isSecondReflection;
	public bool bottomCollide;

	static float lastAudioPlayTime;

	Sound hitSound;

	void Start ()
	{
		hitSound = Database.Get.playersData [User.GetInfo.curPlayerIndex].hitSound;
	}

	void FixedUpdate ()
	{
		if (isThrowing) {

			MoveBall ();
			transform.position += transform.up * Time.fixedDeltaTime * BallController.Instance.ballSpeed;

			//float clampedPosX = Mathf.Clamp(transform.position.x, -2.8f, 2.8f);
			//float clampedPosY = Mathf.Clamp(transform.position.y, -3.85f, 3.85f);
			//transform.position = new Vector3(clampedPosX, clampedPosY, 0);
		}
	}

	Collider2D oldHit;

	//Queue<string> lastCollisions = new Queue<string> (6);
	int leftCollCountHit, collCountHit = 0;
	BlockWithText BallSpawned = null;

	void CheckStacking (string name, float angle)
	{
		//print (leftCollCountHit + "   " + rightCollCountHit);
		if (Mathf.Abs (90 - angle) < 2) {

			//lastCollisions.Enqueue (name);

			if (name == "LeftCollider" || name == "RightCollider") {
				collCountHit++;
			} else {
				collCountHit = 0;
			}

			print (collCountHit);

			print (angle);

			if (collCountHit > 6 && BallSpawned == null) {
				BallSpawned = BlocksController.Instance.SpawnBlock (new Vector3 (BlocksController.Instance.centerOfScreen.x, transform.position.y));
				//StartCoroutine (BlocksController.Instance.DestroyBlockWhenLevelEnd (BallSpawned.gameObject, 1));
			}

		} else {
			collCountHit = 0;
			if (BallSpawned != null)
				Destroy (BallSpawned.gameObject);
		}
	}




	void MoveBall ()
	{
		if (this == null)
			return;


		Debug.DrawRay (transform.position - transform.up * ballRadius, transform.up * (ballRadius * 2 + .02f));

		RaycastHit2D hit = Physics2D.Raycast (transform.position - (transform.up * ballRadius), transform.up, ballRadius * 2, colliderMask);

		if (hit.collider == null)
			return;

		if (oldHit != hit.collider || hit.collider.CompareTag ("Edge")) {

			oldHit = hit.collider;

			if (!hit.collider.isTrigger) {
				
				Vector3 reflectDir = Vector3.Reflect (transform.up, hit.normal.normalized) + (Vector3.up + Vector3.left) * ((enableRandomMove) ? Random.Range (-.01f, .01f) : 0);

				float angle = Quaternion.Angle (Quaternion.Euler (reflectDir), transform.rotation);

				transform.position = hit.point - (Vector2)transform.up * ballRadius;

				CheckStacking (hit.transform.name, angle);

				if (lastAudioPlayTime + .05f < Time.time && hit.transform.name != "BottomCollider") {
					lastAudioPlayTime = Time.time;

					if (hitSound.clip == null)
						AudioManager.PlaySoundFromLibrary ("Ball1");
					else
						AudioManager.PlaySound (hitSound);
				}

				//print (hit.transform.name + "    " + hit.normal + "    " + reflectDir);

				ChangeDirection (reflectDir);
			}


			if (hit.transform.CompareTag ("Block")) {
				BlockWithText block = hit.collider.GetComponentInParent<BlockWithText> ();

				if (block != null)
					block.Hit (this);
			}

			if (hit.transform.name == "BottomCollider" && oldHit != null) {

				if (!bottomCollide && isSecondReflection) {
					bottomCollide = true;
				} else {

					isThrowing = false;
					if (isClone) {
						BallController.Instance.ballsList.Remove (this);
						Destroy (gameObject);
						return;
					}

					ChangeToOriginal ();

					if (!BallController.Instance.startPosChanged) {

						BallController.Instance.ChangeStartThrowPos (hit.point.x);
                        
						BallController.Instance.startPosChanged = true;
					}

					oldHit = hit.collider;
					ToStartPos ();
				}
			}

		}
	}

	public void ChangeDirection (Vector3 rot)
	{
		transform.rotation = Quaternion.FromToRotation (Vector3.up, rot);
	}

	public void ChangeDirection (float eulerAngle)
	{
		transform.localEulerAngles = new Vector3 (0, 0, eulerAngle);
	}

	public void GoThrow (Vector3 dir)
	{
		transform.rotation = Quaternion.FromToRotation (Vector3.up, dir);
		isThrowing = true;

	}

	public void ReturnBall ()
	{
		isThrowing = false;

		if (isClone) {
			BallController.Instance.ballsList.Remove (this);
			Destroy (gameObject);
			return;
		}
        
		ToStartPos ();
	}

	void ToStartPos ()
	{
		iTween.MoveTo (gameObject, BallController.Instance.startThrowPos, .7f);
		transform.eulerAngles = Vector3.zero;
		oldHit = null;
		ChangeToOriginal ();
		//transform.position = ballControl.startThrowPos;
	}

	public void ChangeToSecondReflection ()
	{
		isSecondReflection = true;
		GetComponent<SpriteRenderer> ().color = new Color (1, 0.5f, 0);
	}

	public void ChangeToClone ()
	{
		isClone = true;
		GetComponent<SpriteRenderer> ().color = Color.red;
	}

	public void ChangeToOriginal ()
	{
		isSecondReflection = false;
		isClone = false;
		bottomCollide = false;
		GetComponent<SpriteRenderer> ().color = Color.white;
        
	}

}
