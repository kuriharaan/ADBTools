using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ADBTools
{
    public class MainWindow : EditorWindow
    {
        [SerializeField]
        string text;

        [SerializeField]
        bool boolean;

        //static string settingAssetName = "Assets/ADBTools/Resources/ADBToolsSettings.asset";
        static string settingAssetName = "ADBToolsSettings";

        [MenuItem("Window/ADBTools")]
        static void Open()
        {
            EditorWindow window = EditorWindow.GetWindow<MainWindow>("ADBTools");

            var settings = Resources.Load(settingAssetName) as Settings;
            if( null == settings)
            {
                var assetpath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(window));
                var dirpath   = System.IO.Directory.GetParent(assetpath);

                settings = CreateInstance<Settings>();
                AssetDatabase.CreateAsset(settings, string.Concat(dirpath, "/Resources/", settingAssetName, ".asset"));
                AssetDatabase.Refresh();
            }
        }
    }
}
