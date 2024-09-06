using System.Collections;
using Content.Scripts.BoatGame.Characters;
using Content.Scripts.DungeonGame.BossAttacks;
using DG.Tweening;
using UnityEngine;

namespace Content.Scripts.DungeonGame.Mobs.States
{
    public class DgBossActionAttack : StateAction<DungeonMob>
    {
        [SerializeField] private BossAttackBase[] attacksPrefabs;
        private int attackID;


        public override void StartState()
        {
            base.StartState();
            
            var attack = Instantiate(attacksPrefabs[attackID], transform.position, transform.rotation);
            attack.Init(Machine);
            attackID++;
            if (attacksPrefabs.Length <= attackID)
            {
                attackID = 0;
            }
            
            attack.OnEnded += AttackOnOnEnded;
        }

        private void AttackOnOnEnded()
        {
            EndState();
        }

        public override void EndState()
        {
            base.EndState();

            Machine.MobAnimator.ResetTriggers();
            StartCoroutine(SkipFrame());
        }

        IEnumerator SkipFrame()
        {
            yield return null;
            Machine.UnAgr();
        }
    }
}
