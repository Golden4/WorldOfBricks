using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrajectoryHelper : Helper
{
    public SpriteRenderer throwingDirectionImage;

    public List<SpriteRenderer> thrDirImages = new List<SpriteRenderer>();
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
            SpriteRenderer newThrDir = Instantiate<SpriteRenderer>(throwingDirectionImage);
            thrDirImages.Add(newThrDir);
        }

        ShowTajectory(false);

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

        float heigth = dirMouse.magnitude;

        ShowTajectory(false);

         iter = 0;
        thrDirImages[0].gameObject.SetActive(dirMouse.sqrMagnitude > 1000);

        if (dirMouse.sqrMagnitude > 1000)
        {
            SetTrajectory(thrDirImages[0], pos, dirMouse.normalized, ref heigth);
        }
    }

    int iter = 0;

    public void SetTrajectory(SpriteRenderer throwingDirectionImage,Vector2 startPos, Vector2 dir, ref float height)
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
            
        } else
        {
            distance = height;
        }

            Vector2 heigth = throwingDirectionImage.size;
            heigth.y = distance;
            throwingDirectionImage.size = heigth;

            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, dir);
            
            throwingDirectionImage.transform.rotation = rotation;

            throwingDirectionImage.transform.position = (Vector2)startPosWorld;

            height -= distance;
            
            if(height > 0 && iter < 4 && hit.collider != null && !hit.collider.isTrigger && hit.collider.name != "BottomCollider")
            {
                throwingDirectionImage.transform.GetChild(0).gameObject.SetActive(false);
                iter++;
                thrDirImages[iter].gameObject.SetActive(true);
                thrDirImages[iter].transform.SetParent(throwingDirectionImage.transform.parent,false);
                SetTrajectory(thrDirImages[iter], Camera.main.WorldToScreenPoint(hit.point - dir * Ball.ballRadius/2f), reflectDir.normalized, ref height);
            } else
            {
                throwingDirectionImage.transform.GetChild(0).gameObject.SetActive(true);
                Vector3 pos = throwingDirectionImage.transform.GetChild(0).transform.localPosition;
                pos.y = height;
                throwingDirectionImage.transform.GetChild(0).transform.localPosition = pos;
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
