using UnityEngine;

public class BulletController : MonoBehaviour
{


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        //Check if colliding with walls
        if (other.gameObject.layer == 6)
        {
            transform.gameObject.SetActive(false);
        }

        //Check if colliding with Balls
        if (other.gameObject.tag == "Ball")
        {
            transform.gameObject.SetActive(false);
        }

    }
}
