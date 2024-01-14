using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    private Vector3 playerCameraLocation;
    private Quaternion playerCameraRotation;
    public Camera playerCamera;
    private Camera vehicleCamera;

    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 10f;


    public float lookSpeed = 2f;
    public float lookXLimit = 45f;

    public GameObject objectLookingAt;
    public GameObject playerVehicle;
    public GameObject interactUI;
    public GameObject exitDriveUI;

    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [SerializeField]
    private float cameraTransitionSpeed;
    private bool groundCamera;
    public bool canMove = true;


    CharacterController characterController;
    void Awake()
    {
        groundCamera = true;
        canMove = true;
        vehicleCamera = GameObject.Find("VehicleCamera").GetComponent<Camera>();
        interactUI = GameObject.Find("InteractButton");
        exitDriveUI = GameObject.Find("ExitVehicleButton");
        playerVehicle = GameObject.Find("VehicleFinal");

        playerCamera = Camera.main;
        playerCamera.transform.parent = this.gameObject.transform;
        playerCamera.transform.localPosition = new Vector3(0, 2, 0);
        playerCamera.transform.localEulerAngles = new Vector3(0, 2, 0);
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        interactUI.SetActive(false);
        exitDriveUI.SetActive(false);
    }

    void Update()
    {

        #region Handles Movment
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        #endregion

        #region Handles Jumping
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        #endregion

        #region Handles Rotation
        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
        #endregion

        #region Handles Raycast
        if (canMove)
        {
            RaycastHit HitInfo;
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out HitInfo, 100.0f))
            {
                objectLookingAt = HitInfo.transform.gameObject;
            }
            Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * 100.0f, Color.yellow);
        }
        #endregion

        #region Handles Interaction Inputs
        #region Handles The UI
        if (groundCamera)
        {
            exitDriveUI.SetActive(false);
            if (objectLookingAt.tag == "Interactable")
            {
                float distance = Vector3.Distance(objectLookingAt.transform.position, this.gameObject.transform.position);
                if (distance <= 3.5)
                {
                    if (interactUI.activeSelf == false)
                    {
                        interactUI.SetActive(true);
                    }
                }
                else
                {
                    if (interactUI.activeSelf == true)
                    {
                        interactUI.SetActive(false);
                    }
                }
            }
            else
            {
                if (interactUI.activeSelf == true)
                {
                    interactUI.SetActive(false);
                }
            }
        }
        else
        {
            interactUI.SetActive(false);
            exitDriveUI.SetActive(true);

            if (Input.GetButtonDown("Interact"))
            {
                if (groundCamera == false)
                {
                    playerCamera.transform.localPosition = new Vector3(0, 2, 0);
                    playerCamera.transform.localEulerAngles = new Vector3(0, 2, 0);
                    groundCamera = true;
                }
            }
        }
        #endregion

        #region Handles The Map
        if (objectLookingAt.name == "Map" && groundCamera)
        {
            float distance = Vector3.Distance(objectLookingAt.transform.position, this.gameObject.transform.position);

            if (distance <= 3.5f)
            {
                if (Input.GetButtonDown("Interact"))
                {
                    if (groundCamera == true)
                    {
                        groundCamera = false;
                    }
                }
            }
        }
        #endregion

        #region Handles The Vehicle Doors
        if (objectLookingAt.name == "VehicleDoorController" && groundCamera)
        {
            float distance = Vector3.Distance(objectLookingAt.transform.position, this.gameObject.transform.position);
            
            if (distance <= 3.5f)
            {
                if (Input.GetButtonDown("Interact"))
                {
                    if (playerVehicle.GetComponent<VehicleControls>().doorOpen == true)
                    {
                        playerVehicle.GetComponent<VehicleControls>().doorOpen = false;
                    }
                    else
                    {
                        playerVehicle.GetComponent<VehicleControls>().doorOpen = true;
                    }
                }
            }
        }
        #endregion
        #endregion

        #region Handles Camera Change And Above View Raycast
        if (groundCamera)
        {
            //make sure we can move
            if (!canMove)
            {
                canMove = true;
            }

            if (playerCamera.transform.localPosition != playerCameraLocation)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
        else
        {
            //make sure we can't move
            if (canMove)
            {
                canMove = false;
            }

            if (playerCamera.transform.position != vehicleCamera.transform.position)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                playerCamera.transform.position = vehicleCamera.transform.position;
                playerCamera.transform.rotation = vehicleCamera.transform.rotation;
            }

            if (Input.GetButtonDown("Fire1"))
            {
                RaycastHit mouseClickCamera;
                Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out mouseClickCamera, Mathf.Infinity))
                {
                    playerVehicle.GetComponent<NavMeshAgent>().SetDestination(mouseClickCamera.point);
                    playerVehicle.GetComponent<VehicleControls>().movingVehicle();
                }
            }
        }
        #endregion
    }
}