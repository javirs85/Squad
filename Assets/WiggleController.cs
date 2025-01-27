using JetBrains.Annotations;
using UnityEngine;

public class WiggleController : MonoBehaviour
{
	[Header("Wiggle Settings")]
	public float wiggleIntensity = 0.1f; // How much the object wiggles
	public float wiggleSpeed = 1.0f;     // Speed of the wiggle motion
	public float wiggleRotationAmplitude = 0.0f; //Rotation amplitude
	public float wiggleRotationSpeed = 0.0f;  //Rotation speed

	private Vector3 originalPosition;   // Base position before wiggle
	private float initialWiggleIntensity;
	private float initialWiggleSpeed;

	public bool MakeSuperJerky = false;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		// Save the object's initial position
		originalPosition = transform.localPosition;
	}

    // Update is called once per frame
    void Update()
    {
		ApplyWiggle();
	}

	private void ApplyWiggle()
	{
		var FinalWiggleSpeed = wiggleSpeed;
		var FinalRotationSpeed = wiggleRotationAmplitude;
		if (MakeSuperJerky)
		{
			float rnd = Random.Range(5.0f, 8.0f);

			FinalWiggleSpeed = wiggleSpeed * 8.0f;
			FinalRotationSpeed = Mathf.Max(1.0f, wiggleRotationSpeed);
			FinalRotationSpeed *= 8.0f;
		}

		// Generate Perlin noise offsets for smooth random motion
		float xOffset = Mathf.PerlinNoise(Time.time * FinalWiggleSpeed, 0f) * 2 - 1;
		float yOffset = Mathf.PerlinNoise(0f, Time.time * FinalWiggleSpeed) * 2 - 1; 
		float rOffset = Mathf.PerlinNoise(Time.time * wiggleRotationAmplitude * 0.1f, Time.time * wiggleRotationAmplitude * 0.1f) * 2 - 1;


		// Apply the offsets scaled by intensity
		Vector3 wiggleOffset = new Vector3(xOffset, yOffset, 0f) * wiggleIntensity;
		float rotationZ = rOffset * wiggleRotationSpeed * 5.0f;	

		// Update position with wiggle
		transform.localPosition = originalPosition + wiggleOffset;
		transform.localRotation = Quaternion.Euler(0f, 0f, rotationZ);
	}	
}
