using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {
    
	public static float ballRadius = .1f;
	public static List<Ability> curAbilites = new List<Ability> ();
	public LayerMask colliderMask;
	public static Collider2D bottomCollider;

	public bool isThrowing = false;
	public bool isClone;
	public int canReflectionCount;
	public bool bottomCollide;

	public enum Ability {
		None,
		RandomDirection,
		Speed,
		CoinChance,
		Bounce2Times,
		Clone3Times,
		SkullDestroy,
		DoubleHitBrick,
		HigherChekpoint
	}

	static float lastAudioPlayTime;

	Sound hitSound;

	void Start ()
	{
		oldHit = bottomCollider;
		hitSound = Database.Get.playersData [User.GetInfo.GetCurPlayerIndex ()].hitSound;
	}

	void FixedUpdate ()
	{
		if (isThrowing) {

			transform.position += transform.up * Time.fixedDeltaTime * BallController.Instance.ballSpeed;
			MoveBall ();


			//float clampedPosX = Mathf.Clamp(transform.position.x, -2.8f, 2.8f);
			//float clampedPosY = Mathf.Clamp(transform.position.y, -3.85f, 3.85f);
			//transform.position = new Vector3(clampedPosX, clampedPosY, 0);
		}
	}

	public static bool HaveAblity (Ball.Ability ability)
	{
		return curAbilites.Contains (ability);
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

			Debug.Log (collCountHit);

			Debug.Log (angle);

			if (collCountHit > 6 && BallSpawned == null) {
				BallSpawned = BlocksController.Instance.SpawnBlock (new Vector3 (0, transform.position.y));
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


		//Debug.DrawRay (transform.position - transform.up * ballRadius, transform.up * (ballRadius * 2 + .02f));

		RaycastHit2D hit = Physics2D.Raycast (transform.position - (transform.up * ballRadius), transform.up, ballRadius * 2, colliderMask);

		if (hit.collider == null)
			return;
		
		if (oldHit != hit.collider || hit.collider.CompareTag ("Edge")) {
			
			oldHit = hit.collider;

			if (!hit.collider.isTrigger) {
				
				Vector3 reflectDir = Vector3.Reflect (transform.up, hit.normal.normalized);

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

			if (hit.collider == bottomCollider/*.transform.name == "BottomCollider"*/) {

				if (canReflectionCount > 0) {
					canReflectionCount--;
					bottomCollide = true;
				} else {

					isThrowing = false;

					if (isClone) {
						DestroyClone ();
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
		transform.rotation = Quaternion.FromToRotation (Vector3.up, dir + (Vector3.up + Vector3.left) * ((HaveAblity (Ability.RandomDirection)) ? UnityEngine.Random.Range (-.08f, .08f) : 0));
		isThrowing = true;
	}

	public void ReturnBall (bool animate = true)
	{
		isThrowing = false;

		if (isClone) {
			DestroyClone ();
			return;
		}

		BallController.Instance.startPosChanged = true;

		ToStartPos (animate);
	}

	public void ToStartPos (bool animate = true)
	{
		if (animate)
			iTween.MoveTo (gameObject, BallController.Instance.startThrowPos, .7f);
		else
			transform.position = BallController.Instance.startThrowPos;
		
		transform.eulerAngles = Vector3.zero;
		oldHit = bottomCollider;
		ChangeToOriginal ();
		//transform.position = ballControl.startThrowPos;
	}

	public void ChangeToSecondReflection ()
	{
		if (bottomCollide)
			return;

		if (!Ball.HaveAblity (Ability.Bounce2Times))
			canReflectionCount = 1;
		else
			canReflectionCount = 2;
		
		GetComponent<SpriteRenderer> ().color = new Color (1, 0.5f, 0);
	}

	public void ChangeToClone ()
	{
		isClone = true;
		GetComponent<SpriteRenderer> ().color = Color.red;
	}

	public void ChangeToOriginal ()
	{
		canReflectionCount = 0;
		isClone = false;
		bottomCollide = false;
		GetComponent<SpriteRenderer> ().color = Color.white;
        
	}

	public void DestroyClone ()
	{
		BallController.Instance.ballsList.Remove (this);
		Destroy (Instantiate<GameObject> (BallController.Instance.cloneDestroyParticle.gameObject, transform.position, Quaternion.identity), 2);
		Destroy (gameObject);
	}

}
