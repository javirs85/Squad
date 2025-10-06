using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    [Header("Friends")]
	public GameObject Friend1;
	public GameObject Friend2;
	public GameObject Friend3;
	public GameObject Friend4;

	[Header("Enemy")]
	public GameObject Enemy;

	[Header("Misiles")]
	public Missile Missile1;
	public Missile Missile2;

	[Header("Cockpit")]
	public GameObject AlphaObject;
	public iAlphaController Alpha;
	public FuelGaugeManager FuelGaugeManager;
	public GlitchController GlitchController;

	public BCI BCI;

	private bool IsSQDemoMode = false;

	public void StartSQDemoMode() => IsSQDemoMode = true;
	public void FinishSQDemoMode() => IsSQDemoMode = false;


	//Planes used for selecting amplifier
	private List<GameObject> PlaneOptions = new();

	//private bool debugSequenceRunning = false;


    private void Awake()
    {
		if(instance != null)
		{
			Debug.Log("Destroying singleton");
			Destroy(gameObject);
		}
		else
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
	{
		Alpha = AlphaObject.GetComponent<iAlphaController>();
	}




	// Update is called once per frame
	void Update()
	{
		if (Keyboard.current.qKey.wasReleasedThisFrame)
			ToggleObjectInScreen(Friend1);
		if (Keyboard.current.wKey.wasReleasedThisFrame)
			ToggleObjectInScreen(Friend2);
		if (Keyboard.current.eKey.wasReleasedThisFrame)
			ToggleObjectInScreen(Friend3);
		if (Keyboard.current.rKey.wasReleasedThisFrame)
			ToggleObjectInScreen(Friend4);
		if (Keyboard.current.tKey.wasReleasedThisFrame)
			ToggleObjectInScreen(Enemy);
		if (Keyboard.current.aKey.wasReleasedThisFrame)
			ToggleFriendJerk(Friend1);
		if (Keyboard.current.sKey.wasReleasedThisFrame)
			ToggleFriendJerk(Friend2);
		if (Keyboard.current.dKey.wasReleasedThisFrame)
			ToggleFriendJerk(Friend3);
		if (Keyboard.current.fKey.wasReleasedThisFrame)
			ToggleFriendJerk(Friend4);


		if (Keyboard.current.digit1Key.wasReleasedThisFrame)
			SetAlphaCurrentPosition(0.1f);
		if (Keyboard.current.digit2Key.wasReleasedThisFrame)
			SetAlphaCurrentPosition(0.2f);
		if (Keyboard.current.digit3Key.wasReleasedThisFrame)
			SetAlphaCurrentPosition(0.3f);
		if (Keyboard.current.digit4Key.wasReleasedThisFrame)
			SetAlphaCurrentPosition(0.4f);
		if (Keyboard.current.digit5Key.wasReleasedThisFrame)
			SetAlphaCurrentPosition(0.5f);
		if (Keyboard.current.digit6Key.wasReleasedThisFrame)
			SetAlphaCurrentPosition(0.6f);
		if (Keyboard.current.digit7Key.wasReleasedThisFrame)
			SetAlphaCurrentPosition(0.7f);
		if (Keyboard.current.digit8Key.wasReleasedThisFrame)
			SetAlphaCurrentPosition(0.8f);
		if (Keyboard.current.digit9Key.wasReleasedThisFrame)
			SetAlphaCurrentPosition(0.9f);
		if (Keyboard.current.digit0Key.wasReleasedThisFrame)
			SetAlphaCurrentPosition(1.0f);
		if (Keyboard.current.zKey.wasReleasedThisFrame)
			SetAlphaReference(0.0f);
		if (Keyboard.current.xKey.wasReleasedThisFrame)
			SetAlphaReference(1.0f);

		if (Keyboard.current.vKey.wasReleasedThisFrame)
			FuelGaugeManager.SetFuelLevel(FuelGaugeManager.FuelColors.green);
		if (Keyboard.current.bKey.wasReleasedThisFrame)
			FuelGaugeManager.SetFuelLevel(FuelGaugeManager.FuelColors.yellow);
		if (Keyboard.current.nKey.wasReleasedThisFrame)
			FuelGaugeManager.SetFuelLevel(FuelGaugeManager.FuelColors.red);

		if (Keyboard.current.hKey.wasReleasedThisFrame)
			GlitchController.TriggerGlitch();

		if (Keyboard.current.mKey.wasReleasedThisFrame)
			FireMissilesDemo();

    }

	void ToggleObjectInScreen(GameObject obj)
	{
		var mo = obj.GetComponentInChildren<MovableObject>();
		if (mo is not null)
		{
			if (mo.IsOnScreen())
				mo.GoOut();
			else
				mo.GoIn();
		}
	}

	/// <summary>
	/// Set't the alpha marker position in % (0 bottom 1 top)
	/// </summary>
	/// <param name="alpha"></param>
	public void SetAlphaCurrentPosition(float alpha)
	{
		Alpha.SetAlphaPosition(alpha);
	}

	public void SetAlphaBetaThetaValues(float alpha, float beta, float theta)
	{
		BCI.ProcessNewMeanBandPowerSample(alpha, beta, theta);
	}

	void SetAlphaReference(float alpha)
	{
		Alpha.SetReferenceValue(alpha);
		Alpha.SetAlphaPosition(Alpha.AlphaValue);
	}

	public void SetBatteryLevel(float val)
	{
		if (FuelGaugeManager != null)
		{
			FuelGaugeManager.SetFuelLevel(val);
		}
	}

	enum ObjectPositions { OnScreen, OutsideScreen };
	void MoveObject(ObjectPositions pos, GameObject obj)
	{
		try
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
		catch (Exception e)
		{
			Debug.Log(e.Message);
		}
	}

	public void FireMissiles()
	{
		Missile1.LaunchMissile();
		Missile2.LaunchMissile();
	}
	public void FireMissilesDemo()
	{
		Missile1.LaunchMisileDemo();
		Missile2.LaunchMisileDemo();
	}

	public void FriendJerkStop(GameObject obj)
	{
		try
		{
			if(obj is null) return;

			var Wiggler = obj.GetComponentInChildren<WiggleController>();
			if (Wiggler is not null)
			{
				Wiggler.MakeSuperJerky = false;
			}
		}
		catch /*(Exception e)*/
		{
			//Debug.Log(e.Message);
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

	public void MakeFriendJerk(int i, bool forced = false)
	{
		if (IsSQDemoMode)
			if (!forced) return;

		if (i == 0) FriendJerkStart(Friend1);
		else if (i == 1) FriendJerkStart(Friend2);
		else if (i == 2) FriendJerkStart(Friend3);
		else if (i == 3) FriendJerkStart(Friend4);
	}


	public void ShowFriend(int i)
	{
		if (i == 0) MoveObject(ObjectPositions.OnScreen, Friend1);
		else if (i == 1) MoveObject(ObjectPositions.OnScreen, Friend2);
		else if (i == 2) MoveObject(ObjectPositions.OnScreen, Friend3);
		else if (i == 3) MoveObject(ObjectPositions.OnScreen, Friend4);
	}
	public void HideFriend(int i, bool forced = false)
	{
		if (IsSQDemoMode) 
			if(!forced) return; // In demo mode we don't hide friends unless forced

		if (i == 0) MoveObject(ObjectPositions.OutsideScreen, Friend1);
		else if (i == 1) MoveObject(ObjectPositions.OutsideScreen, Friend2);
		else if (i == 2) MoveObject(ObjectPositions.OutsideScreen, Friend3);
		else if (i == 3) MoveObject(ObjectPositions.OutsideScreen, Friend4);
	}

	public void MakeFriendHappy(int i, bool forced = false)
	{
		if (IsSQDemoMode)
			if (!forced) return;

		GameObject f = null;
		if (i == 0) f = Friend1;
		else if (i == 1) f = Friend2;
		else if (i == 2) f = Friend3;
		else if (i == 3) f = Friend4;
		else return;

		if(f is not null)
		{
			MoveObject(ObjectPositions.OnScreen, f);
			FriendJerkStop(f);
		}
	}


	public void ShowEnemy()
	{
		Enemy.SetActive(true);
		ToggleObjectInScreen(Enemy);
	}

	public void ShowAllFriends() => StartCoroutine(ShowAllFriendsAnimation());
	public void HideAllFriends() => StartCoroutine(HideAllFriendsAnimation());
	IEnumerator ShowAllFriendsAnimation()
	{
		MoveObject(ObjectPositions.OnScreen, Friend1);
		yield return new WaitForSeconds(0.4f);
		MoveObject(ObjectPositions.OnScreen, Friend2);
		yield return new WaitForSeconds(0.6f);
		MoveObject(ObjectPositions.OnScreen, Friend3);
		yield return new WaitForSeconds(0.4f);
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



    IEnumerator FullDebugSequence()
    {
        //debugSequenceRunning = true;
        ShowAllFriends();
        yield return new WaitForSeconds(5);
        ToggleObjectInScreen(Enemy);
        yield return new WaitForSeconds(1);
        ToggleFriendJerk(Friend1);
        yield return new WaitForSeconds(1);
        ToggleFriendJerk(Friend1);
        ToggleFriendJerk(Friend2);
        yield return new WaitForSeconds(1);
        ToggleFriendJerk(Friend2);
        ToggleFriendJerk(Friend3);
        yield return new WaitForSeconds(1);
        ToggleFriendJerk(Friend3);
        ToggleFriendJerk(Friend4);
        yield return new WaitForSeconds(1);
        ToggleFriendJerk(Friend4);

        for (float i = 0f; i <= 1f; i += 0.1f)
        {
            SetAlphaCurrentPosition(i);
            yield return new WaitForSeconds(1);
        }

        SetAlphaCurrentPosition(0.0f);
        HideAllFriends();
        ToggleObjectInScreen(Enemy);
        yield return new WaitForSeconds(2);

        //debugSequenceRunning = false;
    }

}
