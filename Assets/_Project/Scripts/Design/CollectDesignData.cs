#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Proyecto26;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Zeff.Core.EndPoint;

[System.Serializable]
public class ExcelData
{
    public List<string> headers;
    public List<List<string>> rows;
}

namespace Zeff.Core.Parser
{
    public class CollectDesignData : MonoBehaviour
    {
        private string _questFilePath = "Assets/_Project/Design/QuestDesign.json";

        [Button(ButtonSizes.Large)]
        [GUIColor(0.3f, 0.8f, 0.8f)]
        private void ReadLoadExcel()
        {
            string excelPath = Application.dataPath + "/_Project/ExcelDesign/QuestDesign.xlsx";
            
        }
        
        
        [Button(ButtonSizes.Large)]
        [GUIColor(0.3f, 0.8f, 0.8f)]
        public void Collect()
        {
            RestClient.Get(EndPointConstants.QUEST_URL).Then(response =>
            {
                File.WriteAllText(_questFilePath, response.Text);

            });
        }
        
        private void OnApplicationQuit()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        
    }
}

#endif

