// Singleton.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour
{
	private static T _instance;
	public static T Instance
	{
		get
		{
			if (Equals(_instance, null) || _instance == null || _instance.Equals(null))
			{
				var instanceGO = FindFirstObjectByType<Singleton<T>>();
				_instance = instanceGO.GetComponent<T>();
				return _instance;
			}
			else
			{
				return _instance;
			}
		}
		set { _instance = value; }
	}

	// The child must call SingletonBuilder() with a reference to itself.
	protected void SingletonBuilder(T newInstance)
	{
		// If another already exists, forget this one
		var instanceGO = FindFirstObjectByType<Singleton<T>>();
		if (instanceGO == null)
		{
			Destroy(this.gameObject);
			return;
		}

		if (_instance == null)
		{
			_instance = newInstance;
		}
		else if (_instance.Equals(newInstance))
		{
			Debug.LogWarning("Found two singletons of type " + this);
			Destroy(gameObject);
		}
	}
}