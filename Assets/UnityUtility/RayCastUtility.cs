using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class RayCastUtility
{
	public static GameObject GetClickedObject(out RaycastHit hit)
	{
		GameObject target = null;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray.origin, ray.direction * 10, out hit))
		{
			target = hit.collider.gameObject;
		}

		return target;
	}

	public static Vector3 GetHitPointOnLayerName(String name)
	{
		int layerId = LayerMask.GetMask(name);
		return GetHitPointOnLayer(layerId);
	}

	public static GameObject GetClickedObjectOnLayerName(String name)
	{
		int layerId = LayerMask.GetMask(name);
		return GetClickedObjectOnLayer(layerId);
	}

	public static GameObject GetClickedObjectOnLayer(int layerId)
	{
		RaycastHit hit;
		GameObject target = null;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray.origin, ray.direction * 10, out hit))
		{
			target = hit.collider.gameObject;
		}

		return target;
	}

	public static Vector3 GetHitPointOnLayer(int layerId)
	{
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerId))
		{
			return hit.point;
		}

		return Vector3.zero;
	}

}
