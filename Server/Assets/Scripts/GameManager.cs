using Assets.Scripts.PacketEvent;
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
    public List<GameObject> prefabEntries = new List<GameObject>();
    Dictionary<string, GameObject> PrefabDicts = new Dictionary<string, GameObject>();
    public static GameManager instance { get; private set; } = null;

    Dictionary<IPEndPoint, EndUser> Endusers = new Dictionary<IPEndPoint, EndUser>();
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
            PrefabDicts.TryAdd(prefabEntries[i].name, prefabEntries[i]);
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
                        obj.transform.position = Vector3.Lerp(obj.transform.position, pos, Mathf.Clamp01(3 * Time.deltaTime));
                        obj.GetComponent<Player>().CinemachineCameraTarget.transform.rotation = Qtn;
                    }
                }
                break;
        }
    }

    void Update()
    {
        if(NetworkManager.instance.NewSyncUsers.TryDequeue(out var user))
        {
            if (!Endusers.TryGetValue(user.RemoteEndPoint, out var _))
            {
                // 새로운 유저가 들어온 경우
                Debug.Log($"New Player! {user.RemoteEndPoint} : {user.SyncID}");
                Endusers.Add(user.RemoteEndPoint, user);
                // 월드의 임의의 위치에 플레이어를 생성
                Vector2 NewPos = GetRandomPosition();
                GameObject player = Instantiate(PrefabDicts["Player"], new Vector3(NewPos.x, 5, NewPos.y), Quaternion.identity);
                var CNET = player.AddComponent<NetComponent>();
                Event_InstantiatePrefab e = new Event_InstantiatePrefab(NetworkManager.instance.AllocNetObjectID(), new Vector3(NewPos.x, 5, NewPos.y), Quaternion.identity, "Player");
                CNET.ID = e.ID;
                CNET.user = user;
                Transform playerCameraRoot = player.transform.Find("PlayerCameraRoot");
                var CNET2 = playerCameraRoot.AddComponent<NetComponent>();
                CNET2.ID = NetworkManager.instance.AllocNetObjectID();
                CNET2.user = user;
                NetObjects.Add(CNET.ID, CNET);
                NetObjects.Add(CNET2.ID, CNET2);
                user.DefferedSend(e.GetBytes());
                Debug.Log($"InstantiatePrefab{"Player"}");
            }
        }

        foreach(var kv in Endusers)
        {
            while (kv.Value.PacketCompleteQueue.TryDequeue(out var e))
            {
                ProcEvent(e,kv.Value);
            }
            kv.Value.Dispatch();
        }
    }
}
