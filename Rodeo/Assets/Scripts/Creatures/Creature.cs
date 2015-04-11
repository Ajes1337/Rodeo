using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GeneticLib;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Creature : MonoBehaviour
{
    public static Vector3[] Directions = new Vector3[]
    {
        Vector3.left,
        Vector3.up,
        Vector3.right,
        Vector3.down,
        Vector3.forward,
        new Vector3(-1,1,1).normalized,
        new Vector3(1,1,1).normalized,
        new Vector3(-1,-1,1).normalized,
        new Vector3(1,-1,1).normalized,
        new Vector3(-1,1,-1).normalized,
        new Vector3(1,1,-1).normalized,
        new Vector3(-1,-1,-1).normalized,
        new Vector3(1,-1,-1).normalized
    };

    private static float _raycastDistance = 20;
    private float _speedd = 220f;
    private Rigidbody _rigidbody;
    private Dictionary<Vector3, float> _directionAmounts = new Dictionary<Vector3, float>();
    private Vector3 _waypoint;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = false;
        _directionAmounts = Directions.ToDictionary(x => x, x => 1.0f);
        SetNewWaypoint(new Vector3(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1)).normalized, true);
    }

    private void SetNewWaypoint(Vector3 direction, bool force = false)
    {
        if (force || Vector3.Distance(this.transform.position, _waypoint) < 5)
        {
            _waypoint = this.transform.position + direction * 100;
        }
    }

    private void FixedUpdate()
    {
        float sum = 0;
        for (int i = 0; i < Directions.Length; i++)
        {
            Ray ray = new Ray(this.transform.position, Directions[i]);
            RaycastHit hit;
            float distance = 0;
            if (Physics.Raycast(ray, out hit, _raycastDistance))
            {
                distance = 1 - Mathf.Min(Vector3.Distance(this.transform.position, hit.point), _raycastDistance) / _raycastDistance;
                sum += distance;
            }
            _directionAmounts[Directions[i]] = distance;
        }

        Vector3 direction = Vector3.zero;
        foreach (var item in _directionAmounts)
        {
            direction += item.Value * item.Key;
        }
        direction *= -1;
        var randomDir = new Vector3(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1)).normalized;

        SetNewWaypoint(Vector3.Lerp(randomDir, direction, sum), sum > 0.6f);

        Vector3 directionToWaypont = (_waypoint - this.transform.position).normalized;

        if (_rigidbody.velocity != Vector3.zero)
        {
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(_rigidbody.velocity), 0.1f);
        }

        _rigidbody.AddForce(Vector3.Lerp(direction, directionToWaypont, sum) * Time.fixedDeltaTime * _speedd);
    }
}