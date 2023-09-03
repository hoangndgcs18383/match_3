using System.Collections.Generic;
using SimpleJSON;
using Zeff.Core.Parser;

namespace Zeff.Core.Service
{
    public class DesignDataService : Service<DesignDataService>
    {
        private const string RootPath = "DesignData/{fileName}.bin";
        private const string DataPackPath = "DesignData/DataPackage.bin";

        private Dictionary<string, IBaseParser> parseData = new Dictionary<string, IBaseParser>();
        
        public override void Initialize()
        {
            base.Initialize();
            BetterStreamingAssets.Initialize();
            RegisterAllParser();
        }
        
        public void RegisterAllParser()
        {
            var dataPackage = GetDataFromStreamingAssets(DataPackPath);

            JSONObject jsonObject = JSON.Parse(dataPackage).AsObject;
            var list = jsonObject["Packs"].AsStringArray;
            //parse data package to list

            foreach (var s in list)
            {
                parseData[s] = ZParserStatic.Instance.RegisterParser(s);
            }

            LoadDataParser();
        }

        private void LoadDataParser()
        {
            if (parseData.Count == 0) RegisterAllParser();
            foreach (var parser in parseData)
            {
                parser.Value.LoadData(GetDataFromStreamingAssets(GetRootPath(parser.Key)));
                ZParserStatic.Instance.AddParser(parser.Key, parser.Value);
            }
        }

        public string GetRootPath(string fileName)
        {
            return RootPath.Replace("{fileName}", fileName);
        }

        public string GetDataFromStreamingAssets(string filePath)
        {
            if (BetterStreamingAssets.FileExists(filePath))
            {
                return BetterStreamingAssets.ReadAllText(filePath);
            }

            return null;
        }
    }
}