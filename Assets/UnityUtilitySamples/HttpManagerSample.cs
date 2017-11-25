using UnityEngine;
using System.Collections;

public class HttpManagerSample : MonoBehaviour {

	// Use this for initialization
	void Start () {
		HttpManager.Instance.BaseUrl = "";
		HttpManager.Instance.GET ("http://www.google.com", text => {
			Debug.Log (text);
				});
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
