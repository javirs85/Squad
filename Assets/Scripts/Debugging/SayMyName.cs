using UnityEngine;

public class SayMyName : MonoBehaviour
{
    public void SayName()
    {
        Debug.Log("My name is " + gameObject.name);
    }
}
