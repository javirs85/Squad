using UnityEngine;

public class Cliff : MonoBehaviour
{
    [SerializeField]
    GameObject cliff;
    [SerializeField]
    Transform startPoint, endPoint;
    [SerializeField]
    float speed = 5f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        cliff.transform.position = Vector3.MoveTowards(cliff.transform.position, endPoint.position, speed * Time.deltaTime);    
    }
}
