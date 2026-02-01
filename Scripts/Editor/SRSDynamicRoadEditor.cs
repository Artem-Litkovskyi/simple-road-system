using UnityEngine;
using UnityEditor;

namespace Crow.SimpleRoadSystem.EditorTools
{
    [CustomEditor(typeof(SRSDynamicSplineRoad))]
    public class SRSDynamicSplineRoadEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var dynamicRoad = (SRSDynamicSplineRoad)target;

            base.OnInspectorGUI();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Actions:", EditorStyles.boldLabel);
            if (GUILayout.Button("Add new anchor"))
                dynamicRoad.AddNewAnchor();
            EditorGUILayout.HelpBox("Creates a new anchor in front of the last one.", MessageType.None);
            EditorGUILayout.Space();

            if (GUILayout.Button("Clean"))
                dynamicRoad.CleanAnchorsAndChildObjects();
            EditorGUILayout.HelpBox(
                "1. Removes null items from anchors list\n2. Destroys child GameObjects that are not in anchors list\n3. Renames anchors according to their order",
                MessageType.None);
        }
    }
}