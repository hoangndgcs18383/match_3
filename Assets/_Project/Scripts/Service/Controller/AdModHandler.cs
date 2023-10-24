using System;
using GoogleMobileAds.Api;
using UnityEngine;

namespace Match_3
{
    public class AdModHandler : IAdsHandler
    {
#if UNITY_ANDROID
        private string _adUnitId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
  private string _adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
  private string _adUnitId = "unused";
#endif
        private RewardedAd _rewardedAd;

        public IAdsHandler.OnInitializedAds OnInitializedAdsEvent { get; set; }
        public IAdsHandler.OnLoadAds OnLoadAdsEvent { get; set; }
        public IAdsHandler.OnShowAds OnShowAdsEvent { get; set; }
        public IAdsHandler.OnShowAds OnRewardAdsEvent { get; set; }

        public void InitializedAds()
        {
            MobileAds.Initialize(OnInitialized);
        }
        
        private BannerView _bannerView;

        private void CreateBannerView()
        {
            if(_bannerView != null)
                _bannerView.Destroy();
            
            _bannerView = new BannerView(_adUnitId, AdSize.Banner, AdPosition.Bottom);
        }
        
        

        public void LoadRewardedAd()
        {
            if (_rewardedAd != null && _rewardedAd.CanShowAd())
            {
                _rewardedAd.Destroy();
                _rewardedAd = null;
            }

            if (_bannerView == null)
            {
                CreateBannerView();
            }

            AdRequest request = new AdRequest();
            request.Keywords.Add("unity-admob-sample");

            if (_bannerView != null) _bannerView.LoadAd(request);
            RewardedAd.Load(_adUnitId, request, OnAdsLoaded);
        }

        public void ShowRewardedAd()
        {
            if (_rewardedAd != null)
                _rewardedAd.Show(UserRewardEarnedCallback);
        }

        private void UserRewardEarnedCallback(Reward reward)
        {
            if (reward == null)
            {
                Debug.LogError("[UserRewardEarnedCallback reward]: is null");
                OnRewardAdsEvent?.Invoke(AdsResult.Fail);
                return;
            }

            Debug.Log("[UserRewardEarnedCallback]: event received" + reward.Type + " " + reward.Amount);
            OnRewardAdsEvent?.Invoke(AdsResult.Success);
        }

        private void OnInitialized(InitializationStatus status)
        {
            try
            {
                OnInitializedAdsEvent?.Invoke(AdsResult.Success);
                LoadRewardedAd();
            }
            catch (Exception e)
            {
                OnInitializedAdsEvent?.Invoke(AdsResult.Fail);

                Debug.LogError($"[OnInitialized Ads]: {e.Message}\n{e.StackTrace}");
            }
        }

        private void OnAdsLoaded(RewardedAd ad, LoadAdError error)
        {
            if (error != null || ad == null)
            {
                OnLoadAdsEvent?.Invoke(AdsResult.Fail);
                return;
            }

            _rewardedAd = ad;
            OnLoadAdsEvent?.Invoke(AdsResult.Success);
            ShowRewardedAd();
            Debug.Log("[OnAdsLoaded]: event received" + ad.GetResponseInfo());
        }
    }
}