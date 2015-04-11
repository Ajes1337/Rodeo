using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlaynomicsPlugin;

public class Chunk : MonoBehaviour {

    public static List<Chunk> Chunks = new List<Chunk>();
    public Vector2I Pos;
    public ConcurrentQueue<LocalPacket> IncommingPackets = new ConcurrentQueue<LocalPacket>();
    public byte[, ,] Map;
    private Chunk NorthChunk;
    private Chunk SouthChunk;
    private Chunk EastChunk;
    private Chunk WestChunk;
    private Chunk NeChunk;
    private Chunk SeChunk;
    private Chunk SwChunk;
    private Chunk NwChunk;
    public bool gotFirstFaceRun;
    private MeshFilter filter;
    private MeshCollider colliiiider;
    private bool destroySoon = false;
    private int isBeingUsedInAnotherThread;

    public void Start() {

        LocalPacket packet = new LocalPacket();
        packet.Type = PacketType.GenMap;
        packet.ChunkRef = this;
        TerrainGen.SendPacketToWorker(packet, Speed.Fast);
        isBeingUsedInAnotherThread++;


        filter = gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        colliiiider = gameObject.AddComponent<MeshCollider>();
        GetComponent<Renderer>().material = TerrainGen.TerrainGenAjesSingletongVildhedRoflKartofel.ChunkMaterial;






    }


    void Update() {

        HandleIncommingPackets();

        HandleSelfDestroying();

    }

    private void HandleIncommingPackets() {

        if (IncommingPackets.Count > 0) {
            LocalPacket packet = IncommingPackets.Dequeue();

            switch (packet.Type) {
                case PacketType.GenMap:
                    Map = packet.Map;
                    UpdateChunkNeighborRelation();
                    CheckIfGotAllNeighborsAndMap();
                    isBeingUsedInAnotherThread--;

                    break;
                case PacketType.GenMesh:



                    Mesh aMesh = new Mesh();
                    aMesh.vertices = packet.verts;
                    aMesh.triangles = packet.tris;
                    aMesh.colors32 = packet.colors;

                    aMesh.RecalculateBounds();
                    aMesh.RecalculateNormals();
                    aMesh.Optimize();

                    filter.mesh = aMesh;
                    colliiiider.sharedMesh = aMesh;

                    isBeingUsedInAnotherThread--;

                    NorthChunk.isBeingUsedInAnotherThread--;
                    EastChunk.isBeingUsedInAnotherThread--;
                    SouthChunk.isBeingUsedInAnotherThread--;
                    WestChunk.isBeingUsedInAnotherThread--;
                    NeChunk.isBeingUsedInAnotherThread--;
                    SeChunk.isBeingUsedInAnotherThread--;
                    SwChunk.isBeingUsedInAnotherThread--;
                    NwChunk.isBeingUsedInAnotherThread--;

                    gotFirstFaceRun = true;

                    break;
            }
        }

    }


    void BuildFaces() {
        isBeingUsedInAnotherThread++;
        NorthChunk.isBeingUsedInAnotherThread++;
        EastChunk.isBeingUsedInAnotherThread++;
        SouthChunk.isBeingUsedInAnotherThread++;
        WestChunk.isBeingUsedInAnotherThread++;
        NeChunk.isBeingUsedInAnotherThread++;
        SeChunk.isBeingUsedInAnotherThread++;
        SwChunk.isBeingUsedInAnotherThread++;
        NwChunk.isBeingUsedInAnotherThread++;

        LocalPacket packet = new LocalPacket();
        packet.Type = PacketType.GenMesh;
        packet.ChunkRef = this;
        TerrainGen.SendPacketToWorker(packet, Speed.Faster);
    }

    public byte GetMap(int x, int y, int z) {

        if (x < 0) {
            return WestChunk.GetMap(x + Constants.ChunkWidth, y, z);
        }
        if (x >= Constants.ChunkWidth) {
            return EastChunk.GetMap(x - Constants.ChunkWidth, y, z);
        }
        if (z < 0) {
            return SouthChunk.GetMap(x, y, z + Constants.ChunkWidth);
        }
        if (z >= Constants.ChunkWidth) {
            return NorthChunk.GetMap(x, y, z - Constants.ChunkWidth);
        }
        if (y < 0) {
            return 1;
        }
        if (y >= Constants.ChunkHeight) {
            return 1;
        }
        return Map[x, y, z];

    }


    private void UpdateChunkNeighborRelation() {

        bool northWasFound = false;
        bool eastWasFound = false;
        bool westWasFound = false;
        bool southWasFound = false;
        bool neWasFound = false;
        bool seWasFound = false;
        bool swWasFound = false;
        bool nwWasFound = false;

        foreach (Chunk chunk in Chunks) {

            if (!northWasFound && chunk.Pos.x == Pos.x && chunk.Pos.y == Pos.y + Constants.ChunkWidth && chunk.Map != null) {
                northWasFound = true;
                NorthChunk = chunk;
                NorthChunk.SouthChunk = this;
                NorthChunk.CheckIfGotAllNeighborsAndMap();
            }
            else if (!southWasFound && chunk.Pos.x == Pos.x && chunk.Pos.y == Pos.y - Constants.ChunkWidth && chunk.Map != null) {
                southWasFound = true;
                SouthChunk = chunk;
                SouthChunk.NorthChunk = this;
                SouthChunk.CheckIfGotAllNeighborsAndMap();
            }
            else if (!eastWasFound && chunk.Pos.x == Pos.x + Constants.ChunkWidth && chunk.Pos.y == Pos.y && chunk.Map != null) {
                eastWasFound = true;
                EastChunk = chunk;
                EastChunk.WestChunk = this;
                EastChunk.CheckIfGotAllNeighborsAndMap();
            }
            else if (!westWasFound && chunk.Pos.x == Pos.x - Constants.ChunkWidth && chunk.Pos.y == Pos.y && chunk.Map != null) {
                westWasFound = true;
                WestChunk = chunk;
                WestChunk.EastChunk = this;
                WestChunk.CheckIfGotAllNeighborsAndMap();
            }
            else if (!neWasFound && chunk.Pos.x == Pos.x + Constants.ChunkWidth && chunk.Pos.y == Pos.y + Constants.ChunkWidth && chunk.Map != null) {
                neWasFound = true;
                NeChunk = chunk;
                NeChunk.SwChunk = this;
                NeChunk.CheckIfGotAllNeighborsAndMap();
            }
            else if (!seWasFound && chunk.Pos.x == Pos.x + Constants.ChunkWidth && chunk.Pos.y == Pos.y - Constants.ChunkWidth && chunk.Map != null) {
                seWasFound = true;
                SeChunk = chunk;
                SeChunk.NwChunk = this;
                SeChunk.CheckIfGotAllNeighborsAndMap();
            }
            else if (!swWasFound && chunk.Pos.x == Pos.x - Constants.ChunkWidth && chunk.Pos.y == Pos.y - Constants.ChunkWidth && chunk.Map != null) {
                swWasFound = true;
                SwChunk = chunk;
                SwChunk.NeChunk = this;
                SwChunk.CheckIfGotAllNeighborsAndMap();
            }
            else if (!nwWasFound && chunk.Pos.x == Pos.x - Constants.ChunkWidth && chunk.Pos.y == Pos.y + Constants.ChunkWidth && chunk.Map != null) {
                nwWasFound = true;
                NwChunk = chunk;
                NwChunk.SeChunk = this;
                NwChunk.CheckIfGotAllNeighborsAndMap();
            }
        }

        if (!northWasFound) {
            NorthChunk = null;
        }
        if (!eastWasFound) {
            EastChunk = null;
        }
        if (!southWasFound) {
            SouthChunk = null;
        }
        if (!westWasFound) {
            WestChunk = null;
        }
        if (!neWasFound) {
            NeChunk = null;
        }
        if (!seWasFound) {
            SeChunk = null;
        }
        if (!swWasFound) {
            SwChunk = null;
        }
        if (!nwWasFound) {
            NwChunk = null;
        }
    }

    public bool CheckIfGotAllNeighborsAndMap() {
        if (!destroySoon) {
            if (NorthChunk != null && SouthChunk != null && EastChunk != null && WestChunk != null &&
                NeChunk != null && SeChunk != null && SwChunk != null && NwChunk != null &&
                NorthChunk.Map != null && SouthChunk.Map != null && EastChunk.Map != null && WestChunk.Map != null &&
                NeChunk.Map != null && SeChunk.Map != null && SwChunk.Map != null && NwChunk.Map != null && Map != null) {
                if (!gotFirstFaceRun) {
                    BuildFaces();
                }
                return true;
            }
            DestroyMeshesIfExists();
            gotFirstFaceRun = false;
        }
        return false;

    }


    void DestroyMeshesIfExists() {
        if (gotFirstFaceRun == true) {
            Destroy(GetComponent<MeshFilter>().sharedMesh);//toto måske sharedmesh her istedet
            Destroy(GetComponent<MeshCollider>().sharedMesh);
            //ReUsableStuff.GiveUsedMeshArrays(oriVerts, oriColors, oriUvs);
        }
    }

    public void DestroyThis() {



        Chunks.Remove(this);



        if (NorthChunk != null) {
            NorthChunk.SouthChunk = null;
            NorthChunk.CheckIfGotAllNeighborsAndMap();
        }
        if (SouthChunk != null) {
            SouthChunk.NorthChunk = null;
            SouthChunk.CheckIfGotAllNeighborsAndMap();
        }
        if (EastChunk != null) {
            EastChunk.WestChunk = null;
            EastChunk.CheckIfGotAllNeighborsAndMap();
        }
        if (WestChunk != null) {
            WestChunk.EastChunk = null;
            WestChunk.CheckIfGotAllNeighborsAndMap();
        }

        if (NeChunk != null) {
            NeChunk.SwChunk = null;
            NeChunk.CheckIfGotAllNeighborsAndMap();
        }
        if (SeChunk != null) {
            SeChunk.NwChunk = null;
            SeChunk.CheckIfGotAllNeighborsAndMap();
        }
        if (SwChunk != null) {
            SwChunk.NeChunk = null;
            SwChunk.CheckIfGotAllNeighborsAndMap();
        }
        if (NwChunk != null) {
            NwChunk.SeChunk = null;
            NwChunk.CheckIfGotAllNeighborsAndMap();
        }

        Destroy(this.gameObject);


    }

    public void DestroySoon() {
        destroySoon = true;


    }

    private void HandleSelfDestroying() {
        if (destroySoon == true && isBeingUsedInAnotherThread == 0 && !TerrainGen.waitingOnChunkPosesFromWorker) {
            DestroyThis();
        }
    }

}
