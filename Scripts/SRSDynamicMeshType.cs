using UnityEngine;

namespace Crow.SimpleRoadSystem
{
    [CreateAssetMenu(fileName = "DynamicMeshType", menuName = "Simple Road System/SRSDynamicMeshType", order = 1)]
    public class SRSDynamicMeshType : ScriptableObject
    {
        public Vector2[] crossSection = {
            new(-3.5f, -0.9f),
            new(-3.5f, -0.15f),
            new(-3.5f, 0.1f),
            new(-3.25f, 0.1f),
            new(-3f, 0.1f),
            new(-2f, 0.1f),
            new(-1f, 0.1f),
            new(0f, 0.1f),
            new(1f, 0.1f),
            new(2f, 0.1f),
            new(3f, 0.1f),
            new(3.25f, 0.1f),
            new(3.5f, 0.1f),
            new(3.5f, -0.15f),
            new(3.5f, -0.9f)
        };

        [Space] public Material material;
        public PhysicsMaterial physicsMaterial;
    }
}