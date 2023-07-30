#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using Cathei.BakingSheet;
using Cathei.BakingSheet.Unity;
using UnityEditor;
using UnityEngine;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Match_3
{
    [Serializable]
    public class SheetContainer : SheetContainerBase
    {
        public ShopSheet ShopDesign { get; private set; }

        public SheetContainer(ILogger logger) : base(logger)
        {
        }
    }

    [Serializable]
    public class ShopSheet : Sheet<ShopSheet.Row>
    {
        public class Row : SheetRow
        {
            public string Name;
            public int Price;
            public string Description;
            public string Icon;
        }
    }

    public class ExcelPostprocessor : AssetPostprocessor
    {
        static async void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            // automatically run postprocessor if any excel file is imported
            string excelAsset = importedAssets.FirstOrDefault(x => x.EndsWith(".xlsx"));

            if (excelAsset != null)
            {
                // excel path as "Assets/_Project/ExcelDesign"
                var excelPath = Path.Combine(Application.dataPath, "_Project/ExcelDesign");

                // json path as "Assets/StreamingAssets/Json"
                var jsonPath = Path.Combine(Application.streamingAssetsPath, "Json");

                var logger = new UnityLogger();
                var sheetContainer = new SheetContainer(logger);

                // create excel converter from path
                var excelConverter = new ExcelSheetConverter(excelPath);

                // bake sheets from excel converter
                await sheetContainer.Bake(excelConverter);

                // create json converter to path
                var jsonConverter = new JsonSheetConverter(jsonPath);

                // save datasheet to streaming assets
                await sheetContainer.Store(jsonConverter);

                AssetDatabase.Refresh();

                Debug.Log("Excel sheet converted.");
            }
        }
    }
}
#endif