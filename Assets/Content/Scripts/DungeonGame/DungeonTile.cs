using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

namespace Content.Scripts.DungeonGame
{
    public class DungeonTile : MonoBehaviour
    {
        [System.Serializable]
        private class Wall
        {
            [SerializeField] private Vector3 dir;
            [SerializeField] private GameObject wall;
            [SerializeField] private GameObject[] addtional;

            public Vector3 Dir => dir;

            public void SetActive(bool state)
            {
                wall.gameObject.SetActive(state);

                foreach (var add in addtional)
                {
                    add.gameObject.SetActive(state);
                }
            }
        }

        [SerializeField] private Wall[] walls;
        [SerializeField] private PropsSelector[] propSelectors;
        [SerializeField] private PropsSelector[] propsForAll;
        [SerializeField] private PropSpawner[] propSpawners;
        [SerializeField] private PropsSelectorDirections[] propSpawnersDirs;
        private Vector3Int coords;
        private WorldGridServiceTyped.ECellType targetCellType;
        private Random rnd;

        public WorldGridServiceTyped.ECellType TargetCellType => targetCellType;

        public void Init(List<Vector3> disabledDirections, WorldGridServiceTyped.ECellType eCellType, Vector3Int coords, System.Random rnd)
        {
            this.rnd = rnd;
            targetCellType = eCellType;
            this.coords = coords;
            ActiveWallsByDirections(disabledDirections);

            ActivateDecor(eCellType);
        }

        private void ActivateDecor(WorldGridServiceTyped.ECellType eCellType)
        {
            if (eCellType != WorldGridServiceTyped.ECellType.Room)
            {
                foreach (var spawner in propSelectors)
                {
                    if (spawner.gameObject.activeInHierarchy)
                    {
                        spawner.Init(rnd);
                    }
                }

                foreach (var spawner in propSpawners)
                {
                    if (spawner.gameObject.activeInHierarchy)
                    {
                        spawner.Init(rnd);
                    }
                }
                
                for (int i = 0; i < propSpawnersDirs.Length; i++)
                {
                    propSpawnersDirs[i].Init(rnd);   
                }
            }
            else
            {
                foreach (var spawner in propSelectors)
                {
                    spawner.gameObject.SetActive(false);
                }

                foreach (var spawner in propSpawners)
                {
                    spawner.gameObject.SetActive(false);
                }
            }
            
            foreach (var spawner in propsForAll)
            {
                spawner.Init(rnd);
            }
        }

        private void ActiveWallsByDirections(List<Vector3> disabledDirections)
        {
            for (int i = 0; i < walls.Length; i++)
            {
                if (disabledDirections.Contains(walls[i].Dir))
                {
                    walls[i].SetActive(false);
                }
            }

            for (int i = 0; i < walls.Length; i++)
            {
                if (!disabledDirections.Contains(walls[i].Dir))
                {
                    walls[i].SetActive(true);
                }
            }

            for (int i = 0; i < propSpawnersDirs.Length; i++)
            {
                propSpawnersDirs[i].SetDirs(disabledDirections);   
            }
        }

        [Button]
        public void GetAllSpawners()
        {
#if UNITY_EDITOR
            propSelectors = GetComponentsInChildren<PropsSelector>();
            propSpawners = GetComponentsInChildren<PropSpawner>();
            EditorUtility.SetDirty(this);
#endif
        }
    }
}
