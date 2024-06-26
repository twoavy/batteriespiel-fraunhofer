using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Events;
using Helpers;
using UnityEngine;
using JumpNRun;
using Models;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class PlayerController : MonoBehaviour, DieEvent.IUseDie
{
    private Rigidbody2D _rb;
    private float _baseSpeed;
    private float _speed = 0;
    public bool isColliding = false;
    public bool isStill = false;
    private bool _finished = false;

    private bool _isGrounded = true;
    private bool _mustFall = false;
    private bool _isJumping = false;
    private float _jumpTimeCounter;
    private float _jumpTime = 0.2f;
    private bool _started = false;
    
    private Animator _animator;
    private Coroutine _boink = null;
    private ScoreController _scoreController;

    private int _collectedCount = 0;

    public float smallest = 0f;
    private bool _dead = false;
    
    private bool _willjumpInFixedUpdate = false;
    
    public float GeneralSpeed
    {
        get
        {
            float maxSpeed = Settings.MovementSpeed;
            float minSpeed = -6.5f;
            return _speed.MapBetween(minSpeed, maxSpeed, -1, 1);
        }
    }

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _baseSpeed = Settings.MovementSpeed;
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        Debug.Log(Camera.main.orthographicSize);
        _scoreController = GameObject.Find("Canvas").GetComponentInChildren<ScoreController>();
        SceneController.Instance.dieEvent.AddListener(UseDie);
        _animator.speed = 2.5f;
        _rb.gravityScale = 2f;
    }

    public void StartRunning()
    {
        _started = true;
        StartCoroutine(Utility.AnimateAnything(1f, 0, Settings.MovementSpeed,
            (progress, start, end) => _speed = Mathf.Lerp(start, end, progress)));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _rb.velocity = new Vector2(_speed, _rb.velocity.y);
        if (_willjumpInFixedUpdate)
        {
            if (!_isGrounded) _mustFall = true;
            if (_mustFall)
            {
                _animator.ResetTrigger("jump");
                _animator.SetTrigger("walk");
            }
            _isGrounded = false;
            //_rb.AddForce(Vector2.up * (_isJumping ? 80f : 100f));
            _rb.velocity = Vector2.up * (_isJumping ? 10f : 12f);
            if (_isJumping)
            {
                NowFalling();
            }
            _isJumping = true;
            _jumpTimeCounter = _jumpTime;
            
            _animator.SetTrigger("jump");
            _willjumpInFixedUpdate = false;
        }
    }

    private void Update()
    {
        if (isColliding || _finished || _dead || !_started)
        {
            return;
        }

        if (transform.position.y < -3.3)
        {
            Debug.Log("die now");
        }
        
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.UpArrow) ||
             (Input.touchCount > 0 && Input.touches.ElementAtOrDefault(0).phase == TouchPhase.Began)) && !_mustFall)
        {
            _willjumpInFixedUpdate = true;
        }
        

        if (Input.GetKeyUp(KeyCode.Space) ||
            (Input.touchCount == 0 && Input.touches.ElementAtOrDefault(0).phase == TouchPhase.Ended))
        {
            NowFalling();
        }

        Debug.ClearDeveloperConsole();
    }

    private void NowFalling()
    {
        _isJumping = false;
        
        StartCoroutine(Utility.AnimateAnything(0.2f, 2.0f, 3.0f,
            (progress, start, end) => _rb.gravityScale = Mathf.Lerp(start, end, progress)));
    }

    private async void OnTriggerEnter2D(Collider2D other)
    {
        if (!_started) return;
        
        if (other.isTrigger)
        {
            switch (other.tag)
            {
                case "Killzone":
                    SceneController.Instance.dieEvent.Invoke();
                    break;
                case "BlueLightning":
                    FadeCollectable(other.GetComponent<SpriteRenderer>());
                    SceneController.Instance.collectEvent.Invoke(Collectable.BlueLightning);
                    if (!Debug.isDebugBuild)
                    {
                        GetComponent<AudioSource>().Play();    
                    }
                    
                    break;
                case "YellowLightning":
                    FadeCollectable(other.GetComponent<SpriteRenderer>());
                    SceneController.Instance.collectEvent.Invoke(Collectable.YellowLightning);
                    if (!Debug.isDebugBuild)
                    {
                        GetComponent<AudioSource>().Play();    
                    }
                    break;
                case "Target":
                    FadeCollectable(other.GetComponent<SpriteRenderer>());
                    _collectedCount++;
                    if (!Debug.isDebugBuild)
                    {
                        GetComponent<AudioSource>().Play();    
                    }
                    SceneController.Instance.collectEvent.Invoke(Collectable.LevelSpecific);
                    if (Math.Min((int)GameState.Instance.GetCurrentMicrogame(), 5) >= 5)
                    {
                        break;
                    }
                    if (_collectedCount == 5)
                    {
                        _finished = true;
                        StartCoroutine(Utility.AnimateAnything(0f, _speed, 0,
                            (progress, start, end) => _speed = Mathf.Lerp(start, end, progress),
                            () =>
                            {
                                Debug.Log("Starting final callbacl");
                                SerializeScore();
                            }));
                    }

                    break;
            }
        }
    }

    private void SerializeScore()
    {
        MicrogameState m = new MicrogameState();
        m.game = GameState.Instance.GetCurrentMicrogame();
        m.unlocked = true;
        m.finished = false;
        m.result = 0;
        m.jumpAndRunResult = _scoreController.GetScoreForApi();

        StartCoroutine(Api.Instance.SetGame(m, GameState.Instance.currentGameState.id, details =>
        {
            Resources.UnloadUnusedAssets();
            GC.Collect();
            GameState.Instance.currentGameState = details;
            int game = ((int)m.game) + 1;
            if (game == 3 || game == 5)
            {
                SceneManager.LoadSceneAsync($"MicroGame{game}");
            }
            else
            {
                SceneManager.LoadSceneAsync($"MicroGame{game}Onboard");
            }
        }));
    }

    private void FadeCollectable(SpriteRenderer s)
    {
        try
        {
            StartCoroutine(Utility.AnimateAnything(0.5f, 1f, 0f,
                (progress, start, end) => s.color = new Color(1f, 1f, 1f, Mathf.Lerp(start, end, progress)),
                () => s.gameObject.SetActive(false)));
        }
        catch (Exception)
        {
            Debug.Log("Cannot collect same collectable twice");
        }
    }

    public bool hasStarted()
    {
        return _started;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        
        if (!_started) return;
        
        switch (other.transform.tag)
        {
            case "Obstacle":
                if (_boink != null)
                {
                    break;
                }
                _isGrounded = true;
                _boink = StartCoroutine(Boink(other.collider, () =>
                {
                    _boink = null;
                    StartCoroutine(Utility.AnimateAnything(1f, 0, Settings.MovementSpeed,
                        (progress, start, end) => _speed = Mathf.Lerp(start, end, progress)));
                }));
                _rb.gravityScale = 2f;
                break;
            case "Floor":
                if (!_isGrounded)
                {
                    _animator.ResetTrigger("jump");
                    _animator.SetTrigger("walk");
                }
                _isGrounded = true;
                _rb.gravityScale = 2f;
                _mustFall = false;
                _speed = Settings.MovementSpeed;
                break;
        }
    }

    private IEnumerator Boink(Collider2D obstacle, Action cb = null)
    {
        isColliding = true;
        _animator.SetTrigger("bounce");
        StartCoroutine(Utility.AnimateAnything(0.5f, 0, -6.5f,
            (progress, start, end) => _speed = Mathf.Lerp(start, end, progress)));
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(Utility.AnimateAnything(1f, _speed, 0,
            (progress, start, end) => _speed = Mathf.Lerp(start, end, progress), () => isStill = true));
        yield return new WaitForSeconds(1f);
        isStill = false;
        isColliding = false;
        cb?.Invoke();
    }

    public void UseDie()
    {
        Debug.Log("-.-.-.-.-.-.-.-.-");
        Debug.Log(GameState.Instance.GetCurrentMicrogame());
        Debug.Log(GameState.Microgames.Microgame6);
        Debug.Log("-.-.-.-.-.-.-.-.-");
        if (GameState.Microgames.Microgame6 == (GameState.Microgames)Math.Min((int)GameState.Instance.GetCurrentMicrogame(), 5))
        {
            MicrogameState e = GameState.Instance.currentGameState.results.FirstOrDefault(r => r.game == GameState.Microgames.Microgame6);

            if (e == null)
            {
                MakeCall();
            }
            else
            {
                if (e.jumpAndRunResult >= _scoreController.GetScoreForApi())
                {
                    Debug.Log("Score is worse than before, consuming...");
                    return;
                }
                else
                {
                    MakeCall();
                }   
            }
        }
        _dead = true;
        GetComponent<BoxCollider2D>().isTrigger = true;
        StartCoroutine(Utility.AnimateAnything(2f, _speed, 0,
            (progress, start, end) => _speed = Mathf.Lerp(start, end, progress),
            () => { _animator.SetTrigger("die"); }));
    }

    private void MakeCall()
    {
        MicrogameState m = new MicrogameState()
        {
            game = GameState.Microgames.Microgame6,
            jumpAndRunResult = _scoreController.GetScoreForApi(),
            result = 0,
            finished = false,
            unlocked = true
        };
        StartCoroutine(Api.Instance.SetGame(m, PlayerPrefs.GetString("uuid"), details =>
        {
            GameState.Instance.currentGameState = details;
        }));
    }
    
}