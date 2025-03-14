using UnityEngine;

public class DevicePlane : MonoBehaviour
{
    AmplifierSelector amplifierSelector;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        amplifierSelector = FindFirstObjectByType<AmplifierSelector>();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaneSelected()
    {
        amplifierSelector.SelectAmplifier(gameObject.name);
    }
}
