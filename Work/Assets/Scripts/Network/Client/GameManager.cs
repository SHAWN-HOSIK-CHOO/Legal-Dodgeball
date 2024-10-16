using Assets.Scripts.PacketEvent;
using NetLibrary;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Client
{
    public class GameManager : MonoBehaviour
    {
        public GameObject MainCamera;

        public static GameManager instance { get; private set; } = null;

        public List<GameObject> prefabEntries = new List<GameObject>();
        Dictionary<string, GameObject> prefabDicts = new Dictionary<string, GameObject>();

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
                prefabDicts.TryAdd(prefabEntries[i].name, prefabEntries[i]);
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
                        var view = NewObject.GetComponent<NetViewer>();
                        view.NetID = ID;
                        view.user = user;
                        view.IsMine = ismine;
                        view.prefabName = prefabName;
                        NetworkManager.instance.NetObjects.Add(ID, view);
                        Debug.Log($"Event_InstantiatePrefab NetID {ID}");
                    }
                    break;

                case EventDefine.JumpInput:
                    {
                        (int id, bool jump) = Event_JumpInput.GetDecode(e);
                        if (NetworkManager.instance.NetObjects.TryGetValue(id, out var obj))
                        {
                            Player unityobj = obj.GetComponent<Player>();
                            unityobj.GetComponent<NetPlayerInput>().JumpInput(jump);
                        }
                    }
                    break;
                case EventDefine.SprintInput:
                    {
                        (int id, bool sprint) = Event_sprintInput.GetDecode(e);
                        if (NetworkManager.instance.NetObjects.TryGetValue(id, out var obj))
                        {
                            Player unityobj = obj.GetComponent<Player>();
                            unityobj.GetComponent<NetPlayerInput>().SprintInput(sprint);
                        }
                    }
                    break;
                case EventDefine.LookInput:
                    {
                        (int id, UnityEngine.Vector2 look) = Event_lookInput.GetDecode(e);
                        if (NetworkManager.instance.NetObjects.TryGetValue(id, out var obj))
                        {
                            Player unityobj = obj.GetComponent<Player>();
                            unityobj.GetComponent<NetPlayerInput>().LookInput(look);
                        }
                    }
                    break;

                case EventDefine.MoveInput:
                    {
                        (int id, UnityEngine.Vector2 move) = Event_MoveInput.GetDecode(e);
                        if (NetworkManager.instance.NetObjects.TryGetValue(id, out var obj))
                        {
                            Player unityobj = obj.GetComponent<Player>();
                            unityobj.GetComponent<NetPlayerInput>().MoveInput(move);
                        }
                    }
                    break;

                case EventDefine.PlayerSyncTransform:
                    {
                        (int id, UnityEngine.Vector3 pos, UnityEngine.Quaternion Qtn) = Event_TansformSync.GetDecode(e);
                        if (NetworkManager.instance.NetObjects.TryGetValue(id, out var obj))
                        {
                            Player unityobj = obj.GetComponent<Player>();
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