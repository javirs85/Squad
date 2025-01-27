using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class LateralClouds : MonoBehaviour
{
    public GameObject FirstCloud;
    public GameObject LastCloud;

    public GameObject Cloud1;
    public GameObject Cloud2;
    public GameObject Cloud3;
    public GameObject Cloud4;

    public float MaxSpeed = 3.0f;
    public float MinSpeed = 1.0f;

    float StartPos = 0.0f;
	float EndPos = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    List<MovingCloud> clouds = new List<MovingCloud>();

    void Start()
    {
        StartPos = FirstCloud.transform.localPosition.x;   
        EndPos = LastCloud.transform.localPosition.x;   

        clouds.Add(new MovingCloud { Cloud = Cloud1, Speed = UnityEngine.Random.Range(MinSpeed, MaxSpeed) });
        clouds.Add(new MovingCloud { Cloud = Cloud2, Speed = UnityEngine.Random.Range(MinSpeed, MaxSpeed) });
        clouds.Add(new MovingCloud { Cloud = Cloud3, Speed = UnityEngine.Random.Range(MinSpeed, MaxSpeed) });
        clouds.Add(new MovingCloud { Cloud = Cloud4, Speed = UnityEngine.Random.Range(MinSpeed, MaxSpeed) });
    }

    // Update is called once per frame
    void Update()
    {
        foreach(var cloud in clouds)
		{
			MoveCloud(cloud);
            var d = LastCloud.transform.localPosition.x - cloud.Cloud.transform.localPosition.x+3;
            if(d < 1.0f)
            {
                cloud.Cloud.transform.localPosition = 
                    new Vector3(
                        FirstCloud.transform.localPosition.x,
                        cloud.Cloud.transform.localPosition.y, 
                        cloud.Cloud.transform.localPosition.z);
                cloud.Speed = UnityEngine.Random.Range(MinSpeed,MaxSpeed);
            }
		}
	}

	private void MoveCloud(MovingCloud cloud)
	{
		cloud.Cloud.transform.localPosition = 
            new Vector3(
				cloud.Cloud.transform.localPosition.x + (cloud.Speed * Time.deltaTime), 
                cloud.Cloud.transform.localPosition.y, 
                cloud.Cloud.transform.localPosition.z);
	}

    class MovingCloud
    {
        public GameObject Cloud;
        public float Speed;
    }
}
