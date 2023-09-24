using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Match_3
{
    public class ShopInfoData: IBuildData
    {
        public string Id;
        public int Count;
    }
    
    public class ShopItemInfo : MonoBehaviour, IBuildItem
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text countText;
        [SerializeField] private Image icon;
        
        
        public void Initialize()
        {
        }
        

        public void SetData(IBuildData data, Action onBuySuccess = null, Action onBuyFail = null)
        {
            if(data == null) return;
            nameText.SetText(((ShopInfoData) data).Id);
            countText.SetText(((ShopInfoData) data).Count.ToString());
            icon.sprite = IconManager.Current.GetIcon(((ShopInfoData) data).Id);
        }
    }
}