using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using Helpers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rb;
    private float _speed;
    private bool _isJumping = false;
    public bool isColliding = false;

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
        if ((Input.touchCount > 0 || Input.GetKeyDown(KeyCode.Space)) && !_isJumping)
        {
            _isJumping = true;
            _rb.AddForce(Vector2.up * 6, ForceMode2D.Impulse);
            _animator.SetTrigger("jump");
        }
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
        } else if (other.transform.tag.Equals("Floor"))
        {
            _isJumping = false;
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