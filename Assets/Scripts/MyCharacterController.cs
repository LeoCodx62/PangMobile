using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class MyCharacterController : MonoBehaviour
{



    [SerializeField]
    private float _velocity = 5f;

    [SerializeField]
    private float _verticalBulletForce = 200f;


    [SerializeField]
    private float _harpoonCooldown = 1.5f;

    [SerializeField]
    private float _shieldDuration = 4.0f;
    [SerializeField]
    private GameObject _bulletPrefab;

    [SerializeField]
    private GameObject _harpoonPrefab;

    [SerializeField]
    private Transform _shootTransform;

    [SerializeField]
    private GameObject _shield;

    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private Rigidbody _rb;
    private bool _isMoving;
    private bool _isClimbing;
    private bool _isGrounded;
    private bool _onTopLadder;
    private bool _canClimb;
    private bool _isShooting;
    private bool _isShieldActive = false;
    private float _horizontalInput;
    private float _verticalInput;

    private PlayerInput _playerInput;
    private InputAction _moveAction;
    private InputAction _shootAction;



    private float _harpoonSpawnOffset = 0.31f;
    private int _numberHarpoonAvailables = 1;
    private WeaponType _currentWeapon = WeaponType.Harpoon;

    private int _playerLayer;
    private int _ignoreBallLayer;

    void Awake()
    {
        _playerLayer = LayerMask.NameToLayer("Player");
        _ignoreBallLayer = LayerMask.NameToLayer("IgnoreCollisionWithBalls");


    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _playerInput = GetComponent<PlayerInput>();


        if (_playerInput)
        {
            _moveAction = _playerInput.actions["Move"];
            _shootAction = _playerInput.actions["Shoot"];
        }

        _shield.SetActive(_isShieldActive);
    }

    // Update is called once per frame
    void Update()
    {

        //Movement
        _horizontalInput = _moveAction.ReadValue<Vector2>().x;
        _verticalInput = _moveAction.ReadValue<Vector2>().y;


        //If on topLadder con't move up
        if (_onTopLadder && _verticalInput > 0)
        {
            _verticalInput = 0;
        }

        //If on ground can't move down
        if (_isGrounded && _verticalInput < 0)
        {
            _verticalInput = 0;
        }


        _isMoving = _horizontalInput != 0 && (_isGrounded || _onTopLadder);

        _isClimbing = _verticalInput != 0 && _horizontalInput == 0 && _canClimb;


        //Manage Animator
        _animator.SetBool("IsMoving", _isMoving);
        _animator.SetBool("IsClimbing", _isClimbing);
        _animator.SetBool("IsGrounded", _isGrounded || _onTopLadder);


        //Shooting
        if (_shootAction.ReadValue<float>() > 0.5f && !_isShooting)
        {
            switch (_currentWeapon)
            {
                case WeaponType.Harpoon:
                    {
                        shootHarpoon();
                    }
                    break;
                case WeaponType.DoubleHarpoon:
                    {
                        shootHarpoon();
                    }
                    break;
                case WeaponType.Gun:
                    {
                        _isShooting = true;
                        _animator.SetBool("Shoot", _isShooting);
                        GameObject bullet = Instantiate(_bulletPrefab, _shootTransform.position, _shootTransform.rotation);
                        bullet.GetComponent<Rigidbody>().AddForce(new Vector3(0, _verticalBulletForce, 0), ForceMode.Force);
                    }
                    break;

            }
        }
    }

    private void shootHarpoon()
    {
        if (_numberHarpoonAvailables != 0)
        {
            _isShooting = true;
            _animator.SetBool("Shoot", _isShooting);
            Instantiate(_harpoonPrefab, new Vector3(transform.position.x, transform.position.y + _harpoonSpawnOffset, transform.position.z), transform.rotation);
            _numberHarpoonAvailables--;
            if (_numberHarpoonAvailables == 0)
            {
                StartCoroutine(StartHarpoonCooldown());
            }
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void LaunchUp()
    {
        _rb.AddForce(Vector2.up * 4, ForceMode.Impulse);
    }

    //Called as animation event at the end of the shoting animation   
    private void StopShooting()
    {
        _isShooting = false;
        _animator.SetBool("Shoot", _isShooting);
    }

    //Called as animation event at the end of the death animation. Reaload the scene   
    private void deathAnimationEnded()
    {
        GameManager.Instance?.ReturnToMenu();
    }

    private void Move()
    {

        if (_isMoving && !_isShooting && (_isGrounded || _onTopLadder))
        {
            float direction = Math.Sign(_horizontalInput);
            _rb.useGravity = true;

            if (direction > 0)
            {
                _spriteRenderer.flipX = false;
            }
            else
            {
                _spriteRenderer.flipX = true;
            }

            if (_rb)
            {
                Vector2 currentPosition = _rb.position;
                Vector2 moveDelta = new Vector2(direction * _velocity * Time.deltaTime * Mathf.Abs(_horizontalInput), 0f);
                Vector2 newPosition = currentPosition + moveDelta;

                _rb.MovePosition(newPosition);
            }
        }



        if (_canClimb && _verticalInput != 0)
        {
            _rb.useGravity = false;
            float direction = Math.Sign(_verticalInput);

            if (_rb)
            {
                Vector2 currentPosition = _rb.position;
                Vector2 moveDelta = new Vector2(0f, direction * _velocity * Time.deltaTime * Mathf.Abs(_verticalInput));
                Vector2 newPosition = currentPosition + moveDelta;

                _rb.MovePosition(newPosition);
            }

        }

    }

    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Floor")
        {
            _isGrounded = true;
            _onTopLadder = false;
        }

        if (collision.gameObject.tag == "TopLadder")
        {
            _canClimb = true;
            if (_isClimbing)
            {
                GameObject TopLadderGO = collision.gameObject;
                TopLadderGO.GetComponent<Collider>().isTrigger = true;
            }
        }

        if (collision.gameObject.tag == "Ball")
        {
            KillPlayer();
        }

        if (collision.gameObject.tag == "Item")
        {
            GameObject itemGO = collision.gameObject;

            switch (itemGO.GetComponent<Item>().itemType)
            {
                case ItemType.Weapon:
                    _currentWeapon = itemGO.GetComponent<WeaponController>().GetWeaponType();
                    UIManager.Instance.updateEquipedWeapon(itemGO.GetComponent<SpriteRenderer>().sprite);
                    SetNumberHarpoon();
                    break;
                case ItemType.Shield:
                    EnableShield();
                    StartCoroutine(StartShieldDuration());
                    break;
            }

            Destroy(itemGO);
        }

    }

    void KillPlayer()
    {
        _animator.SetBool("IsDead", true);
        LaunchUp();
        GameManager.Instance?.ResetPoints();
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            _isGrounded = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        //Check if player is on top and is trying going down
        if (collision.gameObject.tag == "TopLadder")
        {
            //Press down
            if (_verticalInput < 0)
            {
                _onTopLadder = false;
                GameObject TopLadderGO = collision.gameObject;
                TopLadderGO.GetComponent<Collider>().isTrigger = true;
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {

        if (collider.gameObject.tag == "BottomLadder")
        {
            _canClimb = true;
        }

    }

    void OnTriggerExit(Collider collider)
    {
        //Check if exiting ladder object 
        if (collider.gameObject.tag == "BottomLadder")
        {
            _canClimb = false;
        }

        if (collider.gameObject.tag == "TopLadder")
        {
            if (_verticalInput > 0)
            {
                _canClimb = false;
                HasToppedLadder();
                //Make it colliding with player
                collider.isTrigger = false;
            }
        }

    }

    void HasToppedLadder()
    {
        _rb.useGravity = true;
        _onTopLadder = true;
    }

    public IEnumerator StartHarpoonCooldown()
    {

        yield return new WaitForSeconds(_harpoonCooldown);

        SetNumberHarpoon();
    }

    public IEnumerator StartShieldDuration()
    {

        yield return new WaitForSeconds(_shieldDuration);
        DisableShield();
    }

    void EnableShield()
    {
        _isShieldActive = true;
        _shield.SetActive(_isShieldActive);
        //Move the player to the ignoreCollisionWithBalls layer, collision with Ball are NOT detected
        gameObject.layer = _ignoreBallLayer;
    }

    void DisableShield()
    {
        _isShieldActive = false;
        _shield.SetActive(_isShieldActive);
        //Move the player to the player level, collision with Ball are detected
        gameObject.layer = _playerLayer;

    }

    void SetNumberHarpoon()
    {
        if (_currentWeapon == WeaponType.Harpoon)
        {
            _numberHarpoonAvailables = 1;
        }
        if (_currentWeapon == WeaponType.DoubleHarpoon)
        {
            _numberHarpoonAvailables = 2;
        }
    }
}
