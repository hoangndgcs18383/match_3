using System;
using UnityEngine;
using UnityEngine.Advertisements;

namespace Match_3
{
    public class UnityAdsHandler : IAdsHandler, IUnityAdsInitializationListener, IUnityAdsShowListener,
        IUnityAdsLoadListener
    {
#if UNITY_ANDROID
        private string _adUnitId = "Rewarded_Android";
#elif UNITY_IPHONE
  private string _adUnitId = "Rewarded_iOS";
#else
  private string _adUnitId = "unused";
#endif

#if UNITY_ANDROID
        private string _gameId = "5364748";
#elif UNITY_IPHONE
  private string _gameId = "5364749";
#else
  private string _gameId = "unused";
#endif

        public IAdsHandler.OnInitializedAds OnInitializedAdsEvent { get; set; }
        public IAdsHandler.OnLoadAds OnLoadAdsEvent { get; set; }
        public IAdsHandler.OnShowAds OnShowAdsEvent { get; set; }
        public IAdsHandler.OnShowAds OnRewardAdsEvent { get; set; }

        public void InitializedAds()
        {
            if (!Advertisement.isInitialized && Advertisement.isSupported)
            {
                Advertisement.Initialize(_gameId, true, this);
            }
        }

        public void LoadRewardedAd()
        {
            Advertisement.Load(_adUnitId, this);
        }

        public void ShowRewardedAd()
        {
            Advertisement.Show(_adUnitId, this);
        }

        public void OnInitializationComplete()
        {
            OnInitializedAdsEvent?.Invoke(AdsResult.Success);
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            OnInitializedAdsEvent?.Invoke(AdsResult.Fail);
            Debug.LogError($"[OnInitialized Ads]: {error} {message}");
        }

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
            OnInitializedAdsEvent?.Invoke(AdsResult.Fail);
            Debug.LogError($"[OnUnityAdsShowFailure Ads]: {placementId} {error} {message}");
        }

        public void OnUnityAdsShowStart(string placementId)
        {
            OnShowAdsEvent?.Invoke(AdsResult.Success);
            Debug.Log($"[OnUnityAdsShowStart Ads]: {placementId}");
        }

        public void OnUnityAdsShowClick(string placementId)
        {
        }

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            if (showCompletionState == UnityAdsShowCompletionState.COMPLETED)
            {
                OnRewardAdsEvent?.Invoke(AdsResult.Success);
                Debug.Log($"[OnUnityAdsShowComplete Ads]: {placementId} {showCompletionState}");
            }
            else
            {
                OnRewardAdsEvent?.Invoke(AdsResult.Fail);
                Debug.LogError($"[OnUnityAdsShowComplete Ads] : {placementId} {showCompletionState}");
            }
        }

        public void OnUnityAdsAdLoaded(string placementId)
        {
            OnLoadAdsEvent?.Invoke(AdsResult.Success);
            Debug.Log($"[OnUnityAdsAdLoaded Ads]: {placementId}");
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
            OnLoadAdsEvent?.Invoke(AdsResult.Fail);
            Debug.LogError($"pOnUnityAdsFailedToLoad Ads]: {placementId} {error} {message}");
        }
    }
}