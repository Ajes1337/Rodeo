using System.Collections;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshCollider))]
public class CreatureShape : MonoBehaviour
{
    private DynamicMesh _mesh;

    [SerializeField]
    private float _sphereRadius = 1;

    private void Start()
    {
        this._mesh = new DynamicMesh(this.GetComponent<MeshFilter>().sharedMesh, this.GetComponent<MeshCollider>(), this.GetComponent<MeshFilter>());
    }

    //public override void OnInspectorGUI()
    //{
    //    if (GUILayout.Button("Generate"))
    //    {
    //        GenerateBaseSphere();
    //    }
    //}

    private void GenerateBaseSphere()
    {
        if (_mesh == null)
        {
            this._mesh = new DynamicMesh(this.GetComponent<MeshCollider>(), this.GetComponent<MeshFilter>());
        }
        MeshUtilities.GenerateCreature(_mesh, _sphereRadius);
        _mesh.PushChanges(true, true);
    }

    public DynamicMesh Mesh { get { return _mesh; } }
}