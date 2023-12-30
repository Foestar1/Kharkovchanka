using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class BlinkingLight : MonoBehaviour
{
    public float blinkInterval = 1.0f; // Adjust this value for the blinking interval
    private Light myLight;
    private bool isLightOn = true;

    void Start()
    {
        myLight = GetComponent<Light>();
        if (myLight == null)
        {
            Debug.LogError("Light component not found!");
            enabled = false; // Disable the script if the light component is missing
        }

        // Start the blinking coroutine
        StartCoroutine(Blink());
    }

    IEnumerator Blink()
    {
        while (true)
        {
            yield return new WaitForSeconds(blinkInterval);
            
            // Toggle the light on and off
            isLightOn = !isLightOn;
            myLight.intensity = isLightOn ? 1.0f : 0.0f;
        }
    }
}
