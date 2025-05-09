using System.Collections;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;



public class PlayerController : MonoBehaviour
{
    //CharacterController _controller;

    [SerializeField] ParticleSystem _hitParticle;           // 피격 파티클
    [SerializeField] ParticleSystem _deadParticle;          // 사망 파티클
    [SerializeField] HUD _hud;                              // HUD 참조
    [SerializeField] Vector3 _moveVector;                   // 방향벡터
    

    Animator _playerAnim;                                   // 플레이어 Animator              
    Rigidbody _playerRb;                                    // 플레이어 Rigidbody
    BoxCollider _playerCollider;                            // 플레이어 BoxCollider
    Coroutine _beforeRestartRunCoroutine;                   // Restart 코루틴 동작을 위한 이전 코루틴 참조변수

    bool _isJump;                                           // 점프 코루틴 실행중인가 
    bool _isKnockBacked;                                    // 넉백 코루틴 실행중인가
    bool _isSliding;                                        // 슬라이딩 코루틴 실행중인가
    int _health;                                            // 현재 체력
    float _maxSpeed;                                        // 초기 속도
    float _speed;                                           // 플레이어의 이동속도

    public bool IsDead;                                     // 사망했는가
    
    const float JUMP_FORCE = 8f;                            // 점프 세기
    const float KNOCK_BACK_FORCE = 10f;                     // 넉백 세기
    const float START_SPEED = 5f;                           // 최초 속도
    const float RUN_RESTART_DELAY = 1f;                     // 충돌후 원래 속도로 돌아가는데 걸리는 시간


    //LifeCycles

    private void Awake()
    {

        // 필드 초기화
        _maxSpeed = START_SPEED;
        _speed = _maxSpeed;
        
    }

    void Start()
    {
        _playerCollider = GetComponent<BoxCollider>();
        _playerAnim = GetComponent<Animator>();
        _playerRb = GetComponent<Rigidbody>();
        _moveVector = Vector3.forward * _speed;
        _health = 3;
        //_controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (IsDead)
            return;

        // 카메라 컨트롤러를 이용해 플레이어 움직임 전에 카메라 연출 진행

        if (Time.timeSinceLevelLoad < CameraController._cameraAnimateDuration)
        {
            //_controller.Move(Vector3.forward * _speed * Time.deltaTime);
            //transform.position += Vector3.forward * _speed * Time.deltaTime;

            return;
        }

        if (transform.position.y > 0.5f)
        {
            _playerAnim.SetBool("isJump", true);
            _playerAnim.SetBool("isRun", false);
            _isJump = true;
        }
        else
        {
            _playerAnim.SetBool("isJump", false);
            _playerAnim.SetBool("isRun", true);
            _isJump = false;
        }


        if (transform.position.x > 3)
        {
            transform.position = new Vector3(3, transform.position.y, transform.position.z);
        }
        if (transform.position.x < -3)
        {
            transform.position = new Vector3(-3, transform.position.y, transform.position.z);
        }

        if (transform.position.y < -10)
        {
            OnDeath();
        }


        #region legacy
        // Input.GetAxisRaw() 
        // 키보드나 조이스틱의 입력 값 반환. 일정한 속도로 이동하는 기능 구현 시 -1, 0, 1 을 반환
        // Input.GetAxis() 
        // 키보드나 조이스틱의 입력 값 반환.일정한 속도로 이동하는 기능 구현 시 -1 ~ 1 을 반환

        //if (_controller.isGrounded)
        //{
        //    _vertical_velocity = 0.0f;

        //}
        //else
        //{
        //    _vertical_velocity -= _gravity * Time.deltaTime;
        //}


        //_moveVector.x = Input.GetAxisRaw("Horizontal") * _speed;
        //_moveVector.y = _vertical_velocity;
        //_moveVector.z = _speed;

        // 설정한 방향대로 이동
        //_controller.Move(_moveVector * Time.deltaTime);
        #endregion


        transform.position += _moveVector * Time.deltaTime;

    }

   

    //Coroutines
    
    IEnumerator C_RestartRun()
    {
        float alphaValue = 0f;
        alphaValue = Mathf.Clamp01(alphaValue);

        while (_speed < GetMaxSpeed())
        {
            alphaValue += (Time.deltaTime / RUN_RESTART_DELAY);
            _speed = Mathf.Lerp(0, GetMaxSpeed(), alphaValue);
            yield return null;
        }
        _speed = GetMaxSpeed();
    }

    IEnumerator C_KnockBack()
    {
        _hitParticle.Play();
        _isKnockBacked = true;
        _playerAnim.SetBool("isDamaged", true);
        _speed = 0f;
        _playerRb.AddForce(Vector3.back * KNOCK_BACK_FORCE, ForceMode.Impulse);
        yield return new WaitForSeconds(0.5f);
        _playerAnim.SetBool("isDamaged", false);
        yield return new WaitForSeconds(1.5f);
        _isKnockBacked = false;

    }

    IEnumerator C_Sliding()
    {
        _isSliding = true;

        Vector3 colliderSize = _playerCollider.size;
        Vector3 colliderCenter = _playerCollider.center;

        _playerCollider.size = new Vector3(_playerCollider.size.x, _playerCollider.size.y / 2, _playerCollider.size.z);
        _playerCollider.center = new Vector3(_playerCollider.center.x, _playerCollider.center.y / 2, _playerCollider.center.z);

        _playerAnim.SetBool("isSlide", true);
        yield return new WaitForSeconds(0.7f);

        _playerCollider.size = colliderSize;
        _playerCollider.center = colliderCenter;

        _playerAnim.SetBool("isSlide", false);
        _isSliding = false;
    }

    IEnumerator C_DeathDelay()
    {
        _deadParticle.Play();
        yield return new WaitForSeconds(2f);
        GameManager.Instance.ScoreManager.SettleScore();
    }


    // PublicMethods

    public void SpeedUp()
    {
        _maxSpeed += 0.35f;
    }

    public float GetSpeed()
    {
        return _speed;
    }

    // PrivateMethods

    private float GetMaxSpeed() 
    {
        return _maxSpeed + GameManager.Instance.ScoreManager.level - 1;
    }

    private void GetDamage()
    {
        
        _health--;
        _hud.GetDamage();

        if (_health <= 0)
        {
            OnDeath();
        }
        StartCoroutine(C_KnockBack());
    }

    private void OnDeath()
    {
        IsDead = true;
        _playerAnim.SetBool("isDead", true);
        StartCoroutine(C_DeathDelay());
    }


    // CollisonActions

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle")&& _isKnockBacked ==false)
        {
            _speed = 0f;
            GetDamage();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            if(_beforeRestartRunCoroutine != null)
            {
                StopCoroutine(_beforeRestartRunCoroutine);
            }
            _beforeRestartRunCoroutine = StartCoroutine(C_RestartRun());
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Heart"))
        {
            _hud.GetHeart();
            if (_health < 3)
            {
                _health++;
            }

            other.gameObject.SetActive(false);

        }

        if (other.CompareTag("Item"))
        {
            GameManager.Instance.ScoreManager.GetScore();
            other.gameObject.SetActive(false);
        }
    }



    //InputActions

    /// <summary>
    /// InputSystem 의 Button 형식으로 동작. X 키 입력 시, 슬라이딩 코루틴 호출
    /// </summary>
    public void OnSlide(InputAction.CallbackContext context)
    {
        if(context.performed && _isJump == false && _isSliding ==false)
        {
            StartCoroutine(C_Sliding());
        }
    }

    /// <summary>
    /// InputSystem 의 Button 형식으로 동작. space 키 입력 시, 점프 코루틴 호출
    /// </summary>
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed&& _isJump == false)
        {
            _playerRb.AddForce(Vector3.up * JUMP_FORCE, ForceMode.Impulse);
        }
    }

    /// <summary>
    /// InputSystem 의 Value 형식으로 동작. 좌우 방향키 또는  A, S키 입력 시, 좌 우 이동.
    /// </summary>
    /// <param name="context"> Vector2 Value Read 가능 </param>
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        
        if(input != null && _isJump == false)
        {
            _moveVector = new Vector3(input.x*GetMaxSpeed(), 0f, _speed);
        }
    }
}
