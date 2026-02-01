using System.Collections.Generic;
using UnityEngine;

namespace Crow.SimpleRoadSystem
{
    public static class SRSCurvePointsProcessing
    {
        public static IEnumerable<SRSCurvePointData> Resample(IEnumerable<SRSCurvePointData> original, float interval)
        {
            return ResampleRandomInterval(original, interval, interval, 0);
        }

        public static IEnumerable<SRSCurvePointData> ResampleRandomInterval(IEnumerable<SRSCurvePointData> original,
            float minInterval, float maxInterval, int randomizerSeed)
        {
            var random = new System.Random(randomizerSeed);

            var curve = original.GetEnumerator();

            curve.MoveNext();
            var segmentA = curve.Current;

            yield return segmentA;
            var interval = RandomRange(random, minInterval, maxInterval);
            var posRelToSegmentA = interval;

            while (curve.MoveNext())
            {
                var segmentB = curve.Current;

                var segment = segmentB.position - segmentA.position;
                var segmentDirection = segment.normalized;
                var segmentLength = segment.magnitude;

                while (posRelToSegmentA < segmentLength)
                {
                    SRSCurvePointData pointData = new()
                    {
                        position = segmentA.position + posRelToSegmentA * segmentDirection,
                        up = segmentA.up,
                        right = segmentA.right,
                        turnRadius = segmentA.turnRadius
                    };

                    yield return pointData;

                    interval = RandomRange(random, minInterval, maxInterval);
                    posRelToSegmentA += interval;
                }

                posRelToSegmentA -= segmentLength;

                segmentA = segmentB;
            }

            yield return segmentA;
        }

        public static IEnumerable<SRSCurvePointData> CalculateTurnRadius(IEnumerable<SRSCurvePointData> original)
        {
            var curve = original.GetEnumerator();

            SRSCurvePointData pointPrev = new();
            SRSCurvePointData pointCur = new();
            SRSCurvePointData pointNext;

            var counter = 1;
            
            var turnRadius = Mathf.Infinity;
            var turnDirection = true;

            while (curve.MoveNext())
            {
                pointNext = curve.Current;

                if (counter != 1)  // pointCur exists
                {
                    turnRadius = Mathf.Infinity;
                    
                    if (counter >= 3)  // Previous, current and next points exist
                    {
                        var toNext = pointNext.position - pointCur.position;
                        var toPrev = pointPrev.position - pointCur.position;

                        var angle = Mathf.Deg2Rad * Vector3.SignedAngle(
                            Vector3.ProjectOnPlane(toNext, Vector3.up),
                            Vector3.ProjectOnPlane(toPrev, Vector3.up),
                            Vector3.up);

                        turnRadius = toNext.magnitude / Mathf.Sqrt(2f * (1f + Mathf.Cos(Mathf.Abs(angle))));
                        turnDirection = angle < 0f;
                    }

                    yield return new SRSCurvePointData
                    {
                        position = pointCur.position,
                        up = pointCur.up,
                        right = pointCur.right,
                        turnRadius = (turnDirection ? 1 : -1) * turnRadius
                    };

                    pointPrev = pointCur;
                }

                pointCur = pointNext;

                counter++;
            }

            yield return new SRSCurvePointData
            {
                position = pointCur.position,
                up = pointCur.up,
                right = pointCur.right,
                turnRadius = (turnDirection ? 1 : -1) * turnRadius
            };
        }

        static float RandomRange(System.Random rand, float min, float max)
        {
            return min + (max - min) * (float)rand.NextDouble();
        }
    }
}