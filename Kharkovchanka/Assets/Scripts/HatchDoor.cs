using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatchDoor : MonoBehaviour
{
    public Animator animator;

    void Start()
    {
        // Initialization code goes here if needed
    }

    // Update is called once per frame
    void Update()
    {
        // Check for 'E' key press
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleDoor();
        }
    }

    void ToggleDoor()
    {
        // Toggle the 'isOpen' parameter in the Animator
        bool isOpen = animator.GetBool("isOpen");
        animator.SetBool("isOpen", !isOpen);
    }
}
