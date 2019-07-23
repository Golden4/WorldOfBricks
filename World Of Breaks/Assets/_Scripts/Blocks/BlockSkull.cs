using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSkull : BlockWithText {
	
	bool needDestroy = false;

	protected override void Start ()
	{
		BlocksController.Instance.OnChangeTopLine += TryDie;
	}

	static float lastAudioPlayTime;

	public override void Hit (Ball ball)
	{
		if (Ball.HaveAblity (Ball.Ability.SkullDestroy)) {
			Die ();
			return;
		}

		needDestroy = true;

		t = 1;

		ball.ReturnBall (false);

		if (lastAudioPlayTime + .05f < Time.time) {
			lastAudioPlayTime = Time.time;
			AudioManager.PlaySoundFromLibrary ("Skull");
		}

	}

	protected override void TimerStart ()
	{
		transform.GetChild (0).localScale = new Vector3 (1 + t / 3f, 1 + t / 3f, 1);
	}

	protected override void TimerEnd ()
	{
		transform.GetChild (0).localScale = Vector3.one;
	}

	void OnDestroy ()
	{
		BlocksController.Instance.OnChangeTopLine -= TryDie;
	}

	void TryDie ()
	{
		if (needDestroy)
			Die ();
	}

	protected override void OnDead ()
	{
		iTween.ScaleTo (transform.GetChild (0).gameObject, Vector3.zero, .2f);
		Destroy (gameObject, .2f);
	}
}
