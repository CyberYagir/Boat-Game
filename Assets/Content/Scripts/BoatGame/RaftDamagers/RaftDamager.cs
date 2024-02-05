using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using UnityEngine;

namespace Content.Scripts.BoatGame.RaftDamagers
{
    public abstract class RaftDamager : MonoBehaviour
    {
        [System.Serializable]
        public class RaftDamagerDataKey
        {
            [SerializeField] private string key;
            [SerializeField] private string data;

            public RaftDamagerDataKey(string key, string data)
            {
                this.key = key;
                this.data = data;
            }

            public string Data => data;

            public string Key => key;
        }
        private int id;
        private RaftBase targetRaft;
        private RaftDamagerService raftDamagerService;

        public RaftBase TargetRaft => targetRaft;

        public RaftDamagerService DamagerService => raftDamagerService;

        public int ID => id;

        public event Action<RaftDamager> OnEndDamager;
        
        public virtual void Init(int id, RaftBase targetRaft, RaftDamagerService raftDamagerService)
        {
            this.id = id;
            this.raftDamagerService = raftDamagerService;
            this.targetRaft = targetRaft;
        }

        public virtual void SetKeysData(List<RaftDamagerDataKey> data)
        {
            
        }

        public virtual List<RaftDamagerDataKey> GetKeysData()
        {
            return new List<RaftDamagerDataKey>();
        }

        public void EndDamager()
        {
            OnEndDamager?.Invoke(this);
        }
    }
}