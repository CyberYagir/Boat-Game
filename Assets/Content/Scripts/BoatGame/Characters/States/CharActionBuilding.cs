using System;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.SkillsSystem;
using UnityEngine;

namespace Content.Scripts.BoatGame.Characters.States
{
    public class CharActionBuilding : CharActionBase
    {
        
        public enum EState
        {
            MoveToTarget,
            Building
        }
        
        public Action OnShowBuildWindow;

        [SerializeField] private GameObject hammerItem;
        [SerializeField] private SkillObject buildingSkill;
        private GameObject spawnedHammer;

        private EState state;
        private RaftBuild targetBuildRaft;

        public override void ResetState()
        {
            base.ResetState();
            state = EState.MoveToTarget;
        }

        public override void StartState()
        {
            base.StartState();

            
            if (SelectionService.SelectedObject == null)
            {
                EndState();
                return;
            }
            
            targetBuildRaft = SelectionService.SelectedObject.Transform.GetComponent<RaftBuild>();

            if (targetBuildRaft == null)
            {
                OnShowBuildWindow?.Invoke();
                EndState();
                return;
            }

            targetBuildRaft.SetTime(Machine.Character.GetSkillMultiply(buildingSkill.SkillID));
            MoveToPoint(targetBuildRaft.transform.position);
        }

        public override void ProcessState()
        {
            switch (state)
            {
                case EState.MoveToTarget:
                    MovingToPointLogic();
                    break;
                case EState.Building:
                    BuildLogic();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        public void BuildLogic()
        {
            targetBuildRaft.AddProgress(TimeService.DeltaTime);
            if (targetBuildRaft.Progress >= 100)
            {
                Machine.AddExp(2);
                EndState();
            }
        }

        protected override void OnMoveEnded()
        {
            state = EState.Building;
            spawnedHammer = Instantiate(hammerItem, Machine.AnimationManager.RightHand);
            Machine.AnimationManager.TriggerBuilding();
        }

        public override void EndState()
        {
            base.EndState();

            if (spawnedHammer != null)
            {
                Destroy(spawnedHammer.gameObject);
            }
            
            ToIdleAnimation();
        }
    }
}
