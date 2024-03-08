using Content.Scripts.BoatGame.Services;
using Content.Scripts.IslandGame;
using UnityEngine;

namespace Content.Scripts.BoatGame.Characters.States
{
    class CharActionBuildDropItem : CharActionBuilding
    {
        private DroppedItem targetDroppedItem;
        private float maxTime;
        private float buildTimer;
        public override void ResetState()
        {
            base.ResetState();
            targetDroppedItem = null;
            buildTimer = 0;
        }

        public override void GetTargetAndStart()
        {

            Agent.SetStopped(false);
            
            targetDroppedItem = SelectionService.SelectedObject.Transform.GetComponent<DroppedItem>();

            
            if (targetDroppedItem == null)
            {
                EndState();
                return;
            }

            maxTime = targetDroppedItem.CraftItem.CraftTime * Machine.Character.GetSkillMultiply(buildingSkill.SkillID);
            targetDroppedItem.SetKinematic();
            if (!MoveToPoint(targetDroppedItem.transform.position))
            {
                EndState();
            }
        }


        public override void BuildLogic()
        {
            if (targetDroppedItem != null)
            {
                buildTimer += TimeService.DeltaTime;

                if (buildTimer < maxTime) return;
                
                
                for (int i = 0; i < targetDroppedItem.CraftItem.FinalItem.Count; i++)
                {
                    Fabric.SpawnItemOnGround(
                            targetDroppedItem.CraftItem.FinalItem.ResourceName.DropPrefab,
                            targetDroppedItem.transform.position + Random.insideUnitSphere / 2f,
                            targetDroppedItem.transform.rotation, null)
                        .With(x => x.Animate());

                }
                    
                targetDroppedItem.DeleteItem();
                    
                EndState();
                return;
            }
            
            EndState();
        }

        public override void EndState()
        {
            base.EndState();

            if (targetDroppedItem != null)
            {
                targetDroppedItem.SetKinematic(false);
            }
        }
    }
}