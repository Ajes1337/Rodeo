using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Missile : MonoBehaviour {

    private Rigidbody daRigidbody;
    private Light daLight;
    public List<GameObject> Explosions;
    public GameObject SmokeFab;
    private GameObject daSmoke;
    public List<GameObject> ParticleBullets;
    // Use this for initialization
    void Start() {
        daRigidbody = GetComponent<Rigidbody>();
        daRigidbody.AddForce(transform.forward * 5, ForceMode.VelocityChange);
        daLight = GetComponent<Light>();

        /*daSmoke = Instantiate(SmokeFab, transform.position, Quaternion.LookRotation(transform.forward * -1)) as GameObject;
        daSmoke.transform.parent = this.transform;
        daSmoke.transform.localScale = new Vector3(0.005f, 0.051f, 0.005f);*/
        int particleBulletToTake = Random.Range(0, ParticleBullets.Count - 1);

        GameObject go = Instantiate(ParticleBullets[particleBulletToTake], transform.position, Quaternion.identity) as GameObject;
        go.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
        go.transform.parent = this.transform;
        //Debug.Log(go.name + " parrticlebullet nr: " + particleBulletToTake);



    }

    // Update is called once per frame
    void Update() {

        //daLight.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

    }

    void FixedUpdate() {

        if (daRigidbody.velocity.magnitude < 25) {
            daRigidbody.AddForce(transform.forward * 10);
        }

    }

    private void OnCollisionEnter(Collision collision) {

        int explosionNr = Random.Range(0, Explosions.Count - 1);
        GameObject go = Instantiate(Explosions[explosionNr], transform.position, Quaternion.identity) as GameObject;
        go.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        //Debug.Log(go.name + " explosion nr: " + explosionNr);

        //Instantiate(Explosions[Random.Range(0, Explosions.Count)], transform.position, Quaternion.identity);

        Destroy(this.gameObject);



    }


}
