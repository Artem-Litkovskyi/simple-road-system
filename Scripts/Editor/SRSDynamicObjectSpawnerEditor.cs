using UnityEngine;
using UnityEditor;

namespace Crow.SimpleRoadSystem.EditorTools
{
    [CustomEditor(typeof(SRSDynamicObjectSpawner))]
    public class SRSDynamicObjectSpawnerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var roadMeshGenerator = (SRSDynamicObjectSpawner)target;

            base.OnInspectorGUI();
            EditorGUILayout.Space();

            if (GUILayout.Button("Update objects"))
                roadMeshGenerator.UpdateObjects();

            EditorGUILayout.HelpBox("1. Destroys child GameObjects\n2. Spawns new GameObjects from prefab",
                MessageType.None);
        }
    }
}