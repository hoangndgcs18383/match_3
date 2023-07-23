namespace Match_3
{
#if UNITY_EDITOR
    using System.IO;
    using UnityEditor;
    using UnityEngine;
    using static System.IO.Directory;
    using static UnityEditor.AssetDatabase;

    public static class SetUpFolders
    {
        [MenuItem("Tools/Setup/Create Default Folders")]
        public static void CreateDefaultFolders()
        {
            Folders.CreateDefault("_Project", "Animation", "ExcelDesign", "Art", "Materials", "Prefabs", "ScriptableObjects",
                "Resources", "Scripts", "Settings");
            Refresh();
            SaveAssets();
        }
    }

    static class Folders
    {
        public static void CreateDefault(string root, params string[] folders)
        {
            // Create root folder if not exist yet

            foreach (var folder in folders)
            {
                var path = Path.Combine(Application.dataPath, root);
                path = Path.Combine(path, folder);
                if (!Exists(path))
                {
                    CreateDirectory(path);
                }
            }
        }
    }
#endif
}