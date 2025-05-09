using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;



public class PlayerController : MonoBehaviour
{
    [SerializeField] ParticleSystem _hitParticle;           // 피격 파티클
    [SerializeField] ParticleSystem _deadParticle;          // 사망 파티클
    [SerializeField] HUD _hud;                              // HUD 참조
    [SerializeField] Vector3 _moveVector;                   // 방향벡터
    

    Animator _playerAnim;                                   // 플레이어 Animator              
    Rigidbody _playerRb;                                    // 플레이어 Rigidbody
    BoxCollider _playerCollider;                            // 플레이어 BoxCollider
    Coroutine _beforeRestartRunCoroutine;                   // Restart 코루틴 동작을 위한 이전 코루틴 참조변수
   
    private bool isInvincible = false;
    private Renderer[] renderers;

    bool _isJump;                                           // 점프 코루틴 실행중인가 
    bool _isKnockBacked;                                    // 넉백 코루틴 실행중인가
    bool _isSliding;                                        // 슬라이딩 코루틴 실행중인가
    int _health;                                            // 현재 체력
    float _maxSpeed;                                        // 플레이어 최대 속도
    float _speed;                                           // 플레이어의 이동속도

    public bool IsDead;                                     // 사망했는가
    
    const float JUMP_FORCE = 20f;                            // 점프 높이
    const float JUMP_Distance = 4f;                           // 점프 거리
    const float KNOCK_BACK_FORCE = 7f;                     // 넉백 세기
    const float START_SPEED = 5f;                           // 최초 속도
    const float RUN_RESTART_DELAY = 0.5f;                     // 충돌후 원래 속도로 돌아가는데 걸리는 시간


    //LifeCycles

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();

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
    }

    void Update()
    {
        if (IsDead)
            return;

        // 카메라 컨트롤러를 이용해 플레이어 움직임 전에 카메라 연출 진행
        if (Time.timeSinceLevelLoad < CameraController._cameraAnimateDuration)
        {
            return;
        }

        // 플레이어의 y축 좌표를 확인하여 점프 한 상태인지 확인
        if (transform.position.y > 0.000001f)
        {
            JumpToggle(true);
        }
        else
        {
            JumpToggle(false);
        }

        // 플레이어의 x축 좌표 제한하여 낙사 방지
        if (transform.position.x > 3)
        {
            transform.position = new Vector3(3, transform.position.y, transform.position.z);
        }
        if (transform.position.x < -3)
        {
            transform.position = new Vector3(-3, transform.position.y, transform.position.z);
        }
        



        // 낙사하면 발생.
        // 플레이 도중 낙사하지 않도록 조정은 하였지만, 시작하자마자 바깥으로 떨어지는 경우에 작동하도록 남겨둠
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


        // 계산된 MoveVector 방향으로 이동
        if (_isJump)
        {

            transform.position += new Vector3(_moveVector.x,_moveVector.y , JUMP_Distance ) * Time.deltaTime;
        }
        else
        {
            transform.position += new Vector3(_moveVector.x, _moveVector.y , _speed) * Time.deltaTime;
        }


    }


    //Coroutines
    
    // 넉백 후 다시 속도를 되찾는 코루틴
    IEnumerator C_RestartRun()
    {
        float alphaValue = 0f;                      // Lerp 를 위한 alpha value
        alphaValue = Mathf.Clamp01(alphaValue);     // alpha value 이므로, 0~1로 값 제한

        while (_speed < _maxSpeed)
        {
            alphaValue += (Time.deltaTime / RUN_RESTART_DELAY); // RUN_RESTART_DELAY 만큼의 시간이 흐르면 alpha value 가 1이 됨
            _speed = Mathf.Lerp(0, _maxSpeed, alphaValue);      // Lerp
            yield return null;
        }
        _speed = _maxSpeed;                                     // 속도 값의 오차가 발생할 경우 상정
    }

    // 장애물과 충돌시 넉백을 발생시키는 코루틴
    IEnumerator C_KnockBack()
    {
        _isKnockBacked = true;                                                  // 해당 코루틴이 실행되는 동안 다른 넉백 코루틴이 등록되지 않도록하는 bool
    
        _hitParticle.Play();
        _playerAnim.SetBool("isDamaged", true);         
        _speed = 0f;                                                            // 해당 속도는 C_RestartRun에서 원상복구됨


        _playerRb.AddForce(KNOCK_BACK_FORCE * Vector3.back, ForceMode.Impulse);

        yield return new WaitForSeconds(0.3f);                                  // 애니메이션 재생을 위한 딜레이


        _playerAnim.SetBool("isDamaged", false);            
        yield return new WaitForSeconds(1.7f);                                  // 연속적으로 발생하지 않도록 피격에 쿨타임을 줌
        _isKnockBacked = false;                                                 // 해당 코루틴이 끝났음을 알림. 이 시점 이후로 플레이어는 다시 피격당할 수 있음

    }

    // 슬라이딩
    IEnumerator C_Sliding()
    {
        _isSliding = true;                                              // 슬라이딩 시작됨을 알림

        Vector3 colliderSize = _playerCollider.size;                    //플레이어의 기존 collider 크기 저장
        Vector3 colliderCenter = _playerCollider.center;                //플레이어의 기존 collider 중심 좌표 저장

        // 플레이어의 collider 크기와 중심좌표의 y값을 2로 나누어 저장
        _playerCollider.size = new Vector3(_playerCollider.size.x, _playerCollider.size.y / 2, _playerCollider.size.z);
        _playerCollider.center = new Vector3(_playerCollider.center.x, _playerCollider.center.y / 2, _playerCollider.center.z);

        _playerAnim.SetBool("isSlide", true);
        yield return new WaitForSeconds(0.7f);                         // 애니메이션 재생을 위한 딜레이

        // 슬라이딩 끝난 이후 조정한 Collider의 값 원복
        _playerCollider.size = colliderSize;                           
        _playerCollider.center = colliderCenter;                       

        _playerAnim.SetBool("isSlide", false);
        _isSliding = false;                                            // 슬라이딩 끝났음을 알림
    }

    public IEnumerator ActivateInvincibility(float duration, float blinkInterval = 0.1f)
    {
        isInvincible = true;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            ToggleRenderers(false); // 숨김
            yield return new WaitForSeconds(blinkInterval);
            ToggleRenderers(true);  // 표시
            yield return new WaitForSeconds(blinkInterval);

            elapsed += blinkInterval * 2;
        }

        ToggleRenderers(true); // 종료 후 표시 유지
        isInvincible = false;
    }

    private void ToggleRenderers(bool visible)
    {
        foreach (var renderer in renderers)
        {
            renderer.enabled = visible;
        }
    }

    // 사망시, 사망 애니메이션 재생되는 것을 볼 수 있도록 딜레이
    IEnumerator C_DeathDelay()
    {
        _deadParticle.Play();
        yield return new WaitForSeconds(2f);
        GameManager.Instance.ScoreManager.SettleScore();
    }

    IEnumerator C_Jump()
    {
        _isJump = true;

        while (_moveVector.y < JUMP_FORCE)
        {
            _moveVector += (Vector3.up) ;
            yield return null;
        }
        while (_moveVector.y > 0)
        {
            _moveVector += (Vector3.down);
            yield return null;
        }
        _isJump = false;
    }

    // PublicMethods

    // 플레이어의 최대속도 증가
    public void SpeedUp()
    {
        _maxSpeed += 0.5f;

        // 속도 복구 코루틴이 실행되고 있지 않다면 현재속도를 변경된 최대속도로 갱신
        if(_beforeRestartRunCoroutine == null)
        {
            _speed = _maxSpeed;
        }
    }

    // 플레이어의 현재 속도 반환
    public float GetSpeed()
    {
        return _speed;
    }

    // PrivateMethods

    // 플레이어의 점프 상태를 토글
    private void JumpToggle(bool isJump)
    {
        _playerAnim.SetBool("isJump", isJump);
        _playerAnim.SetBool("isRun", !isJump);
        _isJump = isJump;
    }

    // 플레이어가 데미지를 입음
    private void GetDamage()
    {
        _health--;


        _hud.GetDamage();                  // HUD 의 하트 이미지 1개 감소하는 함수 호출

        if (_health <= 0)
        {
            OnDeath();                     // 체력 <0 이면 사망 메서드 호출
        }
        else
        {
            StartCoroutine(ActivateInvincibility(2f));
            StartCoroutine(C_KnockBack());     // 넉백
        }
        
         
    }

    // 사망
    private void OnDeath()
    {
        IsDead = true;
        _playerAnim.SetBool("isDead", true);
        StartCoroutine(C_DeathDelay());      // 사망 딜레이
    }


    // CollisonActions

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle")&& _isKnockBacked ==false && IsDead ==false)
        {
            _speed = 0f;
            GetDamage();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            // 이전에 등록된 C_RestartRun 이 있으면 지움
            if (_beforeRestartRunCoroutine != null)
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
            _hud.GetHeart();                        //HUD 의 하트 하나 증가시키는 메서드 호출

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
        if (Time.timeSinceLevelLoad < CameraController._cameraAnimateDuration)
        {
            return;
        }

        if (context.performed && _isJump == false && _isSliding ==false)
        {
            StartCoroutine(C_Sliding());
        }
    }

    /// <summary>
    /// InputSystem 의 Button 형식으로 동작. space 키 입력 시, 점프 코루틴 호출
    /// </summary>
    public void OnJump(InputAction.CallbackContext context)
    {
        if (Time.timeSinceLevelLoad < CameraController._cameraAnimateDuration)
        {
            return;
        }


        if (context.performed&& _isJump == false)
        {
            //_playerRb.AddForce(Vector3.up * JUMP_FORCE, ForceMode.Impulse);
            StartCoroutine(C_Jump());
        }
    }

 

    /// <summary>
    /// InputSystem 의 Value 형식으로 동작. 좌우 방향키 또는  A, S키 입력 시, 좌 우 이동.
    /// </summary>
    /// <param name="context"> Vector2 Value Read 가능 </param>
    public void OnMove(InputAction.CallbackContext context)
    {
        if (Time.timeSinceLevelLoad < CameraController._cameraAnimateDuration)
        {
            return;
        }

        Vector2 input = context.ReadValue<Vector2>();
        
        if(input != null && _isJump == false)
        {
            _moveVector = new Vector3(input.x*_maxSpeed, _moveVector.y, _speed);
        }
    }
}
