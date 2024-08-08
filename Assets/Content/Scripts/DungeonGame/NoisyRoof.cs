using System;
using UnityEngine;

namespace Content.Scripts.DungeonGame
{
    public class NoisyRoof : MonoBehaviour
    {
        // private void Awake()
        // {
        //     return;
        //     Init();
        // }

        private void Init()
        {
            var startMesh = GetComponent<MeshFilter>().sharedMesh;
            var verts = startMesh.vertices;


            for (int i = 0; i < verts.Length; i++)
            {
                var globalSpace = transform.TransformPoint(verts[i]);
                verts[i].z += Mathf.PerlinNoise(globalSpace.x, globalSpace.z);
            }

            var mesh = new Mesh();
            mesh.SetVertices(verts);
            mesh.SetTriangles(startMesh.GetTriangles(0), 0);
            mesh.SetNormals(startMesh.normals);
            mesh.SetTangents(startMesh.tangents);

            GetComponent<MeshFilter>().mesh = mesh;
        }
    }
}
