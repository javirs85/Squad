using UnityEngine;
using static SceneControl;
using UnityEngine.InputSystem;
using System.Collections;

public class CliffCalibrationTest : MonoBehaviour
{
    [SerializeField] VRFader fader;
    [SerializeField] Cliff cliffs;
    [SerializeField] AlphaBarChar testManager;

    [Header("Test Attributes")]
    [SerializeField] float timeBeforeReset = 20f;
    [SerializeField] int repeats = 3;
    private bool endOfTest = false;


    private void Update()
    {
        if (endOfTest)
        {
            testManager.FreeRun();
            endOfTest = false;
        }
    }

    public void StartCliffCalibration()
    {
        StartCoroutine(ActivateCliffsCoroutine());
    }

    private IEnumerator ActivateCliffsCoroutine()
    {
        for (int i = 0; i < repeats; i++)
        {
            fader.FadeOut();

            yield return new WaitForSeconds(3);

            cliffs.ResetCliffs();
            cliffs.gameObject.SetActive(true);
            fader.FadeIn();

            yield return new WaitForSeconds(timeBeforeReset);
        }

        endOfTest = true;
    }
}
