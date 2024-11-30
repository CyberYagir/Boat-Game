using System.Collections;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Characters;
using Pathfinding;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private AIPath[] path;



    IEnumerator Start()
    {
        AstarPath.active.Scan();
        while (true)
        {
            yield return null;

            for (int i = 0; i < path.Length; i++)
            {
                path[i].GetComponent<INavAgentProvider>().SetDestination(Random.insideUnitSphere * 5);
            }
            yield return new WaitForSeconds(Random.Range(1, 4));
        }
    }
}
