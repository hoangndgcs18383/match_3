using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
using Zeff.Core.Service;

namespace Match_3
{
    public enum IAPPurchaseType
    {
        RemoveAds
    }

    public class IAPService : Service<IAPService>, IStoreListener
    {
        public Action OnInitializedIAPEvent;
        public Action<Status> OnInitializedCallbackIAPEvent;
        public Action<Status> OnPurchaseCallbackIAPEvent;

        private IStoreController _storeController;
        private IExtensionProvider _extensionProvider;

        public override void Initialize()
        {
            base.Initialize();

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

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            OnInitializedCallbackIAPEvent?.Invoke(Status.Fail);
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
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
            OnPurchaseCallbackIAPEvent?.Invoke(validPurchase ? Status.Success : Status.Fail);
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            OnPurchaseCallbackIAPEvent?.Invoke(Status.Fail);
        }

        #endregion

        public void Purchase(IAPPurchaseType type)
        {
#if UNITY_EDITOR
            OnPurchaseCallbackIAPEvent?.Invoke(Status.Success);
#else
            var product = _storeController.products.WithID(type.ToString());

            if (product != null && product.availableToPurchase)
            {
                _storeController.InitiatePurchase(product);
            }
            else
            {
                OnPurchaseCallbackIAPEvent?.Invoke(Status.Fail);
            }
#endif
        }
    }
}