using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zeff.Core.Parser;
using Zeff.Extensions;

namespace Match_3
{
    public class UIShop : ScreenBase
    {
        public Button backButton;
        
        public ShopItem shopItemPrefab;
        public Transform shopItemParent;
        

        private void Start()
        {
            var shopDesign = ZParserStatic.Instance.ShopDesign.GetAll();

            foreach (var data in shopDesign)
            {
                ShopItem shopItem = Instantiate(shopItemPrefab, shopItemParent);
                shopItem.SetData(new ShopData
                {
                    Id = data.Key,
                    Rewards = data.Value.rewards.TryToParserDictionary(),
                    Price = data.Value.price
                });
            }
            
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            backButton.onClick.AddListener(OnBackClick);
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
        

    }
}