using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessCall
{
    static public string Execute(string file, string args)
    {
        System.Diagnostics.Process p = new System.Diagnostics.Process();

        p.StartInfo.FileName = file;
        p.StartInfo.Arguments = args;
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.RedirectStandardInput = false;
        p.StartInfo.CreateNoWindow = true;

        p.Start();

        string results = p.StandardOutput.ReadToEnd();
        p.WaitForExit();
        p.Close();

        return results;
    }
}
