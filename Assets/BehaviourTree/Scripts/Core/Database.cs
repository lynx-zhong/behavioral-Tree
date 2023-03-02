using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 数据库是典型的黑板系统中的黑板。
/// (我发现“黑板”这个名字有点难理解，所以我叫它数据库;p)
///
/// 它是存储本地节点、跨树节点甚至其他脚本数据的地方。
/// 节点可以通过使用字符串或数据的int id读取数据库中的数据。
/// 出于效率的考虑，人们更喜欢后者。
/// </summary>
public class Database : MonoBehaviour 
{

	// _database & _dataNames是1对1的关系
	private List<object> _database = new List<object>();
	private List<string> _dataNames = new List<string>();


	// 应该使用dataId作为参数来获取数据而不是这个
	public T GetData<T> (string dataName) 
	{
		int dataId = IndexOfDataId(dataName);
		if (dataId == -1) 
			Debug.LogError("Database: Data for " + dataName + " does not exist!");

		return (T) _database[dataId];
	}

	// 应该使用这个函数来获取数据!
	public T GetData<T> (int dataId) 
	{
		if (BT_BehaviourTree.BTConfiguration.ENABLE_DATABASE_LOG) 
		{
			Debug.Log("Database: getting data for " + _dataNames[dataId]);
		}
		return (T) _database[dataId];
	}
	
	public void SetData<T> (string dataName, T data) 
	{
		int dataId = GetDataId(dataName);
		_database[dataId] = (object) data;
	}

	public void SetData<T> (int dataId, T data) 
	{
		_database[dataId] = (object) data;
	}

	public int GetDataId (string dataName) 
	{
		int dataId = IndexOfDataId(dataName);
		if (dataId == -1) 
		{
			_dataNames.Add(dataName);
			_database.Add(null);
			dataId = _dataNames.Count - 1;
		}

		return dataId;
	}

	private int IndexOfDataId (string dataName) 
	{
		for (int i=0; i<_dataNames.Count; i++) 
		{
			if (_dataNames[i].Equals(dataName)) 
				return i;
		}

		return -1;
	}

	public bool ContainsData (string dataName) 
	{
		return IndexOfDataId(dataName) != -1;
	}
}


// IMPORTANT: users may want to put Jargon in a separate file
//public enum Jargon {
//	ShouldReset = 1,
//}



