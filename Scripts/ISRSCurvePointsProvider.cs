using System.Collections.Generic;
using UnityEngine;

namespace Crow.SimpleRoadSystem
{
    public struct SRSCurvePointData
    {
        public Vector3 position;
        public Vector3 up;
        public Vector3 right;
        public float turnRadius;
    }

    public interface ISRSCurvePointsProvider
    {
        public IEnumerable<SRSCurvePointData> GetCurvePointsData();
    }
}