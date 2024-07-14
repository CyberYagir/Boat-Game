using Content.Scripts.Global;
using Content.Scripts.Map;
using Content.Scripts.Map.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public class UIVillageTransferItem : MonoBehaviour
    {
        [SerializeField] private UIMark mark;
        [SerializeField] private Button button;

        public Button Button => button;

        public void Init(SaveDataObject.MapData.IslandData islandData)
        {
            mark.Init(new IslandSeedData(islandData.IslandPos), islandData.IslandName);
        }
    }
}
