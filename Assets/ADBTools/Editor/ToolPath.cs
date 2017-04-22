using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ADBTools
{
    public class ToolPath
    {
        /// <summary>
        /// Verify if the specific directory contains adb
        /// </summary>
        /// <param name="dirpath"></param>
        /// <returns></returns>
        public static bool VerifyPath(string sdkPath)
        {
            if( !System.IO.Directory.Exists(sdkPath) )
            {
                return false;
            }

            if( !System.IO.File.Exists(AdbPath(sdkPath)) )
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// returns
        /// </summary>
        /// <param name="dirpath"></param>
        /// <returns></returns>
        public static string AdbPath(string sdkPath)
        {
            var adbPath = sdkPath;
            if (!adbPath.EndsWith("/"))
            {
                adbPath += "/";
            }
            adbPath += "platform-tools/adb.exe";

            return adbPath;
        }

        public static string AaptPath(string sdkPath)
        {
            if (!sdkPath.EndsWith("/"))
            {
                sdkPath += "/";
            }
            string[] directories = System.IO.Directory.GetDirectories(sdkPath + "build-tools");

            for( int i = 0; i < directories.Length; ++i )
            {
                string[] files = System.IO.Directory.GetFiles(directories[i], "aapt.exe", System.IO.SearchOption.AllDirectories);

                if( files.Length > 0 )
                {
                    return files[0];
                }
            }

            return null;
        }
    }
}