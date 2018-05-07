////////////////////////////////////////////////////////////////////////////////
//  
// @module Android Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////
 
using UnityEngine;
using System.Collections;

public class GameBillingManagerExample : MonoBehaviour {

	private static bool _isInited = false;
	
	//--------------------------------------
	//  INITIALIZE
	//--------------------------------------


	//replace with your consumable item
	public const string COINS_ITEM = "small_coins_bag";

	//replace with your non-consumable item
	public const string COINS_BOOST = "coins_bonus";


	//public const string base64EncodedPublicKey = "REPLACE_WITH_YOUR_PUBLIC_KEY";
	public const string base64EncodedPublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAu3lQAtgM0jdQzWyFQfuIepx/tMrqHv03o15zZBajEJ4/VGxbuW2A4fpPFevYh23xdnAVGVdUg++wsHRb1KAfWNGzSTHWaza3hqWmyC8zvvsHS/mweP0I9nXJUygJhPq/z1Rnv3s2mzC6VIvDseVcKQJDjsqFzTw+tiB5sq7xoD/9dZER35J70R2HJHuMZR8/yGoOjObo2dPalSW3v/kPbwWtVoVRYb0e/DhXUi8Lcb+PlC7oFmuaoEycvGCsybdc9bVGeQDVcrFHHonnwPZ/JRwZEdAGXTJQqxm2tjhU4eBFEM12/5FNRI5OgjB4uTBLN4oOLftAjBTnzTi7RqzECwIDAQAB";
	
	
	
	public static void init() {
		
		
		//Filling product list
		AndroidInAppPurchaseManager.instance.addProduct(COINS_ITEM);
		AndroidInAppPurchaseManager.instance.addProduct(COINS_BOOST);

		
		//listening for purchase and consume events
		AndroidInAppPurchaseManager.instance.addEventListener (AndroidInAppPurchaseManager.ON_PRODUCT_PURCHASED, OnProductPurchased);
		AndroidInAppPurchaseManager.instance.addEventListener (AndroidInAppPurchaseManager.ON_PRODUCT_CONSUMED,  OnProductConsumed);
		
		//initilaizing store
		AndroidInAppPurchaseManager.instance.addEventListener (AndroidInAppPurchaseManager.ON_BILLING_SETUP_FINISHED, OnBillingConnected);
		AndroidInAppPurchaseManager.instance.loadStore(base64EncodedPublicKey);
		
		
		
	}
	
	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------
	
	
	public static void purchase(string SKU) {
		AndroidInAppPurchaseManager.instance.purchase (SKU);
	}
	
	public static void consume(string SKU) {
		AndroidInAppPurchaseManager.instance.consume (SKU);
	}
	
	//--------------------------------------
	//  GET / SET
	//--------------------------------------
	
	public static bool isInited {
		get {
			return _isInited;
		}
	}
	
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------
	
	private static void OnProcessingPurchasedProduct(GooglePurchaseTemplate purchase) {
		//some stuff for processing product purchse. Add coins, unlock track, etc

		switch(purchase.SKU) {
		case COINS_ITEM:
			consume(COINS_ITEM);
			break;
		case COINS_BOOST:
			GameDataExample.EnableCoinsBoost();
			break;
		}
	}
	
	private static void OnProcessingConsumeProduct(GooglePurchaseTemplate purchase) {
		switch(purchase.SKU) {
		case COINS_ITEM:
			GameDataExample.AddCoins(100);
			break;
		}
	}
	
	private static void OnProductPurchased(CEvent e) {
		BillingResult result = e.data as BillingResult;
		
		
		if(result.isSuccess) {
			OnProcessingPurchasedProduct (result.purchase);
		} else {
			AndroidMessage.Create("Product Purchase Failed", result.response.ToString() + " " + result.message);
		}
		
		Debug.Log ("Purchased Responce: " + result.response.ToString() + " " + result.message);
	}
	
	
	private static void OnProductConsumed(CEvent e) {
		BillingResult result = e.data as BillingResult;
		
		if(result.isSuccess) {
			OnProcessingConsumeProduct (result.purchase);
		} else {
			AndroidMessage.Create("Product Cousume Failed", result.response.ToString() + " " + result.message);
		}
		
		Debug.Log ("Cousume Responce: " + result.response.ToString() + " " + result.message);
	}
	
	
	private static void OnBillingConnected(CEvent e) {
		BillingResult result = e.data as BillingResult;
		AndroidInAppPurchaseManager.instance.removeEventListener (AndroidInAppPurchaseManager.ON_BILLING_SETUP_FINISHED, OnBillingConnected);
		
		
		if(result.isSuccess) {
			//Store connection is Successful. Next we loading product and customer purchasing details
			AndroidInAppPurchaseManager.instance.addEventListener (AndroidInAppPurchaseManager.ON_RETRIEVE_PRODUC_FINISHED, OnRetriveProductsFinised);
			AndroidInAppPurchaseManager.instance.retrieveProducDetails();

		} 
		
		AndroidMessage.Create("Connection Responce", result.response.ToString() + " " + result.message);
		Debug.Log ("Connection Responce: " + result.response.ToString() + " " + result.message);
	}
	
	
	
	
	private static void OnRetriveProductsFinised(CEvent e) {
		BillingResult result = e.data as BillingResult;
		AndroidInAppPurchaseManager.instance.removeEventListener (AndroidInAppPurchaseManager.ON_RETRIEVE_PRODUC_FINISHED, OnRetriveProductsFinised);
		
		if(result.isSuccess) {

			UpdateStoreData();
			_isInited = true;


		} else {
			AndroidMessage.Create("Connection Responce", result.response.ToString() + " " + result.message);
		}

	}



	private static void UpdateStoreData() {
		//chisking if we already own some consuamble product but forget to consume those
		if(AndroidInAppPurchaseManager.instance.inventory.IsProductPurchased(COINS_ITEM)) {
			consume(COINS_ITEM);
		}

		//Check if non-consumable rpduct was purchased, but we do not have local data for it.
		//It can heppens if game was reinstalled or download on oher device
		//This is replacment for restore purchase fnunctionality on IOS


		if(AndroidInAppPurchaseManager.instance.inventory.IsProductPurchased(COINS_BOOST)) {
			GameDataExample.EnableCoinsBoost();
		}


	}
	
}
