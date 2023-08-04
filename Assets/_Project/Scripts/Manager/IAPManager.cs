using System;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Match_3
{
    public class IAPManager : MonoBehaviour, IStoreListener
    {
        
        

        #region Callback

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            throw new NotImplementedException();
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            throw new NotImplementedException();
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            throw new NotImplementedException();
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            throw new NotImplementedException();
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            throw new NotImplementedException();
        }

        #endregion
        
    }
}
