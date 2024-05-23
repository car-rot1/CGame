using System;
using System.Collections.Generic;
using CGame;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public struct ConnectWaveBlockInfo
{
    public Direction connectDirection;
    public WaveBlock[] connectWaveBlocks;
}

[Serializable]
public struct WaveBlockConnectInfo
{
    public WaveBlock waveBlock;
    public ConnectWaveBlockInfo[] connectWaveBlockInfos;
}

public class CreateWaveId : MonoBehaviour
{
    public int startId;
    public WaveBlock[] allWaveBlock;
    public WaveBlockConnectInfo[] waveBlockConnectInfos;

    [Button]
    public void SetWaveBlockId()
    {
        foreach (var waveBlock in allWaveBlock)
        {
            waveBlock.directionIdInfos ??= new List<DirectionIdInfo>();
            waveBlock.directionIdInfos.Clear();
            foreach (Direction direction in Direction.MainFour)
            {
                waveBlock.directionIdInfos.Add(new DirectionIdInfo { direction = direction, id = -1 });
            }
        }
        
        foreach (var waveBlockConnectInfo in waveBlockConnectInfos)
        {
            foreach (var connectWaveBlockInfo in waveBlockConnectInfo.connectWaveBlockInfos)
            {
                var needAdd = false;
                var currentIdInfo = waveBlockConnectInfo.waveBlock.directionIdInfos.Find(d => d.direction == connectWaveBlockInfo.connectDirection);
                foreach (var connectWaveBlock in connectWaveBlockInfo.connectWaveBlocks)
                {
                    var connectIdInfo = connectWaveBlock.directionIdInfos.Find(d => d.direction == connectWaveBlockInfo.connectDirection.Reverse());
                    if (currentIdInfo.id == -1)
                    {
                        currentIdInfo.id = startId;
                        needAdd = true;
                    }
                    connectIdInfo.id = currentIdInfo.id;
                }

                if (needAdd)
                    startId++;
            }
        }
    }
}