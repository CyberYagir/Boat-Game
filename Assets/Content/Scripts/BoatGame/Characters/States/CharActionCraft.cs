using System;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.CraftsSystem;
using Content.Scripts.ItemsSystem;
using Content.Scripts.SkillsSystem;
using UnityEngine;

namespace Content.Scripts.BoatGame.Characters.States
{
    public class CharActionCraft : CharActionBase
    {
        public enum EState
        {
            MoveToTarget,
            Crafting
        }

        [SerializeField] private SkillObject buildingSkill;
        [SerializeField] private GameObject hammerItem;
        
        private EState state;
        private CraftingTable targetCraftingTable;
        private GameObject spawnedHammer;
        private float time;

        public Action OnShowBuildWindow;
        public Action OnCloseBuildWindow;
        public override void ResetState()
        {
            base.ResetState();
            state = EState.MoveToTarget;
            time = 0;
        }

        public override void StartState()
        {
            base.StartState();
            
            if (SelectionService.SelectedObject == null)
            {
                EndState();
                return;
            }
            
            
            targetCraftingTable = SelectionService.SelectedObject.Transform.GetComponent<CraftingTable>();

            if (targetCraftingTable != null && targetCraftingTable.CurrentCraft == null)
            {
                OnShowBuildWindow?.Invoke();
                EndState();
                return;
            }
        }

        public void SetCraft(CraftObject craftItem)
        {
            targetCraftingTable.StartCraft(craftItem, Machine.Character.GetSkillMultiply(buildingSkill.SkillID));
            Agent.SetStopped(false);
            MoveToPoint(targetCraftingTable.transform.position);
            OnCloseBuildWindow?.Invoke();
        }

        public override void ProcessState()
        {
            base.ProcessState();

            switch (state)
            {
                case EState.MoveToTarget:
                    MovingToPointLogic();
                    break;
                case EState.Crafting:
                    CraftingLogic();
                    break;
            }
        }

        private void CraftingLogic()
        {
            if (targetCraftingTable == null)
            {
                EndState();
                return;
            } 
            if (Machine.AIMoveManager.HaveMaterials(targetCraftingTable.CurrentCraft.Ingredients))
            {
                time += TimeService.DeltaTime;

                Machine.transform.LookAt(new Vector3(targetCraftingTable.transform.position.x, Machine.transform.position.y, targetCraftingTable.transform.position.z));

                if (targetCraftingTable.CraftingTimeTime <= time)
                {
                    AddCraftToStorage();
                }
            }
            else
            {
                EndState();
            }
        }

        private void AddCraftToStorage()
        {
            var storage = Machine.AIMoveManager.GoToEmptyStorages(targetCraftingTable.CurrentCraft.FinalItem.ResourceName, 1);
            if (storage.Count != 0)
            {
                for (int i = 0; i < targetCraftingTable.CurrentCraft.Ingredients.Count; i++)
                {
                    for (int j = 0; j < targetCraftingTable.CurrentCraft.Ingredients[i].Count; j++)
                    {
                        Machine.AIMoveManager.RemoveFromAnyStorage(targetCraftingTable.CurrentCraft.Ingredients[i].ResourceName);
                    }
                }

                int finalCount = targetCraftingTable.CurrentCraft.FinalItem.Count;

                for (int i = 0; i < storage.Count; i++)
                {
                    if (finalCount != 0)
                    {
                        while (storage[i].IsEmptyStorage(targetCraftingTable.CurrentCraft.FinalItem.ResourceName, 1) && finalCount > 0)
                        {
                            finalCount--;
                            storage[i].AddToStorage(targetCraftingTable.CurrentCraft.FinalItem.ResourceName, 1);
                        }
                    }
                }


                EndState();
            }
        }

        protected override void OnMoveEnded()
        {
            base.OnMoveEnded();
            Machine.AnimationManager.TriggerCrafting();
            state = EState.Crafting;

            spawnedHammer = Instantiate(hammerItem, Machine.AnimationManager.RightHand);
        }

        public override void EndState()
        {
            base.EndState();
            if (targetCraftingTable != null)
            {
                targetCraftingTable.ClearCraft();
            }

            if (spawnedHammer != null)
            {
                Destroy(spawnedHammer.gameObject);
            }
            
            ToIdleAnimation();
        }
    }
}
