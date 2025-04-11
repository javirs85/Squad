using Gtec.Bandpower;
using Gtec.Chain.Common.Nodes.InputNodes;
using Gtec.Chain.Common.Templates.DataAcquisitionUnit;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Test_MyGame2 : MonoBehaviour
{

	public GameObject Cube;

	public GameObject Channel1;
	public GameObject Channel2;
	public GameObject Channel3;
	public GameObject Channel4;
	public GameObject Channel5;
	public GameObject Channel6;
	public GameObject Channel7;
	public GameObject Channel8;

	private List<GameObject> ChannelBoxes = new();
	private void Awake()
	{
		ChannelBoxes.Add(Channel1);
		ChannelBoxes.Add(Channel2);
		ChannelBoxes.Add(Channel3);
		ChannelBoxes.Add(Channel4);
		ChannelBoxes.Add(Channel5);
		ChannelBoxes.Add(Channel6);
		ChannelBoxes.Add(Channel7);
		ChannelBoxes.Add(Channel8);
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (Keyboard.current.qKey.wasReleasedThisFrame)
			SceneManager.LoadScene("TEST_MainScene");
		if (Keyboard.current.wKey.wasReleasedThisFrame)
			SceneManager.LoadScene("TEST_MainScene 2");
		if (Keyboard.current.eKey.wasReleasedThisFrame)
			SceneManager.LoadScene("TEST_UnicornConfiguration");
	}

	public void ApplyBandPower(Dictionary<string, double> data)
	{
		float alpha = (float)data["alpha"];
		Cube.transform.localScale = new Vector3(alpha / 10f, alpha / 10f, alpha / 10f);
	}

	public void ApplySignalQualityChages(List<ChannelQuality.ChannelStates> data)
	{
		for (int i = 0; i < data.Count; i++)
		{
			if (i < ChannelBoxes.Count)
			{
				if (data[i] == ChannelQuality.ChannelStates.Good)
				{
					ChannelBoxes[i].GetComponent<Renderer>().material.color = Color.green;
				}
				else if (data[i] == ChannelQuality.ChannelStates.BadFloating)
				{
					ChannelBoxes[i].GetComponent<Renderer>().material.color = Color.yellow;
				}
				else if (data[i] == ChannelQuality.ChannelStates.BadGrounded)
				{
					ChannelBoxes[i].GetComponent<Renderer>().material.color = Color.red;
				}
			}
			else
			{
				ChannelBoxes[i].GetComponent<Renderer>().material.color = Color.gray;
			}
		}
	}

}
