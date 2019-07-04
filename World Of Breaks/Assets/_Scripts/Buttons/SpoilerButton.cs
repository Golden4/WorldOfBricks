﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpoilerButton : MonoBehaviour {

	public GameObject spoilerParent;
    public GUIAnim[] anims;
	public bool isShow = false;


	IEnumerator Start ()
	{
		
        Button btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener (() => {

			if (isShow) {
				Close ();
			} else {
				Show ();
			}

		});

        Show();
        Close ();
        yield return new WaitForSeconds(.2f);
        btn.interactable = true;
    }

    public bool isChangingColor;

	public void Show ()
	{
        if (isShow)
            return;

		isShow = true;
        isChangingColor = GetComponent<ButtonIcon>().changingColor;
        GetComponent<ButtonIcon>().changingColor = false;
        spoilerParent.gameObject.SetActive (true);

        for (int i = 0; i < anims.Length; i++)
        {
            anims[i].MoveIn(GUIAnimSystem.eGUIMove.Self);
        }
	}

	public void Close ()
	{
        if (!isShow)
            return;

        if (isChangingColor && BuyCoinScreen.CanTakeGift())
            GetComponent<ButtonIcon>().changingColor = true;
        else
            GetComponent<ButtonIcon>().changingColor = false;

        isShow = false;
		spoilerParent.gameObject.SetActive (false);
	}

}
