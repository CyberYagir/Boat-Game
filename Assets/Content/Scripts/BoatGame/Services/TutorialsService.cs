using System;
using System.Linq;
using Content.Scripts.BoatGame.UI;
using Content.Scripts.Global;
using UnityEngine;
using Zenject;

namespace Content.Scripts.BoatGame.Services
{
    public class TutorialsService : MonoBehaviour
    {
        [SerializeField] private UITutorialsDisplay tutorialsDisplay;
        [SerializeField] private TutorialDialogObject actionsTutorial;
        [SerializeField] private TutorialDialogObject eatTutorial;
        [SerializeField] private TutorialDialogObject storageTutorial;
        [SerializeField] private TutorialDialogObject levelupTutorial;
        
        
        private float time;
        private GameDataObject gameDataObject;
        private SaveDataObject saveData;
        private PlayerCharacter playerCharacter;
        private RaftBuildService raftBuildService;

        [Inject]
        private void Construct(TickService tickService, GameDataObject gameDataObject, SaveDataObject saveData, CharacterService characterService, RaftBuildService raftBuildService)
        {
            this.raftBuildService = raftBuildService;
            this.saveData = saveData;
            this.gameDataObject = gameDataObject;

            

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
            }
           
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

        private bool waitForDisplayStorageTutorial = false;
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
            ActionsTutorial();
            EatingTutorial();
            StorageTutorial();
            LevelUpTutorial();
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
                    if (playerCharacter == null) return;
                    if (playerCharacter.NeedManager.Hunger < PlayerCharacter.NeedsManager.minimalScores || playerCharacter.NeedManager.Thirsty < PlayerCharacter.NeedsManager.minimalScores)
                    {
                        tutorialsDisplay.DrawDialogue(eatTutorial);
                        saveData.Tutorials.EatTutorialSet();
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
