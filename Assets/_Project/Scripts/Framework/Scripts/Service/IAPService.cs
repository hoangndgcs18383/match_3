using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.Purchasing.Security;
using Zeff.Core.Service;

namespace Match_3
{
    public enum IAPPurchaseType
    {
        RemoveAds
    }

    public class IAPService : Service<IAPService>, IDetailedStoreListener
    {
        public Action OnInitializedIAPEvent;
        public Action<Status> OnInitializedCallbackIAPEvent;
        public Action<string, Status> OnPurchaseCallbackIAPEvent;

        private IStoreController _storeController;
        private IExtensionProvider _extensionProvider;

        public override void Initialize()
        {
            base.Initialize();

            Debug.Log("[IAP] Initialize");

            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            foreach (var iap in Enum.GetNames(typeof(IAPPurchaseType)))
            {
                builder.AddProduct(iap, ProductType.Consumable);
            }

            UnityPurchasing.Initialize(this, builder);
        }

        #region Purchase Callback

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _storeController = controller;
            _extensionProvider = extensions;

            OnInitializedIAPEvent?.Invoke();
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            OnPurchaseCallbackIAPEvent?.Invoke(product.definition.id, Status.Fail);
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            OnInitializedCallbackIAPEvent?.Invoke(Status.Fail);
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            OnInitializedCallbackIAPEvent?.Invoke(Status.Fail);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            bool validPurchase = true;

#if UNITY_ANDROID || UNITY_IOS
            var validator =
                new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);

            try
            {
                var result = validator.Validate(purchaseEvent.purchasedProduct.receipt);

                foreach (IPurchaseReceipt productReceipt in result)
                {
                    Debug.Log($"[IAP] ProcessPurchase: {productReceipt.productID}");
                    Debug.Log($"[IAP] ProcessPurchase: {productReceipt.purchaseDate}");
                    Debug.Log($"[IAP] ProcessPurchase: {productReceipt.transactionID}");
                }
            }
            catch (IAPSecurityException e)
            {
                Debug.LogError($"[IAP] ProcessPurchase: {e}");
                validPurchase = false;
            }
#endif
            OnPurchaseCallbackIAPEvent?.Invoke(purchaseEvent.purchasedProduct.definition.id,
                validPurchase ? Status.Success : Status.Fail);
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            OnPurchaseCallbackIAPEvent?.Invoke(product.definition.id, Status.Fail);
        }

        #endregion

        public void Purchase(string productId)
        {
#if UNITY_EDITOR
            OnPurchaseCallbackIAPEvent?.Invoke(productId, Status.Success);
#else
            var product = _storeController.products.WithID(productId);

            if (product != null && product.availableToPurchase)
            {
                _storeController.InitiatePurchase(product);
            }
            else
            {
                OnPurchaseCallbackIAPEvent?.Invoke(productId,Status.Fail);
            }
#endif
        }
    }
}