using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BT_BehaviourTree 
{
	/// <summary>
	/// 选择节点
	/// 对所有节点的可执行性进行检测，获取到可执行的第一个节点，该节点执行完毕，则停止节点检测
	/// </summary>
	public class BTPrioritySelectorNode : BTNode 
	{
		private BTNode _activeChild;
		public BTPrioritySelectorNode (BTPrecondition precondition = null) : base (precondition) {}

		protected override bool CustomNodeExecuteCondition () 
		{
			foreach (BTNode child in children) 
			{
				if (child.CheckNodeCanExecute()) 
				{
					if (_activeChild != null && _activeChild != child) 
					{
						_activeChild.Clear();	
					}
					_activeChild = child;
					return true;
				}
			}

			// 清理保护
			if (_activeChild != null) 
			{
				_activeChild.Clear();
				_activeChild = null;
			}

			return false;
		}
		
		public override void Clear () 
		{
			if (_activeChild != null) 
			{
				_activeChild.Clear();
				_activeChild = null;
			}
		}
		
		public override BTResult Tick () 
		{
			if (_activeChild == null) 
			{
				return BTResult.Ended;
			}

			BTResult result = _activeChild.Tick();
			if (result != BTResult.Running) 
			{
				_activeChild.Clear();
				_activeChild = null;
			}
			return result;
		}
	}
}