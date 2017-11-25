using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraManager : MonoBehaviour {

	private static Vector3 invalidPosition = new Vector3(-99999, -99999, -99999);
	private static Bounds invalidBounds = new Bounds(new Vector3(-99999, -99999, -99999), new Vector3(0, 0, 0));
	public static Vector3 InvalidPosition { get { return invalidPosition; } }
	public static Bounds InvalidBounds { get { return invalidBounds; } }
    public static int ScrollWidth { get { return 15; } }
    public static float ScrollSpeed { get { return 25; } }

	public static GameObject FindHitObject(Vector3 origin)
	{
		Ray ray = Camera.main.ScreenPointToRay(origin);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit)) return hit.collider.gameObject;
		return null;
	}
	
	public static Vector3 FindHitPoint(Vector3 origin)
	{
		Ray ray = Camera.main.ScreenPointToRay(origin);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit)) return hit.point;
		return CameraManager.InvalidPosition;
	}

    public static Rect CalculateSelectionBoxOnGUI(Camera cam, Component comp)
    {
        Rect playingArea = new Rect(Screen.width - CameraManager.ScrollWidth,
            CameraManager.ScrollWidth,
            Screen.width - CameraManager.ScrollWidth * 2,
            Screen.height - CameraManager.ScrollWidth * 2);
        return CalculateSelectionBoxOnGUI(cam, comp, playingArea);
    }

    public static Rect CalculateSelectionBoxOnGUI(Camera cam, Component comp, Rect playingArea)
    {
        Bounds selectionBounds = CalculateBounds(comp);
        return CalculateSelectionBoxOnGUI(cam, selectionBounds, playingArea);
    }


	public static Bounds CalculateBounds(Component comp)
	{
		Bounds selectionBounds = new Bounds(comp.transform.position, Vector3.zero);
		Renderer[] renderers = comp.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < renderers.Length; ++i )
		{
			selectionBounds.Encapsulate(renderers[i].bounds);
		}
		return selectionBounds;
	}

	public static Rect CalculateSelectionBoxOnGUI(Camera cam, Bounds selectionBounds, Rect playingArea)
	{
		//shorthand for the coordinates of the centre of the selection bounds
		float cx = selectionBounds.center.x;
		float cy = selectionBounds.center.y;
		float cz = selectionBounds.center.z;
		//shorthand for the coordinates of the extents of the selection bounds
		float ex = selectionBounds.extents.x;
		float ey = selectionBounds.extents.y;
		float ez = selectionBounds.extents.z;
		
		//Determine the screen coordinates for the corners of the selection bounds
		List<Vector3> corners = new List<Vector3>();
		corners.Add(cam.WorldToScreenPoint(new Vector3(cx + ex, cy + ey, cz + ez)));
		corners.Add(cam.WorldToScreenPoint(new Vector3(cx + ex, cy + ey, cz - ez)));
		corners.Add(cam.WorldToScreenPoint(new Vector3(cx + ex, cy - ey, cz + ez)));
		corners.Add(cam.WorldToScreenPoint(new Vector3(cx - ex, cy + ey, cz + ez)));
		corners.Add(cam.WorldToScreenPoint(new Vector3(cx + ex, cy - ey, cz - ez)));
		corners.Add(cam.WorldToScreenPoint(new Vector3(cx - ex, cy - ey, cz + ez)));
		corners.Add(cam.WorldToScreenPoint(new Vector3(cx - ex, cy + ey, cz - ez)));
		corners.Add(cam.WorldToScreenPoint(new Vector3(cx - ex, cy - ey, cz - ez)));
		
		//Determine the bounds on screen for the selection bounds
		Bounds screenBounds = new Bounds(corners[0], Vector3.zero);
		for (int i = 1; i < corners.Count; i++)
		{
			screenBounds.Encapsulate(corners[i]);
		}
		
		//Screen coordinates start in the bottom left corner, rather than the top left corner
		//this correction is needed to make sure the selection box is drawn in the correct place
		float selectBoxTop = playingArea.height - (screenBounds.center.y + screenBounds.extents.y);
		float selectBoxLeft = screenBounds.center.x - screenBounds.extents.x;
		float selectBoxWidth = 2 * screenBounds.extents.x;
		float selectBoxHeight = 2 * screenBounds.extents.y;
		
		return new Rect(selectBoxLeft, selectBoxTop, selectBoxWidth, selectBoxHeight);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
