using UnityEngine;
using UnityEditor;

namespace Crow.SimpleRoadSystem.EditorTools
{
    [CustomEditor(typeof(SimpleRoadSystem))]
    public class SimpleRoadSystemEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var simpleRoadSystem = (SimpleRoadSystem)target;

            EditorGUILayout.HelpBox(
                "This component controls bumps generation.\nCan be used to update all roads at once.",
                MessageType.None);

            base.OnInspectorGUI();
            EditorGUILayout.Space();

            if (GUILayout.Button("Update all roads"))
                simpleRoadSystem.UpdateChildObjects();
        }
    }
}