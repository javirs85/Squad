using UnityEngine;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

public class MainThreadExecutor : MonoBehaviour
{
	public static MainThreadExecutor Instance { get; private set; }
	private static SynchronizationContext unityContext;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			unityContext = SynchronizationContext.Current;
			DontDestroyOnLoad(gameObject); // Keep it across scenes
		}
		else
		{
			Destroy(gameObject); // Prevent duplicates
		}
	}

	public static Task RunOnMainThread(Action action)
	{
		
		var tcs = new TaskCompletionSource<bool>();
		if (unityContext == null)
		{
			Debug.LogError("MainThreadExecutor is not initialized. Add it to the scene.");
			return tcs.Task;
		}

		unityContext.Post(_ =>
		{
			try
			{
				action();
				tcs.SetResult(true);
			}
			catch (Exception ex)
			{
				tcs.SetException(ex);
			}
		}, null);

		return tcs.Task;
	}
	public static Task<T> RunOnMainThread<T>(Func<T> function)
	{
		var tcs = new TaskCompletionSource<T>();

		if (unityContext == null)
		{
			Debug.LogError("MainThreadExecutor is not initialized. Add it to the scene.");
			return tcs.Task;
		}

		unityContext.Post(_ =>
		{
			try
			{
				T result = function();  
				tcs.SetResult(result); 
			}
			catch (Exception ex)
			{
				tcs.SetException(ex);
			}
		}, null);

		return tcs.Task;
	}

	public static async Task<string> GetNameFromGameObject(GameObject obj)
	{
		var tcs = new TaskCompletionSource<string>();
		await MainThreadExecutor.RunOnMainThread(() =>
		{
			tcs.SetResult(obj.name);
		});

		string result = await tcs.Task;
		return result;
	}
}
