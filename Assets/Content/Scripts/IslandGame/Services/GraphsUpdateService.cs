using System;
using System.Collections;
using System.Collections.Generic;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.Characters;
using Content.Scripts.BoatGame.Services;
using Pathfinding;
using Unity.VisualScripting;
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
            private INavAgentProvider agent;

            public CharData(GridGraph gridGraph, Transform character)
            {
                this.gridGraph = gridGraph;
                this.character = character;
                agent = character.Get<INavAgentProvider>();
            }

            public Transform Character => character;

            public GridGraph GridGraph => gridGraph;

            public INavAgentProvider Seeker => agent;
        }
        
        private CharacterService characterService;

        private List<CharData> charactersDatas = new List<CharData>(10);
        
        private int ticksCount;
        private TickService tickService;
        private IslandTransferRaftService raftConverterService;

        private INavMeshProvider navMeshProvider;

        private int raftGraphMask;
        private int terrainGraphMask;
        private float switchRadius;

        [SerializeField] private float switchRadiusModify;
        
        [Inject]
        private void Construct(
            IslandGenerator islandGenerator, 
            CharacterService characterService, 
            TickService tickService, 
            IslandTransferRaftService raftTransferService, 
            RaftBuildService raftBuildService,
            INavMeshProvider navMeshProvider)
        {
            this.navMeshProvider = navMeshProvider;
            raftConverterService = raftTransferService;
            this.tickService = tickService;
            this.characterService = characterService;

            raftBuildService.OnChangeRaft += RebuildTargetNavGrid;
            raftTransferService.OnRaftTransferingEnding += OnRaftTransfer;
        }



        private void OnRaftTransfer()
        {
            StartCoroutine(SkipFrameForBuild());
        }


        IEnumerator SkipFrameForBuild()
        {
            yield return null;
            
            CenterNavGraph();
            AddCharacters();
            CalculateMasks();
            
            
            navMeshProvider.BuildNavMesh();
            EnableCharactersAIAfterMoving();
            
            tickService.OnTick += GraphSwitcher;
        }
        
        private void RebuildTargetNavGrid()
        {
            navMeshProvider.BuildNavMeshAsync(1);
        }

        private void CalculateMasks()
        {
            raftGraphMask = GraphMask.FromGraph(navMeshProvider.GetNavMeshByID(1));
            terrainGraphMask = GraphMask.FromGraph(navMeshProvider.GetNavMeshByID(0));
        }

        private void EnableCharactersAIAfterMoving()
        {
            for (int i = 0; i < characterService.SpawnedCharacters.Count; i++)
            {
                characterService.SpawnedCharacters[i].Get<AIPath>().enabled = true;
            }
        }

        private void AddCharacters()
        {
            OnCharactersAdded();
            characterService.OnCharactersChange += OnCharactersAdded;
        }

        private void OnCharactersAdded()
        {
            charactersDatas.Clear();
            for (int i = 0; i < characterService.SpawnedCharacters.Count; i++)
            {
                charactersDatas.Add(new CharData(null, characterService.SpawnedCharacters[i].transform));
            }
        }

        private void CenterNavGraph()
        {
            var nav = (navMeshProvider.GetNavMeshByID(1) as GridGraph);
            nav.center = raftConverterService.RaftPoint;
            switchRadius = (nav.width/2f) * nav.nodeSize;
        }

        private void GraphSwitcher(float delta)
        {
            for (int i = 0; i < charactersDatas.Count; i++)
            {
                var dist = Vector3.Distance(charactersDatas[i].Character.transform.position, raftConverterService.RaftPoint) <= switchRadius * switchRadiusModify;
                if (!charactersDatas[i].Seeker.IsStopped)
                {
                    var targetGraph = dist ? raftGraphMask : terrainGraphMask;
                    
                    if (charactersDatas[i].Seeker.GetCurrentGraphMask() == terrainGraphMask && targetGraph == raftGraphMask)
                    {
                        var nearestPosOnTerrain = navMeshProvider
                            .GetNavMeshByID(0)
                            .GetNearest(charactersDatas[i].Seeker.TargetPoint, NNConstraint.Default)
                            .clampedPosition;
                        
                        print(Vector3.Distance(nearestPosOnTerrain, charactersDatas[i].Seeker.TargetPoint));
                        
                        if (Vector3.Distance(nearestPosOnTerrain, charactersDatas[i].Seeker.TargetPoint) < 1f)
                        {
                            charactersDatas[i].Seeker.ChangeMask(terrainGraphMask);
                            continue;
                        }
                    }
                    charactersDatas[i].Seeker.ChangeMask(targetGraph);
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (AstarPath.active == null || AstarPath.active.graphs.Length == 0) return;
            Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.2f);
            Gizmos.DrawSphere((AstarPath.active.graphs[1] as GridGraph).center, switchRadius * switchRadiusModify);
        }
    }
}
