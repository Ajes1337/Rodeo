using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LocalPacket {
    public PacketType Type;
    public List<Vector2I> ChunksToCreate;
    public List<Chunk> ChunksToRemove;
    public Chunk ChunkRef;
    public byte[, ,] Map;
    public Vector3[] verts;
    public int[] tris;
    public Color32[] colors;
}
