using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {
	
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			BoolMap boolMap = new BoolMap(Map.nodeMap, 0);
			print(boolMap);
			ParallelTaskManager.Instance.CallFuncParallel<IEnumerable<RotationStep>>(
				() => RotatingProblemSolver.FindRotationSequence(boolMap),
				Done);
		}
	}

	private void Done (IEnumerable<RotationStep> steps)
	{
		print("done");
		foreach (var s in steps)
		{
			print(s);
		}
	}
	
}
