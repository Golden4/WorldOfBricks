using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallController : MonoBehaviour {

	public Ball ballPrefab;
	public List<Ball> ballsList;
	public float ballSpeed = 3;
	public int ballCount = 2;
	public Vector2 startThrowPos;
	public bool startPosChanged = false;
	public float timeBetweenBalls = .1f;
	public Image throwingDirectionImage;
	public Text ballCountText;
	public Image mousePivotPointImage;
	public static BallController Instance;
	bool needReturnAllBalls;
	bool canThrow = true;

	enum MouseState {
		mouseDragging,
		mouseUp
	}

	MouseState mouseCurState = MouseState.mouseUp;

	void Awake ()
	{
		if (Instance == null) {
			Instance = this;
		}
	}

	void Start ()
	{
		if (Game.isChallenge)
			ballCount = Game.curChallengeInfo.ballCount;
		else if (UIScreen.newGame)
				ballCount = UIScreen.Ins.level;
		SpriteRenderer ballPrebSpr = ballPrefab.GetComponent <SpriteRenderer> ();
		ballPrebSpr.sprite = Database.Get.playersData [User.GetInfo.curPlayerIndex].ballSprite;
		ballPrebSpr.size = Vector2.one * Database.Get.playersData [User.GetInfo.curPlayerIndex].ballRadius * 2;
		ballPrebSpr.transform.localScale = Vector2.one;
		Ball.ballRadius = Database.Get.playersData [User.GetInfo.curPlayerIndex].ballRadius;

		ChangeStartThrowPos (0);

		throwingDirectionImage.gameObject.SetActive (false);
		InstantiateBallsList ();
		UpdateBallCount ();
        
	}

	void InstantiateBallsList ()
	{
		if ((ballCount - ballsList.Count) >= 1) {

			int ballCountMax = ballCount - ballsList.Count;
			
			for (int i = 0; i < ballCountMax; i++) {
				Transform ballTmp = Instantiate<GameObject> (ballPrefab.gameObject).transform;
				ballTmp.position = startThrowPos;
				ballsList.Add (ballTmp.GetComponent<Ball> ());
			}
		}
	}

	Vector3 startMousePos;
	Vector3 curMousePos;
	Vector3 dirMouse;
	Vector3 dirToThrow;
	float lastThrowTime;
	float dirMouseMagnitudeWorld;
	bool isMouseOnUIObject;

	void Update ()
	{

		if (UIScreen.Ins.playerLose)
			return;

		if (!canThrow) {
			if (lastThrowTime + 2 < Time.time) {
				UIScreen.Ins.ShowTimeAcceleratorBtn ();
			}
			return;
		}
        

		if (Input.GetMouseButton (0) || Input.GetMouseButtonDown (0)) {
			
			if (mouseCurState == MouseState.mouseUp) {
				
				#if UNITY_ANDROID && !UNITY_EDITOR
				isMouseOnUIObject = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject (Input.GetTouch (0).fingerId);
				#else
				isMouseOnUIObject = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject ();
				#endif

				mouseCurState = MouseState.mouseDragging;

				if (!isMouseOnUIObject)
					MouseDown ();
				
                
			} else if (mouseCurState == MouseState.mouseDragging) {

					if (!isMouseOnUIObject)
						MouseHold ();

				}

		} else if (mouseCurState == MouseState.mouseDragging) {
			
				mouseCurState = MouseState.mouseUp;
            
				MouseUp (dirMouseMagnitudeWorld > .6f && !isMouseOnUIObject && validDir);
			}

	}

	bool validDir;

	void MouseDown ()
	{
		startMousePos = Input.mousePosition;
        
		mousePivotPointImage.gameObject.SetActive (true);
		mousePivotPointImage.transform.position = startMousePos;
        
		Vector2 heigth = throwingDirectionImage.rectTransform.sizeDelta;
		heigth.y = 10;

		throwingDirectionImage.rectTransform.sizeDelta = heigth;

		throwingDirectionImage.transform.position = Camera.main.WorldToScreenPoint (startThrowPos);
	}

	void MouseHold ()
	{
		curMousePos = Input.mousePosition;

		dirMouse = ((InputMobileController.curInputType == InputMobileController.InputType.Touch) ? 1 : -1) * (-curMousePos + startMousePos);
		dirMouseMagnitudeWorld = Mathf.Abs (Vector2.Distance ((Vector2)Camera.main.ScreenToWorldPoint (curMousePos), (Vector2)Camera.main.ScreenToWorldPoint (startMousePos)));

		Quaternion rotation = Quaternion.FromToRotation (Vector3.up, dirMouse.normalized);

		if (Quaternion.Angle (rotation, Quaternion.Euler (Vector3.up)) < 85) {
			validDir = true;
			if (!TrajectoryHelper.Ins.isActive) {
				if (dirMouseMagnitudeWorld > .6f)
					throwingDirectionImage.gameObject.SetActive (true);
				else
					throwingDirectionImage.gameObject.SetActive (false);

				throwingDirectionImage.transform.rotation = rotation;

				Vector2 heigth = throwingDirectionImage.rectTransform.sizeDelta;
				heigth.y = Mathf.Clamp (dirMouse.magnitude, 1, 150) * 4.5f;

				throwingDirectionImage.rectTransform.sizeDelta = heigth;
			} else {
				TrajectoryHelper.Ins.CalculateTrajectory (startThrowPos, dirMouse, dirMouseMagnitudeWorld);

			}

			dirToThrow = dirMouse;
		} else {
			validDir = false;
			MouseUp (false);

			mousePivotPointImage.gameObject.SetActive (true);
            
		}
	}

	void MouseUp (bool needThrow)
	{
		mousePivotPointImage.gameObject.SetActive (false);

		throwingDirectionImage.gameObject.SetActive (false);

		if (needThrow) {
			ThrowBalls (dirToThrow.normalized);
		}

		TrajectoryHelper.Ins.ShowTajectory (false);
	}

	void ThrowBalls (Vector3 dir)
	{
		InstantiateBallsList ();
		needReturnAllBalls = false;
		UIScreen.Ins.HideTutorial ();
		StartCoroutine (ThrowBallsCoroutine (dir));
	}

	void CalcTimeBeweenBalls ()
	{
		float time = .1f - ballCount * .01f;
		if (ballCount > 50) {
			timeBetweenBalls = .04f;
		} else {
			timeBetweenBalls = (time < .05) ? .05f : time;
		}
	}

	IEnumerator ThrowBallsCoroutine (Vector3 dir)
	{

		canThrow = false;

		CalcTimeBeweenBalls ();

		int tmpBallCount = ballCount;

		List<Ball> ballList = new List<Ball> (ballsList);

		UIScreen.Ins.EnableReturnBallsBtn (true);
		UIScreen.Ins.HideTimeAcceleratorBtn ();

		lastThrowTime = Time.time;

		for (int i = 0; i < ballList.Count; i++) {
			if (needReturnAllBalls)
				break;

			ballList [i].GoThrow (dir);
			tmpBallCount--;
			ballCountText.text = "x" + tmpBallCount;
			yield return new WaitForSeconds (timeBetweenBalls);
		}

		iTween.ScaleTo (ballCountText.gameObject, Vector3.zero, .5f);

		//ballCountText.gameObject.SetActive (false);
        

		while (!canThrow) {
			
			for (int i = 0; i < ballsList.Count; i++) {
				
				if (ballsList [i].isThrowing) {
					break;
				}

				if (i == ballsList.Count - 1) {
					canThrow = true;
				}
			}

			yield return null;
		}

		UIScreen.Ins.EnableReturnBallsBtn (false);

		//ballCountText.gameObject.SetActive (true);

		//iTween.FadeFrom (ballCountText.gameObject, 0, .5f);

		iTween.ScaleTo (ballCountText.gameObject, Vector3.one, .5f);

		UpdateBallCount ();

		startPosChanged = false;

		BlocksController.Instance.blockDestroyCount = 0;

		if (!UIScreen.Ins.playerLose) {
			if (!Game.isChallenge)
				BlocksController.Instance.ShiftBlockMapDown ();
			else
				BlocksController.Instance.ChallengeProgress ();
		}
	}

	public void ReturnAllBalls ()
	{

		List<Ball> balls = new List<Ball> (ballsList);
		needReturnAllBalls = true;
		for (int i = 0; i < balls.Count; i++) {
			balls [i].ReturnBall (); 
		}
	}

	public void	BallCountPlus ()
	{
		ballCount++;
	}

	public void UpdateBallCount ()
	{
		Vector3 pos = Camera.main.WorldToScreenPoint (startThrowPos + Vector2.left * .5f) + Vector3.up * 7f;
		if (pos.x < 0) {
			pos.x += Mathf.Abs (pos.x) + 50;
		}

		ballCountText.transform.position = pos;
		ballCountText.text = "x" + ballCount;
	}

	public void ChangeStartThrowPos (float x)
	{
		Vector3 cameraPos = Camera.main.transform.position;

		float x1 = Mathf.Clamp (x, EdgeScreenCollisions.GetScreenEdgeLeft ().x + Ball.ballRadius, EdgeScreenCollisions.GetScreenEdgeRight ().x - Ball.ballRadius);

		startThrowPos = EdgeScreenCollisions.GetScreenEdgeBottom () + Vector2.up * Ball.ballRadius + Vector2.right * x1;
	}

}
