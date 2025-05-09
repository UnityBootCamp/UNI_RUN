using UnityEngine;

public class CameraController : MonoBehaviour
{
    Transform _target;  // �÷��̾� ��ġ
    Vector3 _offset;    // ī�޶�� �÷��̾� ���� �Ÿ� ����
    Vector3 _moveVector; // ī�޶� �� ������ �̵��� ��ġ
    
    float _transition = 0.0f;   // ���� ��
    public static float _cameraAnimateDuration = 3.0f; // ī�޶� �̿��� �ִϸ��̼� ���� ���� �ð�
    public Vector3 animateOffset = new Vector3(0, 5, 5); // �ִϸ��̼��� ���� ���� ������

    void Start()
    {
        _target = GameObject.FindGameObjectWithTag("Player").transform;
        _offset = transform.position - _target.position;

    }

    // Update is called once per frame
    void Update()
    {
        _moveVector = _target.position + _offset;
        _moveVector.x = 0f;                                 // ī�޶� x�� ����

        // Mathf.Clamp Ư�� ���� ������ ���� �����ϴ� �޼���
        // Mathf.Clamp01 ������ 0 ~ 1�� �����ϴ� �޼���
        _moveVector.y = Mathf.Clamp(_moveVector.y, 3, 5);   // ī�޶� y�� ����(3~5)

        if (_transition > 1.0f)
        {
            transform.position = _moveVector;
        }
        else
        {
            // _moveVector + animateOffset ���� _moveVector ���� ��������
            transform.position = Vector3.Lerp(_moveVector + animateOffset, _moveVector, _transition);

            _transition += Time.deltaTime/_cameraAnimateDuration;

            transform.LookAt(_target.position + Vector3.up);
        }
    }
}
