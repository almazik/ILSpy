using System;
using System.Collections;
using System.Collections.Generic;

namespace Decompiler.ControlFlow
{
	public abstract partial class Node
	{
		public void Remove()
		{
			if (this.Parent != null) {
				this.Parent.Childs.Remove(this);
			}
		}
		
		public void MoveTo(Node newNode)
		{
			MoveTo(newNode, newNode.Childs.Count);
		}
		
		public void MoveTo(Node newNode, int index)
		{
			this.Remove();
			newNode.Childs.Insert(index, this);
		}
		
		T MergeChilds<T>(params Node[] nodes) where T: Node, new()
		{
			foreach(Node node in nodes) {
				if (node == null) throw new ArgumentNullException("nodes");
				if (node.Parent != this) throw new ArgumentException("The node is not my child");
			}
			if (nodes.Length == 0) throw new ArgumentException("At least one node must be specified");
			
			T mergedNode = new T();
			
			// Add the merged node
			int headIndex = this.Childs.IndexOf(nodes[0]);
			this.Childs.Insert(headIndex, mergedNode);
			
			foreach(Node node in nodes) {
				node.MoveTo(mergedNode);
			}
			
			return mergedNode;
		}
		
		public void FalttenAcyclicChilds()
		{
		Reset:
			foreach(Node child in this.Childs) {
				if (child is AcyclicGraph) {
					child.Childs.MoveTo(this, child.Index);
					child.Remove();
					goto Reset;
				}
			}
		}
	}
}
