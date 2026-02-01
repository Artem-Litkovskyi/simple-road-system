using UnityEngine;

namespace Crow.SimpleRoadSystem
{
    public class SRSStaticRoadTerrainUpdater : MonoBehaviour
    {
        public float brushSize = 15f;

        public void Start()
        {
            Destroy(this);
        }

        public void UpdateTerrains()
        {
            // Update Terrains
            foreach (var terrain in FindObjectsByType<Terrain>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                var maxRadius = Mathf.RoundToInt(brushSize / 2f / terrain.terrainData.heightmapScale.x);
                var heightmap = terrain.terrainData.GetHeights(
                    0, 0,
                    terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution
                );

                for (var i = 0; i < maxRadius; i++)
                {
                    var localPoint = terrain.transform.InverseTransformPoint(transform.position);
                    var centerHeightmapX = Mathf.RoundToInt(localPoint.z / terrain.terrainData.heightmapScale.z);
                    var centerHeightmapY = Mathf.RoundToInt(localPoint.x / terrain.terrainData.heightmapScale.x);
                    var centerHeightmapHeight = Mathf.Clamp01(localPoint.y / terrain.terrainData.heightmapScale.y);

                    var minHeightmapX = centerHeightmapX - maxRadius;
                    var minHeightmapY = centerHeightmapY - maxRadius;
                    var maxHeightmapX = centerHeightmapX + maxRadius;
                    var maxHeightmapY = centerHeightmapY + maxRadius;

                    for (var x = minHeightmapX; x <= maxHeightmapX; x++)
                    {
                        for (var y = minHeightmapY; y <= maxHeightmapY; y++)
                        {
                            if (x < 0 || x >= terrain.terrainData.heightmapResolution)
                                continue;
                            if (y < 0 || y >= terrain.terrainData.heightmapResolution)
                                continue;

                            var radius = Mathf.Sqrt(Mathf.Pow(centerHeightmapX - x, 2) +
                                                    Mathf.Pow(centerHeightmapY - y, 2));
                            if (radius < maxRadius - i)
                                heightmap[x, y] = centerHeightmapHeight;
                        }
                    }
                }

                terrain.terrainData.SetHeights(0, 0, heightmap);
            }
        }
    }
}