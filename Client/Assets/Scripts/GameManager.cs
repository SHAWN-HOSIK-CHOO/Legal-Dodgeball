using Assets.Scripts.PacketEvent;
using Cinemachine;
using NetLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; } = null;
    public GameObject VirtualCamera;
    public GameObject MainCamera;


    public List<GameObject> prefabEntries = new List<GameObject>();
    Dictionary<string,GameObject> PrefabDicts = new Dictionary<string, GameObject>();
    Dictionary<IPEndPoint, EndUser> Players = new Dictionary<IPEndPoint, EndUser>();
    Dictionary<int, NetComponent> NetObjects = new Dictionary<int, NetComponent>();

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }
    }
    void Start()
    {
        for (int i = 0; i < prefabEntries.Count; i++)
        {
            var prefabType = PrefabUtility.GetPrefabAssetType(prefabEntries[i]);
            if (prefabType != PrefabAssetType.NotAPrefab)
            {
                PrefabDicts.TryAdd(prefabEntries[i].name, prefabEntries[i]);
            }
        }
    }
    public Vector2 GetRandomPosition()
    {
        float x = UnityEngine.Random.Range(-10f, 10f); 
        float y = UnityEngine.Random.Range(-10f, 10f); 
        return new Vector2(x, y);
    }


    void ProcEvent(Memory<byte> e , EndUser user)
    {
        EventDefine type = (EventDefine)e.Span[0];
        switch (type)
        {
            case EventDefine.InstantiatePrefab:
                {
                    (int ID, UnityEngine.Vector3 pos, UnityEngine.Quaternion Qtn, string prefabName) = Event_InstantiatePrefab.GetDecode(e);
                    GameObject Object = Instantiate(PrefabDicts[prefabName], pos, Qtn);
                    var Cnet = Object.AddComponent<NetComponent>();
                    Cnet.ID = ID;
                    Cnet.user = user;
                    Debug.Log($"InstantiatePrefab{prefabName}");
                    NetObjects.Add(ID, Cnet);
                }
                break;
            case EventDefine.ChargeInput:
                {
                    (int ID, bool charge) = Event_chargeInput.GetDecode(e);
                    if (NetObjects.TryGetValue(ID, out var obj))
                    {
                        obj.GetComponent<NetPlayerInput>().ChargeInput(charge);
                    }
                }
                break;
            case EventDefine.ThrowInput:
                {
                    (int id, bool Throw) = Event_ThrowInput.GetDecode(e);
                    if (NetObjects.TryGetValue(id, out var obj))
                    {
                        obj.GetComponent<NetPlayerInput>().ThrowInput(Throw);
                    }
                }
                break;
            case EventDefine.JumpInput:
                {
                    (int id, bool jump) = Event_JumpInput.GetDecode(e);
                    if (NetObjects.TryGetValue(id, out var obj))
                    {
                        obj.GetComponent<NetPlayerInput>().JumpInput(jump);
                    }
                }
                break;
            case EventDefine.SprintInput:
                {
                    (int id, bool sprint) = Event_sprintInput.GetDecode(e);
                    if (NetObjects.TryGetValue(id, out var obj))
                    {
                        obj.GetComponent<NetPlayerInput>().SprintInput(sprint);
                    }
                }
                break;
            case EventDefine.LookInput:
                {
                    (int id, UnityEngine.Vector2 look) = Event_lookInput.GetDecode(e);
                    if (NetObjects.TryGetValue(id, out var obj))
                    {
                        obj.GetComponent<NetPlayerInput>().LookInput(look);
                    }
                }
                break;

            case EventDefine.MoveInput:
                {
                    (int id, UnityEngine.Vector2 move) = Event_MoveInput.GetDecode(e);
                    if (NetObjects.TryGetValue(id, out var obj))
                    {
                        obj.GetComponent<NetPlayerInput>().MoveInput(move);
                    }
                }
                break;

            case EventDefine.TansformSync:
                {
                    (int id, UnityEngine.Vector3 pos, UnityEngine.Quaternion Qtn) = Event_TansformSync.GetDecode(e);
                    if (NetObjects.TryGetValue(id, out var obj))
                    {
                        CharacterController CC = obj.GetComponent<CharacterController>();
                        CC.enabled = false;
                        obj.transform.position = Vector3.Lerp(obj.transform.position, pos, Mathf.Clamp01(3 * Time.deltaTime));
                        obj.transform.rotation = Quaternion.Lerp(obj.transform.rotation, Qtn, Mathf.Clamp01(3 * Time.deltaTime));
                        CC.enabled = true;
                    }
                }
                break;
        }
    }

    void Update()
    {
        if(NetworkManager.instance.NewSyncUsers.TryDequeue(out var user))
        {
            if (!Players.TryGetValue(user.RemoteEndPoint, out _))
            {
                // 새로 접속을 시도하고 있음
                Debug.Log($"New Player! {user.RemoteEndPoint} : {user.SyncID}");
                Players.Add(user.RemoteEndPoint, user);
                //Connect 패킷을 서버에 준다?
            }
            else
            {
                //유저의 네트워크 상태가 변경되어 다시 접속함
                Debug.Log($"ReConnect Player! {user.RemoteEndPoint} : {user.SyncID}");
            }
        }

        foreach(var kv in Players)
        {
            while(kv.Value.PacketCompleteQueue.TryDequeue(out var e))
            {
                ProcEvent(e,kv.Value);
            }
            kv.Value.Dispatch();
        }
    }
}
