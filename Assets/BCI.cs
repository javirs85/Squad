using Gtec.Chain.Common.Nodes.FilterNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class BCI : MonoBehaviour
{
	[Header("Plots")]
	[SerializeField] SimplePlot alphaPlot;
	[SerializeField] SimplePlot eiPlot;
	[SerializeField] SimplePlot TaskReference;
	[SerializeField] TextMeshPro DebugChalk;

	public enum Metrics { NotSet, Alpha, EI }
	public enum MeasuringStatuses { Nothing, Stress, Relax, FreeRun }
	public enum MindStatuses { Unknown, Relaxed, Stressed }

	public Metrics ChosenMetric = Metrics.NotSet;
	public MeasuringStatuses CurrentMeasuringStatus = MeasuringStatuses.Nothing;
	public MindStatuses CurrentMindStatus { get; private set; } = MindStatuses.Unknown;

	public float AlphaThreshold { get; private set; } = 0f;
	public float EIThreshold { get; private set; } = 0f;
	public float CurrentThreshold =>
		ChosenMetric == Metrics.Alpha ? AlphaThreshold :
		ChosenMetric == Metrics.EI ? EIThreshold : 0f;

	public float AlphaBasedStressAverage { get; private set; }
	public float AlphaBasedRelaxAverage { get; private set; }
	public float EIBasedStressAverage { get; private set; }
	public float EIBasedRelaxAverage { get; private set; }

	private readonly List<BandPowersValue> StressTimeValues = new();
	private readonly List<BandPowersValue> RelaxTimeValues = new();
	private readonly List<BandPowersValue> FreeRunValues = new();

	// ---------------------------------------------------------

	#region Measuring control


	public void StartStressMeasuring() => CurrentMeasuringStatus = MeasuringStatuses.Stress;
	public void StartRelaxMeasuring() => CurrentMeasuringStatus = MeasuringStatuses.Relax;

	public void FinishStressMeasuring()
	{
		CurrentMeasuringStatus = MeasuringStatuses.Nothing;
	}

	public void FinishRelaxMeasuring()
	{
		CurrentMeasuringStatus = MeasuringStatuses.Nothing;
		UpdateAverages();
	}

	public void StartFreeRun()
	{
		CurrentMeasuringStatus = MeasuringStatuses.FreeRun;
		FreeRunValues.Clear();
	}

	public void FinishFreeRun()
	{
		CurrentMeasuringStatus = MeasuringStatuses.Nothing;
		CurrentMindStatus = CalculateFreeRunMindStatus();
	}

	#endregion

	// ---------------------------------------------------------

	// convert to power for EI-based metrics
	float Power(float db) => Mathf.Pow(10f, db / 10f);

	private void UpdateAverages()
	{
		// mean values
		AlphaBasedStressAverage = StressTimeValues.Select(x => x.Alpha).Average();
		AlphaBasedRelaxAverage = RelaxTimeValues.Select(x => x.Alpha).Average();	
		
		float test(float x) { return x + 2; };
		

		EIBasedStressAverage = StressTimeValues
			.Select(x => Power(x.Beta) / (Power(x.Alpha) + Power(x.Theta)))
			.Average();

		EIBasedRelaxAverage = RelaxTimeValues
			.Select(x => Power(x.Beta) / (Power(x.Alpha) + Power(x.Theta)))
			.Average();

		// effect sizes
		float dAlpha = ComputeCohensD(
			StressTimeValues.Select(x => x.Alpha).ToList(),
			RelaxTimeValues.Select(x => x.Alpha).ToList());

		float dEI = ComputeCohensD(
			StressTimeValues
				.Select(x => Power(x.Beta) / (Power(x.Alpha) + Power(x.Theta)))
				.ToList(),
			RelaxTimeValues
				.Select(x => Power(x.Beta) / (Power(x.Alpha) + Power(x.Theta)))
				.ToList());

		// thresholds
		AlphaThreshold = (AlphaBasedStressAverage + AlphaBasedRelaxAverage) / 2f;
		EIThreshold = (EIBasedStressAverage + EIBasedRelaxAverage) / 2f;

		// auto-select metric
		ChosenMetric = Mathf.Abs(dAlpha) > Mathf.Abs(dEI) ? Metrics.Alpha : Metrics.EI;

		DebugChalk.text =
			$"A_r:{AlphaBasedRelaxAverage:F1}  A_s:{AlphaBasedStressAverage:F1}  " +
			$"EI_r:{EIBasedRelaxAverage:F1}  EI_s:{EIBasedStressAverage:F1}\n" +
			$"dA:{dAlpha:F2}  dEI:{dEI:F2}  Metric:{ChosenMetric}";
	}

	private MindStatuses CalculateFreeRunMindStatus()
	{
		if (!FreeRunValues.Any()) return MindStatuses.Unknown;

		float meanValue;
		if (ChosenMetric == Metrics.Alpha)
		{
			meanValue = FreeRunValues.Average(x => x.Alpha);
			CurrentMindStatus = meanValue > CurrentThreshold ? MindStatuses.Relaxed : MindStatuses.Stressed;
		}
		else
		{
			meanValue = FreeRunValues.Average(x => x.Beta / (x.Alpha + x.Theta));
			CurrentMindStatus = meanValue > CurrentThreshold ? MindStatuses.Stressed : MindStatuses.Relaxed;
		}

		return CurrentMindStatus;
	}

	// ---------------------------------------------------------

	public void ProcessNewMeanBandPowerSample(float alpha, float beta, float theta)
	{
		var sample = new BandPowersValue(alpha, beta, theta);
		var ei = Power(beta) / (Power(alpha) + Power(theta));

		switch (CurrentMeasuringStatus)
		{
			case MeasuringStatuses.Stress:
				StressTimeValues.Add(sample);
				alphaPlot.AddValue(Math.Max( Math.Min(alpha/10, 2), 0));
				eiPlot.AddValue(ei);
				TaskReference.AddValue(1);
				break;

			case MeasuringStatuses.Relax:
				RelaxTimeValues.Add(sample);
				alphaPlot.AddValue(Math.Max(Math.Min(alpha / 10, 2), 0));
				eiPlot.AddValue(ei);
				TaskReference.AddValue(-1);
				break;

			case MeasuringStatuses.FreeRun:
				FreeRunValues.Add(sample);
				alphaPlot.AddValue(Math.Max(Math.Min(alpha / 10, 2), 0));
				eiPlot.AddValue(ei);
				TaskReference.AddValue(0);
				break;

			case MeasuringStatuses.Nothing:
				alphaPlot.AddValue(Math.Max(Math.Min(alpha / 10, 2), 0));
				eiPlot.AddValue(ei);
				TaskReference.AddValue(0);
				break;
		}
	}

	// ---------------------------------------------------------

	private float ComputeCohensD(List<float> stress, List<float> relax)
	{
		float meanStress = stress.Average();
		float meanRelax = relax.Average();
		float varStress = stress.Select(v => (v - meanStress) * (v - meanStress)).Average();
		float varRelax = relax.Select(v => (v - meanRelax) * (v - meanRelax)).Average();
		float pooledStd = Mathf.Sqrt((varStress + varRelax) / 2f);

		return pooledStd > 0 ? Mathf.Abs(meanStress - meanRelax) / pooledStd : 0f;
	}
}

// ---------------------------------------------------------

public class BandPowersValue
{
	public float Alpha;
	public float Beta;
	public float Theta;

	public BandPowersValue(float alpha, float beta, float theta)
	{
		Alpha = alpha;
		Beta = beta;
		Theta = theta;
	}
}
