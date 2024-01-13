using UnityEngine;

public class NewBehaviour : MonoBehaviour
{
    private Transform playerTransform;
    private Transform vehicleTransform;

    private bool isInsideVehicle = false;

    void Start()
    {
        playerTransform = transform;
        // Assign the vehicle GameObject's transform
        vehicleTransform = GameObject.Find("YourVehicleName").transform;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("VehicleTrigger"))
        {
            EnterVehicle();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("VehicleTrigger"))
        {
            ExitVehicle();
        }
    }

    void EnterVehicle()
    {
        // Make the player a child of the vehicle
        playerTransform.parent = vehicleTransform;
        isInsideVehicle = true;
    }

    void ExitVehicle()
    {
        // Unparent the player from the vehicle
        playerTransform.parent = null;
        isInsideVehicle = false;
    }
}
