﻿using UnityEngine;
using System.Collections;

public abstract class MonoSingleton <T> : MonoBehaviour where T : MonoBehaviour {

	private static T instance;

	private static object _lock = new object();

	public static T Instance {
		get {
			Debug.Log ("hi" + typeof(T).ToString ());
//			lock (_lock) {
				if (instance == null) {
					instance = (T) FindObjectOfType (typeof(T));

					if ( FindObjectsOfType(typeof(T)).Length > 1 )
					{
						Debug.LogError("[Singleton] Something went really wrong " +
							" - there should never be more than 1 singleton of type " + typeof(T).ToString () +
							"! Reopening the scene might fix it.");
						return instance;
					}
				}

				return instance;
//			}
		}
	}


}
