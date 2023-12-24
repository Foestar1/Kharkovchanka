using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField]
    private float rotationPerSec = 6;
    private Vector3 sunRot = Vector3.zero;

    private void Update()
    {
        sunRot.x = rotationPerSec * Time.deltaTime;
        this.gameObject.transform.Rotate(sunRot, Space.World);
    }
}
