using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Content.Scripts.BoatGame.Ropes
{
    public class RopeGenerator : MonoBehaviour
    {
        [SerializeField] protected GameObject ropePart;
        [SerializeField] protected Transform point;
        [SerializeField] protected LineRenderer lineRenderer;

        [SerializeField] protected List<Rope> ropeParts = new List<Rope>();
        public Transform Point => point;
        private Transform endPointPosition;


        public Action OnRopeEnded;
        public Action OnRopeAnimationEnded;
        
        [System.Serializable]
        public class Rope
        {
            [SerializeField] private Joint joint;
            [SerializeField] private Rigidbody rb;

            public Rope(Joint start)
            {
                this.joint = start;
                this.rb = joint.GetComponent<Rigidbody>();
            }

            public Rigidbody Rb => rb;

            public Joint Joint => joint;
        }



        [Button]
        public void GenerateRope()
        {
            var dist = transform.position.ToDistance(Point.transform.position) * 0.8f;

            var partsCount = Mathf.FloorToInt(dist);

            if (partsCount < 2)
            {
                partsCount = 2;
            }

            for (int i = 0; i < partsCount; i++)
            {
                var spawnedPart = Instantiate(ropePart, transform);
                spawnedPart.transform.localPosition = Vector3.zero;

                var joint = spawnedPart.GetComponentInChildren<Joint>();

                var last = new Rope(joint);




                if (i >= 1)
                {
                    last.Joint.connectedBody = ropeParts[ropeParts.Count - 1].Rb;
                }

                last.Rb.isKinematic = i == 0;

                ropeParts.Add(last);

            }


            lineRenderer.positionCount = ropeParts.Count;
            ropeParts.Last().Rb.isKinematic = true;


            AfterGeneration();
        }

        public virtual void AfterGeneration()
        {
            
        }


        private void Update()
        {
            if (ropeParts.Count == 0) return;

            for (int i = 0; i < ropeParts.Count; i++)
            {
                lineRenderer.SetPosition(i, ropeParts[i].Rb.transform.position);
            }

            ropeParts.Last().Rb.MovePosition(Point.position);
            ropeParts.First().Rb.transform.position = (endPointPosition.position);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(transform.position, Point.position);
        }

        public void SetTargetPosition(Vector3 fishingTarget)
        {
            Point.transform.position = fishingTarget;
        }

        public void SetFirstChainPointPos(Transform endPointPosition)
        {
            this.endPointPosition = endPointPosition;
        }


        public virtual void RopeBack()
        {
            
        }
    }
}
