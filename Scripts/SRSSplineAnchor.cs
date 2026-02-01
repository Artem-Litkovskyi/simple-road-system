using UnityEngine;

namespace Crow.SimpleRoadSystem
{
    public class SRSSplineAnchor : MonoBehaviour
    {
        public float weightBack = 30f;
        public float weightFront = 30f;

        public void Start()
        {
            Destroy(gameObject);
        }

        public static SRSSplineAnchor CreateNew(Vector3 position, Quaternion rotation, Transform parent)
        {
            GameObject newAnchor = new("Road Anchor") { transform = { parent = parent } };
            newAnchor.transform.SetPositionAndRotation(position, rotation);
            return newAnchor.AddComponent<SRSSplineAnchor>();
        }

        public Vector3 GetFrontHandlePos()
        {
            return transform.position + transform.forward * weightFront;
        }

        public Vector3 GetBackHandlePos()
        {
            return transform.position - transform.forward * weightBack;
        }

        void OnValidate()
        {
            weightBack = Mathf.Max(0f, weightBack);
            weightFront = Mathf.Max(0f, weightFront);
        }

        void OnDrawGizmos()
        {
            // Draw marker
            Gizmos.color = weightBack == 0 ? Color.black : Color.yellow;
            Gizmos.DrawSphere(transform.position, .25f);
            Gizmos.DrawLine(transform.TransformPoint(Vector3.left), transform.TransformPoint(Vector3.right));
        }

        void OnDrawGizmosSelected()
        {
            // Draw handles
            Gizmos.color = Color.black;
            Gizmos.DrawLine(transform.position, GetFrontHandlePos());
            Gizmos.DrawLine(transform.position, GetBackHandlePos());
        }
    }
}