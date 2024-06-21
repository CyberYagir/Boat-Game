using Content.Scripts.IslandGame;
using Content.Scripts.ItemsSystem;
using UnityEngine;

namespace Content.Scripts.Global
{
    [CreateAssetMenu(menuName = "Create ConfigDataObject", fileName = "ConfigDataObject", order = 0)]
    public class ConfigDataObject : ScriptableObject
    {
        [SerializeField] private float startNeedsActiveTime;
        [SerializeField] private float actionsTutorialActiveTime;
        [SerializeField] private int actionsCountToTutorial;
        [SerializeField] private int paddlesToTravelCount;
        [SerializeField] private ItemObject paddleItem;
        [SerializeField] private NoiseGenerator mapNoisePreset;
        
        public float StartNeedsActiveTime => startNeedsActiveTime;
        public int ActionsCountToTutorial => actionsCountToTutorial;
        public float ActionsTutorialActiveTime => actionsTutorialActiveTime;
        public NoiseGenerator MapNoisePreset => mapNoisePreset;
        public int PaddlesToTravelCount => paddlesToTravelCount;
        public ItemObject PaddleItem => paddleItem;
    }
}
