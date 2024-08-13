using System;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.Characters.States;
using Content.Scripts.ItemsSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Content.Scripts.DungeonGame.Characters.States
{
    public class Dungeon_CharActionAttack : Dungeon_CharActionBase
    {
        private string targetCharacterWeaponID;
        private ItemObject targetCharacterWeapon;
        private Transform characterHips;

        private void Reset()
        {
            targetCharacterWeaponID = null;
            targetCharacterWeapon = null;
            isMove = false;
        }

        public override void StartState()
        {
            base.StartState();
            characterHips = Machine.AppearanceDataManager.GetBone(PlayerCharacter.AppearanceManager.EBones.Hips);
            Agent.SetStopped(false);
            DungeonCharacter.PlayerCharacter.AnimationManager.HideTorch();
            if (DungeonCharacter.TargetEnemy != null)
            {
                Machine.AnimationManager.AnimationEvents.OnAttack += AttackEnemy;
              
            }
            else
            {
                EndState();
            }

        }

        private void AttackEnemy()
        {
            var allMobs = DungeonCharacter.GetAllEnemiesNear(DungeonCharacter.TargetEnemy.transform.position);

            foreach (var mob in allMobs)
            {
                mob.Damage(Machine.ParametersCalculator.Damage, Machine.gameObject);
            }
            
            if (DungeonCharacter.TargetEnemy.IsDead)
            {
                var notDeadMob = allMobs.Find(x => x.IsDead == false);
                if (!notDeadMob)
                {
                    EndState();
                }
                else
                {
                    DungeonCharacter.SetTarget(notDeadMob);
                }
            }
        }

        bool isMove = false;
        public override void ProcessState()
        {
            base.ProcessState();

            if (DungeonCharacter.TargetEnemy != null && DungeonCharacter.TargetEnemy.transform.position.ToDistance(transform.position) < DungeonCharacter.AttackRange * 1.5f)
            {
                isMove = false;
                RotateToTarget();
                
                if (targetCharacterWeaponID != Machine.Character.Equipment.WeaponID)
                {
                    targetCharacterWeapon = Machine.GetEquipmentWeapon();
                    targetCharacterWeaponID = targetCharacterWeapon != null ? targetCharacterWeapon.ID : "";
                }

                Machine.AppearanceDataManager.ActiveMeleeWeapon(true);

                Machine.AnimationManager.SetAttackTarget(
                    targetCharacterWeapon != null ? targetCharacterWeapon.AnimationType : EWeaponAnimationType.None,
                    DungeonCharacter.TargetEnemy.HeightPoint.position.y < characterHips.position.y);
            }
            else
            {
                if (DungeonCharacter.TargetEnemy.transform.ToDistance(transform) > DungeonCharacter.AttackRange && !isMove)
                {
                    MoveToPoint(DungeonCharacter.TargetEnemy.transform.position + Random.insideUnitSphere * 1.5f);
                    ToIdleAnimation();
                    isMove = true;
                }
                else
                {
                    EndState();
                }
            }
        }


        private void RotateToTarget()
        {
            var pos = DungeonCharacter.TargetEnemy.transform.position - DungeonCharacter.TargetEnemy.transform.forward;
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(pos.x, Machine.transform.position.y, pos.z) - Machine.transform.position);
            Machine.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 30 * Time.deltaTime);
        }

        public override void EndState()
        {
            base.EndState();
            
            DungeonCharacter.PlayerCharacter.AnimationManager.ShowTorch();
            Machine.AnimationManager.AnimationEvents.OnAttack -= AttackEnemy;
            Machine.AppearanceDataManager.ActiveMeleeWeapon(false);
            ToIdleAnimation();
        }
    }
}
