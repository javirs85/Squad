using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEditor.Networking.PlayerConnection;
using UnityEngine;
using UnityEngine.InputSystem;

public class AlphaBarChar : MonoBehaviour, iAlphaController
{
	public GameObject FirstBar;
	public GameObject LastBar;
	public int segmentCount = 10;
	public Material ActiveMaterial;
	public Material InactiveMaterial;
	public Material MarkerMaterial;
	public TextMeshPro SerialNumberTextMesh;
	public TextMeshPro ScreenTextMesh;
	public GameObject ProgressBarObject;
	public ProgressBar ProgressBar;

	[Header("Ouputs")]
	public float StressAverage = 0.0f;
	public float RelaxAverage = 0.0f;

	public float AlphaValue { get; set; } = 0.0f;

	[Header("SignalProcessingMeasures")]
	public int TrainingSamples = 200;

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
		GoTo(Statuses.NothingReady);
	}

	float CurrentMarkerValue = 0;

	public void SetAlphaPosition(float v)
	{
		if(currentStatus == Statuses.MathMeasuring || currentStatus == Statuses.RelaxMeasuring)
		{
			CurrentMeasure.Add(v);

			ProgressBar.ProgressValue = (float)CurrentMeasure.Count / TrainingSamples;

			if (CurrentMeasure.Count >= TrainingSamples)
			{
				MathAlphaAverage = (int)math.round(CurrentMeasure.Average() * 100);

				if(currentStatus == Statuses.MathMeasuring)
				{
					StressAverage = MathAlphaAverage;
					GoTo(Statuses.MathReady);
				}
				else if (currentStatus == Statuses.RelaxMeasuring)
				{
					RelaxAlphaAverage = MathAlphaAverage;
					GoTo(Statuses.FreeRun);
				}

				Debug.Log($"Avg after {CurrentMeasure.Count} = {CurrentMeasure.Average()}");
			}
		}
		else if (currentStatus == Statuses.FreeRun)
			PaintBars(v);

	}

	private int MathSolution = 0;
	private void PrintNextMathChallenge()
	{

		var operation = UnityEngine.Random.Range(0, 4); // 0 = +, 1 = - , 2 = *, 3 = /
		string TextOnScreen = "";
		if (operation == 0)
		{
			TextOnScreen = "+";
			var i = UnityEngine.Random.Range(7, 50);
			TextOnScreen += i.ToString();
			MathSolution += i;
		}
		else if (operation == 1)
		{
			TextOnScreen = "-";

			var i = 3;
			if (MathSolution > 5)
			{
				i = UnityEngine.Random.Range(3, MathSolution-3);
			}
			TextOnScreen += i.ToString();
			MathSolution -= i;
		}
		else if (operation == 2)
		{
			TextOnScreen = "x";
			var i = UnityEngine.Random.Range(2, 4);
			TextOnScreen += i.ToString();
			MathSolution *= i;
		}
		else if (operation == 3)
		{
			TextOnScreen = "/";
			var i = 0;
			if (MathSolution % 5 == 0) i = 5;

			else if (MathSolution % 4 == 0) i = 4;
			else if (MathSolution % 3 == 0) i = 3;
			else if (MathSolution % 2 == 0) i = 2;

			if(i == 0)
			{
				TextOnScreen = "+";
				var ni = UnityEngine.Random.Range(7, 50);
				TextOnScreen += ni.ToString();
				MathSolution += ni;
			}
			else
			{
				TextOnScreen += i.ToString();
				MathSolution /= i;
			}				
		}

		ScreenTextMesh.text = TextOnScreen;
	}

	int NextAutoCorrect = 0;
	int SamplesToAutoCorrect = 10;

	private void PaintBars(float v)
	{
		if (NextAutoCorrect == 0)
		{
			NextAutoCorrect = SamplesToAutoCorrect;
			if (GoingTo == GoingToDirections.Up)
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

			if (GoingTo != GoingToDirections.noWhere && i == CurrentMarkerValue)
				BarStatus = BarStatuses.Marker;

			if (BarStatus == BarStatuses.Marker && i == 0)
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



	private DateTime NextMathChallengeTime = DateTime.Now;
	// Update is called once per frame
	void Update()
	{
		if(currentStatus == Statuses.NothingReady)
		{
			if (IsScreenClicked())
				GoTo(Statuses.MathMeasuring);
		}
		else if(currentStatus == Statuses.MathMeasuring)
		{
			if(DateTime.Now > NextMathChallengeTime)
			{
				PrintNextMathChallenge();
				NextMathChallengeTime = DateTime.Now.AddSeconds(3);
			}
		}
		else if(currentStatus == Statuses.MathReady)
		{
			CurrentMeasure.Clear();
			if (IsScreenClicked())
				GoTo(Statuses.RelaxMeasuring);
			
		}
		else if(currentStatus == Statuses.RelaxMeasuring)
		{

		}
		else if (currentStatus == Statuses.FreeRun)
		{
			if (IsScreenClicked())
				GoTo(Statuses.NothingReady);
		}
	}

	public int MathAlphaAverage { get; set; } = 0;
	public int RelaxAlphaAverage { get; set; } = 0;

	enum Statuses { NothingReady, MathMeasuring, MathReady, RelaxMeasuring, FreeRun };
	Statuses currentStatus = Statuses.NothingReady;

	List<float> CurrentMeasure = new();
	

	public void StartMathTraining()
	{
		GoTo(Statuses.MathMeasuring);
	}

	public void StartRelaxTraining()
	{
		GoTo(Statuses.RelaxMeasuring);
	}

	public void FreeRun()
	{
		GoTo(Statuses.FreeRun);
	}


	private void GoTo(Statuses newStatus)
	{
		if(newStatus == Statuses.NothingReady)
		{
			HideAllBars();
			ScreenTextMesh.text = "Tap the screen to start the stress test";
			ProgressBarObject.SetActive(false);
		}
		else if(newStatus == Statuses.MathMeasuring)
		{
			HideAllBars();
			CurrentMeasure.Clear();
			MathSolution = UnityEngine.Random.Range(7, 50);
			ScreenTextMesh.text = MathSolution.ToString();
			NextMathChallengeTime = DateTime.Now.AddSeconds(3);
			ProgressBar.ProgressValue = 0;
			ProgressBarObject.SetActive(true);
		}
		else if(newStatus == Statuses.MathReady)
		{
			HideAllBars(); 
			ProgressBarObject.SetActive(false);
			ScreenTextMesh.text = "Tap the screen to start the relax test";
		}
		else if(newStatus == Statuses.RelaxMeasuring)
		{
			ScreenTextMesh.text = "Keep your eyes closed";
			ProgressBarObject.SetActive(true);
		}
		else if (newStatus == Statuses.FreeRun)
		{
			ScreenTextMesh.text = "Ready! Move your alpha UP to shoot!";
			ProgressBarObject.SetActive(false);
			ShowAllBars();
		}

		currentStatus = newStatus;
	}

	private bool IsScreenClicked()
	{
		if (Mouse.current is not null && Mouse.current.leftButton.wasPressedThisFrame)
		{
			Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
			{
				if (hit.collider != null)
				{
					if (hit.collider.name != null && hit.collider.name == "AlphaScreen")
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	private void HideAllBars()
	{
		foreach (var bar in bars)
		{
			bar.SetActive(false);
		}
	}
	private void ShowAllBars()
	{
		foreach (var bar in bars)
		{
			bar.SetActive(true);
		}
	}
}
