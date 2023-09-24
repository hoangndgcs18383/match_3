using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zeff.Core.Parser;
using Zeff.Extensions;

namespace Match_3
{
    public class UIShop : BaseScreen
    {
        public Button backButton;
        
        public ShopItem shopItemPrefab;
        public Transform shopItemParent;
        
        public TMP_Text goldText;
        
        private void Start()
        {
            var shopDesign = ZParserStatic.Instance.ShopDesign.GetAll();

            foreach (var data in shopDesign)
            {
                ShopItem shopItem = Instantiate(shopItemPrefab, shopItemParent);
                shopItem.SetData(new ShopData
                {
                    Id = data.Key,
                    Rewards = data.Value.Rewards.TryToParserDictionary(),
                    Price = data.Value.Price
                }, UpdateGold, OnBuyFail);
            }
            
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            backButton.onClick.AddListener(OnBackClick);
            UpdateGold();
        }


        protected override void OnDisable()
        {
            base.OnDisable();
            backButton.onClick.RemoveListener(OnBackClick);

        }
        private void OnBackClick()
        {
            gameObject.SetActive(false);
        }
        
        public void UpdateGold()
        {
            goldText.SetText($"Coin: {ProfileDataService.Instance.ProfileData.Gold}");
        }

        public void OnBuyFail()
        {
            FlyTextManager.Instance.SetFlyText("Not enough gold");
        }
    }
}