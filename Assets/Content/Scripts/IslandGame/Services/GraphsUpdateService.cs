using System.Collections;
using System.Collections.Generic;
using Content.Scripts.BoatGame;
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
            private GridData gridData;

            public CharData(GridGraph gridGraph, Transform character,GridData gridData)
            {
                this.gridGraph = gridGraph;
                this.character = character;
                this.gridData = gridData;
            }

            public void SetGridData(GridData gridData)
            {
                this.gridData = gridData;
            }
            
            
            public Transform Character => character;

            public GridGraph GridGraph => gridGraph;

            public GridData GridData => gridData;
        }

        [System.Serializable]
        public class GridData
        {
            [SerializeField] private int width, depth;
            [SerializeField] private float cellSize;
            [SerializeField] private int erosion;
            [SerializeField] private float maxClimb = 1f;
            
            public float CellSize => cellSize;

            public int Depth => depth;

            public int Width => width;

            public int Erosion => erosion;

            public float MaxClimb => maxClimb;
        }

        [SerializeField] private int generateMaxTerrainDistance = 15;
        
        private CharacterService characterService;

        private List<CharData> charactersDatas = new List<CharData>(10);

        private int ticksCount;
        private TickService tickService;
        private IslandTransferRaftService raftConverterService;

        [SerializeField] private GridData onRaftGrid, onTerrainGrid;

        [Inject]
        private void Construct(IslandGenerator islandGenerator, CharacterService characterService, TickService tickService, IslandTransferRaftService raftService)
        {
            raftConverterService = raftService;
            this.tickService = tickService;
            this.characterService = characterService;

            
            raftService.OnRaftTransferingEnding += OnRaftTransfered;
        }

        private void OnUnitExitRaft(Collider obj)
        {
            ChangeGrid(obj, onTerrainGrid);
            print("Exit");
        }

        private void ChangeGrid(Collider obj, GridData dimensions)
        {
            var ch = GetCharacter(obj);
            if (ch != null)
            {
                var charData = charactersDatas.Find(x => x.Character == ch);

                ChangeGridData(dimensions, charData);
            }
        }

        private static void ChangeGridData(GridData dimensions, CharData charData)
        {
            charData.GridGraph.SetDimensions((int) dimensions.Width, (int) dimensions.Depth, dimensions.CellSize);
            charData.GridGraph.erodeIterations = dimensions.Erosion;
            charData.GridGraph.maxClimb = dimensions.MaxClimb;
            charData.SetGridData(dimensions);
            charData.GridGraph.Scan();
        }

        private static Transform GetCharacter(Collider obj)
        {
            var n = obj.GetComponentInParent<PlayerCharacter>();
            if (n != null)
            {
                return n.transform;
            }

            return null;
        }

        private void OnUnitEnterRaft(Collider obj)
        {
            ChangeGrid(obj, onRaftGrid);
            print("Enter");
        }

        private void OnRaftTransfered()
        {
            
            raftConverterService.EnterRaftTrigger.OnTriggerEntered += OnUnitEnterRaft;
            raftConverterService.ExitRaftTrigger.OnTriggerEntered += OnUnitExitRaft;
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
                var needDistance = (charactersDatas[i].GridGraph.width * charactersDatas[i].GridGraph.nodeSize) / 4.1f;
                var dist = Vector3.Distance(charactersDatas[i].Character.position, charactersDatas[i].GridGraph.center);
                
                
                
                if (dist >= needDistance)
                {
                    var pos = charactersDatas[i].Character.position + Vector3.down * 10;
                    if (pos.y < -0.5f)
                    {
                        pos.y = -0.5f;
                    }
                    
                    charactersDatas[i].GridGraph.center = pos;
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
                
                graph.SetDimensions(onRaftGrid.Width, onRaftGrid.Depth, onRaftGrid.CellSize);
                graph.erodeIterations = onRaftGrid.Erosion;
                
                graph.graphIndex = chId;
                graph.collision.collisionCheck = true;
                graph.collision.heightCheck = true;
                graph.collision.heightMask = LayerMask.GetMask("Raft", "Default");
                graph.collision.mask = LayerMask.GetMask("Trees", "Obstacle");
                
                
                graph.maxClimb = onRaftGrid.MaxClimb;
                graph.maxSlope = 60;

                ch.GetComponent<Seeker>().graphMask = 1 << (int) chId;
                
                charactersDatas.Add(new CharData(graph, ch.transform, onRaftGrid));
                
                graph.Scan();
                
                chId++;
            }

            path.FlushGraphUpdates();
        }
    }
}
