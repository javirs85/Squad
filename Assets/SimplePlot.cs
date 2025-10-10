using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class SimplePlot : MonoBehaviour
{
	[Header("Plot Settings")]
	[SerializeField] int maxPoints = 200;          // visible window size
	[SerializeField] float xStep = 0.02f;          // horizontal spacing
	[SerializeField] float yScale = 0.1f;          // vertical scale
	[SerializeField] bool prefillWithZeros = true; // optional startup fill

	LineRenderer line;
	readonly Queue<float> values = new();

	void Awake()
	{
		line = GetComponent<LineRenderer>();
		line.useWorldSpace = false;
		line.widthCurve = AnimationCurve.Constant(0, 1, 0.01f);

		if (prefillWithZeros)
			Prefill();
	}

	/// <summary>Manually clear plot and optionally prefill with zeros.</summary>
	public void Clear(bool refill = false)
	{
		values.Clear();
		line.positionCount = 0;
		if (refill && prefillWithZeros)
			Prefill();
	}

	/// <summary>Add a single new value and update line (scrolls automatically).</summary>
	public void AddValue(float v)
	{
		values.Enqueue(v);
		if (values.Count > maxPoints)
			values.Dequeue();
		UpdateLine();
	}

	/// <summary>Force-update current line positions (e.g. after scaling change).</summary>
	public void Refresh() => UpdateLine();

	void Prefill()
	{
		values.Clear();
		for (int i = 0; i < maxPoints; i++)
			values.Enqueue(0f);
		UpdateLine();
	}

	void UpdateLine()
	{
		int count = values.Count;
		line.positionCount = count;

		int i = 0;
		foreach (var val in values)
		{
			line.SetPosition(i, new Vector3(i * xStep, val * yScale, 0));
			i++;
		}
		//Debug.Log($"Plot scale: {transform.lossyScale}, total length: {values.Count * xStep}");
	}

	// --- Optional runtime tuning API ---
	public void SetMaxPoints(int n, bool preserve = true)
	{
		maxPoints = Mathf.Max(1, n);
		if (!preserve)
			Clear(refill: prefillWithZeros);
		else
		{
			while (values.Count > maxPoints)
				values.Dequeue();
			UpdateLine();
		}
	}

	public void SetScale(float newYScale) { yScale = newYScale; UpdateLine(); }
}
