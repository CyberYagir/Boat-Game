using DG.Tweening;
using UnityEngine;

namespace Content.Scripts.ManCreator
{
    public class ManCreatorInputService : MonoBehaviour
    {
        private static readonly int HitTrigger = Animator.StringToHash("HitTrigger");
        private static readonly int HitHeadTrigger = Animator.StringToHash("HitHeadTrigger");
        private static readonly int KnokoutTrigger = Animator.StringToHash("KnokoutTrigger");
        
        
        [SerializeField] private Camera camera;
        [SerializeField] private GameObject particle;
        [SerializeField] private int maxHitsCount = 200;
        
        private int count;
        
        private float timer;


        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && count < maxHitsCount)
            {
                var hit = camera.MouseRaycast(out bool isHit, Input.mousePosition);

                if (isHit)
                {
                    var animator = hit.collider.GetComponentInChildren<Animator>();

                    if (animator == null) return;
                    
                    if (hit.collider is CapsuleCollider)
                    {
                        animator.ResetTrigger(HitTrigger);
                        animator.SetTrigger(HitTrigger);
                        count++;
                    }
                    else
                    {
                        animator.ResetTrigger(HitHeadTrigger);
                        animator.SetTrigger(HitHeadTrigger);
                        count += 5;
                    }

                    if (count >= maxHitsCount)
                    {
                        animator.ResetTrigger(KnokoutTrigger);
                        animator.SetTrigger(KnokoutTrigger);

                        DOVirtual.DelayedCall(5, delegate
                        {
                            count = 0;
                        });
                    }

                    var p = Instantiate(particle, hit.point, Quaternion.identity);

                    Destroy(p.gameObject, 3f);
                }
            }
        }
    }
}
