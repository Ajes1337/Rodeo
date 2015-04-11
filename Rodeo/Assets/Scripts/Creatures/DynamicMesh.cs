using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class DynamicMesh
{
    public Vector3[] Vertices;
    public Vector3[] Normals;
    public Color[] Colors;
    public Bounds Bounds;
    public int[] Triangles;

    private Mesh _mesh;
    private MeshCollider _collider;
    private MeshFilter _meshFilter;

    public DynamicMesh(MeshCollider collider, MeshFilter filter)
    {
        this._collider = collider;
        this._meshFilter = filter;
    }

    public DynamicMesh(Mesh mesh, MeshCollider collider, MeshFilter filter)
    {
        this._mesh = mesh;
        this._collider = collider;
        this._meshFilter = filter;

        this.Normals = mesh.normals;
        this.Colors = mesh.colors;
        this.Bounds = mesh.bounds;
        this.Triangles = mesh.triangles;
        this.Vertices = mesh.vertices;
    }

    public void PushChanges(bool calculateNormals, bool calculateBounds)
    {
        if (_mesh != null)
        {
            if (Application.isPlaying)
            {
                GameObject.Destroy(_mesh);
                GameObject.Destroy(_meshFilter.sharedMesh);
                GameObject.Destroy(_collider.sharedMesh);
            }
            else
            {
                GameObject.DestroyImmediate(_mesh);
                GameObject.DestroyImmediate(_meshFilter.sharedMesh);
                GameObject.DestroyImmediate(_collider.sharedMesh);
            }
        }

        _mesh = new Mesh();

        _mesh.vertices = Vertices;
        _mesh.triangles = Triangles;
        _mesh.colors = Colors;
        _mesh.bounds = Bounds;

        if (calculateNormals)
        {
            _mesh.RecalculateNormals();
            Normals = _mesh.normals;
        }
        else
        {
            _mesh.normals = Normals;
        }

        if (calculateBounds)
        {
            _mesh.RecalculateBounds();
            Bounds = _mesh.bounds;
        }

        _meshFilter.sharedMesh = _mesh;

        _collider.sharedMesh = null;
        _collider.sharedMesh = _mesh;
    }
}