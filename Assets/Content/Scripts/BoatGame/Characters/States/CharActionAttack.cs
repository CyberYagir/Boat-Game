using Content.Scripts.ItemsSystem;
using UnityEngine;

namespace Content.Scripts.BoatGame.Characters.States
{
    
    public class CharActionAttack : CharActionBase
    {
        private DamageObject attackObject;
        
        private bool attack;
        private string targetCharacterWeaponID;
        private ItemObject targetCharacterWeapon;
        private Transform characterHips;

        [SerializeField] private float baseDamage = 15;
        
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
            attackObject = SelectionService.SelectedObject.Transform.GetComponent<DamageObject>();
            if (attackObject)
            {
                characterHips = Machine.AppearanceDataManager.GetBone(PlayerCharacter.AppearanceManager.EBones.Hips);
                attackObject.AttackStart();
                MoveToPoint(attackObject.AttackPoint.position);
            }
            else
            {
                EndState();
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
                    attackObject.transform.position.y < characterHips.position.y);

            }
            else
            {
                MovingToPointLogic();
            }
        }

        protected override void OnMoveEnded()
        {
            if (Vector3.Distance(attackObject.AttackPoint.position, transform.position) < 0.5f)
            {
                Machine.AnimationManager.ResetAllTriggers();
                attack = true;
                Machine.AnimationManager.AnimationEvents.OnAttack += AttackEnemy;
            }
        }

        private void AttackEnemy()
        {
            if (targetCharacterWeapon != null)
            {
                attackObject.Damage(targetCharacterWeapon.ParametersData.Damage);
            }
            else
            {
                attackObject.Damage(baseDamage);
            }
        }

        private void RotateToTarget()
        {
            var pos = attackObject.transform.position - attackObject.transform.forward;
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(pos.x, Machine.transform.position.y, pos.z) - Machine.transform.position);
            Machine.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.deltaTime);
        }

        public override void EndState()
        {
            base.EndState();
            if (attackObject != null)
            {
                if (!attackObject.IsDead)
                {
                    attackObject.AttackStop();
                }
            }
            Machine.AppearanceDataManager.ActiveMeleeWeapon(false);
            ToIdleAnimation();
            Machine.AnimationManager.AnimationEvents.OnAttack -= AttackEnemy;
        }
    }
}
