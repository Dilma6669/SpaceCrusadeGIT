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

    private float orbitCamDist = 10;

    private float _turnSpeed = 4.0f;

    private Vector3 _offset;


    ////////////////////////////////////////////////


    void Start()
    {
        _cameraObject = GameObject.Find("CameraObject").transform;

        _offset = new Vector3(orbitCamDist, orbitCamDist, orbitCamDist);
    }


    void Update()
    {

        KeyboardMovement();

        MouseMovement();
    }


    void KeyboardMovement()
    {

        if (_unitOrbitCamEnable)
        {
            var d = Input.GetAxis("Mouse ScrollWheel");
            //if (d > 0f)
            //{
            //    _offset = new Vector3(_offset.x + 1, _offset.y, _offset.z);
            //}
            //else if (d < 0f)
            //{
            //    _offset = new Vector3(_offset.x - 1, _offset.y, _offset.z);
            //}

            int keyNum = 0;
            if (Input.GetKey(KeyCode.Q))
            {
                keyNum = 1;
            }
            if (Input.GetKey(KeyCode.E))
            {
                keyNum = -1;
            }
            // _offset = Quaternion.AngleAxis(keyNum * _turnSpeed, Vector3.up) * _offset;
            _cameraObject.transform.localPosition = _offset;
            _cameraObject.transform.LookAt(transform.position);
        }
        else
        {
            if (Input.GetMouseButton(2))
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
    }


    public void MouseMovement()
    {
        // Mouse rotation
        if (Input.GetMouseButton(2))
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
            }
            else
            {
                // Set camera orientation..
                Quaternion aimRotation = Quaternion.Euler(-angleV, angleH, 0);
                _cameraObject.rotation = aimRotation;

            }
        }
    }
}
