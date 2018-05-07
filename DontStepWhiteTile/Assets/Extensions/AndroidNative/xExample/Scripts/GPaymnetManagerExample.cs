////////////////////////////////////////////////////////////////////////////////
//  
// @module Android Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////


 

using UnityEngine;
using System.Collections;

public class GPaymnetManagerExample : MonoBehaviour {

	private static bool _isInited = false;

	//--------------------------------------
	//  INITIALIZE
	//--------------------------------------
	

	public const string ANDROID_TEST_PURCHASED = "android.test.purchased";

	public const string ANDROID_TEST_CANCELED = "android.test.canceled";
	public const string ANDROID_TEST_REFUNDED = "android.test.refunded";
	public const string ANDROID_TEST_ITEM_UNAVALIABLE = "android.test.item_unavailable";

	//example
	//public const string base64EncodedPublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAu3lQAtgM0jdQzWyFQfuIepx/tMrqHv03o15zZBajEJ4/VGxbuW2A4fpPFevYh23xdnAVGVdUg++wsHRb1KAfWNGzSTHWaza3hqWmyC8zvvsHS/mweP0I9nXJUygJhPq/z1Rnv3s2mzC6VIvDseVcKQJDjsqFzTw+tiB5sq7xoD/9dZER35J70R2HJHuMZR8/yGoOjObo2dPalSW3v/kPbwWtVoVRYb0e/DhXUi8Lcb+PlC7oFmuaoEycvGCsybdc9bVGeQDVcrFHHonnwPZ/JRwZEdAGXTJQqxm2tjhU4eBFEM12/5FNRI5OgjB4uTBLN4oOLftAjBTnzTi7RqzECwIDAQAB";
	//public const string base64EncodedPublicKey = "REPLACE_WITH_YOUR_PUBLIC_KEY";
	public const string base64EncodedPublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAsV676BTvO5djSDdUwotbLCIPtGZ5OVCbIn402RXuEpDwuHZMIOy5E6DQjUlQPKCiB7A1Vx+ePQI50Gk8NO1zuPRBgCgvW/oTTf863KkF34QLZD+Ii8fc6VE0UKp3GfApnLmq2qtr1fwDmRCteBUET1h0EcRn3/6R/BA5DMmF1aTv8yUY5LQETWqEPIjGdyNaAhmnWf2sTliYLANiR51WXsfbDdCNT4Ux3gQo/XJynGadfwRS7A9N9e5SgvMEFUR6EwnANOF9QXgE2d0HEitpS56D3uHH/2LwICrTWAmbLX3qPYlQ3Ncf1SRyjqiKae2wW8QUnDFU5BSozwGW6tcQvQIDAQAB";



	public static void init() {


		//Filling product list
		AndroidInAppPurchaseManager.instance.addProduct(ANDROID_TEST_PURCHASED);
		AndroidInAppPurchaseManager.instance.addProduct(ANDROID_TEST_CANCELED);
		AndroidInAppPurchaseManager.instance.addProduct(ANDROID_TEST_REFUNDED);
		AndroidInAppPurchaseManager.instance.addProduct(ANDROID_TEST_ITEM_UNAVALIABLE);


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
	}

	private static void OnProcessingConsumeProduct(GooglePurchaseTemplate purchase) {
		//some stuff for processing product consume. Reduse tip anount, reduse gold token, etc
	}

	private static void OnProductPurchased(CEvent e) {
		BillingResult result = e.data as BillingResult;


		if(result.isSuccess) {
			AndroidMessage.Create ("Product Purchased", result.purchase.SKU);
			OnProcessingPurchasedProduct (result.purchase);
		} else {
			AndroidMessage.Create("Product Purchase Failed", result.response.ToString() + " " + result.message);
		}

		Debug.Log ("Purchased Responce: " + result.response.ToString() + " " + result.message);
	}


	private static void OnProductConsumed(CEvent e) {
		BillingResult result = e.data as BillingResult;

		if(result.isSuccess) {
			AndroidMessage.Create ("Product Consumed", result.purchase.SKU);
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
			AndroidInAppPurchaseManager.instance.retrieveProducDetails();
			AndroidInAppPurchaseManager.instance.addEventListener (AndroidInAppPurchaseManager.ON_RETRIEVE_PRODUC_FINISHED, OnRetriveProductsFinised);
		} 

		AndroidMessage.Create("Connection Responce", result.response.ToString() + " " + result.message);
		Debug.Log ("Connection Responce: " + result.response.ToString() + " " + result.message);
	}




	private static void OnRetriveProductsFinised(CEvent e) {
		BillingResult result = e.data as BillingResult;
		AndroidInAppPurchaseManager.instance.removeEventListener (AndroidInAppPurchaseManager.ON_RETRIEVE_PRODUC_FINISHED, OnRetriveProductsFinised);

		if(result.isSuccess) {
			_isInited = true;
			AndroidMessage.Create("Success", "Billing init complete inventory contains: " + AndroidInAppPurchaseManager.instance.inventory.purchases.Count + " products");
		} else {
			 AndroidMessage.Create("Connection Responce", result.response.ToString() + " " + result.message);
		}

		Debug.Log ("Connection Responce: " + result.response.ToString() + " " + result.message);

	}






















}
