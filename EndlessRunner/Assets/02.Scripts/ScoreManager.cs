using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    #region
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
    #endregion
        
    [SerializeField] PlayerController playerController;
    [SerializeField] TextMeshProUGUI scoreText;             // 이번에 기록한 점수
    [SerializeField] TextMeshProUGUI levelText;             // 레벨
    [SerializeField] TextMeshProUGUI HighScoreText;         // 최고 점수
    [SerializeField] TextMeshProUGUI RunDistanceText;       // 현재 이동거리

    public int level;

    private float _score;
    private float _distance;
    private int _levelPerScore;

    const int POINT_PER_ITEM = 10;
    const int MAX_LEVEL = 10;


    private void Awake()
    {
        // 필드 초기화
        level = 1;
        _score = 0.0f;
        _distance = 0.0f;
        _levelPerScore = 50;

        // PlayerPrefs 를 통해 최고점수 관리
        if (PlayerPrefs.HasKey("HIGH_SCORE") == false)
        {
            PlayerPrefs.SetInt("HIGH_SCORE", 0);
        }

        HighScoreText.text = $"High Score : {PlayerPrefs.GetInt("HIGH_SCORE")}";
        SetText();
    }

    void Update()
    {
        // 플레이어 사망시 실행 x
        if (playerController.IsDead)
            return;

        // 스타트 애니메이션 진행중이면 실행 x
        if (Time.timeSinceLevelLoad < CameraController._cameraAnimateDuration)
            return;

        // 지정된 스코어 넘기면 레벨업 메서드 호출
        if (_distance >= _levelPerScore)
        {
            LevelUp();
        }

        // 이동거리 = Time.deltaTime * 플레이어의 현재 속도
        _distance += Time.deltaTime*playerController.GetSpeed();

        // 텍스트 갱신
        SetText();

    }

    private void LevelUp()
    {
        // 이미 최대레벨이면 실행 x
        if(level >= MAX_LEVEL)
        {
            return;
        }

        level++;
        _levelPerScore *= 2;            // 레벨업에 필요한 점수 2배
        playerController.SpeedUp();     // 플레이어의 최대속도 증가
    }

    private void SetText()
    {
        // 현재 점수가 최고 점수를 넘었다면 최고점수도 함께 갱신
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
        _score += POINT_PER_ITEM;       // 아이템 배당 점수 만큼 점수 증가
    }

    public void SettleScore()
    {
        GameManager.Instance.DeadManager.SetScoreText(_score, _distance);   // 사망시 결과창에 점수가 표시될 수 있도록 값 전달
    }
   
}
