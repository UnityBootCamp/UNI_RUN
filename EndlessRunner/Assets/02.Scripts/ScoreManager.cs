using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    #region
    // ���� ��� ����
    // 1. �ΰ��� ������ �����ϰ� ����� ���, �ʵ�� �����
    // 2. ������ ���� ���� ���Ŀ��� �����ؾ� �� ��� ������ ����Ѵ�.
    //    2-1 PlayerPrefs : ����Ƽ�� �����͸� ������Ʈ���� ������ �� ���
    //                      ����, �Ǽ� ,���� ������ ������ ������ ���� ����
    //    2-2 Json        : JavaScript Object Notation 
    //                      ��ü, �迭 ���ڿ�, Bool, ���� ���� ������ ���� ����
    //                      �ַ� ���ӿ��� ��ųʸ��� ����Ʈ ���� ���ؼ� ������ ���̺�, �κ��丮, ���� ���̺� ����
    //                      ������ �� ���
    //    2-3 Firebase    : �����ͺ��̽����� ������ ���� �����͸� ����
    //    2-4 ScriptableObject : ����Ƽ ���ο� Asset ���·ν� �����͸� �����ؼ� ���
    //                           �ΰ��� �����͸� ������ �� ���� ���� ��
    //    2-5 CSV         : ���� ���Ͽ� �ʿ��� �����͵��� �����صΰ�, C# ��ũ��Ʈ�� ���� �ش� ���� ���ͼ� ����
    //                      �ַ� �� ����, ��ũ��Ʈ ��ȭ ȣ��, �⺻���� ������
    #endregion
        
    [SerializeField] PlayerController playerController;
    [SerializeField] TextMeshProUGUI scoreText;             // �̹��� ����� ����
    [SerializeField] TextMeshProUGUI levelText;             // ����
    [SerializeField] TextMeshProUGUI HighScoreText;         // �ְ� ����
    [SerializeField] TextMeshProUGUI RunDistanceText;       // ���� �̵��Ÿ�

    public int level;

    private float _score;
    private float _distance;
    private int _levelPerScore;

    const int POINT_PER_ITEM = 10;
    const int MAX_LEVEL = 10;


    private void Awake()
    {
        // �ʵ� �ʱ�ȭ
        level = 1;
        _score = 0.0f;
        _distance = 0.0f;
        _levelPerScore = 50;

        // PlayerPrefs �� ���� �ְ����� ����
        if (PlayerPrefs.HasKey("HIGH_SCORE") == false)
        {
            PlayerPrefs.SetInt("HIGH_SCORE", 0);
        }

        HighScoreText.text = $"High Score : {PlayerPrefs.GetInt("HIGH_SCORE")}";
        SetText();
    }

    void Update()
    {
        // �÷��̾� ����� ���� x
        if (playerController.IsDead)
            return;

        // ��ŸƮ �ִϸ��̼� �������̸� ���� x
        if (Time.timeSinceLevelLoad < CameraController._cameraAnimateDuration)
            return;

        // ������ ���ھ� �ѱ�� ������ �޼��� ȣ��
        if (_distance >= _levelPerScore)
        {
            LevelUp();
        }

        // �̵��Ÿ� = Time.deltaTime * �÷��̾��� ���� �ӵ�
        _distance += Time.deltaTime*playerController.GetSpeed();

        // �ؽ�Ʈ ����
        SetText();

    }

    private void LevelUp()
    {
        // �̹� �ִ뷹���̸� ���� x
        if(level >= MAX_LEVEL)
        {
            return;
        }

        level++;
        _levelPerScore *= 2;            // �������� �ʿ��� ���� 2��
        playerController.SpeedUp();     // �÷��̾��� �ִ�ӵ� ����
    }

    private void SetText()
    {
        // ���� ������ �ְ� ������ �Ѿ��ٸ� �ְ������� �Բ� ����
        if (_score > PlayerPrefs.GetInt("HIGH_SCORE"))
        {

            HighScoreText.text = "High Score : " + ((int)_score).ToString();
        }

        scoreText.text = ((int)_score).ToString();
        RunDistanceText.text = ((int)_distance).ToString() + "M";
        levelText.text = "LEVEL : " + level.ToString();
    }
    
    public void GetScore()
    {
        _score += POINT_PER_ITEM;       // ������ ��� ���� ��ŭ ���� ����
    }

    public void SettleScore()
    {
        GameManager.Instance.DeadManager.SetScoreText(_score, _distance);   // ����� ���â�� ������ ǥ�õ� �� �ֵ��� �� ����
    }
   
}
