using System.Collections;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using Pathfinding;
using UnityEngine;
using Zenject;

namespace Content.Scripts.IslandGame.Services
{
    public class GraphsUpdateService : MonoBehaviour
    {
        public class CharData
        {
            private GridGraph gridGraph;
            private Transform character;


            public CharData(GridGraph gridGraph, Transform character)
            {
                this.gridGraph = gridGraph;
                this.character = character;
            }

            public Transform Character => character;

            public GridGraph GridGraph => gridGraph;
        }
        
        private IslandGenerator islandGenerator;
        private CharacterService characterService;

        private List<CharData> charactersDatas = new List<CharData>();

        private int ticksCount;
        private TickService tickService;


        [Inject]
        private void Construct(IslandGenerator islandGenerator, CharacterService characterService, TickService tickService, IslandTransferRaftService raftService)
        {
            this.tickService = tickService;
            this.characterService = characterService;
            this.islandGenerator = islandGenerator;

            raftService.OnRaftTransferingEnding += OnRaftTransfered;
        }

        private void OnRaftTransfered()
        {
            StartCoroutine(SkipFrameForBuild());
        }


        IEnumerator SkipFrameForBuild()
        {
            yield return null;
            tickService.OnTick += OnTick;
            AddGraphs();
        }

        private void OnTick(float delta)
        {
            for (int i = 0; i < charactersDatas.Count; i++)
            {
                var needDistance = 2;
                var dist = Vector3.Distance(charactersDatas[i].Character.position, charactersDatas[i].GridGraph.center);
                if (dist > needDistance)
                {
                    charactersDatas[i].GridGraph.center = charactersDatas[i].Character.position + Vector3.down;
                    charactersDatas[i].GridGraph.Scan();
                }
            }
        }


        public void AddGraphs()
        {
            uint chId = 0;
            var path = AstarPath.active;

            if (path == null)
            {
                path = FindObjectOfType<AstarPath>();
            }
            
            foreach (var ch in characterService.SpawnedCharacters)
            {
                ch.SetCharacterRaftPosition();

                path.data.AddGraph(typeof(GridGraph));
                
                GridGraph graph = path.data.graphs[^1] as GridGraph;
                graph.center = ch.transform.position;
                graph.nodeSize = 0.25f;
                graph.width = 60;
                graph.depth = 60;

                graph.graphIndex = chId;
                graph.collision.collisionCheck = true;
                graph.collision.heightCheck = true;
                graph.collision.heightMask = LayerMask.GetMask("Raft", "Default");
                graph.collision.mask = LayerMask.GetMask("Trees");
                
                
                graph.erodeIterations = 1;
                graph.maxClimb = 5;
                graph.maxSlope = 70;

                ch.GetComponent<Seeker>().graphMask = 1 << (int) chId;
                
                charactersDatas.Add(new CharData(graph, ch.transform));
                
                graph.Scan();
                
                chId++;
            }

            path.FlushGraphUpdates();
        }
    }
}
