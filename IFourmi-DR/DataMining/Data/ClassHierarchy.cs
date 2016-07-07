using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.DataMining.Data
{
    public class ClassHierarchy
    {
        #region Data Members

        private List<Node> _nodes;
        private Node _root;

        #endregion

        #region Properties

        public Node[] Nodes
        {
            get { return this._nodes.ToArray(); }

        }

        public Node Root
        {
            get { return this._root; }
        }

        public Node this[string name]
        {
            get
            {
                return this._nodes.Find(n => n.Name == name);
            }
        }

        public int Size
        {
            get { return this._nodes.Count; }
        }

        #endregion

        #region Constructors


        public ClassHierarchy(Node[] nodes)
        {
            this._root = nodes[0];
            this._nodes = nodes.ToList<Node>();

            foreach (Node parent in nodes)
            {
                Node[] children = this.GetChildren(parent);
                if (children != null)
                    foreach (Node child in children)
                        child.AddParent(parent.Name);
            }

        }



        #endregion

        #region Methods


        public void AddNode(Node node)
        {
            if (!this._nodes.Exists(n => n.Name == node.Name))
                this._nodes.Add(node);

        }

        public Node[] GetParents(Node node)
        {
            if (node.Parents == null)
                return null;

            List<Node> parents = new List<Node>();
            foreach (string parent in node.Parents)
                parents.Add(this[parent]);

            return parents.ToArray();
        }

        public Node[] GetAncestors(Node node)
        {
            if (node.Parents == null)
                return null;

            List<Node> ancestors = new List<Node>();
            Queue<Node> queue = new Queue<Node>();
            queue.Enqueue(node);

            while (queue.Count != 0)
            {
                Node current = queue.Dequeue();
                Node[] currentParents = this.GetParents(current);

                if (currentParents != null)
                {
                    foreach (Node parent in currentParents)
                    {
                        if (!ancestors.Contains(parent))
                        {
                            ancestors.Add(parent);
                            queue.Enqueue(parent);
                        }
                    }
                }


            }

            return ancestors.ToArray();


        }

        public Node[] GetChildren(Node node)
        {
            if (node.Children == null)
                return null;

            List<Node> children = new List<Node>();
            foreach (string child in node.Children)
            {
                Node childNode = this[child];
                children.Add(childNode);
            }

            return children.ToArray();
        }

        public Node[] GetDescendants(Node node)
        {
            if (node.Children == null)
                return null;

            List<Node> Descendants = new List<Node>();
            Queue<Node> queue = new Queue<Node>();
            queue.Enqueue(node);

            while (queue.Count != 0)
            {
                Node current = queue.Dequeue();
                Node[] currentChildren = this.GetChildren(current);

                if (currentChildren != null)
                {
                    foreach (Node child in currentChildren)
                    {
                        if (!Descendants.Contains(child))
                        {
                            Descendants.Add(child);
                            queue.Enqueue(child);

                        }


                    }
                }
            }

            return Descendants.ToArray();

        }

        public Node[] GetSiblings(Node node)
        {
            if (node.Parents == null)
                return null;

            Node[] parents = this.GetParents(node);
            List<Node> siblings = new List<Node>();
            foreach (Node parent in parents)
            {
                Node[] sib = this.GetChildren(parent);
                if (sib != null)
                {
                    foreach (Node sibling in sib)
                        if (sibling != node)
                            siblings.Add(sibling);
                }
            }

            return siblings.ToArray();


        }

        public Node[] GetNodes(string[] names)
        {
            List<Node> nodes = new List<Node>();
            foreach (string name in names)
                nodes.Add(this[name]);

            return nodes.ToArray();
        }

        public Node[] GetLeaves()
        {
            List<Node> leaves = new List<Node>();

            Stack<Node> nodes = new Stack<Node>();
            nodes.Push(this._root);

            while (nodes.Count != 0)
            {
                Node curent = nodes.Pop();
                Node[] children = this.GetChildren(curent);
                if (children == null)
                    leaves.Add(curent);
                else
                    foreach (Node node in children)
                        nodes.Push(node);

            }

            return leaves.ToArray();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            List<Node> Descendants = new List<Node>();
            Queue<Node> queue = new Queue<Node>();
            queue.Enqueue(this._root);

            while (queue.Count != 0)
            {
                Node current = queue.Dequeue();
                builder.Append(current.ToString());
                builder.Append(Environment.NewLine);

                Node[] children = this.GetChildren(current);
                if (children != null)
                {
                    foreach (Node child in children)
                    {
                        if (!Descendants.Exists(n => n.Name == child.Name))
                        {
                            Descendants.Add(child);
                            queue.Enqueue(child);

                        }

                    }
                }
            }

            return builder.ToString();
        }

        #endregion


       
    }

    public class Node
    {
        #region Data Members

        private string _name;
        private int _valueIdex;
        private List<string> _parents;
        private List<string> _children;

        #endregion

        #region Properties

        public string Name
        {
            get { return this._name; }
        }

        public int ValueIndex
        {
            get { return this._valueIdex; }
            set { this._valueIdex = value; }
        }


        public string[] Parents
        {
            get { return this._parents == null ? null : this._parents.ToArray(); }
        }

        public string[] Children
        {
            get { return this._children == null ? null : this._children.ToArray(); }
        }





        #endregion

        #region Constructors

        public Node(string name)
        {
            this._name = name;
        }
        #endregion

        #region Methods

        public void AddChild(string child)
        {
            if (this._children == null)
                _children = new List<string>();
            if (!_children.Contains(child))
                this._children.Add(child);
        }

        public void AddChildren(List<string> children)
        {
            if (this._children == null)
                this._children = new List<string>();
            foreach (string child in children)
                if (!this._children.Contains(child))
                    this._children.Add(child);

        }

        public void AddParent(string parent)
        {
            if (this._parents == null)
                this._parents = new List<string>();
            if (!this._parents.Contains(parent))
                this._parents.Add(parent);
        }

        public void AddParents(List<string> parents)
        {
            if (this._parents == null)
                this._parents = new List<string>();
            foreach (string parent in parents)
                if (!this._parents.Contains(parent))
                    this._parents.Add(parent);

        }

        public override string ToString()
        {

            StringBuilder builder = new StringBuilder();
            builder.Append(this._name);

            if (this._children != null)
            {
                builder.Append('\\');
                foreach (string child in this._children)
                {
                    builder.Append(child);
                    builder.Append(',');
                }

                builder.Remove(builder.Length - 1, 1);
            }
            return builder.ToString();

        }

        #endregion
    }
}
