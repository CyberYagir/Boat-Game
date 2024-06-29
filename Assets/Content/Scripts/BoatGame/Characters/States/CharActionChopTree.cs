using Content.Scripts.IslandGame;
using Content.Scripts.SkillsSystem;
using DG.Tweening;
using Packs.YagirConsole.ShellScripts.Base.Shell;
using UnityEngine;

namespace Content.Scripts.BoatGame.Characters.States
{
    public class CharActionChopTree : CharActionBase
    {
        [SerializeField] private GameObject handTool;
        [SerializeField] private float damage;
        [SerializeField] private SkillObject damageSkill;
        
        protected GameObject spawnedAxe;
        private bool isMoveEnded;

        private TerrainObject selectedTree;
        
        
        public override void ResetState()
        {
            isMoveEnded = false;
            selectedTree = null;
        }

        public override void StartState()
        {
            base.StartState();

            Agent.SetStopped(false);
            selectedTree = SelectionService.SelectedObject.Transform.GetComponent<TerrainObject>();
            
            
            if (selectedTree == null)
            {
                EndState();
                return;
            }

            selectedTree.OnSpawnDrop += GoToDrop;
            if (!MoveToPoint(selectedTree.transform.position))
            {
                EndState();
            }
        }

        private void GoToDrop(DroppedItem obj)
        {
            if (Machine.CurrentState == EStateType.Idle)
            {
                Machine.GetCharacterAction<CharActionPickup>().SetCachedItem(obj);
                Machine.ActiveAction(EStateType.PickupItem);
            }
        }

        public override void ProcessState()
        {
            if (!isMoveEnded)
            {
                MovingToPointLogic();
            }
        }

        protected override void OnMoveEnded()
        {
            Agent.SetStopped(true);
            spawnedAxe = Instantiate(handTool, Machine.AnimationManager.RightHand);
            StartAnimation();
            isMoveEnded = true;
            Machine.transform.DOLookAt(new Vector3(selectedTree.transform.position.x, Machine.transform.position.y, selectedTree.transform.position.z), 0.2f);
            AddAnimationEvent();
        }

        protected virtual void AddAnimationEvent()
        {
            Machine.AnimationManager.AnimationEvents.OnChop += Damage;
        }
        protected virtual void RemoveAnimationEvent()
        {
            Machine.AnimationManager.AnimationEvents.OnChop -= Damage;
        }
        
        protected virtual void StartAnimation()
        {
            Machine.AnimationManager.TriggerChopTree();
        }
        
   
        private void Damage()
        {
            if (selectedTree != null)
            {
                selectedTree.Damage(damage * Machine.Character.GetSkillMultiply(damageSkill.SkillID), Machine.transform);
                OnDamage();
                if (selectedTree.IsDead)
                {
                    EndState();
                }
                return;
            }
            
            EndState();
        }

        protected virtual void OnDamage()
        {
            
        }


        public override void EndState()
        {
            base.EndState();
            
            ToIdleAnimation();

            // if (selectedTree != null)
            // {
            //     selectedTree.OnSpawnDrop -= GoToDrop;
            // }

            if (spawnedAxe != null)
            {
                Destroy(spawnedAxe.gameObject);
            }
            RemoveAnimationEvent();
        }


    }
}
