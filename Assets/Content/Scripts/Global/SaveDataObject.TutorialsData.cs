using System;
using UnityEngine;

namespace Content.Scripts.Global
{
    public partial class SaveDataObject
    {
        [Serializable]
        public class TutorialsData
        {
            [SerializeField] private bool clickTutorial;
            [SerializeField] private bool eatTutorial;
            [SerializeField] private bool storageTutorial;
            [SerializeField] private bool levelUpTutorial;
            [SerializeField] private bool villageDialogTutorial;

            public bool StorageTutorial => storageTutorial;

            public bool EatTutorial => eatTutorial;

            public bool ClickTutorial => clickTutorial;

            public bool LevelUpTutorial => levelUpTutorial;

            public bool VillageDialogTutorial => villageDialogTutorial;


            public void ClickTutorialSet()
            {
                clickTutorial = true;
                
                Debug.Log("Complete Action Tutorial");
            }
            
            public void EatTutorialSet()
            {
                eatTutorial = true;
                Debug.Log("Complete Eat Tutorial");
            }
            
            public void StorageTutorialSet()
            {
                storageTutorial = true;
                Debug.Log("Complete Storage Tutorial");
            }
            
            public void LevelUpTutorialSet()
            {
                levelUpTutorial = true;
                Debug.Log("Complete LevelUp Tutorial");
            }
            
            public void VillageDialogTutorialSet()
            {
                villageDialogTutorial = true;
                Debug.Log("Complete LevelUp Tutorial");
            }
        }
    }
}