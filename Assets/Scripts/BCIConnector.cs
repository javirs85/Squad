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

public class BCIConnector : DevicesManager

{

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	public override void Start()
	{
		base.Start();

		if(Unicorn is not null)
		{
			//_bci.OnDevicesAvailable.AddListener(UpdateAvailableDevices);
			Unicorn.OnDeviceStateChanged.AddListener(OnDeviceStateChanged);
			Unicorn.OnMeanBandpowerAvailable.AddListener(OnBandPowerChanges);
			Unicorn.OnSignalQualityAvailable.AddListener(OnNewSignalQualityChanges);
			Unicorn.OnBatteryLevelAvailable.AddListener(OnBatteryLevelChanges);
		}
		
		//Game = GetComponent<GameController>();		
	}

	public void OnBandPowerChanges(Dictionary<string, double> arg0)
	{
		if (GameController.instance is null) return;

		double alpha = arg0["alpha"];
		double beta = arg0["beta-mid"];
		double theta = arg0["theta"];
		//Debug.Log(alpha);
		//GameController.instance.SetAlphaCurrentPosition((float)alpha);
		GameController.instance.SetAlphaBetaThetaValues((float)alpha, (float)beta, (float)theta);
	}

	List<ChannelQuality.ChannelStates> CurrentQualities = new List<ChannelQuality.ChannelStates>();

	public void OnNewSignalQualityChanges(List<ChannelQuality.ChannelStates> arg0)
	{
		if (GameController.instance is null) return;

		if (CurrentQualities.Count != arg0.Count)
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
					GameController.instance.MakeFriendJerk(i);
				else if (arg0[i] == ChannelQuality.ChannelStates.BadGrounded)
                    GameController.instance.HideFriend(i);
				else if (arg0[i] == ChannelQuality.ChannelStates.Good)
                    GameController.instance.MakeFriendHappy(i);

				CurrentQualities[i] = arg0[i];
			}
		}
	}


	//only for the already connected device
	private void OnDeviceStateChanged(DataAcquisitionUnit.States arg0)
	{
		if(ConnectedSN is null || arg0 == DataAcquisitionUnit.States.Connecting) return;

		Debug.Log(ConnectedSN + " changed to: " + arg0.ToString());

		//if(arg0 == DataAcquisitionUnit.States.Acquiring) GameController.instance.ShowAllFriends();
		//if(arg0 == DataAcquisitionUnit.States.Disconnected) GameController.instance.HideAllFriends();
	}

	private void OnBatteryLevelChanges(float arg0)
	{
		if (GameController.instance is null) return;
		GameController.instance.SetBatteryLevel(arg0);
	}


	// Update is called once per frame
	void Update()
	{

	}
}
