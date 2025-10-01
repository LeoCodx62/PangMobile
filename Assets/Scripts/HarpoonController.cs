using UnityEngine;

public class HarpoonController : MonoBehaviour
{

    private BoxCollider _collider;
    private Animator _animator;
    private SpriteRenderer _spriteRender;
    public Sprite[] spritesArray;

    private bool _isWiring = false;
    private float _initialColliderSizeY = 0.35f;
    private float _maxColliderSizeY = 1.9243f;


    void Awake()
    {
        _collider = GetComponent<BoxCollider>();
        _animator = GetComponent<Animator>();
        _spriteRender = GetComponent<SpriteRenderer>();
        _initialColliderSizeY = _collider.size.y;
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _isWiring = true;
        _animator.SetBool("IsWiring", _isWiring);
    }

    // Update is called once per frame
    void Update()
    {
        if (_isWiring)
        {
            if (_spriteRender.sprite != null)
            {
                // Match collider size to sprite bounds
                _collider.size = _spriteRender.sprite.bounds.size;
                _collider.center = _spriteRender.sprite.bounds.center;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ceiling" || other.gameObject.tag == "Brick")
        {
            StopWiring();
        }

        if (other.gameObject.tag == "Ball")
        {
            DestroyParent();
        }
    }


    void StopWiring()
    {
        _isWiring = false;
        _animator.SetBool("IsWiring", _isWiring);
        UpdateSpriteBasedOnColliderHeight();
        DestroyParent();
    }


    private void UpdateSpriteBasedOnColliderHeight()
    {
        float currentY = _collider.size.y;
        float t = Mathf.InverseLerp(_initialColliderSizeY, _maxColliderSizeY, currentY);
        int index = Mathf.FloorToInt(t * (spritesArray.Length - 1));

        if (index != spritesArray.Length - 1)
        {
            index++;    //If is not the last sprite, use the next spirte. To avoid empty space.
        }

        index = Mathf.Clamp(index, 0, spritesArray.Length - 1);
        _animator.enabled = false; // Disattiva animazioni
        _spriteRender.sprite = spritesArray[index];

    }

    private void DestroyParent()
    {
        Destroy(transform.parent.gameObject);
    }
}
