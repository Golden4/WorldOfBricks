﻿using System;
using UnityEngine;
using UnityEngine.Purchasing;
using System.Collections.Generic;

public class PurchaseManager : SingletonResourse<PurchaseManager> , IStoreListener {
	private static IStoreController m_StoreController;
	private static IExtensionProvider m_StoreExtensionProvider;
	private int currentProductIndex;

	public string[] NC_PRODUCTS;

	public string[] C_PRODUCTS;

	/// <summary>
	/// Событие, которое запускается при удачной покупке многоразового товара.
	/// </summary>
	public static event OnSuccessConsumable OnPurchaseConsumable;
	/// <summary>
	/// Событие, которое запускается при удачной покупке не многоразового товара.
	/// </summary>
	public static event OnSuccessNonConsumable OnPurchaseNonConsumable;
	/// <summary>
	/// Событие, которое запускается при неудачной покупке какого-либо товара.
	/// </summary>
	public static event OnFailedPurchase PurchaseFailed;

	public override void OnInit ()
	{
		TryInit ();
	}

	/// <summary>
	/// Проверить, куплен ли товар.
	/// </summary>
	/// <param name="id">Индекс товара в списке.</param>
	/// <returns></returns>
	public static bool CheckBuyState (string id)
	{
		Product product = m_StoreController.products.WithID (id);
		if (product.hasReceipt) {
			return true;
		} else {
			return false;
		}
	}

	public void TryInit ()
	{
		if (!IsInitialized ()) {
			InitializePurchasing ();
			DontDestroyOnLoad (gameObject);
		}
	}

	public void InitializePurchasing ()
	{
		List<string> strs = new List<string> ();
		for (int i = 0; i < Database.Get.playersData.Length; i++) {
			strs.Add (Database.Get.playersData [i].purchaseID);
		}

		NC_PRODUCTS = strs.ToArray ();


		C_PRODUCTS = new string[] { "coins_1", "coins_2", "coins_3" };


		var builder = ConfigurationBuilder.Instance (StandardPurchasingModule.Instance ());

		foreach (string s in C_PRODUCTS)
			builder.AddProduct (s, ProductType.Consumable);
		
		foreach (string s in NC_PRODUCTS)
			builder.AddProduct (s, ProductType.NonConsumable);
		
		UnityPurchasing.Initialize (this, builder);
	}

	public bool IsInitialized ()
	{
		return m_StoreController != null && m_StoreExtensionProvider != null;
	}

	public void BuyConsumable (int index)
	{
		currentProductIndex = index;
		BuyProductID (C_PRODUCTS [index]);
	}

	public void BuyConsumable (string id)
	{
		TryInit ();
		List<string> temp = new List<string> (C_PRODUCTS);

		if (temp.Contains (id)) {
			int index = temp.IndexOf (id);
			currentProductIndex = index;
			BuyProductID (C_PRODUCTS [index]);
		}
	}

	public void BuyNonConsumable (int index)
	{

		TryInit ();

		for (int i = 0; i < NC_PRODUCTS.Length; i++) {
			if (Database.Get.playersData [index].purchaseID == NC_PRODUCTS [i]) {
				currentProductIndex = i;
			}
		}

		BuyProductID (NC_PRODUCTS [currentProductIndex]);
	}

	public string GetLocalizedPrice (string productId)
	{
		if (IsInitialized ()) {
			return m_StoreController.products.WithID (productId).metadata.localizedPriceString;
		} else {
			Debug.Log ("Not Initialized - GetLocalizedPrice");
			return "null";
		}
	}

	void BuyProductID (string productId)
	{
		if (IsInitialized ()) {
			Product product = m_StoreController.products.WithID (productId);

			if (product != null && product.availableToPurchase) {
				Debug.Log (string.Format ("Purchasing product asychronously: '{0}'", product.definition.id));
				m_StoreController.InitiatePurchase (product);
			} else {
				Debug.Log ("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
				OnPurchaseFailed (product, PurchaseFailureReason.ProductUnavailable);
			}
		}
	}

	public void OnInitialized (IStoreController controller, IExtensionProvider extensions)
	{
		m_StoreController = controller;
		m_StoreExtensionProvider = extensions;
	}

	public void OnInitializeFailed (InitializationFailureReason error)
	{
		Debug.Log ("OnInitializeFailed InitializationFailureReason:" + error);
	}

	public PurchaseProcessingResult ProcessPurchase (PurchaseEventArgs args)
	{
		if (C_PRODUCTS.Length > 0 && currentProductIndex < C_PRODUCTS.Length && String.Equals (args.purchasedProduct.definition.id, C_PRODUCTS [currentProductIndex], StringComparison.Ordinal))
			OnSuccessC (args);
		else if (NC_PRODUCTS.Length > 0 && String.Equals (args.purchasedProduct.definition.id, NC_PRODUCTS [currentProductIndex], StringComparison.Ordinal))
				OnSuccessNC (args);
			else
				Debug.Log (string.Format ("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
		return PurchaseProcessingResult.Complete;
	}

	public delegate void OnSuccessConsumable (PurchaseEventArgs args);

	protected virtual void OnSuccessC (PurchaseEventArgs args)
	{
		if (OnPurchaseConsumable != null)
			OnPurchaseConsumable (args);
		Debug.Log (C_PRODUCTS [currentProductIndex] + " Buyed!");
	}

	public delegate void OnSuccessNonConsumable (PurchaseEventArgs args);

	protected virtual void OnSuccessNC (PurchaseEventArgs args)
	{
		if (OnPurchaseNonConsumable != null)
			OnPurchaseNonConsumable (args);
		Debug.Log (NC_PRODUCTS [currentProductIndex] + " Buyed!");
	}

	public delegate void OnFailedPurchase (Product product, PurchaseFailureReason failureReason);

	protected virtual void OnFailedP (Product product, PurchaseFailureReason failureReason)
	{
		if (PurchaseFailed != null)
			PurchaseFailed (product, failureReason);
		Debug.Log (string.Format ("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
	}

	public void OnPurchaseFailed (Product product, PurchaseFailureReason failureReason)
	{
		OnFailedP (product, failureReason);
	}
}