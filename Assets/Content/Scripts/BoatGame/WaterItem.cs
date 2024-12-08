using System.Collections;
using Content.Scripts.BoatGame.PlayerActions;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Boot;
using Content.Scripts.Global;
using Content.Scripts.ItemsSystem;
using DG.Tweening;
using Sirenix.OdinInspector;
using StylizedWater2;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
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
        private int maxDistance;
        private Collider collider;
        public DropData Drop => dropData;

        public bool IsStopped => isStopped;

        public bool IsOnDeath => isOnDeath;

        private bool isStaticInited = false;

        [SerializeField, ReadOnly] private bool isBeVisible;
        private Camera camera;


        public void Init(Vector3 dir, int maxDistance, float itemSpeed, Camera camera)
        {
            this.camera = camera;
            this.maxDistance = maxDistance;
            startVelocity = dir.normalized * itemSpeed;
            rb.AddForce(startVelocity, ForceMode.VelocityChange);
            floatingTransform = GetComponent<FloatingTransform>();
            collider = GetComponent<Collider>();
            StartCoroutine(Loop());
        }

        IEnumerator Loop()
        {
            float notVisibleTimer = 0;
            while (transform.position.magnitude < maxDistance)
            {
                yield return null;
                
                
                if (!isBeVisible)
                {
                    if (Extensions.ObjectIsVisible(camera, transform.position, out var n, 0))
                    {
                        isBeVisible = true;
                    }
                    else
                    {
                        notVisibleTimer += Time.deltaTime;
                        if (notVisibleTimer >= 15)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    if (!Extensions.ObjectIsVisible(camera, transform.position, out var n, -0.05f))
                    {
                        break;
                    }
                }
                
                
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
            
            collider.enabled = false;
            
            
            if (isStaticInited) return;
            floatingTransform.enabled = false;
            isStopped = true;
            rb.isKinematic = true;
           
        }

        public void EnableItem()
        {

            collider.enabled = true;
            if (isStaticInited) return;
            floatingTransform.enabled = true;
            rb.isKinematic = false;
            isStopped = false;
            rb.velocity = startVelocity;
        }

        public void InitStaticItem()
        {
            isStaticInited = true;
            floatingTransform = GetComponent<FloatingTransform>();
            floatingTransform.enabled = false;
            rig.SetYLocalEulerAngles(Random.Range(0, 360));
            var scale = rig.transform.localScale;
            rig.transform.localScale = Vector3.zero;
            rig.transform.DOScale(scale, 0.25f);
            collider = GetComponent<Collider>();
            
            
            rb.isKinematic = true;
        }

        public void CheckIsVisible()
        {
            
        }
    }
}
