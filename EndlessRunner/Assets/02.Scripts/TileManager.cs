using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;
using Random = System.Random;
using Unity.VisualScripting;


// 1. Ÿ�� ����
// 2. �÷��̾� �̵�
// 3. �÷��̾ �ǳʰ� Ÿ�� ����
// 4. ���忡 �����ϴ� Ÿ�� ���� �����ϰ� ����

public class TileManager : MonoBehaviour
{
    public GameObject[] tilePrefabs;            // ����� Ÿ��
    [SerializeField] GameObject _bonusHealth;
    [SerializeField] PlayerController _playerController;


    Queue<GameObject> _tiles;                    // Ÿ�� ����Ʈ
    Queue<GameObject> _hearts;

    Transform _playerTransform;                 // �÷��̾� ��ġ
    float _spawnZ = 8.0f;                       // Spawn Z Pos
    float _tileLength = 6.0f;                   // Ÿ���� ����
    float _passZone = 15.0f;                    // Ÿ�� ���� �Ÿ�
    int _tileOnScreen = 7;                      // ȭ�鿡 ��ġ�� Ÿ�� ����
    int _preTileIndex;

    void Start()
    {
        _preTileIndex = 1;
        _tiles = new Queue<GameObject>();
        _hearts = new Queue<GameObject>();
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        GameManager.Instance.ScoreManager = FindFirstObjectByType<ScoreManager>();


        for (int i = 0; i< _tileOnScreen; i++)
        {
            Spawn();
        }
    }

   

    // Update is called once per frame
    void Update()
    {
        if (_playerController.IsDead) 
            return;

        if (_playerTransform.position.z - _passZone > (_spawnZ - _tileOnScreen * _tileLength))
        {
            Spawn();
            Release();
        }
    }

  

    private void Spawn()
    {
        // Ÿ�� ����
        var go = Instantiate(ChooseRandomPlatform(Mathf.Min(GameManager.Instance.ScoreManager.level,5)));
        go.transform.SetParent(transform);
        go.transform.position = new Vector3(0, -0.5f, _spawnZ);

        if(_preTileIndex==0 || _preTileIndex == 1)
        {
            GenerateRandomHeart(go);
        }
        

        _spawnZ += _tileLength;

        _tiles.Enqueue(go);
    }

    private void GenerateRandomHeart(GameObject tile)
    {
        Random random = new Random();
        int checkRandom = random.Next(0, 99);
        if(checkRandom < 2 * GameManager.Instance.ScoreManager.level)
        { 
            GameObject heart = Instantiate(_bonusHealth);
            heart.transform.position = tile.transform.position +  new Vector3( random.Next(-1,2),  3f , random.Next(-(int)(_tileLength/2), (int)(_tileLength/2)));
            heart.transform.SetParent(transform);
            _hearts.Enqueue(heart);
        }

    }

    private void Release()
    {
        
        if (_hearts.Count != 0)
        {
            Transform tile = _tiles.Peek().transform;
            Transform heart = _hearts.Peek().transform;

            if (heart.position.z> tile.position.z - _tileLength/2 && heart.position.z < tile.position.z + _tileLength / 2)
            {
                Destroy(_hearts.Dequeue());
            }
        }
        Destroy(_tiles.Dequeue());
    }

    private GameObject ChooseRandomPlatform(int tileRange)
    {
        Random random = new Random();
        int tileIndex = 0;

        tileIndex = random.Next(0, tileRange);

        if(tileIndex == _preTileIndex)
        {
            tileIndex = (tileIndex + 1) % tileRange;
        }
        
        

        _preTileIndex = tileIndex;

        return tilePrefabs[tileIndex];

        
    }
}
