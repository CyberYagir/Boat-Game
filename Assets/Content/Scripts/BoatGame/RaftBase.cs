using System;
using Content.Scripts.BoatGame.Services;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Content.Scripts.BoatGame
{
    public class RaftBase : MonoBehaviour
    {
        [SerializeField, ReadOnly] private Vector3Int coords;
        [SerializeField] private float maxHealth;
        [SerializeField] private RaftBuildService.RaftItem.ERaftType raftType;
        [SerializeField, ReadOnly] private string uid;

        private float health;

        public Vector3Int Coords => coords;

        public float Health => health;

        public RaftBuildService.RaftItem.ERaftType RaftType => raftType;

        public string Uid => uid;

        public virtual void Init()
        {
            health = maxHealth;
            uid = Guid.NewGuid().ToString();
        }
        
        public void SetCoords(Vector3Int coords)
        {
            this.coords = coords;
        }

        public void LoadData(float raftHealth, string id)
        {
            health = raftHealth;
            uid = id;
        }
    }
}
