using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.UI.UIEquipment;
using Content.Scripts.Global;
using Content.Scripts.ItemsSystem;
using Unity.AI.Navigation;
using UnityEngine;
using Zenject;

namespace Content.Scripts.BoatGame.Services
{
    public interface ICharacterService
    {
        List<PlayerCharacter> GetSpawnedCharacters();
        PlayerCharacter GetClosestCharacter(Vector3 pos, out float distance);
    }

    public abstract class CharacterServiceBase : MonoBehaviour
    {
        public virtual List<PlayerCharacter> GetSpawnedCharacters() => null;
        
        public PlayerCharacter GetClosestCharacter(Vector3 pos, out float distance)
        {
            var minDist = 9999f;

            var characters = GetSpawnedCharacters();
            var character = characters[0];
            foreach (var c in characters)
            {
                var dist = Vector3.Distance(pos, c.transform.position);

                if (minDist > dist)
                {
                    minDist = dist;
                    character = c;
                }
            }


            distance = minDist;
            return character;
        }
    }

    public class CharacterService : CharacterServiceBase, ICharacterService
    {
        [SerializeField] private PlayerCharacter prefab;
        [SerializeField] private List<PlayerCharacter> spawnedCharacters;
        [SerializeField] private bool buildNavMeshAfterCharacters = true;
        private SelectionService selectionService;
        private SaveDataObject saveData;
        private INavMeshProvider navMeshProvider;

        public List<PlayerCharacter> SpawnedCharacters => spawnedCharacters;

        public event Action OnCharactersChange;
        public event Action<PlayerCharacter> OnCharactersFocus;

        [Inject]
        private void Construct(
            SaveDataObject saveData,
            RaftBuildService raftBuildService,
            GameDataObject gameDataObject,
            TickService tickService,
            WeatherService weatherService,
            SelectionService selectionService,
            PrefabSpawnerFabric prefabSpawnerFabric,
            INavMeshProvider navMeshProvider,
            ResourcesService resourcesService
        )
        {
            this.navMeshProvider = navMeshProvider;
            this.saveData = saveData;
            this.selectionService = selectionService;
            
            RebuildNavMesh();
            for (int i = 0; i < saveData.Characters.Count; i++)
            {
                var character = saveData.Characters.GetCharacter(i);
                character.ClearEvents();

                Instantiate(prefab, Vector3.zero, Quaternion.identity, raftBuildService.Holder)
                    .With(x => x.Init(
                        character,
                        gameDataObject,
                        raftBuildService,
                        weatherService,
                        tickService,
                        selectionService,
                        prefabSpawnerFabric,
                        navMeshProvider,
                        saveData
                    ))
                    .With(x => SpawnedCharacters.Add(x))
                    .With(x => x.NeedManager.OnDeath += OnDeath)
                    .With(x => x.GetComponent<PlayerCharacterTutorial>()?.Init(gameDataObject, this.saveData));
            }
            
            

            raftBuildService.OnChangeRaft += RebuildNavMesh;

            if (SpawnedCharacters.Count >= 1)
            {
                selectionService.ChangeCharacter(SpawnedCharacters[0]);
            }

            OnCharactersChange?.Invoke();            

        }

        public override List<PlayerCharacter> GetSpawnedCharacters() => spawnedCharacters;
        private void OnDeath(Character target)
        {
            SpawnedCharacters.RemoveAll(x => x == null || x.NeedManager.IsDead);
            if (SpawnedCharacters.Count != 0)
            {
                selectionService.ChangeCharacter(SpawnedCharacters[0]);
            }
            else
            {
                selectionService.ChangeCharacter(null);
                Debug.LogError("END GAME");
            }

            saveData.Characters.RemoveCharacter(target.Uid);
            
            OnCharactersChange?.Invoke();
        }

        private void RebuildNavMesh()
        {
            if (buildNavMeshAfterCharacters)
            {
                navMeshProvider.BuildNavMeshAsync();
            }
        }

        public void SaveCharacters()
        {
            foreach (var sp in spawnedCharacters)
            {
                if (!sp.NeedManager.IsDead)
                {
                    sp.Character.SetParameters(sp.NeedManager.GetParameters());
                    sp.Character.SetEffects(sp.ParametersCalculator.GetEffectsData());
                }
            }
        }

        public void FocusTo(PlayerCharacter targetCharacter)
        {
            OnCharactersFocus?.Invoke(targetCharacter);
        }
        
        
        public int CalculateWeaponsCount(ItemObject item)
        {
            int count = 0;
            foreach (var spawned in SpawnedCharacters)
            {
                if (spawned.AppearanceDataManager.WeaponItem != null)
                {
                    if (item == spawned.AppearanceDataManager.WeaponItem)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        public void RemoveWeapon(ItemObject item)
        {
            foreach (var spawned in SpawnedCharacters)
            {
                if (spawned.AppearanceDataManager.WeaponItem != null)
                {
                    if (item == spawned.AppearanceDataManager.WeaponItem)
                    {
                        spawned.Character.Equipment.SetEquipment(null, EEquipmentType.Weapon);
                        break;
                    }
                }
            }
        }
    }
}
