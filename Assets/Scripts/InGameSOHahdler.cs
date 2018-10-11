using UnityEngine;
using System.Collections;

/// <summary>
/// Stores link to config files <c>LevelConfigHandler</c>
/// </summary>
public class InGameSOHahdler : MonoSingleton <InGameSOHahdler> {

	/// <summary>
	/// Level configuration files storage
	/// </summary>
	public LevelConfigHandler levelConfigHandler;

	void Awake () {
		LevelConfigHandler.Instance = levelConfigHandler;
	}

}
