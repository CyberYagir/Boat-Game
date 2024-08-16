using Content.Scripts.BoatGame.Characters;
using Content.Scripts.ItemsSystem;
using DG.DemiLib;
using UnityEngine;

namespace Content.Scripts.IslandGame
{
    public class TreeData : MonoBehaviour
    {
        [SerializeField] private CollisionSO collisionObject;
        [SerializeField] private EStateType action;
        [SerializeField] private Range healthRange;
        [SerializeField] private ItemObject dropItem;
        [SerializeField] private bool withFallAnimation;
        public EStateType Action => action;

        public Range HealthRange => healthRange;

        public ItemObject DropItem => dropItem;

        public bool WithFallAnimation => withFallAnimation;

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
