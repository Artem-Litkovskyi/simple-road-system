using UnityEngine;
using UnityEditor;

namespace Crow.SimpleRoadSystem.EditorTools
{
    [CustomEditor(typeof(SRSSplineAnchor))]
    public class SRSSplineAnchorEditor : Editor
    {
        public void OnSceneGUI()
        {
            var roadAnchor = (SRSSplineAnchor)target;

            // Front handle
            Handles.color = Color.green;

            EditorGUI.BeginChangeCheck();
            var newFrontHandlePos = Handles.FreeMoveHandle(
                roadAnchor.GetFrontHandlePos(), .5f, Vector3.zero, Handles.SphereHandleCap);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(roadAnchor, "Change front handle weight");
                roadAnchor.weightFront = (newFrontHandlePos - roadAnchor.transform.position).magnitude;
            }

            // Back handle
            if (roadAnchor.weightBack != 0f)
            {
                Handles.color = Color.red;

                EditorGUI.BeginChangeCheck();
                var newBackHandlePos = Handles.FreeMoveHandle(
                    roadAnchor.GetBackHandlePos(), .5f, Vector3.zero, Handles.SphereHandleCap);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(roadAnchor, "Change back handle weight");
                    roadAnchor.weightBack = (newBackHandlePos - roadAnchor.transform.position).magnitude;
                }
            }
        }
    }
}