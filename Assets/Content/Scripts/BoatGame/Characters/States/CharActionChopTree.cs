using Content.Scripts.IslandGame;
using Content.Scripts.SkillsSystem;
using DG.Tweening;
using Packs.YagirConsole.ShellScripts.Base.Shell;
using UnityEngine;

namespace Content.Scripts.BoatGame.Characters.States
{
    public class CharActionChopTree : CharActionBase
    {
        [SerializeField] private GameObject axe;
        [SerializeField] private float damage;
        [SerializeField] private SkillObject damageSkill;
        
        private GameObject spawnedAxe;
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

            Agent.isStopped = false;
            selectedTree = SelectionService.SelectedObject.Transform.GetComponent<TerrainObject>();

            
            if (selectedTree == null)
            {
                EndState();
                return;
            }

            if (!MoveToPoint(selectedTree.transform.position))
            {
                EndState();
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
            Agent.isStopped = true;
            spawnedAxe = Instantiate(axe, Machine.AnimationManager.RightHand);
            Machine.AnimationManager.TriggerChopTree();
            isMoveEnded = true;
            Machine.transform.DOLookAt(new Vector3(selectedTree.transform.position.x, Machine.transform.position.y, selectedTree.transform.position.z), 0.2f);
            Machine.AnimationManager.AnimationEvents.OnChop += Damage;
        }

        private void Damage()
        {
            if (selectedTree != null)
            {
                selectedTree.Damage(damage * Machine.Character.GetSkillMultiply(damageSkill.SkillID), Machine.transform);

                if (selectedTree.IsDead)
                {
                    EndState();
                }
                return;
            }
            
            EndState();
        }


        public override void EndState()
        {
            base.EndState();
            
            ToIdleAnimation();

            if (spawnedAxe != null)
            {
                Destroy(spawnedAxe.gameObject);
            }
            
            
            Machine.AnimationManager.AnimationEvents.OnChop -= Damage;
        }
    }
}
