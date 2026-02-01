using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Crow.SimpleRoadSystem
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class SRSDynamicMeshGenerator : MonoBehaviour
    {
        public GameObject curvePointsProvider;

        [Header("Optional objects:")] public GameObject startObjectPrefab;
        public GameObject endObjectPrefab;

        [Header("Main settings:")] public SRSDynamicMeshType dynamicMeshType;

        [Space] public Vector2 offset = Vector2.zero;
        public bool flip = false;
        public bool ignoreBumpsGenerator = false;

        public void OnValidate()
        {
            if (curvePointsProvider && curvePointsProvider.GetComponent<ISRSCurvePointsProvider>() == null)
                Debug.LogError(
                    "Assigned GameObject does not have a component which implements ISRSCurvePointsProvider.");
        }

        public void Start()
        {
            Destroy(this);
        }

        public void UpdateMesh()
        {
            var curvePointsData = curvePointsProvider.GetComponent<ISRSCurvePointsProvider>()
                .GetCurvePointsData().ToList();

            var mesh = GenerateMesh(curvePointsData);
            
            var meshFilter = GetComponent<MeshFilter>();
            var meshRenderer = GetComponent<MeshRenderer>();
            var meshCollider = GetComponent<MeshCollider>();
            
            if (meshFilter) meshFilter.sharedMesh = mesh;
            if (meshRenderer && dynamicMeshType.material) meshRenderer.material = dynamicMeshType.material;
            if (meshCollider) meshCollider.sharedMesh = mesh;
            if (meshCollider && dynamicMeshType.physicsMaterial) meshCollider.material = dynamicMeshType.physicsMaterial;
            
            DestroyChildObjects();
            SpawnStartEndObjects(curvePointsData);
        }
        
        void DestroyChildObjects()
        {
            for (var i = transform.childCount - 1; i >= 0; i--)
            {
                var child = transform.GetChild(i);
                DestroyImmediate(child.gameObject);
            }
        }
        
        void SpawnStartEndObjects(List<SRSCurvePointData> curvePointsData)
        {
            SRSCurvePointData pointData;
            
            if (startObjectPrefab)
            {
                pointData = curvePointsData[0];
                var go = Instantiate(
                    startObjectPrefab,
                    pointData.position + pointData.right * offset.x * (flip ? -1 : 1) + pointData.up * offset.y,
                    Quaternion.LookRotation(Vector3.Cross(pointData.right, pointData.up), pointData.up),
                    transform);
                go.transform.localScale = flip ? new Vector3(-1f, 1f, -1f) : new Vector3(1f, 1f, -1f);
            }

            if (endObjectPrefab)
            {
                pointData = curvePointsData[^1];
                var go = Instantiate(
                    endObjectPrefab,
                    pointData.position + pointData.right * offset.x * (flip ? -1 : 1) + pointData.up * offset.y,
                    Quaternion.LookRotation(Vector3.Cross(pointData.right, pointData.up), pointData.up),
                    transform);
                go.transform.localScale = flip ? new Vector3(-1f, 1f, 1f) : new Vector3(1f, 1f, 1f);
            }
        }

        Mesh GenerateMesh(List<SRSCurvePointData> curvePointsData)
        {
            var (vertices, uv) = CalculateVerticesAndUV(curvePointsData);
            var triangles = CalculateTriangles(curvePointsData.Count);
            
            var mesh = new Mesh
            {
                vertices = vertices,
                uv = uv,
                triangles = triangles
            };
                
            mesh.RecalculateNormals();

            return mesh;
        }

        (Vector3[], Vector2[]) CalculateVerticesAndUV(List<SRSCurvePointData> curvePointsData)
        {
            var simpleRoadSystem = GetComponentInParent<SimpleRoadSystem>();
            
            if (!simpleRoadSystem)
            {
                Debug.LogError("DynamicMeshGenerator must be a child of a GameObject with SimpleRoadSystem component.");
                return (null, null);
            }

            if (!dynamicMeshType)
            {
                Debug.LogError("DynamicMeshType is not set.");
                return (null, null);
            }
            
            var vertices = new Vector3[curvePointsData.Count * dynamicMeshType.crossSection.Length];
            var uv = new Vector2[curvePointsData.Count * dynamicMeshType.crossSection.Length];
            
            var surfaceWidth = 0f;
            for (var i = 0; i < dynamicMeshType.crossSection.Length - 1; i++)
                surfaceWidth += Vector2.Distance(dynamicMeshType.crossSection[i], dynamicMeshType.crossSection[i + 1]);
            
            var vertexIndex = 0;
            var longitudinalPosOnSurface = 0f;

            for (var i = 0; i < curvePointsData.Count; i++)
            {
                var pointData = curvePointsData[i];

                if (i != 0)
                    longitudinalPosOnSurface += Vector3.Distance(curvePointsData[i - 1].position, pointData.position);

                var lateralPosOnSurface = 0f;
                for (var j = 0; j < dynamicMeshType.crossSection.Length; j++)
                {
                    var vertex = dynamicMeshType.crossSection[j] + offset;
                    if (flip) vertex.x *= -1f;

                    if (j != 0)
                        lateralPosOnSurface += Vector2.Distance(dynamicMeshType.crossSection[j - 1],
                            dynamicMeshType.crossSection[j]);

                    var worldPos = pointData.position + pointData.right * vertex.x + pointData.up * vertex.y;

                    if (!ignoreBumpsGenerator && simpleRoadSystem)
                        worldPos += pointData.up * simpleRoadSystem.GetBumpHeight(worldPos);

                    vertices[vertexIndex] = transform.InverseTransformPoint(worldPos);
                    uv[vertexIndex] = new Vector2(lateralPosOnSurface, longitudinalPosOnSurface) / surfaceWidth;
                    vertexIndex++;
                }
            }
            
            return (vertices, uv);
        }

        int[] CalculateTriangles(int curvePointsNumber)
        {
            var triangles = new int[(curvePointsNumber - 1) * (dynamicMeshType.crossSection.Length - 1) * 6];
            
            var vertexIndex = 0;
            var trianglesArrayIndex = 0;
            for (var i = 0; i < curvePointsNumber - 1; i++)
            {
                for (var j = 0; j < dynamicMeshType.crossSection.Length - 1; j++)
                {
                    triangles[trianglesArrayIndex] = vertexIndex;
                    trianglesArrayIndex++;
                    triangles[trianglesArrayIndex] = vertexIndex + dynamicMeshType.crossSection.Length + (flip ? 1 : 0);
                    trianglesArrayIndex++;
                    triangles[trianglesArrayIndex] = vertexIndex + dynamicMeshType.crossSection.Length + (flip ? 0 : 1);
                    trianglesArrayIndex++;

                    triangles[trianglesArrayIndex] = vertexIndex + (flip ? 1 : 0);
                    
                    trianglesArrayIndex++;
                    triangles[trianglesArrayIndex] = vertexIndex + dynamicMeshType.crossSection.Length + 1;
                    trianglesArrayIndex++;
                    triangles[trianglesArrayIndex] = vertexIndex + (flip ? 0 : 1);
                    
                    trianglesArrayIndex++;

                    vertexIndex++;
                }

                vertexIndex++;
            }

            return triangles;
        }
    }
}