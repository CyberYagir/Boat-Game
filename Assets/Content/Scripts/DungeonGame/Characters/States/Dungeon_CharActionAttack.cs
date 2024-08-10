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
        }

        public override void StartState()
        {
            base.StartState();
            characterHips = Machine.AppearanceDataManager.GetBone(PlayerCharacter.AppearanceManager.EBones.Hips);
            Agent.SetStopped(false);

            if (DungeonCharacter.TargetEnemy != null)
            {
                Machine.AnimationManager.AnimationEvents.OnAttack += AttackEnemy;
            }
        }

        private void AttackEnemy()
        {
            DungeonCharacter.TargetEnemy.Damage(Machine.ParametersCalculator.Damage, Machine.gameObject);
            if (DungeonCharacter.TargetEnemy.IsDead)
            {
                EndState();
            }
        }

        public override void ProcessState()
        {
            base.ProcessState();

            if (DungeonCharacter.TargetEnemy != null)
            {
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
                EndState();
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
            
            Machine.AnimationManager.AnimationEvents.OnAttack -= AttackEnemy;
            Machine.AppearanceDataManager.ActiveMeleeWeapon(false);
            ToIdleAnimation();
        }
    }
}
