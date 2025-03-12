using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
    public static SceneControl instance;

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
        if(Input.GetKeyDown(KeyCode.F1))
        {
            ChangeScene(Scenes.MainScene);
        }
    }

    public enum Scenes { AmplfierSelector, MainScene}

	public void ChangeScene(Scenes scene)
    {
        if(scene == Scenes.AmplfierSelector)
		{
			SceneManager.LoadScene("Unicorn Selection");
		}
		else if (scene == Scenes.MainScene)
		{
			SceneManager.LoadScene("Main Scene");
		}
    }
}
