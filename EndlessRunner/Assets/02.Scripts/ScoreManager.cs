using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreManager : MonoBehaviour
{
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

    [SerializeField] DeadManager _deadManager;

   [ SerializeField] PlayerController playerController;
   [ SerializeField] TextMeshProUGUI scoreText;
   [ SerializeField] TextMeshProUGUI levelText;
   [ SerializeField] TextMeshProUGUI HighScoreText;
   [ SerializeField] TextMeshProUGUI RunDistanceText;
    public int level = 1;

    private int _maxLevel = 10;
    private int _levelPerScore = 200;
    private float _score = 0.0f;
    private float _distance = 0.0f;

    const int POINT_PER_ITEM = 10;



    private void Awake()
    {
        if (PlayerPrefs.HasKey("HIGH_SCORE") == false)
        {
            Debug.Log("High_Score Ű ����");
            PlayerPrefs.SetInt("HIGH_SCORE", 0);
        }
        else
        {
            Debug.Log("High_Score Ű ����");
        }

        HighScoreText.text = $"High Score : {PlayerPrefs.GetInt("HIGH_SCORE")}";
        SetText();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerController.IsDead)
            return;

        if (Time.timeSinceLevelLoad < CameraController._cameraAnimateDuration)
            return;

        if (_score >= _levelPerScore)
        {
            LevelUp();
        }

        _distance += Time.deltaTime*playerController.GetSpeed();

       

        SetText();

    }

    private void LevelUp()
    {
        if(level == _maxLevel)
        {
            return;
        }

        _levelPerScore *= 2;
        level++;
        playerController.SpeedUp();
    }

    private void SetText()
    {
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
        _score += POINT_PER_ITEM;
    }

    public void SettleScore()
    {
        _deadManager.SetScoreText(_score, _distance);
    }
   
}
