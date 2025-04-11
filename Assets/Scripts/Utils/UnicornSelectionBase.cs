using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public abstract class UnicornSelectionBase : MonoBehaviour
{

	public DevicesManager DevicesManager;


	protected List<GameObject> AmpOptions = new();

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (DevicesManager.AvailableAmplifiers.Count > AmpOptions.Count)
		{
			var missingSN = DevicesManager.AvailableAmplifiers.Find(device => AmpOptions.Find(amp => amp.name == device) == null);
			CreateNewAmpOption(missingSN);
		}
		else if (DevicesManager.AvailableAmplifiers.Count < AmpOptions.Count)
		{
			var NonExistingOption = AmpOptions.Find(amp => DevicesManager.AvailableAmplifiers.Find(x => amp.name == x) == null);
			DestroyAmpOption(NonExistingOption);
		}

		var ClickedSN = HasClickedOnAnOption();
		if (ClickedSN != string.Empty)
		{
			AmpSelected(ClickedSN);
		}


	}

	abstract public void CreateNewAmpOption(string SN);

	abstract public void DestroyAmpOption(GameObject option);

	abstract public void AmpSelected(string SN);

	/// <summary>
	/// This method checks if the user has clicked on an amplifier option. This base method only checks for mouse clicks.
	/// In order to use VR handling or other input methods, this method should be overridden.
	/// </summary>
	/// <returns></returns>
	public virtual string HasClickedOnAnOption()
	{
		// detect click on the amplifier
		if (Mouse.current is not null && Mouse.current.leftButton.wasPressedThisFrame)
		{
			Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
			{
				if (hit.collider != null)
				{
					var SelectedAmp = AmpOptions.Find(amp => amp.name == hit.collider.name);
					if (SelectedAmp != null)
					{
						return SelectedAmp.name;
					}
				}
			}
		}
		return string.Empty;
	}

}
