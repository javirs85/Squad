using UnityEngine;
using static SceneControl;
using UnityEngine.InputSystem;
using System.Collections;

public class CliffCalibrationTest : MonoBehaviour
{
    [SerializeField] VRFader fader;
    [SerializeField] Cliff cliffs;
    private Animator myAnimator;
    private AudioSource myAudioSource;

    private BCI bciController;

    [Header("Test Attributes")]
    [SerializeField] float timeBeforeReset = 20f;
    [SerializeField] int repeats = 3;
    private bool endOfTest = false;
    private bool cycleEnd = false;
    public bool freeRunMode = false;


    private void Start()
    {
        bciController = FindAnyObjectByType<BCI>();
        myAnimator = GetComponent<Animator>();
        myAudioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (endOfTest)
        {
            bciController.StartFreeRun();
            myAudioSource.Play();
            endOfTest = false;
        }

        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            StartCliffCalibration();
        }
    }

    public void StartCliffCalibration()
    {
        StartCoroutine(ActivateCliffsCoroutine());

    }

    private IEnumerator ActivateCliffsCoroutine()
    {
        if(freeRunMode)
        {
            fader.FadeOut();

            yield return new WaitForSeconds(3);

            myAnimator.SetBool("Active", true);
            cliffs.gameObject.SetActive(true);
            myAnimator.SetTrigger("Reset");

            yield return new WaitForSeconds(1);

            fader.FadeIn();
        }
        else
        {
            for (int i = 0; i < repeats; i++)
            {
                cycleEnd = false;
                fader.FadeOut();

                yield return new WaitForSeconds(3);

                myAnimator.SetBool("Active", true);
                cliffs.gameObject.SetActive(true);
                myAnimator.SetTrigger("Reset");

                yield return new WaitForSeconds(1);
                
                fader.FadeIn();

                while(!cycleEnd)
                {
                    yield return null;
                }
            }
        }

        myAnimator.SetBool("Active", false);
        endOfTest = true;
    }

    public void EnteringCliffs()
    {
        if (freeRunMode)
        {
            bciController.StartFreeRun();
        }
        else
        {
            bciController.StartStressMeasuring();
        }

        myAudioSource.Play();
    }

    public void ExitingCliffs()
    {
        if (freeRunMode)
        {
            bciController.FinishFreeRun();
        }
        else
        {
            bciController.FinishStressMeasuring();
            bciController.StartRelaxMeasuring();
        }

        myAudioSource.Play();
    }

    public void EndRelaxPeriod()
    {
        if (freeRunMode)
            return;

        cycleEnd = true;
        bciController.FinishRelaxMeasuring();

        myAudioSource.Play();
    }


}
