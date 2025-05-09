using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreManager : MonoBehaviour
{
    // 점수 기믹 설계
    // 1. 인게임 내에서 간단하게 사용할 경우, 필드로 만든다
    // 2. 점수를 게임 종료 이후에도 저장해야 할 경우 다음을 고려한다.
    //    2-1 PlayerPrefs : 유니티의 데이터를 레지스트리에 저장할 때 사용
    //                      정수, 실수 ,문장 정도의 간단한 데이터 저장 가능
    //    2-2 Json        : JavaScript Object Notation 
    //                      객체, 배열 문자열, Bool, 숫자 등의 데이터 유형 제공
    //                      주로 게임에서 딕셔너리나 리스트 등을 통해서 아이템 테이블, 인벤토리, 몬스터 테이블 등을
    //                      구현할 때 사용
    //    2-3 Firebase    : 데이터베이스와의 연동을 통해 데이터를 관리
    //    2-4 ScriptableObject : 유니티 내부에 Asset 형태로써 데이터를 저장해서 사용
    //                           인게임 데이터를 구성할 때 가장 쉽고 편리
    //    2-5 CSV         : 엑셀 파일에 필요한 데이터들을 나열해두고, C# 스크립트를 통해 해당 값을 얻어와서 적용
    //                      주로 맵 패턴, 스크립트 대화 호출, 기본적인 데이터

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
            Debug.Log("High_Score 키 생성");
            PlayerPrefs.SetInt("HIGH_SCORE", 0);
        }
        else
        {
            Debug.Log("High_Score 키 갱신");
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
