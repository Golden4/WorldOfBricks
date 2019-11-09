using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinBlock : BlockWithText {
    protected override void Start () {
        needEffects = true;
    }

    public override void Hit (Ball ball) {
        int coinAmount = 5;
        User.AddCoin (coinAmount);
        if (CoinUI.Ins != null)
            CoinUI.Ins.AddCoin (coinAmount);

        //Vector3 fromPos = Camera.main.WorldToScreenPoint(transform.position);
        //Vector3 toPos = CoinUI.Ins.coinImage.transform.position;

        //Utility.CoinsAnimate(CoinUI.Ins, CoinUI.Ins.coinImage.gameObject, CoinUI.Ins.transform, coinAmount, fromPos, toPos, .5f, CoinUI.Ins.curve, () => {
        //    AudioManager.PlaySoundFromLibrary("Coin");
        //});

        Die ();
    }

    protected override void OnDead () {

        if (needEffects) {
            ShowParticle ();
        }

        Destroy (gameObject);
    }

    protected override void ShowParticle () {
        AudioManager.PlaySoundFromLibrary ("Coin");
        Destroy (Instantiate<GameObject> (destroyParticle.gameObject, transform.position + (Vector3.up - Vector3.left) * .5f, Quaternion.identity), 2);
    }
}