using UnityEngine;
using System.Collections;

namespace BT_BehaviourTree 
{
	/// <summary>
	/// 顺序执行节点
	/// 所有子节点 顺序执行结束后 节点执行结束
	/// </summary>
	public class BTSequence : BTNode 
	{
		/// <summary>
		/// 当前执行的子节点
		/// </summary>
		private BTNode _activeChild;
		private int _activeIndex = -1;

		public BTSequence (BTPrecondition precondition = null) : base (precondition) {}
		
		protected override bool CustomNodeExecuteCondition () 
		{
			if (_activeChild != null) 
			{
				bool result = _activeChild.CheckNodeCanExecute();
				if (!result) 
				{
					_activeChild.Clear();
					_activeChild = null;
					_activeIndex = -1;
				}
				return result;
			}
			else 
			{
				return children[0].CheckNodeCanExecute();
			}
		}
		
		public override BTResult Tick () 
		{
			if (_activeChild == null) 
			{
				_activeChild = children[0];
				_activeIndex = 0;
			}

			BTResult result = _activeChild.Tick();
			if (result == BTResult.Ended) 
			{
				_activeIndex++;
				if (_activeIndex >= children.Count) 		// 所有子节点都运行过了，节点执行结束
				{
					_activeChild.Clear();
					_activeChild = null;
					_activeIndex = -1;
				}
				else 										// 获取下一个执行节点
				{
					_activeChild.Clear();
					_activeChild = children[_activeIndex];
					result = BTResult.Running;
				}
			}
			return result;
		}
		
		public override void Clear () 
		{
			if (_activeChild != null) 
			{
				_activeChild = null;
				_activeIndex = -1;
			}

			foreach (BTNode child in children) 
			{
				child.Clear();
			}
		}
	}
}