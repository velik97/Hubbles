using UnityEngine;
using System.Collections;

public class AnimationFunctions {

	public static float EasyIn (float par, int pow) {
		return Mathf.Sign(par) * (Mathf.Pow (Mathf.Abs(par), pow));
	}

	public static float EasyOut (float par, int pow) {
		return Mathf.Sign(par) * (-Mathf.Abs(Mathf.Pow(Mathf.Abs(par) - 1, pow))+ 1);
	}

	public static float EasyInOut (float par, int pow) {
		if (Mathf.Abs(par) < 0.5f) {
			return EasyIn(par * 2, pow) * 0.5f;
		} else {
			return Mathf.Sign(par) * (EasyOut((Mathf.Abs(par) - 0.5f) * 2, pow) * 0.5f + 0.5f);
		}
	}

	public static float EasyOutIn (float par, int pow) {
		if (Mathf.Abs(par) < 0.5f) {
			return EasyOut(par * 2, pow) * 0.5f;
		} else {
			return Mathf.Sign(par) * (EasyIn((Mathf.Abs(par) - 0.5f) * 2, pow) * 0.5f + 0.5f);
		}
	}

	public static float EasyBetween (float par, float percent, int pow) {
		return percent + (Mathf.Pow(Mathf.Abs(Mathf.Abs(par)*2 - 1), pow)) * (1-percent);

	}

	void DeterminePolynomial (float x1,float y1, float difx, float dify, out float a, out float b, out float c) {
		a = (y1 - dify) / ((x1 - difx) * (x1 - difx));
		b = - 2 * a * difx;
		c = dify + a * difx * difx;
	}
}
