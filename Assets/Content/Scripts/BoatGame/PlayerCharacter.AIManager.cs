using System.Collections.Generic;
using Content.Scripts.BoatGame.Characters;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.CraftsSystem;
using Content.Scripts.ItemsSystem;
using Content.Scripts.SkillsSystem;
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
            [SerializeField] private SkillObject moveSkill;
            private RaftBuildService raftBuildService;
            private ResourcesService resourcesService;

            private INavAgentProvider agent;
            private INavMeshProvider navMeshProvider;
            
            public INavAgentProvider NavMeshAgent => agent;
            public INavMeshProvider NavMesh => navMeshProvider;

            public void Init(RaftBuildService raftBuildService, INavMeshProvider navMeshProvider, Character character, CharacterParameters parameters)
            {
                this.parameters = parameters;
                this.character = character;
                this.navMeshProvider = navMeshProvider;
                this.raftBuildService = raftBuildService;
                this.resourcesService = raftBuildService.ResourcesService;
                agent = navMeshAgent.GetComponent<INavAgentProvider>();
                agent.SetMovingSpeed(parameters.Speed);
                character.OnSkillUpgraded += CharacterOnOnSkillUpgraded;
                CharacterOnOnSkillUpgraded();
            }
            
            public void Init(INavMeshProvider navMeshProvider, Character character, CharacterParameters parameters)
            {
                this.parameters = parameters;
                this.character = character;
                this.navMeshProvider = navMeshProvider;
                agent = navMeshAgent.GetComponent<INavAgentProvider>();
                agent.SetMovingSpeed(parameters.Speed);
                character.OnSkillUpgraded += CharacterOnOnSkillUpgraded;
                CharacterOnOnSkillUpgraded();
            }

            private void CharacterOnOnSkillUpgraded()
            {
                agent.SetMovingSpeed(parameters.Speed);
            }


            public Vector3 WalkToAnyPoint()
            {
                NavMeshAgent.SetStopped(false);
                return GenerateRandomPos();
            }



            public Vector3 GenerateRandomPos()
            {
                var targetRaft = raftBuildService.GetRandomWalkableRaft();
                return targetRaft.transform.position + new Vector3(GetRandomOffset(), 0, GetRandomOffset());
            }

            private float GetRandomOffset()
            {
                return Random.Range(-0.35f, 0.35f);
            }

            private RaycastHit[] raycastResults = new RaycastHit[4];
            private Character character;
            private CharacterParameters parameters;

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

            public RaftStorage GoToEmptyStorage(ItemObject item, int value, bool throwIsFull = true)
            {
                var emptyStorage = raftBuildService.FindEmptyStorage(item, value);

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

            public List<RaftStorage> GoToEmptyStorages(ItemObject itemObject, int value = 1)
            {
                var storages = resourcesService.FindEmptyStorages(itemObject, value);
                if (storages.Count == 0)
                {
                    SpawnFullPopup();
                }
                return storages;
            }

            public RaftStorage FindResource(EResourceTypes type)
            {
                return resourcesService.FindStorageByResource(type);
            }
            
            public bool HaveMaterials(List<CraftObject.CraftItem> currentCraftIngredients)
            {
                return resourcesService.HaveMaterialsForCrafting(currentCraftIngredients);
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