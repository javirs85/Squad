using Gtec.Chain.Common.Nodes.Devices;
using Gtec.Chain.Common.SignalProcessingPipelines;
using Gtec.Chain.Common.Templates.DataAcquisitionUnit;
using Gtec.Chain.Common.Templates.Utilities;
using Gtec.Chain.Common.Nodes.Devices;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using static Gtec.Chain.Common.Nodes.InputNodes.ChannelQuality;
using static Gtec.Chain.Common.Templates.DataAcquisitionUnit.DataAcquisitionUnit;

namespace Gtec.Bandpower
{
    public class Device : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        [Tooltip("The buffer overlap in samples.")]
        public int BuffersizeInSamples = 250;
        [SerializeField]
        [Tooltip("The buffer size in samples.")]
        public int BufferOverlapInSamples = 225;
        [Tooltip("Show or hide debug messages.")]
        public bool DebugMessagesEnabled = true;

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
        public UnityEvent<Dictionary<string, double>> OnBandpowerMeanAvailable;
        [SerializeField]
        [Tooltip("The event called when new signal quality values are available.")]
        public UnityEvent<List<ChannelStates>> OnSignalQualityAvailable;
        [SerializeField]
        [Tooltip("The event called when battery level data is available.")]
        public UnityEvent<float> OnBatteryLevelAvailable;
        [SerializeField]
        [Tooltip("The event called when data is lost.")]
        public UnityEvent OnDataLost;

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
        /// Initializes device list.
        /// </summary>
        private void InitializeDevices()
        {
            try
            {
                _devices = new List<DataAcquisitionUnit>();
                _devices.Add(new UnicornBCICore4Simulator());
                _devices.Add(new UnicornBCICore8Simulator());
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
                GetAvailableDevices();
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
                    Debug.Log(string.Format("Could not get available devices for '{0}'. Exception: {1}", device.GetType().Name, ex.Message));
                }
            }

            if (devicesChanged)
                EventHandler.Instance.Enqueue(() => { OnDevicesAvailable.Invoke(_deviceSerials); });
        }


        void Start()
        {
            //Thread apartment state must be set properly for windows
#if PLATFORM_STANDALONE_WIN || UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            ThreadApartmentState.Initialize();
#endif
            InitializeDevices();
            StartDiscoveryThread();
        }

        /// <summary>
        /// Stops all threads if running.
        /// </summary>
        private void OnDestroy()
        {
            //TODO: CHECK IF DISCONNECT CAN OR MUST BE CALLED HERE
            if(_device is not null && (_device.State == States.Connected ||  _device.State == States.Acquiring))
                Disconnect();
            StopDiscoveryThread();
            UninitializeDevices();
        }

        /// <summary>
        /// Connects to a device.
        /// </summary>
        /// <param name="serial">The serial of the device.</param>
        /// <exception cref="Exception"></exception>
        public void Connect(string serial)
        {
            //get device object by serial
            WriteDebugMessage(string.Format("Connecting to '{0}'...", serial));
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
            _bpPipeline = new BandpowerPipeline();
            _bpPipeline.PipelineStateChanged += OnPipelineStateChangedInternal;
            _bpPipeline.DataAvailable += OnBandpowerDataAvailable;
            _bpPipeline.RuntimeExceptionOccured += OnRuntimeExceptionOccuredInternal;
            _bpPipeline.Initialize(_device, _bpConfig);

            //create and attach sq pipeline
            _sqPipelineConfig = new SQPipelineConfiguration();
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
            //stop acquisition
            _device.StopAcquisition();

            //destroy bandpower pipeline
            _bpPipeline.Uninitialize();
            _bpPipeline.PipelineStateChanged -= OnPipelineStateChangedInternal;
            _bpPipeline.DataAvailable -= OnBandpowerDataAvailable;
            _bpPipeline.RuntimeExceptionOccured -= OnRuntimeExceptionOccuredInternal;

            //destroy sq pipeline
            _sqPipeline.Uninitialize();
            _sqPipeline.PipelineStateChanged -= OnPipelineStateChangedInternal;
            _sqPipeline.DataAvailable -= OnBandpowerDataAvailable;
            _sqPipeline.RuntimeExceptionOccured -= OnRuntimeExceptionOccuredInternal;

            //destroy data lost pipeline
            _dlPipeline.Uninitialize();
            _dlPipeline.PipelineStateChanged -= OnPipelineStateChangedInternal;
            _dlPipeline.DataAvailable -= OnBandpowerDataAvailable;
            _dlPipeline.RuntimeExceptionOccured -= OnRuntimeExceptionOccuredInternal;

            //destroy battery pipeline
            _batPipeline.Uninitialize();
            _batPipeline.PipelineStateChanged -= OnPipelineStateChangedInternal;
            _batPipeline.DataAvailable -= OnBandpowerDataAvailable;
            _batPipeline.RuntimeExceptionOccured -= OnRuntimeExceptionOccuredInternal;

            //disconnect from device
            _device.Uninitialize();

            WriteDebugMessage(string.Format("Disconnected from '{0}'.", _device.Serial));

            //detach from device events
            _device.StateChanged -= OnDeviceStateChangedInternal;
            _device.RuntimeExceptionOccured -= OnRuntimeExceptionOccuredInternal;

            _device = null;
            _bpPipeline = null;
            GC.Collect();
        }

        /// <summary>
        /// On bandpower data available. Called when new bandpower data is available.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBandpowerDataAvailable(object sender, DataEventArgs e)
        {
            BandpowerData data = (BandpowerData)e.Data;
            EventHandler.Instance.Enqueue(() => { OnBandpowerAvailable.Invoke(data.Bandpower); });
            EventHandler.Instance.Enqueue(() => { OnBandpowerMeanAvailable.Invoke(data.BandpowerMean); });
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
        /// Writes debug messages if enabled
        /// </summary>
        /// <param name="message"></param>
        private void WriteDebugMessage(string message)
        {
            if (DebugMessagesEnabled)
                Debug.Log(message);
        }

        /// <summary>
        /// Executes events on main thread
        /// </summary>
        public void Update()
        {
            EventHandler.Instance.DequeueAll();
        }
    }
}