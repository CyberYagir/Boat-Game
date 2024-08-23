using System;
using System.Collections.Generic;

namespace Content.Scripts.BoatGame.Services
{
    public interface IRaftBuildService
    {
        List<RaftStorage> Storages { get; }
        public List<RaftBase> SpawnedRafts { get; }
        event Action OnChangeRaft;
    }
}