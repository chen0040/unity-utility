using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour
{
    private float initialFOV;
    public float minYRotate = 0f;
    public float maxYRotate = 360f;
    private Quaternion origRotation;
    private Vector3 origPosition;
        
    void Start()
    {
        initialFOV = camera.fieldOfView;
        origPosition = transform.position;
        origRotation = transform.rotation;
    }

    void LateUpdate()
    {
        bool cameraMoved = false;
        bool cameraRotated = false;
        bool cameraZoomed = false;

        ControlCameraByKeyCode(ref cameraZoomed, ref cameraRotated, ref cameraMoved);

        MoveCamera(ref cameraMoved);
        RotateCamera(ref cameraRotated);
        OrbitCamera(ref cameraRotated);

        MouseActivity();
    }

    public void Reset()
    {
        transform.position = origPosition;
        transform.rotation = origRotation;
        camera.fieldOfView = initialFOV;
    }

        

    public void ZoomCamera(bool isZoomIn, bool isZoomOut)
    {
        float zoomDirection = 0f;
        if (isZoomIn) zoomDirection = 1f;
        else if (isZoomOut) zoomDirection = -1f;
        camera.fieldOfView -= zoomDirection * Time.deltaTime * CameraManager.ScrollSpeed;
    }

    public void OrbitCamera(ref bool cameraRotated)
    {
        bool isCtrlKeyDown = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        if (isCtrlKeyDown && Input.GetMouseButton(0))
        {
            Vector3 target = Vector3.zero;
            float y_rotate = Input.GetAxis("Mouse X") * CameraManager.RotateAmount;
            float x_rotate = Input.GetAxis("Mouse Y") * CameraManager.RotateAmount;
            OrbitCamera(target, y_rotate, x_rotate);
        }
    }

    public void OrbitCamera(Vector3 target, float y_rotate, float x_rotate)
    {
        Vector3 angles = transform.eulerAngles;
        angles.z = 0;
        transform.eulerAngles = angles;
        transform.RotateAround(target, Vector3.up, y_rotate);
        transform.RotateAround(target, Vector3.left, x_rotate);

        transform.LookAt(target);
    }

    public void OrbitCamera(bool isOrbitUp, bool isOrbitDown, bool isOrbitLeft, bool isOrbitRight)
    {
        Vector3 target = Vector3.zero;
        if (CameraManager.Instance.SelectedObject != null)
        {
            target = CameraManager.Instance.SelectedObject.transform.position;
        }

        float xRotateDirection = 0f;
        float yRotateDirection = 0f;
        if (Input.GetKey(KeyCode.UpArrow)) xRotateDirection = 1f;
        if (Input.GetKey(KeyCode.DownArrow)) xRotateDirection = -1f;
        if (Input.GetKey(KeyCode.LeftArrow)) yRotateDirection = 1f;
        if (Input.GetKey(KeyCode.RightArrow)) yRotateDirection = -1f;

        OrbitCamera(target, yRotateDirection * Time.deltaTime * CameraManager.RotateAmount,
            -xRotateDirection * Time.deltaTime * CameraManager.RotateAmount);
    }

    private void RotateCamera(bool isRotateUp, bool isRotateDown, bool isRotateLeft, bool isRotateRight)
    {
        Vector3 origin = transform.eulerAngles;
        Vector3 destination = origin;

        float xRotateDirection = 0f;
        float yRotateDirection = 0f;
        if (isRotateUp) xRotateDirection = 1f;
        if (isRotateDown) xRotateDirection = -1f;
        if (isRotateLeft) yRotateDirection = -1f;
        if (isRotateRight) yRotateDirection = 1f;

        destination.x -= xRotateDirection * Time.deltaTime * CameraManager.RotateAmount;
        destination.y += yRotateDirection * Time.deltaTime * CameraManager.RotateAmount;

        //if a change in position is detected perform the necessary update
        if (destination != origin)
        {
            transform.eulerAngles = destination;
        }
    }

    private void ControlCameraByKeyCode(ref bool cameraZoomed, ref bool cameraRotated, ref bool cameraMoved)
    {
        bool isUpArrowKeyPressed = Input.GetKey(KeyCode.UpArrow);
        bool isDownArrowKeyPressed = Input.GetKey(KeyCode.DownArrow);
        bool isUpDownArrowKeyPressed = isUpArrowKeyPressed || isDownArrowKeyPressed;

        bool isLeftArrowKeyPressed = Input.GetKey(KeyCode.LeftArrow);
        bool isRightArrowKeyPressed = Input.GetKey(KeyCode.RightArrow);
        bool isLeftRightArrowKeyPressed = isLeftArrowKeyPressed || isRightArrowKeyPressed;

        bool isArrowKeyPressed = isUpDownArrowKeyPressed || isLeftRightArrowKeyPressed;
        if (isArrowKeyPressed)
        {
            bool isAltKeyPressed = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
            bool isCtrlKeyPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            bool isShiftKeyPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

            bool mustZoom = isUpDownArrowKeyPressed && isAltKeyPressed && !cameraZoomed;
            bool mustOrbit = isCtrlKeyPressed && !cameraRotated;
            bool mustRotate = isShiftKeyPressed && !cameraRotated;

            if (mustZoom) //Zoom In | Zoom Out
            {
                ZoomCamera(isUpArrowKeyPressed, isDownArrowKeyPressed);
                cameraZoomed = true;
            }
            else if (mustOrbit)
            {
                OrbitCamera(isUpArrowKeyPressed, isDownArrowKeyPressed, isLeftArrowKeyPressed, isRightArrowKeyPressed);
                cameraRotated = true;
            }
            else if (mustRotate)
            {
                RotateCamera(isUpArrowKeyPressed, isDownArrowKeyPressed, isLeftArrowKeyPressed, isRightArrowKeyPressed);
                cameraRotated = true;
            }
            else if (!cameraMoved)
            {
                MoveCamera(isUpArrowKeyPressed, isDownArrowKeyPressed, isLeftArrowKeyPressed, isRightArrowKeyPressed);
                cameraMoved = true;
            }
        }
        else if (Input.GetKey(KeyCode.U))
        {
            ElevateCamera(true, false);
            cameraMoved = true;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            ElevateCamera(false, true);
            cameraMoved = true;
        }
    }

    private void ElevateCamera(bool isMoveUp, bool isMoveDown)
    {
        Vector3 movement = new Vector3(0, 0, 0);

        
        //vertical camera movement
        if (isMoveUp)
        {
            movement.y += CameraManager.ScrollSpeed;
            CameraManager.Instance.PlayerHUD.SetCursorState(CursorState.Select);
        }
        else if (isMoveDown)
        {
            movement.y -= CameraManager.ScrollSpeed;
            CameraManager.Instance.PlayerHUD.SetCursorState(CursorState.Select);
        }

        //make sure movement is in the direction the camera is pointing
        //but ignore the vertical tilt of the camera to get sensible scrolling
        movement = camera.transform.TransformDirection(movement);

        //calculate desired camera position based on received input
        Vector3 origin = transform.position;
        Vector3 destination = origin;
        destination.y += movement.y;

        //if a change in position is detected perform the necessary update
        if (destination != origin)
        {
            transform.position = Vector3.MoveTowards(origin, destination, Time.deltaTime * CameraManager.ScrollSpeed);
        }
    }

    private void MoveCamera(bool isMoveForward, bool isMoveBackward, bool isMoveLeft, bool isMoveRight)
    {
        Vector3 movement = new Vector3(0, 0, 0);

        //horizontal camera movement
        if (isMoveLeft)
        {
            movement.x -= CameraManager.ScrollSpeed;
            CameraManager.Instance.PlayerHUD.SetCursorState(CursorState.PanLeft);
        }
        else if (isMoveRight)
        {
            movement.x += CameraManager.ScrollSpeed;
            CameraManager.Instance.PlayerHUD.SetCursorState(CursorState.PanRight);
        }

        //vertical camera movement
        if (isMoveForward)
        {
            movement.z += CameraManager.ScrollSpeed;
            CameraManager.Instance.PlayerHUD.SetCursorState(CursorState.PanDown);
        }
        else if (isMoveBackward)
        {
            movement.z -= CameraManager.ScrollSpeed;
            CameraManager.Instance.PlayerHUD.SetCursorState(CursorState.PanUp);
        }

        //make sure movement is in the direction the camera is pointing
        //but ignore the vertical tilt of the camera to get sensible scrolling
        movement = camera.transform.TransformDirection(movement);

        //calculate desired camera position based on received input
        Vector3 origin = transform.position;
        Vector3 destination = origin;
        destination.x += movement.x;
        destination.z += movement.z;


        //if a change in position is detected perform the necessary update
        if (destination != origin)
        {
            transform.position = Vector3.MoveTowards(origin, destination, Time.deltaTime * CameraManager.ScrollSpeed);
        }
    }

    private void MoveCamera(ref bool cameraMoved)
    {
        if (cameraMoved) return;

        float xpos = Input.mousePosition.x;
        float ypos = Input.mousePosition.y;
        Vector3 movement = new Vector3(0, 0, 0);
        bool mouseScroll = false;

        //horizontal camera movement
        if (xpos >= 0 && xpos < CameraManager.ScrollWidth)
        {
            movement.x -= CameraManager.ScrollSpeed;
            CameraManager.Instance.PlayerHUD.SetCursorState(CursorState.PanLeft);
            mouseScroll = true;
        }
        else if (xpos <= Screen.width && xpos > Screen.width - CameraManager.ScrollWidth)
        {
            movement.x += CameraManager.ScrollSpeed;
            CameraManager.Instance.PlayerHUD.SetCursorState(CursorState.PanRight);
            mouseScroll = true;
        }

        bool dragged = false;
        float fov = camera.fieldOfView;
        bool isAltKeyDown = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
        if (!isAltKeyDown && Input.GetMouseButton(1))
        {
            movement.x -= Input.GetAxis("Mouse X") * CameraManager.DragSpeed;
            movement.z -= Input.GetAxis("Mouse Y") * CameraManager.DragSpeed;
            cameraMoved = true;
            dragged = true;
        }

        //vertical camera movement
        if (ypos >= 0 && ypos < CameraManager.ScrollWidth)
        {
            movement.z -= CameraManager.ScrollSpeed;
            CameraManager.Instance.PlayerHUD.SetCursorState(CursorState.PanDown);
            mouseScroll = true;
        }
        else if (ypos <= Screen.height && ypos > Screen.height - CameraManager.ScrollWidth)
        {
            movement.z += CameraManager.ScrollSpeed;
            CameraManager.Instance.PlayerHUD.SetCursorState(CursorState.PanUp);
            mouseScroll = true;
        }

        //make sure movement is in the direction the camera is pointing
        //but ignore the vertical tilt of the camera to get sensible scrolling
        movement = camera.transform.TransformDirection(movement);
        movement.y = 0;

        //away from ground movement
        movement.y -= CameraManager.ScrollSpeed * Input.GetAxis("Mouse ScrollWheel");

        //calculate desired camera position based on received input
        Vector3 origin = transform.position;
        Vector3 destination = origin;
        destination.x += movement.x;
        destination.y += movement.y;
        destination.z += movement.z;

        //limit away from ground movement to be between a minimum and maximum distance
        //if (destination.y > ResourceManager.MaxCameraHeight)
        //{
        //    destination.y = ResourceManager.MaxCameraHeight;
        //}
        //else if (destination.y < ResourceManager.MinCameraHeight)
        //{
        //    destination.y = ResourceManager.MinCameraHeight;
        //}

        //if a change in position is detected perform the necessary update
        if (dragged)
        {
            transform.position = Vector3.MoveTowards(origin, destination, Time.deltaTime * CameraManager.DragSpeed * fov);
        }
        else if (destination != origin)
        {
            transform.position = Vector3.MoveTowards(origin, destination, Time.deltaTime * CameraManager.ScrollSpeed);
            cameraMoved = true;
        }

        //set cursor back to default state it should be in
        if (!mouseScroll)
        {
            CameraManager.Instance.PlayerHUD.SetCursorState(CursorState.Select);
        }
    }

    private void RotateCamera(ref bool cameraRotated)
    {
        if (cameraRotated) return;

        Vector3 origin = transform.eulerAngles;
        Vector3 destination = origin;

        //detect rotation amount if ALT is being held and the Right mouse button is down
        if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetMouseButton(1))
        {
            destination.x -= Input.GetAxis("Mouse Y") * CameraManager.RotateAmount;
            destination.y += Input.GetAxis("Mouse X") * CameraManager.RotateAmount;
        }

        //if a change in position is detected perform the necessary update
        if (destination != origin)
        {
            transform.eulerAngles = Vector3.MoveTowards(origin, destination, Time.deltaTime * CameraManager.RotateSpeed);
            cameraRotated = true;
        }
    }

    private void MouseActivity()
    {
        if (Input.GetMouseButtonDown(0)) LeftMouseClick();
        else if (Input.GetMouseButtonDown(1)) RightMouseClick();
        MouseHover();
    }

    private void LeftMouseClick()
    {
        if (CameraManager.Instance.PlayerHUD.MouseInBounds())
        {
            GameObject hitObject = CameraManager.FindHitObject(Input.mousePosition);
            Vector3 hitPoint = CameraManager.FindHitPoint(Input.mousePosition);
            if (hitObject && hitPoint != CameraManager.InvalidPosition)
            {
                MonoBehaviour worldObject = hitObject.GetComponent<MonoBehaviour>();
                CameraManager.Instance.SelectedObject = worldObject;
            }
        }
    }

    private void RightMouseClick()
    {
        if (CameraManager.Instance.PlayerHUD.MouseInBounds() && !Input.GetKey(KeyCode.LeftAlt) && CameraManager.Instance.SelectedObject)
        {
            CameraManager.Instance.SelectedObject = null;
        }
    }

    private void MouseHover()
    {
        if (CameraManager.Instance.PlayerHUD.MouseInBounds())
        {
            GameObject hoveredGameObject = CameraManager.FindHitObject(Input.mousePosition);
            Vector3 hitPoint = CameraManager.FindHitPoint(Input.mousePosition);
            if (hoveredGameObject && hitPoint != CameraManager.InvalidPosition)
            {
                CameraManager.Instance.PlayerHUD.SetCursorState(CursorState.Select);

                MonoBehaviour hoveredWorldObject = hoveredGameObject.GetComponent<MonoBehaviour>();

                CameraManager.Instance.HoveredObject = hoveredWorldObject;
            }
        }
    }
}