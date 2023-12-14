using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataStructures
{
    [Serializable]
    public class BloofiNode
    {
        public BloomFilter Value;
        public BloofiNode? Parent;
        public List<BloofiNode> Children;

        public BloofiNode(BloomFilter value)
        {
            Value = value;
            Parent = null;
            Children = new();
        }

        public bool IsLeaf()
        {
            return Children.Count == 0;
        }

        public bool IsOverflown(int order)
        {
            return !(Children == null || Children.Count <= 2 * order);
        }

        public void ComputeValue()
        {
            foreach (var child in Children)
            {
                Value.Or(child.Value);
            }
        }

        public BloofiNode FindClosestChild(List<BloofiNode> nodeList)
        {
            var index = FindClosestChildIndex(nodeList);
            return nodeList[index];
        }

        public int FindClosestChildIndex(List<BloofiNode> nodeList)
        {
            if (nodeList.Count == 0)
            {
                throw new ArgumentException("nodeList must have children");
            }

            BloofiNode currentNode = nodeList[0];
            int minDistance = Value.HammingDistance(currentNode.Value);
            int minIndex = 0;
            int currentDistance;

            for (int i = 0; i < nodeList.Count - 1; i++)
            {
                currentNode = nodeList[i];
                currentDistance = Value.HammingDistance(currentNode.Value);

                if (currentDistance < minDistance)
                {
                    minDistance = currentDistance;
                    minIndex = i;
                }
            }

            return minIndex;
        }
    }
}