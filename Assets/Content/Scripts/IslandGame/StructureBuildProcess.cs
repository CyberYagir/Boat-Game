using System;
using System.Globalization;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.CraftsSystem;
using Content.Scripts.Global;
using TMPro;
using UnityEngine;
using Zenject;

namespace Content.Scripts.IslandGame
{
    public class StructureBuildProcess : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private TMP_Text text;
        private SelectionService selectionService;
        private GameDataObject gameDataObject;
        private CraftObject craft;
        private DateTime date;
        private DateTime endTime;
        private StructuresBuildService structureBuildService;

        [Inject]
        private void Construct(SelectionService selectionService, StructuresService structuresService, StructuresBuildService structureBuildService, GameDataObject gameDataObject)
        {
            this.structureBuildService = structureBuildService;
            this.gameDataObject = gameDataObject;
            this.selectionService = selectionService;
        }

        public void Init(string craftID, string startBuildTime)
        {
            craft = gameDataObject.Crafts.Find(x => x.Uid == craftID);

            date = DateTime.Parse(startBuildTime, CultureInfo.InvariantCulture);

            endTime = this.date.AddSeconds(craft.CraftTime);
        }
        
        
        private void Update()
        {
            canvas.transform.LookAt(selectionService.Camera.transform.position);
            var time = (endTime - DateTime.Now);
            text.text = time.Hours.ToString("00") + ":" + time.Minutes.ToString("00") + ":" + time.Seconds.ToString("00");

            if (time.TotalSeconds < 0)
            {
                structureBuildService.SpawnStructures();
                enabled = false;
            }
        }
    }
}
