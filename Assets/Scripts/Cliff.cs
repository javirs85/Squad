using UnityEngine;

public class Cliff : MonoBehaviour
{
    [SerializeField]
    GameObject cliff;
    [SerializeField]
    Transform startPoint, endPoint;
    [SerializeField]
    float speed = 5f;

    void Update()
    {
        cliff.transform.position = Vector3.MoveTowards(cliff.transform.position, endPoint.position, speed * Time.deltaTime);    
    }

    public void ResetCliffs()
    {
        cliff.transform.position = startPoint.position;
    }
}
