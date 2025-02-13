using TMPro;
using UnityEngine;

public class MovableObject : MonoBehaviour
{
	[Header("Positions")]
	public Transform outPosition; // Assign this in the editor for the "out" position
	public bool StartOut = true; //true to start out, false to start in

	Quaternion OriginalRotation;
	Quaternion TargetRotation;
	Vector3 targetPosition; //Just for debugging purposes
	Vector3 OriginalPosition;


	[Header("Movement Settings")]
	public float moveSpeed = 5f;  // Speed at which the object moves

	private bool _isIn = true;

	public bool IsIn
	{
		get { return _isIn; }
		set { 
			_isIn = value;
		}
	}


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		OriginalPosition = transform.position;
		OriginalRotation = transform.rotation;

		if (StartOut)
		{
			targetPosition = outPosition.transform.position;
			TargetRotation = outPosition.transform.rotation;
			IsIn = false;
		}
		else
		{
			targetPosition = OriginalPosition;
			TargetRotation = OriginalRotation;
			IsIn = true;
		}

		transform.localPosition = targetPosition;
		transform.rotation = TargetRotation;
	}

    // Update is called once per frame
    void Update()
    {
		MoveObject();
	}
	private void MoveObject()
	{
		// Smoothly interpolate towards the target position
		transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, moveSpeed * Time.deltaTime);
		transform.localRotation = Quaternion.Lerp(transform.localRotation, TargetRotation, moveSpeed * Time.deltaTime);
	}

	public void SetTargetPosition(Vector3 newPosition)
	{
		targetPosition = OriginalPosition + newPosition;
	}

	public void SetTargetRotation(Quaternion newRotation)
	{
		TargetRotation = OriginalRotation * newRotation;
	}

	public void GoIn()
	{
		if (!IsIn)
		{
			SetTargetPosition(Vector3.zero);
			SetTargetRotation(Quaternion.identity);
			IsIn = true;
		}		
	}
	public void GoOut()
	{
		if (IsIn)
		{
			SetTargetPosition(outPosition.transform.position);
			SetTargetRotation(outPosition.transform.rotation);
			IsIn = false;
		}		
	}
	public bool IsOnScreen() => IsIn;
}
