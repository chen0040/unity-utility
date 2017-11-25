using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour
{

    public GUISkin mouseCursorSkin;
    public GUISkin selectBoxSkin;
    public GUISkin hoverBoxSkin;

    private CursorState activeCursorState;
    public Texture2D activeCursor;
    public Texture2D selectCursor, leftCursor, rightCursor, upCursor, downCursor;
    public Texture2D[] moveCursors;
    private int currentFrame = 0;
        
    // Use this for initialization
    void Start()
    {
            
    }

    void OnGUI()
    {
        DrawMouseCursor();
    }

    private void DrawMouseCursor()
    {
        bool mouseOverHud = (!MouseInBounds() && activeCursorState != CursorState.PanRight && activeCursorState != CursorState.PanUp && activeCursorState != CursorState.PanLeft && activeCursorState != CursorState.PanDown);
        if (mouseOverHud)
        {
            Screen.showCursor = true;
        }
        else
        {
            Screen.showCursor = false;
            GUI.skin = mouseCursorSkin;
            GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));
            UpdateCursorAnimation();
            Rect cursorPosition = GetCursorDrawPosition();
            GUI.Label(cursorPosition, activeCursor);
            GUI.EndGroup();
        }
    }

    public bool MouseInBounds()
    {
        return false;
    }

    private bool Contains(Rect rect, float x, float y)
    {
        //Debug.Log(string.Format("x: {0}, y: {1}, rect: {2}", x, y, rect));
        bool isContained = x >= rect.xMin && x <= rect.xMax && y >= rect.yMin && y <= rect.yMax;
        //if (!isContained)
        //{
        //    Debug.Log(string.Format("x: {0}, y: {1}, rect: [{2}, {3}, {4}, {5}]", x, y, rect.xMin, rect.xMax, rect.yMin, rect.yMax));
        //}
        return isContained;
    }

    public void SetCursorState(CursorState newState)
    {
        activeCursorState = newState;
        switch (newState)
        {
            case CursorState.Select:
                activeCursor = selectCursor;
                break;
            case CursorState.Move:
                currentFrame = (int)Time.time % moveCursors.Length;
                activeCursor = moveCursors[currentFrame];
                break;
            case CursorState.PanLeft:
                activeCursor = leftCursor;
                break;
            case CursorState.PanRight:
                activeCursor = rightCursor;
                break;
            case CursorState.PanUp:
                activeCursor = upCursor;
                break;
            case CursorState.PanDown:
                activeCursor = downCursor;
                break;
            default: break;
        }
    }

    private void UpdateCursorAnimation()
    {
        //sequence animation for cursor (based on more than one image for the cursor)
        //change once per second, loops through array of images
        if (activeCursorState == CursorState.Move)
        {
            currentFrame = (int)Time.time % moveCursors.Length;
            activeCursor = moveCursors[currentFrame];
        }
    }

    private Rect GetCursorDrawPosition()
    {
        //set base position for custom cursor image
        float leftPos = Input.mousePosition.x;
        float topPos = Screen.height - Input.mousePosition.y; //screen draw coordinates are inverted
        //adjust position base on the type of cursor being shown
        if (activeCursorState == CursorState.PanRight) leftPos = Screen.width - activeCursor.width;
        else if (activeCursorState == CursorState.PanDown) topPos = Screen.height - activeCursor.height;
        else if (activeCursorState == CursorState.Move || activeCursorState == CursorState.Select)
        {
            topPos -= activeCursor.height / 2;
            leftPos -= activeCursor.width / 2;
        }
        return new Rect(leftPos, topPos, activeCursor.width, activeCursor.height);
    }

}
