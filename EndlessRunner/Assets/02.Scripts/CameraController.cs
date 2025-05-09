using UnityEngine;

public class CameraController : MonoBehaviour
{
    Transform _target;  // 플레이어 위치
    Vector3 _offset;    // 카메라와 플레이어 간의 거리 간격
    Vector3 _moveVector; // 카메라가 매 프레임 이동할 위치
    
    float _transition = 0.0f;   // 보간 값
    public static float _cameraAnimateDuration = 3.0f; // 카메라를 이용한 애니메이션 연출 지속 시간
    public Vector3 animateOffset = new Vector3(0, 5, 5); // 애니메이션을 위한 시작 오프셋

    void Start()
    {
        _target = GameObject.FindGameObjectWithTag("Player").transform;
        _offset = transform.position - _target.position;

    }

    // Update is called once per frame
    void Update()
    {
        _moveVector = _target.position + _offset;
        _moveVector.x = 0f;                                 // 카메라 x축 고정

        // Mathf.Clamp 특정 범위 내에서 값을 제한하는 메서드
        // Mathf.Clamp01 범위를 0 ~ 1로 제한하는 메서드
        _moveVector.y = Mathf.Clamp(_moveVector.y, 3, 5);   // 카메라 y축 고정(3~5)

        if (_transition > 1.0f)
        {
            transform.position = _moveVector;
        }
        else
        {
            // _moveVector + animateOffset 에서 _moveVector 까지 선형보간
            transform.position = Vector3.Lerp(_moveVector + animateOffset, _moveVector, _transition);

            _transition += Time.deltaTime/_cameraAnimateDuration;

            transform.LookAt(_target.position + Vector3.up);
        }
    }
}
