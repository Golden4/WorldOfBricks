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
		Video
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
		public Sound hitSound;
	}

}
