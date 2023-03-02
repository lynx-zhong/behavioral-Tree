using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BT_BehaviourTree 
{

	/// <summary>
	/// 逻辑节点 从左到右遍历自己的子节点，如果子节点的准入条件符合信息的话，就执行该子节点。
	/// </summary>
	public class BTYouXianXuanZhe : BTNode 
	{
		
		private BTNode _activeChild;

		/// <summary>
		/// 逻辑节点 从左到右遍历自己的子节点，如果子节点的准入条件符合信息的话，就执行该子节点。
		/// </summary>
		public BTYouXianXuanZhe (BTPrecondition precondition = null) : base (precondition) {}

		/// <summary>
		/// 选择激活的 子节点
		/// </summary>
		/// <returns></returns>
		protected override bool DoEvaluate () 
		{
			foreach (BTNode child in children) 
			{
				if (child.Evaluate()) 
				{
					if (_activeChild != null && _activeChild != child) 
					{
						_activeChild.Clear();	
					}
					_activeChild = child;
					return true;
				}
			}

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