using UnityEngine;

namespace Crow.SimpleRoadSystem
{
    public class SRSDynamicMeshTypePreview : MonoBehaviour
    {
        public float resolution = 1f;
        public SRSDynamicMeshType dynamicMeshType;
        static int _segments = 5;

        void OnDrawGizmos()
        {
            if (!dynamicMeshType) return;

            Gizmos.color = Color.yellow;

            for (var i = 0; i <= _segments; i++)
            {
                for (var j = 0; j < dynamicMeshType.crossSection.Length; j++)
                {
                    var a = transform.TransformPoint(dynamicMeshType.crossSection[j]) +
                            Vector3.forward * i / resolution;

                    if (i != _segments)
                        Gizmos.DrawRay(a, transform.forward / resolution);

                    if (j + 1 != dynamicMeshType.crossSection.Length)
                    {
                        var b = transform.TransformPoint(dynamicMeshType.crossSection[j + 1]) +
                                Vector3.forward * i / resolution;
                        Gizmos.DrawLine(a, b);
                    }
                }
            }
        }

        void OnValidate()
        {
            resolution = Mathf.Max(0.1f, resolution);
            _segments = Mathf.Max(1, _segments);
        }
    }
}