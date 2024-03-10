using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Content.Scripts.IslandGame
{
    public class DirectionalTrigger : MonoBehaviour
    {
        [SerializeField] private float detectionAngle;
        [SerializeField, FoldoutGroup("Gizmo")] private Color color;
        [SerializeField, FoldoutGroup("Gizmo")] private Mesh arrowMesh;
        public Action<Collider> OnTriggerEntered;


        private void OnTriggerEnter(Collider other)
        {
            if (Vector3.Angle(other.transform.forward, transform.forward) < detectionAngle)
            {
                OnTriggerEntered?.Invoke(other);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = color;
            var col = GetComponent<BoxCollider>();

            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            
            Gizmos.DrawCube(col.center.MultiplyVector3(transform.localScale), col.size.MultiplyVector3(transform.localScale));
            Gizmos.color = Color.blue;
            Gizmos.DrawMesh(arrowMesh, Vector3.zero, Quaternion.Euler(Vector3.right * 90), Vector3.one * 20);
        }
    }
}
