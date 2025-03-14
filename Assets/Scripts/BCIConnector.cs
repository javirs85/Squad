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
	//public GameController Game;
	private string connectedSN;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		DontDestroyOnLoad(this);

		_bci = GetComponent<Device>();
		if(_bci is not null)
		{
			//_bci.OnDevicesAvailable.AddListener(UpdateAvailableDevices);
			_bci.OnDeviceStateChanged.AddListener(OnDeviceStateChanged);
			_bci.OnMeanBandpowerAvailable.AddListener(OnBandPowerChanges);
			_bci.OnSignalQualityAvailable.AddListener(OnNewSignalQualityChanges);
		}
		
		//Game = GetComponent<GameController>();		
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

	public void OnBandPowerChanges(Dictionary<string, double> arg0)
	{
		if (GameController.instance is null) return;

		double alpha = arg0["alpha"];
		Debug.Log(alpha);
		GameController.instance.SetAlphaCurrentPosition((float)alpha);
	}

	//only for the already connected device
	private void OnDeviceStateChanged(DataAcquisitionUnit.States arg0)
	{
		if(connectedSN is null || arg0 == DataAcquisitionUnit.States.Connecting) return;

		Debug.Log(connectedSN + " changed to: " + arg0.ToString());
		if(arg0 == DataAcquisitionUnit.States.Acquiring) GameController.instance.ShowAllFriends();
		if(arg0 == DataAcquisitionUnit.States.Disconnected) GameController.instance.HideAllFriends();
	}


	// Update is called once per frame
	void Update()
	{

	}
}
