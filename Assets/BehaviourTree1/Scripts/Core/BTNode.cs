using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BT_BehaviourTree 
{
	public abstract class BTNode 
	{
		public string name;

		protected List<BTNode> _children;
		public List<BTNode> children {get{return _children;}}

		/// <summary>
		/// 所有节点都可以有的，节点执行条件的前置节点
		/// 可有可无
		/// </summary>
		public BTPrecondition precondition;

		public Database database;

		public float interval = 0;
		private float _lastTimeEvaluated = 0;

		public bool activated;


		public BTNode () : this (null) {}

		public BTNode (BTPrecondition precondition) 
		{
			this.precondition = precondition;
		}

		/// <summary>
		/// 激活节点 传入数据
		/// </summary>
		public virtual void Activate (Database database) 
		{
			if (activated) 
				return ;
			
			this.database = database;
			
			if (precondition != null) 
			{
				precondition.Activate(database);
			}

			if (_children != null) 
			{
				foreach (BTNode child in _children) 
				{
					child.Activate(database);
				}
			}
			
			activated = true;
		}

		/// <summary>
		/// 检测节点是否可执行
		/// </summary>
		public bool CheckNodeCanExecute () 
		{
			bool coolDownOK = CheckCoolTimer();

			return activated && coolDownOK && (precondition == null || precondition.Check()) && CustomNodeExecuteCondition();
		}

		/// <summary>
		/// 自定义节点的执行条件
		/// </summary>
		protected virtual bool CustomNodeExecuteCondition () 
		{
			return true;
		}

		/// <summary>
		/// 每帧检测行为树节点执行结果
		/// </summary>
		public virtual BTResult Tick () 
		{
			return BTResult.Ended;
		}

		/// <summary>
		/// 清楚数据或者其他东西
		/// </summary>
		public virtual void Clear () {}
		
		/// <summary>
		/// 添加子节点
		/// </summary>
		public virtual void AddChild (BTNode aNode) 
		{
			if (_children == null) 
			{
				_children = new List<BTNode>();	
			}
			if (aNode != null) 
			{
				_children.Add(aNode);
			}
		}

		/// <summary>
		/// 移出子节点
		/// </summary>
		public virtual void RemoveChild (BTNode aNode) 
		{
			if (_children != null && aNode != null) 
			{
				_children.Remove(aNode);
			}
		}

		/// <summary>
		/// 冷却时间检测
		/// </summary>
		private bool CheckCoolTimer () 
		{
			if (Time.time - _lastTimeEvaluated > interval) 
			{
				_lastTimeEvaluated = Time.time;
				return true;
			}
			return false;
		}
	}
	
	/// <summary>
	/// 节点执行结果
	/// </summary>
	public enum BTResult 
	{
		/// <summary>
		/// 执行结束
		/// </summary>
		Ended = 1,

		/// <summary>
		/// 执行中
		/// </summary>
		Running = 2,
	}
}