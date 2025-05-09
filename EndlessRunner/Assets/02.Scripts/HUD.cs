using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{

    [SerializeField] GameObject _healthBar; // ��Ʈ �̹������� ��� �ִ� �� ���� ������Ʈ. Grid Layout ����
    [SerializeField] GameObject _health;    // ��Ʈ �̹��� ������

    Queue<GameObject> _hearts;              // Ȱ��ȭ�� ��Ʈ �̹��� ��Ƶδ� ť
    Queue<GameObject> _damagedHearts;       // ��Ȱ��ȭ�� ��Ʈ �̹��� ��Ƶδ� ť

    const int MAX_HEALTH = 3;               // �ִ� ü��

    private void Awake()
    {
        _hearts = new Queue<GameObject>(MAX_HEALTH);
        _damagedHearts = new Queue<GameObject>(MAX_HEALTH);

        // ���ʿ��� ������ ������ŭ ü�� �ʱ�ȭ
        for (int i = 0; i < MAX_HEALTH; i++)
        {
            _hearts.Enqueue(Instantiate(_health, _healthBar.transform));
        }
    }

    public void GetDamage()
    {
        GameObject damageHeart = _hearts.Dequeue();
        _damagedHearts.Enqueue(damageHeart);            // Ȱ��ȭ ��Ʈ ť���� �ϳ� ������ ��Ȱ��ȭ ��Ʈ ť�� ���
        damageHeart.SetActive(false);                   // ��Ȱ��ȭ ��Ŵ

    }

    public void GetHeart()
    {
        if (_damagedHearts.Count ==0)
            return;
        GameObject heart = _damagedHearts.Dequeue();
        _hearts.Enqueue(heart);                        // ��Ȱ��ȭ ��Ʈ ť���� �ϳ� ������ Ȱ��ȭ ��Ʈ ť�� ���
        heart.SetActive(true);                         // Ȱ��ȭ ��Ŵ
    }

    
}