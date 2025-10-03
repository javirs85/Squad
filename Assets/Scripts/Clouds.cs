using UnityEngine;

public class Clouds : MonoBehaviour
{
    [SerializeField] ParticleSystem clouds;
    [SerializeField] GameObject hiddenObject;

    private void Start()
    {
        Invoke("ShowHiddenObject", 5);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ShowHiddenObject()
    {
        hiddenObject.SetActive(true);
    }
}
