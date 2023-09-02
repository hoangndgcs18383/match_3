using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using Zeff.Core.Localization;
using Zeff.Extensions;

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
        
        public void Initialize()
        {
        }

        public void SetData(IBuildData data)
        {
            _shopData = (ShopData) data;
            
            priceText.SetText(_shopData.Price.ToString());
            nameText.SetText(_shopData.Id);

            foreach (var shopDataReward in _shopData.Rewards)
            {
                if(shopDataReward.Value <= 0) continue;
                
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
            UIManager.Current.UpdateGUIAllPowerUp();
        }
        
        private void OnBuyFail()
        {
           
        }

    }
}
