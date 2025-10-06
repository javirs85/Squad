using Gtec.Chain.Common.Nodes.FilterNodes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BCI : MonoBehaviour
{

	[SerializeField] SimplePlot alphaPlot;
	[SerializeField] SimplePlot eiPlot;
	[SerializeField] SimplePlot alphaPlotReference;
	[SerializeField] SimplePlot eiPlotReference;


	public enum Metrics { NotSet, Alpha, EI, }
	public enum MeasuringStatuses { Nothing, Stress, Relax, FreeRun }
	public enum MindStatuses { Unknown, Relaxed, Stressed }


	public Metrics ChosenMetric = Metrics.NotSet;
	public float CurrentThreshold => ChosenMetric == Metrics.Alpha ? AlphaThreshold : ChosenMetric == Metrics.EI ? EIThreshold : 0;
	public float AlphaThreshold { get; set; } = 0;	
	public float EIThreshold { get; set; } = 0;	

	public MeasuringStatuses CurrentMeasuringStatus = MeasuringStatuses.Nothing;
	public MindStatuses CurrentMindStatus { get; set; } = MindStatuses.Unknown;

	public void StartStressMeasuring() { CurrentMeasuringStatus = MeasuringStatuses.Stress; }
	public void StartRelaxMeasuring() { CurrentMeasuringStatus = MeasuringStatuses.Relax; }

	public void FinishStressMeasuring() { CurrentMeasuringStatus = MeasuringStatuses.Nothing; }
	public void FinishRelaxMeasuring() {
		CurrentMeasuringStatus = MeasuringStatuses.Nothing;
		UpdateAverages();
	}

	public void StartFreeRun() { 
		CurrentMeasuringStatus = MeasuringStatuses.FreeRun;
		FreeRunValues.Clear();
	}
	public void FinishFreeRun() { 
		CurrentMeasuringStatus = MeasuringStatuses.Nothing;
		MindStatuses MindStatus = CalculateFreeRunMindStatus();
	}

	private void UpdateAverages()
	{
		AlphaBasedStressAverage = StressTimeValues.Select(x => x.Alpha).Average();
		AlphaBasedRelaxAverage = RelaxTimeValues.Select(x => x.Alpha).Average();

		EIBasedStressAverage = StressTimeValues.Select(x => x.Beta / (x.Alpha + x.Theta)).Average();
		EIBasedRelaxAverage = RelaxTimeValues.Select(x => x.Beta / (x.Alpha + x.Theta)).Average();

		float dAlpha = ComputeCohensD(
			StressTimeValues.Select(x => x.Alpha).ToList(),
			RelaxTimeValues.Select(x => x.Alpha).ToList());

		float dEI = ComputeCohensD(
			StressTimeValues.Select(x => x.Beta / (x.Alpha + x.Theta)).ToList(),
			RelaxTimeValues.Select(x => x.Beta / (x.Alpha + x.Theta)).ToList());

		AlphaThreshold = (AlphaBasedStressAverage + AlphaBasedRelaxAverage) / 2f;
		EIThreshold = (EIBasedStressAverage + EIBasedRelaxAverage) / 2f;
	}
	private MindStatuses CalculateFreeRunMindStatus()
	{
		float meanValue;

		if (ChosenMetric == Metrics.Alpha)
		{
			meanValue = FreeRunValues.Select(x => x.Alpha).Average();
			// typically: higher alpha → more relaxed
			CurrentMindStatus = (meanValue > CurrentThreshold)
				? MindStatuses.Relaxed
				: MindStatuses.Stressed;
		}
		else // Engagement Index
		{
			meanValue = FreeRunValues.Select(x => x.Beta / (x.Alpha + x.Theta)).Average();
			// typically: higher EI → more stressed / engaged
			CurrentMindStatus = (meanValue > CurrentThreshold)
				? MindStatuses.Stressed
				: MindStatuses.Relaxed;
		}

		return CurrentMindStatus;
	}

	public float AlphaBasedStressAverage { get; set; }
	public float AlphaBasedRelaxAverage { get; set; }
	public float EIBasedStressAverage { get; set; }
	public float EIBasedRelaxAverage { get; set; }

	private List<BandPowersValue> StressTimeValues = new();
	private List<BandPowersValue> RelaxTimeValues = new();
	private List<BandPowersValue> FreeRunValues = new();


	public void ProcessNewMeanBandPowerSample(float alpha, float beta, float theta)
	{
		if (CurrentMeasuringStatus == MeasuringStatuses.Stress) {
			StressTimeValues.Add(new BandPowersValue(alpha, beta, theta)); 
			alphaPlot.AddValue(FreeRunValues.Select(x => x.Alpha).Average()); 
			eiPlot.AddValue(FreeRunValues.Select(x => x.Beta / (x.Alpha + x.Theta)).Average());
			alphaPlotReference.AddValue(0f);
			eiPlotReference.AddValue(0f);
		}

		else if (CurrentMeasuringStatus == MeasuringStatuses.Relax) {
			RelaxTimeValues.Add(new BandPowersValue(alpha, beta, theta));
			alphaPlot.AddValue(FreeRunValues.Select(x => x.Alpha).Average());
			eiPlot.AddValue(FreeRunValues.Select(x => x.Beta / (x.Alpha + x.Theta)).Average());
			alphaPlotReference.AddValue(0f);
			eiPlotReference.AddValue(0f);
		}

		else if (CurrentMeasuringStatus == MeasuringStatuses.FreeRun)
		{
			FreeRunValues.Add(new BandPowersValue(alpha, beta, theta));
			alphaPlot.AddValue(FreeRunValues.Select(x => x.Alpha).Average());
			eiPlot.AddValue(FreeRunValues.Select(x => x.Beta / (x.Alpha + x.Theta)).Average());
			alphaPlotReference.AddValue(AlphaThreshold);
			eiPlotReference.AddValue(EIThreshold);
		}

	}

	private float ComputeCohensD(List<float> stress, List<float> relax)
	{
		float meanStress = stress.Average();
		float meanRelax = relax.Average();
		float varStress = stress.Select(v => (v - meanStress) * (v - meanStress)).Average();
		float varRelax = relax.Select(v => (v - meanRelax) * (v - meanRelax)).Average();
		float pooledStd = (float)Math.Sqrt((varStress + varRelax) / 2f);

		return Math.Abs(meanStress - meanRelax) / pooledStd;
	}


}

public class BandPowersValue
{
	public float Alpha;
	public float Beta;
	public float Theta;

	public BandPowersValue(float _alpha, float _beta, float _theta)
	{
		Alpha = _alpha; Beta = _beta; Theta = _theta;
	}
}




