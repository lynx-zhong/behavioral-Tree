using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BT_BehaviourTree 
{

	/// <summary>
	/// �߼��ڵ� �����ұ����Լ����ӽڵ㣬����ӽڵ��׼������������Ϣ�Ļ�����ִ�и��ӽڵ㡣
	/// </summary>
	public class BTYouXianXuanZhe : BTNode 
	{
		
		private BTNode _activeChild;

		/// <summary>
		/// �߼��ڵ� �����ұ����Լ����ӽڵ㣬����ӽڵ��׼������������Ϣ�Ļ�����ִ�и��ӽڵ㡣
		/// </summary>
		public BTYouXianXuanZhe (BTPrecondition precondition = null) : base (precondition) {}

		/// <summary>
		/// ѡ�񼤻�� �ӽڵ�
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