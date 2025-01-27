using Gtec.Bandpower;
using Gtec.Chain.Common.Nodes.InputNodes;
using Gtec.Chain.Common.Templates.DataAcquisitionUnit;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BCIConnector : MonoBehaviour
{
	private Device _bci;
	private GameController _controller;
	private string connectedSN;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		//_bci = GetComponent<Device>();
		//_bci.OnDevicesAvailable.AddListener(UpdateAvailableDevices);
		//_bci.OnDeviceStateChanged.AddListener(OnDeviceStateChanged);
		//_bci.OnBandpowerMeanAvailable.AddListener(OnBandPowerChanges);
		//_bci.OnSignalQualityAvailable.AddListener(OnNewSignalQualityChanges);
		//_controller = GetComponent<GameController>();
	}

	List<ChannelQuality.ChannelStates> CurrentQualities = new List<ChannelQuality.ChannelStates>();

	private void OnNewSignalQualityChanges(List<ChannelQuality.ChannelStates> arg0)
	{
		if(CurrentQualities.Count != arg0.Count)
		{
			CurrentQualities.Clear();
			CurrentQualities.AddRange(arg0);
		}

		for (int i = 0; i < arg0.Count; i++)
		{
			if(arg0[i] != CurrentQualities[i])
			{
				if (arg0[i] == ChannelQuality.ChannelStates.BadFloating) _controller.MakeFriendJerk(i);
				else if (arg0[i] == ChannelQuality.ChannelStates.BadGrounded) _controller.HideFriend(i);
				else if (arg0[i] == ChannelQuality.ChannelStates.Good) _controller.MakeFriendHappy(i);
			}
		}
	}

	private void OnBandPowerChanges(Dictionary<string, double> arg0)
	{
		string s = string.Empty;
		foreach (KeyValuePair<string, double> kvp in arg0)
		{
			s += kvp.Key + " : ";
			s += "[" + kvp.Value.ToString() + "]\n";
		}
		Debug.Log(s);
	}

	//only for the already connected device
	private void OnDeviceStateChanged(DataAcquisitionUnit.States arg0)
	{
		Debug.Log(connectedSN + " changed to: " + arg0.ToString());
		if(arg0 == DataAcquisitionUnit.States.Acquiring) _controller.ShowAllFriends();
		if(arg0 == DataAcquisitionUnit.States.Disconnected) _controller.HideAllFriends();
	}

	//get's the list of all available devices
	private void UpdateAvailableDevices(List<string> arg0)
	{
		//connectedSN = arg0[0];
		//_bci.Connect(connectedSN);
	}

	// Update is called once per frame
	void Update()
	{

	}
}
