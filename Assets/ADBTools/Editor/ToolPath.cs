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
        public static bool VerifyPath(string dirpath)
        {
            if( !System.IO.Directory.Exists(dirpath) )
            {
                return false;
            }

            if( !System.IO.File.Exists(AdbPath(dirpath)) )
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
        public static string AdbPath(string dirpath)
        {
            var adbPath = dirpath;
            if (!adbPath.EndsWith("/"))
            {
                adbPath += "/";
            }
            adbPath += "adb.exe";

            return adbPath;
        }
    }
}