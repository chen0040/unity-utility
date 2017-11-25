using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;


public class ProcessManager
{

    public static float NormalRandom(float mu, float sigma)
    {
        float u1 = Random.Range(0f, 1f);
        float u2 = Random.Range(0f, 1f);
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) *
                        Mathf.Sin(2.0f * Mathf.PI * u2); 
        float randNormal =
                        mu + sigma * randStdNormal;
        return randNormal;
    }

       

    public static bool StartProcessAndForget(string exeName, params string[] args)
    {
        string exePath = Path.Combine(Path.Combine(Application.dataPath, "Resources"), exeName);

        if (File.Exists(exePath))
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                }
            };

            if (args.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < args.Length; ++i)
                {
                    if (i == 0)
                    {
                        sb.Append(args[i]);
                    }
                    else
                    {
                        sb.AppendFormat(" {0}", args[i]);
                    }
                }
                process.StartInfo.Arguments = sb.ToString();
            }

            process.Start();
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool RunProcess(string exeName, params string[] args)
    {
        string exePath = Path.Combine(Path.Combine(Application.dataPath, "Resources"), exeName);

        if (File.Exists(exePath))
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                }
            };

            if (args.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < args.Length; ++i)
                {
                    if (i == 0)
                    {
                        sb.Append(args[i]);
                    }
                    else
                    {
                        sb.AppendFormat(" {0}", args[i]);
                    }
                }
                process.StartInfo.Arguments = sb.ToString();
            }

            process.Start();
            process.WaitForExit();
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void ShowExplorer(string itemPath)
    {
        itemPath = itemPath.Replace(@"/", @"\");   // explorer doesn't like front slashes
        System.Diagnostics.Process.Start("explorer.exe", "/select," + itemPath);
    }
}
