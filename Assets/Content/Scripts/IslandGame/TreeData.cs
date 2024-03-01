using Content.Scripts.BoatGame.Characters;
using DG.DemiLib;
using UnityEngine;

namespace Content.Scripts.IslandGame
{
    public class TreeData : MonoBehaviour
    {
        [SerializeField] private CollisionSO collisionObject;
        [SerializeField] private EStateType action;
        [SerializeField] private DG.DemiLib.Range healthRange;
        public EStateType Action => action;

        public Range HealthRange => healthRange;

        public void SetCollisionAsset(CollisionSO collisionObject)
        {
            this.collisionObject = collisionObject;
        }

        public void LoadCollider(CapsuleCollider capsuleCollider)
        {
            collisionObject.LoadCollider(capsuleCollider, transform.localScale.magnitude/2f);
        }
    }
}
