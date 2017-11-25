using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class HttpManager : MonoBehaviour
{
    protected static HttpManager mInstance = null;
    private string mBaseUrl = "http://localhost";

    public string BaseUrl
    {
        get { return mBaseUrl; }
        set { mBaseUrl = value; }
    }

    void Awake()
    {
        mInstance = this;
        string[] arguments = Environment.GetCommandLineArgs();
        if (arguments.Length > 1)
        {
            mBaseUrl = arguments[1];
        }
    }

    public static HttpManager Instance
    {
        get
        {
            if (!mInstance)
            {
                mInstance = GameObject.FindObjectOfType<HttpManager>();
            }
            return mInstance;
        }
    }

    public string WebPortalUrl
    {
        get
        {
            return mBaseUrl;
        }
    }

    public WWW GET(string relativeUrl, Action<string> handler)
    {
        return _GET(GetFullUrl(relativeUrl), handler);
    }

    public WWW _GET(string url, Action<string> handler)
    {
        WWW www = new WWW(url);
        StartCoroutine(WaitForRequest(www, handler));
        return www;
    }

    private WWW FormPOST(string url, Dictionary<string, string> post, Action<string> handler)
    {
        WWWForm form = new WWWForm();
        foreach (KeyValuePair<string, string> post_arg in post)
        {
            form.AddField(post_arg.Key, post_arg.Value);
        }
        WWW www = new WWW(url, form);
        StartCoroutine(WaitForRequest(www, handler));
        return www;
    }

    public string ToJsonString(Dictionary<string, string> map)
    {
        JObject o = new JObject();
        foreach (string name in map.Keys)
        {
            string value = map[name];
            o[name] = value;
        }

        return o.ToString();
    }

    public WWW POST(string relativeUrl, Action<string> handler)
    {
        Dictionary<string, string> temp = new Dictionary<string, string>();
        temp["time"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        temp["command"] = "POST";
        return JsonPOST(GetFullUrl(relativeUrl), ToJsonString(temp), handler);
    }

    public WWW POST(string relativeUrl, Dictionary<string, string> parameters, Action<string> handler)
    {
        parameters["time"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        parameters["command"] = "POST";
        return JsonPOST(GetFullUrl(relativeUrl), ToJsonString(parameters), handler);
    }

    public WWW DELETE(string relativeUrl, Action<string> handler)
    {
        Dictionary<string, string> temp = new Dictionary<string, string>();
        temp["time"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        temp["command"] = "DELETE";

        return JsonPOST(GetFullUrl(relativeUrl), ToJsonString(temp), handler);
    }


    public WWW JsonPOST(string url, string jsonString, Action<string> handler)
    {
        var encoding = new System.Text.UTF8Encoding();
        var postHeader = new Dictionary<string, string>();

        postHeader.Add("Content-Type", "text/json");

        WWW www = new WWW(url, encoding.GetBytes(jsonString), postHeader);
        StartCoroutine(WaitForRequest(www, handler));
        return www;
    }

    protected string GetFullUrl(string relativeUrl)
    {
        return mBaseUrl + relativeUrl;
    }

    private IEnumerator WaitForRequest(WWW www, Action<string> handler)
    {
        yield return www;
        // check for errors
        if (www.error == null)
        {
            handler(www.text);
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
