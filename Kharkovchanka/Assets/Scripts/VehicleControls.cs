using UnityEngine;
using UnityEngine.AI;

public class VehicleControls : MonoBehaviour
{
    [SerializeField]
    private Camera vehicleCamera;
    [SerializeField]
    private NavMeshAgent vehicleAgent;
    [SerializeField]
    private Animator doorAnimator;
    [SerializeField]
    private Animator voiceAnimator;
    [SerializeField]
    private AudioSource voiceSystem;
    [SerializeField]
    private AudioClip[] differentLines;
    public bool doorOpen { get; set; }

    private void Update()
    {
        #region open and close door
        if (doorOpen)
        {
            if (doorAnimator.GetBool("isOpen") == false)
            {
                voiceSystem.clip = differentLines[1];
                voiceSystem.Play();
                ToggleDoor();
            }
        }
        else
        {
            if (doorAnimator.GetBool("isOpen") == true)
            {
                voiceSystem.clip = differentLines[2];
                voiceSystem.Play();
                ToggleDoor();
            }
        }
        #endregion

        #region voice system
        if (voiceSystem.isPlaying)
        {
            if (!voiceAnimator.GetCurrentAnimatorStateInfo(0).IsName("Talking"))
            {
                voiceAnimator.Play("Talking");
            }
        }
        else
        {
            if (!voiceAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                voiceAnimator.Play("Idle");
            }
        }
        #endregion
    }

    #region custom functions
    public void movingVehicle()
    {
        voiceSystem.clip = differentLines[0];
        voiceSystem.Play();
    }

    public void ToggleDoor()
    {
        // Toggle the 'isOpen' parameter in the Animator
        bool isOpen = doorAnimator.GetBool("isOpen");
        doorAnimator.SetBool("isOpen", !isOpen);
    }
    #endregion
}
