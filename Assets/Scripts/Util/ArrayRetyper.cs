using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows to create array with new type, based on array with initial type and way to convert one type to another
/// </summary>
public static class ArrayRetyper
{
	/// <summary>
	/// Converts initial type to new
	/// </summary>
	/// <param name="initialElement">element of initial type</param>
	/// <typeparam name="T">new type</typeparam>
	/// <typeparam name="U">initial type</typeparam>
	public delegate T RetypeDelegate<in U, out T>(U initialElement);
	
	/// <summary>
	/// Creates array with new type, based on array with initial type and way to convert one type to another
	/// </summary>
	/// <param name="initialArray">array of elements with initial type</param>
	/// <param name="retypeDelegate">func, that converts initial type to new</param>
	/// <typeparam name="T">new type</typeparam>
	/// <typeparam name="U">initial type</typeparam>
	/// <returns></returns>
	public static T[,] RetypeArray<U, T>(U[,] initialArray, RetypeDelegate<U, T> retypeDelegate)
	{
		var array = new T[initialArray.GetLength(0),initialArray.GetLength(1)];
		for (var i = 0; i < initialArray.GetLength(0); i++)
		{
			for (var j = 0; j < initialArray.GetLength(1); j++)
			{
				array[i, j] = retypeDelegate(initialArray[i, j]);
			}	
		}
		return array;
	}
}
