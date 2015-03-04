using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.IE.Qwiq
{
    public class NodeProxy : INode
    {
        private readonly TeamFoundation.WorkItemTracking.Client.Node _node;

        internal NodeProxy(TeamFoundation.WorkItemTracking.Client.Node node)
        {
            _node = node;
        }

        public IEnumerable<INode> ChildNodes
        {
            get { return _node.ChildNodes.Cast<TeamFoundation.WorkItemTracking.Client.Node>().Select(item => new NodeProxy(item)); }
        }

        public bool HasChildNodes
        {
            get { return _node.HasChildNodes; }
        }

        public int Id
        {
            get { return _node.Id; }
        }

        public bool IsAreaNode
        {
            get { return _node.IsAreaNode; }
        }

        public bool IsIterationNode
        {
            get { return _node.IsIterationNode; }
        }

        public string Name
        {
            get { return _node.Name; }
        }

        public INode ParentNode
        {
            get { return new NodeProxy(_node.ParentNode); }
        }

        public string Path
        {
            get { return _node.Path; }
        }

        public Uri Uri
        {
            get { return _node.Uri; }
        }
    }
}