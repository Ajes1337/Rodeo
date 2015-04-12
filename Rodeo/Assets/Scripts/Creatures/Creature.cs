using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Light))]
public class Creature : MonoBehaviour {
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

    private static Color _lineColor = new Color(167 / 255f, 252 / 255f, 0.4f, 0.63f);
    private static float _raycastDistance = 12;
    private static float _raycastLineDistance = 10;

    private float _speedd = 220f;
    private float _rayDistance = 24;
    private float _maxStepDistance = 0.4f;
    private Rigidbody _rigidbody;
    private Dictionary<Vector3, float> _directionAmounts = new Dictionary<Vector3, float>();
    private Vector3 _waypoint;
    private Vector3[] _lightningRays;
    private Vector3[] _lightningSpeeds;
    private Vector3[] _lightningHits;
    private LineRenderer[] _lineRenderes;
    public float Health = 100;

    [SerializeField]
    private Material _lineMaterial;

    private Light _light;
    private Renderer _renderer;

    public List<GameObject> Explosions;
    private Color _color;

    private void Start() 
    {
        _color = new Color(Random.value, Random.value, Random.value);    
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = false;
        _directionAmounts = Directions.ToDictionary(x => x, x => 1.0f);
        _lightningRays = Enumerable.Range(0, 5/*Random.Range(15, 30)*/).Select(x => new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10))).ToArray();
        _lightningSpeeds = Enumerable.Range(0, _lightningRays.Length).Select(x => new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1))).ToArray();
        _lightningHits = new Vector3[_lightningRays.Length];
        _lineRenderes = new LineRenderer[_lightningRays.Length];

        DynamicMesh mesh = new DynamicMesh(this.GetComponent<MeshCollider>(), this.GetComponent<MeshFilter>());
        MeshUtilities.GenerateCreature(mesh, 1);
        mesh.PushChanges(true, true);

        SetNewWaypoint(new Vector3(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1)).normalized, true);
        AddLineRenderers();
        _light = this.gameObject.GetComponent<Light>();
        _renderer = this.gameObject.GetComponent<Renderer>();
    }

    private void AddLineRenderers() {
        for (int i = 0; i < _lineRenderes.Length; i++) {
            var go = new GameObject();
            go.transform.parent = this.transform;
            go.transform.localPosition = Vector3.zero;
            _lineRenderes[i] = go.AddComponent<LineRenderer>();
            _lineRenderes[i].material = _lineMaterial;
            _lineRenderes[i].SetWidth(0.1f, 0.1f);
            _lineRenderes[i].SetColors(_lineColor, _lineColor);
        }
    }

    private void SetNewWaypoint(Vector3 direction, bool force = false) {
        if (force || Vector3.Distance(this.transform.position, _waypoint) < 5) {
            _waypoint = this.transform.position + direction * 100;
        }
    }

    private void FixedUpdate() {
        float sum = 0;
        for (int i = 0; i < Directions.Length; i++) {
            Ray ray = new Ray(this.transform.position, Directions[i]);
            RaycastHit hit;
            float distance = 0;
            if (Physics.Raycast(ray, out hit, _raycastDistance)) {
                distance = 1 - Mathf.Min(Vector3.Distance(this.transform.position, hit.point), _raycastDistance) / _raycastDistance;
                sum += distance;
            }
            _directionAmounts[Directions[i]] = distance;
        }

        Vector3 direction = Vector3.zero;
        foreach (var item in _directionAmounts) {
            direction += item.Value * item.Key;
        }
        direction *= -1;
        Vector3 randomDir;

        float distanceToPlayer = Vector3.Distance(Player.Instance.transform.position, this.transform.position);

        if (distanceToPlayer < 70) {
            randomDir = Player.Instance.transform.position;
        }
        else {
            randomDir = new Vector3(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1)).normalized;
        }

        if (distanceToPlayer < 10) {
            float dmg = (1 - distanceToPlayer / 10) * Time.deltaTime * 500;
            Player.Instance.Health -= dmg;
        }

        SetNewWaypoint(Vector3.Lerp(randomDir, direction, sum), sum > 0.6f);

        Vector3 directionToWaypont = (_waypoint - this.transform.position).normalized;

        if (_rigidbody.velocity != Vector3.zero) {
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(_rigidbody.velocity), 0.1f);
        }

        _rigidbody.AddForce(Vector3.Lerp(direction, directionToWaypont, sum) * Time.fixedDeltaTime * _speedd);

        UpdateLineRenderes();

        UpdateColors();

        if (Health < 0) {
            MakeExplosion();
            GameObject.Destroy(this.gameObject);
            CreatureSpawner.Instance.Creatures.Remove(this);
            Player.Instance.IncreseScore();
        }
    }

    private void UpdateColors() {
        var color = Color.Lerp(_color, Color.red, 1 - Health / 100f);
        _light.color = color;
        _renderer.material.color = color;
    }

    private void UpdateLineRenderes() {
        for (int i = 0; i < _lightningRays.Length; i++) {
            Vector3 direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            Ray ray = new Ray(this.transform.position, direction);
            RaycastHit hit = default(RaycastHit);

            float d = Vector3.Distance(_lightningHits[i], this.transform.position);

            if (d < _raycastLineDistance || Physics.Raycast(ray, out hit, _raycastLineDistance)) {
                if (d < _raycastLineDistance) {
                    d = Vector3.Distance(this.transform.position, _lightningHits[i]);
                    direction = (_lightningHits[i] - this.transform.position).normalized;
                    RenderLine(direction, _lineRenderes[i], _lightningHits[i], d);
                }
                else {
                    d = Vector3.Distance(this.transform.position, hit.point);
                    RenderLine(direction, _lineRenderes[i], hit.point, d);
                    _lightningHits[i] = hit.point;
                }
                _lineColor.a = 1 - d / _raycastLineDistance;
                _lineRenderes[i].SetColors(_lineColor, _lineColor);
            }
            else {
                _lineRenderes[i].SetVertexCount(0);
                _lineColor.a = 0;
                _lineRenderes[i].SetColors(_lineColor, _lineColor);
            }
        }
    }

    private void RenderLine(Vector3 direction, LineRenderer lineRenderer, Vector3 target, float d) {
        int i = 0;
        int numOfPoints = GetNumberOfPoints(direction, lineRenderer, target, d);
        lineRenderer.SetVertexCount(numOfPoints);
        float currentDistance = 0;
        Vector3 currentPosition = this.transform.position;
        while (currentDistance < d) {
            if (i == 0) {
                lineRenderer.SetPosition(i, currentPosition);
            }
            else {
                /*   Vector3 noise = new Vector3(SimplexNoise.Noise.Generate(Time.time + i * 1), SimplexNoise.Noise.Generate(Time.time * 1 + i * 3), SimplexNoise.Noise.Generate(Time.time * 1 + i * 3));*/
                Vector3 noise = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                lineRenderer.SetPosition(i, currentPosition + noise * 0.25f);
            }
            i++;
            currentDistance += _maxStepDistance;
            currentPosition += currentDistance * direction;
        }

        lineRenderer.SetPosition(i, target);
    }

    private int GetNumberOfPoints(Vector3 direction, LineRenderer lineRenderer, Vector3 target, float d) {
        int i = 0;
        float currentDistance = 0;
        while (currentDistance < d) {
            i++;
            currentDistance += _maxStepDistance;
        }
        return i + 1;
    }

    private void MakeExplosion() {
        int explosionNr = Random.Range(0, Explosions.Count - 1);
        GameObject go = Instantiate(Explosions[explosionNr], transform.position, Quaternion.identity) as GameObject;
        go.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }

    private void OnDestroy() {
    }
}