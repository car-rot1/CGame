using UnityEngine;

namespace CGame
{
    public class MergeWaveBlock : MonoBehaviour
    {
        private Transform _transform;
        public WaveBlock[] _waveBlocks;

        public void Init()
        {
            _transform = gameObject.transform;
            _waveBlocks = GetComponentsInChildren<WaveBlock>(true);
        }

        public void ShowItem(int index)
        {
            _transform.GetChild(index).gameObject.SetActive(true);
        }

        public void ShowItem(WaveBlock waveBlock)
        {
            foreach (var block in _waveBlocks)
            {
                if (block.name != waveBlock.name)
                    continue;
                block.gameObject.SetActive(true);
                break;
            }
        }

        public void ShowAll()
        {
            foreach (var block in _waveBlocks)
                block.gameObject.SetActive(true);
        }

        public void HideItem(int index)
        {
            _transform.GetChild(index).gameObject.SetActive(false);
        }

        public void HideItem(WaveBlock waveBlock)
        {
            foreach (var block in _waveBlocks)
            {
                if (block.name != waveBlock.name)
                    continue;
                block.gameObject.SetActive(false);
                break;
            }
        }
        
        public void HideAll()
        {
            foreach (var block in _waveBlocks)
                block.gameObject.SetActive(false);
        }
    }
}
