using UnityEngine;
using UnityEngine.SceneManagement;

public class Test_ChangeScene : MonoBehaviour
{
    private void Start()
    {
        Invoke("ChangeScene", 3);
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene("Unicorn Selection");
    }
}
