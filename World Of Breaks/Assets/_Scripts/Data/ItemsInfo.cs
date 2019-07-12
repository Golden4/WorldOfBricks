using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu ()]
public class ItemsInfo : ScriptableObject {
	public ItemData[] playersData;

	[System.Serializable]
	public enum BuyType {
		Coin,
		RealMoney,
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
	}

}
