using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassingClouds : MonoBehaviour
{
    public Transform Camera;
    public Transform TopLeftBig;
    public Transform TopRightBig;
    public Transform TopLeftSmall;
    public Transform TopRightSmall;

    public GameObject Big;
    public GameObject Medium;
    public GameObject Small;

    public float BigCloudMaxSpeed = 10.0f;
    public float BigCloudMinSpeed = 5.0f;
    public float SmallCloudMaxSpeed = 5.0f;
    public float SmallCloudMinSpeed = 1.0f;

    float NextCloudTime;
	List<MovingCloud> Clouds;
	GameObject Parent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Parent = this.gameObject;
        Big.gameObject.SetActive(false);
        Medium.gameObject.SetActive(false);
        Small.gameObject.SetActive(false);
		TopLeftBig.gameObject.SetActive(false);
        TopRightBig.gameObject.SetActive(false);
        TopLeftSmall.gameObject.SetActive(false);
        TopRightSmall.gameObject.SetActive(false);

		Clouds = new List<MovingCloud>();
		Clouds.Add(CreateCloud());
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var cloud in Clouds) 
        {
            if(!cloud.Finished && cloud.Cloud is not null)
            {
				cloud.Cloud.transform.position = new Vector3(
				cloud.Cloud.transform.position.x,
				cloud.Cloud.transform.position.y,
				cloud.Cloud.transform.position.z - cloud.Speed * Time.deltaTime
				);

				if (cloud.Cloud.transform.position.z <= Camera.transform.position.z)
				{
					cloud.Finished = true;
					Destroy(cloud.Cloud);
				}
			}            
        }

        Clouds.RemoveAll(x=>x.Finished);

        if(Time.time > NextCloudTime) CreateCloud();
    }

    MovingCloud CreateCloud()
    {
        MovingCloud mc = new MovingCloud();

        var size = Random.Range(0.0f, 1.0f);
        if (size > 0.8)
        {
			mc.Size = CloudSize.big;
			mc.Cloud = Instantiate(Big, Parent.transform);
            var ReferenceVector = TopRightBig.position - TopLeftBig.position;
            ReferenceVector = ReferenceVector * Random.Range(0.0f, 1.0f);
            mc.Cloud.transform.position = TopLeftBig.position + ReferenceVector;
            mc.Speed = Random.Range(BigCloudMaxSpeed, BigCloudMinSpeed);
		}
        else if(size > 0.4)
        {
            mc.Size = CloudSize.medium;
			mc.Cloud = Instantiate(Medium, Parent.transform);
			var ReferenceVector = TopRightSmall.position - TopLeftSmall.position;
			ReferenceVector = ReferenceVector * Random.Range(0.0f, 1.0f);
			mc.Cloud.transform.position = TopLeftSmall.position + ReferenceVector;
			mc.Speed = Random.Range(SmallCloudMaxSpeed, SmallCloudMinSpeed);
		}
        else { 
            mc.Size = CloudSize.small;
			mc.Cloud = Instantiate(Small, Parent.transform);
			var ReferenceVector = TopRightSmall.position - TopLeftSmall.position;
			ReferenceVector = ReferenceVector * Random.Range(0.0f, 1.0f);
			mc.Cloud.transform.position = TopLeftSmall.position + ReferenceVector;
			mc.Speed = Random.Range(SmallCloudMaxSpeed, SmallCloudMinSpeed);
		}

		mc.Cloud.SetActive(true);
		Clouds.Add(mc);

		NextCloudTime = Time.time + UnityEngine.Random.Range(0.2f, 1.5f);
		return mc;
    }


    enum CloudSize { big,medium, small }    

    class MovingCloud
    {
        public GameObject Cloud;
        public CloudSize Size;
        public float Speed;
        public bool Finished = false;
    }
}
