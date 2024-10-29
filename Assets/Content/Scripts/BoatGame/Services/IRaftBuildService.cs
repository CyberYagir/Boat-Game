using System;
using System.Collections.Generic;
using Content.Scripts.CraftsSystem;
using Content.Scripts.ItemsSystem;
using UnityEngine;

namespace Content.Scripts.BoatGame.Services
{
    public interface IRaftBuildService
    {
        List<RaftStorage> Storages { get; }
        public List<RaftBase> SpawnedRafts { get; }
        Transform Holder { get; }
        IResourcesService ResourcesService { get; }
        Transform RaftEndPoint { get; }
        event Action OnChangeRaft;
        RaftBase GetRaftByID(string raftID);
        RaftBase GetRandomWalkableRaft();
        RaftStorage FindEmptyStorage(ItemObject item, int value);
        void SetEndRaftPoint(Transform spawnPointLadderPoint);
        bool IsCanMoored();
        void SetTargetCraft(CraftObject item);
    }
}