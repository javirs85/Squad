using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
	[Header("Friends")]
	public GameObject Friend1;  
	public GameObject Friend2;  
	public GameObject Friend3;  
	public GameObject Friend4;

	[Header("Enemy")]
	public GameObject Enemy;

	[Header("AlphaMarker")]
	public GameObject AlphaObject;
	AlphaMarkerController Alpha;

	float CurrentAlphaPosition = 0.0f;
	float CurrentAlphaReference = 0.0f;


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        Alpha = AlphaObject.GetComponent<AlphaMarkerController>();
    }

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Q))
			ToggleObjectInScreen(Friend1);
		if (Input.GetKeyUp(KeyCode.W))
			ToggleObjectInScreen(Friend2);
		if (Input.GetKeyUp(KeyCode.E))
			ToggleObjectInScreen(Friend3);
		if (Input.GetKeyUp(KeyCode.R))
			ToggleObjectInScreen(Friend4);
		if (Input.GetKeyUp(KeyCode.T))
			ToggleObjectInScreen(Enemy);
		if (Input.GetKeyUp(KeyCode.A))
			ToggleFriendJerk(Friend1);
		if (Input.GetKeyUp(KeyCode.S))
			ToggleFriendJerk(Friend2);
		if (Input.GetKeyUp(KeyCode.D))
			ToggleFriendJerk(Friend3);
		if (Input.GetKeyUp(KeyCode.F))
			ToggleFriendJerk(Friend4);
		if(Input.GetKeyUp(KeyCode.Alpha1))
			SetAlphaCurrentPosition(0.1f);
		if (Input.GetKeyUp(KeyCode.Alpha2))
			SetAlphaCurrentPosition(0.2f);
		if (Input.GetKeyUp(KeyCode.Alpha3))
			SetAlphaCurrentPosition(0.3f);
		if (Input.GetKeyUp(KeyCode.Alpha4))
			SetAlphaCurrentPosition(0.4f);
		if (Input.GetKeyUp(KeyCode.Alpha5))
			SetAlphaCurrentPosition(0.5f);
		if (Input.GetKeyUp(KeyCode.Alpha6))
			SetAlphaCurrentPosition(0.6f);
		if (Input.GetKeyUp(KeyCode.Alpha7))
			SetAlphaCurrentPosition(0.7f);
		if (Input.GetKeyUp(KeyCode.Alpha8))
			SetAlphaCurrentPosition(0.8f);
		if (Input.GetKeyUp(KeyCode.Alpha9))
			SetAlphaCurrentPosition(0.9f);
		if (Input.GetKeyUp(KeyCode.Alpha0))
			SetAlphaCurrentPosition(1.0f);
		if (Input.GetKeyUp(KeyCode.Z))
			SetAlphaReference(0.0f);
		if (Input.GetKeyUp(KeyCode.X))
			SetAlphaReference(1.0f);

	}

	void ToggleObjectInScreen(GameObject obj)
	{
		var mo = obj.GetComponentInChildren<MovableObject>();
		if(mo is not null)
		{
			if(mo.IsOnScreen())
				mo.GoOut();
			else
				mo.GoIn();
		}
	}	

	void SetAlphaCurrentPosition(float alpha)
	{
		Alpha.SetAlphaPosition(alpha);
		CurrentAlphaPosition = alpha;
	}

	void SetAlphaReference(float alpha)
	{
		CurrentAlphaReference = alpha;
		Alpha.SetReferenceValue(alpha);
		Alpha.SetAlphaPosition(CurrentAlphaPosition);
	}

	enum ObjectPositions { OnScreen, OutsideScreen};
	void MoveObject(ObjectPositions pos,  GameObject obj)
	{
		var mo = obj.GetComponentInChildren<MovableObject>();
		if (mo is not null)
		{
			if (pos == ObjectPositions.OutsideScreen)
				mo.GoOut();
			else
				mo.GoIn();
		}
	}	

	public void FriendJerkStop(GameObject obj)
	{
		var Wiggler = obj.GetComponentInChildren<WiggleController>();
		if (Wiggler is not null)
		{
			Wiggler.MakeSuperJerky = false;
		}
	}
	public void FriendJerkStart(GameObject obj)
	{
		var Wiggler = obj.GetComponentInChildren<WiggleController>();
		if (Wiggler is not null)
		{
			Wiggler.MakeSuperJerky = true;
		}
	}
	public void ToggleFriendJerk(GameObject obj)
	{
		var Wiggler = obj.GetComponentInChildren<WiggleController>();
		if (Wiggler is not null)
		{
			Wiggler.MakeSuperJerky = !Wiggler.MakeSuperJerky;
		}
	}

	public void MakeFriendJerk(int i)
	{
		if (i == 0) FriendJerkStart(Friend1);
		else if (i == 1) FriendJerkStart(Friend2);
		else if (i == 2) FriendJerkStart(Friend3);
		else if (i == 3) FriendJerkStart(Friend4);
	}

	public void ShowFriend(int i)
	{
		if (i == 0) MoveObject( ObjectPositions.OnScreen,Friend1);
		else if (i == 1) MoveObject(ObjectPositions.OnScreen, Friend2);
		else if (i == 2) MoveObject(ObjectPositions.OnScreen, Friend3);
		else if (i == 3) MoveObject(ObjectPositions.OnScreen, Friend4);
	}
	public void HideFriend(int i)
	{
		if (i == 0) MoveObject(ObjectPositions.OutsideScreen, Friend1);
		else if (i == 1) MoveObject(ObjectPositions.OutsideScreen, Friend2);
		else if (i == 2) MoveObject(ObjectPositions.OutsideScreen, Friend3);
		else if (i == 3) MoveObject(ObjectPositions.OutsideScreen, Friend4);
	}
	
	public void MakeFriendHappy(int i)
	{
		GameObject f = null;
		if (i == 0) f = Friend1;
		else if (i == 1) f = Friend2;
		else if (i == 2) f = Friend3;
		else if (i == 3) f = Friend4;

		MoveObject(ObjectPositions.OnScreen, f);
		FriendJerkStop(f);

	}

	public void ShowAllFriends() => StartCoroutine(ShowAllFriendsAnimation());
	public void HideAllFriends() => StartCoroutine(HideAllFriendsAnimation());
	IEnumerator ShowAllFriendsAnimation()
	{
		MoveObject(ObjectPositions.OnScreen, Friend1);
		yield return new WaitForSeconds(1);
		MoveObject(ObjectPositions.OnScreen, Friend2);
		yield return new WaitForSeconds(1);
		MoveObject(ObjectPositions.OnScreen, Friend3);
		yield return new WaitForSeconds(1);
		MoveObject(ObjectPositions.OnScreen, Friend4);
	}
	IEnumerator HideAllFriendsAnimation()
	{
		MoveObject(ObjectPositions.OutsideScreen, Friend1);
		yield return new WaitForSeconds(1);
		MoveObject(ObjectPositions.OutsideScreen, Friend2);
		yield return new WaitForSeconds(1);
		MoveObject(ObjectPositions.OutsideScreen, Friend3);
		yield return new WaitForSeconds(1);
		MoveObject(ObjectPositions.OutsideScreen, Friend4);
	}
}
