using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ADBTools
{
    public class MainWindow : EditorWindow
    {
        [MenuItem("Window/ADBTools")]
        static void Open()
        {
            System.Type inspectorType = System.Type.GetType("UnityEditor.InspectorWindow,UnityEditor.dll");
            EditorWindow window = EditorWindow.GetWindow<MainWindow>("ADBTools", inspectorType);
        }
    }
}
