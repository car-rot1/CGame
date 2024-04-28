using System;
using UnityEngine;
using UnityEngine.UI;

namespace CGame
{
    public class RedPointTest : MonoBehaviour
    {
        public int defaultPoint;
        public RedPointTest parentRedPoint;
        public bool isLeaf;

        public Text numText;
        public GameObject redPoint;
        
        public RedPointNode currentNode { get; private set; }

        private void Awake()
        {
            currentNode = new RedPointNode(parentRedPoint?.currentNode, isLeaf);
            currentNode.onPointNumChange += (oldValue, newValue) =>
            {
                if (newValue > 0)
                {
                    if (currentNode.IsLeaf)
                    {
                        if (oldValue <= 0)
                            numText.gameObject.SetActive(true);
                        numText.text = newValue.ToString();
                    }
                    else
                    {
                        if (oldValue <= 0)
                            redPoint.SetActive(true);
                    }
                }
                else
                {
                    if (oldValue > 0)
                    {
                        numText.gameObject.SetActive(false);
                        redPoint.SetActive(false);
                    }
                }
            };
            AddPoint(defaultPoint);
        }

        public void AddPoint(int point)
        {
            currentNode.AddPoint(point);
        }
        public void RemovePoint(int point)
        {
            currentNode.RemovePoint(point);
        }
    }
}