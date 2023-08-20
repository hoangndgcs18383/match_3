using GoogleMobileAds.Api;

namespace Match_3
{
    public enum AdsResult
    {
        Success,
        Fail,
        Skipped
    }
    
    public interface IAdsHandler
    {
        public delegate void OnInitializedAds(AdsResult status);
        public delegate void OnLoadAds(AdsResult status);
        public delegate void OnShowAds(AdsResult status);
        public delegate void OnRewardAds(AdsResult status);
        public OnInitializedAds OnInitializedAdsEvent { get; set; }
        public OnLoadAds OnLoadAdsEvent { get; set; }
        public OnShowAds OnShowAdsEvent { get; set; }
        public OnShowAds OnRewardAdsEvent { get; set; }
        public void InitializedAds();
        public void LoadRewardedAd();
        public void ShowRewardedAd();
    }

    public class NotAdsHandler : IAdsHandler
    {
        public IAdsHandler.OnInitializedAds OnInitializedAdsEvent { get; set; }
        public IAdsHandler.OnLoadAds OnLoadAdsEvent { get; set; }
        public IAdsHandler.OnShowAds OnShowAdsEvent { get; set; }
        public IAdsHandler.OnShowAds OnRewardAdsEvent { get; set; }

        public void InitializedAds()
        {
        }

        public void LoadRewardedAd()
        {
        }

        public void ShowRewardedAd()
        {
        }
    }
}