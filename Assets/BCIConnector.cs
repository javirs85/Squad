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
			_bci.OnMeanBandpowerAvailable.AddListener(OnBandPowerChanges);
			_bci.OnSignalQualityAvailable.AddListener(OnNewSignalQualityChanges);
		}
		
		Game = GetComponent<GameController>();		
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
				if (arg0[i] == ChannelQuality.ChannelStates.BadFloating) Game.MakeFriendJerk(i);
				else if (arg0[i] == ChannelQuality.ChannelStates.BadGrounded) Game.HideFriend(i);
				else if (arg0[i] == ChannelQuality.ChannelStates.Good) Game.MakeFriendHappy(i);
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
			if (arg0.Count > 0)
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
		await Game.DestroyAmplifiersOnScreenAsync();
		await Game.StartTheGame();
	}

	// Update is called once per frame
	void Update()
	{

	}
}
