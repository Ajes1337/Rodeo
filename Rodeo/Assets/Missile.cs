using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Missile : MonoBehaviour
{
    public enum MissileTypes
    {
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
    private void Start()
    {
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

    public void SetMissileType(int type)
    {
        GameObject go = Instantiate(ParticleBullets[type], transform.position, Quaternion.identity) as GameObject;

        go.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
        go.transform.parent = this.transform;
    }

    // Update is called once per frame
    private void Update()
    {
        //daLight.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }

    private void FixedUpdate()
    {
        if (daRigidbody.velocity.magnitude < 35)
        {
            daRigidbody.AddForce(transform.forward * 10);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        int explosionNr = Random.Range(0, Explosions.Count - 1);
        GameObject go = Instantiate(Explosions[explosionNr], transform.position, Quaternion.identity) as GameObject;
        go.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        //Debug.Log(go.name + " explosion nr: " + explosionNr);

        //Instantiate(Explosions[Random.Range(0, Explosions.Count)], transform.position, Quaternion.identity);

        float maxDistance = 15;
        float maxDamage = 120;
        foreach (var item in CreatureSpawner.Instance.Creatures)
        {
            float distance = Vector3.Distance(item.transform.position, this.transform.position);
            if (distance < maxDistance)
            {
                var f = 1 - distance / maxDistance;
                item.Health -= maxDamage * f;
            }
        }

        Destroy(this.gameObject);

        //explosionSounds[Random.Range(0, explosionSounds.Count - 1)].Play();
    }
}