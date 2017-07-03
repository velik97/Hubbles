using UnityEngine;
using System.Collections;

public class InGameSOHahdler : MonoSingleton <InGameSOHahdler> {

	public LevelConfigHandler levelConfigHandler;

	void Awake () {
		LevelConfigHandler.Instance = levelConfigHandler;
	}

}
