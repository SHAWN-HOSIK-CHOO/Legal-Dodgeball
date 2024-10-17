using Assets.Scripts.PacketEvent;
using NetLibrary;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
namespace Client
{
    public class GameManager : Assets.Scripts.Network.GameManager
    {
        public GameObject MainCamera;


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
            for (int i = 0; i < prefabLists.Count; i++)
            {
                prefabDicts.TryAdd(prefabLists[i].name, prefabLists[i]);
            }
        }
        public Vector2 GetRandomPosition()
        {
            float x = UnityEngine.Random.Range(-10f, 10f);
            float y = UnityEngine.Random.Range(-10f, 10f);
            return new Vector2(x, y);
        }

        void ProcEvent(Memory<byte> e, EndUser user)
        {
            EventDefine type = (EventDefine)e.Span[0];
            switch (type)
            {
                case EventDefine.InstantiatePrefab:
                    {
                        (int ID, UnityEngine.Vector3 pos, UnityEngine.Quaternion qtn, bool ismine, string prefabName) = Event_InstantiatePrefab.GetDecode(e);
                        GameObject prefab = ismine ? prefabDicts["CPlayerGroup"] : prefabDicts["SPlayerGroup"];
                        GameObject NewObject = Instantiate(prefab, pos, qtn);
                        var view = NewObject.GetComponentInChildren<NetViewer>();
                        view.NetID = ID;
                        view.user = user;
                        view.IsMine = ismine;
                        view.prefabName = prefabName;
                        NetworkManager.instance.NetObjects.Add(ID, view);
                        Debug.Log($"Event_InstantiatePrefab NetID {ID}");
                    }
                    break;
                case EventDefine.MoveInput:
                    {
                        (int id, UnityEngine.Vector2 move) = Event_MoveInput.GetDecode(e);
                        if (NetworkManager.instance.NetObjects.TryGetValue(id, out var obj))
                        {
                            Assets.Scripts.Network.Player unityobj = obj.GetComponent<Assets.Scripts.Network.Player>();
                            unityobj.GetComponent<NetPlayerInput>().MoveInput(move);
                        }
                    }
                    break;
                case EventDefine.JumpInput:
                    {
                        (int id, bool jump) = Event_JumpInput.GetDecode(e);
                        if (NetworkManager.instance.NetObjects.TryGetValue(id, out var obj))
                        {
                            Assets.Scripts.Network.Player unityobj = obj.GetComponent<Assets.Scripts.Network.Player>();
                            unityobj.GetComponent<NetPlayerInput>().JumpInput(jump);
                        }
                    }
                    break;
                case EventDefine.SprintInput:
                    {
                        (int id, bool sprint) = Event_sprintInput.GetDecode(e);
                        if (NetworkManager.instance.NetObjects.TryGetValue(id, out var obj))
                        {
                            Assets.Scripts.Network.Player unityobj = obj.GetComponent<Assets.Scripts.Network.Player>();
                            unityobj.GetComponent<NetPlayerInput>().SprintInput(sprint);
                        }
                    }
                    break;
                case EventDefine.LookInput:
                    {
                        (int id, UnityEngine.Vector2 look) = Event_lookInput.GetDecode(e);
                        if (NetworkManager.instance.NetObjects.TryGetValue(id, out var obj))
                        {
                            Assets.Scripts.Network.Player unityobj = obj.GetComponent<Assets.Scripts.Network.Player>();
                            unityobj.GetComponent<NetPlayerInput>().LookInput(look);
                        }
                    }
                    break;
                case EventDefine.ChargeInput:
                    {
                        (int id, bool charge) = Event_chargeInput.GetDecode(e);
                        if (NetworkManager.instance.NetObjects.TryGetValue(id, out var obj))
                        {
                            Assets.Scripts.Network.Player unityobj = obj.GetComponent<Assets.Scripts.Network.Player>();
                            unityobj.GetComponent<NetPlayerInput>().ChargeInput(charge);
                        }
                    }
                    break;
                case EventDefine.ChangeTurn:
                    {
                        int id = Event_ChangeTurn.GetDecode(e);
                        if (NetworkManager.instance.NetObjects.TryGetValue(id, out var obj))
                        {
                            GManager.Instance.TurnPlayer = obj.gameObject;
                            if (obj.gameObject == GManager.Instance.Players[0])
                            {
                                Debug.Log($"My Turn!");
                            }
                            else if (obj.gameObject == GManager.Instance.Players[1])
                            {
                                Debug.Log($"Enemy Turn!");
                            }
                        }
                    }
                    break;
                case EventDefine.SetHp:
                    {
                        (int id, float hp) = Event_SetHp.GetDecode(e);
                        if (NetworkManager.instance.NetObjects.TryGetValue(id, out var obj))
                        {
                            if(obj.gameObject == GManager.Instance.Players[0])
                            {
                                Debug.Log($"SetPlayer1 HP {hp}");
                                GManager.Instance.SetPlayer1HP(hp);
                            }
                            else if (obj.gameObject == GManager.Instance.Players[1])
                            {
                                Debug.Log($"SetPlayer2 HP {hp}");
                                GManager.Instance.SetPlayer2HP(hp);
                            }
                        }
                    }
                    break;
                case EventDefine.SetPlayer:
                    {
                        (int p1ID, int p2ID) = Event_SetPlayer.GetDecode(e);
                        if (NetworkManager.instance.NetObjects.TryGetValue(p1ID, out var obj))
                        {
                            GManager.Instance.Players[0] = obj.gameObject;

                                Debug.Log($"SetPlayer1{p1ID}");
                        }
                        if (NetworkManager.instance.NetObjects.TryGetValue(p2ID, out var obj2))
                        {
                            GManager.Instance.Players[1] = obj2.gameObject;

                            Debug.Log($"SetPlayer2P{p2ID}");
                        }
                    }
                    break;
                case EventDefine.ThrowInput:
                    {
                        (int id, bool Throw) = Event_chargeInput.GetDecode(e);
                        if (Throw && NetworkManager.instance.NetObjects.TryGetValue(id, out var obj))
                        {
                            Assets.Scripts.Network.Player unityobj = obj.GetComponent<Assets.Scripts.Network.Player>();
                            if (obj.gameObject == GManager.Instance.Players[0])
                            {
                                Debug.Log($"Throw!");
                            }
                            else if (obj.gameObject == GManager.Instance.Players[1])
                            {
                                Debug.Log($"Enemy Throw!");
                            }
                            unityobj.GetComponent<NetPlayerInput>().ThrowInput(Throw);
                            unityobj.GetComponent<ThrowBallController>().AttackShootAction();
                        }
                    }
                    break;
                case EventDefine.PlayerSyncTransform:
                    {
                        (int id, UnityEngine.Vector3 pos, UnityEngine.Quaternion Qtn) = Event_TansformSync.GetDecode(e);
                        if (NetworkManager.instance.NetObjects.TryGetValue(id, out var obj))
                        {
                            Assets.Scripts.Network.Player unityobj = obj.GetComponent<Assets.Scripts.Network.Player>();
                            var controller = unityobj.GetComponent<CharacterController>();
                            controller.enabled = false;
                            unityobj.transform.position = Vector3.Lerp(unityobj.transform.position, pos, Mathf.Clamp01(3 * Time.deltaTime));
                            unityobj.CinemachineCameraTarget.transform.rotation = Qtn;
                            controller.enabled = true; ;
                        }
                    }
                    break;
            }
        }

        void Update()
        {
            while (NetworkManager.instance.EndUser?.PacketCompleteQueue.TryDequeue(out var e) == true)
            {
                ProcEvent(e, NetworkManager.instance.EndUser);
            }
            NetworkManager.instance.EndUser?.Dispatch();
        }

    }
}