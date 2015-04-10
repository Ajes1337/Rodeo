using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshCollider))]
public class CreatureShape : BaseMonoBehaviour
{
    private Mesh _mesh;
    private float _sphereRadius = 1;

    private void Start()
    {
    }

    private void Update()
    {
    }

    private void OnGUI()
    {
        //if (GUI.Button()
        //{
        //}
    }

    private void Initialize()
    {
        if (_mesh == null)
        {
            _mesh = new Mesh();
            this.GetComponent<MeshFilter>().sharedMesh = _mesh;
        }
    }
}