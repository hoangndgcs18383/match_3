using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Match_3
{
    [Serializable]
    public class ShopData : IBuildData
    {
        public string Id;
        public string Name;
        public Sprite Icon;
        public Dictionary<string, int> Rewards;
        public int Price;
    }

    public class ShopItem : MonoBehaviour, IBuildItem
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text priceText;
        [SerializeField] private Button buyButton;
        [SerializeField] private ShopItemInfo shopItemInfoPrefab;
        [SerializeField] private RectTransform shopItemInfoParent;
        private ShopData _shopData;
        private Action _onBuySuccess;
        private Action _onBuyFail;

        public void Initialize()
        {
        }

        public void SetData(IBuildData data, Action onBuySuccess = null, Action onBuyFail = null)
        {
            _shopData = (ShopData)data;
            _onBuySuccess = onBuySuccess;
            _onBuyFail = onBuyFail;

            priceText.SetText(_shopData.Price.ToString());
            nameText.SetText(_shopData.Id);

            foreach (var shopDataReward in _shopData.Rewards)
            {
                if (shopDataReward.Value <= 0) continue;

                ShopItemInfo shopItemInfo = Instantiate(shopItemInfoPrefab, shopItemInfoParent);
                shopItemInfo.SetData(new ShopInfoData
                {
                    Id = shopDataReward.Key,
                    Count = shopDataReward.Value
                });
            }

            shopItemInfoPrefab.gameObject.SetActive(false);

            buyButton.onClick.AddListener(OnBuyButtonClick);
        }

        private void OnBuyButtonClick()
        {
            ProfileDataService.Instance.BuyPowerUpData(_shopData.Rewards, _shopData.Price, OnBuySuccess, OnBuyFail);
        }

        private void OnBuySuccess()
        {
            UIManager.Current.ShowRewardUI(_shopData.Rewards);
            _onBuySuccess?.Invoke();
        }

        private void OnBuyFail()
        {
            _onBuyFail?.Invoke();
        }
    }
}