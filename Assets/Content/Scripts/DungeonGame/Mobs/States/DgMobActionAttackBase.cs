using System;
using UnityEngine;

namespace Content.Scripts.DungeonGame.Mobs.States
{
    public class DgMobActionAttackBase : DgMobActionBase
    {
        [SerializeField] protected float attackDamage;
        [SerializeField] protected float attackDistance;
        [SerializeField] protected float attackCooldown;
        [SerializeField] protected float attackDelay;
        protected bool isCooldown;


        private void Awake()
        {
            attackDamage *= Machine.GetDamageModify();
        }
    }
}