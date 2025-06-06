using UnityEngine;
using UnityEngine.InputSystem;

public class Missile : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float speed;
    [SerializeField] GameObject missileModel;
    [SerializeField] ParticleSystem missileSmoke;
    [SerializeField] GameObject explosionPrefab;
    Vector3 defaultPosition;
    bool launched = false;

    Vector3 TargetPosition = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        defaultPosition = transform.position;
        missileSmoke.Stop();
        TargetPosition = target.position;
	}

    // Update is called once per frame
    void Update()
    {
        if (launched)
        {
            transform.position = Vector3.MoveTowards(transform.position, TargetPosition, speed * Time.deltaTime);
        
            if(Vector3.Distance(transform.position, TargetPosition) < 1)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                ResetMissile();
            }
        }

        if(Keyboard.current.mKey.wasReleasedThisFrame)
        {
            if(!launched)
                LaunchMissile();
        }
    }

    public void LaunchMisileDemo()
    {
		TargetPosition = new Vector3(0,0,50);
		missileModel.SetActive(true);
		missileSmoke.Play();
		launched = true;
	}

    public void LaunchMissile()
    {
        missileModel.SetActive(true);
        missileSmoke.Play();
        launched = true;
    }

    void ResetMissile()
    {
        transform.position = defaultPosition;
		TargetPosition = target.position;
		missileModel.SetActive(false);
        missileSmoke.Stop();
        launched = false;
    }
}
