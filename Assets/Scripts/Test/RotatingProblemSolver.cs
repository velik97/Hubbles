using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// This class solves problem of finding rotations for collecting all hubbles of certain color in a group
/// </summary>
public static class RotatingProblemSolver
{
	/// <summary>
	/// Returns sequence of steps, that you need to make to collect all hubbles of certain color in a group 
	/// </summary>
	/// <param name="boolMap">initial map</param>
	/// <returns>sequence of steps, that you need to make to collect all hubbles of certain color in a group</returns>
	public static IEnumerable<RotationStep> FindRotationSequence(BoolMap boolMap)
	{
		Queue<BoolMap> allPossibleSteps = new Queue<BoolMap>();
		int minGroupsCount = boolMap.GroupsCount();

		allPossibleSteps.Enqueue(boolMap);
		Debug.Log(boolMap);

		while (minGroupsCount > 1)
		{
			foreach (var step in allPossibleSteps.Dequeue().PossibleSteps())
			{
				int groupsCount = step.GroupsCount();
				if (groupsCount < minGroupsCount)
				{
					allPossibleSteps.Clear();
					
					allPossibleSteps.Enqueue(step);
										
					minGroupsCount = groupsCount;
					if (minGroupsCount == 1)
					{
						boolMap = step;
					}

					break;
				}

				allPossibleSteps.Enqueue(step);
			}
		}
		Debug.Log(boolMap);

		Stack<RotationStep> steps = new Stack<RotationStep>();
		
		while (boolMap != null)
		{
			if (boolMap.turnCoord != null)
			{
				steps.Push(new RotationStep(boolMap.turnCoord, boolMap.turns));
			}
			boolMap = boolMap.parent;
		}

		return steps;
	}

}

/// <summary>
/// Step of rotation (coord and turns count)
/// </summary>
public struct RotationStep
{
	public Coord coord;
	public int turns;

	public RotationStep(Coord coord, int turns)
	{
		this.coord = coord;
		this.turns = turns;
	}

	public override string ToString()
	{
		return "coord: " + coord + "\n" + "turns: " + turns;
	}
}
