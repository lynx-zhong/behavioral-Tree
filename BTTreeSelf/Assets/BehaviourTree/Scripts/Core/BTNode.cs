using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BT_BehaviourTree 
{

	/// <summary>
	/// ���нڵ�ĸ��ڵ� ����
	/// </summary>
	public abstract class BTNode 
	{
		public string name;

		protected List<BTNode> _children;
		public List<BTNode> children {get{return _children;}}

		/// <summary>
		/// �ڵ��׼��������ÿһ��BTNode������һ����Ҳ�����Ը��ڵ�
		/// </summary>
		public BTPrecondition precondition;

		/// <summary>
		/// ���������Ϊ�� ��ʹ�õ����ݣ���Unity3d���ﾳ�£�Database�ɼ̳�MonoBehavior�������ṩ����Component���ڵ�ʹ�ã�
		/// </summary>
		public Database database;

		/// <summary>
		/// ��ȴʱ�� ���
		/// </summary>
		public float interval = 0;
		private float _lastTimeEvaluated = 0;

		/// <summary>
		/// �Ƿ񼤻�
		/// </summary>
		public bool activated;


		public BTNode () : this (null) {}

		public BTNode (BTPrecondition precondition) 
		{
			this.precondition = precondition;
		}

		// public virtual void Init () {}
		/// <summary>
		/// �ڵ��ʼ���Ľӿڣ��������ݣ����ü���״̬������Ҫ�Ǵ������ݣ�Database���ṩUnity3d�е�Component���ڵ�ʹ��
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
		/// // ���ڵ��ܷ�ִ�У������Ƿ�activated���Ƿ���ȴ��ɣ��Ƿ�ͨ��׼���������͸��Ի���� (DoEvaluate)
		/// </summary>
		public bool Evaluate () 
		{
			bool coolDownOK = CheckTimer();

			return activated && coolDownOK && (precondition == null || precondition.Check()) && DoEvaluate();
		}

		/// <summary>
		/// // �������ṩ���Ի����Ľӿ�
		/// </summary>
		protected virtual bool DoEvaluate () 
		{
			return true;
		}

		/// <summary>
		/// �ڵ�ִ�еĽӿڣ���Ҫ����BTResult.Running������BTResult.Ended
		/// </summary>
		public virtual BTResult Tick () 
		{
			return BTResult.Ended;
		}

		/// <summary>
		/// �ڵ�����Ľӿ�
		/// </summary>
		public virtual void Clear () {}
		
		/// <summary>
		/// ����ӽڵ�
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
		/// �Ƴ��ӽڵ�
		/// </summary>
		public virtual void RemoveChild (BTNode aNode) 
		{
			if (_children != null && aNode != null) 
			{
				_children.Remove(aNode);
			}
		}

		/// <summary>
		/// ��� ��ȴʱ�� ���ʱ�� �Ƿ����
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
	/// ִ�н��
	/// </summary>
	public enum BTResult 
	{
		/// <summary>
		/// ���
		/// </summary>
		Ended = 1,

		/// <summary>
		/// ִ����
		/// </summary>
		Running = 2,
	}
}