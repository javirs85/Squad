using UnityEngine;

public class FlipCoin : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.N))
        {
			//rotate this element 180deg on y axis
			transform.Rotate(0, 180, 0);
		}
    }
}
