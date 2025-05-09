using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using NUnit.Framework;
using System;


public class HUD : MonoBehaviour
{

    [SerializeField] GameObject _healthBar;
    [SerializeField] GameObject _health;

    Queue<GameObject> _hearts;
    Queue<GameObject> _damagedHearts;
    const int MAX_HEALTH = 3;

    private void Awake()
    {
        _hearts = new Queue<GameObject>(MAX_HEALTH);
        _damagedHearts = new Queue<GameObject>(MAX_HEALTH);

        for (int i = 0; i < MAX_HEALTH; i++)
        {
            _hearts.Enqueue(Instantiate(_health, _healthBar.transform));
        }
    }

    public void GetDamage()
    {
        GameObject damageHeart = _hearts.Dequeue();
        _damagedHearts.Enqueue(damageHeart);
        damageHeart.SetActive(false);

    }

    public void GetHeart()
    {
        if (_damagedHearts.Count ==0)
            return;
        GameObject heart = _damagedHearts.Dequeue();
        _hearts.Enqueue(heart);
        heart.SetActive(true);
    }

    
}