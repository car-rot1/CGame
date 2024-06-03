using UnityEditor;
using UnityEngine;

namespace CGame
{
    public static class WaveFunctionCollapseUtility
    {
        public static float LineWidthRate = 0.1f;
        
        public static MergeWaveBlock MergeWaveBlocks(WaveBlock[] waveBlocks, Vector2 itemSize) => MergeWaveBlocks(waveBlocks, itemSize, Vector2.one);
        
        public static MergeWaveBlock MergeWaveBlocks(WaveBlock[] waveBlocks, Vector2 itemSize, Vector2 scale)
        {
            var length = Mathf.CeilToInt(Mathf.Sqrt(waveBlocks.Length));
            return MergeWaveBlocks(waveBlocks, itemSize, length, length, scale);
        }
        
        public static MergeWaveBlock MergeWaveBlocks(WaveBlock[] waveBlocks, Vector2 itemSize, int row, int column) => MergeWaveBlocks(waveBlocks, itemSize, row, column, Vector2.one);
        
        public static MergeWaveBlock MergeWaveBlocks(WaveBlock[] waveBlocks, Vector2 itemSize, int row, int column, Vector2 scale)
        {
            var square = new GameObject("Square");
            var spriteRenderer = square.AddComponent<SpriteRenderer>();
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Packages/com.unity.2d.sprite/Editor/ObjectMenuCreation/DefaultAssets/Textures/v2/Square.png");
            spriteRenderer.sprite = sprite;
            spriteRenderer.sortingOrder = 5;

            itemSize = new Vector2(itemSize.x * scale.x, itemSize.y * scale.y);
        
            var go = new GameObject(waveBlocks[0].name + "Info");
            
            for (var i = 0; i < column; i++)
            for (var j = 0; j < row; j++)
            {
                var index = i * row + j;
                if (index >= waveBlocks.Length)
                    break;
                var item = Object.Instantiate(waveBlocks[index], go.transform);
                item.gameObject.name = waveBlocks[index].name;
                var transform = item.transform;
                transform.localScale = scale;
                transform.position = new Vector3(j * itemSize.y, i * itemSize.x) - new Vector3((column - 1) / 2f * itemSize.x, (row - 1) / 2f * itemSize.y);
            }
            
            for (var i = 0; i <= column; i++)
            {
                var line = Object.Instantiate(spriteRenderer, go.transform);
                var transform = line.transform;
                transform.localScale = new Vector3(itemSize.x * LineWidthRate, itemSize.y * row, 1);
                transform.position = new Vector3(i * itemSize.x, 0) - new Vector3(column / 2f * itemSize.x, 0);
                if (i == 0 || i == column)
                    line.color = Color.black;
            }
        
            for (var i = 0; i <= row; i++)
            {
                var line = Object.Instantiate(spriteRenderer, go.transform);
                var transform = line.transform;
                transform.localScale = new Vector3(itemSize.x * column, itemSize.y * LineWidthRate, 1);
                transform.position = new Vector3(0, i * itemSize.x) - new Vector3(0, row / 2f * itemSize.x);
                if (i == 0 || i == row)
                    line.color = Color.black;
            }
            Object.DestroyImmediate(square);
            return go.AddComponent<MergeWaveBlock>();
        }
    }
}