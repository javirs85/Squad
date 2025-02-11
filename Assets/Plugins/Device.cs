using Gtec.Chain.Common.Nodes.Devices;
using Gtec.Chain.Common.SignalProcessingPipelines;
using Gtec.Chain.Common.Templates.DataAcquisitionUnit;
using Gtec.Chain.Common.Templates.Utilities;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;
using static Gtec.Chain.Common.Nodes.InputNodes.ChannelQuality;
using static Gtec.Chain.Common.Templates.DataAcquisitionUnit.DataAcquisitionUnit;

namespace Gtec.Bandpower
{
    public class Device : MonoBehaviour
    {
        #region Enumerations...

        public enum DeviceType { AllDevices, UnicornBCICore, Simulator  }

        public enum SimulatorSignal { Grounded, Floating, GoodEEG}

        #endregion

        #region Properties...

        public string Serial
        {
            get
            {
                if (_device != null)
                    return _device.Serial;
                else
                    return String.Empty;
            }
        }

        #endregion

        #region Public members...

        [Header("Settings")]
        [SerializeField]
        [Tooltip("Filter for certain device types.")]
        public DeviceType Type = Device.DeviceType.AllDevices;
        [SerializeField]
        [Tooltip("The signal the simulator is generating.")]
        public SimulatorSignal Signal = Device.SimulatorSignal.GoodEEG;
        [Tooltip("The alpha level in µV.")]
        [Range(0, 100)]
        public float SimulatorAlphaLevelUv = 20;
        [SerializeField]
        [Tooltip("Show/hide advanced settings..")]
        public bool AdvancedSettings = false;
        [SerializeField]
        [Tooltip("The buffer overlap in samples.")]
        public int BuffersizeInSamples = 250;
        [SerializeField]
        [Tooltip("The buffer size in samples.")]
        public int BufferOverlapInSamples = 225;
        [FrequencyBand]
        [SerializeField]
        [Tooltip("The cutoff frequencies for the delta frequency band.")]
        public Vector2 Delta = new Vector2(1.0f,4.0f);
        [FrequencyBand]
        [SerializeField]
        [Tooltip("The cutoff frequencies for the theta frequency band.")]
        public Vector2 Theta = new Vector2(4.0f, 8.0f);
        [FrequencyBand]
        [SerializeField]
        [Tooltip("The cutoff frequencies for the alpha frequency band.")]
        public Vector2 Alpha = new Vector2(8.0f, 12.0f);
        [FrequencyBand]
        [SerializeField]
        [Tooltip("The cutoff frequencies for the beta-low frequency band.")]
        public Vector2 BetaLow = new Vector2(12.0f, 16.0f);
        [FrequencyBand]
        [SerializeField]
        [Tooltip("The cutoff frequencies for the beta-mid frequency band.")]
        public Vector2 BetaMid = new Vector2(16.0f, 20.0f);
        [FrequencyBand]
        [SerializeField]
        [Tooltip("The cutoff frequencies for the beta-high frequency band.")]
        public Vector2 BetaHigh = new Vector2(20.0f, 30.0f);
        [FrequencyBand]
        [SerializeField]
        [Tooltip("The cutoff frequencies for the gamma frequency band.")]
        public Vector2 Gamma = new Vector2(30.0f, 50.0f);

        #endregion

        #region Events...

        [Header("Events")]
        [SerializeField]
        [Tooltip("The event called when devices are discovered.")]
        public UnityEvent<List<string>> OnDevicesAvailable;
        [SerializeField]
        [Tooltip("The event called when device state changed.")]
        public UnityEvent<States> OnDeviceStateChanged;
        [SerializeField]
        [Tooltip("The event called when a pipeline state changed.")]
        public UnityEvent<string> OnPipelineStateChanged;
        [SerializeField]
        [Tooltip("The event called when a runtime exception occured.")]
        public UnityEvent<Exception> OnRuntimeExceptionOccured;
        [SerializeField]
        [Tooltip("The event called when bandpower values for each channel are available.")]
        public UnityEvent<Dictionary<string, double[]>> OnBandpowerAvailable;
        [SerializeField]
        [Tooltip("The event called averaged bandpower values over all channels are available.")]
        public UnityEvent<Dictionary<string, double>> OnMeanBandpowerAvailable;
        [SerializeField]
        [Tooltip("The event called when bandpower ratios for each channel are available.")]
        public UnityEvent<Dictionary<string, double[]>> OnRatiosAvailable;
        [SerializeField]
        [Tooltip("The event called averaged bandpower ratios over all channels are available.")]
        public UnityEvent<Dictionary<string, double>> OnMeanRatiosAvailable;
        [SerializeField]
        [Tooltip("The event called when new signal quality values are available.")]
        public UnityEvent<List<ChannelStates>> OnSignalQualityAvailable;
        [SerializeField]
        [Tooltip("The event called when battery level data is available.")]
        public UnityEvent<float> OnBatteryLevelAvailable;
        [SerializeField]
        [Tooltip("The event called when data is lost.")]
        public UnityEvent OnDataLost;
        [Tooltip("Show or hide debug messages.")]

        #endregion

        private DataAcquisitionUnit _device;
        private List<DataAcquisitionUnit> _devices;
        private List<string> _deviceSerials;
        private BandpowerPipeline _bpPipeline;
        private BandpowerPipelineConfiguration _bpConfig;
        private SQPipeline _sqPipeline;
        private SQPipelineConfiguration _sqPipelineConfig;
        private DataLostPipeline _dlPipeline;
        private DataLostPipelineConfiguration _dlPipelineConfig;
        private BatteryLevelPipeline _batPipeline;
        private BatteryLevelPipelineConfiguration _batPipelineConfig;
        private bool _discoveryThreadRunning;
        private Thread _discoveryThread;

		/// <summary>
        /// Requests required android permissions.
        /// </summary>
        private void RequestBluetoothPermissions()
        {
            // Check if permissions are already granted
            if (!Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_SCAN") ||
                !Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_CONNECT"))

            {
                // Request the necessary Bluetooth permissions
                string[] permissions = {
                "android.permission.BLUETOOTH_SCAN",
                "android.permission.BLUETOOTH_CONNECT",
            };
                Permission.RequestUserPermissions(permissions);
                Debug.Log("Bluetooth permissions granted.");
            }
            else
            {
                Debug.Log("Bluetooth permissions already granted.");
            }
        }

        /// <summary>
        /// Initializes device list.
        /// </summary>
        private void InitializeDevices()
        {
            try
            {
                _devices = new List<DataAcquisitionUnit>();
                if(Type == DeviceType.AllDevices || Type == DeviceType.Simulator)
                {
                    _devices.Add(new UnicornBCICore4Simulator());
                    _devices.Add(new UnicornBCICore8Simulator());
                }
                if (Type == DeviceType.AllDevices || Type == DeviceType.UnicornBCICore)
                    _devices.Add(new GtecBLEDevice());
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Clears device list
        /// </summary>
        private void UninitializeDevices()
        {
            try
            {
                _devices.Clear();
                _devices = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Starts the device discovery thread.
        /// </summary>
        private void StartDiscoveryThread()
        {
            if (!_discoveryThreadRunning)
            {
                foreach (DataAcquisitionUnit device in _devices)
                {
                    if (device.GetType().Equals(typeof(Gtec.Chain.Common.Nodes.Devices.GtecBLEDevice)))
                    {
                        Gtec.Chain.Common.Nodes.Devices.GtecBLEDevice d = (Gtec.Chain.Common.Nodes.Devices.GtecBLEDevice)device;
                        d.StartDeviceDiscovery();
                        Debug.Log("Start scanning...");
                    }
                }

                _discoveryThreadRunning = true;
                _discoveryThread = new Thread(DiscoveryThread_DoWork);
                _discoveryThread.Start();
            }
        }

        /// <summary>
        /// Stops the device discovery thread.
        /// </summary>
        private void StopDiscoveryThread()
        {
            if (_discoveryThreadRunning)
            {
                foreach (DataAcquisitionUnit device in _devices)
                {
                    if (device.GetType().Equals(typeof(Gtec.Chain.Common.Nodes.Devices.GtecBLEDevice)))
                    {
                        Gtec.Chain.Common.Nodes.Devices.GtecBLEDevice d = (Gtec.Chain.Common.Nodes.Devices.GtecBLEDevice)device;
                        d.StopDeviceDiscovery();
                        Debug.Log("Stop scanning...");
                    }
                }

                _discoveryThreadRunning = false;
                _discoveryThread.Join(500);
            }
        }

        /// <summary>
        /// Device discovery thread. Gets available devices.
        /// </summary>
        /// <param name="obj"></param>
        private void DiscoveryThread_DoWork(object obj)
        {
            while (_discoveryThreadRunning)
            {
                EventHandler.Instance.Enqueue(() => { GetAvailableDevices();});
                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// Scans for available devices. Calls an event if new devices are detected.
        /// </summary>
        private void GetAvailableDevices()
        {
            bool devicesChanged = false;
            if (_deviceSerials == null)
                _deviceSerials = new List<string>();

            foreach (DataAcquisitionUnit device in _devices)
            {
                try
                {
                    List<string> devices = device.GetAvailableDevices();
                    if (device.GetType().Equals(typeof(Gtec.Chain.Common.Nodes.Devices.UnicornBCICore4Simulator)) || device.GetType().Equals(typeof(Gtec.Chain.Common.Nodes.Devices.UnicornBCICore8Simulator)))
                    {
                        if (!_deviceSerials.Contains(devices[0]))
                        {
                            _deviceSerials.Add(devices[0]);
                            devicesChanged = true;
                        }
                    }
                    else
                    {
                        foreach (string dev in devices)
                        {
                            if (!_deviceSerials.Contains(dev))
                            {
                                _deviceSerials.Add(dev);
                                devicesChanged = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log(string.Format("Could not get available devices for '{0}'.\nException: {1}\nStackTrace: {2}", device.GetType().Name, ex.Message, ex.StackTrace));
                }
            }

            if (devicesChanged)
                EventHandler.Instance.Enqueue(() => { OnDevicesAvailable.Invoke(_deviceSerials); });
        }


        void Start()
        {
            _device = null;
            RuntimePlatform platform = Application.platform;

#if PLATFORM_STANDALONE_WIN || UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            //Thread apartment state must be set properly for windows
            if (platform == RuntimePlatform.WindowsPlayer || platform == RuntimePlatform.WindowsEditor)
                ThreadApartmentState.Initialize();
#endif
            //Permissions must be acquired for Android
            if (platform == RuntimePlatform.Android)
                RequestBluetoothPermissions();

            InitializeDevices();
            StartDiscoveryThread();
        }

        /// <summary>
        /// Stops all threads if running.
        /// </summary>
        private void OnDestroy()
        {
			Disconnect();
            StopDiscoveryThread();
            UninitializeDevices();
			GC.Collect();
        }
		
		/// <summary>
        /// Stops all threads if running.
        /// </summary>
        private void OnApplicationQuit()
        { 
			Disconnect();
            StopDiscoveryThread();
            UninitializeDevices();
			GC.Collect();
        }

        /// <summary>
        /// Connects to a device.
        /// </summary>
        /// <param name="serial">The serial of the device.</param>
        /// <exception cref="Exception"></exception>
        public void Connect(string serial)
        {
            //clean up if device is already connected
            if (_device != null)
                Disconnect();

            //get device object by serial
            Debug.Log(string.Format("Connecting to '{0}'...", serial));
            if (_deviceSerials == null || _devices == null)
                InitializeDevices();

            if (!_deviceSerials.Contains(serial))
                throw new Exception(string.Format("Could not find device with the specified serial number '{0}'.", serial));

            _device = null;
            foreach (DataAcquisitionUnit daq in _devices)
            {
                try
                {
                    if (daq.GetAvailableDevices().Contains(serial))
                    {
                        _device = daq;
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log(string.Format("Could not get available devices for '{0}'. Exception: {1}", daq.GetType().Name, ex.Message));
                }
            }

            if (_device == null)
                throw new Exception(string.Format("Could not get device instance with serial '{0}'.", serial));

            StopDiscoveryThread();

            //attach to device events
            _device.StateChanged += OnDeviceStateChangedInternal;
            _device.RuntimeExceptionOccured += OnRuntimeExceptionOccuredInternal;

            //connect to device
            _device.Initialize(serial);

            if (_device.State != States.Connected)
                throw new Exception("Could not connect to device.");

            //create and attach bandpower pipeline
            _bpConfig = new BandpowerPipelineConfiguration();
            _bpConfig.BufferSizeInSamples = BuffersizeInSamples;
            _bpConfig.BufferOverlapInSamples = BufferOverlapInSamples;
            _bpConfig.FrequencyBands[BandpowerConstants.Delta][0] = Delta[0];
            _bpConfig.FrequencyBands[BandpowerConstants.Delta][1] = Delta[1];
            _bpConfig.FrequencyBands[BandpowerConstants.Theta][0] = Theta[0];
            _bpConfig.FrequencyBands[BandpowerConstants.Theta][1] = Theta[1];
            _bpConfig.FrequencyBands[BandpowerConstants.Alpha][0] = Alpha[0];
            _bpConfig.FrequencyBands[BandpowerConstants.Alpha][1] = Alpha[1];
            _bpConfig.FrequencyBands[BandpowerConstants.BetaLow][0] = BetaLow[0];
            _bpConfig.FrequencyBands[BandpowerConstants.BetaLow][1] = BetaLow[1];
            _bpConfig.FrequencyBands[BandpowerConstants.BetaMid][0] = BetaMid[0];
            _bpConfig.FrequencyBands[BandpowerConstants.BetaMid][1] = BetaMid[1];
            _bpConfig.FrequencyBands[BandpowerConstants.BetaHigh][0] = BetaHigh[0];
            _bpConfig.FrequencyBands[BandpowerConstants.BetaHigh][1] = BetaHigh[1];
            _bpConfig.FrequencyBands[BandpowerConstants.Delta][0] = Gamma[0];
            _bpConfig.FrequencyBands[BandpowerConstants.Delta][1] = Gamma[1];

            if (!_bpConfig.FrequencyBands.ContainsKey(BandpowerConstants.Delta) ||
                    !_bpConfig.FrequencyBands.ContainsKey(BandpowerConstants.Theta) ||
                    !_bpConfig.FrequencyBands.ContainsKey(BandpowerConstants.Alpha) ||
                    !_bpConfig.FrequencyBands.ContainsKey(BandpowerConstants.BetaLow) ||
                    !_bpConfig.FrequencyBands.ContainsKey(BandpowerConstants.BetaMid) ||
                    !_bpConfig.FrequencyBands.ContainsKey(BandpowerConstants.BetaHigh) ||
                    !_bpConfig.FrequencyBands.ContainsKey(BandpowerConstants.Gamma)
                    )
                throw new ArgumentException("Could not set frequency bands. Missing Key.");

            foreach (KeyValuePair<string, double[]> kvp in _bpConfig.FrequencyBands)
            {
                if (kvp.Value.Length != 2)
                    throw new ArgumentException("Two cutoff frequencies required.");

                if (kvp.Value[1] < kvp.Value[0])
                    throw new ArgumentException("Second cutoff frequency must be bigger then the first cutoff frequency.");
            }

            _bpPipeline = new BandpowerPipeline();
            _bpPipeline.PipelineStateChanged += OnPipelineStateChangedInternal;
            _bpPipeline.DataAvailable += OnBandpowerDataAvailable;
            _bpPipeline.RuntimeExceptionOccured += OnRuntimeExceptionOccuredInternal;
            _bpPipeline.Initialize(_device, _bpConfig);

            //create and attach sq pipeline
            _sqPipelineConfig = new SQPipelineConfiguration();
            _sqPipelineConfig.BpSQLowCutoff = 1;
            _sqPipelineConfig.BpSQHighCutoff = 50;
            _sqPipelineConfig.BpSQFilterOrder = 2;
            _sqPipelineConfig.SignalQualityGroundedThreshold = 3;
            _sqPipelineConfig.SignalQualityFloatingThreshold = 30;

            _sqPipeline = new SQPipeline();
            _sqPipeline.PipelineStateChanged += OnPipelineStateChangedInternal;
            _sqPipeline.DataAvailable += OnSQDataAvailable;
            _sqPipeline.RuntimeExceptionOccured += OnRuntimeExceptionOccuredInternal;
            _sqPipeline.Initialize(_device, _sqPipelineConfig);

            //create and attach data lost pipeline
            _dlPipelineConfig = new DataLostPipelineConfiguration();
            _dlPipeline = new DataLostPipeline();
            _dlPipeline.PipelineStateChanged += OnPipelineStateChangedInternal;
            _dlPipeline.DataAvailable += OnDataLostDataAvailable;
            _dlPipeline.RuntimeExceptionOccured += OnRuntimeExceptionOccuredInternal;
            _dlPipeline.Initialize(_device, _dlPipelineConfig);

            //create and attach battery pipeline
            _batPipelineConfig = new BatteryLevelPipelineConfiguration();
            _batPipeline = new BatteryLevelPipeline();
            _batPipeline.PipelineStateChanged += OnPipelineStateChangedInternal;
            _batPipeline.DataAvailable += OnBatteryDataAvailable;
            _batPipeline.RuntimeExceptionOccured += OnRuntimeExceptionOccuredInternal;
            _batPipeline.Initialize(_device, _batPipelineConfig);

            //start acquisition
            _device.StartAcquisition();
        }

        /// <summary>
        /// Disconnects from a device
        /// </summary>
        public void Disconnect()
        {
            if(_device != null)
            {
                //stop acquisition
                if (_device.State == States.Connected || _device.State == States.Acquiring)
                    _device.StopAcquisition();

                //destroy bandpower pipeline
                if (_bpPipeline != null)
                {
                    _bpPipeline.Uninitialize();
                    _bpPipeline.PipelineStateChanged -= OnPipelineStateChangedInternal;
                    _bpPipeline.DataAvailable -= OnBandpowerDataAvailable;
                    _bpPipeline.RuntimeExceptionOccured -= OnRuntimeExceptionOccuredInternal;
                }

                //destroy sq pipeline
                if (_sqPipeline != null)
                {
                    _sqPipeline.Uninitialize();
                    _sqPipeline.PipelineStateChanged -= OnPipelineStateChangedInternal;
                    _sqPipeline.DataAvailable -= OnSQDataAvailable;
                    _sqPipeline.RuntimeExceptionOccured -= OnRuntimeExceptionOccuredInternal;
                }


                //destroy data lost pipeline
                if (_dlPipeline != null)
                {
                    _dlPipeline.Uninitialize();
                    _dlPipeline.PipelineStateChanged -= OnPipelineStateChangedInternal;
                    _dlPipeline.DataAvailable -= OnDataLostDataAvailable;
                    _dlPipeline.RuntimeExceptionOccured -= OnRuntimeExceptionOccuredInternal;
                }

                //destroy battery pipeline
                if (_batPipeline != null)
                {
                    _batPipeline.Uninitialize();
                    _batPipeline.PipelineStateChanged -= OnPipelineStateChangedInternal;
                    _batPipeline.DataAvailable -= OnBatteryDataAvailable;
                    _batPipeline.RuntimeExceptionOccured -= OnRuntimeExceptionOccuredInternal;
                }

                //disconnect from device
                if (_device != null)
                {
                    _device.Uninitialize();

                    //detach from device events
                    _device.StateChanged -= OnDeviceStateChangedInternal;
                    _device.RuntimeExceptionOccured -= OnRuntimeExceptionOccuredInternal;
                }

                _device = null;
                _bpPipeline = null;
                _sqPipeline = null;
                _dlPipeline = null;
                _batPipeline = null;
                GC.Collect();

                StartDiscoveryThread();
            }
        }


        /// <summary>
        /// Simulates alpha activity and noisw for Unicorn BCI Core 4/8 devices
        /// </summary>
        /// <param name="alphaLevel">Alpha level in microvolts</param>
        /// <param name="noiseLevel">Noise level in microvolts</param>
        private void SetSimulatorSignals(float alphaLevel, float noiseLevel)
        {
            if (_device != null && (_device.GetType().Equals(typeof(UnicornBCICore4Simulator)) || _device.GetType().Equals(typeof(UnicornBCICore8Simulator))))
            {
                if(_device.GetType().Equals(typeof(UnicornBCICore4Simulator)))
                {
                    UnicornBCICore4Simulator deviceTemp = (UnicornBCICore4Simulator)_device;
                    deviceTemp.SetAlphaLevel(alphaLevel);
                }

                if (_device.GetType().Equals(typeof(UnicornBCICore8Simulator)))
                {
                    UnicornBCICore8Simulator deviceTemp = (UnicornBCICore8Simulator)_device;
                    deviceTemp.SetAlphaLevel(alphaLevel);
                }

                if (_device.GetType().Equals(typeof(UnicornBCICore4Simulator)))
                {
                    UnicornBCICore4Simulator deviceTemp = (UnicornBCICore4Simulator)_device;
                    deviceTemp.SetNoiseLevel(noiseLevel);
                }

                if (_device.GetType().Equals(typeof(UnicornBCICore8Simulator)))
                {
                    UnicornBCICore8Simulator deviceTemp = (UnicornBCICore8Simulator)_device;
                    deviceTemp.SetNoiseLevel(noiseLevel);
                }
            }
        }

        /// <summary>
        /// On bandpower data available. Called when new bandpower data is available.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBandpowerDataAvailable(object sender, DataEventArgs e)
        {
            if(e.Data.GetType().Equals(typeof(BandpowerData)))
            {
                BandpowerData data = (BandpowerData)e.Data;
                EventHandler.Instance.Enqueue(() => { OnBandpowerAvailable.Invoke(data.Bandpower); });
                EventHandler.Instance.Enqueue(() => { OnMeanBandpowerAvailable.Invoke(data.BandpowerMean); });
            }
            
            if (e.Data.GetType().Equals(typeof(RatioData)))
            {
                RatioData data = (RatioData)e.Data;
                EventHandler.Instance.Enqueue(() => { OnRatiosAvailable.Invoke(data.Ratios); });
                EventHandler.Instance.Enqueue(() => { OnMeanRatiosAvailable.Invoke(data.RatiosMean); });
            }

        }

        /// <summary>
        /// Device state changed event. Called when the device state changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDeviceStateChangedInternal(object sender, DataAcquisitionUnit.StateChangedEventArgs e)
        {
            EventHandler.Instance.Enqueue(new Action(() => { OnDeviceStateChanged.Invoke(e.State); }));
        }

        /// <summary>
        /// Runtime exception event. Called when some node in the signal processing chain throws an error.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRuntimeExceptionOccuredInternal(object sender, RuntimeExceptionEventArgs e)
        {
            EventHandler.Instance.Enqueue(new Action(() => { OnRuntimeExceptionOccured.Invoke(e.Exception); }));
        }

        private void OnPipelineStateChangedInternal(object sender, PipelineStateEventArgs e)
        {
            EventHandler.Instance.Enqueue(new Action(() => { OnPipelineStateChanged.Invoke(string.Format("Pipeline: {0}\nState: {1}.", sender.ToString(), e.State.ToString())); }));
        }

        private void OnSQDataAvailable(object sender, DataEventArgs e)
        {
            EventHandler.Instance.Enqueue(new Action(() => { OnSignalQualityAvailable.Invoke(((ChannelStatesUpdateEventArgs)e.Data).ChannelStates); }));
        }

        private void OnBatteryDataAvailable(object sender, DataEventArgs e)
        {
            EventHandler.Instance.Enqueue(new Action(() => { OnBatteryLevelAvailable.Invoke((float)e.Data); }));
        }

        private void OnDataLostDataAvailable(object sender, DataEventArgs e)
        {
            EventHandler.Instance.Enqueue(new Action(() => { OnDataLost.Invoke(); }));
        }

        /// <summary>
        /// Executes events on main thread
        /// </summary>
        public void Update()
        {
            EventHandler.Instance.DequeueAll();
            
            if(Signal == SimulatorSignal.GoodEEG)
                SetSimulatorSignals(SimulatorAlphaLevelUv, 1);
            else if (Signal == SimulatorSignal.Grounded)
                SetSimulatorSignals(0, 0);
            else if (Signal == SimulatorSignal.Floating)
                SetSimulatorSignals(50, 50);
        }
    }
}