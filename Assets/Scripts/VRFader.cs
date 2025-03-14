using System.Collections;
using UnityEngine;

public class VRFader : MonoBehaviour
{
	private Renderer FaderPanelRenderer;

	public bool FadeOnStart = true;
	public float FadeDuration = 2.0f;
	public Color fadeColor;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		FaderPanelRenderer = GetComponent<Renderer>();
		if(FadeOnStart) FadeIn();
	}


	public void FadeIn() => Fade(1, 0);
	public void FadeOut() => Fade(0, 1);

	public void Fade(float alphaIn, float alphaOut) => StartCoroutine(FadeRoutine(alphaIn, alphaOut));


	IEnumerator FadeRoutine(float alphaIn, float AlphaOut)
	{
		float timer = 0;
		while (timer < FadeDuration)
		{
			timer += Time.deltaTime;
			fadeColor.a = Mathf.Lerp(alphaIn, AlphaOut, timer / FadeDuration);
			FaderPanelRenderer.material.color = fadeColor;
			yield return null;
		}
		fadeColor.a = AlphaOut;
		FaderPanelRenderer.material.color = fadeColor;
	}

	public void BlockingFadeIn() => Fade(1, 0);
	public void BlockingFadeOut() => Fade(0, 1);
	public void BlockingFade(float alphaIn, float alphaOut)
	{
		float timer = 0;
		while (timer < FadeDuration)
		{
			timer += Time.deltaTime;
			fadeColor.a = Mathf.Lerp(alphaIn, alphaOut, timer / FadeDuration);
			FaderPanelRenderer.material.color = fadeColor;
		}
		fadeColor.a = alphaOut;
		FaderPanelRenderer.material.color = fadeColor;
	}
}
