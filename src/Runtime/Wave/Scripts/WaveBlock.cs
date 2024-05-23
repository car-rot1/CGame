using System;
using System.Collections.Generic;
using CGame;
using UnityEngine;

[Serializable]
public class DirectionIdInfo
{
    public Direction direction;
    public int id;
}

public class WaveBlock : MonoBehaviour
{
    public List<DirectionIdInfo> directionIdInfos;
    public Dictionary<Direction, int> directionIdDic;

    public void Init()
    {
        directionIdDic = new Dictionary<Direction, int>();
        foreach (var directionIdInfo in directionIdInfos)
        {
            directionIdDic.Add(directionIdInfo.direction, directionIdInfo.id);
        }
    }
}
