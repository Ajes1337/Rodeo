using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(CreatureShape))]
public class SphereEditorTool : MonoBehaviour
{
    [SerializeField]
    private float _brushSize = 0.4f;

    private CreatureShape _creatureShape;
    private MeshCollider _sphereCollider;
    private Vector3 _prevMousePos;
    private Vector3 _extrudeDirection;

    private void Start()
    {
        _creatureShape = GetComponent<CreatureShape>();
        _sphereCollider = _creatureShape.GetComponent<MeshCollider>();
        Camera.main.gameObject.AddComponent<MouseOrbitImproved>().target = this.transform;
    }

    private void Update()
    {
        //RaycastHit hit = new RaycastHit();
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //bool hasHit = _sphereCollider.Raycast(ray, out hit, 1000);
        //var localPosition = _creatureShape.transform.InverseTransformPoint(hit.point);
        //if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        //{
        //    _extrudeDirection = MeshUtilities.GetNormal(_creatureShape.Mesh, localPosition);
        //}

        //if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        //{
        //    if (hasHit)
        //    {
        //        if (_extrudeDirection != Vector3.zero)
        //        {
        //            MeshUtilities.Extrude(_creatureShape.Mesh, localPosition, _brushSize, Input.GetMouseButton(0));
        //        }
        //    }
        //}
        //if (hasHit)
        //{
        //    MeshUtilities.ColorBrush(_creatureShape.Mesh, localPosition, _brushSize, Color.green, Color.white);
        //}
        //else
        //{
        //    MeshUtilities.ColorMesh(_creatureShape.Mesh, Color.white);
        //}

        //_creatureShape.Mesh.PushChanges(Input.GetMouseButton(0) || Input.GetMouseButton(1), Input.GetMouseButton(0) || Input.GetMouseButton(1));
        //_prevMousePos = Input.mousePosition;
    }

    private void OnGUI()
    {
        _brushSize = GUI.HorizontalSlider(new Rect(20, 20, 200, 10), _brushSize, 0.1f, 2);
    }
}