using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CGame
{
    public sealed class DialogueOptionNodeAsset : NodeAssetBase
    {
        public string author;
        public string content;
        
        [SerializeField] public List<OptionNodeAsset> optionNodes;
        private int _currentIndex;

        public override NodeAssetBase NextNode
        {
            get => optionNodes[_currentIndex].NextNode;
            set => throw new Exception();
        }

        public override bool Finish { get; protected set; }

        public override void Execute()
        {
            UITest.Instance.ShowContent(author, content);
            UITest.Instance.ShowOption(optionNodes.Select(optionNode => optionNode.option).ToList(), index =>
            {
                _currentIndex = index;
                Finish = true;
            });
        }
    }
}