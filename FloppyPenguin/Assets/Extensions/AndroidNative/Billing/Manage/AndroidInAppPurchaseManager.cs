////////////////////////////////////////////////////////////////////////////////
//  
// @module Android Native Plugin for Unity3D 
// @author Osipov Stanislav lacost.st@gmail.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AndroidInAppPurchaseManager : Singletone<AndroidInAppPurchaseManager> {


	public const string ON_PRODUCT_PURCHASED   = "on_product_purchased";
	public const string ON_PRODUCT_CONSUMED    = "on_product_consumed";

	public const string ON_BILLING_SETUP_FINISHED   = "on_billing_setup_finished";
	public const string ON_RETRIEVE_PRODUC_FINISHED = "on_retrieve_produc_finished";

	private List<string> _productsIds =  new List<string>();

	private AndroidInventory _inventory;


	//--------------------------------------
	// INITIALIZE
	//--------------------------------------
	
	void Awake() {
		DontDestroyOnLoad(gameObject);
		_inventory = new AndroidInventory ();
	}


	//--------------------------------------
	// PUBLIC METHODS
	//--------------------------------------


	//Fill your products befor loading store
	public void addProduct(string SKU) {
		_productsIds.Add(SKU);
	}

	public void retrieveProducDetails() {
		AndroidNative.retrieveProducDetails ();
	}

	public void purchase(string SKU) {
		AndroidNative.purchase (SKU);
	}

	public void consume(string SKU) {
		AndroidNative.consume (SKU);
	}


	public void loadStore(string base64EncodedPublicKey) {

		string ids = "";
		int len = _productsIds.Count;
		for(int i = 0; i < len; i++) {
			if(i != 0) {
				ids += ",";
			}

			ids += _productsIds[i];
		}

		AndroidNative.connectToBilling (ids, base64EncodedPublicKey);
	}



	//--------------------------------------
	// GET / SET
	//--------------------------------------

	public AndroidInventory inventory {
		get {
			return _inventory;
		}
	}



	//--------------------------------------
	// EVENTS
	//--------------------------------------



	public void OnPurchaseFinishedCallback(string data) {
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);

		int resp = System.Convert.ToInt32 (storeData[0]);
		GooglePurchaseTemplate purchase = null;


		if(resp == BillingResponseCodes.BILLING_RESPONSE_RESULT_OK) {
			purchase = new GooglePurchaseTemplate ();
			purchase.SKU 				= storeData[2];
			purchase.packageName 		= storeData[3];
			purchase.developerPayload 	= storeData[4];
			purchase.orderId 	        = storeData[5];

			if(_inventory != null) {
				_inventory.addPurchase (purchase);
			}

		}

		BillingResult result = new BillingResult (resp, storeData [1], purchase);


		dispatch (ON_PRODUCT_PURCHASED, result);
	}


	public void OnConsumeFinishedCallBack(string data) {
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);

		int resp = System.Convert.ToInt32 (storeData[0]);
		GooglePurchaseTemplate purchase = null;


		if(resp == BillingResponseCodes.BILLING_RESPONSE_RESULT_OK) {
			purchase = new GooglePurchaseTemplate ();
			purchase.SKU 				= storeData[2];
			purchase.packageName 		= storeData[3];
			purchase.developerPayload 	= storeData[4];
			purchase.orderId 	        = storeData[5];

			if(_inventory != null) {
				_inventory.removePurchase (purchase);
			}

		}

		BillingResult result = new BillingResult (resp, storeData [1], purchase);


		dispatch (ON_PRODUCT_CONSUMED, result);
	}



	public void OnBillingSetupFinishedCallback(string data) {
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);

		int resp = System.Convert.ToInt32 (storeData[0]);

		BillingResult result = new BillingResult (resp, storeData [1]);
		dispatch (ON_BILLING_SETUP_FINISHED, result);
	}


	public void OnQueryInventoryFinishedCallBack(string data) {
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);

		int resp = System.Convert.ToInt32 (storeData[0]);

		BillingResult result = new BillingResult (resp, storeData [1]);
		dispatch (ON_RETRIEVE_PRODUC_FINISHED, result);
	}



	public void OnPurchasesRecive(string data) {
		if(data.Equals(string.Empty)) {
			Debug.Log("InAppPurchaseManager, no purchases avaiable");
			return;
		}

		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);



		for(int i = 0; i < storeData.Length; i+=4) {
			GooglePurchaseTemplate tpl =  new GooglePurchaseTemplate();
			tpl.SKU 				= storeData[i];
			tpl.packageName 		= storeData[i + 1];
			tpl.developerPayload 	= storeData[i + 2];
			tpl.orderId 	        = storeData[i + 3];
			_inventory.addPurchase (tpl);
		}

		Debug.Log("InAppPurchaseManager, tottal purchases loaded: " + _inventory.purchases.Count);

	}


	public void OnProducttDetailsRecive(string data) {
		if(data.Equals(string.Empty)) {
			Debug.Log("InAppPurchaseManager, no products avaiable");
			return;
		}

		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);


		for(int i = 0; i < storeData.Length; i+=4) {
			GoogleProductTemplate tpl =  new GoogleProductTemplate();
			tpl.SKU 		  = storeData[i];
			tpl.price 		  = storeData[i + 1];
			tpl.title 	      = storeData[i + 2];
			tpl.description   = storeData[i + 3];
			_inventory.addProduct (tpl);
		}

		Debug.Log("InAppPurchaseManager, tottal products loaded: " + _inventory.products.Count);
	}


}
