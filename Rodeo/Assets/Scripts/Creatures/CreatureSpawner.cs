using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CreatureSpawner : MonoBehaviour
{
    public static CreatureSpawner Instance;
    public Creature _creaturePrefab;
    public List<Creature> _creatures = new List<Creature>();
    private float _spawnProperbility = 100 / 1025.0f;

    private void Start()
    {
        Instance = this;
        Chunk.OnChunkCreated = OnChunkCreated;
        TerrainGen.OnChunkBorderPassd = OnChunkBorderPassd;
    }

    private void OnChunkBorderPassd()
    {
        foreach (var item in _creatures.ToList())
        {
            if (Vector3.Distance(item.transform.position, this.transform.position) > Constants.ViewRadiusChunk * Constants.ChunkWidth)
            {
                GameObject.Destroy(item.gameObject);
                _creatures.Remove(item);
            }
        }
    }

    private void OnChunkCreated(Chunk chunk)
    {
        if (_spawnProperbility > Random.value)
        {
            bool creatureFound = false;
            Vector3 spawnPos = Vector3.zero;
            for (int x = 1; x < Constants.ChunkWidth - 1; x++)
            {
                bool fromTop = Random.value > 0.5f;
                for (int y = fromTop ? Constants.ChunkHeight - 1 : 0; fromTop ? y >= 2 : (y < Constants.ChunkHeight - 1); y += fromTop ? -1 : 1)
                {
                    for (int z = 1; z < Constants.ChunkWidth - 1; z++)
                    {
                        if (chunk.Map[x, y, z] == 0 && chunk.Map[x, y + 2, z] == 0 && chunk.Map[x, y - 2, z] == 0)
                        {
                            spawnPos = chunk.transform.position + new Vector3(x, y, z);
                            creatureFound = true;
                            break;
                        }
                    }
                    if (creatureFound)
                        break;
                }
                if (creatureFound)
                    break;
            }

            if (creatureFound)
            {
                GameObject go = (GameObject)(GameObject.Instantiate(_creaturePrefab.gameObject, spawnPos, Quaternion.identity));
                _creatures.Add(go.GetComponent<Creature>());
            }
        }
    }

    public void IncreaseDifficulty()
    {
    }
}