using System;
using UnityEngine;

/********************************************************************************
** auth： SwordMater 
** desc： Create the CameraController class
** Ver.:  V1.0.0
*********************************************************************************/

/// <summary>
/// The class of CameraController which control the camera to follow the target gameObject 
/// </summary>
public class CameraController : MonoBehaviour
{
    // The target gameObject to follow
    public GameObject followTarget;

    //The speed When root gameObject lerp to the position of target gameObject   .
    public float followSpeed = 10.0f;

    // The speed When root gameObject rotate around y axis or pivot gameObject rotate around x axis.
    public float rotateSpeed = 3.0f;

    // The minimum value of the x axis rotation of the pivot.
    public float rotationXMinValue = -30.0f;

    // The maximum value of the x axis rotation of the pivot.
    public float rotationXMaxValue = 70.0f;

    // The bool flag which mark the cursor should be hidden and locked.
    public bool lockCursorFlag = false;

    // The transform of the camera
    private Transform m_CameraTransform;

    // The gameObject at which the camera pivots around
    private Transform m_PivotTransform;

    void Awake()
    {
        m_CameraTransform = GetComponentInChildren<Camera>().transform;

        m_PivotTransform = m_CameraTransform.parent;

        // Lock or unlock the cursor.
        if (lockCursorFlag == true)
        {
            Cursor.lockState = CursorLockMode.Locked;

            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;

            Cursor.visible = true;
        }
    }

    void LateUpdate()
    {  
        float inputDeltaHorizontalAngle = Input.GetAxis("Mouse X") * rotateSpeed;

        // Make the root gameObject rotate around Y axis:
        transform.rotation = transform.rotation * Quaternion.Euler(0.0f, inputDeltaHorizontalAngle, 0.0f);


        Quaternion tempQuaternion = m_PivotTransform.localRotation;

        float inputDeltaVerticalAngle = -Input.GetAxis("Mouse Y") * rotateSpeed;

        // Make the pivot gameObject rotate around X axis:
        tempQuaternion = tempQuaternion * Quaternion.Euler(inputDeltaVerticalAngle, 0.0f, 0.0f);

        float pivotCurrentEulerXValue = tempQuaternion.eulerAngles.x;

        //if pivotCurrentEulerXValue variable is larger than 180.0f,convert it to negative number which means to rotate counterclockwise
        if (pivotCurrentEulerXValue > 180.0f)
        {
            pivotCurrentEulerXValue = pivotCurrentEulerXValue - 360.0f;
        }

        //Make sure the euler x value of m_PivotTransform is within this range
        pivotCurrentEulerXValue = Mathf.Clamp(pivotCurrentEulerXValue, rotationXMinValue, rotationXMaxValue);

       
        m_PivotTransform.localRotation = Quaternion.Euler(pivotCurrentEulerXValue, tempQuaternion.eulerAngles.y, tempQuaternion.eulerAngles.z); ;

        //Click down the left button of mouse to unlock the cursor and to be visible.
        if (lockCursorFlag && Input.GetMouseButtonDown(0))
        {
            //Unlock the cursor.
            Cursor.lockState = CursorLockMode.None;

            //Let cursor to be visible
            Cursor.visible = true;
        }

        //Let the root gameObject lerp to the position of followTarget's transform
        transform.position = Vector3.Lerp(transform.position, followTarget.transform.position, Time.deltaTime * followSpeed);
    }

    void OnDisable()
    {
        //Unlock the cursor.
        Cursor.lockState = CursorLockMode.None;

        //Let cursor to be visible
        Cursor.visible = true;
    }

}