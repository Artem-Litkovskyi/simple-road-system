using UnityEngine;
using UnityEditor;

namespace Crow.SimpleRoadSystem.EditorTools
{
    [CustomEditor(typeof(SRSStaticRoadTerrainUpdater))]
    public class SRSStaticRoadTerrainUpdaterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var terrainUpdater = (SRSStaticRoadTerrainUpdater)target;

            base.OnInspectorGUI();
            EditorGUILayout.Space();

            if (GUILayout.Button("Update terrains"))
                terrainUpdater.UpdateTerrains();

            EditorGUILayout.HelpBox("Updates terrains to match the road", MessageType.None);
        }
    }
}