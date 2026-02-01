using UnityEngine;

namespace Crow.SimpleRoadSystem
{
    public static class SRSBezier
    {
        public static Vector3 CubicInterpolation(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
        {
            return Mathf.Pow(1 - t, 3f) * a + 3f * Mathf.Pow(1 - t, 2f) * t * b + 3f * (1 - t) * Mathf.Pow(t, 2f) * c +
                   Mathf.Pow(t, 3f) * d;
        }

        public static float GetEstimatedCurveLength(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            var controlNetLength = Vector3.Distance(a, b) + Vector3.Distance(b, c) + Vector3.Distance(c, d);
            var estimatedCurveLength = Vector3.Distance(a, d) + controlNetLength / 2f;
            return estimatedCurveLength;
        }

        public static void DrawBezierGizmo(Vector3 a, Vector3 b, Vector3 c, Vector3 d, int segments, bool dots = true)
        {
            var lastPoint = a;
            for (var i = 1; i <= segments; i++)
            {
                var currentPoint = CubicInterpolation(a, b, c, d, (float)i / (float)segments);
                if (dots && i != segments) Gizmos.DrawSphere(currentPoint, .125f);
                Gizmos.DrawLine(lastPoint, currentPoint);
                lastPoint = currentPoint;
            }
        }
    }
}