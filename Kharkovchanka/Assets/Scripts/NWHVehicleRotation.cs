using UnityEngine;
using NWH.VehiclePhysics2;

public class NWHVehicleRotation : MonoBehaviour
{
    public float rotationSpeed = 50f;
    public float propulsionForce = 10f;

    private VehicleController vehicleController;
    private Rigidbody vehicleRigidbody;

    void Start()
    {
        vehicleController = GetComponent<VehicleController>();
        vehicleRigidbody = vehicleController != null ? vehicleController.GetComponent<Rigidbody>() : null;
    }

    void Update()
    {
        // Rotate forward when the W key is pressed
        if (Input.GetKey(KeyCode.W))
        {
            RotateForward();
        }

        // Rotate backward when the S key is pressed
        if (Input.GetKey(KeyCode.S))
        {
            RotateBackward();
        }
    }

    void RotateForward()
    {
        transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
        ApplyPropulsion(Vector3.right);
    }

    void RotateBackward()
    {
        transform.Rotate(Vector3.left * rotationSpeed * Time.deltaTime);
        ApplyPropulsion(Vector3.left);
    }

    void ApplyPropulsion(Vector3 direction)
    {
        if (vehicleRigidbody != null)
        {
            // Assuming VehicleController provides a way to access the Rigidbody
            // Adjust this based on the actual method provided by VehicleController
            // For example, if there's a GetRigidbody() method, use that instead.
            vehicleRigidbody.AddForce(direction * propulsionForce, ForceMode.Force);
        }
    }
}
