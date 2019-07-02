using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallController : MonoBehaviour {

	public Ball ballPrefab;
	public List<Ball> ballsList;
	public float ballSpeed = 3;
	public int ballCount = 2;
	public Vector2 startThrowPos = new Vector2 (-.5f, -3.75f);
	public bool startPosChanged = false;
	public float timeBetweenBalls = .1f;
	public Image throwingDirectionImage;
	public Text ballCountText;
	public Image mousePivotPointImage;
	public static BallController Instance;

	bool canThrow = true;

	enum MouseState
	{
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
		throwingDirectionImage.gameObject.SetActive (false);

		InstantiateBallsList ();
		UpdateBallCountText ();
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

	void Update ()
	{
		if (!canThrow || UIScreen.Ins.playerLose)
			return;


		if (Input.GetMouseButton (0)) {
			
			if (mouseCurState == MouseState.mouseUp) {
				mouseCurState = MouseState.mouseDragging;
				startMousePos = Input.mousePosition;

				mousePivotPointImage.gameObject.SetActive (true);
				mousePivotPointImage.transform.position = startMousePos;

				throwingDirectionImage.gameObject.SetActive (true);

				Vector2 heigth = throwingDirectionImage.rectTransform.sizeDelta;
				heigth.y = 10;

				throwingDirectionImage.rectTransform.sizeDelta = heigth;

				throwingDirectionImage.transform.position = Camera.main.WorldToScreenPoint (startThrowPos);

			} else if (mouseCurState == MouseState.mouseDragging) {
				
				curMousePos = Input.mousePosition;

				dirMouse = -curMousePos + startMousePos;

                Quaternion rotation = Quaternion.FromToRotation(Vector3.up, dirMouse.normalized);

                if (Quaternion.Angle(rotation, Quaternion.Euler(Vector3.up)) < 90)
                {
                    if (!TrajectoryHelper.Ins.isActive)
                    {
                        throwingDirectionImage.transform.rotation = rotation;

                        Vector2 heigth = throwingDirectionImage.rectTransform.sizeDelta;
                        heigth.y = Mathf.Clamp(dirMouse.magnitude, 10, 150) * 4.5f + 100;

                        throwingDirectionImage.rectTransform.sizeDelta = heigth;
                    }
                    else
                    {
                        TrajectoryHelper.Ins.CalculateTrajectory(startThrowPos, dirMouse);
                    }

                    dirToThrow = dirMouse;
                }

            }

		} else if (mouseCurState == MouseState.mouseDragging) {
			
			mouseCurState = MouseState.mouseUp;

			mousePivotPointImage.gameObject.SetActive (false);

			throwingDirectionImage.gameObject.SetActive (false);
            if (dirMouse.sqrMagnitude > 1000)
            {
                ThrowBalls(dirToThrow.normalized);
            }
            TrajectoryHelper.Ins.ShowTajectory(false);
        }

	}

	void ThrowBalls (Vector3 dir)
	{
		InstantiateBallsList ();
		StartCoroutine (ThrowBallsCoroutine (dir));
	}

	IEnumerator ThrowBallsCoroutine (Vector3 dir)
	{

		canThrow = false;

		int tmpBallCount = ballCount;

		for (int i = 0; i < ballsList.Count; i++) {
			ballsList [i].GoThrow (dir);
			tmpBallCount--;
			ballCountText.text = "x" + tmpBallCount;

			yield return new WaitForSeconds (timeBetweenBalls);
		}

		iTween.ScaleTo (ballCountText.gameObject, Vector3.zero, .5f);

        //ballCountText.gameObject.SetActive (false);

        UIScreen.Ins.EnableReturnBallsBtn(true);

        float lastThrowTime = Time.time;

		while (!canThrow) {
			
			for (int i = 0; i < ballsList.Count; i++) {
				
				if (ballsList [i].isThrowing) {
					break;
				}

				if (i == ballsList.Count - 1) {
					canThrow = true;
				}
			}

            if(lastThrowTime + 3 < Time.time)
            {
                UIScreen.Ins.ShowTimeAcceleratorBtn();
            }

			yield return null;
		}

        UIScreen.Ins.EnableReturnBallsBtn(false);

        //ballCountText.gameObject.SetActive (true);

        //iTween.FadeFrom (ballCountText.gameObject, 0, .5f);

        iTween.ScaleTo (ballCountText.gameObject, Vector3.one, .5f);

		ballCountText.text = "x" + ballCount;

		Vector3 pos = Camera.main.WorldToScreenPoint (startThrowPos + Vector2.left * .5f) + Vector3.up * 7f;
		if (pos.x < 0) {
			pos.x += Mathf.Abs (pos.x) + 50;
		}

		ballCountText.transform.position = pos;


		BlocksController.Instance.ShiftBlockMapDown ();
		startPosChanged = false;
	}

    public void ReturnAllBalls()
    {

        List<Ball> balls = new List<Ball>(ballsList);

        for (int i = 0; i < balls.Count; i++)
        {
            balls[i].ReturnBall(); 
        }
    }

	public void	BallCountPlus ()
	{
		ballCount++;
		float time = .1f - ballCount * .01f;
		if (ballCount > 50) {
			timeBetweenBalls = .01f;
		} else {
			timeBetweenBalls = (time < .05) ? .05f : time;
		}


	}

	void UpdateBallCountText ()
	{
		ballCountText.text = "x" + ballCount;
	}

}
