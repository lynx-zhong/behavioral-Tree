using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BT_BehaviourTree 
{
	/// <summary>
	/// 并行节点2
	/// 同时执行的节点 会同时检测、执行所有子节点
	/// 所有节点执行完了之后，节点停止
	/// </summary>
	public class BTParallelFlexibleNode : BTNode {

		private List<bool> _activeList = new List<bool>();

		public BTParallelFlexibleNode (BTPrecondition precondition = null) : base (precondition) {}
		
		protected override bool CustomNodeExecuteCondition () 
		{
			int numActiveChildren = 0;
			for (int i=0; i<children.Count; i++) 
			{
				BTNode child = children[i];
				if (child.CheckNodeCanExecute()) 
				{
					_activeList[i] = true;
					numActiveChildren++;
				}
				else 
				{
					_activeList[i] = false;
				}
			}
			if (numActiveChildren == 0) 
			{
				return false;
			}
			return true;
		}

		public override BTResult Tick () 
		{
			int numRunningChildren = 0;
			for (int i=0; i<_children.Count; i++) 
			{
				bool active = _activeList[i];
				if (active) 
				{
					BTResult result = _children[i].Tick();
					if (result == BTResult.Running) 
					{
						numRunningChildren++;
					}
				}
			}

			if (numRunningChildren == 0) 
			{
				return BTResult.Ended;
			}

			return BTResult.Running;
		}

		public override void AddChild (BTNode aNode) 
		{
			base.AddChild (aNode);
			_activeList.Add(false);
		}

		public override void RemoveChild (BTNode aNode) 
		{
			int index = _children.IndexOf(aNode);
			_activeList.RemoveAt(index);
			base.RemoveChild (aNode);
		}

		public override void Clear () 
		{
			base.Clear ();

			foreach (BTNode child in children) 
			{
				child.Clear();
			}
		}
	}
}