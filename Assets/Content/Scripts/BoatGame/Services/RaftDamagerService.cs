using System.Collections.Generic;
using Content.Scripts.BoatGame.RaftDamagers;
using Content.Scripts.Global;
using DG.DemiLib;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Content.Scripts.BoatGame.Services
{
    public class RaftDamagerService : MonoBehaviour
    {
        [SerializeField] private List<RaftDamager> damagerItems;
        [SerializeField] private List<RaftDamager> spawnedItems;
        [SerializeField] private Range tickRange;


        [SerializeField, ReadOnly] private float targetTicks;
        [SerializeField, ReadOnly] private int currentTicks;


        private RaftBuildService raftBuildService;
        private WorldGridService worldGridService;
        private bool isDamagerStarted;


        [Inject]
        private void Construct(
            TickService tickService,
            RaftBuildService raftBuildService,
            WorldGridService worldGridService,
            SaveDataObject saveDataObject
        )
        {
            this.worldGridService = worldGridService;
            this.raftBuildService = raftBuildService;
            ResetTimer();
            tickService.OnTick += TickServiceOnOnTick;
        }

        public void ResetTimer()
        {
            targetTicks = (int) tickRange.RandomWithin();
            currentTicks = 0;

            if (raftBuildService.SpawnedRafts.Count > 30)
            {
                targetTicks *= 0.5f;
            }
            else if (raftBuildService.SpawnedRafts.Count > 15)
            {
                targetTicks *= 0.7f;
            }
            else if (raftBuildService.SpawnedRafts.Count > 6)
            {
                targetTicks *= 0.9f;
            }

    
        }

        private void TickServiceOnOnTick(float delta)
        {
            if (isDamagerStarted)
            {
                currentTicks++;

                if (currentTicks >= targetTicks)
                {
                    ResetTimer();
                    CreateRandomSituation();
                }
            }
            else
            {
                if (raftBuildService.SpawnedRafts.Count > 4)
                {
                    isDamagerStarted = true;
                }
            }
        }

        private void CreateRandomSituation()
        {
            var index = damagerItems.GetRandomIndex();
            CreateSituationByID(index);
        }

        public RaftDamager CreateSituationByID(int id)
        {
            var getValidRaft = GetRaftOnSide();

            if (getValidRaft)
            {
                var item = Instantiate(damagerItems[id], getValidRaft.transform)
                    .With(x => spawnedItems.Add(x))
                    .With(x => x.Init(getValidRaft, this));

                return item;
            }

            return null;
        }

        List<RaftBase> tmpRafts = new List<RaftBase>();
        private RaftBase GetRaftOnSide()
        {
            tmpRafts.Clear();
            for (var i = 0; i < raftBuildService.SpawnedRafts.Count; i++)
            {
                if (raftBuildService.SpawnedRafts[i].RaftType != RaftBuildService.RaftItem.ERaftType.Building)
                {
                    var raft = raftBuildService.SpawnedRafts[i];
                    if (
                        IsEmpty(raft.Coords, Vector3Int.forward) ||
                        IsEmpty(raft.Coords, Vector3Int.back) ||
                        IsEmpty(raft.Coords, Vector3Int.left) ||
                        IsEmpty(raft.Coords, Vector3Int.right)
                    )
                    {
                        tmpRafts.Add(raft);
                    }
                }
            }

            if (tmpRafts.Count == 0) return null;

            return tmpRafts.GetRandomItem();
        }

        public bool IsEmpty(Vector3Int raftCoords, Vector3Int forward)
        {
            return !worldGridService.IsHavePoint(raftCoords + forward);
        }
    }
}
