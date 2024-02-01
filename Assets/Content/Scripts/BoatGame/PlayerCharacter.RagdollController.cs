using System.Collections.Generic;
using UnityEngine;

namespace Content.Scripts.BoatGame
{
    public partial class PlayerCharacter
    {
        [System.Serializable]
        public class RagdollController
        {
            [SerializeField] private List<Rigidbody> rb;
            [SerializeField] private List<Collider> colliders;
            [SerializeField] private Animator animator;


            public void ActiveRagdoll()
            {
                animator.enabled = false;

                for (int i = 0; i < rb.Count; i++)
                {
                    rb[i].isKinematic = false;
                }

                for (int i = 0; i < colliders.Count; i++)
                {
                    colliders[i].enabled = true;
                }
                
                rb[0].AddForce(Vector3.up * 100, ForceMode.Impulse);
            }

            public void SetUnderWaterRagdoll()
            {
                for (int i = 0; i < colliders.Count; i++)
                {
                    colliders[i].enabled = false;
                }
                for (int i = 0; i < rb.Count; i++)
                {
                    rb[i].drag = 4;
                    rb[i].angularDrag = 2;
                }
            }
        }
    }
}