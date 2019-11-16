using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeScreenCollisions : MonoBehaviour
{
    public static EdgeScreenCollisions Ins;
    public float colThickness = 4f;
    public float zPosition = 0f;
    private Vector2 screenSize;
    public Sprite edgeSprite;
    //public static Vector2 screenResolution;
    public SpriteRenderer background;
    private void Awake()
    {
        Ins = this;
    }

    void Start()
    {

        //Screen.SetResolution (Mathf.RoundToInt (Screen.height / 16f * 10f), Screen.height, false);

        System.Collections.Generic.Dictionary<string, Transform> colliders = new System.Collections.Generic.Dictionary<string, Transform>();

        colliders.Add("Top", new GameObject().transform);
        colliders.Add("Bottom", new GameObject().transform);
        colliders.Add("Right", new GameObject().transform);
        colliders.Add("Left", new GameObject().transform);

        Vector3 cameraPos = Camera.main.transform.position;

        screenSize = BlocksController.Instance.GetBlockHolerSize() /*+ Vector2.one * .05f*/;

        foreach (KeyValuePair<string, Transform> valPair in colliders)
        {
            valPair.Value.gameObject.AddComponent<BoxCollider2D>();
            valPair.Value.name = valPair.Key + "Collider";
            valPair.Value.parent = transform;
            valPair.Value.gameObject.layer = LayerMask.NameToLayer("Walls");

            if (valPair.Key == "Left" || valPair.Key == "Right")
                valPair.Value.localScale = new Vector3(colThickness, screenSize.y * 2 + colThickness * 2, colThickness);
            else
                valPair.Value.localScale = new Vector3(screenSize.x * 2, colThickness, colThickness);
        }

        colliders["Right"].position = new Vector3(cameraPos.x + screenSize.x + (colliders["Right"].localScale.x * 0.5f), cameraPos.y, zPosition);
        colliders["Left"].position = new Vector3(cameraPos.x - screenSize.x - (colliders["Left"].localScale.x * 0.5f), cameraPos.y, zPosition);
        colliders["Top"].position = new Vector3(cameraPos.x, cameraPos.y + screenSize.y + (colliders["Top"].localScale.y * 0.5f), zPosition);
        colliders["Bottom"].position = new Vector3(cameraPos.x, cameraPos.y - screenSize.y - (colliders["Bottom"].localScale.y * 0.5f), zPosition);

        Ball.bottomCollider = colliders["Bottom"].GetComponent<Collider2D>();
        // Camera.main.orthographicSize = cameraPos.y + screenSize.y + (colliders["Top"].localScale.y * 0.5f);

        ResizeBackground();


        /*		GameObject TopRight = new GameObject ("TopRight");
                TopRight.transform.position = new Vector3 (cameraPos.x + screenSize.x + .1f, cameraPos.y + screenSize.y + .1f);
                TopRight.transform.eulerAngles = new Vector3 (0, 0, 45);
                TopRight.transform.parent = transform;
                TopRight.transform.localScale = new Vector3 (.4f, .8f, 1);
                TopRight.tag = "Edge";

                GameObject TopLeft = new GameObject ("TopLeft");
                TopLeft.transform.position = new Vector3 (cameraPos.x - screenSize.x - .1f, cameraPos.y + screenSize.y + .1f);
                TopLeft.transform.eulerAngles = new Vector3 (0, 0, 45);
                TopLeft.transform.parent = transform;
                TopLeft.transform.localScale = new Vector3 (.8f, .4f, 1);
                TopLeft.tag = "Edge";*/
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

        /*		CircleCollider2D cc = TopRight.AddComponent<CircleCollider2D> ();
                cc.radius = .3f;
                cc = TopLeft.AddComponent<CircleCollider2D> ();
                cc.radius = .3f;*/
        /*BottomLeft.AddComponent<BoxCollider2D> ();
		BottomRight.AddComponent<BoxCollider2D> ();*/

    }

    void ResizeBackground()
    {
        Vector2 screenSizeBG = default;
        screenSizeBG.x = Vector2.Distance(Camera.main.ScreenToWorldPoint(new Vector2(0, 0)), Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)));
        screenSizeBG.y = Vector2.Distance(Camera.main.ScreenToWorldPoint(new Vector2(0, 0)), Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height)));
        background.transform.localScale = screenSizeBG * 100;
    }

    void ShowEdges(System.Collections.Generic.Dictionary<string, Transform> colliders)
    {
        foreach (KeyValuePair<string, Transform> valPair in colliders)
        {
            if (valPair.Key != "Bottom")
                valPair.Value.tag = "Edge";

            GameObject go = new GameObject("EdgeSprite" + valPair.Key);
            go.transform.SetParent(colliders[valPair.Key].transform, false);
            go.transform.localScale = Vector3.one;
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = edgeSprite;
            sr.drawMode = SpriteDrawMode.Sliced;

            if (valPair.Key == "Left" || valPair.Key == "Right")
            {
                sr.size = new Vector2(.015f, .655f);
                if (valPair.Key == "Left")
                {

                    go.transform.localPosition = new Vector3(.4925f, 0);
                }
                else
                {
                    go.transform.localPosition = new Vector3(-.4925f, 0);
                }
            }
            else
            {

                sr.size = new Vector2(1f, .05f);
                if (valPair.Key == "Top")
                {
                    go.transform.localPosition = new Vector3(0, -.475f);
                }
                else
                {
                    go.transform.localPosition = new Vector3(0, .475f);
                }

            }


        }
    }

    public static Vector2 GetScreenEdgeTop()
    {
        Vector3 cameraPos = Camera.main.transform.position;
        return new Vector2(cameraPos.x, cameraPos.y + BlocksController.Instance.GetBlockHolerSize().y);
    }

    public static Vector2 GetScreenEdgeBottom()
    {
        Vector3 cameraPos = Camera.main.transform.position;
        return new Vector2(cameraPos.x, cameraPos.y - BlocksController.Instance.GetBlockHolerSize().y);
    }

    public static Vector2 GetScreenEdgeLeft()
    {
        Vector3 cameraPos = Camera.main.transform.position;
        return new Vector2(cameraPos.x - BlocksController.Instance.GetBlockHolerSize().x, cameraPos.y);
    }

    public static Vector2 GetScreenEdgeRight()
    {
        Vector3 cameraPos = Camera.main.transform.position;
        return new Vector2(cameraPos.x + BlocksController.Instance.GetBlockHolerSize().x, cameraPos.y);
    }

    void Update()
    {


        //Camera.main.aspect = Screen.currentResolution.width / Screen.currentResolution.height;
    }

}
