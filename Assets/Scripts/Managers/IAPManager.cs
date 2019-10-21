using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

// Deriving the Purchaser class from IStoreListener enables it to receive messages from Unity Purchasing.
public class IAPManager : MonoBehaviour, IStoreListener
{
    public ManagersContainer managers;
    public static IAPManager Instance { get; set; }

    private static IStoreController m_StoreController;          // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.
    
    public static string PRODUCT_5_DIAMONDS = "diamonds5";
    public static string PRODUCT_20_DIAMONDS = "diamonds20";
    public static string PRODUCT_100_DIAMONDS = "diamonds100";
    public static string PRODUCT_500_DIAMONDS = "diamonds500";

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (m_StoreController == null)
        {
            InitializePurchasing();
        }
    }

    public void InitializePurchasing()
    {
        if (IsInitialized())
        {
            return;
        }
        
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        
        builder.AddProduct(PRODUCT_5_DIAMONDS, ProductType.Consumable);
        builder.AddProduct(PRODUCT_20_DIAMONDS, ProductType.Consumable);
        builder.AddProduct(PRODUCT_100_DIAMONDS, ProductType.Consumable);
        builder.AddProduct(PRODUCT_500_DIAMONDS, ProductType.Consumable);

        UnityPurchasing.Initialize(this, builder);
    }


    private bool IsInitialized()
    {
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }


    public void Buy5diamonds()
    {
        BuyProductID(PRODUCT_5_DIAMONDS);
    }
    public void Buy20diamonds()
    {
        BuyProductID(PRODUCT_20_DIAMONDS);
    }
    public void Buy100diamonds()
    {
        BuyProductID(PRODUCT_100_DIAMONDS);
    }
    public void Buy500diamonds()
    {
        BuyProductID(PRODUCT_500_DIAMONDS);
    }




    private void BuyProductID(string productId)
    {
        // If Purchasing has been initialized ...
        if (IsInitialized())
        {
            // ... look up the Product reference with the general product identifier and the Purchasing 
            // system's products collection.
            Product product = m_StoreController.products.WithID(productId);

            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
                m_StoreController.InitiatePurchase(product);
            }
            // Otherwise ...
            else
            {
                // ... report the product look-up failure situation  
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        // Otherwise ...
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }


    //  
    // --- IStoreListener
    //

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        Debug.Log("OnInitialized: PASS");

        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;
    }
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        // A consumable product has been purchased by this user.
        if (String.Equals(args.purchasedProduct.definition.id, PRODUCT_5_DIAMONDS, StringComparison.Ordinal))
        {
            Debug.Log("add 5 diamonds");
            managers.sceneManager.Player.addDiamonds(5);
        }
        // Or ... a non-consumable product has been purchased by this user.
        else if (String.Equals(args.purchasedProduct.definition.id, PRODUCT_20_DIAMONDS, StringComparison.Ordinal))
        {
            Debug.Log("add 20 diamonds");
            managers.sceneManager.Player.addDiamonds(20);
        }
        else if (String.Equals(args.purchasedProduct.definition.id, PRODUCT_100_DIAMONDS, StringComparison.Ordinal))
        {
            Debug.Log("add 100 diamonds");
            managers.sceneManager.Player.addDiamonds(100);
        }
        else if (String.Equals(args.purchasedProduct.definition.id, PRODUCT_500_DIAMONDS, StringComparison.Ordinal))
        {
            Debug.Log("add 500 diamonds");
            managers.sceneManager.Player.addDiamonds(500);
        }
        else
        {
            Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
        }
        return PurchaseProcessingResult.Complete;
    }
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }
}