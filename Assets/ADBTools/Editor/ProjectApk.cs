using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace ADBTools
{
    public class ProjectApk
    {
        public class ApkData
        {
            public string fullPath;
            public string relativePath;
            public string packageName;
        }

        public List<ApkData> Apks { get; private set; }
        public System.Action onRefreshComplete;

        string packageToolPath;
        Thread thread;

        private void ProcessRefresh()
        {
            Apks = new List<ApkData>();

            string projectDir = System.IO.Directory.GetCurrentDirectory();
            string[] files = System.IO.Directory.GetFiles(projectDir, "*.apk", System.IO.SearchOption.AllDirectories);

            for (int i = 0; i < files.Length; ++i)
            {
                ApkData data = new ApkData();
                data.fullPath = files[i];
                data.relativePath = files[i].Replace(projectDir, "");
                //data.packageName  = ToolPath.AaptPath
                Apks.Add(data);
            }

            for (int i = 0; i < Apks.Count; ++i)
            {
                System.Diagnostics.Process p = new System.Diagnostics.Process();

                p.StartInfo.FileName = packageToolPath;
                p.StartInfo.Arguments = "l -a " + Apks[i].fullPath;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardInput = false;
                p.StartInfo.CreateNoWindow = true;

                p.Start();

                string results = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                p.Close();

                System.IO.StringReader reader = new System.IO.StringReader(results);
                while (reader.Peek() > -1)
                {
                    var line = reader.ReadLine();
                    if (line.TrimStart().StartsWith("A: package"))
                    {
                        Apks[i].packageName = line.Split('"')[1];
                    }
                }
            }

            if (null != onRefreshComplete)
            {
                onRefreshComplete();
            }
        }

        /// <summary>
        /// Refresh list apks those are under project directory.
        /// </summary>
        /// <returns></returns>
        public void RefreshList(string packageToolPath)
        {
            this.packageToolPath = packageToolPath;
            thread = new Thread(ProcessRefresh);
            thread.Start();
        }
    }
}