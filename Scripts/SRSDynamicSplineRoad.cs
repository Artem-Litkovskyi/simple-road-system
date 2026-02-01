using System.Collections.Generic;
using UnityEngine;

namespace Crow.SimpleRoadSystem
{
    public class SRSDynamicSplineRoad : MonoBehaviour, ISRSCurvePointsProvider
    {
        public float resolution = 0.5f;

        [Header("Optional anchors:")] public SRSSplineAnchor startAnchor;
        public SRSSplineAnchor endAnchor;

        [Header("Main anchors:")] public List<SRSSplineAnchor> anchors;
        
        const float NewAnchorDistance = 100f;

        public void Start()
        {
            Destroy(this);
        }

        public void AddNewAnchor()
        {
            var newAnchor = SRSSplineAnchor.CreateNew(Vector3.zero, Quaternion.identity, transform);

            anchors ??= new List<SRSSplineAnchor>();
            
            if (anchors.Count != 0)
            {
                for (var i = anchors.Count - 1; i >= 0; i--)
                {
                    if (!anchors[i])
                    {
                        anchors.RemoveAt(i);
                        continue;
                    }

                    var lastAnchor = anchors[^1].transform;
                    newAnchor.transform.SetPositionAndRotation(
                        lastAnchor.position + lastAnchor.forward * NewAnchorDistance,
                        lastAnchor.rotation
                    );
                }
            }

            anchors.Add(newAnchor);
        }

        public void CleanAnchorsAndChildObjects()
        {
            // Remove null items from the list
            anchors ??= new List<SRSSplineAnchor>();
            for (var i = anchors.Count - 1; i >= 0; i--)
                if (!anchors[i])
                    anchors.RemoveAt(i);

            // Rename all child GameObjects to "TMP"
            for (var i = 0; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.name = "TMP";

            // Rename anchors
            for (var i = 0; i < anchors.Count; i++)
                anchors[i].name = "Road Anchor " + i;

            // Destroy unused child GameObjects
            for (var i = transform.childCount - 1; i >= 0; i--)
            {
                var child = transform.GetChild(i);
                if (child.gameObject.name == "TMP")
                    DestroyImmediate(child.gameObject);
            }
        }

        public IEnumerable<SRSCurvePointData> GetCurvePointsData()
        {
            List<SRSCurvePointData> pointsData = new();

            if (anchors == null || anchors.Count == 0)
            {
                Debug.LogWarning("Anchors list is empty.");
                return pointsData;
            }


            // Get points data (right vectors are NOT calculated)
            if (startAnchor)
                pointsData.AddRange(GetSegmentPointsData(startAnchor, anchors[0]));

            for (var i = 0; i < anchors.Count - 1; i++)
                pointsData.AddRange(GetSegmentPointsData(anchors[i], anchors[i + 1]));


            if (endAnchor)
                pointsData.AddRange(GetSegmentPointsData(anchors[^1], endAnchor, true));

            SRSCurvePointData lastPoint = new();
            var lastAnchor = !endAnchor ? anchors[^1] : endAnchor;
            lastPoint.position = lastAnchor.transform.position;
            lastPoint.up = lastAnchor.transform.up;
            pointsData.Add(lastPoint);


            // Calculate right vectors
            for (var i = 0; i < pointsData.Count; i++)
            {
                var pointData = pointsData[i];

                if (i == 0)
                    pointData.right = startAnchor ? startAnchor.transform.right : anchors[0].transform.right;
                else if (i == pointsData.Count - 1)
                    pointData.right = !endAnchor ? anchors[^1].transform.right : -endAnchor.transform.right;
                else
                {
                    var prevForward = pointData.position - pointsData[i - 1].position;
                    var nextForward = pointsData[i + 1].position - pointData.position;
                    var thisForward = (prevForward + nextForward) / 2f;
                    pointData.right = Vector3.Cross(pointData.up, thisForward).normalized;
                }

                pointsData[i] = pointData;
            }

            return SRSCurvePointsProcessing.CalculateTurnRadius(pointsData);
        }

        SRSCurvePointData[] GetSegmentPointsData(SRSSplineAnchor anchor0, SRSSplineAnchor anchor1,
            bool anchor1IsReversed = false)
        {
            // Returns an array of SRSCurvePointData (right vectors are NOT calculated).
            
            var a = anchor0.transform.position;
            var b = anchor0.GetFrontHandlePos();
            var c = anchor1IsReversed ? anchor1.GetFrontHandlePos() : anchor1.GetBackHandlePos();
            var d = anchor1.transform.position;

            var pointsNumber = Mathf.CeilToInt(resolution * SRSBezier.GetEstimatedCurveLength(a, b, c, d));

            var pointsData = new SRSCurvePointData[pointsNumber];
            for (var i = 0; i < pointsNumber; i++)
            {
                var t = i / (float)pointsNumber;

                SRSCurvePointData newPoint = new()
                {
                    position = SRSBezier.CubicInterpolation(a, b, c, d, t),
                    up = Vector3.Lerp(anchor0.transform.up, anchor1.transform.up, t).normalized
                };

                pointsData[i] = newPoint;
            }

            return pointsData;
        }

        void OnValidate()
        {
            resolution = Mathf.Max(0.1f, resolution);
        }

        void OnDrawGizmos()
        {
            if (anchors == null || anchors.Count == 0)
                return;

            // Draw start segment
            Gizmos.color = Color.gray;
            if (!anchors[0])
            {
                Debug.LogWarning("Null reference in anchors list found. Press 'Clean' button to remove them.");
                return;
            }

            if (startAnchor)
            {
                var a = startAnchor.transform.position;
                var b = startAnchor.GetFrontHandlePos();
                var c = anchors[0].GetBackHandlePos();
                var d = anchors[0].transform.position;

                var curveSegments = Mathf.CeilToInt(resolution * SRSBezier.GetEstimatedCurveLength(a, b, c, d));

                SRSBezier.DrawBezierGizmo(a, b, c, d, curveSegments);
            }

            // Draw main segments
            Gizmos.color = Color.yellow;
            for (var i = 1; i < anchors.Count; i++)
            {
                if (!anchors[i])
                {
                    Debug.LogWarning("Null reference in anchors list found. Press 'Clean' button to remove them.");
                    return;
                }

                var a = anchors[i - 1].transform.position;
                var b = anchors[i - 1].GetFrontHandlePos();
                var c = anchors[i].GetBackHandlePos();
                var d = anchors[i].transform.position;

                var curveSegments = Mathf.CeilToInt(resolution * SRSBezier.GetEstimatedCurveLength(a, b, c, d));

                SRSBezier.DrawBezierGizmo(a, b, c, d, curveSegments);
            }

            // Draw end segment
            Gizmos.color = Color.gray;
            if (endAnchor)
            {
                var a = anchors[^1].transform.position;
                var b = anchors[^1].GetFrontHandlePos();
                var c = endAnchor.GetFrontHandlePos();
                var d = endAnchor.transform.position;

                var curveSegments = Mathf.CeilToInt(resolution * SRSBezier.GetEstimatedCurveLength(a, b, c, d));

                SRSBezier.DrawBezierGizmo(a, b, c, d, curveSegments);
            }
        }
    }
}