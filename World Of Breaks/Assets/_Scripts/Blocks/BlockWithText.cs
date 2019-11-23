using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BlockWithText : MonoBehaviour
{

    public bool active;

    public int coordsX;

    public int coordsY;

    protected SpriteRenderer spriteRenderer;

    protected TextMesh textMesh;

    float curColorValue;

    public ParticleSystem destroyParticle;

    public bool canLooseDown;
    [System.NonSerialized]
    public bool isDead;
    [System.NonSerialized]
    public bool isLoadingBlock;

    protected virtual void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        textMesh = GetComponentInChildren<TextMesh>();
        UpdateText();

        ChangeSpriteColor((BlocksController.Instance.blockMap[coordsY][coordsX].blockLife % 50) / 50f);

        needEffects = true;

        if (needEffects)
        {
            float dissolveAmount = 0;

            DOTween.To(() => dissolveAmount, (x) =>
            {
                spriteRenderer.material.SetFloat("_DissolveAmount", 1 - x);

                Color color = textMesh.color;
                color.a = x;
                textMesh.color = color;

            }, 1f, 1f);

            Color myColor = textMesh.color;
            myColor.a = 0;
            textMesh.color = myColor;

            spriteRenderer.material.SetFloat("_DissolveAmount", 1f);
        }

    }

    public virtual void Hit(Ball ball)
    {
        Hit();
    }

    protected float t = 0;
    protected float timerSpeed = 6;

    void Update()
    {

        if (t > 0)
        {
            t -= Time.deltaTime * timerSpeed;

            TimerStart();

        }
        else if (t <= -0.01f)
        {
            t = 0;

            TimerEnd();
        }
    }

    Color colorToLerp;

    protected virtual void TimerStart()
    {
        spriteRenderer.material.SetColor("_BlendColor", colorToLerp);
        spriteRenderer.material.SetFloat("_FillAmount", Mathf.Lerp(.5f, 1f, 1f - t));
    }

    protected virtual void TimerEnd()
    {
        spriteRenderer.material.SetColor("_BlendColor", colorToLerp);
        spriteRenderer.material.SetFloat("_FillAmount", 1f);
    }

    protected virtual void ChangeSpriteColor(float value)
    {
        spriteRenderer.material.SetColor("_TopColor", BlocksController.Instance.GetGradientColor(true, value));
        spriteRenderer.material.SetColor("_BottomColor", BlocksController.Instance.GetGradientColor(false, value));
    }

    public virtual void Hit()
    {
        int hitAmount = 1;

        colorToLerp = BlocksController.Instance.colorOnHit;

        if (Ball.HaveAblity(Ball.Ability.DoubleHitBrick))
        {
            if (Random.Range(0, 10) == 0)
            {
                hitAmount = 2;
                colorToLerp = BlocksController.Instance.colorOnHitAbility;
            }
        }

        int blockLifeAfterHit = BlocksController.Instance.blockMap[coordsY][coordsX].blockLife - hitAmount;

        BlocksController.Instance.blockMap[coordsY][coordsX].blockLife = (blockLifeAfterHit < 0) ? 0 : blockLifeAfterHit;

        if (BlocksController.Instance.blockMap[coordsY][coordsX].blockLife <= 0)
        {
            Die();
            return;
        }

        t = 1;

        ChangeSpriteColor((BlocksController.Instance.blockMap[coordsY][coordsX].blockLife % 50) / 50f);

        UpdateText();
    }

    public virtual void UpdateText()
    {
        textMesh.text = BlocksController.Instance.blockMap[coordsY][coordsX].blockLife.ToString();
        //spriteRenderer.color = g.Evaluate (BlocksController.Instance.blockMap [coordsY] [coordsX].blockLife / 100);
    }
    [System.NonSerialized]
    public bool justDestroy;
    [System.NonSerialized]
    public bool needEffects;

    public void Die()
    {

        if (isDead)
            return;
        isDead = true;

        BlocksController.Instance.blockMap[coordsY][coordsX].blockLife = 0;

        BlocksController.Instance.blockMap[coordsY][coordsX].blockIndex = -1;

        OnDead();

    }

    protected virtual void OnDead()
    {
        if (!justDestroy)
        {
            AddScoreWithText();
        }

        if (needEffects)
        {
            ShowParticle();
        }

        Destroy(gameObject);

    }

    protected void AddScoreWithText()
    {

        BlocksController.Instance.blockDestroyCount++;

        int multiplier;

        if (!Game.isChallenge)
            multiplier = UIScreen.Ins.multiplyer;
        else
            multiplier = 10;

        int scoreToAdd = BlocksController.Instance.blockDestroyCount * multiplier;

        if (textMesh != null)
        {
            textMesh.transform.SetParent(null, false);
            textMesh.transform.position = transform.position + (Vector3.up - Vector3.left) * .3f + Vector3.back;
            textMesh.transform.localEulerAngles = Vector3.zero;
            textMesh.transform.localScale = Vector3.one;
            textMesh.gameObject.SetActive(true);
            textMesh.text = "+" + scoreToAdd;
            // textMesh.color = Color.yellow;
            textMesh.fontSize = 40;

            // iTween.FadeTo(textMesh.gameObject, 0, 1f);

            textMesh.transform.DOScale(Vector2.one, 1f).ChangeStartValue(Vector3.zero).SetEase(Ease.OutElastic);
            textMesh.transform.DOScale(Vector2.one, .2f).From().ChangeEndValue(Vector3.zero).SetDelay(1f);

            Destroy(textMesh.gameObject, 1.4f);
        }

        UIScreen.Ins.AddPlayerScore(scoreToAdd);
        BlocksController.Instance.CalculateBlockLife();
    }

    protected virtual void OnDestroy()
    {

    }

    protected virtual void ShowParticle()
    {
        AudioManager.PlaySoundFromLibrary("Destroy");
        Destroy(Instantiate<GameObject>(destroyParticle.gameObject, transform.position + (Vector3.up - Vector3.left) * .5f, Quaternion.identity), 2);
    }

}