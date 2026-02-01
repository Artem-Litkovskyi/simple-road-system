using UnityEngine;

namespace Crow.SimpleRoadSystem
{
    public class SimpleRoadSystem : MonoBehaviour
    {
        [Header("Bumps settings:")] public float bumpsHeight = 0.1f;
        public float perlinNoiseScale = 0.5f;
        public float perlinNoiseSeed = 1000f;

        public void Start()
        {
            Destroy(this);
        }

        public void UpdateChildObjects()
        {
            var staticRoadMeshGenerators = GetComponentsInChildren<SRSStaticMeshGenerator>();
            var dynamicMeshGenerators = GetComponentsInChildren<SRSDynamicMeshGenerator>();
            var dynamicObjectSpawners = GetComponentsInChildren<SRSDynamicObjectSpawner>();
            var dynamicRoadTerrainUpdaters =
                GetComponentsInChildren<SRSDynamicRoadTerrainUpdater>();
            var staticRoadTerrainUpdaters =
                GetComponentsInChildren<SRSStaticRoadTerrainUpdater>();

            foreach (var item in staticRoadMeshGenerators)
                item.UpdateMesh();

            foreach (var item in dynamicMeshGenerators)
                item.UpdateMesh();

            foreach (var item in dynamicObjectSpawners)
                item.UpdateObjects();

            foreach (var item in dynamicRoadTerrainUpdaters)
                item.UpdateTerrains();

            foreach (var item in staticRoadTerrainUpdaters)
                item.UpdateTerrains();
        }

        public float GetBumpHeight(Vector3 worldPos)
        {
            return bumpsHeight * Mathf.PerlinNoise((worldPos.x + perlinNoiseSeed) * perlinNoiseScale,
                (worldPos.z + perlinNoiseSeed) * perlinNoiseScale);
        }
    }
}