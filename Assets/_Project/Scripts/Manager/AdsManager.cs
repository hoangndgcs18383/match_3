using System;
using UnityEngine;

namespace Match_3
{
    public class AdsManager : MonoBehaviour
    {
        public static AdsManager Current;

        private IAdsHandler _adsHandler = new NotAdsHandler();
        private IAdsHandler _adsModHandler = new NotAdsHandler();
        
        public Action OnRewardAdsEvent;

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
            InitializedAdsHandler(new UnityAdsHandler());
        }

        private void InitializedAdsHandler(IAdsHandler adsHandler)
        {
            _adsHandler = adsHandler;
            _adsHandler.OnInitializedAdsEvent += OnInitializedAdsEvent;
            _adsHandler.OnLoadAdsEvent += OnLoadAdsEvent;
            _adsHandler.OnShowAdsEvent += OnShowAdsEvent;
            _adsHandler.OnRewardAdsEvent += OnRewardAdEvent;
            _adsHandler.InitializedAds();
        }

        private void InitializeAdsModHandler()
        {
            _adsModHandler = new AdModHandler();
            _adsModHandler.InitializedAds();
        }

        private void OnRewardAdEvent(AdsResult status)
        {
            if (status == AdsResult.Success)
            {
                OnRewardAdsEvent?.Invoke();
                OnRewardAdsEvent = null;
            }
        }

        private void OnLoadAdsEvent(AdsResult status)
        {
            Debug.Log("OnLoadAdsEvent " + status);
            if (status == AdsResult.Success)
            {
                UIManager.Current.SpinAds();
            }
            else
            {
                UIManager.Current.DisableAds();
            }
        }

        private void OnShowAdsEvent(AdsResult status)
        {
            Debug.Log("OnShowAdsEvent " + status);
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

                if (_adsHandler == null) UIManager.Current.DisableAds();
            }
        }


        public void ShowRewardedAd(Action onRewardAdsEvent = null)
        {
            OnRewardAdsEvent = onRewardAdsEvent;
            _adsHandler.ShowRewardedAd();
        }
    }
}