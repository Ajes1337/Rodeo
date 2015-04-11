using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class Missile : MonoBehaviour {
    public enum MissileTypes {
        Hole,
        LightningRotateBall,
        Portal,
        RuneOfMagic,
        Strom,
        SummonMagicCircle2,
        SummonMagicCircle3
    }

    private Rigidbody daRigidbody;
    private Light daLight;
    public List<GameObject> Explosions;
    public GameObject SmokeFab;
    private GameObject daSmoke;
    public List<GameObject> ParticleBullets;
    //public List<AudioClip> audioClips;
    //private List<AudioSource> explosionSounds = new List<AudioSource>();
    // Use this for initialization
    void Start() {
        daRigidbody = GetComponent<Rigidbody>();
        daRigidbody.AddForce(transform.forward * 5, ForceMode.VelocityChange);
        daLight = GetComponent<Light>();

        daRigidbody.velocity =
            TerrainGen.TerrainGenAjesSingletongVildhedRoflKartofel.ThePlayer.GetComponent<Rigidbody>().velocity;

        /*daSmoke = Instantiate(SmokeFab, transform.position, Quaternion.LookRotation(transform.forward * -1)) as GameObject;
        daSmoke.transform.parent = this.transform;
        daSmoke.transform.localScale = new Vector3(0.005f, 0.051f, 0.005f);*/
        /*  int particleBulletToTake = Random.Range(0, ParticleBullets.Count - 1);

          GameObject go = Instantiate(ParticleBullets[particleBulletToTake], transform.position, Quaternion.identity) as GameObject;
          go.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
          go.transform.parent = this.transform;*/
        //Debug.Log(go.name + " parrticlebullet nr: " + particleBulletToTake);

        /* foreach (AudioClip audioClip in audioClips) {
             GameObject go2 = new GameObject();
             AudioSource AudioSourceeee = go2.AddComponent<AudioSource>();
             AudioSourceeee.clip = audioClip;

         }*/

    }

    public void SetMissileType(MissileTypes type) {
        GameObject go = null;
        switch (type) {
            case MissileTypes.Hole:
                go = Instantiate(ParticleBullets[0], transform.position, Quaternion.identity) as GameObject;
                break;
            case MissileTypes.LightningRotateBall:
                go = Instantiate(ParticleBullets[1], transform.position, Quaternion.identity) as GameObject;
                break;
            case MissileTypes.Portal:
                go = Instantiate(ParticleBullets[2], transform.position, Quaternion.identity) as GameObject;
                break;
            case MissileTypes.RuneOfMagic:
                go = Instantiate(ParticleBullets[3], transform.position, Quaternion.identity) as GameObject;
                break;
            case MissileTypes.Strom:
                go = Instantiate(ParticleBullets[4], transform.position, Quaternion.identity) as GameObject;
                break;
            case MissileTypes.SummonMagicCircle2:
                go = Instantiate(ParticleBullets[5], transform.position, Quaternion.identity) as GameObject;
                break;
            case MissileTypes.SummonMagicCircle3:
                go = Instantiate(ParticleBullets[6], transform.position, Quaternion.identity) as GameObject;
                break;
            default:
                throw new ArgumentOutOfRangeException("type", type, null);
        }

        go.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
        go.transform.parent = this.transform;

    }

    // Update is called once per frame
    void Update() {

        //daLight.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

    }

    void FixedUpdate() {

        if (daRigidbody.velocity.magnitude < 35) {
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

        //explosionSounds[Random.Range(0, explosionSounds.Count - 1)].Play();

    }


}
