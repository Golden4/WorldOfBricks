using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

	CircleCollider2D col;
	public LayerMask colliderMask;

	public bool isThrowing = false;
	public BallController ballControl;
	public bool enableRandomMove = false;

	void Awake ()
	{
		col = GetComponent<CircleCollider2D> ();
		ballControl = FindObjectOfType<BallController> ();



	}

	/*void Start ()
	{
		TrailRenderer tr = GetComponent<TrailRenderer> ();

		tr.startColor = Random.ColorHSV ();

		tr.endColor = Random.ColorHSV ();

	}*/

	void FixedUpdate ()
	{
		if (isThrowing) {
			MoveBall ();
		}

	}

	RaycastHit2D oldHit;

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
		transform.position += transform.up * Time.fixedDeltaTime * ballControl.ballSpeed;

		Debug.DrawRay (transform.position - transform.up * col.radius, transform.up * (col.radius * 2 + .02f));

		RaycastHit2D hit = Physics2D.Raycast (transform.position - (transform.up * col.radius), transform.up, col.radius * 2 + .02f, colliderMask);

		if (hit.collider != null && oldHit.transform != hit.transform) {

			oldHit = hit;


			if (!hit.collider.isTrigger) {
				
				Vector3 reflectDir = Vector3.Reflect (transform.up, hit.normal.normalized) + (Vector3.up + Vector3.left) * ((enableRandomMove) ? Random.Range (-.01f, .01f) : 0);

				float angle = Quaternion.Angle (Quaternion.Euler (reflectDir), transform.rotation);

				CheckStacking (hit.transform.name, angle);

				//print (hit.transform.name + "    " + hit.normal + "    " + reflectDir);

				ChangeDirection (reflectDir);
			}

			if (hit.transform.CompareTag ("Block")) {
				BlockWithText block = hit.collider.GetComponentInParent<BlockWithText> ();

				if (block != null)
					block.Hit (this);
				print (block);
			}

			if (hit.transform.name == "BottomCollider") {
				
				isThrowing = false;

				if (!BallController.Instance.startPosChanged) {

					Vector3 pos = new Vector3 (Mathf.Clamp (hit.point.x, -3, 3), -4.8f);

					BallController.Instance.startThrowPos = pos; //hit.point + Vector2.up * .2f;
					BallController.Instance.startPosChanged = true;
				}

				ToStartPos ();
			}

		}
	}

	public void ChangeDirection (Vector3 rot)
	{
		transform.rotation = Quaternion.FromToRotation (Vector3.up, rot);
	}

	public void GoThrow (Vector3 dir)
	{
		transform.rotation = Quaternion.FromToRotation (Vector3.up, dir);
		isThrowing = true;
	}

	void ToStartPos ()
	{
		iTween.MoveTo (gameObject, ballControl.startThrowPos, .7f);
		//transform.position = ballControl.startThrowPos;
	}



}
