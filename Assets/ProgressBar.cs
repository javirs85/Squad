using UnityEngine;
using UnityEngine.UIElements;

public class ProgressBar : MonoBehaviour
{
    public Transform Contanier;
    public Transform GrowingBar;

    public float ProgressValue = 0.0f;

	private Vector3 initialPosition;
	private float fullWidth; // The full width of the bar;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		fullWidth = GrowingBar.GetComponent<Renderer>().bounds.size.x;
		initialPosition = GrowingBar.position - new Vector3(fullWidth / 2, 0, 0);
	}

	// Update is called once per frame
	void Update()
    {
		ProgressValue = Mathf.Clamp(ProgressValue, 0f, 1f);

		// Scale the bar in X direction
		GrowingBar.localScale = new Vector3(ProgressValue, 1f, 1f);

		// Adjust position so the left edge stays fixed while growing
		float newWidth = fullWidth * ProgressValue;
		GrowingBar.position = initialPosition + new Vector3(newWidth / 2, 0, 0);
	}
}
