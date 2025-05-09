using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{

    [SerializeField] GameObject _healthBar; // 하트 이미지들을 담고 있는 빈 게임 오브젝트. Grid Layout 보유
    [SerializeField] GameObject _health;    // 하트 이미지 프리팹

    Queue<GameObject> _hearts;              // 활성화된 하트 이미지 담아두는 큐
    Queue<GameObject> _damagedHearts;       // 비활성화된 하트 이미지 담아두는 큐

    const int MAX_HEALTH = 3;               // 최대 체력

    private void Awake()
    {
        _hearts = new Queue<GameObject>(MAX_HEALTH);
        _damagedHearts = new Queue<GameObject>(MAX_HEALTH);

        // 최초에는 정해진 갯수만큼 체력 초기화
        for (int i = 0; i < MAX_HEALTH; i++)
        {
            _hearts.Enqueue(Instantiate(_health, _healthBar.transform));
        }
    }

    public void GetDamage()
    {
        GameObject damageHeart = _hearts.Dequeue();
        _damagedHearts.Enqueue(damageHeart);            // 활성화 하트 큐에서 하나 꺼내서 비활성화 하트 큐에 담고
        damageHeart.SetActive(false);                   // 비활성화 시킴

    }

    public void GetHeart()
    {
        if (_damagedHearts.Count ==0)
            return;
        GameObject heart = _damagedHearts.Dequeue();
        _hearts.Enqueue(heart);                        // 비활성화 하트 큐에서 하나 꺼내서 활성화 하트 큐에 담고
        heart.SetActive(true);                         // 활성화 시킴
    }

    
}