using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField]
    private GameObject _mediumBall;
    [SerializeField]
    private GameObject _smallBall;
    [SerializeField]
    private Size _size;

    [SerializeField]
    private int _pointsValue;
    [SerializeField]
    private float _ballSpawnForce = 2;
    [SerializeField]
    private float _maxHeight = 5f;

    [SerializeField]
    private GameObject[] items;

    [SerializeField]
    private float _spawnItemChance = 0.3f;


    [SerializeField]
    private AudioClip _popClip;
    private Rigidbody _rb;

    private Vector3 _previousVelocity; // Store velocity before collision

    private float _maxVelocity = 9.0f;


    private enum Size
    {
        Big,
        Medium,
        Small,
    }

    void OnEnable()
    {
        _rb = GetComponent<Rigidbody>();

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb.AddForce(Vector3.right * _ballSpawnForce, ForceMode.Impulse);
        if (GameManager.Instance)
        {
            GameManager.Instance.RegisterBall();
        }
        else
        {
            Debug.Log("Game manager not loaded");
        }

    }


    void FixedUpdate()
    {
        // Store velocity BEFORE collision happens
        if (_rb.linearVelocity.magnitude > Mathf.Epsilon) // Avoid storing near-zero values
        {
            _previousVelocity = _rb.linearVelocity;
        }

        if (_rb.linearVelocity.magnitude > 10.0f)
        {
            _rb.linearVelocity = _rb.linearVelocity.normalized * _maxVelocity;

        }
    }

    void OnTriggerEnter(Collider other)
    {
        //Check if colliding with a bullet
        if (other.gameObject.tag == "Bullet")
        {

            AudioManager.Instance?.PlaySEClip(_popClip);
            GameManager.Instance.UpdatePoints(_pointsValue);

            //Spawn or destroy the GO
            switch (_size)
            {
                case Size.Big:
                    {
                        GameObject mediumBall1 = Instantiate(_mediumBall, transform.position, transform.rotation);
                        GameObject mediumBall2 = Instantiate(_mediumBall, transform.position, transform.rotation);

                        mediumBall1.GetComponent<Rigidbody>().AddForce(Vector3.left * _ballSpawnForce, ForceMode.Impulse);
                        mediumBall2.GetComponent<Rigidbody>().AddForce(Vector3.right * _ballSpawnForce, ForceMode.Impulse);
                        SpawnItem();
                        break;
                    }
                case Size.Medium:
                    {

                        GameObject smallBall1 = Instantiate(_smallBall, transform.position, transform.rotation);
                        GameObject smallBall2 = Instantiate(_smallBall, transform.position, transform.rotation);

                        smallBall2.GetComponent<Rigidbody>().AddForce(Vector3.left * _ballSpawnForce, ForceMode.Impulse);
                        smallBall1.GetComponent<Rigidbody>().AddForce(Vector3.right * _ballSpawnForce, ForceMode.Impulse);
                        SpawnItem();
                        break;

                    }
                case Size.Small:
                    {
                        transform.gameObject.SetActive(false);
                        break;
                    }
            }
            transform.gameObject.SetActive(false);
            GameManager.Instance?.DeregisterBall();
        }


    }


    void SpawnItem()
    {

        if (Random.Range(0.0f, 1.0f) < _spawnItemChance)
        {
            Instantiate(items[Random.Range(0, items.Length)], transform.position, transform.rotation);
        }

    }
    void OnCollisionEnter(Collision collision)
    {
        //Check if colliding with walls
        if (collision.gameObject.tag == "LateralWalls" || collision.gameObject.tag == "Brick")
        {
            _rb.linearVelocity = Vector3.Reflect(_previousVelocity, collision.contacts[0].normal);
        }

        if (collision.gameObject.tag == "Floor")
        {
            parabolicBounce(_previousVelocity, collision.contacts[0].normal);
        }
    }

    void parabolicBounce(Vector3 incomingDirection, Vector3 normaleImpatto)
    {
        Vector2 reflectedDirection = Vector2.Reflect(incomingDirection, normaleImpatto);

        float angle = Vector2.Angle(incomingDirection, normaleImpatto);

        if ((angle > 165 && angle < 195) || angle < 100 || angle > 260)
        {
            reflectedDirection.x = normaleImpatto.x > 0 ? 3f : -3f;
            reflectedDirection = reflectedDirection.normalized;
        }

        // Calculate the required velocity to reach the target height
        float velocityY = Mathf.Sqrt(2 * Physics2D.gravity.magnitude * _maxHeight);

        // Calculate the magnitude of the total velocity needed
        float velocityMagnitude = velocityY / Mathf.Abs(reflectedDirection.y);

        // Scale the reflected direction to match the required velocity
        Vector2 launchVelocity = reflectedDirection * velocityMagnitude;

        // Compute the impulse: Impulse = mass * velocity
        Vector2 impulse = launchVelocity * _rb.mass;

        // Apply the impulse to the Rigidbody
        _rb.linearVelocity = Vector2.zero; // Reset velocity before applying new impulse
        _rb.AddForce(impulse, ForceMode.Impulse);

    }

}
