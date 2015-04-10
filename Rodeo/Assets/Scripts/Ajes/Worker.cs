using System;
using UnityEngine;
using System.Collections;
using System.Threading;
using PlaynomicsPlugin;

public class Worker {

    private bool Run = true;
    private ConcurrentQueue<LocalPacket> IncommingNormalSpeed = new ConcurrentQueue<LocalPacket>();
    private ConcurrentQueue<LocalPacket> IncommingFastSpeed = new ConcurrentQueue<LocalPacket>();
    private readonly AutoResetEvent WaitHandle = new AutoResetEvent(false);

    public void Go() {



        while (Run) {

            LocalPacket packet = null;
            if (IncommingNormalSpeed.Count > 0) {
                packet = IncommingNormalSpeed.Dequeue();
            }
            else if (IncommingFastSpeed.Count > 0) {
                packet = IncommingFastSpeed.Dequeue();
            }

            if (ReferenceEquals(null, packet)) {
                WaitHandle.WaitOne();
            }
            else {

                switch (packet.Type) {
                    case PacketType.NeedChunkPoses:

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


    }

    public void StopThread() {
        Run = false;
    }


}
