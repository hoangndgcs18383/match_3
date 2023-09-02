#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using Zeff.Core.Parser;


namespace Zeff.Core.EditorWindow
{
    public class GenerateParserConstant : OdinEditorWindow
    {
        [FolderPath] [SerializeField] private string generatePath;

        [Sirenix.OdinInspector.FilePath] [BoxGroup("Json File Path")]
        public string[] jsonFilePath;

        private string _streamingAssetsPath = "Assets/StreamingAssets/DesignData";

        [MenuItem("Tools/Design/Open Parser Tool")]
        private static void OpenWindow()
        {
            GetWindow<GenerateParserConstant>()
                .position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);
        }

        [Button(ButtonSizes.Large)]
        [GUIColor("GetButtonColor")]
        public void BuildParser()
        {
            if (jsonFilePath == null || jsonFilePath.Length == 0)
                return;

            string[] strings = new string[jsonFilePath.Length];
            string _generatePath = "Assets/_Project/Scripts/Framework/Scripts/Generate";


            foreach (var filePath in jsonFilePath)
            {
                if (File.Exists(filePath))
                {
                    string content = File.ReadAllText(filePath);

                    string pathStream = Path.Combine(_streamingAssetsPath,
                        Path.GetFileName(filePath).Replace(".json", ".bin"));

                    File.WriteAllText(pathStream, content);

                    string fileName = Path.GetFileName(filePath).Replace(".json", "Parser");

                    Debug.Log("Read file: " + Path.GetFileName(filePath).Replace(".json", "Parser"));

                    if (!File.Exists(Path.Combine(_generatePath, fileName + ".cs")))
                    {
                        //Generate Parser
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("using UnityEngine;");
                        sb.AppendLine("");
                        sb.AppendLine("namespace Zeff.Core.Parser");
                        sb.AppendLine("{");
                        sb.AppendLine($"\tpublic class {fileName} : ZBaseParser");
                        sb.AppendLine("\t{");
                        sb.AppendLine("\t\tpublic override void LoadData(string data)");
                        sb.AppendLine("\t\t{");
                        sb.AppendLine("\t\t\tbase.LoadData(data);");
                        sb.AppendLine("\t\t\t// TODO: Parse data");
                        sb.AppendLine("\t\t\t");
                        sb.AppendLine("\t\t}");
                        sb.AppendLine("\t}");
                        sb.AppendLine("}");

                        File.WriteAllText(Path.Combine(_generatePath, fileName + ".cs"), sb.ToString());
                    }
                }
            }


            ParseString parseString = new ParseString();
            parseString.Parse(jsonFilePath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private Color GetButtonColor() => Color.yellow;

        private const string RootPath = "DesignDara/{fileName}.bin";
        private const string DataPackPath = "DesignData/DataPackage.bin";
    }

    public class ParseString
    {
        private const string _assetsPath = "Assets/_Project/Scripts/Framework/Scripts/Parser";
        private const string _fileNameGenerate = "ZParserStatic.cs";
        private const string _streamingAssetsPath = "Assets/StreamingAssets/DesignData";
        private const string _fileNameDataPackage = "DataPackage.bin";

        private Dictionary<string, ZBaseParser> _parserDictionary = new Dictionary<string, ZBaseParser>();

        public void Parse(string[] fileJsonCollects)
        {
            StringBuilder sb = new StringBuilder();

            //Generate ZParserStatic

            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("// This file is auto-generated. Do not modify it manually.");
            sb.AppendLine("namespace Zeff.Core.Parser");
            sb.AppendLine("{");
            sb.AppendLine("\tpublic class ZParserStatic");
            sb.AppendLine("\t{");
            sb.AppendLine(
                "\t\tprivate static Dictionary<string, IBaseParser> _parsers = new Dictionary<string, IBaseParser>();");
            sb.AppendLine("\t\tprivate static ZParserStatic _instance;");
            sb.AppendLine("\t\tpublic static ZParserStatic Instance");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\tget");
            sb.AppendLine("\t\t\t{");
            sb.AppendLine("\t\t\t\tif (_instance == null)");
            sb.AppendLine("\t\t\t\t{");
            sb.AppendLine("\t\t\t\t\t_instance = new ZParserStatic();");
            sb.AppendLine("\t\t\t\t}");
            sb.AppendLine("\t\t\t\treturn _instance;");
            sb.AppendLine("\t\t\t}");
            sb.AppendLine("\t\t\tprivate set => _instance = value;");
            sb.AppendLine("\t\t}");
            sb.AppendLine("\t\tpublic void AddParser(string key, IBaseParser parser)");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\tif (_parsers.ContainsKey(key))");
            sb.AppendLine("\t\t\t{");
            sb.AppendLine("\t\t\t\t_parsers[key] = parser;");
            sb.AppendLine("\t\t\t}");
            sb.AppendLine("\t\t\telse");
            sb.AppendLine("\t\t\t{");
            sb.AppendLine("\t\t\t\t_parsers.Add(key, parser);");
            sb.AppendLine("\t\t\t}");
            sb.AppendLine("\t\t}");
            sb.AppendLine("\t\tprivate static T Get<T>(string key) where T : IBaseParser, new()");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\tif (_parsers.ContainsKey(key))");
            sb.AppendLine("\t\t\t{");
            sb.AppendLine("\t\t\t\treturn (T)_parsers[key];");
            sb.AppendLine("\t\t\t}");
            sb.AppendLine("\t\t\treturn new T();");
            sb.AppendLine("\t\t}");
            sb.AppendLine("\t\tpublic ZBaseParser RegisterParser(string key)");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\tswitch (key)");
            sb.AppendLine("\t\t\t{");

            foreach (var fileJson in fileJsonCollects)
            {
                string fileName = Path.GetFileName(fileJson).Replace(".json", "");
                sb.AppendLine($"\t\t\t\tcase \"{fileName}\": return new {fileName}Parser();");
            }

            sb.AppendLine("\t\t\t}");
            sb.AppendLine("\t\t\treturn null;");
            sb.AppendLine("\t\t}");
            foreach (var fileJson in fileJsonCollects)
            {
                string fileName = Path.GetFileName(fileJson).Replace(".json", "");
                sb.AppendLine($"\t\tpublic {fileName}Parser {fileName} => Get<{fileName}Parser>(\"{fileName}\");");
            }

            sb.AppendLine("\t}");

            sb.AppendLine("}");
            File.WriteAllText(Path.Combine(_assetsPath, _fileNameGenerate), sb.ToString());

            //to json
            string newJson = "{ \"Packs\": [,]}";

            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            List<string> _designDataList = new List<string>();

            foreach (var fileJson in fileJsonCollects)
            {
                _designDataList.Add(Path.GetFileName(fileJson).Replace(".json", ""));
            }

            dictionary["Packs"] = _designDataList;

            //newJson = newJson.Replace(",", $"{GetParserString(JsonConvert.SerializeObject(fileJsonCollects))}");
            File.WriteAllText(Path.Combine(_streamingAssetsPath, _fileNameDataPackage), dictionary.ToJson());
        }

        public string GetParserString(string value)
        {
            return Path.GetFileName(value).Replace(".json", "Parser");
        }
    }
}


#endif