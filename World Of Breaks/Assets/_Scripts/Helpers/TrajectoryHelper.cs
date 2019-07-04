using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrajectoryHelper : Helper
{
    public Image throwingDirectionImage;

    public List<Image> thrDirImages = new List<Image>();
    public Button trajectoryBtn;

    public LayerMask colliderMask;
    public static TrajectoryHelper Ins;

    private void Awake()
    {
        Ins = this;
    }

    private void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            Image newThrDir = Instantiate<Image>(throwingDirectionImage);
            thrDirImages.Add(newThrDir);
        }

        trajectoryBtn.GetComponent<ButtonIcon>().EnableBtn(true);

        trajectoryBtn.onClick.RemoveAllListeners();
        trajectoryBtn.onClick.AddListener(() =>
        {
            if (User.BuyWithCoin(50))
            {
                isActive = true;
                trajectoryBtn.GetComponent<ButtonIcon>().EnableBtn(false);
            }
        });
    }

    private void Update()
    {

    }

    public void CalculateTrajectory(Vector2 startThrowPos, Vector2 dirMouse)
    {
        
        Vector2 pos = Camera.main.WorldToScreenPoint(startThrowPos);

        float heigth = dirMouse.magnitude*3;

        ShowTajectory(false);

         iter = 0;

        SetTrajectory(throwingDirectionImage,pos, dirMouse.normalized, ref heigth);
    }

    int iter = 0;

    public void SetTrajectory(Image throwingDirectionImage,Vector2 startPos, Vector2 dir, ref float height)
    {
        Vector3 startPosWorld = Camera.main.ScreenToWorldPoint(startPos + dir);
        Vector3 endPosWorld = Camera.main.ScreenToWorldPoint(startPos + dir*height);

        RaycastHit2D hit = Physics2D.Raycast(startPosWorld, (endPosWorld - startPosWorld).normalized, (endPosWorld - startPosWorld).magnitude, colliderMask);

        float distance;
        Vector3 reflectDir = Vector3.zero;
        if (hit.collider != null)
        {
            reflectDir = Vector3.Reflect(dir, hit.normal.normalized);

            distance = Mathf.Abs( Vector2.Distance(hit.point, startPosWorld));
            Debug.Log(distance);
            distance *= 88f;
            Debug.Log(distance);
        } else {         
            distance = height;
        }

            Vector2 heigth = throwingDirectionImage.rectTransform.sizeDelta;
            heigth.y = distance;
            throwingDirectionImage.rectTransform.sizeDelta = heigth;

            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, dir);
            
            throwingDirectionImage.transform.rotation = rotation;

            throwingDirectionImage.transform.position = startPos;
            height -= distance;
            
            if(height > 0 && iter < 5 && hit.collider != null && !hit.collider.isTrigger && hit.collider.name != "BottomCollider")
            {
            throwingDirectionImage.transform.GetChild(0).gameObject.SetActive(false);
            iter++;
            thrDirImages[iter-1].gameObject.SetActive(true);
            thrDirImages[iter - 1].transform.SetParent(throwingDirectionImage.transform.parent,false);
            SetTrajectory(thrDirImages[iter - 1], Camera.main.WorldToScreenPoint(hit.point - dir * Ball.ballRadius/2f), reflectDir.normalized, ref height);
            } else
            {
                throwingDirectionImage.transform.GetChild(0).gameObject.SetActive(true);
            }
        }

    public void ShowTajectory(bool show)
    {
        for (int i = 0; i < 5; i++)
        {

            thrDirImages[i].gameObject.SetActive(show);
        }
    }
    
}
