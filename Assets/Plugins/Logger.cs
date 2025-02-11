using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Gtec.Chain.Common.Nodes.InputNodes.ChannelQuality;
using static Gtec.Chain.Common.Templates.DataAcquisitionUnit.DataAcquisitionUnit;

namespace Gtec.Bandpower
{
    public class Logger : MonoBehaviour
    {
        private Device _bci;
        void Start()
        {
            try
            {
                _bci = GetComponentInParent<Device>();
            }
            catch
            {
                _bci = null;
            }
            if (_bci != null)
            {
                _bci.OnDevicesAvailable.AddListener(LogDevicesAvailable);
                _bci.OnDeviceStateChanged.AddListener(LogDeviceState);
                _bci.OnPipelineStateChanged.AddListener(LogPipelineState);
                _bci.OnRuntimeExceptionOccured.AddListener(LogRuntimeException);
                _bci.OnBandpowerAvailable.AddListener(LogBandpower);
                _bci.OnMeanBandpowerAvailable.AddListener(LogMeanBandpower);
                _bci.OnRatiosAvailable.AddListener(LogRatios);
                _bci.OnMeanRatiosAvailable.AddListener(LogMeanRatios);
                _bci.OnSignalQualityAvailable.AddListener(LogSignalQuality);
                _bci.OnBatteryLevelAvailable.AddListener(LogBatteryLevel);
                _bci.OnDataLost.AddListener(LogDataLost);
            }
        }

        public void LogBandpower(Dictionary<string, double[]> bandpower)
        {
            string s = string.Empty;
            foreach (KeyValuePair<string, double[]> kvp in bandpower)
            {
                s += kvp.Key + " : ";
                s += "[";
                s += string.Join(", ", kvp.Value.Select(n => n.ToString()));
                s += "]\n";
            }
            Debug.Log(s);
        }

        public void LogMeanBandpower(Dictionary<string, double> bandpowerMean)
        {
            string s = string.Empty;
            foreach (KeyValuePair<string, double> kvp in bandpowerMean)
            {
                s += kvp.Key + " : ";
                s += "[" + kvp.Value.ToString() + "]\n";
            }
            Debug.Log(s);
        }

        public void LogRatios(Dictionary<string, double[]> bandpower)
        {
            string s = string.Empty;
            foreach (KeyValuePair<string, double[]> kvp in bandpower)
            {
                s += kvp.Key + " : ";
                s += "[";
                s += string.Join(", ", kvp.Value.Select(n => n.ToString()));
                s += "]\n";
            }
            Debug.Log(s);
        }

        public void LogMeanRatios(Dictionary<string, double> bandpowerMean)
        {
            string s = string.Empty;
            foreach (KeyValuePair<string, double> kvp in bandpowerMean)
            {
                s += kvp.Key + " : ";
                s += "[" + kvp.Value.ToString() + "]\n";
            }
            Debug.Log(s);
        }

        public void LogRuntimeException(Exception e)
        {
            Debug.Log(string.Format("{0}:{1}\n{2}", e.Source.ToString(), e.Message, e.StackTrace));
        }

        public void LogDevicesAvailable(List<string> devices)
        {
            string s = string.Empty;
            foreach (string device in devices)
                s += device + "\n";
            Debug.Log(s);
        }

        public void LogDeviceState(States state)
        {
            Debug.Log(string.Format("Device State: {0}", state.ToString()));
        }

        public void LogDataLost()
        {
            Debug.Log("Data lost.");
        }

        public void LogBatteryLevel(float bat)
        {
            Debug.Log(string.Format("Battery Level: {0}", bat.ToString()));
        }

        public void LogSignalQuality(List<ChannelStates> channelStates)
        {
            string s = string.Empty;
            s += "Channel States: [";
            for (int i = 0; i < channelStates.Count(); i++)
                s += channelStates[i].ToString() + ",";
            s += "]";
            Debug.Log(s);
        }

        public void LogPipelineState(string pipelineState)
        {
            Debug.Log(pipelineState);
        }
    }
}
