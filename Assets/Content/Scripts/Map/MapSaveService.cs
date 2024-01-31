using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using UnityEngine;
using Zenject;

namespace Content.Scripts.Map
{
    public class MapSaveService : MonoBehaviour
    {
        private SaveDataObject saveDataObject;

        [Inject]
        private void Construct(SaveDataObject saveDataObject)
        {
            this.saveDataObject = saveDataObject;
        }
        
        
        private void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus) return;
            SaveWorld();
        }

        private void OnApplicationQuit()
        {
            SaveWorld();
        }

        private void SaveWorld()
        {
            saveDataObject.Global.AddTime(TimeService.PlayedTime);
            TimeService.ClearPlayedTime();
            saveDataObject.SaveFile();
        }
    }
}
