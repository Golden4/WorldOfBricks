using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu ()]
public class ItemsInfo : ScriptableObject {
	public ItemData[] playersData;

	public void OnValidate ()
	{
		/*for (int i = 0; i < playersData.Length; i++) {
			playersData [i].purchaseID = "Ball_" + (i + 1);
		}*/
	}

	[System.Serializable]
	public enum BuyType {
		Coin,
		Video,
		RealMoney
	}

	[System.Serializable]
	public class ItemData {
		public string name;
		public string purchaseID;
		public BuyType buyType;
		public string price;
		//public Ball playerPrefab;
		public Sprite ballSprite;
		public float ballRadius = .1f;
		public Ball.Ability[] abilites;
		public Sound hitSound;

		public string GetDescription ()
		{

			string desc = "";

			desc = LocalizationManager.GetLocalizedText ("ball_size") + ": " + ballRadius * 100 + " cm";

			desc += "\n" + LocalizationManager.GetLocalizedText ("ability") + ": ";

			if (abilites.Length == 0) {
				desc += LocalizationManager.GetLocalizedText (System.Enum.GetName (typeof(Ball.Ability), Ball.Ability.None));
			} else {
				for (int i = 0; i < abilites.Length; i++) {
					desc += "\n- " + LocalizationManager.GetLocalizedText (System.Enum.GetName (typeof(Ball.Ability), abilites [i]));
				}
			}

			desc += "\n" + LocalizationManager.GetLocalizedText (name + "_desc");

			return desc;
		}


	}

}
