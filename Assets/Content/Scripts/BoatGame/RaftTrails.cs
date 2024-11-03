using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Content.Scripts.BoatGame
{
    public class RaftTrails : MonoBehaviour
    {
        private IRaftBuildService raftBuildService;
        [SerializeField] private List<LineRenderer> lines;

        [Inject]
        private void Construct(IRaftBuildService raftBuildService)
        {
            this.raftBuildService = raftBuildService;
            
            raftBuildService.OnChangeRaft += RaftBuildServiceOnOnChangeRaft;
            RaftBuildServiceOnOnChangeRaft();
        }

        private void RaftBuildServiceOnOnChangeRaft()
        {
            RaftBase minRaft = raftBuildService.SpawnedRafts[0];
            RaftBase maxRaft = raftBuildService.SpawnedRafts[0];
            
            
            
            for (int i = 0; i < raftBuildService.SpawnedRafts.Count; i++)
            {
                var raft = raftBuildService.SpawnedRafts[i];
                if (raft.transform.position.z > maxRaft.transform.position.z)
                {
                    maxRaft = raft;
                }
                if (raft.transform.position.z < minRaft.transform.position.z)
                {
                    minRaft = raft;
                }
            }

            maxRaft = GetMostXRaft(maxRaft);
            minRaft = GetMostXRaft(minRaft);

            lines[0].transform.position = maxRaft.transform.position + new Vector3(0.5f, 0, 0.5f);
            lines[1].transform.position = minRaft.transform.position + new Vector3(0.5f, 0, -0.5f);;
            
            Lines();
        }

        RaftBase GetMostXRaft(RaftBase start)
        {
            for (int i = 0; i < raftBuildService.SpawnedRafts.Count; i++)
            {
                RaftBase raft = raftBuildService.SpawnedRafts[i];
                if (Math.Abs(raft.transform.position.z - start.transform.position.z) < 0.25f)
                {
                    if (raft.transform.position.x > start.transform.position.x)
                    {
                        start = raft;
                    }
                }
            }

            return start;
        }

        [SerializeField] private float noiseScale;
        [SerializeField] private float noiseFactor;
        [SerializeField] private float speedFactor;
        private void Update()
        {
            for (int i = 0; i < lines.Count; i++)
            {
                for (int j = 0; j < lines[i].positionCount; j++)
                {
                    var pos = lines[i].transform.position + new Vector3(j / (float) lines[i].positionCount, 0, 0);
                    
                    float xCoord = (Time.time * speedFactor) + pos.x * noiseScale;
                    float yCoord = (Time.time * speedFactor) + pos.z  * noiseScale;
                    float sample = Mathf.PerlinNoise(xCoord, yCoord) - 0.5f;
                    
                    
                    var percent = j / (float) lines[i].positionCount;
                    lines[i].SetPosition(j,  pos + new Vector3(0, 1, 0) * sample * noiseFactor * percent);
                }
            }
        }


        [Button]
        public void Lines()
        {
            for (int i = 0; i < lines.Count; i++)
            {
                for (int j = 0; j < lines[i].positionCount; j++)
                {
                    lines[i].SetPosition(j, lines[i].transform.position + new Vector3(j/(float)lines[i].positionCount, 0, 0));
                }
            }
        }
    }
}
