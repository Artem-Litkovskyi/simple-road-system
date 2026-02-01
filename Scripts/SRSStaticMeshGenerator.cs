using UnityEngine;

namespace Crow.SimpleRoadSystem
{
    public class SRSStaticMeshGenerator : MonoBehaviour
    {
        public Mesh baseMesh;
        public Material material;

        public void Start()
        {
            Destroy(this);
        }

        public void UpdateMesh()
        {
            var simpleRoadSystem = GetComponentInParent<SimpleRoadSystem>();
            var meshFilter = GetComponent<MeshFilter>();
            var meshRenderer = GetComponent<MeshRenderer>();
            var meshCollider = GetComponent<MeshCollider>();
            
            
            if (!simpleRoadSystem)
            {
                Debug.LogError("StaticMeshGenerator must be a child of a GameObject with SimpleRoadSystem component.");
                return;
            }

            if (!baseMesh)
            {
                Debug.LogError("No base mesh is selected.");
                return;
            }


            Mesh mesh = new();
            var vertices = baseMesh.vertices;

            for (var i = 0; i < vertices.Length; i++)
            {
                var localPos = vertices[i];
                localPos.y += simpleRoadSystem.GetBumpHeight(transform.TransformPoint(localPos));
                vertices[i] = localPos;
            }
            
            
            mesh.vertices = vertices;
            mesh.uv = baseMesh.uv;
            mesh.triangles = baseMesh.triangles;
            mesh.normals = baseMesh.normals;

            if (meshFilter) meshFilter.mesh = mesh;
            if (meshRenderer) meshRenderer.material = material;
            if (meshCollider) meshCollider.sharedMesh = mesh;
        }
    }
}