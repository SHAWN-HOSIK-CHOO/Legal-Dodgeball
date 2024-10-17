using Assets.Scripts.Network;
using Assets.Scripts.PacketEvent;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GManager : MonoBehaviour
{
    //Singleton
    private static GManager _instance;
    public static GManager Instance => _instance == null ? null : _instance;
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
    [Header("Player1 Status")]
    public float PlayerHP = 60f;
    public float PlayerMaxHP = 100f;
    public int PlayerCurrentBallCount = 1;

    //Enemy Status
    [Space(5)]
    //Player Status
    [Header("Player2 Status")]
    public float Player2HP = 60f;
    public float Player2MaxHP = 100f;
    public int Player2CurrentBallCount = 1;

    //Level Status

    //Game Status
    [Space(5)]
    [Header("Game Status")]
    public int RoundCount = 1;
    public float TimePerGame = 180f;

    public GameObject TurnPlayer;

    public GameObject[] Players = new GameObject[2];
    //Methods

    public void ChangeTurn()
    {
        if (Players[0] == TurnPlayer)
        {
            TurnPlayer = Players[1];
        }
        else
        {
            TurnPlayer = Players[0];
        }

        if (Assets.Scripts.Network.GameManager.instance.IsServer)
        {
            var GM = Assets.Scripts.Network.GameManager.instance as Server.GameManager;
            int newTrunId = TurnPlayer.GetComponent<NetViewer>().NetID;
            var ChangeEvent = new Event_ChangeTurn(newTrunId);
            GM.SendAllUser(ChangeEvent);
        }

        if (TurnPlayer == GManager.Instance.Players[0])
        {
            Debug.Log($"Changed Player1 Turn!");
        }
        else if (TurnPlayer == GManager.Instance.Players[1])
        {
            Debug.Log($"Changed Player2 Turn!");
        }
    }

    public void SetPlayer1HP(float hp)
    {
        PlayerHP = hp;
    }
    public void SetPlayer2HP(float hp)
    {
        Player2HP = hp;
    }

    public void MinusPlayerHP(GameObject obj,float minus)
    {
       
        if (Assets.Scripts.Network.GameManager.instance.IsServer)
        {
            var GM = Assets.Scripts.Network.GameManager.instance as Server.GameManager;

            if (obj == GManager.Instance.Players[0])
            {
                Debug.Log($"Minus Player1 HP!");

                if (PlayerHP - minus <= 0f)
                {
                    PlayerHP = 0f;
                }
                else
                {
                    PlayerHP -= minus;
                }
                var HP_Event = new Event_SetHp(obj.GetComponent<NetViewer>().NetID, PlayerHP);
                GM.SendAllUser(HP_Event);
            }
            else if (obj == GManager.Instance.Players[1])
            {
                Debug.Log($"Minus Player2 HP!");
                if (Player2HP - minus <= 0f)
                {
                    Player2HP = 0f;
                }
                else
                {
                    Player2HP -= minus;
                }
                var HP_Event = new Event_SetHp(obj.GetComponent<NetViewer>().NetID, Player2HP);
                GM.SendAllUser(HP_Event);
            }
        }
    }

}
