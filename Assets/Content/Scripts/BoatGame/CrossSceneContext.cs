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
        private ResourcesService resourcesService;
        private RaftDamagerService raftDamagerService;
        private SelectionService selectionService;

        [Inject]
        private void Construct(CharacterService characterService, SaveService saveService, ResourcesService resourcesService, RaftDamagerService raftDamagerService, SelectionService selectionService)
        {
            this.selectionService = selectionService;
            this.raftDamagerService = raftDamagerService;
            Instance = this;
            this.resourcesService = resourcesService;
            this.saveService = saveService;
            this.characterService = characterService;
        }

        public static CharacterService GetCharactersService() => Instance.characterService;
        public static SaveService GetSaveService() => Instance.saveService;
        public static ResourcesService GetResourcesService() => Instance.resourcesService;
        public static RaftDamagerService GetDamagerService() => Instance.raftDamagerService;
        public static SelectionService GetSelectionService() => Instance.selectionService;

    }
}
