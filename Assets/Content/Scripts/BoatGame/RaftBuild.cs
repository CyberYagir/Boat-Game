using System;
using Content.Scripts.BoatGame.PlayerActions;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.CraftsSystem;
using Content.Scripts.Global;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Content.Scripts.BoatGame
{
    public class RaftBuild : RaftBase
    {
        [SerializeField] private CraftObject craft;
        [SerializeField, Range(0, 100), OnValueChanged(nameof(ChangeValue))] private float progress;
        [SerializeField] private ActionsHolder actionsHolder;

        private float maxTime;
        private float time;
        private RaftBuildService raftBuildService;
        public event Action<float> OnChangeProgress;

        public float Progress => progress;

        public CraftObject Craft => craft;

        public float Time => time;

        public void SetCraft(CraftObject craftObject, SelectionService selectionService, RaftBuildService raftBuildService, GameDataObject gameDataObject)
        {
            this.raftBuildService = raftBuildService;
            craft = craftObject;
            actionsHolder.Construct(selectionService, gameDataObject);
            time = 0;
        }

        public void ChangeValue()
        {
            OnChangeProgress?.Invoke(progress);

            if (progress >= 100)
            {
                raftBuildService.RemoveRaft(Coords);
                raftBuildService.AddRaft(Coords, Craft);
            }
        }

        public void SetTime(float getSkillMultiply)
        {
            maxTime = Craft.CraftTime * getSkillMultiply;
        }


        public void AddProgress(float deltaTime)
        {
            time = Time + deltaTime;
            progress = (Time / maxTime * 100f);
            ChangeValue();
        }

        public void LoadBuild(SaveDataObject.RaftsData.RaftCraft data, GameDataObject gameDataObject, SelectionService selectionService, RaftBuildService raftBuildService)
        {
            time = data.BuildedTime;
            craft = gameDataObject.GetCraftByID(data.CraftID);
            actionsHolder.Construct(selectionService, gameDataObject);
            this.raftBuildService = raftBuildService;
            
            ChangeValue();
        }
    }
}
