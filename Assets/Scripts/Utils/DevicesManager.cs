using Gtec.Bandpower;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class DevicesManager : MonoBehaviour, IDisposable
{
	public UnityEvent<string> AmplifierAutoConnected = new();

	public static DevicesManager instance;
	public static string SceneToGoAfterConnection { get; internal set; } = string.Empty;

	public bool AutoConnectToFavoriteAmplifier = false;
	public bool AutoConnectToAnything = false;

	protected string FavoriteAmplifier = string.Empty;
    public Device Unicorn;
	public string ConnectedSN = string.Empty;
	public bool IsConnected => Unicorn != null && ConnectedSN != string.Empty;


	public List<string> AvailableAmplifiers = new();

	public void Awake()
	{
		if (instance is not null)
		{
			Debug.Log("Destroying singleton");
			Destroy(gameObject);
		}
		else
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}

	public virtual void Start()
    {
		Debug.Log("DevicesManager Start");
		FindDevice();
		if (Unicorn is not null)
		{
			if (Unicorn.Serial != string.Empty)
				Unicorn.Disconnect();

			Unicorn.OnDevicesAvailable.AddListener(UpdateAvailableDevices);
		}
		else
		{
			Debug.LogError("No device found in the hierarchy");
			return;
		}

		FavoriteAmplifier = RetrieveString("FavoriteAmplifier");
	}

	private void UpdateAvailableDevices(List<string> arg0)
	{
		foreach(var device in arg0)
		{
			if (!string.IsNullOrEmpty(device) && !AvailableAmplifiers.Contains(device))
			{
				AvailableAmplifiers.Add(device);
				Debug.Log("Device found: " + device);

				if(!IsConnected)
				{
					if(AutoConnectToFavoriteAmplifier && device == FavoriteAmplifier)
					{
						AmplifierAutoConnected.Invoke(device);
					}

					if (!IsConnected && AutoConnectToAnything)
					{
						AmplifierAutoConnected.Invoke(device);
					}
				}
			}
		}
	}

	public void ConnectTo(string serial)
	{
		if (serial != null && !string.IsNullOrEmpty(serial))
		{
			Debug.Log("Forcing connection to : " + serial);
			Unicorn.Connect(serial);
			ConnectedSN = serial;
			if(!IsSimulator(serial))
				StoreString("FavoriteAmplifier", serial);
		}
	}

	public void QuickConnect()
	{
		if(Unicorn is not null)
		{
			var favoriteAmplifier = RetrieveString("FavoriteAmplifier");
			if (favoriteAmplifier != null && AvailableAmplifiers.Contains(favoriteAmplifier))
			{
				Unicorn.Connect(favoriteAmplifier);
				ConnectedSN = favoriteAmplifier;
				Debug.Log("Connecting to favorite amplifier: " + favoriteAmplifier);
			}
			else
			{
				var validUnicorn = AvailableAmplifiers.FirstOrDefault(x=> !IsSimulator(x));
				if(validUnicorn != null && !string.IsNullOrEmpty(validUnicorn))
				{
					Unicorn.Connect(validUnicorn);
					ConnectedSN = validUnicorn;
					Debug.Log("Connecting to a non favorite amplifier: " + validUnicorn);
				}
				else
				{
					var validSim = AvailableAmplifiers.FirstOrDefault(x => IsSimulator(x));
					Unicorn.Connect(validUnicorn);
					ConnectedSN = validSim;
					Debug.Log("Connecting to simulator: " + validUnicorn);
				}
			}
		}
	}

	public virtual void FindDevice()
	{
		Unicorn = FindFirstObjectByType<Device>();
		if (Unicorn == null)
		{
			Debug.LogError("No device found in the hierarchy");
			return;
		}
		Debug.Log("Device ready at prefab");
	}

	//Stores the string value, on the string key using playerprefs
	protected void StoreString(string key, string value)
	{
		PlayerPrefs.SetString(key, value);
		PlayerPrefs.Save();
	}

	//retrieves the specified key from playerprefs	
	protected string RetrieveString(string key)
	{
		if (PlayerPrefs.HasKey(key))
		{
			return PlayerPrefs.GetString(key);
		}
		else
		{
			Debug.Log("Key not found in playerprefs: " + key);
			return null;
		}
	}

	public void Dispose()
	{
		if(Unicorn is not null)
		{
			Unicorn.OnDevicesAvailable.RemoveAllListeners();
		}
	}

	public void OnDestroy()
	{
		Dispose();
	}

	protected bool IsSimulator(string SN) => SN.Contains("0000");
}
