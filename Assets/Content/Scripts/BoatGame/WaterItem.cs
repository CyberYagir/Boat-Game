using System.Collections;
using Content.Scripts.BoatGame.PlayerActions;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Content.Scripts.ItemsSystem;
using DG.Tweening;
using StylizedWater2;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Content.Scripts.BoatGame
{
    public class WaterItem : MonoBehaviour
    {
        [System.Serializable]
        public class DropData
        {
            [SerializeField] private ItemObject item;
            [SerializeField] private int value;

            public int Value => value;

            public ItemObject Item => item;
        }
        
        [SerializeField] private Rigidbody rb;
        [SerializeField] private Transform rig;
        [SerializeField] private DropData dropData;
        
        private float stayTimer;
        private Vector3 startVelocity;
        private bool isStopped;
        private bool isOnDeath;
        private FloatingTransform floatingTransform;

        public DropData Drop => dropData;

        public bool IsStopped => isStopped;

        public bool IsOnDeath => isOnDeath;


        public void Init(Vector3 dir, SelectionService selectionService, GameDataObject gameDataObject)
        {
            startVelocity = dir.normalized * Random.Range(0.5f, 2f);
            rb.AddForce(startVelocity, ForceMode.VelocityChange);
            rig.SetYLocalEulerAngles(Random.Range(0, 360));
            GetComponent<ActionsHolder>().Construct(selectionService, gameDataObject);
            floatingTransform = GetComponent<FloatingTransform>();
            StartCoroutine(Loop());
        }

        IEnumerator Loop()
        {
            while (transform.position.magnitude < 40)
            {
                yield return null;

                if (!IsStopped)
                {
                    if (rb.velocity.magnitude <= 0.2f)
                    {
                        stayTimer += TimeService.DeltaTime;

                        if (stayTimer > 10)
                        {
                            floatingTransform.enabled = false;
                            rb.isKinematic = true;

                            transform.DOMoveY(-10f, 4f);
                            transform.DORotate(transform.eulerAngles + rig.right * 90, 1f);
                            isOnDeath = true;

                            yield return new WaitForSeconds(4f);
                            break;
                        }
                    }
                    else
                    {
                        stayTimer = 0;
                    }
                }
            }
            Destroy(gameObject);
        }

        public void DisableItem()
        {
            isStopped = true;
            rb.isKinematic = true;
            floatingTransform.enabled = false;
        }

        public void EnableItem()
        {
            isStopped = false;
            rb.isKinematic = false;
            floatingTransform.enabled = true;
            rb.velocity = startVelocity;
        }
    }
}
