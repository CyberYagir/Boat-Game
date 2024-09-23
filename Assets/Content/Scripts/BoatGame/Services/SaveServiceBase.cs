using System.Collections;
using System.Collections.Generic;
using Content.Scripts.Global;
using UnityEngine;

namespace Content.Scripts.BoatGame.Services
{
    public interface ISaveServiceBase
    {
        void SaveWorld();
        void ReplaceJson(string json);
    }

    public class SaveServiceBase : MonoBehaviour, ISaveServiceBase
    {
        protected SaveDataObject saveDataObject;
        protected ICharacterService characterService;
        protected IRaftBuildService raftBuildService;

        public virtual void SaveWorld()
        {
        }

        public void ReplaceJson(string json)
        {
            saveDataObject.ReplaceJson(json);
        }

        public virtual void SaveRafts()
        {
            var raftsData = new SaveDataObject.RaftsData();
            for (int i = 0; i < raftBuildService.SpawnedRafts.Count; i++)
            {
                raftsData.AddSpawnedRaft(raftBuildService.SpawnedRafts[i]);
            }
            saveDataObject.SetRaftsData(raftsData);
        }

        public void ChangeTime()
        {
            saveDataObject.Global.AddTime(TimeService.PlayedTime);
            saveDataObject.Global.AddTimeOnRaft(TimeService.PlayedBoatTime);
            TimeService.ClearPlayedTime();
        }

        public bool SaveCharacters()
        {
            if (characterService.GetSpawnedCharacters().Count == 0) return true;


            List<bool> notDead = new List<bool>();

            for (int i = 0; i < characterService.GetSpawnedCharacters().Count; i++)
            {
                if (!characterService.GetSpawnedCharacters()[i].NeedManager.IsDead)
                {
                    notDead.Add(true);
                }
            }

            if (characterService.GetSpawnedCharacters().Count == 1 && notDead.Count == 0) return true;


            characterService.SaveCharacters();
            return false;
        }

        public IEnumerator AutoSave()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(300);
                SaveWorld();
            }
        }
        
        private void OnApplicationQuit()
        {
            SaveWorld();
        }
        
        private void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus) return;
            SaveWorld();
        }
    }
}