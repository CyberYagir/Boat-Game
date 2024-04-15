using System.Collections.Generic;
using Content.Scripts.BoatGame.Characters;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.CraftsSystem;
using Content.Scripts.ItemsSystem;
using UnityEngine;
using UnityEngine.AI;

namespace Content.Scripts.BoatGame
{
    public partial class PlayerCharacter
    {
        [System.Serializable]
        public class AIManager
        {
            [SerializeField] private MonoBehaviour navMeshAgent;
            private RaftBuildService raftBuildService;

            private INavAgentProvider agent;
            private INavMeshProvider navMeshProvider;
            
            public INavAgentProvider NavMeshAgent => agent;
            public INavMeshProvider NavMesh => navMeshProvider;

            public void Init(RaftBuildService raftBuildService, INavMeshProvider navMeshProvider)
            {
                this.navMeshProvider = navMeshProvider;
                this.raftBuildService = raftBuildService;
                agent = navMeshAgent.GetComponent<INavAgentProvider>();
            }
            
            public Vector3 WalkToAnyPoint()
            {
                NavMeshAgent.SetStopped(false);
                return GenerateRandomPos();
            }

            public Vector3 GenerateRandomPos()
            {
                var targetRaft = raftBuildService.SpawnedRafts.FindAll(x=>
                    x.RaftType != RaftBuildService.RaftItem.ERaftType.Building && 
                    x.RaftType != RaftBuildService.RaftItem.ERaftType.CraftTable && 
                    x.RaftType != RaftBuildService.RaftItem.ERaftType.Fishing
                ).GetRandomItem();
                return targetRaft.transform.position + new Vector3(GetRandomOffset(), 0, GetRandomOffset());
            }

            private float GetRandomOffset()
            {
                return Random.Range(-0.35f, 0.35f);
            }

            private RaycastHit[] raycastResults = new RaycastHit[4];

            public bool IsOnGround()
            {
                var size = Physics.RaycastNonAlloc(NavMeshAgent.Transform.position + Vector3.up, -NavMeshAgent.Transform.up, raycastResults, 20, LayerMask.GetMask("Raft", "Default", "Terrain"), QueryTriggerInteraction.Ignore);

                for (int i = 0; i < size; i++)
                {
                    if (raycastResults[i].transform.GetComponent<RaftBase>() || raycastResults[i].transform.GetComponent<Terrain>())
                    {
                        return true;
                    }
                }

                return false;
            }

            public void ExtraRotation()
            {
                NavMeshAgent.ExtraRotation();
            }

            public RaftStorage GoToEmptyStorage(ItemObject item, int value)
            {
                return raftBuildService.FindEmptyStorage(value);
            }
            
            public List<RaftStorage> GoToEmptyStorages(ItemObject item, int value)
            {
                return raftBuildService.FindEmptyStorages(item, value);
            }

            public RaftStorage FindResource(EResourceTypes type)
            {
                return raftBuildService.FindResourceInStorages(type);
            }
            
            public bool HaveMaterials(List<CraftObject.CraftItem> currentCraftIngredients)
            {
                foreach (var currentCraftIngredient in currentCraftIngredients)
                {
                    int count = 0;
                    for (int i = 0; i < raftBuildService.Storages.Count; i++)
                    {
                        var storage = raftBuildService.Storages[i].GetItem(currentCraftIngredient.ResourceName.Type);
                        if (storage != null)
                        {
                            var item = storage.Find(x => x.Item.ID == currentCraftIngredient.ResourceName.ID);
                            if (item != null)
                            {
                                count += item.Count;
                                if (count >= currentCraftIngredient.Count)
                                {
                                    break;
                                }
                            }
                        }
                    }

                    if (count < currentCraftIngredient.Count)
                    {
                        return false;
                    }
                }

                return true;
            }

            public void RemoveFromAnyStorage(ItemObject resourceName)
            {
                foreach (var raftStorage in raftBuildService.Storages)
                {
                    if (!raftStorage.HaveItem(resourceName)) continue;
                    raftStorage.RemoveFromStorage(resourceName);
                    return;
                }
            }
            
        }
    }
}