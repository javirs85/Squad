using Gtec.Bandpower;
using Gtec.Chain.Common.Nodes.InputNodes;
using Gtec.Chain.Common.Templates.DataAcquisitionUnit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Rendering.GPUSort;

public class BCIConnector : MonoBehaviour
{
	private Device _bci;
	public GameController Game;
	private string connectedSN;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		_bci = GetComponent<Device>();
		if(_bci is not null)
		{
			_bci.OnDevicesAvailable.AddListener(UpdateAvailableDevices);
			_bci.OnDeviceStateChanged.AddListener(OnDeviceStateChanged);
			_bci.OnBandpowerMeanAvailable.AddListener(OnBandPowerChanges);
			_bci.OnSignalQualityAvailable.AddListener(OnNewSignalQualityChanges);
		}
		
		Game = GetComponent<GameController>();		
	}

	List<ChannelQuality.ChannelStates> CurrentQualities = new List<ChannelQuality.ChannelStates>();

	public void OnNewSignalQualityChanges(List<ChannelQuality.ChannelStates> arg0)
	{
		if(CurrentQualities.Count != arg0.Count)
		{
			CurrentQualities.Clear();
			for(int i=0; i<arg0.Count; i++) CurrentQualities.Add(ChannelQuality.ChannelStates.Good);
		}
		string dbg = "";
		foreach(var ch in arg0)
		{
			dbg += ch.ToString() + ", ";
		}
		//Debug.Log(dbg);

		for (int i = 0; i < arg0.Count; i++)
		{
			if(arg0[i] != CurrentQualities[i])
			{
				if (arg0[i] == ChannelQuality.ChannelStates.BadFloating)
					Game.MakeFriendJerk(i);
				else if (arg0[i] == ChannelQuality.ChannelStates.BadGrounded) 
					Game.HideFriend(i);
				else if (arg0[i] == ChannelQuality.ChannelStates.Good) 
					Game.MakeFriendHappy(i);

				CurrentQualities[i] = arg0[i];
			}
		}
	}

	List<double> AlphaValues = new List<double>();
	int FirstSamplesIgnored = 10;

	public void OnBandPowerChanges(Dictionary<string, double> arg0)
	{

		double alpha = arg0["alpha"];

		if (FirstSamplesIgnored >  0)
		{ 
			FirstSamplesIgnored--;
		}
		else
		{
			AlphaValues.Add(alpha);
			if (AlphaValues.Count > 50)
			{
				AlphaValues.Add(alpha);
				if (AlphaValues.Count > 100)
					AlphaValues.RemoveAt(0);

				//float alphaPercent = Mathf.InverseLerp((float)AlphaValues.Min(), (float)AlphaValues.Max(), (float)alpha) * 100f;
				//Debug.Log($"{alpha.ToString("F2")}: ({AlphaValues.Min().ToString("F2")} / {AlphaValues.Max().ToString("F2")}) => {alphaPercent} [{AlphaValues.Average()}]");


				var ScreenValue = Mathf.InverseLerp(-1.0f, 8.0f, (float)alpha);
				Debug.Log(ScreenValue);
				Game.SetAlphaCurrentPosition(ScreenValue);
			}
			else
				Debug.Log($"Loading ... {AlphaValues.Count*2}%");
		}

	}

	//only for the already connected device
	private void OnDeviceStateChanged(DataAcquisitionUnit.States arg0)
	{
		Debug.Log(connectedSN + " changed to: " + arg0.ToString());
		if(arg0 == DataAcquisitionUnit.States.Acquiring) Game.ShowAllFriends();
		if(arg0 == DataAcquisitionUnit.States.Disconnected) Game.HideAllFriends();
	}

	//get's the list of all available devices
	private void UpdateAvailableDevices(List<string> arg0)
	{
		if (arg0 is not null)
		{
			if (arg0.Count == 1)
			{
				connectedSN = arg0[0];
				_bci.Connect(connectedSN);
			}
			if (arg0.Count > 2) // the "only on simulator mode needs to be fixed"
			{
				Task.Run(() => StartDeviceSelection(arg0.ToList()));
			}
		}
	}

	async Task StartDeviceSelection(List<string> options)
	{
		var SelectedAmplifier = string.Empty;
		while(string.IsNullOrEmpty(SelectedAmplifier))
		{
			Debug.Log($"Starting amp selection");
			SelectedAmplifier = await Game.GetUserSelection(options);
		}
		Debug.Log($"Selected amp: {SelectedAmplifier}");

		_bci.Connect(SelectedAmplifier);

		await Game.DestroyAmplifiersOnScreenAsync();
		await Game.StartTheGame();
	}

	// Update is called once per frame
	void Update()
	{

	}
}
