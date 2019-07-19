using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDoubleBalls : BlockWithText {
	bool needDestroy;

	protected override void Start ()
	{
		BlocksController.Instance.OnChangeTopLine += TryDie;
	}

	protected override void TimerStart ()
	{
		transform.GetChild (0).localScale = new Vector3 (1 + t / 3f, 1 + t / 3f, 1);
	}

	protected override void TimerEnd ()
	{
		transform.GetChild (0).localScale = Vector3.one;
	}

	static float lastAudioPlayTime;

	public override void Hit (Ball ball)
	{
		if (ball.isClone)
			return;
		t = 1;
		needDestroy = true;
		Transform ballTmp = Instantiate<GameObject> (ball.gameObject).transform;
		ballTmp.position = ball.transform.position;
		Ball ballClone = ballTmp.GetComponent<Ball> ();
		ballClone.ChangeToClone ();
		BallController.Instance.ballsList.Add (ballClone);

		float angle = ball.transform.localEulerAngles.z;
		ball.ChangeDirection (angle - 30);

		ballClone.ChangeDirection (angle + 30);

		if (lastAudioPlayTime + .05f < Time.time) {
			lastAudioPlayTime = Time.time;
			AudioManager.PlaySoundFromLibrary ("DoubleBall");
		}

	}

	void TryDie ()
	{
		if (needDestroy)
			Die ();
	}

	void OnDestroy ()
	{
		BlocksController.Instance.OnChangeTopLine -= TryDie;
	}

	protected override void OnDead ()
	{
		Destroy (gameObject);
	}
}
