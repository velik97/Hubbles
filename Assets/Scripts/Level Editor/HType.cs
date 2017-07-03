using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class HType : IComparable {

	public int index;
	public string name;
	public string shortCut;

	public HType (int index, string name, string shortCut) {
		this.index = index;
		this.name = name;
		this.shortCut = shortCut;
	}

	public static bool operator == (HType a, HType b) {
		System.Object aObj = a as System.Object;
		System.Object bObj = b as System.Object;

		if (aObj == null ^ bObj == null) {
			return false;
		} else if (aObj == null && bObj == null) {
			return true;
		}

		return (a.index == b.index && a.name == b.name && a.shortCut == b.shortCut);
	}

	public static bool operator != (HType a, HType b) {
		return !(a == b);
	}

	public int CompareTo (System.Object obj) {
		if (obj == null) {
			return 1;
		}

		HType otherHType = obj as HType;

		if (otherHType == null) {
			Debug.LogError ("Object is not a HType");
			return 1;
		}

		return this.index.CompareTo (otherHType.index);
	}

	//-2 -- none
	//-1 -- usual
	// 0 -- empty
	// 1 -- health
	// 2 -- multiplayer
	// 3 -- ice
	// 4 -- gear
	// 5 -- versicolor

}
