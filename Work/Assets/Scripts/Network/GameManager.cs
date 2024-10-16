using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Singleton
    private static GameManager _instance;
    public static  GameManager Instance => _instance == null ? null : _instance;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
        //Player Status
    [Header("Player Status")] 
    public float PlayerHP = 60f;
    public float PlayerMaxHP = 100f;
    public int   PlayerCurrentBallCount = 1;

    //Enemy Status
    [Space(5)]
    [Header("Enemy Status")] 
    public float EnemyHP = 100f;
    public float EnemyMaxHP            = 100f;
    public int   EnemyCurrentBallCount = 1;

    //Level Status

    //Game Status
    [Space(5)] 
    [Header("Game Status")] 
    public bool isPlayerTurn = true;
    public int   RoundCount  = 1;
    public float TimePerGame = 180f;
    
    //Methods
    //Player
    public void MinusPlayerHP(float minus)
    {
        if (PlayerHP - minus <= 0f)
        {
            PlayerHP = 0f;
        }
        else
        {
            PlayerHP -= minus;
        }
    }
    public void PlusPlayerHP(float plus)
    {
        if (PlayerHP + plus >= PlayerMaxHP)
        {
            PlayerHP = PlayerMaxHP;
        }
        else
        {
            PlayerHP += plus;
        }
    }
    
    //Enemy
    public void MinusEnemyHP(float minus)
    {
        if (EnemyHP - minus <= 0f)
        {
            EnemyHP = 0f;
        }
        else
        {
            EnemyHP -= minus;
        }
    }
    public void PlusEnemyHP(float plus)
    {
        if (EnemyHP + plus >= EnemyMaxHP)
        {
            EnemyHP = EnemyMaxHP;
        }
        else
        {
            EnemyHP += plus;
        }
    }
    
    //Level
    
    //Game
    
}
