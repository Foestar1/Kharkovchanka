using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

/********************************************************************************
** auth£º SwordMater 
** desc£º Create the track simulator class
** Ver.:  V1.0.0
*********************************************************************************/

/// <summary>
/// The class of track simulator
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class TrackSimulator : MonoBehaviour
{
    /// <summary>
    /// The class of track's wheel which is on the higher position 
    /// </summary>
    [System.Serializable]
    public class UpperWheelData
    {
        //The transform of wheel model
        public Transform wheelTransform;
    }

    /// <summary>
    /// The class of track's wheel which is on the lower position 
    /// </summary>
    [System.Serializable]
    public class SuspendedWheelData
    {
        //The transform of wheel model
        public Transform wheelTransform;

        //The transform of wheel bone
        public Transform wheelBoneTransform;

        //The wheelCollider of wheel
        public WheelCollider wheelCollider;
    }

    /// <summary>
    /// The base class of track
    /// </summary>
    [System.Serializable]
    public class BaseTrackData
    {
        //Track's wheels which are on the higher position 
        public UpperWheelData[] upperWheelDataArray;

        //Track's wheels which are on the lower position 
        public SuspendedWheelData[] suspendedWheelDataArray;

        //Track's skinnedMeshRenderer
        public SkinnedMeshRenderer trackSkinnedMeshRenderer;
    }

    /// <summary>
    /// The class of left track
    /// </summary>
    [System.Serializable]
    public class LeftTrackData : BaseTrackData { }

    /// <summary>
    /// The class of right track
    /// </summary>
    [System.Serializable]
    public class RightTrackData : BaseTrackData { }

    // The instance of left track
    public LeftTrackData leftTrackData;

    // The instance of right track
    public RightTrackData rightTrackData;

    /// <summary>
    /// The class of wheel's feature curves which are based on the wheel's rotation speed
    /// </summary>
    [System.Serializable]
    public class WheelFeaturesAnimationCurves
    {
        //The animationCurve which recorded the relationship between wheel's rotation speed and wheel's motor torque
        public AnimationCurve singleWheelMotorAnimationCurve = AnimationCurve.Linear(0.0f, 10000.0f, 72.42f, 0.0f);

        //The animationCurve which recorded the relationship between wheel's rotation speed and wheel's brake torque
        public AnimationCurve singleWheelBrakeAnimationCurve = AnimationCurve.Linear(0.0f, 10000.0f, 72.42f, 12000.0f);

        //The animationCurve which recorded the relationship between wheel's rotation speed and the torque that applied to the vehicle's Y axis 
        public AnimationCurve singleWheelSteerTorqueYValueAnimationCurve = AnimationCurve.Linear(0.0f, 0.5f, 72.42f, 0.2f);

    }

    //The instance of wheel's feature curves which are based on the wheel's rotation speed
    public WheelFeaturesAnimationCurves wheelFeaturesAnimationCurves;

    /// <summary>
    /// The offset direction of wheel model and wheel bone
    /// </summary>
    public enum WheelAndBoneTransformOffsetDirection
    {
        X,
        Y,
        Z
    }

    /// <summary>
    /// The wheel model's rotation axis
    /// </summary>
    public enum WheelRotateAxis
    {
        X,
        Y,
        Z
    }

    /// <summary>
    ///The uv offset direction of track's texture 
    /// </summary>
    public enum TrackTextureOffsetType
    {
        X,
        Y
    }

    /// <summary>
    /// The class of track's tuning parameters 
    /// </summary>
    [System.Serializable]
    public class AdjustParameters
    {
        //The offset direction of wheel model and wheel bone
        public WheelAndBoneTransformOffsetDirection wheelAndBoneTransformOffsetDirection = WheelAndBoneTransformOffsetDirection.Y;
        //The switch that inverse the current offset direction of wheel model and wheel bone
        public bool inverseWheelAndBoneTransformOffsetDirection = false;
        //The offset value of wheel model and wheel bone
        public float singleWheelAndBoneTransfromVerticalOffset = 0.1f;

        //The wheel model's rotation axis
        public WheelRotateAxis wheelRotateAxis = WheelRotateAxis.X;
        //The switch that inverse the current rotation direction of the wheel
        public bool inverseWheelRotateDirection = false;

        //The uv offset direction of track's texture 
        public TrackTextureOffsetType trackTextureOffsetType = TrackTextureOffsetType.X;
        //The switch that inverse the current rotation direction of the texture's uv of the track
        public bool inverseTrackTextureOffsetDirection = false;
        //The multiplier of uv offset speed of track's texture 
        public float singleTrackTextureOffsetSpeedMultiplier = 1.111111f;

    }

    // The instance of track's tuning parameters 
    public AdjustParameters adjustParameters;

    // The center of mass of the vehicle which the track belongs to
    public Transform centerOfMass;

    // The max angular velocity of the vehicle which the track belongs to
    public float rigidbodyMaxAngularVelocity = 5.0f;

    // The switch which displayed debug information on the screen
    public bool showDebugInformation = false;

    // If tracks are set correctly 
    public bool IsSetUpCorrectly
    {
        set;
        get;
    }

    // The rigidbody of the vehicle which the track belongs to
    private Rigidbody m_Rigidbody;


#if UNITY_EDITOR

    /// <summary>
    /// Check if Tracks are set correctly
    /// </summary>
    /// <param name="trackData">The track data</param>
    /// <returns>if track is set correctly</returns>
    public bool CheckIfTrackDataSettingIsCorrect(BaseTrackData trackData)
    {
        string trackDataName = string.Empty;

        if (trackData is LeftTrackData)
        {
            trackDataName = "LeftTrackData";
        }
        else
        {
            trackDataName = "RightTrackData";
        }

        UpperWheelData[] upperWheelArray = trackData.upperWheelDataArray;

        SuspendedWheelData[] suspendedWheelDataArray = trackData.suspendedWheelDataArray;

        SkinnedMeshRenderer trackRender = trackData.trackSkinnedMeshRenderer;

        for (int i = 0; i < upperWheelArray.Length; i++)
        {
            UpperWheelData upperWheelData = upperWheelArray[i];

            Transform wheelTransform = upperWheelData.wheelTransform;

            if (wheelTransform == null)
            {
                string message = string.Format("You are not set {0}-> UpperWheelData0{1} -> Wheel Model !", trackDataName, i);

                EditorUtility.DisplayDialog("Track Simulator", message, "OK");

                return false;
            }
        }

        for (int i = 0; i < suspendedWheelDataArray.Length; i++)
        {
            SuspendedWheelData suspendedWheelData = suspendedWheelDataArray[i];

            Transform wheelTransform = suspendedWheelData.wheelTransform;

            Transform wheelBoneTransform = suspendedWheelData.wheelBoneTransform;

            WheelCollider wheelCollider = suspendedWheelData.wheelCollider;

            if (wheelTransform == null)
            {
                string message = string.Format("You are not set {0} -> SuspendedWheelData0{1} -> Wheel Model !", trackDataName, i);

                EditorUtility.DisplayDialog("Track Simulator", message, "OK");

                return false;
            }

            if (wheelBoneTransform == null)
            {
                string message = string.Format("You are not set {0} -> SuspendedWheelData0{1} -> Wheel Bone !", trackDataName, i);

                EditorUtility.DisplayDialog("Track Simulator", message, "OK");

                return false;
            }

            if (wheelCollider == null)
            {
                string message = string.Format("You are not set {0} -> suspendedWheelData0{1} -> Wheel Collider !", trackDataName, i);

                EditorUtility.DisplayDialog("Track Simulator", message, "OK");

                return false;
            }

        }

        if (trackRender == null)
        {
            string message = string.Format("You are not set {0} -> Track SkinnedMeshRenderer !", trackDataName);

            EditorUtility.DisplayDialog("Track Simulator", message, "OK");

            return false;
        }

        return true;

    }

    /// <summary>
    /// Check if the center of mass is set correctly
    /// </summary>
    /// <param name="massCenter">The transform of mass center</param>
    /// <returns>if mass center is set correctly</returns>
    public bool CheckIfMassCenterSettingIsCorrect(Transform massCenter)
    {
        if (massCenter == null)
        {
            EditorUtility.DisplayDialog("Track Simulator", "massCenter is null,please set it correctly", "OK");

            return false;
        }

        return true;
    }

    /// <summary>
    /// Check if the collider that attach to vehicle's mainbody is set correctly
    /// </summary>
    /// <returns>if have other type colliders besides wheel colliders attach to it and it's children </returns>
    public bool CheckIfBodyColliderIsSetUp()
    {
        Collider[] colliderArray = transform.GetComponentsInChildren<Collider>();

        for (int i = 0; i < colliderArray.Length; i++)
        {
            Collider collider = colliderArray[i];

            if (collider is WheelCollider)
            {
                continue;
            }
            else
            {
                return true;
            }
        }

        return false;
    }

#endif

    public void Awake()
    {

#if UNITY_EDITOR

        if (CheckIfTrackDataSettingIsCorrect(leftTrackData) == false)
        {
            IsSetUpCorrectly = false;

            EditorApplication.isPlaying = false;

            return;
        }

        if (CheckIfTrackDataSettingIsCorrect(rightTrackData) == false)
        {
            IsSetUpCorrectly = false;

            EditorApplication.isPlaying = false;

            return;
        }

        if (CheckIfMassCenterSettingIsCorrect(centerOfMass) == false)
        {
            IsSetUpCorrectly = false;

            EditorApplication.isPlaying = false;

            return;
        }

        if (CheckIfBodyColliderIsSetUp() == false)
        {
            EditorUtility.DisplayDialog("Track Simulator", "Please add a box or mesh collider to its main body", "OK");

            IsSetUpCorrectly = false;

            EditorApplication.isPlaying = false;

            return;
        }

        IsSetUpCorrectly = true;

#endif

        m_Rigidbody = GetComponent<Rigidbody>();

        m_Rigidbody.maxAngularVelocity = rigidbodyMaxAngularVelocity;

        m_Rigidbody.centerOfMass = centerOfMass.localPosition;

        IsSetUpCorrectly = true;

        UpdateMove();
    }

    public void OnGUI()
    {
        if (showDebugInformation == true && IsSetUpCorrectly == true)
        {
            GUILayout.BeginVertical();

            GUILayout.Label(string.Format("Rigidbody Speed = {0} km/ph", m_Rigidbody.velocity.magnitude * 3600.0f / 1000.0f));

            GUILayout.EndVertical();

        }

    }

    /// <summary>
    /// The update function of the track simulator
    /// </summary>
    /// <param name="motorInput">The value of motor input</param>
    /// <param name="steerInput">The value of steer input</param>
    public void UpdateMove(float motorInput = 0.0f, float steerInput = 0.0f)
    {
        float leftTrackSteerTorqueYValue = UpdateTrackController(motorInput, steerInput, leftTrackData);

        float rightTrackSteerTorqueYValue = UpdateTrackController(motorInput, steerInput, rightTrackData);

        float totalSteerTorqueYValue = leftTrackSteerTorqueYValue + rightTrackSteerTorqueYValue;


        if (Mathf.Abs(m_Rigidbody.angularVelocity.y) < 1.0f)
        {
            if (CheckIfMoveBackward(motorInput) == false)
            {
                m_Rigidbody.AddRelativeTorque(0.0f, totalSteerTorqueYValue, 0.0f, ForceMode.Acceleration);
            }
            else
            {
                m_Rigidbody.AddRelativeTorque(0.0f, -totalSteerTorqueYValue, 0.0f, ForceMode.Acceleration);
            }
        }

    }

    /// <summary>
    /// Update track and then return the torque taht will apply to the vehicle's mass center
    /// </summary>
    /// <param name="motorInput">The value of motor input</param>
    /// <param name="steerInput">The value of steer input</param>
    /// <param name="trackData">The track data</param>
    /// <returns>The torque taht will apply to the vehicle's mass center</returns>
    private float UpdateTrackController(float motorInput, float steerInput, BaseTrackData trackData)
    {
        float totalWheelSteerTorqueYValue = 0.0f;


        #region Prepare

        UpperWheelData[] upperWheelArray = trackData.upperWheelDataArray;

        SuspendedWheelData[] suspendedWheelDataArray = trackData.suspendedWheelDataArray;

        SkinnedMeshRenderer trackRender = trackData.trackSkinnedMeshRenderer;


        WheelAndBoneTransformOffsetDirection wheelAndBoneTransformOffsetDirection = adjustParameters.wheelAndBoneTransformOffsetDirection;

        bool inverseWheelAndBoneTransformOffsetDirection = adjustParameters.inverseWheelAndBoneTransformOffsetDirection;

        float singleWheelAndBoneTransfromVerticalOffset = adjustParameters.singleWheelAndBoneTransfromVerticalOffset;


        WheelRotateAxis wheelRotateAxis = adjustParameters.wheelRotateAxis;

        bool inverseWheelRotateDirection = adjustParameters.inverseWheelRotateDirection;


        TrackTextureOffsetType trackTextureOffsetType = adjustParameters.trackTextureOffsetType;

        bool inverseTrackTextureOffsetDirection = adjustParameters.inverseTrackTextureOffsetDirection;

        float singleTrackTextureOffsetSpeedMultiplier = adjustParameters.singleTrackTextureOffsetSpeedMultiplier;


        float inverseWheelAndBoneTransformOffsetDirectionFlag = 0.0f;

        if (inverseWheelAndBoneTransformOffsetDirection == true)
        {
            inverseWheelAndBoneTransformOffsetDirectionFlag = -1.0f;
        }
        else
        {
            inverseWheelAndBoneTransformOffsetDirectionFlag = 1.0f;
        }


        float inverseWheelRotateDirectionFlag = 0.0f;

        if (inverseWheelRotateDirection == true)
        {
            inverseWheelRotateDirectionFlag = -1.0f;
        }
        else
        {
            inverseWheelRotateDirectionFlag = 1.0f;
        }


        float inverseTrackTextureOffsetDirectionFlag = 0.0f;

        if (inverseTrackTextureOffsetDirection == true)
        {
            inverseTrackTextureOffsetDirectionFlag = -1.0f;
        }
        else
        {
            inverseTrackTextureOffsetDirectionFlag = 1.0f;
        }


        float wheelAverageSpeedRpm = CalculateWheelsAverageRPM(suspendedWheelDataArray);

        float wheelAverageSpeedDegreePerSecond = Time.fixedDeltaTime * wheelAverageSpeedRpm * 360.0f / 60.0f;

        #endregion

        #region Handle UpperWheelArray

        for (int i = 0; i < upperWheelArray.Length; i++)
        {
            UpperWheelData upperWheelData = upperWheelArray[i];

            Transform wheelTransform = upperWheelData.wheelTransform;

            if (wheelRotateAxis == WheelRotateAxis.X)
            {
                wheelTransform.rotation = wheelTransform.rotation * Quaternion.Euler(inverseWheelRotateDirectionFlag * wheelAverageSpeedDegreePerSecond, 0.0f, 0.0f);
            }
            else if (wheelRotateAxis == WheelRotateAxis.Y)
            {
                wheelTransform.rotation = wheelTransform.rotation * Quaternion.Euler(0.0f, inverseWheelRotateDirectionFlag * wheelAverageSpeedDegreePerSecond, 0.0f);
            }
            else  //(wheelRotateAxis == WheelRotateAxis.Z)
            {
                wheelTransform.rotation = wheelTransform.rotation * Quaternion.Euler(0.0f, 0.0f, inverseWheelRotateDirectionFlag * wheelAverageSpeedDegreePerSecond);
            }

        }

        #endregion

        #region Handle SuspendedWheelDataArray

        for (int i = 0; i < suspendedWheelDataArray.Length; i++)
        {
            SuspendedWheelData suspendedWheelData = suspendedWheelDataArray[i];


            WheelCollider wheelCollider = suspendedWheelData.wheelCollider;

            wheelCollider.wheelDampingRate = Mathf.Lerp(100.0f, 0.0f, Mathf.Abs(motorInput));


            Vector3 wheelTranformTargetPosition = Vector3.zero;

            Quaternion wheelTransfromTargetRotation = Quaternion.identity;

            wheelCollider.GetWorldPose(out wheelTranformTargetPosition, out wheelTransfromTargetRotation);


            Vector3 wheelColliderVerticalDirection = Vector3.zero;

            if (wheelAndBoneTransformOffsetDirection == WheelAndBoneTransformOffsetDirection.X)
            {
                wheelColliderVerticalDirection = wheelCollider.transform.right * inverseWheelAndBoneTransformOffsetDirectionFlag;
            }
            else if (wheelAndBoneTransformOffsetDirection == WheelAndBoneTransformOffsetDirection.Y)
            {
                wheelColliderVerticalDirection = wheelCollider.transform.up * inverseWheelAndBoneTransformOffsetDirectionFlag;
            }
            else   //wheelAndBoneTransformOffsetDirection == WheelAndBoneTransformOffsetDirection.Z
            {
                wheelColliderVerticalDirection = wheelCollider.transform.forward * inverseWheelAndBoneTransformOffsetDirectionFlag;
            }


            Transform wheelBoneTransform = suspendedWheelData.wheelBoneTransform;

            Vector3 wheelBoneTransformVerticalOffset = wheelColliderVerticalDirection * singleWheelAndBoneTransfromVerticalOffset;

            wheelBoneTransform.position = wheelTranformTargetPosition + wheelBoneTransformVerticalOffset;



            Transform wheelTransform = suspendedWheelData.wheelTransform;

            Vector3 wheelTranformVerticalOffset = wheelColliderVerticalDirection * singleWheelAndBoneTransfromVerticalOffset;

            wheelTransform.position = wheelTranformTargetPosition + wheelTranformVerticalOffset;

            if (wheelRotateAxis == WheelRotateAxis.X)
            {
                wheelTransform.rotation = wheelTransform.rotation * Quaternion.Euler(inverseWheelRotateDirectionFlag * wheelAverageSpeedDegreePerSecond, 0.0f, 0.0f);
            }
            else if (wheelRotateAxis == WheelRotateAxis.Y)
            {
                wheelTransform.rotation = wheelTransform.rotation * Quaternion.Euler(0.0f, inverseWheelRotateDirectionFlag * wheelAverageSpeedDegreePerSecond, 0.0f);
            }
            else   //wheelRotateAxis == WheelRotateAxis.Z
            {
                wheelTransform.rotation = wheelTransform.rotation * Quaternion.Euler(0.0f, 0.0f, inverseWheelRotateDirectionFlag * wheelAverageSpeedDegreePerSecond);
            }


            float wheelSpeedKMPH = Mathf.Abs(ConvertSingleWheelRPMToKMPH(wheelCollider.radius, wheelCollider.rpm));

            CalculateSingleWheelMotorTorque(motorInput, steerInput, trackData, wheelCollider, wheelSpeedKMPH);

            CalculateSingleWheelBrakeTorque(motorInput, steerInput, wheelCollider, wheelSpeedKMPH);

            float singleWheelSteerTorqueYValue = CalculateSingleWheelSteerTorqueYValue(motorInput, steerInput, wheelCollider, wheelSpeedKMPH);

            totalWheelSteerTorqueYValue = totalWheelSteerTorqueYValue + singleWheelSteerTorqueYValue;

        }

        #endregion

        #region Handle TrackSkinnedMeshRenderer

        Vector2 trackMainTextureOffset = trackRender.material.GetTextureOffset("_MainTex");

        Vector2 trackBumpMapOffset = trackRender.material.GetTextureOffset("_BumpMap");

        if (trackTextureOffsetType == TrackTextureOffsetType.X)
        {
            trackMainTextureOffset.x = Mathf.Repeat(trackMainTextureOffset.x + (inverseTrackTextureOffsetDirectionFlag * wheelAverageSpeedDegreePerSecond * singleTrackTextureOffsetSpeedMultiplier) / 360.0f, 1.0f);

            trackBumpMapOffset.x = Mathf.Repeat(trackBumpMapOffset.x + (inverseTrackTextureOffsetDirectionFlag * wheelAverageSpeedDegreePerSecond * singleTrackTextureOffsetSpeedMultiplier) / 360.0f, 1.0f);
        }
        else
        {
            trackMainTextureOffset.y = Mathf.Repeat(trackMainTextureOffset.y + (inverseTrackTextureOffsetDirectionFlag * wheelAverageSpeedDegreePerSecond * singleTrackTextureOffsetSpeedMultiplier) / 360.0f, 1.0f);

            trackBumpMapOffset.y = Mathf.Repeat(trackBumpMapOffset.y + (inverseTrackTextureOffsetDirectionFlag * wheelAverageSpeedDegreePerSecond * singleTrackTextureOffsetSpeedMultiplier) / 360.0f, 1.0f);
        }

        trackRender.material.SetTextureOffset("_MainTex", trackMainTextureOffset);

        trackRender.material.SetTextureOffset("_BumpMap", trackBumpMapOffset);

        #endregion


        return totalWheelSteerTorqueYValue;
    }

    /// <summary>
    /// Calculate the wheel's motor torque and then apply to wheel's wheel collider
    /// </summary>
    /// <param name="motorInput">The value of motor input</param>
    /// <param name="steerInput">The value of steer input</param>
    /// <param name="trackData">The track data</param>
    /// <param name="wheelCollider">The wheelCollider</param>
    /// <param name="wheelSpeedKMPH">The wheel's rotation speed in the form of kilometers per hour</param>
    private void CalculateSingleWheelMotorTorque(float motorInput, float steerInput, BaseTrackData trackData, WheelCollider wheelCollider, float wheelSpeedKMPH)
    {
        AnimationCurve singleWheelMotorAnimationCurve = wheelFeaturesAnimationCurves.singleWheelMotorAnimationCurve;

        float direction = 0.0f;

        if (trackData is LeftTrackData)
        {
            direction = 1.0f;
        }
        else
        {
            direction = -1.0f;
        }

        if (motorInput == 0.0f && steerInput == 0.0f)
        {
            wheelCollider.motorTorque = 0.0f;

        }
        else if (motorInput == 0.0f && steerInput != 0.0f)
        {
            wheelCollider.motorTorque = steerInput * direction * singleWheelMotorAnimationCurve.Evaluate(wheelSpeedKMPH); ;
        }
        else if (motorInput != 0.0f && steerInput == 0.0f)
        {
            wheelCollider.motorTorque = motorInput * singleWheelMotorAnimationCurve.Evaluate(wheelSpeedKMPH);
        }
        else //if (motorInput != 0.0f && steerInput != 0.0f)
        {
            wheelCollider.motorTorque = motorInput * singleWheelMotorAnimationCurve.Evaluate(wheelSpeedKMPH);
        }
    }

    /// <summary>
    ///  Calculate the wheel's brake torque and then apply to wheel's wheel collider
    /// </summary>
    /// <param name="motorInput">The value of motor input</param>
    /// <param name="steerInput">The value of steer input</param>
    /// <param name="wheelCollider">The wheelCollider</param>
    /// <param name="wheelSpeedKMPH">The wheel's rotation speed in the form of kilometers per hour</param>
    private void CalculateSingleWheelBrakeTorque(float motorInput, float steerInput, WheelCollider wheelCollider, float wheelSpeedKMPH)
    {
        AnimationCurve singleWheelBrakeAnimationCurve = wheelFeaturesAnimationCurves.singleWheelBrakeAnimationCurve;

        if (motorInput == 0.0f && steerInput == 0.0f)
        {
            wheelCollider.brakeTorque = singleWheelBrakeAnimationCurve.Evaluate(wheelSpeedKMPH);
        }
        else if (motorInput == 0.0f && steerInput != 0.0f)
        {
            wheelCollider.brakeTorque = 0.0f;
        }
        else if (motorInput != 0.0f && steerInput == 0.0f)
        {
            if (wheelCollider.rpm > 0.0f && motorInput < 0.0f)
            {
                wheelCollider.brakeTorque = singleWheelBrakeAnimationCurve.Evaluate(wheelSpeedKMPH);
            }
            else if (wheelCollider.rpm < 0.0f && motorInput > 0.0f)
            {
                wheelCollider.brakeTorque = singleWheelBrakeAnimationCurve.Evaluate(wheelSpeedKMPH);
            }
            else
            {
                wheelCollider.brakeTorque = 0.0f;
            }

        }
        else //if (motorInput != 0.0f && steerInput != 0.0f)
        {
            if (wheelCollider.rpm > 0.0f && motorInput < 0.0f)
            {
                wheelCollider.brakeTorque = singleWheelBrakeAnimationCurve.Evaluate(wheelSpeedKMPH);
            }
            else if (wheelCollider.rpm < 0.0f && motorInput > 0.0f)
            {
                wheelCollider.brakeTorque = singleWheelBrakeAnimationCurve.Evaluate(wheelSpeedKMPH);
            }
            else
            {
                wheelCollider.brakeTorque = 0.0f;
            }
        }
    }

    /// <summary>
    /// Calculate the rotation torque that will apply to the vehicle
    /// </summary>
    /// <param name="motorInput">The value of motor input</param>
    /// <param name="steerInput">The value of steer input</param>
    /// <param name="wheelCollider">The wheelCollider</param>
    /// <param name="wheelKMPHSpeed">The wheel's rotation speed in the form of kilometers per hour</param>
    /// <returns>The rotation torque that will apply to the vehicle</returns>
    private float CalculateSingleWheelSteerTorqueYValue(float motorInput, float steerInput, WheelCollider wheelCollider, float wheelKMPHSpeed)
    {
        AnimationCurve singleWheelSteerTorqueYValueAnimationCurve = wheelFeaturesAnimationCurves.singleWheelSteerTorqueYValueAnimationCurve;

        float singleWheelSteerTorqueYValue = 0.0f;

        if (motorInput == 0.0f && steerInput == 0.0f)
        {
            singleWheelSteerTorqueYValue = 0.0f;
        }
        else if (motorInput == 0.0f && steerInput != 0.0f)
        {
            if (wheelCollider.isGrounded == true)
            {
                singleWheelSteerTorqueYValue = (steerInput * singleWheelSteerTorqueYValueAnimationCurve.Evaluate(wheelKMPHSpeed));
            }
            else
            {
                singleWheelSteerTorqueYValue = 0.0f;
            }

        }
        else if (motorInput != 0.0f && steerInput == 0.0f)
        {
            singleWheelSteerTorqueYValue = 0.0f;
        }
        else //if (motorInput != 0.0f && steerInput != 0.0f)
        {
            if (wheelCollider.isGrounded == true)
            {
                if (wheelCollider.rpm > 0.0f && motorInput < 0.0f)
                {
                    singleWheelSteerTorqueYValue = 0.0f;
                }
                else if (wheelCollider.rpm < 0.0f && motorInput > 0.0f)
                {
                    singleWheelSteerTorqueYValue = 0.0f;
                }
                else
                {
                    singleWheelSteerTorqueYValue = (steerInput * singleWheelSteerTorqueYValueAnimationCurve.Evaluate(wheelKMPHSpeed));
                }
            }
            else
            {
                singleWheelSteerTorqueYValue = 0.0f;
            }
        }

        return singleWheelSteerTorqueYValue;
    }

    /// <summary>
    /// Check if the vehicle is moving backward
    /// </summary>
    /// <param name="motorInput">The value of the motor input</param>
    /// <returns>if the vehicle is moving backward</returns>
    private bool CheckIfMoveBackward(float motorInput)
    {
        bool isMoveBackward = false;

        if (motorInput < 0.0f && transform.InverseTransformDirection(m_Rigidbody.velocity).z < 1.0f)
        {
            isMoveBackward = true;
        }
        else
        {
            isMoveBackward = false;
        }

        return isMoveBackward;
    }

    /// <summary>
    /// Calculate the average speed of track's wheels which are on the lower position 
    /// </summary>
    /// <param name="suspendedWheelDataArray">Track's wheels which are on the lower position</param>
    /// <returns>The average speed of track's wheels which are on the lower position</returns>
    private float CalculateWheelsAverageRPM(SuspendedWheelData[] suspendedWheelDataArray)
    {
        float AverageRpmOfWheels = 0.0f;

        List<int> groundedWheelIndexList = new List<int>();

        for (int i = 0; i < suspendedWheelDataArray.Length; i++)
        {
            if (suspendedWheelDataArray[i].wheelCollider.isGrounded == true)
            {
                groundedWheelIndexList.Add(i);
            }
        }

        if (groundedWheelIndexList.Count == 0)
        {
            float totalRpmOfWheelArray = 0.0f;

            for (int i = 0; i < suspendedWheelDataArray.Length; i++)
            {
                SuspendedWheelData suspendedWheelData = suspendedWheelDataArray[i];

                totalRpmOfWheelArray = totalRpmOfWheelArray + suspendedWheelData.wheelCollider.rpm;
            }

            AverageRpmOfWheels = totalRpmOfWheelArray / suspendedWheelDataArray.Length;
        }
        else
        {
            float totalRpmOfGroundedWheelList = 0.0f;

            for (int i = 0; i < groundedWheelIndexList.Count; i++)
            {
                int groundedWheelIndex = groundedWheelIndexList[i];

                totalRpmOfGroundedWheelList = totalRpmOfGroundedWheelList + suspendedWheelDataArray[groundedWheelIndex].wheelCollider.rpm;
            }

            AverageRpmOfWheels = totalRpmOfGroundedWheelList / groundedWheelIndexList.Count;
        }

        return AverageRpmOfWheels;
    }

    /// <summary>
    /// Convert the wheel's rotation speed in the form of rotatation per minute to kilometers per hour
    /// </summary>
    /// <param name="wheelRadius">The radius of wheel</param>
    /// <param name="revolutionsPerMinute">The rotation speed in the form of rotatation per minute</param>
    /// <returns>The rotation speed in the form of kilometers per hour</returns>
    private float ConvertSingleWheelRPMToKMPH(float wheelRadius, float revolutionsPerMinute)
    {
        float wheelPerimeter = 2.0f * Mathf.PI * wheelRadius;

        float meterPerMinute = revolutionsPerMinute * wheelPerimeter;

        float meterPerHour = meterPerMinute * 60.0f;

        float kilometerPerHour = meterPerHour / 1000.0f;

        return kilometerPerHour;

    }

}