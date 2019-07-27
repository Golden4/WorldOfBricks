using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu ()]
public class ItemsInfo : ScriptableObject {
	public ItemData[] playersData;

	public int[] abilityPrices;

	public void OnValidate ()
	{
		/*for (int i = 0; i < playersData.Length; i++) {
			playersData [i].purchaseID = "Ball_" + (i + 1);
		}*/

		for (int i = 0; i < playersData.Length; i++) {
			int priceWithAbility = Mathf.RoundToInt (50f / (playersData [i].ballRadius * 10));
			int abilityCount = 0;
			for (int j = 0; j < playersData [i].abilites.Length; j++) {
				priceWithAbility += abilityPrices [(int)playersData [i].abilites [j]];
				abilityCount++;
			}

			priceWithAbility = Mathf.RoundToInt (priceWithAbility * ((abilityCount * 0.2f) + 1f) / 10f) * 10;

			playersData [i].price = priceWithAbility.ToString ();
		}
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
