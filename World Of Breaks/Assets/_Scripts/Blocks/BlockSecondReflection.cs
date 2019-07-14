using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSecondReflection : BlockWithText {
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

		if (!ball.isSecondReflection)
			ball.ChangeToSecondReflection ();
        
		if (lastAudioPlayTime + .05f < Time.time) {
			lastAudioPlayTime = Time.time;
			AudioManager.PlaySoundFromLibrary ("Reflection");
		}

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
		Destroy (gameObject);
	}
}
