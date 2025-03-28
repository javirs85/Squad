using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
//using UnityEditor.Networking.PlayerConnection;
using UnityEngine;
using UnityEngine.InputSystem;

public class AlphaBarChar : MonoBehaviour, iAlphaController
{
	public GameObject FirstBar;
	public GameObject LastBar;
	public GameObject FirstPointsBar;
	public GameObject LastPointsBar;
	public int segmentCount = 10;
	public Material ActiveMaterial;
	public Material InactiveMaterial;
	public Material MarkerMaterial;
	public Material PointNo;
	public Material PointYes;
	public TextMeshPro SerialNumberTextMesh;
	public TextMeshPro ScreenTextMesh;
	public GameObject ProgressBarObject;
	public ProgressBar ProgressBar;

	[Header("Ouputs")]
	public float StressAverage = 0.0f;
	public float RelaxAverage = 10.0f;

	public float AlphaValue { get; set; } = 0.0f;

	[Header("SignalProcessingMeasures")]
	public int TrainingSamples = 200;

	enum GoingToDirections { Up, Down, noWhere};
	GoingToDirections GoingTo = GoingToDirections.Up;
	public enum BarStatuses { Active, Inactive, Marker };

	private AudioSource Audio;
	private List<GameObject> bars = new List<GameObject>();
	private List<GameObject> Points = new List<GameObject>();
	private int CurrentPoints = 0;


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		Audio = GetComponent<AudioSource>();

		Transform parentTransform = FirstBar.transform.parent; // Keep reference to the parent
		bars.Add(FirstBar);

		for (int i = 1; i < segmentCount - 1; i++)
		{
			float t = (float)i / (segmentCount-1);
			Vector3 newPosition = Vector3.Lerp(FirstBar.transform.position, LastBar.transform.position, t);
			GameObject newBar = Instantiate(FirstBar, newPosition, FirstBar.transform.rotation, parentTransform);
			newBar.transform.localScale = FirstBar.transform.localScale;
			bars.Add(newBar);
		}
		bars.Add(LastBar);



		Points.Add(FirstPointsBar);

		for (int i = 1; i < segmentCount - 1; i++)
		{
			float t = (float)i / (segmentCount-1);
			Vector3 newPosition = Vector3.Lerp(FirstPointsBar.transform.position, LastPointsBar.transform.position, t);
			GameObject newBar = Instantiate(FirstPointsBar, newPosition, FirstPointsBar.transform.rotation, parentTransform);
			newBar.transform.localScale = FirstPointsBar.transform.localScale;
			SetPoints(newBar, false);
			Points.Add(newBar);
		}
		Points.Add(LastPointsBar);
		SetPoints(LastPointsBar, false);


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
				MathAlphaAverage = CurrentMeasure.Average();

				if(currentStatus == Statuses.MathMeasuring)
				{
					StressAverage = MathAlphaAverage;
					GoTo(Statuses.MathReady);
				}
				else if (currentStatus == Statuses.RelaxMeasuring)
				{
					RelaxAverage = MathAlphaAverage;
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

	


	private void PaintBars(float v)
	{
		int activeBars = CalculateMeterLevel(v, StressAverage, RelaxAverage);

		//Debug.Log($"{StressAverage}, {RelaxAverage} [{v}] => {activeBars}");


		for (int i = 0; i < bars.Count; i++)
		{
			var BarStatus = BarStatuses.Inactive;
			if (i <= activeBars)
				if (i <= segmentCount-2)
					BarStatus = BarStatuses.Active;
				else
					BarStatus = BarStatuses.Marker;

			SetBarActive(bars[i], BarStatus);
		}
	}
	int CalculateMeterLevel(double v, double StressAverage, double RelaxAverage)
	{
		if (StressAverage == RelaxAverage)
			throw new ArgumentException($"StressAverage and RelaxAverage cannot be the same value {StressAverage}, {RelaxAverage}.");

		int level = (int)Math.Round((v - StressAverage) / (RelaxAverage - StressAverage) * 10);
		return Math.Clamp(level, 0, 10);
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

	public void SetPoints(GameObject pointsBar, bool v)
	{
		Renderer renderer = pointsBar.GetComponent<Renderer>();
		if (renderer != null)
		{
			renderer.material = v ? PointYes : PointNo;
		}
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
		if(barStatus == BarStatuses.Marker)
		{
			CurrentPoints++;
			int ActivePoints = (int)math.round(CurrentPoints / 10);
			for (int i = 0; i < Points.Count; i++)
			{
				SetPoints(Points[i], i < ActivePoints);
			}

			if(CurrentPoints >= 100)
			{
				CurrentPoints = 0;
				GameController.instance.FireMissiles();
			}
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

	public float MathAlphaAverage { get; set; } = 0;

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
			HidePointsBars();
			ScreenTextMesh.text = "Tap the screen to start the stress test";
			ProgressBarObject.SetActive(false);
		}
		else if(newStatus == Statuses.MathMeasuring)
		{
			HideAllBars();
			CurrentMeasure.Clear();
			int a = UnityEngine.Random.Range(7, 50);
			int b = UnityEngine.Random.Range(7, 50);
			MathSolution = a+b;
			ScreenTextMesh.text = $"{a} + {b}";
			NextMathChallengeTime = DateTime.Now.AddSeconds(3);
			ProgressBar.ProgressValue = 0;
			ProgressBarObject.SetActive(true);
		}
		else if(newStatus == Statuses.MathReady)
		{
			HideAllBars();
			ProgressBar.ProgressValue = 0;
			ProgressBarObject.SetActive(false);
			ScreenTextMesh.text = "Tap the screen to start the relax test";
		}
		else if(newStatus == Statuses.RelaxMeasuring)
		{
			HideAllBars();
			CurrentMeasure.Clear();
			ScreenTextMesh.text = "Keep your eyes closed until you hear a beep";
			ProgressBar.ProgressValue = 0;
			ProgressBarObject.SetActive(true);
		}
		else if (newStatus == Statuses.FreeRun)
		{
			Audio.Play();
			ScreenTextMesh.text= "";
			ProgressBarObject.SetActive(false);
			ShowAllBars();
			ShowPointsBars();
			GameController.instance.ShowEnemy();
		}

		currentStatus = newStatus;
	}

	public void SimulateClick()
	{
		if (currentStatus == Statuses.NothingReady)
		{
			GoTo(Statuses.MathMeasuring);
		}
		else if (currentStatus == Statuses.MathReady)
		{
			GoTo(Statuses.RelaxMeasuring);
		}
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

	private void HidePointsBars()
	{
		foreach (var bar in Points)
		{
			bar.SetActive(false);
		}
	}

	private void ShowPointsBars()
	{
		foreach (var bar in Points)
		{
			bar.SetActive(true);
		}
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
