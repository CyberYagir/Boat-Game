using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class DungeonBossRoom : MonoBehaviour
{
    [SerializeField] private float angle, radius;
    [SerializeField] private List<Vector3> poses;
    [SerializeField] private int segments;

    [SerializeField] private GameObject walls;


#if UNITY_EDITOR

    [Button]
    public void Generate()
    {
        angle = 0;
        poses.Clear();
        for (int i = 0; i < (segments + 1); i++)
        {
            var x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            var y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            poses.Add(transform.position + new Vector3(x, 0f, y));

            angle += (360f / segments);
        }


        for (int i = 0; i < poses.Count - 1; i++)
        {
            var p = PrefabUtility.InstantiatePrefab(walls, transform) as GameObject;
            p.transform.position = poses[i];
            p.transform.LookAt(poses[i + 1]);
            p.gameObject.SetActive(true);
        }
    }

#endif


    private void OnDrawGizmos()
    {
        angle = 0;
        for (int i = 0; i < (segments + 1); i++)
        {
            var x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            var y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            Gizmos.DrawSphere(transform.position + new Vector3(x, 0f, y), 0.2f);

            angle += (360f / segments);
        }
    }
}
