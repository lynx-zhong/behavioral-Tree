using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BT_BehaviourTree 
{

	/// <summary>
	/// 所有节点的根节点 基类
	/// </summary>
	public abstract class BTNode 
	{
		public string name;

		protected List<BTNode> _children;
		public List<BTNode> children {get{return _children;}}

		/// <summary>
		/// 节点的准入条件，每一个BTNode都会有一个。也就是以父节点
		/// </summary>
		public BTPrecondition precondition;

		/// <summary>
		/// 存放所有行为树 所使用的数据（在Unity3d的语境下，Database可继承MonoBehavior，可以提供各种Component给节点使用）
		/// </summary>
		public Database database;

		/// <summary>
		/// 冷却时间 间隔
		/// </summary>
		public float interval = 0;
		private float _lastTimeEvaluated = 0;

		/// <summary>
		/// 是否激活
		/// </summary>
		public bool activated;


		public BTNode () : this (null) {}

		public BTNode (BTPrecondition precondition) 
		{
			this.precondition = precondition;
		}

		// public virtual void Init () {}
		/// <summary>
		/// 节点初始化的接口，传入数据，设置激活状态，最主要是传入数据，Database可提供Unity3d中的Component给节点使用
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
		/// // 检查节点能否执行，包括是否activated，是否冷却完成，是否通过准入条件，和个性化检查 (DoEvaluate)
		/// </summary>
		public bool Evaluate () 
		{
			bool coolDownOK = CheckTimer();

			return activated && coolDownOK && (precondition == null || precondition.Check()) && DoEvaluate();
		}

		/// <summary>
		/// // 给子类提供个性化检查的接口
		/// </summary>
		protected virtual bool DoEvaluate () 
		{
			return true;
		}

		/// <summary>
		/// 节点执行的接口，需要返回BTResult.Running，或者BTResult.Ended
		/// </summary>
		public virtual BTResult Tick () 
		{
			return BTResult.Ended;
		}

		/// <summary>
		/// 节点清零的接口
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
		/// 移除子节点
		/// </summary>
		public virtual void RemoveChild (BTNode aNode) 
		{
			if (_children != null && aNode != null) 
			{
				_children.Remove(aNode);
			}
		}

		/// <summary>
		/// 检测 冷却时间 间隔时间 是否完成
		/// </summary>
		private bool CheckTimer () 
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
	/// 执行结果
	/// </summary>
	public enum BTResult 
	{
		/// <summary>
		/// 完成
		/// </summary>
		Ended = 1,

		/// <summary>
		/// 执行中
		/// </summary>
		Running = 2,
	}
}