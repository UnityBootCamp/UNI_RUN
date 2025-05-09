using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;



public class PlayerController : MonoBehaviour
{
    [SerializeField] ParticleSystem _hitParticle;           // �ǰ� ��ƼŬ
    [SerializeField] ParticleSystem _deadParticle;          // ��� ��ƼŬ
    [SerializeField] HUD _hud;                              // HUD ����
    [SerializeField] Vector3 _moveVector;                   // ���⺤��
    

    Animator _playerAnim;                                   // �÷��̾� Animator              
    Rigidbody _playerRb;                                    // �÷��̾� Rigidbody
    BoxCollider _playerCollider;                            // �÷��̾� BoxCollider
    Coroutine _beforeRestartRunCoroutine;                   // Restart �ڷ�ƾ ������ ���� ���� �ڷ�ƾ ��������

    bool _isJump;                                           // ���� �ڷ�ƾ �������ΰ� 
    bool _isKnockBacked;                                    // �˹� �ڷ�ƾ �������ΰ�
    bool _isSliding;                                        // �����̵� �ڷ�ƾ �������ΰ�
    int _health;                                            // ���� ü��
    float _maxSpeed;                                        // �÷��̾� �ִ� �ӵ�
    float _speed;                                           // �÷��̾��� �̵��ӵ�

    public bool IsDead;                                     // ����ߴ°�
    
    const float JUMP_FORCE = 8f;                            // ���� ����
    const float KNOCK_BACK_FORCE = 10f;                     // �˹� ����
    const float START_SPEED = 5f;                           // ���� �ӵ�
    const float RUN_RESTART_DELAY = 1f;                     // �浹�� ���� �ӵ��� ���ư��µ� �ɸ��� �ð�


    //LifeCycles

    private void Awake()
    {

        // �ʵ� �ʱ�ȭ
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

        // ī�޶� ��Ʈ�ѷ��� �̿��� �÷��̾� ������ ���� ī�޶� ���� ����
        if (Time.timeSinceLevelLoad < CameraController._cameraAnimateDuration)
        {
            return;
        }

        // �÷��̾��� y�� ��ǥ�� Ȯ���Ͽ� ���� �� �������� Ȯ��
        if (transform.position.y > 0.005f)
        {
            JumpToggle(true);
        }
        else
        {
            JumpToggle(false);
        }

        // �÷��̾��� x�� ��ǥ �����Ͽ� ���� ����
        if (transform.position.x > 3)
        {
            transform.position = new Vector3(3, transform.position.y, transform.position.z);
        }
        if (transform.position.x < -3)
        {
            transform.position = new Vector3(-3, transform.position.y, transform.position.z);
        }

        // �����ϸ� �߻�.
        // �÷��� ���� �������� �ʵ��� ������ �Ͽ�����, �������ڸ��� �ٱ����� �������� ��쿡 �۵��ϵ��� ���ܵ�
        if (transform.position.y < -10)
        {
            OnDeath();
        }


        #region legacy
        // Input.GetAxisRaw() 
        // Ű���峪 ���̽�ƽ�� �Է� �� ��ȯ. ������ �ӵ��� �̵��ϴ� ��� ���� �� -1, 0, 1 �� ��ȯ
        // Input.GetAxis() 
        // Ű���峪 ���̽�ƽ�� �Է� �� ��ȯ.������ �ӵ��� �̵��ϴ� ��� ���� �� -1 ~ 1 �� ��ȯ

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

        // ������ ������ �̵�
        //_controller.Move(_moveVector * Time.deltaTime);
        #endregion


        // ���� MoveVector �������� �̵�
        transform.position += _moveVector * Time.deltaTime;
    }


    //Coroutines
    
    // �˹� �� �ٽ� �ӵ��� ��ã�� �ڷ�ƾ
    IEnumerator C_RestartRun()
    {
        float alphaValue = 0f;                      // Lerp �� ���� alpha value
        alphaValue = Mathf.Clamp01(alphaValue);     // alpha value �̹Ƿ�, 0~1�� �� ����

        while (_speed < _maxSpeed)
        {
            alphaValue += (Time.deltaTime / RUN_RESTART_DELAY); // RUN_RESTART_DELAY ��ŭ�� �ð��� �帣�� alpha value �� 1�� ��
            _speed = Mathf.Lerp(0, _maxSpeed, alphaValue);      // Lerp
            yield return null;
        }
        _speed = _maxSpeed;                                     // �ӵ� ���� ������ �߻��� ��� ����
    }

    // ��ֹ��� �浹�� �˹��� �߻���Ű�� �ڷ�ƾ
    IEnumerator C_KnockBack()
    {
        _isKnockBacked = true;                                                  // �ش� �ڷ�ƾ�� ����Ǵ� ���� �ٸ� �˹� �ڷ�ƾ�� ��ϵ��� �ʵ����ϴ� bool
        _hitParticle.Play();
        _playerAnim.SetBool("isDamaged", true);         
        _speed = 0f;                                                            // �ش� �ӵ��� C_RestartRun���� ���󺹱���
        _playerRb.AddForce(Vector3.back * KNOCK_BACK_FORCE, ForceMode.Impulse); // ������ ����� ��ŭ �÷��̾��� ���� �������� �������� ���� ����
        yield return new WaitForSeconds(0.5f);                                  // �ִϸ��̼� ����� ���� ������
        _playerAnim.SetBool("isDamaged", false);            
        yield return new WaitForSeconds(1.5f);                                  // ���������� �߻����� �ʵ��� �ǰݿ� ��Ÿ���� ��
        _isKnockBacked = false;                                                 // �ش� �ڷ�ƾ�� �������� �˸�. �� ���� ���ķ� �÷��̾�� �ٽ� �ǰݴ��� �� ����

    }

    // �����̵�
    IEnumerator C_Sliding()
    {
        _isSliding = true;                                              // �����̵� ���۵��� �˸�

        Vector3 colliderSize = _playerCollider.size;                    //�÷��̾��� ���� collider ũ�� ����
        Vector3 colliderCenter = _playerCollider.center;                //�÷��̾��� ���� collider �߽� ��ǥ ����

        // �÷��̾��� collider ũ��� �߽���ǥ�� y���� 2�� ������ ����
        _playerCollider.size = new Vector3(_playerCollider.size.x, _playerCollider.size.y / 2, _playerCollider.size.z);
        _playerCollider.center = new Vector3(_playerCollider.center.x, _playerCollider.center.y / 2, _playerCollider.center.z);

        _playerAnim.SetBool("isSlide", true);
        yield return new WaitForSeconds(0.7f);                         // �ִϸ��̼� ����� ���� ������

        // �����̵� ���� ���� ������ Collider�� �� ����
        _playerCollider.size = colliderSize;                           
        _playerCollider.center = colliderCenter;                       

        _playerAnim.SetBool("isSlide", false);
        _isSliding = false;                                            // �����̵� �������� �˸�
    }

    // �����, ��� �ִϸ��̼� ����Ǵ� ���� �� �� �ֵ��� ������
    IEnumerator C_DeathDelay()
    {
        _deadParticle.Play();
        yield return new WaitForSeconds(2f);
        GameManager.Instance.ScoreManager.SettleScore();
    }


    // PublicMethods

    // �÷��̾��� �ִ�ӵ� ����
    public void SpeedUp()
    {
        _maxSpeed += 0.5f;

        // �ӵ� ���� �ڷ�ƾ�� ����ǰ� ���� �ʴٸ� ����ӵ��� ����� �ִ�ӵ��� ����
        if(_beforeRestartRunCoroutine == null)
        {
            _speed = _maxSpeed;
        }
    }

    // �÷��̾��� ���� �ӵ� ��ȯ
    public float GetSpeed()
    {
        return _speed;
    }

    // PrivateMethods

    // �÷��̾��� ���� ���¸� ���
    private void JumpToggle(bool isJump)
    {
        _playerAnim.SetBool("isJump", isJump);
        _playerAnim.SetBool("isRun", !isJump);
        _isJump = isJump;
    }

    // �÷��̾ �������� ����
    private void GetDamage()
    {
        _health--;
        _hud.GetDamage();                  // HUD �� ��Ʈ �̹��� 1�� �����ϴ� �Լ� ȣ��

        if (_health <= 0)
        {
            OnDeath();                     // ü�� <0 �̸� ��� �޼��� ȣ��
        }
        StartCoroutine(C_KnockBack());     // �˹�
    }

    // ���
    private void OnDeath()
    {
        IsDead = true;
        _playerAnim.SetBool("isDead", true);
        StartCoroutine(C_DeathDelay());      // ��� ������
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
            // ������ ��ϵ� C_RestartRun �� ������ ����
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
            _hud.GetHeart();                        //HUD �� ��Ʈ �ϳ� ������Ű�� �޼��� ȣ��

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
    /// InputSystem �� Button �������� ����. X Ű �Է� ��, �����̵� �ڷ�ƾ ȣ��
    /// </summary>
    public void OnSlide(InputAction.CallbackContext context)
    {
        if(context.performed && _isJump == false && _isSliding ==false)
        {
            StartCoroutine(C_Sliding());
        }
    }

    /// <summary>
    /// InputSystem �� Button �������� ����. space Ű �Է� ��, ���� �ڷ�ƾ ȣ��
    /// </summary>
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed&& _isJump == false)
        {
            _playerRb.AddForce(Vector3.up * JUMP_FORCE, ForceMode.Impulse);
        }
    }

    /// <summary>
    /// InputSystem �� Value �������� ����. �¿� ����Ű �Ǵ�  A, SŰ �Է� ��, �� �� �̵�.
    /// </summary>
    /// <param name="context"> Vector2 Value Read ���� </param>
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        
        if(input != null && _isJump == false)
        {
            _moveVector = new Vector3(input.x*_maxSpeed, 0f, _speed);
        }
    }
}
