using System;
using System.Collections.Generic;
using CGame;
using UnityEngine;

[Serializable]
public struct DirectionIdInfo
{
    public Direction direction;
    public int id;

    public DirectionIdInfo(Direction direction, int id)
    {
        this.direction = direction;
        this.id = id;
    }
}

[RequireComponent(typeof(SpriteRenderer))]
public class WaveBlock : MonoBehaviour
{
    public Sprite sprite;
    public List<DirectionIdInfo> directionIdInfos;
    public Dictionary<Direction, int> directionIdDic;

    [CButton]
    public void Reset()
    {
        sprite = GetComponent<SpriteRenderer>().sprite;
    }
    
    public void Init()
    {
        directionIdDic = new Dictionary<Direction, int>();
        foreach (var directionIdInfo in directionIdInfos)
        {
            directionIdDic.Add(directionIdInfo.direction, directionIdInfo.id);
        }
    }
}
