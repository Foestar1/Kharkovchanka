using UnityEngine;

public class Test : MonoBehaviour
{
    public float gravity = 9.8f; // Adjust this value to control gravity strength

    void Update()
    {
        // Apply gravity force in the downward direction
        Vector3 gravityVector = new Vector3(0, -gravity, 0);
        GetComponent<Rigidbody>().AddForce(gravityVector, ForceMode.Acceleration);
    }
}
