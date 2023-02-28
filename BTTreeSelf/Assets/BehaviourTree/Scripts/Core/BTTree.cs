using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BT_BehaviourTree;

// How to use:
// 1。初始化 数据的值（Database类）供子节点使用。
// 2。初始化BT _root
// 3。稍后将使用的一些操作和先决条件
// 4。添加子节点
// 5。激活_root，包括子节点的初始化

public abstract class BTTree : MonoBehaviour 
{
	protected BTNode _root = null;

	[HideInInspector]
	public Database database;

	[HideInInspector]
	public bool isRunning = true;

	public const string RESET = "Rest";
	private static int _resetId;

	void Awake () 
	{
		Init();

		_root.Activate(database);
	}
	void Update () 
	{
		if (!isRunning) 
			return;
		
		if (database.GetData<bool>(RESET)) 
		{
			Reset();	
			database.SetData<bool>(RESET, false);
		}

		// Iterate the BT tree now!
		if (_root.Evaluate()) 
		{
			_root.Tick();
		}
	}

	void OnDestroy () 
	{
		if (_root != null) 
		{
			_root.Clear();
		}
	}

	// 需要在子节点的初始化代码中调用。
	protected virtual void Init () 
	{
		database = GetComponent<Database>();
		if (database == null) 
		{
			database = gameObject.AddComponent<Database>();
		}

		_resetId = database.GetDataId(RESET);
		database.SetData<bool>(_resetId, false);
	}

	protected void Reset () 
	{
		if (_root != null) 
		{
			_root.Clear();	
		}
	}
}
