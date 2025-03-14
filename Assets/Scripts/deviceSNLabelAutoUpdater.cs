using Gtec.Bandpower;
using TMPro;
using UnityEngine;

public class deviceSNLabelAutoUpdater : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		var device = FindFirstObjectByType<Device>();
        var text = GetComponent<TextMeshPro>();
		if (device != null)
        {
            text.text = device.Serial;
        }
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
