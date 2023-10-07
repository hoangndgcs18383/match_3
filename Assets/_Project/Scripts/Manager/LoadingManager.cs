using System;
using System.Collections.Generic;
using DG.Tweening;
using MEC;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zeff.Core.Service;

namespace Match_3
{
    public class LoadingManager : MonoBehaviour
    {
        public static LoadingManager Instance { get; private set; }

        [SerializeField] private RawImage loadingImage;

        private async void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(this);

            try
            {
                await UnityServices.InitializeAsync();
            }
            catch (Exception e)
            {
                Debug.Log(e);
                throw;
            }

            SetTargetFPS();
            RegisterService();
            LoadService();
            
            ProfileDataService.Instance.AutoSaveProfile();
            TimerManager.Current.Initialize();
            
        }
        
        private void SetTargetFPS()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }

        private void RegisterService()
        {
            ServiceLocator.Instance.RegisterService("TimingService", new TimingService());
            ServiceLocator.Instance.RegisterService("DesignDataService", new DesignDataService());
            ServiceLocator.Instance.RegisterService("ProfileDataService", new ProfileDataService());
            ServiceLocator.Instance.RegisterService("IAPService", new IAPService());
        }

        private void LoadService()
        {
            ServiceLocator.Instance.LoadService();

            Timing.RunCoroutine(LoadSceneAsync("Menu"));
        }

        public void LoadScene(string sceneName)
        {
            Timing.RunCoroutine(LoadSceneAsync(sceneName));
        }

        private Sequence _loadingSequence;

        private IEnumerator<float> LoadSceneAsync(string sceneName)
        {
            loadingImage.DOKill();
            AnimFadeOut();

            var async = SceneManager.LoadSceneAsync(sceneName);
            async.allowSceneActivation = false;
            while (!async.isDone)
            {
                if (async.progress >= 0.9f)
                {
                    async.allowSceneActivation = true;
                }

                yield return Timing.WaitForOneFrame;
            }

            AnimFadeIn();
        }

        public void AnimFadeIn()
        {
            loadingImage.DOFade(0, 0.5f).SetEase(Ease.InBack);
        }

        public void AnimFadeOut()
        {
            loadingImage.DOFade(1, 0.5f).SetEase(Ease.OutBack);
        }
    }
}