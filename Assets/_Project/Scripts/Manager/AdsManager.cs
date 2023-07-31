using UnityEngine;

namespace Match_3
{
    public class AdsManager : MonoBehaviour
    {
        public static AdsManager Current;

        private IAdsHandler _adsHandler = new NotAdsHandler();

        private void Awake()
        {
            Current = this;
            //DontDestroyOnLoad(this);

            InitializedAdsHandler(new UnityAdsHandler());
        }

        private void InitializedAdsHandler(IAdsHandler adsHandler)
        {
            _adsHandler = adsHandler;
            _adsHandler.OnInitializedAdsEvent += OnInitializedAdsEvent;
            _adsHandler.OnLoadAdsEvent += OnLoadAdsEvent;
            _adsHandler.OnShowAdsEvent += OnShowAdsEvent;
            _adsHandler.InitializedAds();
        }

        private void OnLoadAdsEvent(AdsResult status)
        {
            Debug.Log("OnLoadAdsEvent " + status);

            UIManager.Current.SpinAds();
        }

        private void OnShowAdsEvent(AdsResult status)
        {
            Debug.Log("OnShowAdsEvent " + status);
            if (status == AdsResult.Success)
            {
                GameManager.Current.AddCoin(100);
                UIManager.Current.StopSpinAds();
            }
        }

        private void OnInitializedAdsEvent(AdsResult status)
        {
            Debug.Log("OnInitializedAdsEvent " + status);
            if (status == AdsResult.Success)
                _adsHandler.LoadRewardedAd();
            else if (status == AdsResult.Fail)
            {
                _adsHandler = null;
                InitializedAdsHandler(new AdModHandler());
            }
        }


        public void ShowRewardedAd()
        {
            _adsHandler.ShowRewardedAd();
        }
    }
}