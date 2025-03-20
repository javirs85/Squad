using Gtec.Bandpower;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class AmplifierSelector : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Device Device;
    private GameObject PlaneCrationPoint;
    public GameObject SimulatorModel;
    public GameObject RealAmplifierModel;
	public bool ConnectAutomaticallyToRealAmplifiers = false;

	List<string> AvailableDevices = new();
    List<Tuple<GameObject,Vector3>> PlaneOptions = new();

	void Start()
    {
        Device.OnDevicesAvailable.AddListener(UpdatePlanes);
		PlaneCrationPoint = this.gameObject;

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
            var missingSN = AvailableDevices.Find(device=>PlaneOptions.Find(Plane=>Plane.Item1.name == device)  == null);
            CreateNewPlaneOption(missingSN);

			if(ConnectAutomaticallyToRealAmplifiers == true)
			{
				if (missingSN.Contains("0000") == false)
					SelectAmplifier(missingSN);
			}
		}
        else if(AvailableDevices.Count < PlaneOptions.Count)
        {
            var NonExistingPlane = PlaneOptions.Find(plane => AvailableDevices.Find(x=>plane.Item1.name == x) == null);    
            DestroyPlaneOption(NonExistingPlane.Item1);
        }

		//on click get raycast and select the plane
		if (Mouse.current is not null && Mouse.current.leftButton.wasPressedThisFrame)
		{
			Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
			{
				if (hit.collider != null)
				{
					var SelectedPlane = PlaneOptions.Find(plane => plane.Item1.name == hit.collider.name);
					if (SelectedPlane != null)
					{
						Debug.Log("Name: " + SelectedPlane.Item1.name);
						SelectAmplifier(SelectedPlane.Item1.name);
					}
				}
			}
		}

		// lerp all planes towars the position in the tuple
		foreach (var plane in PlaneOptions)
		{
			plane.Item1.transform.position = Vector3.Lerp(plane.Item1.transform.position, plane.Item2, Time.deltaTime * 3);
		}
	}


	public void SelectAmplifier(string SN)
	{
		Device.Connect(SN);
		SceneControl.instance.ChangeScene(
							SceneControl.Scenes.MainScene,
							() => { Device.Connect(SN); },
							true);
	}


	public

	void UpdatePlanes(List<string> arg0)
    {
        AvailableDevices = arg0;
    }

	void CreateNewPlaneOption(string SN)
    {
        int i = PlaneOptions.Count;

		Vector3 newPos = PlaneCrationPoint.transform.position;

		newPos.x += i * 40;
		newPos.y = -30;
		newPos.z = -10;


		GameObject F = null;
        if (SN.Contains("0000")) F = SimulatorModel;
        else F = RealAmplifierModel;


		GameObject NewOption = Instantiate(F, PlaneCrationPoint.transform);
		NewOption.SetActive(true);
		var textMesh = NewOption.GetComponentInChildren<TextMeshPro>();
		textMesh.text = SN;
		NewOption.transform.position = newPos;
		NewOption.name = SN;
        NewOption.layer = LayerMask.NameToLayer("UI");
		var wiggler = NewOption.GetComponentInChildren<WiggleController>();
		if(wiggler is not null)
		{
			wiggler.enabled = true;
			wiggler.wiggleIntensity = UnityEngine.Random.Range(0.22f, 0.30f);	
			wiggler.wiggleSpeed = UnityEngine.Random.Range(0.25f, 0.35f);	
		}

		PlaneOptions.Add(new Tuple<GameObject, Vector3>(NewOption, newPos));

		
		for (int j = 0; j < PlaneOptions.Count; j++)
		{
			var plane = PlaneOptions[j];
			plane = new Tuple<GameObject, Vector3>(plane.Item1, new Vector3(PlaneCrationPoint.transform.position.x + (j * 40) - ((PlaneOptions.Count-1)*40/2), -10, 70));
			PlaneOptions[j] = plane;
		}
	}
	void DestroyPlaneOption(GameObject plane)
    {
        Destroy(plane);
    }
}
