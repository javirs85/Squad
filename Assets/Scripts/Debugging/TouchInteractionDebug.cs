using UnityEngine;

public class TouchInteractionDebug : MonoBehaviour
{
    public void ChangeMaterialColor()
    {
        gameObject.GetComponent<Renderer>().material.color = Color.red;
    }
}
