using UnityEngine;

public class CameraPivot : MonoBehaviour
{
    ////////////////////////////////////////////////

    private float mouseRotationSpeed = 10f;                         // Horizontal turn speed.
    private float keysMovementSpeed = 1.0f;                           // Vertical turn speed.

    private float zoomMinFOV = 15f;
    private float zoomMaxFOV = 90f;
    private float zoomSensitivity = 10f;

    private float smooth = 10f;                                         // Speed of camera responsiveness.

    private float maxVerticalAngle = 0f;                               // Camera max clamp angle. 
    private float minVerticalAngle = 0f;                                 // Camera min clamp angle.

    private float h;
    private float v;
    private float x;
    private float y;

    public float angleH = 180;                                          // Float to store camera horizontal angle related to mouse movement.
    public float angleV = -26;                                          // Float to store camera vertical angle related to mouse movement.

    private float targetFOV;                                           // Target camera FIeld of View.
    private float targetMaxVerticalAngle;                              // Custom camera max vertical clamp angle. 

    ////////////////////////////////////////////////

    [HideInInspector]
    public Transform _cameraObject;

    public bool _unitOrbitCamEnable = false;

    private float _turnSpeed = 4.0f;

    private Vector3 _offset;


    ////////////////////////////////////////////////


    void Start()
    {
        _cameraObject = transform.Find("CameraObject");
    }


    void FixedUpdate()
    {
        if (Input.GetMouseButton(2))
        {
            KeyboardMovement();

            MouseMovement();
        }
    }


    void KeyboardMovement()
    {
        if (_unitOrbitCamEnable)
        {

        }
        else
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                keysMovementSpeed = 5.0f;
            }
            else
            {
                keysMovementSpeed = 1.0f;
            }

            // Basic Movement Player //
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");

            //Sets x and y basic movement
            transform.parent.Translate(new Vector3(keysMovementSpeed * h, 0, 0));
            transform.parent.Translate(new Vector3(0, 0, keysMovementSpeed * v));
        }
    }


    public void MouseMovement()
    {
        // mouse look around
        x = Input.GetAxis("Mouse X");
        y = Input.GetAxis("Mouse Y");

        // Get mouse movement to orbit the camera.
        angleH += Mathf.Clamp(x, -1, 1) * mouseRotationSpeed;
        angleV += Mathf.Clamp(y, -1, 1) * mouseRotationSpeed;


        if (_unitOrbitCamEnable)
        {
            // Set camera orientation..
            Quaternion aimRotation = Quaternion.Euler(angleV, angleH, 0);
            transform.rotation = aimRotation;
            _cameraObject.transform.LookAt(transform.position, Vector3.up);

            // Zoom
            var d = Input.GetAxis("Mouse ScrollWheel");
            if (d < 0f)
            {
                _offset = new Vector3(0, 1, 0);
            }
            else if (d > 0f)
            {
                _offset = new Vector3(0, -1, 0);
            }
            _cameraObject.transform.localPosition += _offset;
            _offset = Vector3.zero;
        }
        else
        {
            // Set camera orientation..
            Quaternion aimRotation = Quaternion.Euler(-angleV, angleH, 0);
            transform.parent.rotation = aimRotation;
        }
    }


    public void SetNewPivot(GameObject pivot)
    {
        // need a smooth transition when camera moves from unit to unit here, not just jump suddenly
        gameObject.transform.position = pivot.transform.position;
        gameObject.transform.SetParent(pivot.transform);
        _cameraObject.transform.localPosition = new Vector3(0, 10, 0);
        _cameraObject.transform.LookAt(transform.position, Vector3.up);
        _unitOrbitCamEnable = true;

    }
}
