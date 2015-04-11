using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using PlaynomicsPlugin;
using SimplexNoise;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Worker {
    public enum WorkingType {
        FindNewChunkPoses,
        GenMap,
        GenMesh,
        Idle
    }

    private bool Run = true;
    private ConcurrentQueue<LocalPacket> IncommingNormalSpeed = new ConcurrentQueue<LocalPacket>();
    private ConcurrentQueue<LocalPacket> IncommingFastSpeed = new ConcurrentQueue<LocalPacket>();
    private ConcurrentQueue<LocalPacket> IncommingFasterSpeed = new ConcurrentQueue<LocalPacket>();
    public readonly AutoResetEvent WaitHandle = new AutoResetEvent(false);
    public WorkingType DoingAtm = WorkingType.Idle;

    public void Go() {

        while (Run) {
            try {


                LocalPacket packet = null;
                if (IncommingFasterSpeed.Count > 0) {
                    packet = IncommingFasterSpeed.Dequeue();
                }
                else if (IncommingFastSpeed.Count > 0) {
                    packet = IncommingFastSpeed.Dequeue();
                }
                else if (IncommingNormalSpeed.Count > 0) {
                    packet = IncommingNormalSpeed.Dequeue();
                }

                if (ReferenceEquals(null, packet)) {
                    WaitHandle.WaitOne();
                }
                else {
                    switch (packet.Type) {
                        case PacketType.NeedChunkPoses:
                            DoingAtm = WorkingType.FindNewChunkPoses;
                            HandleNeedChunkPoses(packet);
                            break;
                        case PacketType.GenMap:
                            DoingAtm = WorkingType.GenMap;
                            HandleGenMap(packet);
                            break;
                        case PacketType.GenMesh:
                            DoingAtm = WorkingType.GenMesh;
                            HandleGenMesh(packet);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                DoingAtm = WorkingType.Idle;

            }
            catch (Exception e) {
                Debug.Log("Worker laver fejl: " + e);
                throw;
            }
        }


    }

    private void HandleGenMesh(LocalPacket packet) {

        List<Vector3> verts = new List<Vector3>();//TODO evt. smid en ca. init størrelse på dem her..
        List<int> tris = new List<int>();

        /*  Vector3[] verts = new Vector3[4];
          int[] tris = new int[6];*/

        /*verts[0] = new Vector3(0, 1, 0);
        verts[1] = new Vector3(0, 1, 1);
        verts[2] = new Vector3(1, 1, 1);
        verts[3] = new Vector3(1, 1, 0);

        tris[0] = 0;
        tris[1] = 1;
        tris[2] = 2;
        tris[3] = 2;
        tris[4] = 3;
        tris[5] = 0;*/


        for (int x = 0; x < Constants.ChunkWidth; x++) {
            for (int y = 0; y < Constants.ChunkHeight; y++) {
                for (int z = 0; z < Constants.ChunkWidth; z++) {
                    if (packet.ChunkRef.GetMap(x, y, z) != 0) {

                        if (packet.ChunkRef.GetMap(x, y + 1, z) == 0) {
                            MakeFace(x, y, z, Direction.Top, ref verts, ref tris);
                        }
                        if (packet.ChunkRef.GetMap(x, y - 1, z) == 0) {
                            MakeFace(x, y, z, Direction.Bottom, ref verts, ref tris);
                        }

                        if (packet.ChunkRef.GetMap(x + 1, y, z) == 0) {
                            MakeFace(x, y, z, Direction.East, ref verts, ref tris);
                        }
                        if (packet.ChunkRef.GetMap(x - 1, y, z) == 0) {
                            MakeFace(x, y, z, Direction.West, ref verts, ref tris);
                        }

                        if (packet.ChunkRef.GetMap(x, y, z + 1) == 0) {
                            MakeFace(x, y, z, Direction.North, ref verts, ref tris);
                        }
                        if (packet.ChunkRef.GetMap(x, y, z - 1) == 0) {
                            MakeFace(x, y, z, Direction.South, ref verts, ref tris);
                        }

                    }
                }

            }
        }

        packet.verts = verts.ToArray();
        packet.tris = tris.ToArray();

        packet.ChunkRef.IncommingPackets.Enqueue(packet);
    }



    private void MakeFace(int x, int y, int z, Direction dir, ref List<Vector3> verts, ref List<int> tris) {

        int vertAmount = verts.Count;
        tris.Add(vertAmount);
        tris.Add(vertAmount + 1);
        tris.Add(vertAmount + 2);

        tris.Add(vertAmount + 2);
        tris.Add(vertAmount + 3);
        tris.Add(vertAmount);


        switch (dir) {
            case Direction.Top:
                verts.AddRange(new Vector3[] { new Vector3(x, y + 1, z), new Vector3(x, y + 1, z + 1), new Vector3(x + 1, y + 1, z + 1), new Vector3(x + 1, y + 1, z) });
                break;
            case Direction.Bottom:
                verts.AddRange(new Vector3[] { new Vector3(x, y, z + 1), new Vector3(x, y, z), new Vector3(x + 1, y, z), new Vector3(x + 1, y, z + 1) });
                break;
            case Direction.North:
                verts.AddRange(new Vector3[] { new Vector3(x + 1, y, z + 1), new Vector3(x + 1, y + 1, z + 1), new Vector3(x, y + 1, z + 1), new Vector3(x, y, z + 1) });
                break;
            case Direction.South:
                verts.AddRange(new Vector3[] { new Vector3(x, y, z), new Vector3(x, y + 1, z), new Vector3(x + 1, y + 1, z), new Vector3(x + 1, y, z) });
                break;
            case Direction.East:
                verts.AddRange(new Vector3[] { new Vector3(x + 1, y, z), new Vector3(x + 1, y + 1, z), new Vector3(x + 1, y + 1, z + 1), new Vector3(x + 1, y, z + 1) });
                break;
            case Direction.West:
                verts.AddRange(new Vector3[] { new Vector3(x, y, z + 1), new Vector3(x, y + 1, z + 1), new Vector3(x, y + 1, z), new Vector3(x, y, z) });
                break;

        }
    }

    private void HandleGenMap(LocalPacket packet) {

        byte[, ,] map = new byte[Constants.ChunkWidth, Constants.ChunkHeight, Constants.ChunkWidth];

        for (int x = 0; x < Constants.ChunkWidth; x++) {
            for (int y = 0; y < Constants.ChunkHeight; y++) {
                for (int z = 0; z < Constants.ChunkWidth; z++) {

                    if (Noise.Generate((x + packet.ChunkRef.Pos.x) / 50f, y / 50f, (z + packet.ChunkRef.Pos.y) / 50f) > 0) {
                        map[x, y, z] = 1;
                    }

                    /*   int noiseHeight = (int)((Noise.Generate((x + packet.ChunkRef.Pos.x) / 100f, (z + packet.ChunkRef.Pos.y) / 100f) + 1f) * 10);
                   map[x, noiseHeight, z] = 1;*/
                    //    map[x, 1, z] = 1;
                }
            }
        }

        for (int x = 0; x < Constants.ChunkWidth; x++) {
            for (int z = 0; z < Constants.ChunkWidth; z++) {
                map[x, 0, z] = 1;
                map[x, Constants.ChunkHeight - 1, z] = 1;
            }
        }
        //map[8, 8, 8] = 1;

        packet.Map = map;
        packet.ChunkRef.IncommingPackets.Enqueue(packet);

    }

    private void HandleNeedChunkPoses(LocalPacket packet) {
        int playerChunkPosX = TerrainGen.LastPlayerChunkCoordPos.x;
        int playerChunkPosZ = TerrainGen.LastPlayerChunkCoordPos.y;
        int minX = (playerChunkPosX - Constants.ViewRadiusChunk - 2) * Constants.ChunkWidth;
        int maxX = (playerChunkPosX + Constants.ViewRadiusChunk + 2) * Constants.ChunkWidth;
        int minZ = (playerChunkPosZ - Constants.ViewRadiusChunk - 2) * Constants.ChunkWidth;
        int maxZ = (playerChunkPosZ + Constants.ViewRadiusChunk + 2) * Constants.ChunkWidth;

        List<Chunk> chunksToDestroy = new List<Chunk>();

        for (int i = 0; i < Chunk.Chunks.Count; i++) {
            Chunk pair = Chunk.Chunks[i];
            if (pair.Pos.x < minX || pair.Pos.x > maxX || pair.Pos.y < minZ || pair.Pos.y > maxZ) {
                chunksToDestroy.Add(pair);
            }
        }

        List<Vector2I> chunksToMake = new List<Vector2I>();

        Chunk aChunk = null;
        bool didFind = false;
        for (int x = (playerChunkPosX - Constants.ViewRadiusChunk) * Constants.ChunkWidth; x < (playerChunkPosX + Constants.ViewRadiusChunk) * Constants.ChunkWidth; x += Constants.ChunkWidth) {
            for (int z = (playerChunkPosZ - Constants.ViewRadiusChunk) * Constants.ChunkWidth; z < (playerChunkPosZ + Constants.ViewRadiusChunk) * Constants.ChunkWidth; z += Constants.ChunkWidth) {
                /*if (!Chunk.Chunks.TryGetValue(new Vector2(x, z), out aChunk)) {
                    chunksToMake.Add(new Vector2I(x, z));
                }*/
                didFind = false;

                for (int i = 0; i < Chunk.Chunks.Count; i++) {
                    Chunk chunk = Chunk.Chunks[i];
                    if (chunk.Pos.x == x && chunk.Pos.y == z) {
                        didFind = true;
                        break;
                    }
                }

                if (!didFind) {
                    chunksToMake.Add(new Vector2I(x, z));
                }

            }
        }

        chunksToMake.Sort((a, b) => Distance2IntsCoords(a.x, a.y, playerChunkPosX, playerChunkPosZ).CompareTo(Distance2IntsCoords(b.x, b.y, playerChunkPosX, playerChunkPosZ)));

        while (chunksToMake.Count > Constants.AntalMaxChunksPerPortion) {
            chunksToMake.RemoveAt(chunksToMake.Count - 1);
        }

        packet.ChunksToCreate = chunksToMake;
        packet.ChunksToRemove = chunksToDestroy;

        TerrainGen.IncommingPackets.Enqueue(packet);
    }

    public void StopThread() {
        Run = false;
    }

    public void QueueueueueWork(LocalPacket packet, Speed speed) {
        switch (speed) {
            case Speed.Normal:
                IncommingNormalSpeed.Enqueue(packet);
                break;
            case Speed.Fast:
                IncommingFastSpeed.Enqueue(packet);
                break;
            case Speed.Faster:
                IncommingFasterSpeed.Enqueue(packet);
                break;
            default:
                throw new ArgumentOutOfRangeException("speed", speed, null);
        }
        WaitHandle.Set();
    }
    public int Distance2IntsCoords(int x1, int x2, int z1, int z2) {
        return (int)Math.Sqrt(Math.Pow((x1 - z1), 2) + Math.Pow((x2 - z2), 2));
    }
}
