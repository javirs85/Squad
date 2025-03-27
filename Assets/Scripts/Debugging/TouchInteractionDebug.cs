using UnityEngine;

public class TouchInteractionDebug : MonoBehaviour
{
    public void ChangeMaterialColor()
    {
        Debug.Log("I am " + gameObject.name + "! Hear me roar!");
        gameObject.GetComponent<Renderer>().material.color = Color.red;
    }
}
