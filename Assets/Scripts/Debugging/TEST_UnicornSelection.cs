using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TEST_UnicornSelection : UnicornSelectionBase
{
	public GameObject SimulatorModel;
	public GameObject RealAmplifierModel;

	public override void CreateNewAmpOption(string SN)
	{
		GameObject ReferenceModel = null;
		if (SN.Contains("0000")) ReferenceModel = SimulatorModel;
		else ReferenceModel = RealAmplifierModel;

		GameObject NewOption = Instantiate(ReferenceModel, this.gameObject.transform);
		NewOption.GetComponentInChildren<TMPro.TextMeshPro>().text=SN;
		NewOption.SetActive(true);
		float X = AmpOptions.Count * 3f;
		NewOption.transform.position = new Vector3(X, -1.5f, 5);
		NewOption.name = SN;
		AmpOptions.Add(NewOption);

		for(int i=0; i< AmpOptions.Count; ++i)
		{
			var x = i*3f - (AmpOptions.Count - 1) * 3f / 2f;
			AmpOptions[i].transform.position = new Vector3(x, AmpOptions[i].transform.position.y, AmpOptions[i].transform.position.z);
		}
	}

	public override void DestroyAmpOption(GameObject option)
	{
		Destroy(option);
	}

	public override void AmpSelected(string ClickedSN)
	{
		Debug.Log("Selected amplifier: " + ClickedSN);
		DevicesManager.ConnectTo(ClickedSN);
		if(DevicesManager.SceneToGoAfterConnection != string.Empty)
			SceneManager.LoadScene(DevicesManager.SceneToGoAfterConnection);
		else
			SceneManager.LoadScene("TEST_MainScene");
	}

}
