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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        defaultPosition = transform.position;
        missileSmoke.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (launched)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        
            if(Vector3.Distance(transform.position, target.position) < 1)
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

    public void LaunchMissile()
    {
        missileModel.SetActive(true);
        missileSmoke.Play();
        launched = true;
    }

    void ResetMissile()
    {
        transform.position = defaultPosition;
        missileModel.SetActive(false);
        missileSmoke.Stop();
        launched = false;
    }
}
