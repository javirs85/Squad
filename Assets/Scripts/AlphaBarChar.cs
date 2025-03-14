using NUnit.Framework;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class AlphaBarChar : MonoBehaviour, iAlphaController
{
	public GameObject FirstBar;
	public GameObject LastBar;
	public int segmentCount = 10;
	public Material ActiveMaterial;
	public Material InactiveMaterial;
	public Material MarkerMaterial;
	public float AlphaValue { get; set; } = 0.0f;

	enum GoingToDirections { Up, Down, noWhere};
	GoingToDirections GoingTo = GoingToDirections.Up;
	public enum BarStatuses { Active, Inactive, Marker };


	private List<GameObject> bars = new List<GameObject>();

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		Transform parentTransform = FirstBar.transform.parent; // Keep reference to the parent
		bars.Add(FirstBar);

		for (int i = 1; i <= segmentCount - 1; i++)
		{
			float t = (float)i / segmentCount;
			Vector3 newPosition = Vector3.Lerp(FirstBar.transform.position, LastBar.transform.position, t);
			GameObject newBar = Instantiate(FirstBar, newPosition, FirstBar.transform.rotation, parentTransform);
			newBar.transform.localScale = FirstBar.transform.localScale;
			bars.Add(newBar);
		}
		bars.Add(LastBar);

		SetAlphaPosition(0);
	}

	float CurrentMarkerValue = 0;

	int SamplesToAutoCorrect = 20;
	int NextAutoCorrect = 0;


	public void SetAlphaPosition(float v)
	{
		if (NextAutoCorrect == 0)
		{
			NextAutoCorrect = SamplesToAutoCorrect;
			if(GoingTo == GoingToDirections.Up)
				CurrentMarkerValue--;
			else if (GoingTo == GoingToDirections.Down)
				CurrentMarkerValue++;
		}
		else
			NextAutoCorrect--;

		int activeBars = (int)(v * segmentCount / 10);

		activeBars = math.max(activeBars, 0);
		activeBars = math.min(activeBars, 10);


		activeBars = math.min(activeBars, 10);
		
		for (int i = 0; i <= segmentCount; i++)
		{
			var BarStatus = BarStatuses.Inactive;
			if (i <= activeBars)
				BarStatus = BarStatuses.Active;

			if (GoingTo == GoingToDirections.Up)
			{
				if (activeBars > CurrentMarkerValue)
				{
					CurrentMarkerValue = activeBars;
				}
			}
			else if (GoingTo == GoingToDirections.Down)
			{
				if (activeBars < CurrentMarkerValue)
				{
					CurrentMarkerValue = activeBars;
				}
			}

			if(GoingTo != GoingToDirections.noWhere && i == CurrentMarkerValue)
				BarStatus = BarStatuses.Marker;

			if(BarStatus == BarStatuses.Marker && i == 0)
			{
				BarStatus = BarStatuses.Marker;
			}

			SetBarActive(bars[i], BarStatus);
		}
	}


	public void SetReferenceValue(float alpha)
	{
		if(alpha == 1)
		{
			GoingTo = GoingToDirections.Up;
			CurrentMarkerValue = 0;
		}
		else if (alpha == 0)
		{
			GoingTo = GoingToDirections.Down;
			CurrentMarkerValue = 10;
		}
		else
			GoingTo = GoingToDirections.noWhere;
	}

	public void SetBarActive(GameObject bar, BarStatuses barStatus)
	{
		Renderer renderer = bar.GetComponent<Renderer>();
		if (renderer != null)
		{
			renderer.material = barStatus switch
			{
				BarStatuses.Active => ActiveMaterial,
				BarStatuses.Inactive => InactiveMaterial,
				BarStatuses.Marker => MarkerMaterial,
				_ => renderer.material
			};
		}
	}


	// Update is called once per frame
	void Update()
	{

	}

}
