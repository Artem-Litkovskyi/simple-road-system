using UnityEngine;
using UnityEditor;

namespace Crow.SimpleRoadSystem.EditorTools
{
    [CustomEditor(typeof(SRSStaticMeshGenerator))]
    public class SRSStaticMeshGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var roadMeshGenerator = (SRSStaticMeshGenerator)target;

            base.OnInspectorGUI();
            EditorGUILayout.Space();

            if (GUILayout.Button("Update mesh"))
                roadMeshGenerator.UpdateMesh();
            EditorGUILayout.HelpBox(
                "1. Adds bumps to the base mesh\n2. Sets it for MeshFilter and MeshCollider\n3. Updates MeshRenderer material",
                MessageType.None);
        }
    }
}