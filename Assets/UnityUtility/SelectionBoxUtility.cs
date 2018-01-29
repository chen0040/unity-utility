using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SelectionBoxUtility
{
	public static Rect CalculateSelectionBoxOnGUI(Component comp)
	{
		Bounds selectionBounds = CalculateBounds(comp);
		Rect selectBox = CalculateSelectionBoxOnGUI(Camera.main, selectionBounds);
		return selectBox;
	}

	public static Bounds CalculateBounds(Component comp)
	{
		Bounds selectionBounds = new Bounds(comp.transform.position, Vector3.zero);
		Renderer[] renderers = comp.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < renderers.Length; ++i)
		{
			selectionBounds.Encapsulate(renderers[i].bounds);
		}
		return selectionBounds;
	}

	public static Rect CalculateSelectionBoxOnGUI(Camera cam, Bounds selectionBounds) //, Rect playingArea)
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
		float selectBoxTop = Screen.height - (screenBounds.center.y + screenBounds.extents.y);
		float selectBoxLeft = screenBounds.center.x - screenBounds.extents.x;
		float selectBoxWidth = 2 * screenBounds.extents.x;
		float selectBoxHeight = 2 * screenBounds.extents.y;

		return new Rect(selectBoxLeft, selectBoxTop, selectBoxWidth, selectBoxHeight);
	}

	static Texture2D _whiteTexture;
	public static Texture2D WhiteTexture
	{
		get
		{
			if (_whiteTexture == null)
			{
				_whiteTexture = new Texture2D(1, 1);
				_whiteTexture.SetPixel(0, 0, Color.white);
				_whiteTexture.Apply();
			}

			return _whiteTexture;
		}
	}

	public static void DrawScreenRect(Rect rect, Color color)
	{
		GUI.color = color;
		GUI.DrawTexture(rect, WhiteTexture);
		GUI.color = Color.white;
	}

	public static void DrawScreenRectBorder(Rect rect, float thickness, Color color)
	{
		// Top
		DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
		// Left
		DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
		// Right
		DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
		// Bottom
		DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
	}
}
