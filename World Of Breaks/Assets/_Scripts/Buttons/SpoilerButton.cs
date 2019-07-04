using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpoilerButton : MonoBehaviour {

	public GameObject spoilerParent;

	public bool isShow = false;


	void Start ()
	{
		Show ();

		GetComponent <Button> ().onClick.AddListener (() => {

			if (isShow) {
				Close ();
			} else {
				Show ();
			}

		});

		Close ();
	}

    public bool isChangingColor;

	public void Show ()
	{
		isShow = true;
        isChangingColor = GetComponent<ButtonIcon>().changingColor;
        GetComponent<ButtonIcon>().changingColor = false;

        spoilerParent.gameObject.SetActive (true);
		GUIAnimSystem.Instance.MoveIn (spoilerParent.transform, true);
	}

	public void Close ()
	{
        if(isChangingColor && BuyCoinScreen.CanTakeGift())
            GetComponent<ButtonIcon>().changingColor = true;
        else
            GetComponent<ButtonIcon>().changingColor = false;

        isShow = false;
		spoilerParent.gameObject.SetActive (false);
	}

}
