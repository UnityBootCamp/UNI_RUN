using UnityEngine;
using System.Collections.Generic;
using Random = System.Random;


// 1. Ÿ�� ����
// 2. �÷��̾� �̵�
// 3. �÷��̾ �ǳʰ� Ÿ�� ����
// 4. ���忡 �����ϴ� Ÿ�� ���� �����ϰ� ����

public class TileManager : MonoBehaviour
{
    public GameObject[] tilePrefabs;                    // ����� Ÿ��
    [SerializeField] GameObject _bonusHealth;           // ü������ ������ ������
    [SerializeField] PlayerController _playerController;


    Queue<GameObject> _tiles;                           // Ÿ�� ����Ʈ
    Queue<GameObject> _hearts;                          // ü������ ������ ����Ʈ

    Transform _playerTransform;                         // �÷��̾� ��ġ
    float _spawnZ = 8.0f;                               // Spawn Z Pos
    float _tileLength = 6.0f;                           // Ÿ���� ����
    float _passZone = 15.0f;                            // Ÿ�� ���� �Ÿ�
    int _preTileIndex;                                  // ������ ��ġ�Ǿ��� Ÿ���� ��ȣ

    const int TILE_ON_SCREEN = 7;                       // ȭ�鿡 ��ġ�� Ÿ�� ����

   
    void Start()
    {
        _preTileIndex = 1;
        _tiles = new Queue<GameObject>();
        _hearts = new Queue<GameObject>();
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        // ���ʿ� ���� �� ���� ��ŭ Ÿ�� ����
        for (int i = 0; i< TILE_ON_SCREEN; i++)
        {
            Spawn();
        }
    }
   
    void Update()
    {
        // �÷��̾ ������¶�� ���� x
        if (_playerController.IsDead) 
            return;

        // ���� �Ÿ� �̵��ϸ� Ÿ�� �ϳ� �����ϰ�, Ÿ�� �ϳ� ����
        if (_playerTransform.position.z - _passZone > (_spawnZ - TILE_ON_SCREEN * _tileLength))
        {
            Spawn();
            Release();
        }
    }

  

    private void Spawn()
    { 
        // Ÿ�� ����
        GameObject tile = Instantiate(ChooseRandomPlatform(Mathf.Min(GameManager.Instance.ScoreManager.level,5)));
        tile.transform.SetParent(transform);
        tile.transform.position = new Vector3(0, -0.5f, _spawnZ);

        // ü�� ���� �������� ������ Ÿ���� 0�� �Ǵ� 1���� ������ ������
        if(_preTileIndex==0 || _preTileIndex == 1)
        {
            GenerateRandomHeart(tile);
        }

        _spawnZ += _tileLength;         // ������ ������ Ÿ���� z ��ǥ�� Ÿ���� ���̸�ŭ ����

        _tiles.Enqueue(tile);           // queue�� ������ Ÿ���� ���� ����
    }


    /// <summary>
    /// ������ Ȯ���� Ÿ�Ͽ� ü������ ������ ����
    /// ����Ȯ�� : 2 * level
    /// </summary>
    /// <param name="tile"> ü������ �������� ������ Ÿ�� </param>
    private void GenerateRandomHeart(GameObject tile)
    {
        

        Random random = new Random();
        int checkRandom = random.Next(0, 100);  // 0 ~ 99
        if(checkRandom < 2 * GameManager.Instance.ScoreManager.level)   
        { 
            GameObject heart = Instantiate(_bonusHealth);
            
            // ü������ �������� ������ġ�� Ÿ���� �߽ɿ��� x���� -1 ~ +1, y���� ������, z���� tile�� z ���� ������ �����Ѵ�.
            heart.transform.position 
                = tile.transform.position +  new Vector3( random.Next(-1,2),  4.5f , random.Next(-(int)(_tileLength/2), (int)(_tileLength/2)));

           
            heart.transform.SetParent(transform);    
            _hearts.Enqueue(heart);             // queue�� ������ ü������ �������� ���� ����
        }

    }

    /// <summary>
    /// �׿��� �����ִ� Ÿ�ϰ� ü������ ������ ����
    /// </summary>
    private void Release()
    {
        

        if (_hearts.Count != 0)
        {
            Transform tile = _tiles.Peek().transform;
            Transform heart = _hearts.Peek().transform;

            // ü������ �������� �̹��� ������� Ÿ���� ���� �ִٸ�
            if (heart.position.z> tile.position.z - _tileLength/2 && heart.position.z < tile.position.z + _tileLength / 2)
            {
                // ť���� ������ ������Ʈ�� �ı�
                Destroy(_hearts.Dequeue());
            }
        }

        // ť���� ������ ������Ʈ�� �ı�
        Destroy(_tiles.Dequeue());
    }

    // �̹��� ������ Ÿ���� ������ �ε����� ����
    private GameObject ChooseRandomPlatform(int tileRange)
    {
        

        Random random = new Random();
        int tileIndex = 0;

        // ������ �´� Ÿ�� �������� ������ �ε����� ����
        tileIndex = random.Next(0, tileRange);

        Debug.Log(tileRange);

        // ���� Ÿ�ϰ� ���ٸ�
        if(tileIndex == _preTileIndex)
        {
            // ���� ��ȣ�� Ÿ�Ϸ� ����
            tileIndex = (tileIndex + 1) % tileRange;
        }
        
        // ������ �����ߴ� Ÿ���� �ε����� �����صδ� ���� ����
        _preTileIndex = tileIndex;

        // �����ؾ��� Ÿ���� ��ȯ
        return tilePrefabs[tileIndex];

        
    }
}
