using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BlockPlusBall : BlockWithText
{

    protected override void Start()
    {
        textMesh = GetComponentInChildren<TextMesh>();
        textMesh.gameObject.SetActive(false);
    }

    public override void Hit()
    {
        AudioManager.PlaySoundFromLibrary("Plus");
        BallController.Instance.BallCountPlus();
        Die();
    }

    protected override void OnDead()
    {
        if (!justDestroy)
        {
            textMesh.transform.SetParent(null, false);
            textMesh.transform.position = transform.position + (Vector3.up - Vector3.left) * .3f;
            textMesh.gameObject.SetActive(true);
            // iTween.MoveTo (textMesh.gameObject, textMesh.transform.position + Vector3.up * .3f, 1f);
            textMesh.transform.DOScale(Vector2.one, 1f).ChangeStartValue(Vector3.zero).SetEase(Ease.OutElastic);
            textMesh.transform.DOScale(Vector2.one, .2f).From().ChangeEndValue(Vector3.zero).SetDelay(1f);
            // iTween.FadeTo(textMesh.gameObject, 0, .5f);
            Destroy(textMesh.gameObject, 1.5f);
        }

        Destroy(gameObject);

    }
}
