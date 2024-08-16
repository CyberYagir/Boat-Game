using Content.Scripts.BoatGame.Services;
using Content.Scripts.CraftsSystem;
using Content.Scripts.IslandGame;
using UnityEngine;

namespace Content.Scripts.BoatGame.Characters.States
{
    class CharActionBuildDropItem : CharActionBuilding
    {
        private DroppedItemBase targetDroppedItem;
        private float maxTime;
        private float buildTimer;
        private CraftObject craftObject;
        
        
        public override void ResetState()
        {
            base.ResetState();
            targetDroppedItem = null;
            buildTimer = 0;
        }

        public override void GetTargetAndStart()
        {

            Agent.SetStopped(false);
            
            targetDroppedItem = SelectionService.SelectedObject.Transform.GetComponent<DroppedItemBase>();
            
            if (targetDroppedItem == null)
            {
                EndState();
                return;
            }

            var dropCraft = targetDroppedItem.GetComponent<DropCraft>();
            if (dropCraft == null)
            {
                EndState();
                return;
            }


            craftObject = dropCraft.CraftItem;
            maxTime = craftObject.CraftTime * Machine.Character.GetSkillMultiply(buildingSkill.SkillID);
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
                
                
                for (int i = 0; i < craftObject.FinalItem.Count; i++)
                {
                    Fabric.SpawnItemOnGround(
                            craftObject.FinalItem.ResourceName.GetDropPrefab(Machine.GameData),
                            targetDroppedItem.transform.position + Random.insideUnitSphere / 2f,
                            targetDroppedItem.transform.rotation, null)
                        .With(x => x.Animate())
                        .With(x => x.SetItem(craftObject.FinalItem.ToStorageItem()));

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