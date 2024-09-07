using System;
using System.Collections;
using Content.Scripts.BoatGame;
using Content.Scripts.Mobs.Mob;
using Content.Scripts.Mobs.MobCrab;
using UnityEngine;

namespace Content.Scripts.DungeonGame.Mobs.States
{
    public class DgBossAttackActionSelector : MonoBehaviour
    {
        private bool isSpecial;
        private DungeonEnemyStateMachine machine;
        private DamageObject damageObject;

        private void Start()
        {
            machine = GetComponent<DungeonEnemyStateMachine>();
            damageObject = GetComponent<DamageObject>();
            machine.OnChangeState += OnOnChangeState;
        }

        private void OnOnChangeState()
        {
            if (machine.CurrentStateType == EMobsState.Attack)
            {
                StartCoroutine(SkipFrame());
            }
        }

        IEnumerator SkipFrame()
        {
            yield return null;
            if (damageObject.IsDead) yield break;

            isSpecial = !isSpecial;
            if (isSpecial)
            {
                machine.StartAction(EMobsState.AttackNormal);
            }
            else
            {
                machine.StartAction(EMobsState.AttackSpecial);
            }
        }
    }
}
