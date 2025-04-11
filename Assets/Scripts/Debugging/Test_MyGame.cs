using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Test_MyGame : MonoBehaviour
{

    public GameObject Cube;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.qKey.wasReleasedThisFrame)
            SceneManager.LoadScene("TEST_MainScene");
        if (Keyboard.current.wKey.wasReleasedThisFrame)
            SceneManager.LoadScene("TEST_MainScene 2");

	}

    public void ApplyBandPower(Dictionary<string, double> data)
    {
        float alpha = (float)data["alpha"];
		Cube.transform.localScale = new Vector3(alpha, Cube.transform.localScale.y, Cube.transform.localScale.z);
	}
}
