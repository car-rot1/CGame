using System.Collections.Generic;
using System.Linq;
using CGame;
using Sirenix.OdinInspector;
using UnityEngine;

public class WaveBlockInfo
{
    public readonly Vector2Int position;
    public bool isSet;
    public readonly List<WaveBlock> waveBlocks;

    public WaveBlockInfo(Vector2Int position, List<WaveBlock> waveBlocks)
    {
        this.position = position;
        this.waveBlocks = waveBlocks;
    }
}

public class WaveMap : MonoBehaviour, ISerializationCallbackReceiver
{
    public int width, height;
    public int xOffset, yOffset;
    public WaveBlock[] allWaveBlock;
    
    private Dictionary<int, List<WaveBlock>> upWaveBlockDic;
    private Dictionary<int, List<WaveBlock>> rightWaveBlockDic;
    private Dictionary<int, List<WaveBlock>> downWaveBlockDic;
    private Dictionary<int, List<WaveBlock>> leftWaveBlockDic;
    private Dictionary<int, List<WaveBlock>> GetWaveBlockDic(Direction direction)
    {
        return direction switch
        {
            Direction.Up => upWaveBlockDic,
            Direction.Right => rightWaveBlockDic,
            Direction.Down => downWaveBlockDic,
            Direction.Left => leftWaveBlockDic,
            _ => null
        };
    }

    public WaveBlockInfo[,] allWaveBlockInfo;
    public Transform waveBlockInfoPrefab;
    private Transform[,] allWaveBlockInfoGameObject;

    private void Awake()
    {
        Init();
    }

    public bool _init;
    public int seed;
    public bool lockSeed;
    
    [Button]
    private void Init()
    {
        if (_init)
            return;
        _init = true;

        upWaveBlockDic = new Dictionary<int, List<WaveBlock>>();
        rightWaveBlockDic = new Dictionary<int, List<WaveBlock>>();
        downWaveBlockDic = new Dictionary<int, List<WaveBlock>>();
        leftWaveBlockDic = new Dictionary<int, List<WaveBlock>>();
        
        foreach (var waveBlock in allWaveBlock)
        {
            foreach (var waveBlockDirectionIdInfo in waveBlock.directionIdInfos)
            {
                var dic = GetWaveBlockDic(waveBlockDirectionIdInfo.direction);
                dic.TryAdd(waveBlockDirectionIdInfo.id, new List<WaveBlock>());
                dic[waveBlockDirectionIdInfo.id].Add(waveBlock);
            }
        }
        
        foreach (var waveBlock in allWaveBlock)
        {
            waveBlock.Init();
        }

        RefreshWaveBlockInfo();
        
        Debug.Log("Init Finish");
    }

    private void RefreshWaveBlockInfo()
    {
        if (!lockSeed)
        {
            seed = new System.Random().Next(int.MinValue, int.MaxValue);
        }
        Random.InitState(seed);

        allWaveBlockInfo = new WaveBlockInfo[width, height];
        allWaveBlockInfoGameObject = new Transform[width, height];
        for (var i = 0; i < allWaveBlockInfo.GetLength(0); i++)
        {
            for (var j = 0; j < allWaveBlockInfo.GetLength(1); j++)
            {
                allWaveBlockInfo[i, j] = new WaveBlockInfo(new Vector2Int(i, j), new List<WaveBlock>(allWaveBlock));
                allWaveBlockInfoGameObject[i, j] = Instantiate(waveBlockInfoPrefab, transform);
                allWaveBlockInfoGameObject[i, j].position = new Vector3(i * xOffset, j * yOffset);
            }
        }

        for (var i = 0; i < allWaveBlockInfo.GetLength(0); i++)
        {
            for (var j = 0; j < allWaveBlockInfo.GetLength(1); j++)
            {
                var removeList = new HashSet<WaveBlock>();
                if (i == 0)
                {
                    foreach (var (id, waveBlocks) in leftWaveBlockDic)
                    {
                        if (id == -1)
                            continue;
                        foreach (var waveBlock in waveBlocks)
                        {
                            removeList.Add(waveBlock);
                        }
                    }
                }
                else if (i == width - 1)
                {
                    foreach (var (id, waveBlocks) in rightWaveBlockDic)
                    {
                        if (id == -1)
                            continue;
                        foreach (var waveBlock in waveBlocks)
                        {
                            removeList.Add(waveBlock);
                        }
                    }
                }

                if (j == 0)
                {
                    foreach (var (id, waveBlocks) in downWaveBlockDic)
                    {
                        if (id == -1)
                            continue;
                        foreach (var waveBlock in waveBlocks)
                        {
                            removeList.Add(waveBlock);
                        }
                    }
                }
                else if (j == height - 1)
                {
                    foreach (var (id, waveBlocks) in upWaveBlockDic)
                    {
                        if (id == -1)
                            continue;
                        foreach (var waveBlock in waveBlocks)
                        {
                            removeList.Add(waveBlock);
                        }
                    }
                }

                foreach (var waveBlock in removeList)
                {
                    allWaveBlockInfo[i, j].waveBlocks.Remove(waveBlock);
                }
                
                if (allWaveBlockInfo[i, j].waveBlocks.Count == 1)
                {
                    var waveBlock = Instantiate(allWaveBlockInfo[i, j].waveBlocks[0], transform);
                    waveBlock.gameObject.transform.position = new Vector3(i * xOffset, j * yOffset);
                
                    allWaveBlockInfo[i, j].isSet = true;
                }

                if (allWaveBlockInfo[i, j].isSet)
                {
                    allWaveBlockInfoGameObject[i, j].gameObject.SetActive(false);
                }
                else
                {
                    foreach (Transform o in allWaveBlockInfoGameObject[i, j].transform)
                    {
                        if (o.TryGetComponent<WaveBlock>(out var w))
                        {
                            o.gameObject.SetActive(false);
                            if (allWaveBlockInfo[i, j].waveBlocks.Any(waveBlock => w.name == waveBlock.name))
                            {
                                o.gameObject.SetActive(true);
                            }
                        }
                    }
                }
                ChangeAllBlock(allWaveBlockInfo[i, j]);
            }
        }
    }

    [Button]
    public void All()
    {
        Init();
        Clear();
        RefreshWaveBlockInfo();
        
        for (var i = 0; i < width; i++)
        {
            SetBlock(i, 0);
            SetBlock(i, height - 1);
        }

        for (var j = 0; j < height; j++)
        {
            SetBlock(0, j);
            SetBlock(width - 1, j);
        }
        
        for (var i = 1; i < width - 1; i++)
        for (var j = 1; j < height - 1; j++)
        {
            SetBlock(i, j);
        }
    }

    [Button]
    private void SetBlock(int i, int j)
    {
        Init();
        if (allWaveBlockInfo[i, j].isSet)
            return;

        if (allWaveBlockInfo[i, j].waveBlocks.Count <= 0)
            return;

        var waveBlockPrefab = allWaveBlockInfo[i, j].waveBlocks.RandomItem();
        var waveBlock = Instantiate(waveBlockPrefab, transform);
        waveBlock.gameObject.transform.position = new Vector3(i * xOffset, j * yOffset);

        allWaveBlockInfo[i, j].isSet = true;
        allWaveBlockInfo[i, j].waveBlocks.Clear();
        allWaveBlockInfo[i, j].waveBlocks.Add(waveBlockPrefab);

        allWaveBlockInfoGameObject[i, j].gameObject.SetActive(false);
        ChangeAllBlock(allWaveBlockInfo[i, j]);
    }

    private void ChangeAllBlock(WaveBlockInfo start)
    {
        var stack = new Stack<WaveBlockInfo>();
        stack.Push(start);

        while (stack.Count > 0)
        {
            var v = stack.Pop();
            foreach (Direction direction in Direction.MainFour)
            {
                var targetV = v.position + direction.ToVector();
                if (targetV.x < 0 || targetV.x >= width || targetV.y < 0 || targetV.y >= height)
                    continue;
                
                var waveBlockInfo = allWaveBlockInfo[targetV.x, targetV.y];
                if (waveBlockInfo.isSet)
                    continue;

                var removeList = new List<WaveBlock>(waveBlockInfo.waveBlocks);
                foreach (var waveBlock in v.waveBlocks)
                {
                    if (!waveBlock.directionIdDic.TryGetValue(direction, out var directionId))
                        continue;

                    var dic = GetWaveBlockDic(direction.Reverse());
                    foreach (var block in dic[directionId])
                    {
                        removeList.Remove(block);
                    }
                }
                if (removeList.Count <= 0)
                    continue;
                
                foreach (var waveBlock in removeList)
                    waveBlockInfo.waveBlocks.Remove(waveBlock);

                if (waveBlockInfo.waveBlocks.Count == 1)
                {
                    var waveBlock = Instantiate(waveBlockInfo.waveBlocks[0], transform);
                    waveBlock.gameObject.transform.position = new Vector3(targetV.x * xOffset, targetV.y * yOffset);
                    waveBlockInfo.isSet = true;
                }

                if (waveBlockInfo.isSet)
                {
                    allWaveBlockInfoGameObject[targetV.x, targetV.y].gameObject.SetActive(false);
                }
                else
                {
                    foreach (Transform o in allWaveBlockInfoGameObject[targetV.x, targetV.y].transform)
                    {
                        if (o.TryGetComponent<WaveBlock>(out var w))
                        {
                            o.gameObject.SetActive(false);
                            if (allWaveBlockInfo[targetV.x, targetV.y].waveBlocks.Any(waveBlock => w.name == waveBlock.name))
                            {
                                o.gameObject.SetActive(true);
                            }
                        }
                    }
                }
                
                stack.Push(waveBlockInfo);
            }
        }
    }
    
    [Button]
    public void Clear()
    {
        var list = new List<GameObject>();
        foreach (Transform o in transform)
        {
            list.Add(o.gameObject);
        }
        foreach (var o in list)
        {
            DestroyImmediate(o);
        }
    }

    public void OnBeforeSerialize()
    {
        
    }

    public void OnAfterDeserialize()
    {
        _init = false;
    }
}
