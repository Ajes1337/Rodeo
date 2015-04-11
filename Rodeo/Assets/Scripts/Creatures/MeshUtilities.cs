using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class MeshUtilities
{
    private static Vector3[] _octahedronVerts = {
            Vector3.down,
            Vector3.forward,
            Vector3.left,
            Vector3.back,
            Vector3.right,
            Vector3.up
        };

    private static int[] _octahedronTris = {
            0, 1, 2,
            0, 2, 3,
            0, 3, 4,
            0, 4, 1,

            5, 2, 1,
            5, 3, 2,
            5, 4, 3,
            5, 1, 4
        };

    public static System.Random _random = new System.Random();

    public static void GenerateCreature(DynamicMesh mesh, float radius)
    {
        int seed = _random.Next(0, 10000);
        List<Vector3> verts = new List<Vector3>(_octahedronVerts.Select(x => GetWorldPos(x, 0, seed)));
        List<int> triangles = new List<int>(_octahedronTris);
        //for (int i = 0; i < 2; i++)
        //{
        //    SubDivide(verts, triangles);
        //    verts = verts.Select(x => GetWorldPos(x, i + 1, seed)).ToList();
        //}

        mesh.Vertices = verts.ToArray();
        mesh.Triangles = triangles.ToArray();
        mesh.Colors = mesh.Vertices.Select(x => Color.green).ToArray();
    }

    private static void SubDivide(List<Vector3> verts, List<int> tris)
    {
        int vertCount = verts.Count;
        int offset = 0;
        Dictionary<Vector3, int> newVerts = new Dictionary<Vector3, int>(verts.Count);
        List<int> trisToRemove = new List<int>();
        int trisCount = tris.Count;
        for (int i = 0; i < trisCount; i += 3)
        {
            int i0 = tris[i + 0];
            int i1 = tris[i + 1];
            int i2 = tris[i + 2];

            Vector3 v0 = verts[i0];
            Vector3 v1 = verts[i1];
            Vector3 v2 = verts[i2];

            if (Vector3.Cross(v0 - v1, v0 - v2).magnitude > 0.1f)
            {
                Vector3 av0 = (v0 + v1) / 2;
                Vector3 av1 = (v1 + v2) / 2;
                Vector3 av2 = (v2 + v0) / 2;

                trisToRemove.Add(i + 0);
                trisToRemove.Add(i + 1);
                trisToRemove.Add(i + 2);

                int b0, b1, b2;
                int newTrisCount = 0;
                if (newVerts.ContainsKey(av0) == false)
                {
                    b0 = offset + newTrisCount;
                    newTrisCount++;
                    verts.Add(av0);
                    newVerts.Add(av0, b0);
                }
                else
                {
                    b0 = newVerts[av0];
                }

                if (newVerts.ContainsKey(av1) == false)
                {
                    b1 = offset + newTrisCount;
                    newTrisCount++;
                    verts.Add(av1);
                    newVerts.Add(av1, b1);
                }
                else
                {
                    b1 = newVerts[av1];
                }

                if (newVerts.ContainsKey(av2) == false)
                {
                    b2 = offset + newTrisCount;
                    newTrisCount++;
                    verts.Add(av2);
                    newVerts.Add(av2, b2);
                }
                else
                {
                    b2 = newVerts[av2];
                }

                offset += newTrisCount;

                // Top
                tris.Add(i0);
                tris.Add(vertCount + b0);
                tris.Add(vertCount + b2);

                // Right
                tris.Add(i1);
                tris.Add(vertCount + b1);
                tris.Add(vertCount + b0);

                // Left
                tris.Add(i2);
                tris.Add(vertCount + b2);
                tris.Add(vertCount + b1);

                // Middle
                tris.Add(vertCount + b1);
                tris.Add(vertCount + b2);
                tris.Add(vertCount + b0);
            }
        }
    }

    private static float ScaledNoise(float x, float y, float z)
    {
        return (1 + SimplexNoise.Noise.Generate(x, y, z)) / 2;
    }

    private static Vector3 GetWorldPos(Vector3 v, int ii, int seed)
    {
        if (ii == 0)
        {
            float factor = 0.2f;
            float scale = 2;
            var result = v.normalized * (0.5f + (1 + SimplexNoise.Noise.Generate(v.x * factor, seed + v.y * factor, v.z * factor)) / 2) * scale;
            result.y *= (1 + SimplexNoise.Noise.Generate(v.x * factor, seed + v.y * factor, v.z * factor)) / 2;
            result.z *= 1 + ((1 + SimplexNoise.Noise.Generate(v.x * factor, seed + v.y * factor, v.z * factor)) / 2) * 2;
            return result * 0.3f;
        }

        if (ii == 1)
        {
            float factor = 14f;
            float scale = 3f;
            if (ScaledNoise(v.x * factor, v.y * factor, seed + v.z * factor) > 0.9f)
            {
                return v * (1 + ScaledNoise(v.x * factor, v.y * factor, seed + v.z * factor) * scale);
            }
            else
            {
                return v;
            }
        }

        return v;
        //if (ii == 1)
        //{
        //    float factor = 0.2f;
        //    float scale = 1.2f;
        //    return v * (1 + (1 + SimplexNoise.Noise.Generate(seed + v.x * factor, v.y * factor, v.z * factor)) / 2) * scale;
        //}

        if (ii == 4)
        {
            float factor = 0.1f;
            float scale = 1.2f;
            return v * (1 + (1 + SimplexNoise.Noise.Generate(seed + v.x * factor, v.y * factor, v.z * factor)) / 2) * scale;
        }

        //if (ii == 4)
        //{
        //    float factor = 0.2f;
        //    float scale = 1.2f;
        //    return v * (1 + (1 + SimplexNoise.Noise.Generate(seed + v.x * factor, v.y * factor, v.z * factor)) / 2) * scale;
        //}

        //if (ii == 3)
        //{
        //    float factor = 0.2f;
        //    return v * (1 + SimplexNoise.Noise.Generate(v.x * factor, v.y * factor, v.z * factor));
        //}
    }

    private static void AutoSubdivide(DynamicMesh dynamicMesh)
    {
        //throw new NotImplementedException();
    }
}