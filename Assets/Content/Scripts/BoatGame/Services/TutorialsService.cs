using System;
using System.Linq;
using Content.Scripts.BoatGame.Characters;
using Content.Scripts.BoatGame.Characters.States;
using Content.Scripts.BoatGame.UI;
using Content.Scripts.Global;
using Content.Scripts.IslandGame.Natives;
using UnityEngine;
using Zenject;

namespace Content.Scripts.BoatGame.Services
{
    public class TutorialsService : MonoBehaviour
    {
        [SerializeField] private UIService uiService;
        [SerializeField] private UITutorialsDisplay tutorialsDisplay;
        [SerializeField] private TutorialDialogObject actionsTutorial;
        [SerializeField] private TutorialDialogObject eatTutorial;
        [SerializeField] private TutorialDialogObject storageTutorial;
        [SerializeField] private TutorialDialogObject levelupTutorial;
        [SerializeField] private TutorialDialogObject villageTutorial;
        
        
        private float time;
        private GameDataObject gameDataObject;
        private SaveDataObject saveData;
        private PlayerCharacter playerCharacter;
        private IRaftBuildService raftBuildService;
        
        private bool waitForDisplayStorageTutorial = false;
        private bool waitForVillageViewTutorial = false;
        private CharacterService characterService;
        private NativeController villageShaman;
        

        [Inject]
        private void Construct(
            TickService tickService, 
            GameDataObject gameDataObject, 
            SaveDataObject saveData, 
            CharacterService characterService, 
            IRaftBuildService raftBuildService)
        {
            this.characterService = characterService;
            this.raftBuildService = raftBuildService;
            this.saveData = saveData;
            this.gameDataObject = gameDataObject;


            tutorialsDisplay.Init(tickService);
            
            time = saveData.Global.TotalSecondsOnRaft;
            
            tickService.OnTick += TickServiceOnOnTick;
            if (!saveData.Tutorials.StorageTutorial)
            {
                raftBuildService.OnChangeRaft += RaftBuildServiceOnOnChangeRaft;
                RaftBuildServiceOnOnChangeRaft();
            }
            
            if (characterService.SpawnedCharacters.Count >= 1)
            {
                playerCharacter = characterService.SpawnedCharacters[0];
                if (!saveData.Tutorials.LevelUpTutorial)
                {
                    playerCharacter.Character.SkillData.OnLevelUp += SkillDataOnOnLevelUp;
                }
                
                if (!saveData.Tutorials.VillageDialogTutorial && saveData.Global.isOnIsland && characterService.SpawnedCharacters.Count == 1)
                {
                    playerCharacter.GetCharacterAction<CharActionViewVillage>().OnFirstOpenWindow += OnFistOpenVillage;
                }
            }
           
        }

        private void OnFistOpenVillage(NativeController villageShaman)
        {
            this.villageShaman = villageShaman;
            waitForVillageViewTutorial = true;
            playerCharacter.GetCharacterAction<CharActionViewVillage>().OnFirstOpenWindow -= OnFistOpenVillage;
        }

        private bool waitForDisplayLevelUp = false;
        private void SkillDataOnOnLevelUp()
        {
            waitForDisplayLevelUp = true;
            playerCharacter.Character.SkillData.OnLevelUp -= SkillDataOnOnLevelUp;
        }

        private void OnDisable()
        {
            if (playerCharacter != null)
            {
                playerCharacter.Character.SkillData.OnLevelUp -= SkillDataOnOnLevelUp;
            }
        }

        private void RaftBuildServiceOnOnChangeRaft()
        {
            for (int i = 0; i < raftBuildService.Storages.Count; i++)
            {
                raftBuildService.Storages[i].OnStorageChange -= OnStorageUpdate;
                raftBuildService.Storages[i].OnStorageChange += OnStorageUpdate;
            }
        }



        private void OnStorageUpdate()
        {
            var max = raftBuildService.Storages.Sum(x => x.MaxItemsCount);
            var inside = raftBuildService.Storages.Sum(x => x.MaxItemsCount - x.GetEmptySlots());

            if (inside >= max)
            {
                waitForDisplayStorageTutorial = true;
            }
        }

        private void Update()
        {
            if (tutorialsDisplay.IsTextDisplayed) return;
            time += TimeService.UnscaledDelta;
        }

        private void TickServiceOnOnTick(float delta)
        {
            if (uiService.WindowManager.isAnyWindowOpened) return;
            if (characterService.SpawnedCharacters.Count > 1) return;
            if (playerCharacter == null || playerCharacter.NeedManager.IsDead) return;
            
            ActionsTutorial();
            EatingTutorial();
            StorageTutorial();
            LevelUpTutorial();
            VillageTutorial();
        }

        private void VillageTutorial()
        {
            if (waitForVillageViewTutorial)
            {
                if (tutorialsDisplay.IsTextDisplayed) return;

                if (!saveData.Tutorials.VillageDialogTutorial)
                {
                    if (villageShaman.NativeType == ENativeType.Shaman)
                    {
                        villageTutorial.SetCharacter(TutorialDialogObject.ECharacter.Shaman);
                    }
                    else if (villageShaman.NativeType == ENativeType.ShamanMedival)
                    {
                        villageTutorial.SetCharacter(TutorialDialogObject.ECharacter.MedivalShaman);
                    }

                    tutorialsDisplay.OnDialogueEnded += OnVillageDialogueEnd;
                    tutorialsDisplay.DrawDialogue(villageTutorial);
                    saveData.Tutorials.VillageDialogTutorialSet();
                    waitForVillageViewTutorial = false;
                }
            }
        }

        private void OnVillageDialogueEnd()
        {
            tutorialsDisplay.OnDialogueEnded -= OnVillageDialogueEnd;
            playerCharacter.GetCharacterAction<CharActionViewVillage>().ApplyShaman(villageShaman);
            playerCharacter.ActiveAction(EStateType.VillageViewInfo);
        }

        private void LevelUpTutorial()
        {
            if (waitForDisplayLevelUp)
            {
                if (tutorialsDisplay.IsTextDisplayed) return;

                if (!saveData.Tutorials.LevelUpTutorial)
                {
                    tutorialsDisplay.DrawDialogue(levelupTutorial);
                    saveData.Tutorials.LevelUpTutorialSet();
                    waitForDisplayLevelUp = false;
                }
            }
        }

        private void StorageTutorial()
        {
            if (waitForDisplayStorageTutorial)
            {
                if (tutorialsDisplay.IsTextDisplayed) return;
                if (!saveData.Tutorials.StorageTutorial)
                {
                    if (raftBuildService.Storages.Count == 1)
                    {
                        tutorialsDisplay.DrawDialogue(storageTutorial);
                        saveData.Tutorials.StorageTutorialSet();

                        raftBuildService.OnChangeRaft -= RaftBuildServiceOnOnChangeRaft;
                        for (int i = 0; i < raftBuildService.Storages.Count; i++)
                        {
                            raftBuildService.Storages[i].OnStorageChange -= OnStorageUpdate;
                        }
                    }
                    else if (raftBuildService.Storages.Count > 1)
                    {
                        saveData.Tutorials.StorageTutorialSet();
                    }
                }
            }
        }

        private void EatingTutorial()
        {
            if (tutorialsDisplay.IsTextDisplayed) return;
            if (!saveData.Tutorials.EatTutorial)
            {
                if (time >= gameDataObject.ConfigData.StartNeedsActiveTime)
                {
                    if (!playerCharacter.NeedManager.IsDead)
                    {
                        if (playerCharacter.NeedManager.Hunger < PlayerCharacter.NeedsManager.MINIMAL_SCORES || playerCharacter.NeedManager.Thirsty < PlayerCharacter.NeedsManager.MINIMAL_SCORES)
                        {
                            tutorialsDisplay.DrawDialogue(eatTutorial);
                            saveData.Tutorials.EatTutorialSet();
                        }
                    }
                }
            }
        }

        private void ActionsTutorial()
        {
            if (tutorialsDisplay.IsTextDisplayed) return;
            if (!saveData.Tutorials.ClickTutorial)
            {
                if (time >= gameDataObject.ConfigData.ActionsTutorialActiveTime)
                {
                    tutorialsDisplay.DrawDialogue(actionsTutorial);
                    saveData.Tutorials.ClickTutorialSet();
                }
            }
        }
    }
}
