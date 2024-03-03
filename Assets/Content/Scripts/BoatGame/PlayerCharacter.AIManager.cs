using System.Collections.Generic;
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
            [SerializeField] private NavMeshAgent navMeshAgent;
            private RaftBuildService raftBuildService;

            public NavMeshAgent NavMeshAgent => navMeshAgent;

            public void Init(RaftBuildService raftBuildService)
            {
                this.raftBuildService = raftBuildService;
            }
            
            public Vector3 WalkToAnyPoint()
            {
                NavMeshAgent.isStopped = false;
                return GenerateRandomPos();
            }

            public Vector3 GenerateRandomPos()
            {
                var targetRaft = raftBuildService.SpawnedRafts.FindAll(x=>x.RaftType != RaftBuildService.RaftItem.ERaftType.Building && x.RaftType != RaftBuildService.RaftItem.ERaftType.CraftTable).GetRandomItem();
                return targetRaft.transform.position + new Vector3(GetRandomOffcet(), 0, GetRandomOffcet());
            }

            private float GetRandomOffcet()
            {
                return Random.Range(-0.35f, 0.35f);
            }

            private RaycastHit[] raycastResults = new RaycastHit[4];
            public bool IsOnGround()
            {
                var size = Physics.RaycastNonAlloc(NavMeshAgent.transform.position + Vector3.up, -NavMeshAgent.transform.up, raycastResults, 20, LayerMask.GetMask("Raft", "Default"), QueryTriggerInteraction.Ignore);

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
                if (NavMeshAgent.enabled && NavMeshAgent.isStopped && NavMeshAgent.isOnNavMesh) return;
                
                Vector3 lookrotation = navMeshAgent.steeringTarget - navMeshAgent.transform.position;
                if (lookrotation != Vector3.zero)
                {
                    navMeshAgent.transform.rotation = Quaternion.Slerp(navMeshAgent.transform.rotation, Quaternion.LookRotation(lookrotation), navMeshAgent.angularSpeed * Time.deltaTime);
                }
            }

            public RaftStorage GoToEmptyStorage(ItemObject item, int value)
            {
                return raftBuildService.FindEmptyStorage(item, value);
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
                        var storage = raftBuildService.Storages[i].GetStorage(currentCraftIngredient.ResourceName.Type, true);
                        if (storage != null)
                        {
                            var item = storage.ItemObjects.Find(x => x.Item.ID == currentCraftIngredient.ResourceName.ID);
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