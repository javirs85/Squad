using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
    public static SceneControl instance;
    public VRFader Fader;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }


	void Update()
    {
        
        if(Keyboard.current.f1Key.wasReleasedThisFrame)
        {
            ChangeScene(Scenes.MainScene);
        }
    }

    public enum Scenes { AmplfierSelector, MainScene}

	public void ChangeScene(Scenes scene, Action action = null, bool doFadeOutFirst = true)
    {
        string sceneName = "";
        if (scene == Scenes.AmplfierSelector) sceneName = "Unicorn Selection";
        if (scene == Scenes.MainScene) sceneName = "Main Scene";

        StartCoroutine(GoToWithFadeAsync(sceneName));
        
	}

	IEnumerator GoToWithFadeAsync(string SceneName, bool doFadeOutFirst = true)
    {
        if(doFadeOutFirst)
			Fader.FadeOut();

        AsyncOperation op = SceneManager.LoadSceneAsync(SceneName);
        op.allowSceneActivation = false;
        float timer = 0;
		while (timer < Fader.FadeDuration)
		{
			timer += Time.deltaTime;
			yield return null;
		}

        op.allowSceneActivation = true;
        Fader.FadeIn();
	}

	IEnumerator ChangeSceneRaw(string SceneName)
	{
		AsyncOperation op = SceneManager.LoadSceneAsync(SceneName);
		op.allowSceneActivation = false;
		float timer = 0;
		while (timer < Fader.FadeDuration)
		{
			timer += Time.deltaTime;
			yield return null;
		}
		op.allowSceneActivation = true;
	}


}
