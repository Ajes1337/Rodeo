using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using PlaynomicsPlugin;

public class TerrainGen : MonoBehaviour {

    private static readonly List<Worker> Workers = new List<Worker>();
    private readonly List<Thread> WorkerThreads = new List<Thread>();
    private static int WhichWorkerToTake;
    private GameObject ThePlayer;
    private Vector2I CurrentPlayerChunkCoordPos;
    public static Vector2I LastPlayerChunkCoordPos;
    public static ConcurrentQueue<LocalPacket> IncommingPackets = new ConcurrentQueue<LocalPacket>();
    public static bool waitingOnChunkPosesFromWorker = false;

    void Start() {

        WhichWorkerToTake = 0;
        IncommingPackets.Clear();

        for (int i = 0; i < Constants.WorkerAmount; i++) {
            Worker workerObject = new Worker();
            Thread workerThread = new Thread(workerObject.Go);
            workerThread.Start();
            Workers.Add(workerObject);
            WorkerThreads.Add(workerThread);
        }



        ThePlayer = GameObject.FindWithTag("Player");

        Application.targetFrameRate = 60;

        OrderNewChunkPoses();

    }

    void OnGUI() {
        GUI.Label(new Rect(50, 50, 200, 50), "Chunks.Count: " + Chunk.Chunks.Count);

    }

    void Update() {

        CurrentPlayerChunkCoordPos.x = Mathf.FloorToInt(ThePlayer.transform.position.x / Constants.ChunkWidth);
        CurrentPlayerChunkCoordPos.y = Mathf.FloorToInt(ThePlayer.transform.position.z / Constants.ChunkWidth);
        if (CurrentPlayerChunkCoordPos.x != LastPlayerChunkCoordPos.x || CurrentPlayerChunkCoordPos.y != LastPlayerChunkCoordPos.y) {
            OnChunkBorderPass();
        }
        LastPlayerChunkCoordPos = CurrentPlayerChunkCoordPos;




        HandleIncommingPackets();


    }

    private void HandleIncommingPackets() {

        if (IncommingPackets.Count > 0) {
            LocalPacket packet = IncommingPackets.Dequeue();

            switch (packet.Type) {
                case PacketType.NeedChunkPoses:


                    foreach (Vector2I vector2I in packet.ChunksToCreate) {
                        GameObject go = new GameObject();
                        go.transform.position = new Vector3(vector2I.x, 0, vector2I.y);
                        Chunk daChunk = go.AddComponent<Chunk>();
                        daChunk.Pos = vector2I;
                        Chunk.Chunks.Add(daChunk);
                    }

                    if (packet.ChunksToCreate.Count > 0) {
                        OrderNewChunkPoses();
                    }

                    waitingOnChunkPosesFromWorker = false;

                    for (int i = packet.ChunksToRemove.Count - 1; i >= 0; i--) {
                        Chunk chunk = packet.ChunksToRemove[i];
                        chunk.DestroySoon();
                    }

                    break;
                case PacketType.GenMap:

                    break;
                case PacketType.GenMesh:

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


        }

    }

    void OnChunkBorderPass() {
        OrderNewChunkPoses();
    }

    void OrderNewChunkPoses() {
        Debug.Log("check for new chunk poses");
        waitingOnChunkPosesFromWorker = true;
        LocalPacket packet = new LocalPacket();
        packet.Type = PacketType.NeedChunkPoses;
        SendPacketToWorker(packet, Speed.Normal);
    }

    public static void SendPacketToWorker(LocalPacket packet, Speed speed) {
        Workers[WhichWorkerToTake].QueueueueueWork(packet, speed);
        if (WhichWorkerToTake >= Constants.WorkerAmount) {
            WhichWorkerToTake = 0;
        }
    }

    void OnApplicationQuit() {
        Debug.Log("teeeest onappquit");
        foreach (Worker worker in Workers) {
            worker.StopThread();
            worker.WaitHandle.Set();
        }
        Debug.Log("venter på worker threads bliver færdige");
        foreach (Thread thread in WorkerThreads) {
            thread.Join();
        }
        Debug.Log("nu er alle worker threads færdige");
    }

}
