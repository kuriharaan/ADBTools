using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ADBTools
{
    public class MainWindow : EditorWindow
    {
        bool     toolPathVerified = false;
        bool     apkListUpdated   = false;
        bool     repaintReq       = false;
        float    refreshElapsedTime = 0.0f;
        Settings settings;
        ProjectApk projectApk = new ProjectApk();

        static string settingAssetName = "ADBToolsSettings";

        [MenuItem("Window/ADBTools")]
        static void Open()
        {
            MainWindow window = EditorWindow.GetWindow<MainWindow>("ADBTools");

            window.settings = Resources.Load(settingAssetName) as Settings;
            if( null == window.settings)
            {
                var assetpath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(window));
                var dirpath   = System.IO.Directory.GetParent(assetpath);

                window.settings = CreateInstance<Settings>();
                AssetDatabase.CreateAsset(window.settings, string.Concat(dirpath, "/Resources/", settingAssetName, ".asset"));
                AssetDatabase.Refresh();
            }

            window.apkListUpdated = false;
            window.projectApk.onRefreshComplete += window.OnApksRefreshComplete;
            window.SetVerified( ToolPath.VerifyPath(window.settings.toolsPath) );
        }

        void OnApksRefreshComplete()
        {
            apkListUpdated = true;
            RequestRepaint();
        }

        void RequestRepaint()
        {
            repaintReq = true;
        }

        private void Update()
        {
            if( repaintReq )
            {
                Repaint();
            }
        }

        private void OnGUI()
        {
            if(toolPathVerified)
            {
                if (apkListUpdated)
                {
                    foreach (var apk in projectApk.Apks)
                    {
                        GUILayout.Label("---");
                        GUILayout.Label(apk.relativePath);
                        GUILayout.Label(apk.packageName);
                        if (GUILayout.Button("install"))
                        {
                            Install(apk.fullPath);
                        }
                        if( apk.installed )
                        {
                            if (GUILayout.Button("uninstall"))
                            {
                                Uninstall(apk.packageName);
                            }
                        }
                    }
                }
                else
                {
                    refreshElapsedTime += Time.deltaTime;
                    GUILayout.Label("updating ...");
                }
            }
            else
            {
                GUILayout.Label("Need android tools path");

                if( GUILayout.Button("Select adb directory") )
                {
                    settings.toolsPath = EditorUtility.OpenFolderPanel("Select adb directory", "", "");
                    SetVerified(ToolPath.VerifyPath(settings.toolsPath));
                }
            }
        }

        void SetVerified(bool verified)
        {
            if (toolPathVerified == verified) return;

            toolPathVerified = verified;
            if( toolPathVerified )
            {
                RefreshApkList();
            }
        }

        /// <summary>
        /// refresh the apk list
        /// </summary>
        void RefreshApkList()
        {
            apkListUpdated = false;
            refreshElapsedTime = 0.0f;
            projectApk.RefreshList(ToolPath.AaptPath(settings.toolsPath), ToolPath.AdbPath(settings.toolsPath));
        }

        /// <summary>
        /// install apk
        /// </summary>
        /// <param name="apkPath">path of apk</param>
        void Install(string apkPath)
        {
            if (string.IsNullOrEmpty(apkPath))
            {
                return;
            }

            string adbPath = ToolPath.AdbPath(settings.toolsPath);
            ProcessCall.Execute(adbPath, "install -r " + apkPath);
            ProcessCall.Execute(adbPath, "shell am start -n " + Application.identifier + "/com.unity3d.player.UnityPlayerActivity");
            RefreshApkList();
        }

        /// <summary>
        /// Uninstall apk
        /// </summary>
        /// <param name="packageName">package name</param>
        void Uninstall(string packageName)
        {
            ProcessCall.Execute(ToolPath.AdbPath(settings.toolsPath), "uninstall " + packageName);
            RefreshApkList();
        }

        /// <summary>
        /// Handle output
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OutputHandler(object sender, System.Diagnostics.DataReceivedEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.Data))
            {
                Debug.Log(args.Data);
            }
        }

        /// <summary>
        /// Handle error output
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ErrorOutputHanlder(object sender, System.Diagnostics.DataReceivedEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.Data))
            {
                Debug.Log(args.Data);
            }
        }

        /// <summary>
        /// handle event on process exit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Process_Exit(object sender, System.EventArgs e)
        {
            System.Diagnostics.Process proc = (System.Diagnostics.Process)sender;
            proc.Kill();
        }
    }
}
