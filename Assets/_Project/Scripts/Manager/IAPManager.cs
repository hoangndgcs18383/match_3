using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.Purchasing.Security;

namespace Match_3
{
    public enum IAPType
    {
        RemoveAds,
        BuyCoin,
        BuyGem,
        BuyCoinAndGem
    }

    public enum StateIAP
    {
        Success,
        Fail,
        Cancel
    }

    public class IAPManager : MonoBehaviour, IDetailedStoreListener
    {
        public static IAPManager Current;

        private IStoreController _storeController;
        private IExtensionProvider _storeExtensionProvider;

        public Action<string, StateIAP> OnBuyProductEvent;

        private void Awake()
        {
            if (Current == null)
            {
                Current = this;
                if (transform.parent == null)
                {
                    DontDestroyOnLoad(this);
                }
            }
        }

        private void Start()
        {
            InitializedIAP();
        }

        private bool IsInitializedIAP()
        {
            return _storeController != null && _storeExtensionProvider != null;
        }

        private void InitializedIAP()
        {
            if (IsInitializedIAP()) return;
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            foreach (var iap in Enum.GetNames(typeof(IAPType)))
            {
                builder.AddProduct(iap, ProductType.Consumable);
            }

            UnityPurchasing.Initialize(this, builder);
        }

        public void BuyProductID(string productId)
        {
#if UNITY_EDITOR
            OnBuyProductEvent?.Invoke(productId, StateIAP.Success);
#else
            if (IsInitializedIAP())
            {
                var product = _storeController.products.WithID(productId);
                if (product != null && product.availableToPurchase)
                {
                    _storeController.InitiatePurchase(product);
                }
                else
                {
                    Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            else
            {
                Debug.Log("BuyProductID FAIL. Not initialized.");
                OnBuyProductEvent?.Invoke(productId, StateIAP.Fail);
            }
#endif
        }

        #region Callback

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.LogError("OnInitializeFailed InitializationFailureReason:" + error);
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Debug.LogError("OnInitializeFailed InitializationFailureReason:" + error + " message:" + message);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            bool validPurchase = true;

            var validator =
                new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);

            try
            {
                var result = validator.Validate(purchaseEvent.purchasedProduct.receipt);

                foreach (IPurchaseReceipt product in result)
                {
                    Debug.Log(product.productID);
                    Debug.Log(product.purchaseDate);
                    Debug.Log(product.transactionID);   
                }
            }
            catch (Exception e)
            {
                validPurchase = false;
            }

            if(validPurchase)
                OnBuyProductEvent?.Invoke(purchaseEvent.purchasedProduct.definition.id, StateIAP.Success);
            else
                OnBuyProductEvent?.Invoke(purchaseEvent.purchasedProduct.definition.id, StateIAP.Fail);
            
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.LogError($"OnPurchaseFailed: FAIL. Product: '{product.definition.storeSpecificId}', PurchaseFailureReason: {failureReason}");
            OnBuyProductEvent?.Invoke(product.definition.id, StateIAP.Fail);
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _storeController = controller;
            _storeExtensionProvider = extensions;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            Debug.LogError($"OnPurchaseFailed: FAIL. Product: '{product.definition.storeSpecificId}', PurchaseFailureReason: {failureDescription}");
            OnBuyProductEvent?.Invoke(product.definition.id, StateIAP.Fail);
        }

        #endregion
    }
}