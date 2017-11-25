using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

public class LogManager : MonoBehaviour {
    protected static LogManager mInstance = null;
    protected string mLogFolderName = "";
    protected string mSimulationFolderPath = "";

    void Awake()
    {
        mInstance = this;
    }

    public static LogManager Instance
    {
        get
        {
            if (!mInstance)
            {
                mInstance = GameObject.FindObjectOfType<LogManager>();
            }
            return mInstance;
        }
    }

    public bool IsLogFolderUpdated
    {
        get { return !string.IsNullOrEmpty(mLogFolderName); }
    }

    public string LogDirectoryPath
    {
        get
        {
            if (!Directory.Exists(Application.persistentDataPath))
            {
                Directory.CreateDirectory(Application.persistentDataPath);
            }
            string simulationPath = Path.Combine(Application.persistentDataPath, "Logs");
            if (!Directory.Exists(simulationPath))
            {
                Directory.CreateDirectory(simulationPath);
            }
            return simulationPath;
        }
    }

    public void ResetState()
    {
        DateTime dt = DateTime.Now;
        mLogFolderName = dt.ToString("yyyy-MM-dd_HH-mm-ss");
        mSimulationFolderPath = Path.Combine(LogDirectoryPath, mLogFolderName);
        if (!Directory.Exists(mSimulationFolderPath))
        {
            Directory.CreateDirectory(mSimulationFolderPath);
        }
    }

    public string GetFilePath(string fileName)
    {
        return Path.Combine(mSimulationFolderPath, fileName);
    }

    public void Log(int timeTick)
    {
    }

    private void WriteContent(string fileName, string content)
    {
        string filePath = GetFilePath(fileName);
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine(content);
        }
    }

    private Dictionary<string, int> Count(List<string> data)
    {
        Dictionary<string, int> result = new Dictionary<string, int>();
        for (int i = 0; i < data.Count; ++i)
        {
            string name = data[i];
            if (result.ContainsKey(name))
            {
                result[name]++;
            }
            else
            {
                result[name] = 1;
            }
        }
        return result;
    }



    public static float Average(List<float> data)
    {
        if (data.Count > 0)
        {
            float sum = 0f;
            for (int i = 0; i < data.Count; ++i)
            {
                sum += data[i];
            }
            return sum / data.Count;
        }
        return 0f;
    }
}
