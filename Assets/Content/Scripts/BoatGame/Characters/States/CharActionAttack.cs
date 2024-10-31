using Content.Scripts.BoatGame.Services;
using Content.Scripts.IslandGame;
using Content.Scripts.ItemsSystem;
using Content.Scripts.Mobs.Mob;
using UnityEngine;

namespace Content.Scripts.BoatGame.Characters.States
{
    
    public class CharActionAttack : CharActionBase
    {
        private DamageObject attackObject;
        private DamageObject autoBattleAttackObject;
        
        private bool attack;
        private string targetCharacterWeaponID;
        private ItemObject targetCharacterWeapon;
        private Transform characterHips;
        
        public override void ResetState()
        {
            base.ResetState();
            targetCharacterWeaponID = null;
            targetCharacterWeapon = null;
            attack = false;
        }

        public override void StartState()
        {
            base.StartState();
            
            Machine.AIMoveManager.NavMeshAgent.SetStopped(false);
            if (autoBattleAttackObject == null)
            {
                attackObject = SelectionService.SelectedObject.Transform.GetComponent<DamageObject>();
            }
            else
            {
                attackObject = autoBattleAttackObject;
            }

            if (attackObject)
            {
                characterHips = Machine.AppearanceDataManager.GetBone(PlayerCharacter.AppearanceManager.EBones.Hips);
                attackObject.AttackStart();
                MoveToPoint(attackObject.AttackPoint.position);
                if ((attackObject is SpawnedMob k))
                {
                    k.OnDropCreated += TakeDrop;
                    print("add event");
                }
            }
            else
            {
                WorldPopupService.StaticSpawnCantPopup(Machine.transform.position);
                EndState();
            }
        }

        private void TakeDrop(DroppedItemBase droppedItemBase)
        {
            print(Machine.CurrentState);
            if (Machine.CurrentState == EStateType.Idle)
            {
                Machine.GetCharacterAction<CharActionPickup>().SetCachedItem(droppedItemBase);
                Machine.ActiveAction(EStateType.PickupItem);
            }
        }

        public override void ProcessState()
        {
            if (attackObject == null || attackObject.Health <= 0)
            {
                EndState();
                return;
            }

            if (attack)
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
                    attackObject.HeightPoint.position.y < characterHips.position.y);

            }
            else
            {
                MovingToPointLogic();
            }
        }

        protected override void OnMoveEnded()
        {
            if (Vector3.Distance(new Vector3(attackObject.AttackPoint.position.x, transform.position.y, attackObject.AttackPoint.position.z), transform.position) < 0.5f)
            {
                Machine.AnimationManager.ResetAllTriggers();
                attack = true;
                Machine.AnimationManager.AnimationEvents.OnAttack += AttackEnemy;
            }
        }

        public virtual void AttackEnemy()
        {
            attackObject.Damage(Machine.ParametersCalculator.Damage, Machine.gameObject);
        }

        private void RotateToTarget()
        {
            var pos = attackObject.transform.position - attackObject.transform.forward;
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(pos.x, Machine.transform.position.y, pos.z) - Machine.transform.position);
            Machine.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.deltaTime);
        }

        public void SetAutoAttackMob(DamageObject damageObject)
        {
            autoBattleAttackObject = damageObject;
        }

        public override void EndState()
        {
            base.EndState();
            if (attackObject != null)
            {
                if (!attackObject.IsDead)
                {
                    attackObject.AttackStop();
                    if ((attackObject is SpawnedMob k))
                    {
                        k.OnDropCreated -= TakeDrop;
                    }
                }
            }

            autoBattleAttackObject = null;
            Machine.AppearanceDataManager.ActiveMeleeWeapon(false);
            ToIdleAnimation();
            Machine.AnimationManager.AnimationEvents.OnAttack -= AttackEnemy;
        }
    }
}
