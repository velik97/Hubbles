using UnityEngine;
using System.Collections;

public abstract class ScriptableSingleton <T> : ScriptableObject where T : ScriptableObject {

	public static T Instance;

}
