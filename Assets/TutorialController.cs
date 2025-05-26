using Gtec.Chain.Common.Nodes.FilterNodes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    List<Subtitle> BT = new List<Subtitle>
    {
		new Subtitle("Whoa—did you feel that? Bit of static just now, like the screen had a hiccup?", 0.0f),
		new Subtitle("That, cadet, was a Bluetooth glitch.", 0.0f),
		new Subtitle("Nothing to worry about—just means a few brain signals from your BCI thingy didn’t get through.", 0.0f),
		new Subtitle("Every now and then, it’s normal.", 0.0f),
		new Subtitle( "But if you start seeing a lot of those, the connection might need checking.", 0.0f),
		new Subtitle("For now, stay sharp and keep flying—minor turbulence like that won’t bring us down.", 0.0f)
	};

    List<Subtitle> Fuel = new List<Subtitle>
    {
		new Subtitle("Alright, now that we’re flying steady—let’s talk fuel.", 0.0f),
		new Subtitle("Look over to the left side of the cockpit. See that gauge?", 0.0f),
		new Subtitle("That’s not just for show—it’s linked straight to the battery level of your BCI thingy.", 0.0f),
		new Subtitle("When it's green, we're good to go.", 0.0f),
		new Subtitle("Yellow? That means power’s running low—might want to land and recharge soon.", 0.0f),
	    new Subtitle("But if it hits red... buckle up. The device could shut down any moment, and we’ll be flying blind.", 0.0f),
		new Subtitle("Keep one eye on that gauge, cadet. A sharp pilot always knows their limits.", 0.0f)
	};

    List<Subtitle> SignalQuality = new List<Subtitle>
    {
		new Subtitle("Howdy, cadet. Name’s John—your guide through this flight.", 0.0f),
		new Subtitle("See those cheerful little planes flying in formation around us?", 0.0f),
		new Subtitle("Those are our wingmen, and they’re tuned in to your BCI thingy—that brain-reading gizmo you’ve got hooked up.", 0.0f),
		new Subtitle("Each plane represents one electrode. If one starts bobbing through turbulence,", 0.0f),
		new Subtitle("its signal’s getting jittery. And if it vanishes from the sky, that signal’s gone dark.", 0.0f),
		new Subtitle("Right now, they’re all flying smooth and steady—that means everything's in working order, and you're in full control.", 0.0f),
	    new Subtitle("Keep it that way, and we’ll have a fine flight ahead.", 0.0f)
	};

	List<Subtitle> PreTraining_Explanation = new List<Subtitle>
	{
		new Subtitle("Alright cadet, listen up. Before we start, you need to know how this bird fires.", 3.5f),
		new Subtitle("Right in front of you are two meters stacked up — one red, one blue.", 3.0f),
		new Subtitle("That big red one on the bottom? That shows how steady your mind is—how relaxed you are.", 3.5f),
		new Subtitle("When that red bar is full—nice and glowing—that’s your sweet spot.", 3.0f),
		new Subtitle("Hold it there long enough, and the blue bar on top starts lighting up, section by section.", 3.5f),
		new Subtitle("Keep calm and let it fill all the way to the end—", 2.5f),
		new Subtitle("and boom—you’ll launch a missile straight at the enemy ship ahead.", 3.0f),
		new Subtitle("But to make that system work right, we first need to calibrate your brain signals.", 3.0f),
		new Subtitle("We’ll do that with a short training flight.", 2.5f),
		new Subtitle("First, a few quick math tasks to see how your brain handles action.", 3.0f),
		new Subtitle("Then you’ll close your eyes and drift—so we can read your calm baseline.", 3.5f),
		new Subtitle("Once we’re dialed in, you’ll be cleared to fire.", 2.5f),
		new Subtitle("Ready? Let’s get you tuned up.", 2.0f)
	};

	List<Subtitle> TrainingGeneralInstructions = new List<Subtitle>
    {
		new Subtitle("Alright, cadet—time to tune your controls.", 0.0f),
		new Subtitle("To fly this craft with your mind, we need to understand how your brain behaves in different conditions.", 0.0f),
		new Subtitle("First, we’ll throw a few challenges your way—nothing too wild, just some quick mental math.", 0.0f),
		new Subtitle("That’ll show us what your brainpower looks like when it's fully engaged.", 0.0f),
		new Subtitle("Then, you’ll close your eyes and relax. Let your thoughts drift.", 0.0f),
		new Subtitle("That helps us read your baseline when you’re calm.", 0.0f),
		new Subtitle("Relax, focus, and let the system do the reading. We’re just calibrating your flight gear.", 0.0f)
	};
	List<Subtitle> Training_MathStarts = new List<Subtitle>
	{
		new Subtitle("Alright, cadet—focus up. Math challenge incoming. Let’s see what you’ve got.", 0.0f) 
	};
	List<Subtitle> Training_RelaxStarts = new List<Subtitle>
	{
		new Subtitle("Now, close your eyes and relax. Stay still until you hear the bell.", 0.0f),
	};

	List<Subtitle> Training_MathEnds = new List<Subtitle>
	{
		new Subtitle("Hmm, looks like we didn’t get a clear read this time.", 0.0f),
		new Subtitle("Sometimes small movements or muscle signals can interfere with the reading—totally normal.", 0.0f),
		new Subtitle("If you want, we can give it another shot. Just try to stay still and focus during the challenges.", 0.0f),
		new Subtitle("No worries if it’s tough right now; some folks need a few tries, and others might find this just isn’t their best fit.", 0.0f),
		new Subtitle("Whenever you’re ready, we’ll take another pass.", 0.0f),
		new Subtitle("Stay still until you hear the bell.", 0.0f)
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
