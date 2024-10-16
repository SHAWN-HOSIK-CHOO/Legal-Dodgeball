using Assets.Scripts.PacketEvent;
using NetLibrary;
using System.Collections.Generic;
using UnityEngine;


public class NetViewer : MonoBehaviour
{
    public bool IsMine;
    public string prefabName;
    public int NetID;
    public EndUser user { get; set; }
    public Queue<Event_Packet> packets = new Queue<Event_Packet>();

    private void Start()
    {

    }
}