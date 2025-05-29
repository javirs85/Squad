using NUnit.Framework;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Tutorial Sequence", menuName = "ScriptableObjects/Tutorial Sequence")]
public class TutorialSequence : ScriptableObject
{
    public string tutorialName;
    public TutorialController.SubtitleName tutorialSubtitles;
    public AudioClip tutorialClip;
    [TextArea(2, 3)]
    public string[] subtitleList;

    //public Subtitle[] subtitles;

    //[Serializable]
    //public struct Subtitle
    //{
    //    [TextArea(2, 3)]
    //    public string subtitleText;
    //    public float subtitleDuration;
    //}
}
