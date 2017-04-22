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
            public bool   installed;
        }

        public List<ApkData> Apks { get; private set; }
        public System.Action onRefreshComplete;

        string aaptPath;
        string adbPath;
        Thread thread;

        void ProcessRefresh()
        {
            Apks = new List<ApkData>();

            string projectDir = System.IO.Directory.GetCurrentDirectory();
            string[] files = System.IO.Directory.GetFiles(projectDir, "*.apk", System.IO.SearchOption.AllDirectories);

            for (int i = 0; i < files.Length; ++i)
            {
                ApkData data = new ApkData();
                data.fullPath = files[i];
                data.relativePath = files[i].Replace(projectDir, "");
                Apks.Add(data);
            }
            UpdatePackageNames();
            UpdateInstalled();

            if (null != onRefreshComplete)
            {
                onRefreshComplete();
            }
        }

        /// <summary>
        /// Update apk package name through call Aapt
        /// </summary>
        void UpdatePackageNames()
        {
            foreach( var apk in Apks )
            {
                string results = ProcessCall.Execute(aaptPath, "l -a " + apk.fullPath);

                System.IO.StringReader reader = new System.IO.StringReader(results);
                while (reader.Peek() > -1)
                {
                    var line = reader.ReadLine();
                    if (line.TrimStart().StartsWith("A: package"))
                    {
                        apk.packageName = line.Split('"')[1];
                    }
                }
            }
        }

        public void UpdateInstalled()
        {
            foreach (var apk in Apks)
            {

                string results = ProcessCall.Execute(adbPath, "shell pm list packages " + apk.packageName);

                System.IO.StringReader reader = new System.IO.StringReader(results);
                while (reader.Peek() > -1)
                {
                    var line = reader.ReadLine();
                    var words = line.Split(':');
                    if (2 <= words.Length && apk.packageName == words[1])
                    {
                        apk.installed = true;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Refresh list apks those are under project directory.
        /// </summary>
        /// <returns></returns>
        public void RefreshList(string aaptPath, string adbPath)
        {
            this.aaptPath = aaptPath;
            this.adbPath  = adbPath;
            thread = new Thread(ProcessRefresh);
            thread.Start();
        }
    }
}