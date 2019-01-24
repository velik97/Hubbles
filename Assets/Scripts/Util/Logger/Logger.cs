using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;
using UnityEngine.Events;

public class Logger : MonoSingleton <Logger>
{
	private StringEvent onNewLog = new StringEvent();
	
	[SerializeField] private string header;
	[SerializeField] private int stringsCount = 20;

	[SerializeField] private bool duplicateInLog;

	private Queue <string> logHistory;

	private void Start()
	{
		if (!string.IsNullOrEmpty(header))
			onNewLog.Invoke(header);
	}

	public static void Subscribe(UnityAction<string> action)
	{
		Instance.onNewLog.AddListener(action);
	}

	public static void Log(object obj)
	{
		Instance.LogOnInstance(obj.ToString());
	}

	private void LogOnInstance(string log)
	{
		if (logHistory == null)
			logHistory = new Queue<string>();
		if (logHistory.Count == stringsCount && logHistory.Count > 0)
		{
			logHistory.Dequeue();
		}
		logHistory.Enqueue(log);

		StringBuilder stringBuilder = new StringBuilder();
		if (!string.IsNullOrEmpty(header))
			stringBuilder.AppendLine(header);

		foreach (string l in logHistory)
		{
			stringBuilder.AppendLine(l);
		}
		
		onNewLog.Invoke(stringBuilder.ToString());

		if (duplicateInLog)
			Debug.Log(log); 
	}
}

public class StringEvent : UnityEvent<string> {}