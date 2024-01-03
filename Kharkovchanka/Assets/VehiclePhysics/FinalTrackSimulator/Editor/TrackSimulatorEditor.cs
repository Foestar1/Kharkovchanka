using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

/********************************************************************************
** auth： SwordMater 
** desc： Create the track simulator custom editor class
** Ver.:  V1.0.0
*********************************************************************************/

/// <summary>
/// The class of the custom inspector editor of track simulator
/// </summary>
[CustomEditor(typeof(TrackSimulator)), CanEditMultipleObjects]
public class TrackSimulatorEditor : Editor
{
    //The switch which controlled the left track parameters to be fold out
    private static bool m_LeftTrackDataFoldOutFlag = false;

    //The switch which controlled the right track parameters to be fold out
    private static bool m_RightTrackDataFoldOutFlag = false;

    //The switch which controlled the wheel's feature curves to be fold out
    private static bool m_WheelFeaturesAnimationCurvesFoldOutFlag = false;

    //The switch which controlled the wheel's tuning parameters to be fold out
    private static bool m_AdjustParamatersFoldOutFlag = false;

    //The switch which controlled the vehicle's mass center parameters to be fold out 
    private static bool m_MassCenterFoldOutFlag = false;

    //The custom color of blue 
    private static Color m_CustomBlueColor = new Color(0.5f, 0.75f, 1.0f, 1.0f);

    // The custom color of red 
    private static Color m_CustomRedColor = new Color(1.0f, 0.5f, 0.5f, 1.0f);

    // The custom color of green 
    private static Color m_CustomGreenColor = new Color(0.5f, 1.0f, 0.7f, 1.0f);

    /// <summary>
    /// The parent gameobject's name of the wheelColliders which is located on the left side of the vehicle
    /// </summary>
    private const string m_LeftTrackWheelCollidersParentGameObjectName = "LeftWheelColliders";

    /// <summary>
    /// The parent gameobject's name of the wheelColliders which is located on the right side of the vehicle
    /// </summary>
    private const string m_RightTrackWheelCollidersParentGameObjectName = "RightWheelColliders";

    /// <summary>
    /// The name of the mass center gameObject
    /// </summary>
    private const string m_MassCenterName = "Mass Center";

    /// <summary>
    /// Add track simulator component to the gameObject which be selected
    /// </summary>
    [MenuItem("Component/TrackSimulator/Add TrackSimulator")]
    public static void AddTrackSimulator()
    {
        GameObject activeGameObject = Selection.activeGameObject;

        if (activeGameObject != null)
        {
            if (activeGameObject.GetComponent<TrackSimulator>() == null)
            {
                activeGameObject.AddComponent<TrackSimulator>();

                Rigidbody activeGameObjectRigidbody = activeGameObject.GetComponent<Rigidbody>();

                activeGameObjectRigidbody.mass = 55000.0f;

                activeGameObjectRigidbody.angularDrag = 0.5f;

                activeGameObjectRigidbody.interpolation = RigidbodyInterpolation.Interpolate;

            }
            else
            {
                EditorUtility.DisplayDialog("Track Simulator ", "Your Gameobject already has TrackSimulator", "OK");
            }

        }

    }

    /// <summary>
    /// Remove track simulator component from the gameObject which be selected
    /// </summary>
    [MenuItem("Component/TrackSimulator/Remove TrackSimulator")]
    public static void RemoveTrackSimulator()
    {
        GameObject activeGameObject = Selection.activeGameObject;

        if (activeGameObject == null)
        {
            return;
        }

        TrackSimulator trackSimulator = activeGameObject.GetComponent<TrackSimulator>();

        if (trackSimulator != null)
        {
            Transform leftWheelCollidersParentGameObjectTransform = activeGameObject.transform.Find(m_LeftTrackWheelCollidersParentGameObjectName);

            if (leftWheelCollidersParentGameObjectTransform != null)
            {
                GameObject leftWheelCollidersParentGameObject = leftWheelCollidersParentGameObjectTransform.gameObject;

                DestroyImmediate(leftWheelCollidersParentGameObject);
            }

            Transform rightWheelCollidersParentGameObjectTransform = activeGameObject.transform.Find(m_RightTrackWheelCollidersParentGameObjectName);

            if (rightWheelCollidersParentGameObjectTransform != null)
            {
                GameObject rightWheelCollidersParentGameObject = rightWheelCollidersParentGameObjectTransform.gameObject;

                DestroyImmediate(rightWheelCollidersParentGameObject);
            }

            Transform massCenterTransform = activeGameObject.transform.Find(m_MassCenterName);

            if (massCenterTransform != null)
            {
                DestroyImmediate(massCenterTransform.gameObject);
            }

            Rigidbody rigidbody = trackSimulator.GetComponent<Rigidbody>();

            DestroyImmediate(trackSimulator);

            DestroyImmediate(rigidbody);

        }
        else
        {
            EditorUtility.DisplayDialog("Track Simulator ", "The TrackSimulator has already been removed from Your Gameobject", "OK");
        }

    }

    /// <summary>
    /// Override the OnInspectorGUI function to show the custom GUILayout of the track simulator component inspector
    /// </summary>
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        serializedObject.Update();

        TrackSimulator trackSimulator = target as TrackSimulator;


        #region Show Left Track

        m_LeftTrackDataFoldOutFlag = EditorGUILayout.Foldout(m_LeftTrackDataFoldOutFlag, new GUIContent("Left Track Data", "Left Track Data"));

        if (m_LeftTrackDataFoldOutFlag == true)
        {
            OnTrackData(trackSimulator.gameObject, "leftTrackData", m_LeftTrackWheelCollidersParentGameObjectName, trackSimulator.leftTrackData.suspendedWheelDataArray);
        }

        #endregion

        #region Show Right Track

        m_RightTrackDataFoldOutFlag = EditorGUILayout.Foldout(m_RightTrackDataFoldOutFlag, new GUIContent("Right Track Data", "Right Track Data"));

        if (m_RightTrackDataFoldOutFlag == true)
        {
            OnTrackData(trackSimulator.gameObject, "rightTrackData", m_RightTrackWheelCollidersParentGameObjectName, trackSimulator.rightTrackData.suspendedWheelDataArray);
        }

        #endregion

        #region Show WheelFeaturesAnimationCurves

        m_WheelFeaturesAnimationCurvesFoldOutFlag = EditorGUILayout.Foldout(m_WheelFeaturesAnimationCurvesFoldOutFlag, new GUIContent("WheelFeaturesAnimationCurves", "WheelFeaturesAnimationCurves"));

        if (m_WheelFeaturesAnimationCurvesFoldOutFlag)
        {
            SerializedProperty maxParamatersSerializedProperty = serializedObject.FindProperty("wheelFeaturesAnimationCurves");


            EditorGUILayout.BeginVertical(GUI.skin.box);

            SerializedProperty wheelMotorAnimationCurveSerializedProperty = maxParamatersSerializedProperty.FindPropertyRelative("singleWheelMotorAnimationCurve");

            EditorGUILayout.PropertyField(wheelMotorAnimationCurveSerializedProperty, new GUIContent("SingleWheelMotorAnimationCurve", "SingleWheelMotorAnimationCurve"), true);

            EditorGUILayout.EndVertical();


            EditorGUILayout.BeginVertical(GUI.skin.box);

            SerializedProperty wheelBrakeAnimationCurveSerializedProperty = maxParamatersSerializedProperty.FindPropertyRelative("singleWheelBrakeAnimationCurve");

            EditorGUILayout.PropertyField(wheelBrakeAnimationCurveSerializedProperty, new GUIContent("SingleWheelBrakeAnimationCurve", "SingleWheelBrakeAnimationCurve"), true);

            EditorGUILayout.EndVertical();


            EditorGUILayout.BeginVertical(GUI.skin.box);

            SerializedProperty wheelSteerTorqueYValueAnimationCurveSerializedProperty = maxParamatersSerializedProperty.FindPropertyRelative("singleWheelSteerTorqueYValueAnimationCurve");

            EditorGUILayout.PropertyField(wheelSteerTorqueYValueAnimationCurveSerializedProperty, new GUIContent("SingleWheelSteerTorqueYValueAnimationCurve", "SingleWheelSteerTorqueYValueAnimationCurve"), true);

            EditorGUILayout.EndVertical();


            GUI.color = m_CustomRedColor;

            if (GUILayout.Button("Reset All WheelFeaturesAnimationCurves"))
            {
                wheelMotorAnimationCurveSerializedProperty.animationCurveValue = AnimationCurve.Linear(0.0f, 10000.0f, 72.42f, 0.0f);

                wheelBrakeAnimationCurveSerializedProperty.animationCurveValue = AnimationCurve.Linear(0.0f, 10000.0f, 72.42f, 12000f);

                wheelSteerTorqueYValueAnimationCurveSerializedProperty.animationCurveValue = AnimationCurve.Linear(0.0f, 0.5f, 72.42f, 0.2f);

            }

            GUI.color = GUI.backgroundColor;
        }

        #endregion

        #region Show AdjustParamaters

        m_AdjustParamatersFoldOutFlag = EditorGUILayout.Foldout(m_AdjustParamatersFoldOutFlag, new GUIContent("Adjust Parameters", "Adjust Parameters"));

        if (m_AdjustParamatersFoldOutFlag == true)
        {

            SerializedProperty adjustParametersSerializedProperty = serializedObject.FindProperty("adjustParameters");


            EditorGUILayout.BeginVertical(GUI.skin.box);


            EditorGUILayout.BeginHorizontal();

            GUI.color = m_CustomBlueColor;

            EditorGUILayout.LabelField("Wheel And Bone Transform Offset Configuration:", EditorStyles.boldLabel);

            GUI.color = GUI.backgroundColor;

            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            EditorGUILayout.PropertyField(adjustParametersSerializedProperty.FindPropertyRelative("wheelAndBoneTransformOffsetDirection"), new GUIContent("OffsetDirection", "Wheel And Bone Transform Offset Direction"), true);

            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            EditorGUILayout.PropertyField(adjustParametersSerializedProperty.FindPropertyRelative("inverseWheelAndBoneTransformOffsetDirection"), new GUIContent("InverseOffsetDirection", "Inverse Wheel And Bone Transform Offset Direction"), true);

            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            EditorGUILayout.PropertyField(adjustParametersSerializedProperty.FindPropertyRelative("singleWheelAndBoneTransfromVerticalOffset"), new GUIContent("OffsetValue", "Wheel And Bone Transform Offset Value"), true);

            EditorGUILayout.EndHorizontal();


            EditorGUILayout.EndVertical();



            EditorGUILayout.Space();



            EditorGUILayout.BeginVertical(GUI.skin.box);


            EditorGUILayout.BeginHorizontal();

            GUI.color = m_CustomBlueColor;

            EditorGUILayout.LabelField("Wheel Rotate Configuration:", EditorStyles.boldLabel);

            GUI.color = GUI.backgroundColor;

            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            EditorGUILayout.PropertyField(adjustParametersSerializedProperty.FindPropertyRelative("wheelRotateAxis"), new GUIContent("Rotate Axis", "Wheel Rotate Axis"), true);

            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            EditorGUILayout.PropertyField(adjustParametersSerializedProperty.FindPropertyRelative("inverseWheelRotateDirection"), new GUIContent("Inverse Rotate Direction", "Inverse Wheel Rotate Direction"), true);

            EditorGUILayout.EndHorizontal();


            EditorGUILayout.EndVertical();



            EditorGUILayout.Space();



            EditorGUILayout.BeginVertical(GUI.skin.box);


            EditorGUILayout.BeginHorizontal();

            GUI.color = m_CustomBlueColor;

            EditorGUILayout.LabelField("Track Texture Offset Configuration:", EditorStyles.boldLabel);

            GUI.color = GUI.backgroundColor;

            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            EditorGUILayout.PropertyField(adjustParametersSerializedProperty.FindPropertyRelative("trackTextureOffsetType"), new GUIContent("OffsetDirection", "Track Texture Offset Direction"), true);

            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            EditorGUILayout.PropertyField(adjustParametersSerializedProperty.FindPropertyRelative("inverseTrackTextureOffsetDirection"), new GUIContent("InverseOffsetDirection", "Inverse Track Texture Offset Direction"), true);

            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            EditorGUILayout.PropertyField(adjustParametersSerializedProperty.FindPropertyRelative("singleTrackTextureOffsetSpeedMultiplier"), new GUIContent("OffsetSpeedMultiplier", "Single Track Texture Offset Speed Multiplier"), true);

            EditorGUILayout.EndHorizontal();


            EditorGUILayout.EndVertical();

        }

        #endregion

        #region Show MassCenter

        m_MassCenterFoldOutFlag = EditorGUILayout.Foldout(m_MassCenterFoldOutFlag, new GUIContent("Mass Center Configuration", "Mass Center Configuration"));

        if (m_MassCenterFoldOutFlag == true)
        {
            SerializedProperty centerOfMassSerializedProperty = serializedObject.FindProperty("centerOfMass");


            float defaultLabelWidth = EditorGUIUtility.labelWidth;

            EditorGUIUtility.labelWidth = 100.0f;

            EditorGUILayout.BeginVertical(GUI.skin.box);


            if (centerOfMassSerializedProperty.objectReferenceValue == null)
            {
                EditorGUILayout.BeginHorizontal(GUI.skin.box);

                EditorGUILayout.PropertyField(centerOfMassSerializedProperty, new GUIContent("Mass Center", "Mass Center"), true);

                EditorGUILayout.EndHorizontal();


                EditorGUILayout.BeginHorizontal();

                GUI.color = m_CustomGreenColor;

                if (GUILayout.Button("Add Mass Center"))
                {
                    AddMassCenter(trackSimulator, m_MassCenterName);
                }

                GUI.color = GUI.backgroundColor;

                EditorGUILayout.EndHorizontal();

            }
            else
            {
                EditorGUILayout.BeginHorizontal(GUI.skin.box);

                EditorGUILayout.PropertyField(centerOfMassSerializedProperty, new GUIContent("Mass Center", "Mass Center"), true);

                EditorGUILayout.EndHorizontal();


                EditorGUILayout.BeginHorizontal(GUI.skin.box);

                //Adjust the relative position of the mass center of vehicle
                Vector3 massCenterRelativePosition = trackSimulator.centerOfMass.localPosition;

                massCenterRelativePosition = EditorGUILayout.Vector3Field("Offset", massCenterRelativePosition);

                trackSimulator.centerOfMass.localPosition = massCenterRelativePosition;

                EditorGUILayout.EndHorizontal();


                EditorGUILayout.BeginHorizontal();

                GUI.color = m_CustomRedColor;

                if (GUILayout.Button("Remove Mass Center"))
                {
                    GameObject massCenterGameObject = trackSimulator.centerOfMass.gameObject;

                    centerOfMassSerializedProperty.DeleteCommand();

                    DestroyImmediate(massCenterGameObject);
                }

                GUI.color = GUI.backgroundColor;

                EditorGUILayout.EndHorizontal();
            }


            EditorGUILayout.EndVertical();

            EditorGUIUtility.labelWidth = defaultLabelWidth;
        }

        #endregion

        #region Show Other Paramaters

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("rigidbodyMaxAngularVelocity"), new GUIContent("Rigidbody MaxAngular Velocity", "Rigidbody MaxAngular Velocity"), true);

        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("showDebugInformation"), new GUIContent("Show Debug Information", "Show Debug Information"), true);

        EditorGUILayout.EndHorizontal();

        #endregion


        serializedObject.ApplyModifiedProperties();

    }

    /// <summary>
    /// To display the trackData parameters
    /// </summary>
    /// <param name="vehicleRootGameObject">The root gameObect of the vehicle</param>
    /// <param name="trackDataPropertyName">The member name of trackData which in trackSimlulator class,named either "leftTrackData" or "rightTrackData"</param>
    /// <param name="wheelCollidersParentGameObjectName">The parent gameobject's name of the wheelColliders<</param>
    /// <param name="suspendedWheelDataArray">The array of suspendedWheelData</param>
    private void OnTrackData(GameObject vehicleRootGameObject, string trackDataPropertyName, string wheelCollidersParentGameObjectName, TrackSimulator.SuspendedWheelData[] suspendedWheelDataArray)
    {
        SerializedProperty trackDataSerializedProperty = serializedObject.FindProperty(trackDataPropertyName);

        #region show upperWheelDataArray

        SerializedProperty upperWheelDataArraySerializedProperty = trackDataSerializedProperty.FindPropertyRelative("upperWheelDataArray");


        EditorGUILayout.BeginVertical(GUI.skin.box);


        EditorGUILayout.BeginHorizontal();

        GUIStyle upperWheelDataArrayLabelFieldGUIStyle = new GUIStyle();

        upperWheelDataArrayLabelFieldGUIStyle.alignment = TextAnchor.MiddleCenter;

        upperWheelDataArrayLabelFieldGUIStyle.normal.textColor = m_CustomBlueColor;

        EditorGUILayout.LabelField(new GUIContent("UpperWheelDataArray:"), upperWheelDataArrayLabelFieldGUIStyle);

        EditorGUILayout.EndHorizontal();


        for (int i = 0; i < upperWheelDataArraySerializedProperty.arraySize; i++)
        {
            SerializedProperty upperWheelDataSerializedProperty = upperWheelDataArraySerializedProperty.GetArrayElementAtIndex(i);

            SerializedProperty wheelTransformSerializedProperty = upperWheelDataSerializedProperty.FindPropertyRelative("wheelTransform");


            EditorGUILayout.BeginVertical(GUI.skin.box);


            EditorGUILayout.BeginHorizontal();

            GUI.color = m_CustomBlueColor;

            EditorGUILayout.LabelField(string.Format("UpperWheelData {0}:", i.ToString()), EditorStyles.boldLabel);

            GUI.color = GUI.backgroundColor;

            GUILayout.FlexibleSpace();

            GUI.color = m_CustomRedColor;

            if (GUILayout.Button("X"))
            {
                upperWheelDataArraySerializedProperty.DeleteArrayElementAtIndex(i);

                return;
            }

            GUI.color = GUI.backgroundColor;

            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            float defaultLabelWidth = EditorGUIUtility.labelWidth;

            EditorGUIUtility.labelWidth = 110.0f;

            EditorGUILayout.PropertyField(wheelTransformSerializedProperty, new GUIContent("Wheel Model", "Wheel Model"), true);

            EditorGUIUtility.labelWidth = defaultLabelWidth;

            EditorGUILayout.EndHorizontal();



            EditorGUILayout.EndVertical();
        }


        EditorGUILayout.BeginHorizontal();

        GUI.color = m_CustomBlueColor;

        if (GUILayout.Button("Add Upper Wheel Data"))
        {
            int endIndexPlusOne = upperWheelDataArraySerializedProperty.arraySize;

            upperWheelDataArraySerializedProperty.InsertArrayElementAtIndex(endIndexPlusOne);

            SerializedProperty endUpperWheelDataSerializedProperty = upperWheelDataArraySerializedProperty.GetArrayElementAtIndex(endIndexPlusOne);

            SerializedProperty wheelTransformSerializedProperty = endUpperWheelDataSerializedProperty.FindPropertyRelative("wheelTransform");

            wheelTransformSerializedProperty.objectReferenceValue = null;

        }

        GUI.color = GUI.backgroundColor;

        EditorGUILayout.EndHorizontal();



        EditorGUILayout.EndVertical();

        #endregion

        EditorGUILayout.Space();

        #region show suspendedWheelDataArray

        SerializedProperty suspendedWheelDataArraySerializedProperty = trackDataSerializedProperty.FindPropertyRelative("suspendedWheelDataArray");

        //Debug.Log(suspendedWheelDataArraySerializedProperty.objectReferenceValue as System.Object);

        EditorGUILayout.BeginVertical(GUI.skin.box);


        EditorGUILayout.BeginHorizontal();

        GUIStyle suspendedWheelDataArrayLabelFieldGUIStyle = new GUIStyle();

        suspendedWheelDataArrayLabelFieldGUIStyle.alignment = TextAnchor.MiddleCenter;

        suspendedWheelDataArrayLabelFieldGUIStyle.normal.textColor = m_CustomBlueColor;

        EditorGUILayout.LabelField(new GUIContent("SuspendedWheelDataArray:"), suspendedWheelDataArrayLabelFieldGUIStyle);

        EditorGUILayout.EndHorizontal();


        for (int i = 0; i < suspendedWheelDataArraySerializedProperty.arraySize; i++)
        {
            SerializedProperty suspendedWheelDataSerializedProperty = suspendedWheelDataArraySerializedProperty.GetArrayElementAtIndex(i);

            SerializedProperty wheelTransformSerializedProperty = suspendedWheelDataSerializedProperty.FindPropertyRelative("wheelTransform");

            SerializedProperty wheelBoneTransformSerializedProperty = suspendedWheelDataSerializedProperty.FindPropertyRelative("wheelBoneTransform");

            SerializedProperty wheelColliderSerializedProperty = suspendedWheelDataSerializedProperty.FindPropertyRelative("wheelCollider");


            EditorGUILayout.BeginVertical(GUI.skin.box);


            EditorGUILayout.BeginHorizontal();

            GUI.color = m_CustomBlueColor;

            EditorGUILayout.LabelField(string.Format("SuspendedWheelData {0}:", i.ToString()), EditorStyles.boldLabel);

            GUI.color = GUI.backgroundColor;

            GUILayout.FlexibleSpace();

            GUI.color = m_CustomRedColor;

            if (GUILayout.Button("X"))
            {
                WheelCollider wheelCollider = wheelColliderSerializedProperty.objectReferenceValue as WheelCollider;

                suspendedWheelDataArraySerializedProperty.DeleteArrayElementAtIndex(i);

                if (wheelCollider != null)
                {
                    DestroyImmediate(wheelCollider.gameObject);
                }

                return;
            }

            GUI.color = GUI.backgroundColor;

            EditorGUILayout.EndHorizontal();




            EditorGUILayout.BeginHorizontal();

            float defaultLabelWidth = EditorGUIUtility.labelWidth;

            EditorGUIUtility.labelWidth = 110.0f;


            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.PropertyField(wheelTransformSerializedProperty, new GUIContent("Wheel Model", "Wheel Model"), true);

            EditorGUILayout.EndVertical();


            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.PropertyField(wheelBoneTransformSerializedProperty, new GUIContent("Wheel Bone", "Wheel Bone"), true);

            EditorGUILayout.EndVertical();



            if (wheelColliderSerializedProperty.objectReferenceValue != null)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);

                EditorGUILayout.PropertyField(wheelColliderSerializedProperty, new GUIContent("Wheel Collider", "Wheel Collider"), true);

                EditorGUILayout.EndVertical();
            }
            else
            {
                EditorGUILayout.BeginVertical();

                GUI.color = m_CustomGreenColor;

                if (GUILayout.Button("Create WheelCollider"))
                {
                    TrackSimulator.SuspendedWheelData newSuspendedWheelData = suspendedWheelDataArray[i];

                    AddWheelCollider(vehicleRootGameObject, wheelCollidersParentGameObjectName, newSuspendedWheelData);
                }

                GUI.color = GUI.backgroundColor;

                EditorGUILayout.EndVertical();
            }

            EditorGUIUtility.labelWidth = defaultLabelWidth;

            EditorGUILayout.EndHorizontal();




            EditorGUILayout.EndVertical();

        }


        GUI.color = m_CustomBlueColor;

        if (GUILayout.Button("Add Suspended Wheel Data"))
        {
            int endIndexPlusOne = suspendedWheelDataArraySerializedProperty.arraySize;

            suspendedWheelDataArraySerializedProperty.InsertArrayElementAtIndex(endIndexPlusOne);

            SerializedProperty endSuspendedWheelDataSerializedProperty = suspendedWheelDataArraySerializedProperty.GetArrayElementAtIndex(endIndexPlusOne);

            SerializedProperty wheelTransformSerializedProperty = endSuspendedWheelDataSerializedProperty.FindPropertyRelative("wheelTransform");

            SerializedProperty wheelBoneTransformSerializedProperty = endSuspendedWheelDataSerializedProperty.FindPropertyRelative("wheelBoneTransform");

            SerializedProperty wheelColliderSerializedProperty = endSuspendedWheelDataSerializedProperty.FindPropertyRelative("wheelCollider");

            wheelTransformSerializedProperty.objectReferenceValue = null;

            wheelBoneTransformSerializedProperty.objectReferenceValue = null;

            wheelColliderSerializedProperty.objectReferenceValue = null;
        }

        GUI.color = GUI.backgroundColor;


        EditorGUILayout.EndVertical();


        #endregion

        EditorGUILayout.Space();

        #region show trackSkinnedMeshRenderer

        SerializedProperty trackSkinnedMeshRendererSerializedProperty = trackDataSerializedProperty.FindPropertyRelative("trackSkinnedMeshRenderer");


        EditorGUILayout.BeginVertical(GUI.skin.box);


        EditorGUILayout.BeginHorizontal();

        GUIStyle trackSkinnedMeshRendererLabelFieldGUIStyle = new GUIStyle();

        trackSkinnedMeshRendererLabelFieldGUIStyle.alignment = TextAnchor.MiddleCenter;

        trackSkinnedMeshRendererLabelFieldGUIStyle.normal.textColor = m_CustomBlueColor;

        EditorGUILayout.LabelField(new GUIContent("Track SkinnedMeshRenderer:"), trackSkinnedMeshRendererLabelFieldGUIStyle);

        EditorGUILayout.EndHorizontal();



        EditorGUILayout.BeginHorizontal(GUI.skin.box);

        EditorGUILayout.PropertyField(trackSkinnedMeshRendererSerializedProperty, new GUIContent("Track SkinnedMeshRenderer", "Track SkinnedMeshRenderer"), true);

        EditorGUILayout.EndHorizontal();



        if (trackSkinnedMeshRendererSerializedProperty.objectReferenceValue != null)
        {
            GUI.color = m_CustomRedColor;

            if (GUILayout.Button("Remove Track SkinnedMeshRenderer"))
            {
                trackSkinnedMeshRendererSerializedProperty.DeleteCommand();
            }

            GUI.color = GUI.backgroundColor;
        }


        EditorGUILayout.EndVertical();

        #endregion

    }

    /// <summary>
    /// Add wheelCollider to current selected wheel which is on the lower position 
    /// </summary>
    /// <param name="vehicleRootGameObject">The root gameObect of the vehicle</param>
    /// <param name="wheelCollidersParentGameObjectName">The wheelColliders' parent gameobject's name </param>
    /// <param name="suspendedWheelData">The wheel which is on the lower position</param>
    private void AddWheelCollider(GameObject vehicleRootGameObject, string wheelCollidersParentGameObjectName, TrackSimulator.SuspendedWheelData suspendedWheelData)
    {
        if (suspendedWheelData.wheelTransform == null)
        {
            EditorUtility.DisplayDialog("Track Simulator ", "Before adding Wheel Collider,you must set its Wheel Model", "OK");

            return;
        }

        Transform wheelCollidersParentGameObjectTransform = vehicleRootGameObject.transform.Find(wheelCollidersParentGameObjectName);

        GameObject wheelCollidersParentGameObject = null;

        if (wheelCollidersParentGameObjectTransform == null)
        {
            wheelCollidersParentGameObject = new GameObject(wheelCollidersParentGameObjectName);

            wheelCollidersParentGameObject.transform.SetParent(vehicleRootGameObject.transform);

            wheelCollidersParentGameObject.transform.localPosition = Vector3.zero;

            wheelCollidersParentGameObject.transform.localRotation = Quaternion.identity;

            wheelCollidersParentGameObject.transform.localScale = Vector3.one;

        }
        else
        {
            wheelCollidersParentGameObject = wheelCollidersParentGameObjectTransform.gameObject;
        }


        GameObject wheelColliderGameObject = new GameObject(string.Format("{0}{1}", suspendedWheelData.wheelTransform.name, "Collider"));

        wheelColliderGameObject.transform.position = suspendedWheelData.wheelTransform.position;

        wheelColliderGameObject.transform.rotation = suspendedWheelData.wheelTransform.rotation;

        wheelColliderGameObject.transform.SetParent(wheelCollidersParentGameObject.transform);

        wheelColliderGameObject.transform.localScale = Vector3.one;


        WheelCollider wheelCollider = wheelColliderGameObject.AddComponent<WheelCollider>();

        MeshRenderer wheelTransformMeshRenderer = suspendedWheelData.wheelTransform.GetComponent<MeshRenderer>();

        wheelCollider.radius = wheelTransformMeshRenderer.bounds.size.y / 2.0f;


        JointSpring spring = wheelCollider.suspensionSpring;

        spring.spring = 400000.0f;

        spring.damper = 10000.0f;

        spring.targetPosition = 0.2f;

        wheelCollider.suspensionSpring = spring;


        wheelCollider.suspensionDistance = 0.4f;

        wheelCollider.forceAppPointDistance = 0.0f;

        wheelCollider.mass = 500.0f;

        wheelCollider.wheelDampingRate = 100.0f;


        WheelFrictionCurve forwardFriction = wheelCollider.forwardFriction;

        forwardFriction.extremumSlip = 0.4f;

        forwardFriction.extremumValue = 1.0f;

        forwardFriction.asymptoteSlip = 0.8f;

        forwardFriction.asymptoteValue = 0.75f;

        forwardFriction.stiffness = 1.75f;

        wheelCollider.forwardFriction = forwardFriction;


        WheelFrictionCurve sidewaysFriction = wheelCollider.sidewaysFriction;

        sidewaysFriction.extremumSlip = 0.25f;

        sidewaysFriction.extremumValue = 1.0f;

        sidewaysFriction.asymptoteSlip = 0.5f;

        sidewaysFriction.asymptoteValue = 0.75f;

        sidewaysFriction.stiffness = 2.0f;

        wheelCollider.sidewaysFriction = sidewaysFriction;


        suspendedWheelData.wheelCollider = wheelCollider;


        Debug.LogFormat("Created wheelcollider for {0}", suspendedWheelData.wheelTransform.name);

    }

    /// <summary>
    /// Add mass center gameObject named by massCenterName variable
    /// </summary>
    /// <param name="trackSimulator">The track simulator which be selected</param>
    /// <param name="massCenterName">The name of mass center</param>
    private void AddMassCenter(TrackSimulator trackSimulator, string massCenterName)
    {
        GameObject massCenterGameObject = new GameObject(massCenterName);

        GameObject vehicleGameObject = trackSimulator.gameObject;

        massCenterGameObject.transform.SetParent(vehicleGameObject.transform);

        massCenterGameObject.transform.localPosition = Vector3.zero;

        massCenterGameObject.transform.localRotation = Quaternion.identity;

        massCenterGameObject.transform.localScale = Vector3.one;

        trackSimulator.centerOfMass = massCenterGameObject.transform;

    }

}
