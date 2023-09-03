using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Zeff.Core.Parser
{
    [Serializable]
    public struct ShopDesign
    {
        public int  Price;
        public string Rewards;
    }

    public class ShopDesignParser : ZBaseParser
    {
        private Dictionary<string, ShopDesign> _shopDesigns;

        public override void LoadData(string data)
        {
            base.LoadData(data);
            // TODO: Parse data

            _shopDesigns = new Dictionary<string, ShopDesign>();

            try
            {
                _shopDesigns = JsonConvert.DeserializeObject<Dictionary<string, ShopDesign>>(data);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public void GetShopDesign(string key, out ShopDesign shopDesign)
        {
            shopDesign = _shopDesigns[key];
        }

        public Dictionary<string, ShopDesign> GetAll()
        {
            return _shopDesigns;
        }
    }
}