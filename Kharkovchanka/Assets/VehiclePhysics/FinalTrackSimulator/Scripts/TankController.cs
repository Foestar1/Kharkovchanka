using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/********************************************************************************
** auth£º SwordMater 
** desc£º Create the tank controller class
** Ver.:  V1.0.0
*********************************************************************************/

/// <summary>
/// The class of tank controller
/// </summary>
public class TankController : MonoBehaviour
{
    /// <summary>
    /// The component of track simulator
    /// </summary>
    private TrackSimulator m_TrackSimulator;

    void Awake()
    {
        m_TrackSimulator = GetComponent<TrackSimulator>();
    }

    void FixedUpdate()
    {
        float motorInput = Input.GetAxisRaw("Vertical");

        float steerInput = Input.GetAxisRaw("Horizontal");

        //Chech the track simulator component is not null and is set correctly
        if (m_TrackSimulator != null && m_TrackSimulator.IsSetUpCorrectly == true)
        {
            //Call the update function of track simulator component
            m_TrackSimulator.UpdateMove(motorInput, steerInput);
        }

    }
}