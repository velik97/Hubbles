using UnityEngine;
using System.Collections;

public static class AnimationF
{
	public delegate float InOutEaseFunc(float par, int pow);
	public delegate float BetweenEaseFunc(float par, float percent, int pow);
	public delegate float GeneralEaseFunc(float par);
	
	public static float EasyIn (float par, int pow)
	{
		float sign = Mathf.Sign(par);
		par = Mathf.Abs(par);
		return sign * Mathf.Pow (par, pow);
	}

	public static float EasyOut (float par, int pow)
	{
		float sign = Mathf.Sign(par);
		par = Mathf.Abs(par);
		return sign * (-Mathf.Abs(Mathf.Pow(par - 1, pow)) + 1);
	}

	public static float EasyInOut (float par, int pow)
	{
		float sign = Mathf.Sign(par);
		par = Mathf.Abs(par);
		if (par < 0.5f)
			return sign * EasyIn(par * 2, pow) * 0.5f;
		return sign * (EasyOut((par - 0.5f) * 2, pow) * 0.5f + 0.5f);
	}

	public static float EasyOutIn (float par, int pow)
	{
		par = Mathf.Abs(par);
		if (par < 0.5f)
			return EasyOut(par * 2, pow) * 0.5f;
		return EasyIn((par - 0.5f) * 2, pow) * 0.5f + 0.5f;
	}

	public static float EasyBetween (float par, float percent, int pow)
	{
		par = Mathf.Abs(par);
		return (1f - Mathf.Pow(1f - Mathf.Abs(par * 2f - 1f), pow)) * percent;
	}

	private static GeneralEaseFunc DeterminePolynomial (float x1, float y1, float difx, float dify) {
		float a = (y1 - dify) / ((x1 - difx) * (x1 - difx));
		float b = - 2 * a * difx;
		float c = dify + a * difx * difx;
		return t => a * t * t + b * t + c;
	}
}
