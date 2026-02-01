using System.Collections.Generic;
using UnityEngine;

namespace Crow.SimpleRoadSystem
{
    public class SRSDynamicObjectSpawner : MonoBehaviour
    {
        public GameObject curvePointsProvider;

        [Header("Main settings:")] public GameObject objectPrefab;

        [Space] public float minDistance = 4f;
        public float maxDistance = 4f;
        public int randomizerSeed = 1000;

        [Space] public Vector2 offset = Vector2.zero;
        public bool flip = false;
        public bool mirror = false;

        [Space] public bool turnDependent = false;
        public float turnRadiusThreshold = 10f;

        public void OnValidate()
        {
            if (curvePointsProvider != null && curvePointsProvider.GetComponent<ISRSCurvePointsProvider>() == null)
                Debug.LogError(
                    "Assigned GameObject does not have a componemt which implements ISRSCurvePointsProvider.");
        }

        public void Start()
        {
            Destroy(this);
        }

        public void UpdateObjects()
        {
            DestroyChildObjects();

            var curvePointsData = SRSCurvePointsProcessing.ResampleRandomInterval(
                curvePointsProvider.GetComponent<ISRSCurvePointsProvider>().GetCurvePointsData(),
                minDistance, maxDistance, randomizerSeed);

            SpawnChildObjects(curvePointsData, flip);
            if (mirror) SpawnChildObjects(curvePointsData, !flip);
        }
        
        void DestroyChildObjects()
        {
            for (var i = transform.childCount - 1; i >= 0; i--)
            {
                var child = transform.GetChild(i);
                DestroyImmediate(child.gameObject);
            }
        }

        void SpawnChildObjects(IEnumerable<SRSCurvePointData> curvePointsData, bool inverseX)
        {
            var curve = curvePointsData.GetEnumerator();

            while (curve.MoveNext())
            {
                var pointCur = curve.Current;

                if (turnDependent && Mathf.Abs(pointCur.turnRadius) > turnRadiusThreshold) continue;

                var mir = turnDependent && (pointCur.turnRadius > 0f);
                if (inverseX) mir = !mir;

                var newObject = Instantiate(
                    objectPrefab,
                    pointCur.position + (mir ? -1 : 1) * offset.x * pointCur.right + offset.y * pointCur.up,
                    Quaternion.LookRotation(Vector3.Cross(pointCur.right, pointCur.up), pointCur.up),
                    transform);

                if (mir) newObject.transform.localScale = new Vector3(-1f, 1f, 1f);
            }
        }
    }
}