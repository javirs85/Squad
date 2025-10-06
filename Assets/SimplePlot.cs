using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SimplePlot : MonoBehaviour
{
	[SerializeField] int maxPoints = 200;
	[SerializeField] float xStep = 0.02f;
	[SerializeField] float yScale = 0.1f;

	LineRenderer line;
	readonly Queue<float> values = new();

	void Awake()
	{
		line = GetComponent<LineRenderer>();
		line.useWorldSpace = false;                 // keep relative to parent (optional)
		line.widthCurve = AnimationCurve.Constant(0, 1, 0.01f); // 0.01f = 1 cm width
	}

	public void AddValue(float v)
	{
		values.Enqueue(v);
		if (values.Count > maxPoints) values.Dequeue();

		int i = 0;
		line.positionCount = values.Count;
		foreach (var val in values)
		{
			// shift along X, scale Y
			line.SetPosition(i, new Vector3(i * xStep, val * yScale, 0));
			i++;
		}
	}
}
