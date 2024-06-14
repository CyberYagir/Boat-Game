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

            private readonly List<RaftBuildService.RaftItem.ERaftType> notWalkableRafts = new()
            {
                RaftBuildService.RaftItem.ERaftType.Building,
                RaftBuildService.RaftItem.ERaftType.CraftTable,
                RaftBuildService.RaftItem.ERaftType.Fishing,
                RaftBuildService.RaftItem.ERaftType.Furnace
            };

            public Vector3 GenerateRandomPos()
            {
                var targetRaft = raftBuildService.SpawnedRafts.FindAll(x=>!notWalkableRafts.Contains(x.RaftType)).GetRandomItem();
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

            public RaftStorage GoToEmptyStorage(int value, bool throwIsFull = true)
            {
                var emptyStorage = raftBuildService.FindEmptyStorage(value);

                if (emptyStorage == null && throwIsFull)
                {
                    SpawnFullPopup();
                }

                return emptyStorage;
            }

            private void SpawnFullPopup()
            {
                if (raftBuildService.SpawnedRafts.Count != 0)
                {
                    WorldPopupService.StaticSpawnPopup(raftBuildService.SpawnedRafts.GetRandomItem().transform.position, "FULL");
                }
            }

            public List<RaftStorage> GoToEmptyStorages(int value = 1)
            {
                var storages = raftBuildService.FindEmptyStorages(value);
                if (storages.Count == 0)
                {
                    SpawnFullPopup();
                }
                return storages;
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