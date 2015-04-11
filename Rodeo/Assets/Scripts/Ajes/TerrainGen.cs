using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using PlaynomicsPlugin;

public class TerrainGen : MonoBehaviour {



    private readonly List<Worker> Workers = new List<Worker>();
    private readonly List<Thread> WorkerThreads = new List<Thread>();
    private GameObject ThePlayer;
    private Vector2I CurrentChunkCoordPos;
    private Vector2I LastPlayerChunkCoordPos;
    private List<Vector2I> incommingNewChunkPoses = new List<Vector2I>();
    public static ConcurrentQueue<LocalPacket> IncommingPackets = new ConcurrentQueue<LocalPacket>();

    void Start() {

        for (int i = 0; i < Constants.WorkerAmount; i++) {
            Worker workerObject = new Worker();
            Thread workerThread = new Thread(workerObject.Go);
            workerThread.Start();
            Workers.Add(workerObject);
            WorkerThreads.Add(workerThread);
        }



        ThePlayer = GameObject.FindWithTag("Player");

        Application.targetFrameRate = 60;

    }


    void Update() {

        CurrentChunkCoordPos.x = Mathf.FloorToInt(ThePlayer.transform.position.x / Constants.ChunkWidth);
        CurrentChunkCoordPos.y = Mathf.FloorToInt(ThePlayer.transform.position.z / Constants.ChunkWidth);
        if (CurrentChunkCoordPos.x != LastPlayerChunkCoordPos.x || CurrentChunkCoordPos.y != LastPlayerChunkCoordPos.y) {
            OnChunkBorderPass();
        }
        LastPlayerChunkCoordPos = CurrentChunkCoordPos;


        Debug.Log(ThePlayer.transform.position + " blabla: " + CurrentChunkCoordPos.x + " " + CurrentChunkCoordPos.y);







    }

    void OnChunkBorderPass() {

        LocalPacket packet = new LocalPacket();
        packet.Type = PacketType.NeedChunkPoses;




    }

    void OnApplicationQuit() {
        Debug.Log("teeeest onappquit");
        foreach (Worker worker in Workers) {
            worker.StopThread();
        }
        Debug.Log("venter på worker threads bliver færdige");
        foreach (Thread thread in WorkerThreads) {
            thread.Join();
        }
        Debug.Log("nu er alle worker threads færdige");
    }

}
