using Gtec.Bandpower;
using TMPro;
using UnityEngine;

public class deviceSNLabelAutoUpdater : MonoBehaviour
{
    public TextMeshPro TextPro;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		var device = FindFirstObjectByType<Device>();
		if (TextPro is not null && device != null)
        {
			TextPro.text = device.Serial;
        }
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
