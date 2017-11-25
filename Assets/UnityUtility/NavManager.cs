using UnityEngine;
using System.Collections;


public class NavManager : MonoBehaviour
{

	private NavComp[] mNavCompCollection;
	private static NavManager mInstance;

	void Awake()
	{
	    mNavCompCollection = FindObjectsOfType<NavComp>();

	    if (mInstance == null)
	    {
	        mInstance = this;
	    }
	    else
	    {
	        Debug.Log("Should not reach here");
	    }
	}

	public static NavManager Instance
	{
	    get
	    {
	        return mInstance;
	    }
	}

	public NavComp FindNearestNavComp(Transform transform)
	{
	    Vector3 source = transform.position;
	    float minDistance = float.MaxValue;
	    NavComp selectedNavComp = null;
	    for (int i = 0; i < mNavCompCollection.Length; ++i )
	    {
	        NavComp nc = mNavCompCollection[i];
	        Vector3 candidate = nc.gameObject.transform.position;
	        float dX = source.x - candidate.x;
	        float dZ = source.z - candidate.z;
	        //float dY = source.y - candidate.y;

	        float distance = Mathf.Sqrt(dX * dX + dZ * dZ);
	        if (distance < minDistance)
	        {
	            minDistance = distance;
	            selectedNavComp = nc;
	        }
	    }

	    if (selectedNavComp == null)
	    {
	        Debug.Log("selectedNavComp == null!!!!");
	    }

	    return selectedNavComp;
	}

	public NavComp FindNearestNavCompOtherThan(Transform transform, NavComp other)
	{
	    Vector3 source = transform.position;
	    float minDistance = float.MaxValue;
	    NavComp selectedNavComp = null;
	    for (int i = 0; i < mNavCompCollection.Length; ++i)
	    {   
	        NavComp nc = mNavCompCollection[i];
	        if (nc == other) continue;

	        Vector3 candidate = nc.gameObject.transform.position;
	        float dX = source.x - candidate.x;
	        float dZ = source.z - candidate.z;
	        //float dY = source.y - candidate.y;

	        float distance = Mathf.Sqrt(dX * dX + dZ * dZ);
	        if (distance < minDistance)
	        {
	            minDistance = distance;
	            selectedNavComp = nc;
	        }
	    }

	    if (selectedNavComp == null)
	    {
	        Debug.Log("selectedNavComp == null!!!!");
	    }

	    return selectedNavComp;
	}
}
