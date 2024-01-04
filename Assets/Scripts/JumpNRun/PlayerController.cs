using System;
using System.Collections;
using Events;
using Helpers;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rb;
    private float _speed;
    public bool isColliding = false;

    private bool _isGrounded = true;
    private bool _mustFall = false;
    private bool _isJumping = false;
    private float _jumpTimeCounter;
    private float _jumpTime = 0.6f;

    private Animator _animator;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _speed = Settings.MovementSpeed;
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _rb.velocity = new Vector2(_speed, _rb.velocity.y);
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || (Input.touchCount > 0 && Input.GetMouseButtonDown(0))) && !_mustFall)
        {
            if (!_isGrounded) _mustFall = true;
            _isGrounded = false;
            _isJumping = true;
            _jumpTimeCounter = _jumpTime;
            StartCoroutine(Utility.AnimateAnything(0.5f, _speed, _speed * 0.7f,
                (progress, start, end) => _speed = Mathf.Lerp(start, end, progress)));
            _rb.velocity = Vector2.up * 8f;
        }

        if ((Input.GetKey(KeyCode.Space) || (Input.touchCount > 0 && Input.GetMouseButton(0))) && _isJumping)
        {
            if (_jumpTimeCounter > 0)
            {
                _rb.velocity = Vector2.up * 6f;
                _jumpTimeCounter -= Time.deltaTime;
                SceneController.Instance.decayEvent.Invoke(new DecayInstance(0.1f, -10));
            }
            else
            {
                NowFalling();
            }
        }

        if (Input.GetKeyUp(KeyCode.Space) || (Input.touchCount == 0 && Input.GetMouseButtonUp(0)))
        {
            NowFalling();
        }
    }

    private void NowFalling()
    {
        StartCoroutine(Utility.AnimateAnything(0.5f, _speed, Settings.MovementSpeed,
            (progress, start, end) => _speed = Mathf.Lerp(start, end, progress)));
        _isJumping = false;
        _rb.gravityScale = 2f;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.isTrigger)
        {
            switch (other.tag)
            {
                case "SmallLightning":
                    SceneController.Instance.regenerateEvent.Invoke(new RegenerationInstance(5f, 2f));
                    break;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.tag.Equals("Obstacle"))
        {
            StartCoroutine(Boink());
        }
        else if (other.transform.tag.Equals("Floor"))
        {
            _isGrounded = true;
            _rb.gravityScale = 1f;
            _mustFall = false;
            _speed = Settings.MovementSpeed;
        }
    }

    private IEnumerator Boink()
    {
        isColliding = true;
        _animator.SetTrigger("bounce");
        _speed *= -1;
        yield return new WaitForSeconds(0.2f);
        _speed = 0;
        yield return new WaitForSeconds(1f);
        _speed = Settings.MovementSpeed;
        isColliding = false;
        yield return null;
    }
}