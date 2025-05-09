using UnityEngine;
using System.Collections.Generic;
using Random = System.Random;


// 1. 타일 생성
// 2. 플레이어 이동
// 3. 플레이어가 건너간 타일 제거
// 4. 월드에 존재하는 타일 개수 균일하게 유지

public class TileManager : MonoBehaviour
{
    public GameObject[] tilePrefabs;                    // 등록할 타일
    [SerializeField] GameObject _bonusHealth;           // 체력증가 아이템 프리팹
    [SerializeField] PlayerController _playerController;


    Queue<GameObject> _tiles;                           // 타일 리스트
    Queue<GameObject> _hearts;                          // 체력증가 아이템 리스트

    Transform _playerTransform;                         // 플레이어 위치
    float _spawnZ = 8.0f;                               // Spawn Z Pos
    float _tileLength = 6.0f;                           // 타일의 길이
    float _passZone = 15.0f;                            // 타일 유지 거리
    int _preTileIndex;                                  // 이전에 배치되었던 타일의 번호

    const int TILE_ON_SCREEN = 7;                       // 화면에 배치할 타일 개수

   
    void Start()
    {
        _preTileIndex = 1;
        _tiles = new Queue<GameObject>();
        _hearts = new Queue<GameObject>();
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        // 최초에 지정 된 숫자 만큼 타일 생성
        for (int i = 0; i< TILE_ON_SCREEN; i++)
        {
            Spawn();
        }
    }
   
    void Update()
    {
        // 플레이어가 사망상태라면 동작 x
        if (_playerController.IsDead) 
            return;

        // 일정 거리 이동하면 타일 하나 생성하고, 타일 하나 지움
        if (_playerTransform.position.z - _passZone > (_spawnZ - TILE_ON_SCREEN * _tileLength))
        {
            Spawn();
            Release();
        }
    }

  

    private void Spawn()
    { 
        // 타일 생성
        GameObject tile = Instantiate(ChooseRandomPlatform(Mathf.Min(GameManager.Instance.ScoreManager.level,5)));
        tile.transform.SetParent(transform);
        tile.transform.position = new Vector3(0, -0.5f, _spawnZ);

        // 체력 증가 아이템은 생성된 타일이 0번 또는 1번일 때에만 생성됨
        if(_preTileIndex==0 || _preTileIndex == 1)
        {
            GenerateRandomHeart(tile);
        }

        _spawnZ += _tileLength;         // 다음에 스폰될 타일의 z 좌표는 타일의 길이만큼 증가

        _tiles.Enqueue(tile);           // queue에 생성한 타일의 참조 전달
    }


    /// <summary>
    /// 랜덤한 확률로 타일에 체력증가 아이템 생성
    /// 생성확률 : 2 * level
    /// </summary>
    /// <param name="tile"> 체력증가 아이템을 생성할 타일 </param>
    private void GenerateRandomHeart(GameObject tile)
    {
        

        Random random = new Random();
        int checkRandom = random.Next(0, 100);  // 0 ~ 99
        if(checkRandom < 2 * GameManager.Instance.ScoreManager.level)   
        { 
            GameObject heart = Instantiate(_bonusHealth);
            
            // 체력증가 아이템의 생성위치는 타일의 중심에서 x축은 -1 ~ +1, y축은 고정값, z축은 tile의 z 범위 안으로 제한한다.
            heart.transform.position 
                = tile.transform.position +  new Vector3( random.Next(-1,2),  4.5f , random.Next(-(int)(_tileLength/2), (int)(_tileLength/2)));

           
            heart.transform.SetParent(transform);    
            _hearts.Enqueue(heart);             // queue에 생성한 체력증가 아이템의 참조 전달
        }

    }

    /// <summary>
    /// 잉여로 남아있는 타일과 체력증가 아이템 해제
    /// </summary>
    private void Release()
    {
        

        if (_hearts.Count != 0)
        {
            Transform tile = _tiles.Peek().transform;
            Transform heart = _hearts.Peek().transform;

            // 체력증가 아이템이 이번에 지우려는 타일의 위에 있다면
            if (heart.position.z> tile.position.z - _tileLength/2 && heart.position.z < tile.position.z + _tileLength / 2)
            {
                // 큐에서 빼내고 오브젝트를 파괴
                Destroy(_hearts.Dequeue());
            }
        }

        // 큐에서 빼내고 오브젝트를 파괴
        Destroy(_tiles.Dequeue());
    }

    // 이번에 생성할 타일의 랜덤한 인덱스를 선택
    private GameObject ChooseRandomPlatform(int tileRange)
    {
        

        Random random = new Random();
        int tileIndex = 0;

        // 레벨에 맞는 타일 범위에서 랜덤한 인덱스를 뽑음
        tileIndex = random.Next(0, tileRange);

        Debug.Log(tileRange);

        // 이전 타일과 같다면
        if(tileIndex == _preTileIndex)
        {
            // 다음 번호의 타일로 변경
            tileIndex = (tileIndex + 1) % tileRange;
        }
        
        // 이전에 생성했던 타일의 인덱스를 저장해두는 변수 갱신
        _preTileIndex = tileIndex;

        // 생성해야할 타일을 반환
        return tilePrefabs[tileIndex];

        
    }
}
