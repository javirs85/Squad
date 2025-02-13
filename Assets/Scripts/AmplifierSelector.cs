using Gtec.Bandpower;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class AmplifierSelector : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Device Device;
    public GameObject MainCamera;
    public GameObject SimulatorModel;
    public GameObject RealAmplifierModel;

    List<string> AvailableDevices = new();
    List<GameObject> PlaneOptions = new();

	void Start()
    {
        Device.OnDevicesAvailable.AddListener(UpdatePlanes);
    }

	private void OnDestroy()
	{
		Device.OnDevicesAvailable.RemoveAllListeners();
	}

	// Update is called once per frame
	void Update()
    {
        if(AvailableDevices.Count > PlaneOptions.Count)
        { 
            var missingSN = AvailableDevices.Find(device=>PlaneOptions.Find(Plane=>Plane.name == device)  == null);
            CreateNewPlaneOption(missingSN);
        }
        else if(AvailableDevices.Count < PlaneOptions.Count)
        {
            var NonExistingPlane = PlaneOptions.Find(plane => AvailableDevices.Find(x=>plane.name == x) == null);    
            DestroyPlaneOption(NonExistingPlane);
        }
    }
    
    void UpdatePlanes(List<string> arg0)
    {
        AvailableDevices = arg0;
    }

    void CreateNewPlaneOption(string SN)
    {
        int i = PlaneOptions.Count;

		Vector3 newPos = MainCamera.transform.localPosition;
		//-64, -160 - 173 
		newPos.x += i * 40;
		newPos.y = -18.2f;
		newPos.z = 85.0f;


		GameObject F = null;
        if (SN.Contains("0000")) F = SimulatorModel;
        else F = RealAmplifierModel;


		GameObject NewOption = Instantiate(F, MainCamera.transform);
		var textMesh = NewOption.GetComponentInChildren<TextMesh>();
		textMesh.text = SN;
		NewOption.transform.localPosition = newPos;
		NewOption.name = SN;
        NewOption.layer = LayerMask.NameToLayer("UI");


		PlaneOptions.Add(NewOption);
	}
    void DestroyPlaneOption(GameObject plane)
    {
        Destroy(plane);
    }
}
