using Gtec.Bandpower;
using Gtec.Chain.Common.Nodes.InputNodes;
using Gtec.Chain.Common.Templates.DataAcquisitionUnit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class DeviceProxy : MonoBehaviour
{
	public string ConfigurationSceneName = "TEST_UnicornConfiguration";

	[Header("Events")]
	[SerializeField]
	[Tooltip("The event called when devices are discovered.")]
	public UnityEvent<List<string>> OnDevicesAvailable;
	private void ForwardOnDevicesAvailable(List<string> data)
	{
		OnDevicesAvailable?.Invoke(data);
	}

	[SerializeField]
	[Tooltip("The event called when device state changed.")]
	public UnityEvent<DataAcquisitionUnit.States> OnDeviceStateChanged;
	private void ForwardOnDeviceStateChanged(DataAcquisitionUnit.States data)
	{
		OnDeviceStateChanged?.Invoke(data);
	}

	[SerializeField]
	[Tooltip("The event called when a pipeline state changed.")]
	public UnityEvent<string> OnPipelineStateChanged;
	private void ForwardOnPipelineStateChanged(string data) => OnPipelineStateChanged?.Invoke(data);

	[SerializeField]
	[Tooltip("The event called when a runtime exception occured.")]
	public UnityEvent<Exception> OnRuntimeExceptionOccured;
	private void ForwardOnRuntimeExceptionOccured(Exception data) => OnRuntimeExceptionOccured?.Invoke(data);

	[SerializeField]
	[Tooltip("The event called when bandpower values for each channel are available.")]
	public UnityEvent<Dictionary<string, double[]>> OnBandpowerAvailable;
	private void ForwardOnBandpowerAvailable(Dictionary<string, double[]> data) => OnBandpowerAvailable?.Invoke(data);

	[SerializeField]
	[Tooltip("The event called when averaged bandpower values over all channels are available.")]
	public UnityEvent<Dictionary<string, double>> OnMeanBandpowerAvailable;
	private void ForwardOnMeanBandpowerAvailable(Dictionary<string, double> data) => OnMeanBandpowerAvailable?.Invoke(data);


	[SerializeField]
	[Tooltip("The event called when bandpower ratios for each channel are available.")]
	public UnityEvent<Dictionary<string, double[]>> OnRatiosAvailable;
	private void ForwardOnRatiosAvailable(Dictionary<string, double[]> data) => OnRatiosAvailable?.Invoke(data);

	[SerializeField]
	[Tooltip("The event called when averaged bandpower ratios over all channels are available.")]
	public UnityEvent<Dictionary<string, double>> OnMeanRatiosAvailable;
	private void ForwardOnMeanRatiosAvailable(Dictionary<string, double> data) => OnMeanRatiosAvailable?.Invoke(data);

	[SerializeField]
	[Tooltip("The event called when new signal quality values are available.")]
	public UnityEvent<List<ChannelQuality.ChannelStates>> OnSignalQualityAvailable;
	private void ForwardOnSignalQualityAvailable(List<ChannelQuality.ChannelStates> data) => OnSignalQualityAvailable?.Invoke(data);

	[SerializeField]
	[Tooltip("The event called when battery level data is available.")]
	public UnityEvent<float> OnBatteryLevelAvailable;
	private void ForwardOnBatteryLevelAvailable(float data) => OnBatteryLevelAvailable?.Invoke(data);

	[SerializeField]
	[Tooltip("The event called when data is lost.")]
	public UnityEvent OnDataLost;
	private void ForwardOnDataLost() => OnDataLost?.Invoke();

	[SerializeField]
	[Tooltip("The event called when raw data is available.")]
	public UnityEvent<Rawdata> OnEEGDataAvailable;
	private void ForwardOnEEGDataAvailable(Rawdata data) => OnEEGDataAvailable?.Invoke(data);

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        if (DevicesManager.instance is null || !DevicesManager.instance.IsConnected)
        {
            GoToSettingsScene();
            return;
        }
        else
        {
            Join(DevicesManager.instance.Unicorn);
		}
    }

	public void Join(Device Unicorn)
	{
		Unicorn.OnDevicesAvailable.AddListener(ForwardOnDevicesAvailable);
		Unicorn.OnDeviceStateChanged.AddListener(ForwardOnDeviceStateChanged);
		Unicorn.OnPipelineStateChanged.AddListener(ForwardOnPipelineStateChanged);
		Unicorn.OnRuntimeExceptionOccured.AddListener(ForwardOnRuntimeExceptionOccured);
		Unicorn.OnMeanBandpowerAvailable.AddListener(ForwardOnMeanBandpowerAvailable);
		Unicorn.OnBandpowerAvailable.AddListener(ForwardOnBandpowerAvailable);
		Unicorn.OnRatiosAvailable.AddListener(ForwardOnRatiosAvailable);
		Unicorn.OnMeanRatiosAvailable.AddListener(ForwardOnMeanRatiosAvailable);
		Unicorn.OnSignalQualityAvailable.AddListener(ForwardOnSignalQualityAvailable);
		Unicorn.OnBatteryLevelAvailable.AddListener(ForwardOnBatteryLevelAvailable);
		Unicorn.OnDataLost.AddListener(ForwardOnDataLost);
		Unicorn.OnEEGDataAvailable.AddListener(ForwardOnEEGDataAvailable);

		Unicorn.OnDeviceStateChanged.AddListener(CatchDeviceChange);
		Unicorn.OnRuntimeExceptionOccured.AddListener(CatchDeviceException);
	}

    public void GoToSettingsScene()
    {
		DevicesManager.SceneToGoAfterConnection = SceneManager.GetActiveScene().name;
		SceneManager.LoadScene(ConfigurationSceneName);
    }

	private void OnDestroy()
	{
		if (DevicesManager.instance != null && DevicesManager.instance.Unicorn != null)
		{
			DevicesManager.instance.Unicorn.OnDevicesAvailable.RemoveListener(ForwardOnDevicesAvailable);
			DevicesManager.instance.Unicorn.OnDeviceStateChanged.RemoveListener(ForwardOnDeviceStateChanged);
			DevicesManager.instance.Unicorn.OnPipelineStateChanged.RemoveListener(ForwardOnPipelineStateChanged);
			DevicesManager.instance.Unicorn.OnRuntimeExceptionOccured.RemoveListener(ForwardOnRuntimeExceptionOccured);
			DevicesManager.instance.Unicorn.OnMeanBandpowerAvailable.RemoveListener(ForwardOnMeanBandpowerAvailable);
			DevicesManager.instance.Unicorn.OnBandpowerAvailable.RemoveListener(ForwardOnBandpowerAvailable);
			DevicesManager.instance.Unicorn.OnRatiosAvailable.RemoveListener(ForwardOnRatiosAvailable);
			DevicesManager.instance.Unicorn.OnMeanRatiosAvailable.RemoveListener(ForwardOnMeanRatiosAvailable);
			DevicesManager.instance.Unicorn.OnSignalQualityAvailable.RemoveListener(ForwardOnSignalQualityAvailable);
			DevicesManager.instance.Unicorn.OnBatteryLevelAvailable.RemoveListener(ForwardOnBatteryLevelAvailable);
			DevicesManager.instance.Unicorn.OnDataLost.RemoveListener(ForwardOnDataLost);
			DevicesManager.instance.Unicorn.OnEEGDataAvailable.RemoveListener(ForwardOnEEGDataAvailable);

			DevicesManager.instance.Unicorn.OnDeviceStateChanged.RemoveListener(CatchDeviceChange);
			DevicesManager.instance.Unicorn.OnRuntimeExceptionOccured.RemoveListener(CatchDeviceException);
		}
	}


	public void CatchDeviceChange(DataAcquisitionUnit.States State)
	{
		Debug.Log("Device changed catched at proxy level:" +State);
	}

	public void CatchDeviceException(Exception e)
	{
		Debug.Log("CatchDeviceException at proxy level:");
		Debug.LogException(e);
	}


}
