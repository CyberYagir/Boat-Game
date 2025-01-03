using Content.Scripts.BoatGame.Services;
using UnityEngine;
using Zenject;

namespace Content.Scripts.BoatGame
{
    public class CrossSceneContext : MonoBehaviour
    {
        private static CrossSceneContext Instance;
        private CharacterService characterService;
        private SaveService saveService;
        private IResourcesService resourcesService;
        private RaftDamagerService raftDamagerService;
        private SelectionService selectionService;
        private TickService tickService;

        [Inject]
        private void Construct(
            CharacterService characterService, 
            SaveService saveService, 
            IResourcesService resourcesService, 
            RaftDamagerService raftDamagerService, 
            SelectionService selectionService,
            TickService tickService)
        {
            this.tickService = tickService;
            this.selectionService = selectionService;
            this.raftDamagerService = raftDamagerService;
            Instance = this;
            this.resourcesService = resourcesService;
            this.saveService = saveService;
            this.characterService = characterService;
        }

        public static CharacterService GetCharactersService() => Instance.characterService;
        public static SaveService GetSaveService() => Instance.saveService;
        public static IResourcesService GetResourcesService() => Instance.resourcesService;
        public static RaftDamagerService GetDamagerService() => Instance.raftDamagerService;
        public static SelectionService GetSelectionService() => Instance.selectionService;
        public static TickService GetTickService() => Instance.tickService;

    }
}
