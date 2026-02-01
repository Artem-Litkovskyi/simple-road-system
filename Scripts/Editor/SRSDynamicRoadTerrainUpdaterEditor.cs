using UnityEngine;
using UnityEditor;

namespace Crow.SimpleRoadSystem.EditorTools
{
    [CustomEditor(typeof(SRSDynamicRoadTerrainUpdater))]
    public class SRSDynamicRoadTerrainUpdaterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var terrainUpdater = (SRSDynamicRoadTerrainUpdater)target;

            base.OnInspectorGUI();
            EditorGUILayout.Space();

            if (GUILayout.Button("Update terrains"))
                terrainUpdater.UpdateTerrains();

            EditorGUILayout.HelpBox("Updates terrains to match the road", MessageType.None);
        }
    }
}