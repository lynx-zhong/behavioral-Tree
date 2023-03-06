using System.Collections.Generic;

namespace BT_BehaviourTree 
{
	/// <summary>
	/// 并行节点1
	/// 同时执行所有子节点
	/// 子节点的结束条件由传入的 ParallelFunction 类型决定
	/// ParallelFunction.And 所有节点执行完停止
	/// ParallelFunction.Or 当有一个条件停止 则停止
	/// </summary>
    public class BTParallelNode : BTNode 
	{
		protected List<BTResult> _results;
		protected ParallelFunction endFunc;

		public BTParallelNode (ParallelFunction func) : this (func, null) {}
        public BTParallelNode (ParallelFunction func, BTPrecondition precondition) : base (precondition) 
		{
			_results = new List<BTResult>();
			this.endFunc = func;
		}

		protected override bool CustomNodeExecuteCondition () 
		{
			foreach (BTNode child in children) 
			{
				if (!child.CheckNodeCanExecute()) 
				{
					return false;	
				}
			}
			return true;
		}
		
		public override BTResult Tick () 
		{
			int endingResultCount = 0;
			
			for (int i=0; i<children.Count; i++) 
			{
				if (endFunc == ParallelFunction.And) 
				{
					if (_results[i] == BTResult.Running) 
					{
						_results[i] = children[i].Tick();	
					}
					if (_results[i] == BTResult.Ended) 
					{
						endingResultCount++;	
					}
				}
				else 
				{
					if (_results[i] == BTResult.Running) 
					{
						_results[i] = children[i].Tick();	
					}
					if (_results[i] == BTResult.Ended) 
					{
						ResetResults();
						return BTResult.Ended;
					}
				}
			}

			if (endingResultCount == children.Count) 
			{
				ResetResults();
				return BTResult.Ended;
			}
			return BTResult.Running;
		}
		
		public override void Clear () {
			ResetResults();
			
			foreach (BTNode child in children) {
				child.Clear();	
			}
		}
		
		public override void AddChild (BTNode aNode) {
			base.AddChild (aNode);
			_results.Add(BTResult.Running);
		}

		public override void RemoveChild (BTNode aNode) {
			int index = _children.IndexOf(aNode);
			_results.RemoveAt(index);
			base.RemoveChild (aNode);
		}
		
		private void ResetResults () 
		{
			for (int i=0; i<_results.Count; i++) 
			{
				_results[i] = BTResult.Running;	
			}
		}
		
		public enum ParallelFunction 
		{
			And = 1,	// 所有节点结束 才结束

			Or = 2,		// 有一个条件结束，则结束节点
		}
	}

}