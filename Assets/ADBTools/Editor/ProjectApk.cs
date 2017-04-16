using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ADBTools
{
    public class ProjectApk
    {
        public struct ApkData
        {
            public string fullPath;
            public string relativePath;
        }

        public List<ApkData> Apks { get; private set; }

        /// <summary>
        /// Refresh list apks those are under project directory.
        /// </summary>
        /// <returns></returns>
        public List<ApkData> RefreshList()
        {
            Apks = new List<ApkData>();

            string projectDir = System.IO.Directory.GetCurrentDirectory();
            string[] files = System.IO.Directory.GetFiles(projectDir, "*.apk", System.IO.SearchOption.AllDirectories);

            for( int i = 0; i < files.Length; ++i )
            {
                ApkData data = new ApkData();
                data.fullPath     = files[i];
                data.relativePath = files[i].Replace(projectDir, "");
                Apks.Add(data);
            }

            return Apks;
        }
    }
}