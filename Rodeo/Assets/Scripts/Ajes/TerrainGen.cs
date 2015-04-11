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
    public Material ChunkMaterial;
    public static TerrainGen TerrainGenAjesSingletongVildhedRoflKartofel;
    private bool F5IsPressed = false;
    public static Texture2D RedTexture;
    public static Texture2D YellowTexture;
    public static Texture2D GreenTexture;
    private bool OrderNewChunksWhenThisIsTrue = true;


    void Start() {

        Constants.WorkerAmount = Environment.ProcessorCount - 2;

        RedTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        RedTexture.SetPixel(0, 0, Color.red);
        RedTexture.Apply();
        YellowTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        YellowTexture.SetPixel(0, 0, Color.yellow);
        YellowTexture.Apply();
        GreenTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        GreenTexture.SetPixel(0, 0, new Color(0f, 1f, 0f, 0.5f));
        GreenTexture.Apply();

        TerrainGenAjesSingletongVildhedRoflKartofel = this;

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

        Application.targetFrameRate = 180;




    }

    void OnGUI() {

        if (F5IsPressed) {
            try {
                int xStart = Screen.width / 2;
                int yStart = Screen.height / 2;
                for (int i = Chunk.Chunks.Count - 1; i >= 0; i--) {
                    Chunk chunk = Chunk.Chunks[i];
                    if (chunk.gotFirstFaceRun) {
                        GUI.DrawTexture(new Rect(xStart - ThePlayer.transform.position.x + chunk.Pos.x, yStart + ThePlayer.transform.position.z - chunk.Pos.y, Constants.ChunkWidth, Constants.ChunkWidth), GreenTexture);
                    }
                    else if (chunk.Map != null) {
                        GUI.DrawTexture(new Rect(xStart - ThePlayer.transform.position.x + chunk.Pos.x, yStart + ThePlayer.transform.position.z - chunk.Pos.y, Constants.ChunkWidth, Constants.ChunkWidth), YellowTexture);
                    }
                    else {
                        GUI.DrawTexture(new Rect(xStart - ThePlayer.transform.position.x + chunk.Pos.x, yStart + ThePlayer.transform.position.z - chunk.Pos.y, Constants.ChunkWidth, Constants.ChunkWidth), RedTexture);
                    }
                }
            }
            catch (Exception e) {
                Debug.Log("f5 problem.. : " + e);
            }
        }


        AjesGuiLabel(new Rect(0, 0, 200, 20), "Memory: " + GC.GetTotalMemory(false) / 1024000 + " MB");
        AjesGuiLabel(new Rect(0, 20, 200, 20), "Chunks.Count: " + Chunk.Chunks.Count);

        GUI.skin.label.alignment = TextAnchor.UpperRight;
        for (int i = 0; i < Workers.Count; i++) {
            Worker worker = Workers[i];
            AjesGuiLabel(new Rect(Screen.width - 250, 20 * i, 250, 20), worker.DoingAtm.ToString() + " <- worker" + i);
        }
        GUI.skin.label.alignment = TextAnchor.UpperLeft;

    }

    void Update() {

        CurrentPlayerChunkCoordPos.x = Mathf.FloorToInt(ThePlayer.transform.position.x / Constants.ChunkWidth);
        CurrentPlayerChunkCoordPos.y = Mathf.FloorToInt(ThePlayer.transform.position.z / Constants.ChunkWidth);
        if (CurrentPlayerChunkCoordPos.x != LastPlayerChunkCoordPos.x || CurrentPlayerChunkCoordPos.y != LastPlayerChunkCoordPos.y) {
            OnChunkBorderPass();
        }
        LastPlayerChunkCoordPos = CurrentPlayerChunkCoordPos;


        if (OrderNewChunksWhenThisIsTrue && Chunk.AChunkAllreadyUsedAGenMeshThisFrame == false) {
            OrderNewChunksWhenThisIsTrue = false;
            OrderNewChunkPoses();
        }

        Chunk.AChunkAllreadyUsedAGenMeshThisFrame = false;


        HandleIncommingPackets();

        if (Input.GetKeyDown(KeyCode.F5)) {
            F5IsPressed = !F5IsPressed;
        }

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
                        go.transform.parent = this.transform;
                        Chunk.Chunks.Add(daChunk);
                    }
                    waitingOnChunkPosesFromWorker = false;
                    if (packet.ChunksToCreate.Count > 0) {
                        OrderNewChunksWhenThisIsTrue = true;
                    }


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
        if (!waitingOnChunkPosesFromWorker) {
            waitingOnChunkPosesFromWorker = true;
            LocalPacket packet = new LocalPacket();
            packet.Type = PacketType.NeedChunkPoses;
            SendPacketToWorker(packet, Speed.Normal);
        }
    }

    public static void SendPacketToWorker(LocalPacket packet, Speed speed) {
        Workers[WhichWorkerToTake].QueueueueueWork(packet, speed);
        WhichWorkerToTake++;
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

    public static void AjesGuiLabel(Rect rect, string s) {
        GUI.color = Color.black;
        GUI.Label(new Rect(rect.x + 1, rect.y + 1, rect.width, rect.height), s);
        GUI.color = Color.white;
        GUI.Label(rect, s);
    }

}
