using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeScreenCollisions : MonoBehaviour {
	public float colThickness = 4f;
	public float zPosition = 0f;
	private Vector2 screenSize;
	//public static Vector2 screenResolution;

	void Start ()
	{


		Screen.SetResolution (Mathf.RoundToInt (Screen.height / 16f * 10f), Screen.height, false);


		System.Collections.Generic.Dictionary<string,Transform> colliders = new System.Collections.Generic.Dictionary<string,Transform> ();

		colliders.Add ("Top", new GameObject ().transform);
		colliders.Add ("Bottom", new GameObject ().transform);
		colliders.Add ("Right", new GameObject ().transform);
		colliders.Add ("Left", new GameObject ().transform);

		Vector3 cameraPos = Camera.main.transform.position;

		screenSize.x = Vector2.Distance (Camera.main.ScreenToWorldPoint (new Vector2 (0, 0)), Camera.main.ScreenToWorldPoint (new Vector2 (Screen.height / 16f * 10f, 0))) * 0.5f;
		screenSize.y = Vector2.Distance (Camera.main.ScreenToWorldPoint (new Vector2 (0, 0)), Camera.main.ScreenToWorldPoint (new Vector2 (0, Screen.height))) * 0.5f;

		foreach (KeyValuePair<string,Transform> valPair in colliders) {
			valPair.Value.gameObject.AddComponent<BoxCollider2D> ();
			valPair.Value.name = valPair.Key + "Collider";
			valPair.Value.parent = transform;
 
			if (valPair.Key == "Left" || valPair.Key == "Right")
				valPair.Value.localScale = new Vector3 (colThickness, screenSize.y * 2 + .5f, colThickness);
			else
				valPair.Value.localScale = new Vector3 (screenSize.x * 2 + .5f, colThickness, colThickness);
		}  


		colliders ["Right"].position = new Vector3 (cameraPos.x + screenSize.x + (colliders ["Right"].localScale.x * 0.5f), cameraPos.y, zPosition);
		colliders ["Left"].position = new Vector3 (cameraPos.x - screenSize.x - (colliders ["Left"].localScale.x * 0.5f), cameraPos.y, zPosition);
		colliders ["Top"].position = new Vector3 (cameraPos.x, cameraPos.y + screenSize.y + (colliders ["Top"].localScale.y * 0.5f), zPosition);
		colliders ["Bottom"].position = new Vector3 (cameraPos.x, cameraPos.y - screenSize.y - (colliders ["Bottom"].localScale.y * 0.5f), zPosition);



		GameObject TopRight = new GameObject ("TopRight");
		TopRight.transform.position = new Vector3 (cameraPos.x + screenSize.x, cameraPos.y + screenSize.y);
		TopRight.transform.eulerAngles = new Vector3 (0, 0, 45);
		TopRight.transform.parent = transform;
		TopRight.transform.localScale = new Vector3 (.3f, .3f, 1);

		GameObject TopLeft = new GameObject ("TopLeft");
		TopLeft.transform.position = new Vector3 (cameraPos.x - screenSize.x, cameraPos.y + screenSize.y);
		TopLeft.transform.eulerAngles = new Vector3 (0, 0, 45);
		TopLeft.transform.parent = transform;
		TopLeft.transform.localScale = new Vector3 (.3f, .3f, 1);

/*		GameObject BottomRight = new GameObject ("BottomCollider");
		BottomRight.transform.position = new Vector3 (cameraPos.x + screenSize.x, cameraPos.y - screenSize.y);
		BottomRight.transform.eulerAngles = new Vector3 (0, 0, 45);
		BottomRight.transform.parent = transform;
		BottomRight.transform.localScale = new Vector3 (.3f, .3f, 1);

		GameObject BottomLeft = new GameObject ("BottomCollider");
		BottomLeft.transform.position = new Vector3 (cameraPos.x + screenSize.x, cameraPos.y + screenSize.y);
		BottomLeft.transform.eulerAngles = new Vector3 (0, 0, 45);
		BottomLeft.transform.parent = transform;
		BottomLeft.transform.localScale = new Vector3 (.3f, .3f, 1);*/

		TopRight.AddComponent<BoxCollider2D> ();
		TopLeft.AddComponent<BoxCollider2D> ();
		/*BottomLeft.AddComponent<BoxCollider2D> ();
		BottomRight.AddComponent<BoxCollider2D> ();*/


	}

	void Update ()
	{
		

		//Camera.main.aspect = Screen.currentResolution.width / Screen.currentResolution.height;
	}

}
