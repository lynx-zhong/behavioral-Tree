using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BT_BehaviourTree;

namespace BT_BehaviourTree 
{

    /// <summary>
    /// 逻辑节点，它的意思是让所有子节点同时运行，那它什么时候结束呢，可以使当所有子节点都完成的时候结束，也可以让任一子节点完成时结束
    /// </summary>
    public class BTPingXingNode : BTNode 
	{
		protected List<BTResult> _results;
		protected ParallelFunction _func;

		/// <summary>
		/// 逻辑节点，它的意思是让所有子节点同时运行，那它什么时候结束呢，可以使当所有子节点都完成的时候结束，也可以让任一子节点完成时结束
		/// </summary>
		public BTPingXingNode (ParallelFunction func) : this (func, null) {}

        /// <summary>
        /// 逻辑节点，它的意思是让所有子节点同时运行，那它什么时候结束呢，可以使当所有子节点都完成的时候结束，也可以让任一子节点完成时结束
        /// </summary>
        public BTPingXingNode (ParallelFunction func, BTPrecondition precondition) : base (precondition) 
		{
			_results = new List<BTResult>();
			this._func = func;
		}

		protected override bool DoEvaluate () 
		{
			foreach (BTNode child in children) 
			{
				if (!child.Evaluate()) 
				{
					return false;	
				}
			}
			return true;
		}
		
		public override BTResult Tick () {
			int endingResultCount = 0;
			
			for (int i=0; i<children.Count; i++) {
				
				if (_func == ParallelFunction.And) {
					if (_results[i] == BTResult.Running) {
						_results[i] = children[i].Tick();	
					}
					if (_results[i] != BTResult.Running) {
						endingResultCount++;	
					}
				}
				else {
					if (_results[i] == BTResult.Running) {
						_results[i] = children[i].Tick();	
					}
					if (_results[i] != BTResult.Running) {
						ResetResults();
						return BTResult.Ended;
					}
				}
			}
			if (endingResultCount == children.Count) {	// only apply to AND func
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
			/// <summary>
			/// 所有节点停止运行时 结束
			/// </summary>
			And = 1,	// returns Ended when all results are not running
			
			/// <summary>
			/// 任一一个节点停止时运行
			/// </summary>
			Or = 2,		// returns Ended when any result is not running
		}
	}

}