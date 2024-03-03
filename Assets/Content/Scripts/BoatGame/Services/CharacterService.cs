using System;
using System.Collections.Generic;
using Content.Scripts.Global;
using Unity.AI.Navigation;
using UnityEngine;
using Zenject;

namespace Content.Scripts.BoatGame.Services
{
    public class CharacterService : MonoBehaviour
    {
        [SerializeField] private PlayerCharacter prefab;
        [SerializeField] private List<PlayerCharacter> spawnedCharacters;
        
        private NavMeshSurface surface;
        private SelectionService selectionService;
        private SaveDataObject saveData;

        public List<PlayerCharacter> SpawnedCharacters => spawnedCharacters;

        public event Action OnCharactersChange;

        [Inject]
        private void Construct(
            SaveDataObject saveData,
            RaftBuildService raftBuildService,
            GameDataObject gameDataObject,
            TickService tickService,
            WeatherService weatherService,
            SelectionService selectionService,
            PrefabSpawnerFabric prefabSpawnerFabric
        )
        {
            this.saveData = saveData;
            this.selectionService = selectionService;
            surface = raftBuildService.Holder.GetComponent<NavMeshSurface>();
            
            RebuildNavMesh();
            for (int i = 0; i < saveData.Characters.Count; i++)
            {
                var id = i;
                Instantiate(prefab, Vector3.zero, Quaternion.identity, raftBuildService.Holder)
                    .With(x => x.Init(
                        saveData.Characters.GetCharacter(id),
                        gameDataObject,
                        raftBuildService,
                        weatherService,
                        tickService,
                        selectionService,
                        prefabSpawnerFabric
                    ))
                    .With(x => SpawnedCharacters.Add(x))
                    .With(x => x.NeedManager.OnDeath += OnDeath);
            }
            
            

            raftBuildService.OnChangeRaft += RebuildNavMesh;

            if (SpawnedCharacters.Count >= 1)
            {
                selectionService.ChangeCharacter(SpawnedCharacters[0]);
            }

            OnCharactersChange?.Invoke();            
            print("execute " + transform.name);

        }

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
            if (surface.enabled)
                surface.BuildNavMesh();
        }

        public void SaveCharacters()
        {
            foreach (var sp in spawnedCharacters)
            {
                sp.Character.SetParameters(sp.NeedManager.GetParameters());
            }
        }
    }
}
