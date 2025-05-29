using Gtec.Chain.Common.Nodes.FilterNodes;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialController : MonoBehaviour
{
    static TutorialController instance;

    [SerializeField] TextMeshProUGUI subtitleUI;
    [SerializeField] AudioSource tutorialAudioSource;
    [SerializeField] Animator tutorialAnimator;

    [SerializeField] TutorialSequence debugTutorial;

    TutorialSequence currentTutorial;
    int subtitleIndex = 0;

    public enum SubtitleName
    {
        BT,
        Fuel,
        SignalQuality,
        TrainingGeneralInstructions,
        Training_MathStarts,
        Training_RelaxStarts,
        Training_MathEnds,
        PreTraining_Explanation_1,
        PreTraining_Explanation_2,
        PreTraining_Explanation_3
    }

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (Keyboard.current.uKey.wasReleasedThisFrame)
        {
            currentTutorial = debugTutorial;

            StartTutorialSequence(currentTutorial);
        }
    }

    public void StartTutorialSequence(TutorialSequence sequence)
    {
        subtitleIndex = 0;

        tutorialAnimator.SetTrigger(currentTutorial.tutorialName);
        tutorialAudioSource.clip = currentTutorial.tutorialClip;
        tutorialAudioSource.Play();
        //ReproduceSubtitles(currentTutorial.tutorialSubtitles);
    }

    public void PlayTutorialAudio()
    {
        tutorialAudioSource.Play();
    }

    public void NextSubtitle()
    {
        if (subtitleIndex < currentTutorial.subtitleList.Length)
        {
            subtitleUI.text = currentTutorial.subtitleList[subtitleIndex];
            subtitleIndex++;
        }
        else
        {
            subtitleUI.text = "";
        }
    }

    public void ReproduceSubtitles(SubtitleName subtitleName)
    {
        StopCoroutine("ShowSubtitles");

        switch (subtitleName)
        {
            case SubtitleName.BT:
                StartCoroutine("ShowSubtitles", BT);
                break;
            case SubtitleName.Fuel:
                StartCoroutine("ShowSubtitles", Fuel);
                break;
            case SubtitleName.SignalQuality:
                StartCoroutine("ShowSubtitles", SignalQuality);
                break;
            case SubtitleName.TrainingGeneralInstructions:
                StartCoroutine("ShowSubtitles", TrainingGeneralInstructions);
                break;
            case SubtitleName.Training_MathStarts:
                StartCoroutine("ShowSubtitles", Training_MathStarts);
                break;
            case SubtitleName.Training_RelaxStarts:
                StartCoroutine("ShowSubtitles", Training_RelaxStarts);
                break;
            case SubtitleName.Training_MathEnds:
                StartCoroutine("ShowSubtitles", Training_MathEnds);
                break;
            case SubtitleName.PreTraining_Explanation_1:
                StartCoroutine("ShowSubtitles", PreTraining_Explanation_1);
                break;
            case SubtitleName.PreTraining_Explanation_2:
                StartCoroutine("ShowSubtitles", PreTraining_Explanation_2);
                break;
            case SubtitleName.PreTraining_Explanation_3:
                StartCoroutine("ShowSubtitles", PreTraining_Explanation_3);
                break;
            default:
                break;
        }
    }

    private IEnumerator ShowSubtitles(List<Subtitle> subtitles)
    {
        foreach (Subtitle subtitle in subtitles)
        {
            subtitleUI.text = subtitle.Text;

            yield return new WaitForSeconds(subtitle.Duration);
        }

        subtitleUI.text = "";
    }

    List<Subtitle> BT = new List<Subtitle>
    {
        new Subtitle("Whoa—did you feel that? Bit of static just now, like the screen had a hiccup?", 4.5f),
        new Subtitle("That, cadet, was a Bluetooth glitch.", 2.0f),
        new Subtitle("Nothing to worry about—just means a few brain signals from your BCI thingy didn’t get through.", 5.0f),
        new Subtitle("Every now and then, it’s normal.", 2.0f),
        new Subtitle("But if you start seeing a lot of those, the connection might need checking.", 4.5f),
        new Subtitle("For now, stay sharp and keep flying—minor turbulence like that won’t bring us down.", 6.0f)
    };

    List<Subtitle> Fuel = new List<Subtitle>
    {
        new Subtitle("Alright, now that we’re flying steady—let’s talk fuel.", 3.0f),
        new Subtitle("Look over to the left side of the cockpit. See that gauge?", 2.5f),
        new Subtitle("That’s not just for show—it’s linked straight to the battery level of your BCI thingy.", 4.5f),
        new Subtitle("When it's green, we're good to go.", 2.5f),
        new Subtitle("Yellow? That means power’s running low—might want to land and recharge soon.", 4.5f),
        new Subtitle("But if it hits red... buckle up. The device could shut down any moment, and we’ll be flying blind.", 6.5f),
        new Subtitle("Keep one eye on that gauge, cadet. A sharp pilot always knows their limits.", 4.0f)
    };

    List<Subtitle> SignalQuality = new List<Subtitle>
    {
        new Subtitle("Howdy, cadet. Name’s John—your guide through this flight.", 4.0f),
        new Subtitle("See those cheerful little planes flying in formation around us?", 3.5f),
        new Subtitle("Those are our wingmen, and they’re tuned in to your BCI thingy—that brain-reading gizmo you’ve got hooked up.", 6.0f),
        new Subtitle("Each plane represents one electrode. If one starts bobbing through turbulence,", 4.0f),
        new Subtitle("its signal’s getting jittery. And if it vanishes from the sky, that signal’s gone dark.", 6.0f),
        new Subtitle("Right now, they’re all flying smooth and steady—that means everything's in working order, and you're in full control.", 6.5f),
        new Subtitle("Keep it that way, and we’ll have a fine flight ahead.", 3.0f)
    };

    List<Subtitle> TrainingGeneralInstructions = new List<Subtitle>
    {
        new Subtitle("Alright, cadet — time to tune your controls.", 3.0f),
        new Subtitle("To fly this craft with your mind, we need to understand how your brain behaves in different conditions.", 6.0f),
        new Subtitle("First, we’ll throw a few challenges your way—nothing too wild, just some quick mental math.", 6.0f),
        new Subtitle("That’ll show us what your brainpower looks like when it's fully engaged.", 3.5f),
        new Subtitle("Then, you’ll close your eyes and relax. Let your thoughts drift.", 5.5f),
        new Subtitle("That helps us read your baseline when you’re calm.", 3.0f),
        new Subtitle("Relax, focus, and let the system do the reading. We’re just calibrating your flight gear.", 5.5f)
    };

    List<Subtitle> Training_MathStarts = new List<Subtitle>
    {
        new Subtitle("Alright, cadet—focus up. Math challenge incoming. Let’s see what you’ve got.", 5.0f)
    };

    List<Subtitle> Training_RelaxStarts = new List<Subtitle>
    {
        new Subtitle("Now, close your eyes and relax. Stay still until you hear the bell.", 4.0f),
    };

    List<Subtitle> Training_MathEnds = new List<Subtitle>
    {
        new Subtitle("Hmm, looks like we didn’t get a clear read this time.", 3.0f),
        new Subtitle("Sometimes small movements or muscle signals can interfere with the reading — totally normal.", 5.5f),
        new Subtitle("If you want, we can give it another shot. Just try to stay still and focus during the challenges.", 6.0f),
        new Subtitle("No worries if it’s tough right now; some folks need a few tries, and others might find this just isn’t their best fit.", 7.0f),
        new Subtitle("Whenever you’re ready, we’ll take another pass.", 2.5f)
    };

    List<Subtitle> PreTraining_Explanation_1 = new List<Subtitle>
{
    new Subtitle("Alright cadet, listen up. Before we start, you need to know how this bird fires.", 3.5f),
    new Subtitle("Let me power up the system…", 2.0f)
};

    List<Subtitle> PreTraining_Explanation_2 = new List<Subtitle>
{
    new Subtitle("There. Do you see those meters lighting up on your dash?", 2.5f),
    new Subtitle("Right in front of you are two meters stacked up — one red, one blue.", 3.0f),
    new Subtitle("That big red one on the bottom? That shows how steady your mind is—how relaxed you are.", 3.5f),
    new Subtitle("When that red bar is full—nice and glowing—that’s your sweet spot.", 3.0f),
    new Subtitle("Hold it there long enough, and the blue bar on top starts lighting up—", 3.0f),
    new Subtitle("Section by section, as you see happening right now.", 2.5f),
    new Subtitle("Keep calm and let it fill all the way to the end—", 2.5f),
    new Subtitle("And boom—you’ll launch a missile straight at the enemy ship ahead.", 3.0f)
};

    List<Subtitle> PreTraining_Explanation_3 = new List<Subtitle>
{
    new Subtitle("Now, to make that system work properly, we first need to calibrate your brain signals.", 3.0f),
    new Subtitle("We’ll do that with a short training flight.", 2.5f),
    new Subtitle("First, a few quick math tasks to see how your brain handles action.", 3.0f),
    new Subtitle("Then you’ll close your eyes and drift—so we can read your calm baseline.", 3.5f),
    new Subtitle("Once we’re dialed in, you’ll be cleared to fire.", 2.5f),
    new Subtitle("Ready? Let’s get you tuned up.", 2.0f)
};


    private class Subtitle
    {
        public string Text { get; set; }
        public float Duration { get; set; }
        public Subtitle(string text, float duration)
        {
            Text = text;
            Duration = duration;
        }
    }

}
