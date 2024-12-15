using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Content.Scripts.IslandGame.WorldStructures.Productor
{
    public class AnimationGears : MonoBehaviour
    {
        [SerializeField] private Transform rotate;
        [SerializeField] private bool setSpeed;
        [SerializeField, ShowIf("@setSpeed")] private float rotateTime;
        private void Awake()
        {
            if (!setSpeed)
            {
                rotateTime = Random.Range(2, 5);
            }
            
            rotate.transform.DOLocalRotate(new Vector3(0,0,360), rotateTime,
                RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
        }
    }
}
