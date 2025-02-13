using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

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

	[Header("AlphaMarker")]
	public GameObject AlphaObject;
	AlphaMarkerController Alpha;

	[Header("HUD")]
	public GameObject HUD;
	public GameObject Cockpit;

	//Planes used for selecting amplifier
	private List<GameObject> PlaneOptions = new();

	private bool debugSequenceRunning = false;


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
		Alpha = AlphaObject.GetComponent<AlphaMarkerController>();
		HideHUD(0.1f);
		HideCockpit();
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
		if (Input.GetKeyUp(KeyCode.Alpha1))
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
		if (Input.GetKeyUp(KeyCode.C))
			ToggleHUD();

		if (Input.GetMouseButtonDown(0))
		{
			ProcessClick(Input.mousePosition);
		}
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
		{
			ProcessClick(Input.GetTouch(0).position);
		}

	}
	void ProcessClick(Vector2 screenPosition)
	{
		//We only care about selections if there is a selection requested
		if (_RunningSelectionTaskResult is not null)
		{
			Ray ray = Camera.main.ScreenPointToRay(screenPosition);
			if (Physics.Raycast(ray, out RaycastHit hit))
			{
				_RunningSelectionTaskResult?.TrySetResult(hit.collider.gameObject);
			}
		}
	}


	private bool isHUDon = false;
	void ShowHUD() {
		var Children = HUD.GetComponentsInChildren<Transform>();
		foreach (var Child in Children)
		{
			StartCoroutine(FadeINOUTHelper.FadeInAndOut(Child.gameObject, true, 1.0f));
		}
		isHUDon = true;
		HUD.SetActive(true);
	}
	void HideHUD(float Time = 1.0f) {
		var Children = HUD.GetComponentsInChildren<Transform>();
		foreach (var Child in Children)
		{
			StartCoroutine(FadeINOUTHelper.FadeInAndOut(Child.gameObject, false, Time));
		}
		isHUDon = false;
	}
	void ToggleHUD()
	{
		if (isHUDon) HideHUD();
		else ShowHUD();
	}

	public void ShowCockpit() => Cockpit?.SetActive(true);
	public void HideCockpit() => Cockpit?.SetActive(false);


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

	void SetAlphaReference(float alpha)
	{
		Alpha.SetReferenceValue(alpha);
		Alpha.SetAlphaPosition(Alpha.AlphaValue);
	}

	enum ObjectPositions { OnScreen, OutsideScreen };
	void MoveObject(ObjectPositions pos, GameObject obj)
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
		if (i == 0) MoveObject(ObjectPositions.OnScreen, Friend1);
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



	private TaskCompletionSource<GameObject> _RunningSelectionTaskResult;
	public async Task<string> GetUserSelection(List<string> options)
	{
		//this is running on a tread that is not mainthread
		//therefore we cannot acces the obj.name straightforwardly

		await MainThreadExecutor.RunOnMainThread(
				async () => {
					await DestroyAmplifiersOnScreenAsync();
				});
		PlaneOptions.Clear();

		PlaneOptions = 
			await MainThreadExecutor.RunOnMainThread<List<GameObject>>(
				() => {
					return ShowAmplifiersOnScreen(options);
				});

		string SelectedAmp = string.Empty;
		var obj = await WaitForUserClick();
		
		SelectedAmp = await MainThreadExecutor.GetNameFromGameObject(obj);

		//Debug.Log(SelectedAmp);


		if (options.Contains(SelectedAmp))
			return SelectedAmp;
		else 
			return string.Empty;

	}

	public async Task StartTheGame()
	{
		await MainThreadExecutor.RunOnMainThread(
				async () => {
					await StartTheGameInternal();
				});


		await MainThreadExecutor.RunOnMainThread(
				async () => {
					await DestroyAmplifiersOnScreenAsync();
				});

	}

	async Task StartTheGameInternal()
	{
		Cockpit.SetActive(true);
		ShowAllFriends();
		await Task.Delay(1400);
		ShowHUD();
	}

	private List<GameObject> ShowAmplifiersOnScreen(List<string> options)
	{
		List<GameObject> PlaneOptions = new List<GameObject>();
		for (int i = 0; i < options.Count; ++i)
		{
			Vector3 newPos = this.gameObject.transform.localPosition;
			newPos.x += i * 40 - (20*options.Count/2);
			newPos.y = -18.2f;
			newPos.z = 79.0f;
			var fnumber = i % 4;

			GameObject F = Friend1;
			if (fnumber == 0) F = Friend1; 
			else if (fnumber == 2) F = Friend2; 
			else if (fnumber == 3) F = Friend3; 
			else F = Friend4;

			

			GameObject NewOption = Instantiate(F, this.gameObject.transform);
			var textMesh = NewOption.GetComponentInChildren<TextMesh>();
			textMesh.text = options[i];

			var MovObj = NewOption.GetComponent<MovableObject>();
			MovObj.enabled = false;
			NewOption.transform.localPosition = newPos;	
			NewOption.name = options[i];
			NewOption.layer = LayerMask.NameToLayer("UI");
			PlaneOptions.Add(NewOption);
		}

		return PlaneOptions;
	}
	public async Task DestroyAmplifiersOnScreenAsync()
	{
		await MainThreadExecutor.RunOnMainThread(() => {
			foreach (var o in PlaneOptions)
				Destroy(o);
		});
	}

	private async Task<GameObject> WaitForUserClick()
	{
		_RunningSelectionTaskResult = new TaskCompletionSource<GameObject>();
		GameObject obj = await _RunningSelectionTaskResult.Task;
		_RunningSelectionTaskResult = null; // Reset after completion

		return obj;
	}

    public void RunDebugSequence()
    {
        if (!debugSequenceRunning)
            StartCoroutine(FullDebugSequence());
    }

    IEnumerator FullDebugSequence()
    {
        debugSequenceRunning = true;
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

        debugSequenceRunning = false;
    }

}
