using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class FuelGaugeManager : MonoBehaviour
{
    public MeshRenderer FuelGaugeMesh;
    public Transform FuelArrow;
    public Material RedMaterial;
    public Material YellowMaterial;
    public Material BlackMaterial;
	public Material WhiteMaterial;

	[Header("Lerp Settings")]
	public float rotationLerpSpeed = 5f;
	public float colorLerpSpeed = 5f;

	public FuelColors CurrentFuelColor;
	public float CurrentBatteryLevel = 100f;

	private Quaternion targetRotation;
	private Material currentMaterialInstance0;
	private Material currentMaterialInstance1;
	private Color target0Color;
	private Color target1Color;

	private bool IsDemoMode = false;

	public void StartDemoMode() => IsDemoMode = true;
	public void FinishDemoMode() => IsDemoMode = false;


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		if (FuelGaugeMesh != null)
		{
			// Start with a copy of the current material
			currentMaterialInstance0 = new Material(FuelGaugeMesh.materials[0]);
			currentMaterialInstance1 = new Material(FuelGaugeMesh.materials[1]);
			var materials = FuelGaugeMesh.materials;
			materials[0] = currentMaterialInstance0;
			materials[1] = currentMaterialInstance1;
			FuelGaugeMesh.materials = materials;
			target0Color = currentMaterialInstance0.color;
			target1Color = currentMaterialInstance1.color;
		}

		if (FuelArrow != null)
			targetRotation = FuelArrow.localRotation;
	}

    // Update is called once per frame
    void Update()
    {
		if (FuelArrow != null)
			FuelArrow.localRotation = Quaternion.Lerp(FuelArrow.localRotation, targetRotation, Time.deltaTime * rotationLerpSpeed);

		if (currentMaterialInstance0 != null)
		{
			currentMaterialInstance0.color = Color.Lerp(currentMaterialInstance0.color, target0Color, Time.deltaTime * colorLerpSpeed);
			if (currentMaterialInstance0.HasProperty("_EmissionColor"))
			{
				Color currentEmission = currentMaterialInstance0.GetColor("_EmissionColor");
				Color targetEmission = target0Color; // or scaled, e.g. targetColor * intensity
				Color newEmission = Color.Lerp(currentEmission, targetEmission, Time.deltaTime * colorLerpSpeed);
				currentMaterialInstance0.SetColor("_EmissionColor", newEmission);
			}
		}
		if (currentMaterialInstance1 != null)
		{
			currentMaterialInstance1.color = Color.Lerp(currentMaterialInstance1.color, target1Color, Time.deltaTime * colorLerpSpeed);
			if (currentMaterialInstance1.HasProperty("_EmissionColor"))
			{
				Color currentEmission = currentMaterialInstance1.GetColor("_EmissionColor");
				Color targetEmission = target1Color; // or scaled, e.g. targetColor * intensity
				Color newEmission = Color.Lerp(currentEmission, targetEmission, Time.deltaTime * colorLerpSpeed);
				currentMaterialInstance1.SetColor("_EmissionColor", newEmission);
			}
		}

		SetFuelLevel(CurrentBatteryLevel);
	}

	public void RotateMarker(float angle)
	{
		if (FuelArrow != null)
			FuelArrow.localRotation = Quaternion.Euler(0, 0, angle);
	}

	public void SetFuelLevel(float val)
	{
		if (IsDemoMode) return;

		CurrentBatteryLevel = val;

		if (val > 60) SetBaseColor(FuelColors.green);
		else if (val > 13) SetBaseColor(FuelColors.yellow);
		else SetBaseColor(FuelColors.red);

		float angle = Mathf.Lerp(-70f, 70f, val / 100f);
		targetRotation = Quaternion.Euler(0, angle, 0);
	}

	public void SetFuelLevel(FuelColors fuelLevel)
	{
		if (FuelArrow != null)
		{
			if (fuelLevel == FuelColors.red)
				targetRotation = Quaternion.Euler(0, -63.9f, 0);
			else if (fuelLevel == FuelColors.yellow)
				targetRotation = Quaternion.Euler(0, -40, 0);
			else if (fuelLevel == FuelColors.green)
				targetRotation = Quaternion.Euler(0, 70, 0);

			SetBaseColor(fuelLevel);
		}
		else
		{
			Debug.LogError("Fuel Arrow is not assigned.");
		}
	}


	public enum FuelColors { red, yellow, green};

	public void SetBaseColor(FuelColors color)
	{
		if (FuelGaugeMesh == null )
			return;

		CurrentFuelColor = color;


		var materials = FuelGaugeMesh.materials;
		if (color == FuelColors.green)
		{
			materials[1] = BlackMaterial;
			materials[0] = WhiteMaterial;
		}
		else if (color == FuelColors.red)
		{
			materials[1] = RedMaterial;
			materials[0] = BlackMaterial;
		}
		else if (color == FuelColors.yellow)
		{
			materials[1] = YellowMaterial;
			materials[0] = BlackMaterial;
		}

		FuelGaugeMesh.materials = materials;
	}
}
