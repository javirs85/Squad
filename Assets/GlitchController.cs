using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GlitchController : MonoBehaviour
{
	public Renderer GlitchRenderer; // Assign the quad's MeshRenderer
	private Material GlitchMaterial; // Assign the material with fade support
	private AudioSource GlitchAudioSource;

	public GameObject ScreenBase;
	private List<GameObject> _hiddenDuringGlitch = new List<GameObject>();

	[UnityEngine.Range(0, 1)] public float MaxAlpha = 0.8f;
	// Start is called once before the first execution of Update after the MonoBehaviour is created

	private void Awake()
	{
		GlitchRenderer = GetComponent<Renderer>();
		GlitchMaterial = GlitchRenderer.material;
		GlitchRenderer.enabled = false;
		GlitchAudioSource = GetComponent<AudioSource>();
		if (GlitchAudioSource == null)
		{
			throw new System.Exception("AudioSource is not assigned to the GlitchController.");
		}
		this.gameObject.SetActive(true);
	}

	public void Hide()
	{
		GlitchRenderer.enabled = false;
		if (GlitchAudioSource != null)
		{
			GlitchAudioSource.Stop();
			GlitchAudioSource.volume = 0f;
		}
		SetAlpha(0f);
	}


	public void TriggerGlitch()
	{
		StartCoroutine(GlitchRoutine());
	}

	private IEnumerator GlitchRoutine()
	{
		HideSiblingsExceptBase();

		GlitchRenderer.enabled = true;
		if (GlitchAudioSource != null)
		{
			GlitchAudioSource.volume = 0f;
			GlitchAudioSource.Play();
		}

		float[] glitchPattern = new float[] {
			0f, 0.1f,  // off briefly
            1f, 0.05f, // on briefly
            0f, 0.02f,
			1f, 0.1f,
			0f, 0.04f,
			1f, 0.1f,
			0.8f, 0.1f,
			0.7f, 0.2f,
			0.5f, 0.2f,
			0.4f, 0.1f,
			0.1f, 0.1f,
			0f, 0.1f
		};

		for (int i = 0; i < glitchPattern.Length; i += 2)
		{
			float targetAlpha = glitchPattern[i];
			float duration = glitchPattern[i + 1];
			SetAlpha(targetAlpha);

			if (GlitchAudioSource != null)
				GlitchAudioSource.volume = targetAlpha; 

			yield return new WaitForSeconds(duration);
		}

		SetAlpha(0f);
		if (GlitchAudioSource != null)
		{
			GlitchAudioSource.volume = 0f;
			GlitchAudioSource.Stop();
		}
		GlitchRenderer.enabled = false;

		RestoreHiddenSiblings();
	}

	private void SetAlpha(float alpha)
	{
		var mat = GlitchMaterial;
		Color c = mat.color;
		c.a = alpha * MaxAlpha;
		mat.color = c;
	}

	private void HideSiblingsExceptBase()
	{
		_hiddenDuringGlitch.Clear();

		Transform parent = transform.parent;
		if (parent == null) return;

		foreach (Transform child in parent)
		{
			if (child.gameObject != ScreenBase && 
				child.gameObject != this.gameObject &&
				child.gameObject.name != "SerialNumber" &&
				child.gameObject.activeSelf)
			{
				child.gameObject.SetActive(false);
				_hiddenDuringGlitch.Add(child.gameObject);
			}
		}
	}

	private void RestoreHiddenSiblings()
	{
		foreach (var obj in _hiddenDuringGlitch)
		{
			obj.SetActive(true);
		}

		_hiddenDuringGlitch.Clear();
	}
}
