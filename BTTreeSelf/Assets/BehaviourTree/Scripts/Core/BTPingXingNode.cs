using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BT_BehaviourTree;

namespace BT_BehaviourTree 
{

    /// <summary>
    /// �߼��ڵ㣬������˼���������ӽڵ�ͬʱ���У�����ʲôʱ������أ�����ʹ�������ӽڵ㶼��ɵ�ʱ�������Ҳ��������һ�ӽڵ����ʱ����
    /// </summary>
    public class BTPingXingNode : BTNode 
	{
		protected List<BTResult> _results;
		protected ParallelFunction _func;

		/// <summary>
		/// �߼��ڵ㣬������˼���������ӽڵ�ͬʱ���У�����ʲôʱ������أ�����ʹ�������ӽڵ㶼��ɵ�ʱ�������Ҳ��������һ�ӽڵ����ʱ����
		/// </summary>
		public BTPingXingNode (ParallelFunction func) : this (func, null) {}

        /// <summary>
        /// �߼��ڵ㣬������˼���������ӽڵ�ͬʱ���У�����ʲôʱ������أ�����ʹ�������ӽڵ㶼��ɵ�ʱ�������Ҳ��������һ�ӽڵ����ʱ����
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
			/// ���нڵ�ֹͣ����ʱ ����
			/// </summary>
			And = 1,	// returns Ended when all results are not running
			
			/// <summary>
			/// ��һһ���ڵ�ֹͣʱ����
			/// </summary>
			Or = 2,		// returns Ended when any result is not running
		}
	}

}