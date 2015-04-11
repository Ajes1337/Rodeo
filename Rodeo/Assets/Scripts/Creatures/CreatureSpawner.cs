using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureSpawner : MonoBehaviour
{
    public static CreatureSpawner Instance;
    public List<Creature> _creatures = new List<Creature>();

    private void Start()
    {
        Instance = this;
    }

    private void Update()
    {
    }

    public void IncreaseDifficulty()
    {
    }
}