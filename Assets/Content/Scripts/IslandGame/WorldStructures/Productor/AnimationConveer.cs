using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Content.Scripts.IslandGame.WorldStructures.Productor
{
    public class AnimationConveer : MonoBehaviour
    {
        [SerializeField] private ConveerItem item;

        [SerializeField] private Transform[] points;
        [SerializeField] private float delay;
        [SerializeField] private float wayTime;


        private List<ConveerItem> spawned = new List<ConveerItem>(20);

        private Vector3[] positions;

        private void Start()
        {
            positions = new Vector3[points.Length];

            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] = points[i].position;
            }

            StartCoroutine(Loop());
        }


        IEnumerator Loop()
        {
            while (true)
            {
                Instantiate(item, item.transform.parent)
                    .With(x =>
                        x.transform.DOPath(positions, wayTime, PathType.Linear).SetEase(Ease.Linear).onComplete +=
                            delegate
                            {
                                x.Activate();
                                DOVirtual.DelayedCall(2, delegate
                                {
                                    spawned.Remove(x);
                                    Destroy(x.gameObject);
                                });
                            })
                    .With(x => x.gameObject.SetActive(true))
                    .With(x => spawned.Add(x));




                yield return new WaitForSeconds(delay);
            }
        }

        public void ActivateItems(BoxCollider boxCollider)
        {
            foreach (var t in spawned)
            {
                if (boxCollider.bounds.Contains(t.transform.position))
                {
                    t.UpgradeLevel();
                }
            }
        }
    }
}
