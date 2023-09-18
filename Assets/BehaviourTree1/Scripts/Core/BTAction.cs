using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BT_BehaviourTree 
{
	/// <summary>
	/// 功能节点
	/// 所有实际的功能操作 继承该节点
	/// </summary>
	public class BTAction : BTNode 
	{
		private BTActionStatus _status = BTActionStatus.Ready;
		
		public BTAction (BTPrecondition precondition = null) : base (precondition) {}


		protected virtual void Enter () 
		{
			if (BTConfiguration.ENABLE_BTACTION_LOG) 
			{
				Debug.Log("Enter " + this.name + " [" + this.GetType().ToString() + "]");
			}
		}

		protected virtual void Exit () 
		{
			if (BTConfiguration.ENABLE_BTACTION_LOG) 
			{
				Debug.Log("Exit " + this.name + " [" + this.GetType().ToString() + "]");
			}
		}

		protected virtual BTResult Execute () 
		{
			return BTResult.Running;
		}
		
		public override void Clear () 
		{
			if (_status != BTActionStatus.Ready) 
			{
				Exit();
				_status = BTActionStatus.Ready;
			}
		}
		
		public override BTResult Tick () 
		{
			BTResult result = BTResult.Ended;
			if (_status == BTActionStatus.Ready) 
			{
				Enter();
				_status = BTActionStatus.Running;
			}
			if (_status == BTActionStatus.Running) 
			{
				result = Execute();
				if (result != BTResult.Running) 
				{
					Exit();
					_status = BTActionStatus.Ready;
				}
			}
			return result;
		}

		public override void AddChild (BTNode aNode) 
		{
			Debug.LogError("BTAction: Cannot add a node into BTAction.");
		}

		public override void RemoveChild (BTNode aNode) 
		{
			Debug.LogError("BTAction: Cannot remove a node into BTAction.");
		}
		
		private enum BTActionStatus {
			Ready = 1,
			Running = 2,
		}
	}
}