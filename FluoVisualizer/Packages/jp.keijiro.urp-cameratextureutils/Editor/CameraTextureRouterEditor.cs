using UnityEngine;
using UnityEditor;

namespace CameraTextureUtils {

[CustomEditor(typeof(CameraTextureRouter))]
sealed class CameraTextureRouterEditor : Editor
{
    SerializedProperty _depthTarget;
    SerializedProperty _motionTarget;
    SerializedProperty _depthEncoding;
    SerializedProperty _motionEncoding;

    static class Styles
    {
        public static readonly GUIContent DepthEncodingLabel =
            new GUIContent("Depth Encoding", "Depth output encoding: Raw device depth, Linear 0–1, or Linear eye-space distance.");

        public static readonly GUIContent MotionEncodingLabel =
            new GUIContent("Motion Encoding", "Motion output encoding: Signed float XY, or remapped to 0–1 with 0.5 center.");

        public static readonly GUIContent[] DepthEncodingOptions = new GUIContent[]
        {
            new GUIContent("Raw Depth", "Raw device depth from the camera depth buffer."),
            new GUIContent("Linear (0–1)", "Linearized depth in 0–1 range (near→far)."),
            new GUIContent("Linear Eye Distance", "Linear eye-space distance from the camera."),
        };

        public static readonly GUIContent[] MotionEncodingOptions = new GUIContent[]
        {
            new GUIContent("Signed Vector", "Signed motion vector in pixels (XY)."),
            new GUIContent("0.5 centered (0-1)", "Motion remapped to 0–1 around 0.5 center."),
        };
    }

    void OnEnable()
    {
        _depthTarget = serializedObject.FindProperty("_depthTarget");
        _motionTarget = serializedObject.FindProperty("_motionTarget");
        _depthEncoding = serializedObject.FindProperty("_depthEncoding");
        _motionEncoding = serializedObject.FindProperty("_motionEncoding");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_depthTarget);
        int depthIndex = _depthEncoding.enumValueIndex;
        depthIndex = EditorGUILayout.Popup(Styles.DepthEncodingLabel, depthIndex, Styles.DepthEncodingOptions);
        if (depthIndex != _depthEncoding.enumValueIndex) _depthEncoding.enumValueIndex = depthIndex;

        EditorGUILayout.PropertyField(_motionTarget);
        int motionIndex = _motionEncoding.enumValueIndex;
        motionIndex = EditorGUILayout.Popup(Styles.MotionEncodingLabel, motionIndex, Styles.MotionEncodingOptions);
        if (motionIndex != _motionEncoding.enumValueIndex) _motionEncoding.enumValueIndex = motionIndex;

        serializedObject.ApplyModifiedProperties();
    }
}

} // namespace CameraTextureUtils
