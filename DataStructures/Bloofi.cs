using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataStructures.Factories;

namespace DataStructures
{
    // TODO: 
    // - delete
    // - update
    [Serializable]
    public class Bloofi
    {
        private readonly int _order;

        public BloofiNode Root { get; private set; }

        public Bloofi(int order, BloomFilter sampleFilter)
        {
            _order = order;

            var emptyFilter = BloomFilterFactory.CreateEmpty(sampleFilter);
            Root = new BloofiNode(emptyFilter);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append("Bloofi: \n");
            sb.Append($"0:\t {Root.Value.ToStringValue()}\n");

            int i = 1;

            ChildrenToString(Root, sb, i);

            return sb.ToString();
        }

        private StringBuilder ChildrenToString(BloofiNode node, StringBuilder sb, int level)
        {
            foreach (var child in node.Children)
            {
                sb.Append($"{level}: \t {child.Value.ToStringValue()}\n");
                if (!child.IsLeaf())
                {
                    ChildrenToString(child, sb, level + 1);
                }
            }

            return sb;
        }

        public List<BloomFilter> FindMatches(string query)
        {
            return FindMatches(Root, query);
        }

        public List<BloomFilter> FindMatches(BloofiNode node, string query)
        {
            List<BloomFilter> matches = new();

            var containsQuery = node.Value.Contains(query);
            if (!containsQuery)
            {
                return matches;
            }

            if (node.IsLeaf())
            {
                matches.Add(node.Value);
                return matches;
            }

            for (int i = 0; i < node.Children.Count; i++)
            {
                var child = node.Children[i];
                var childMatches = FindMatches(child, query);
                matches.AddRange(childMatches);
            }

            return matches;
        }

        public void Insert(BloomFilter newBloomFilter)
        {
            var newNode = new BloofiNode(newBloomFilter);

            if (Root.IsLeaf())
            {
                Root.Children.Add(newNode);
                newNode.Parent = Root;
                Root.ComputeValue();
                Root.Value.Or(newBloomFilter);
            }
            else
            {
                Insert(Root, newNode);
            }
        }

        private BloofiNode? Insert(BloofiNode node, BloofiNode newNode)
        {
            BloofiNode? newSibling;

            if (node.IsLeaf())
            {
                newSibling = InsertIntoParent(newNode, node);
                return newSibling;
            }

            node.Value.Or(newNode.Value);

            var closestChild = newNode.FindClosestChild(node.Children);
            newSibling = Insert(closestChild, newNode);

            if (newSibling == null)
            {
                return null;
            }
            else
            {
                if (node.Parent == null)
                {
                    var newRootFilter = BloomFilterFactory.CreateEmpty(node.Value);
                    var newRoot = new BloofiNode(newRootFilter);

                    newRoot.Value.Or(node.Value);
                    newRoot.Value.Or(newSibling.Value);

                    newRoot.Children.Add(node);
                    node.Parent = newRoot;

                    newRoot.Children.Add(newSibling);
                    newSibling.Parent = newRoot;

                    Root = newRoot;

                    return null;
                }
                else
                {
                    newSibling = InsertIntoParent(newSibling, node);
                    return newSibling;
                }
            }
        }

        public BloofiNode? InsertIntoParent(BloofiNode newEntry, BloofiNode node)
        {
            if (node.Parent == null)
            {
                throw new ArgumentNullException(nameof(node.Parent));
            }

            int index = node.Parent.Children.IndexOf(node);

            node.Parent.Children.Insert(index + 1, newEntry);
            newEntry.Parent = node.Parent;

            if (!node.Parent.IsOverflown(_order))
            {
                return null;
            }

            return Split(node.Parent);
        }

        private BloofiNode? Split(BloofiNode current)
        {
            BloomFilter sampleFilter = current.Value;
            BloomFilter emptyFilter = BloomFilterFactory.CreateEmpty(sampleFilter);

            BloofiNode newChild;
            BloofiNode newNode = new(emptyFilter);

            for (int i = _order + 1; i < current.Children.Count; i++)
            {
                newChild = current.Children[i];
                newNode.Children.Add(newChild);
                newChild.Parent = newNode;

                newNode.Value.Or(newChild.Value);
            }

            current.Children.RemoveRange(_order + 1, current.Children.Count - _order - 1);
            current.ComputeValue();

            return newNode;
        }
    }
}
