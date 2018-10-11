using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Deprecated
/// </summary>
public class HTypeList : ScriptableSingleton <HTypeList> {

	public List <HType> list = new List<HType> ();

	public static int DefaultIndex {
		get {
			if (Get () == null || Instance.list.Count == 0) {
				return 0;
			} else {
				return Instance.list [0].index; 
			}
		}
	}

	public static HType[] Get () {
		return Instance.list.ToArray ();
	}

	public static void UpdateList (HType newHType) {
		HType htypeToUpdate = Instance.HTypeWithIndex (newHType.index);
		if (htypeToUpdate == null) {
			Instance.list.Add (newHType);
			Instance.list.Sort ();
		} else {
			htypeToUpdate.name = newHType.name;
			htypeToUpdate.shortCut = newHType.shortCut;
		}
			
	}
		

	public static bool DelHType (HType htype) {
		if (Instance.list.Count > 0) {
			HType htypeToDelete = Instance.HTypeWithIndex (htype.index);
			if (htypeToDelete != null) {
				Instance.list.Remove (htypeToDelete);
				return true;
			}
		}
		return false;
	}

	public static bool Contains (int index) {
		bool contains = false;
		foreach (HType htype in Instance.list) {
			if (htype.index == index) {
				contains = true;
				break;
			}
		}
		return contains;
	}

	public static HType GetWithIndex (int index) {
		HType _htype = null;
		foreach (HType htype in Instance.list) {
			if (htype.index == index) {
				_htype = htype;
				break;
			}
		}
		return _htype;
	}

	public HType HTypeWithIndex (int index) {
		HType htypeToReturn = null;
		foreach (HType htype in list) {
			if (htype.index == index) {
				htypeToReturn = htype;
				break;
			}
		}
		return htypeToReturn;
	}
}
