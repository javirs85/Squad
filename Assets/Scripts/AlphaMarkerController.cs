using TMPro;
using UnityEngine;

public class AlphaMarkerController : MonoBehaviour
{
	[Header("Objects")]
	public GameObject AlphaMarker;
	public GameObject AUP1;
	public GameObject AUP2;
	public GameObject AUP3;
	public GameObject ADown1;
	public GameObject ADown2;
	public GameObject ADown3;

	[Header("References")]
	public GameObject MaxValuePosition;
	public GameObject MinValuePosition;

	bool IsReferenceSet = false;
	private Vector3 Destination;

	[HideInInspector]
	public float AlphaValue = 0f;
	[HideInInspector]
	public float ReferenceValue = 0.5f;


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		HideAllArrows();
    }

	void HideAllArrows()
	{
		AUP1.SetActive(false);
		AUP2.SetActive(false);
		AUP3.SetActive(false);
		ADown1.SetActive(false);
		ADown2.SetActive(false);
		ADown3.SetActive(false);
	}

	/// <summary>
	/// Sets the aplha value. Expected valus are 0..1
	/// </summary>
	/// <param name="alpha">0..1 value for alpha level</param>
	public void SetAlphaPosition(float alpha)
	{
		AlphaValue = alpha;
		var MinToMaxVector = MinValuePosition.transform.localPosition - MaxValuePosition.transform.localPosition;
		var AppliedVector = MinToMaxVector * alpha;

		Destination = MinValuePosition.transform.localPosition - AppliedVector;

		if (IsReferenceSet)
		{
			var diff = ReferenceValue - alpha;
			if (diff > 0.7f) SetAlphaHints(true, true, true, false, false, false);
			else if(diff > 0.45f) SetAlphaHints(true, true, false, false, false, false);
			else if(diff > 0.11f) SetAlphaHints(true, false, false, false, false, false);
			else if(diff >= 0.0f) SetAlphaHints(false, false, false, false, false, false);
			else if(diff > -0.21f) SetAlphaHints(false, false, false, true, false, false);
			else if(diff > -0.7f) SetAlphaHints(false, false, false, true, true, false);
			else SetAlphaHints(false, false, false, true, true, true);
		}
	}

	void SetAlphaHints(bool u1, bool u2, bool u3, bool d1, bool d2, bool d3)
	{
		AUP1.SetActive(u1);
		AUP2.SetActive(u2);
		AUP3.SetActive(u3);
		ADown1.SetActive(d1);
		ADown2.SetActive(d2);
		ADown3.SetActive(d3);
	}

	public void SetReferenceValue(float val)
	{
		ReferenceValue = val;
		IsReferenceSet = true;
	}
	public void ClearReferenceValue() => IsReferenceSet = false;

    // Update is called once per frame
    void Update()
	{
		AlphaMarker.transform.localPosition = Vector3.Lerp(
			AlphaMarker.transform.localPosition,
			Destination,
			10.0f * Time.deltaTime);
	}
}
