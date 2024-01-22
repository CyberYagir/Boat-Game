using System.Collections;
using DG.Tweening;
using StylizedWater2;
using UnityEngine;

namespace Content.Scripts.BoatGame.Ropes
{
    class RopeGeneratorFishing : RopeGenerator
    {
        [SerializeField] private Transform floatTransform;

        public override void AfterGeneration()
        {
            var fishingTarget = Point.position;
            var floating = Point.GetComponent<FloatingTransform>();
            floating.enabled = false;
            Point.transform.localPosition = Vector3.zero;

            floatTransform.LookAt(new Vector3(fishingTarget.x, Point.transform.position.y, fishingTarget.z));

            Point.DOMove(fishingTarget, 0.5f)
                .SetLink(gameObject)
                .onComplete += delegate
            {
                floating.enabled = true; 
                OnRopeAnimationEnded?.Invoke();
            };
        }


        public override void RopeBack()
        {
            var time = Vector3.Distance(transform.position, point.transform.position) * 0.5f;
            point.
                DOMove(
                    new Vector3(transform.position.x, point.position.y, transform.position.z), 
                    time)
                .SetUpdate(UpdateType.Fixed)
                .SetLink(gameObject)
                .onComplete += delegate { OnRopeEnded?.Invoke(); };

            StartCoroutine(BackRopeRemoveChains(time));
        }


        IEnumerator BackRopeRemoveChains(float time)
        {
            var count = ropeParts.Count;
            for (int i = 0; i < count - 3; i++)
            {
                yield return time / count;

                Destroy(ropeParts[1].Rb.gameObject);
                ropeParts.RemoveAt(1);
                ropeParts[1].Joint.connectedBody = ropeParts[0].Rb;
                lineRenderer.positionCount--;
            }
        }
    }
}