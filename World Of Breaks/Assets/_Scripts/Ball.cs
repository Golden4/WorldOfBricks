using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public static float ballRadius = .1f;
    public static List<Ability> curAbilites = new List<Ability>();
    public LayerMask colliderMask;
    public static Collider2D bottomCollider;

    public bool isThrowing = false;
    public bool isClone;
    public int canReflectionCount;

    public bool bottomCollide;
    public bool isGhost;
    public LayerMask ghostColliderMask;
    Collider2D ghostOldCollider;

    bool waitingForHitBlock;
    ParticleSystem trailParticle;

    public TrailRenderer trailRenderer;
    SpriteRenderer spriteRenderer;

    public enum Ability
    {
        None,
        RandomDirection,
        Speed,
        CoinChance,
        Bounce2Times,
        Clone2Times,
        SkullDestroy,
        DoubleHitBrick,
        HigherChekpoint
    }

    static float lastAudioPlayTime;
    Sound hitSound;

    void Awake()
    {

        if (!isClone)
            oldHit = bottomCollider;
        else
            oldHit = null;
        
        spriteRenderer = GetComponent<SpriteRenderer>();

        hitSound = ItemsInfo.Get.playersData[User.GetInfo.GetCurPlayerIndex()].hitSound;

        trailRenderer = GetComponentInChildren<TrailRenderer>();

        if (trailRenderer != null)
        {
            trailRenderer.widthMultiplier = ballRadius * 2;
            Color origTrailColor = ItemsInfo.Get.playersData[User.GetInfo.curPlayerIndex].trailColor;
            ChangeToOriginal();
        }
    }

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        if (isThrowing)
        {
            transform.position += transform.up * Time.fixedDeltaTime * BallController.Instance.ballSpeed;
            MoveBall();
        }
    }

    public static bool HaveAblity(Ability ability)
    {
        return curAbilites.Contains(ability);
    }

    Collider2D oldHit;

    //Queue<string> lastCollisions = new Queue<string> (6);
    int leftCollCountHit, collCountHit = 0;
    BlockWithText BallSpawned = null;

    void CheckStacking(string name, float angle)
    {
        if (Mathf.Abs(90 - angle) < 2)
        {

            if (name == "LeftCollider" || name == "RightCollider")
            {
                collCountHit++;
            }
            else
            {
                collCountHit = 0;
            }

            if (collCountHit > 6 && BallSpawned == null)
            {
                BallSpawned = BlocksController.Instance.SpawnBlock(new Vector3(0, transform.position.y));
            }

        }
        else
        {
            collCountHit = 0;
            if (BallSpawned != null)
                Destroy(BallSpawned.gameObject);
        }
    }

    void MoveBall()
    {
        if (this == null)
            return;

        LayerMask mask = colliderMask;

        if (isGhost)
        {
            RaycastHit2D defaultHit = Physics2D.Raycast(transform.position - (transform.up * ballRadius), transform.up, ballRadius * 2, colliderMask);

            mask = ghostColliderMask;

            if (defaultHit.collider != null &&
                ghostOldCollider != defaultHit.collider &&
                !defaultHit.collider.isTrigger &&
                !(defaultHit.collider.name == "LeftCollider" || defaultHit.collider.name == "RightCollider" || defaultHit.collider.name == "TopCollider" || defaultHit.collider.name == "BottomCollider"))
            {
                ghostOldCollider = defaultHit.collider;
                waitingForHitBlock = false;
            }

            if (!waitingForHitBlock && defaultHit.collider == null)
            {
                mask = colliderMask;
                ChangeToGhost(false);
            }
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position - (transform.up * ballRadius), transform.up, ballRadius * 2, mask);

        if (hit.collider == null)
            return;

        if (oldHit != hit.collider /*|| hit.collider.CompareTag ("Edge")*/ )
        {
            oldHit = hit.collider;

            if (!hit.collider.isTrigger)
            {
                OnHitColliderBlock(hit);
            }

            if (hit.transform.CompareTag("Block"))
            {
                BlockWithText block = hit.collider.GetComponentInParent<BlockWithText>();

                if (block != null)
                    block.Hit(this);
            }

            if (hit.collider == bottomCollider /*.transform.name == "BottomCollider"*/ )
            {

                OnHitBottomCollider(hit);
            }

        }
    }

    void OnHitColliderBlock(RaycastHit2D hit)
    {
        Vector3 reflectDir = Vector3.Reflect(transform.up, hit.normal.normalized);

        float angle = Quaternion.Angle(Quaternion.Euler(reflectDir), transform.rotation);

        transform.position = hit.point - (Vector2)transform.up * ballRadius;

        CheckStacking(hit.transform.name, angle);

        if (lastAudioPlayTime + .05f < Time.time && hit.transform.name != "BottomCollider")
        {
            lastAudioPlayTime = Time.time;

            if (hitSound.clip == null)
                AudioManager.PlaySoundFromLibrary("Ball1");
            else
                AudioManager.PlaySound(hitSound);
        }

        //print (hit.transform.name + "    " + hit.normal + "    " + reflectDir);

        ChangeDirection(reflectDir);
    }

    void OnHitBottomCollider(RaycastHit2D hit)
    {
        if (canReflectionCount > 0)
        {
            canReflectionCount--;
            bottomCollide = true;
        }
        else
        {
            isThrowing = false;

            if (isClone)
            {
                DestroyClone();
                return;
            }

            ChangeToOriginal();

            if (!BallController.Instance.startPosChanged)
            {

                BallController.Instance.ChangeStartThrowPos(hit.point.x);

                BallController.Instance.startPosChanged = true;
            }
            oldHit = hit.collider;
            ToStartPos();
        }
    }

    public void ChangeDirection(Vector3 rot)
    {
        transform.rotation = Quaternion.FromToRotation(Vector3.up, rot);
    }

    public void ChangeDirection(float eulerAngle)
    {
        transform.localEulerAngles = new Vector3(0, 0, eulerAngle);
    }

    public void GoThrow(Vector3 dir)
    {
        transform.rotation = Quaternion.FromToRotation(Vector3.up, dir + (Vector3.up + Vector3.left) * ((HaveAblity(Ability.RandomDirection)) ? UnityEngine.Random.Range(-.08f, .08f) : 0));
        isThrowing = true;
        // trailParticle.Play();
        // trailParticle.startSpeed = BallController.Instance.ballSpeed / 5f;
    }

    public void ReturnBall(bool animate = true)
    {
        isThrowing = false;

        if (isClone)
        {
            DestroyClone();
            return;
        }

        BallController.Instance.startPosChanged = true;

        ToStartPos(animate);
    }

    public void ToStartPos(bool animate = true)
    {

        if (animate)
        {
            transform.DOMove(BallController.Instance.startThrowPos, .3f);
        }
        // iTween.MoveTo(gameObject, BallController.Instance.startThrowPos, .5f);
        else
        {
            transform.position = BallController.Instance.startThrowPos;
        }

        trailRenderer.Clear();
        trailRenderer.

        // trailParticle.Stop();
        transform.eulerAngles = Vector3.zero;
        oldHit = bottomCollider;
        ChangeToOriginal();
        //transform.position = ballControl.startThrowPos;
    }

    public void ChangeToSecondReflection()
    {
        if (bottomCollide)
            return;

        if (!Ball.HaveAblity(Ability.Bounce2Times))
            canReflectionCount = 1;
        else
            canReflectionCount = 2;

        ChangeColor(new Color(1, 0.5f, 0), new Color(1, 0.5f, 0), .7f);
    }

    public void ChangeToClone()
    {
        isClone = true;
        ChangeColor(Color.red, Color.red, .7f);
    }

    public void ChangeToGhost(bool enable = true)
    {
        if (enable)
        {
            isGhost = true;
            waitingForHitBlock = true;
            ChangeSpriteAlpha(.4f);
        }
        else
        {
            isGhost = false;
            ChangeSpriteAlpha(1);
        }
    }

    public void ChangeToOriginal()
    {
        canReflectionCount = 0;
        isClone = false;
        ChangeToGhost(false);
        bottomCollide = false;
        
        Color origTrailColor = ItemsInfo.Get.playersData[User.GetInfo.curPlayerIndex].trailColor;
        ChangeColor(Color.white, origTrailColor, 0);
        
    }

    public void DestroyClone()
    {
        BallController.Instance.ballsList.Remove(this);
        Destroy(Instantiate<GameObject>(BallController.Instance.cloneDestroyParticle.gameObject, transform.position, Quaternion.identity), 2);
        Destroy(gameObject);
    }

    void ChangeColor(Color color, Color trailColor, float fillAmount)
    {
        spriteRenderer.material.SetColor("_FillColor", color);
        spriteRenderer.material.SetFloat("_FillAlpha", fillAmount);

        trailColor.a = .7f;
        trailRenderer.startColor = trailColor;
        trailColor.a = 0;
        trailRenderer.endColor = trailColor;
    }

    void ChangeSpriteAlpha(float alpha)
    {
        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;

        color = trailRenderer.startColor;
        color.a = .7f * alpha;
        trailRenderer.startColor = color;
        color.a = 0;
        trailRenderer.endColor = color;
    }

}