using UnityEngine;
using UnityEditor;

namespace Crow.SimpleRoadSystem.EditorTools
{
    [CustomEditor(typeof(SRSDynamicMeshGenerator))]
    public class SRSDynamicMeshGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var roadMeshGenerator = (SRSDynamicMeshGenerator)target;

            base.OnInspectorGUI();
            EditorGUILayout.Space();

            if (GUILayout.Button("Update mesh"))
                roadMeshGenerator.UpdateMesh();

            EditorGUILayout.HelpBox(
                "1. Destroys child GameObjects\n2. Generates road mesh\n3. Sets it for MeshFilter and MeshCollider\n4. Updates MeshRenderer material",
                MessageType.None);
        }
    }
}