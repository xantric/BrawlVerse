using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class RoomList : MonoBehaviourPunCallbacks
{
    // ← Singleton instance for external access
    public static RoomList Instance { get; private set; }

    [Header("Room Manager Link")]
    [Tooltip("Drag your RoomManager GameObject here")]
    public RoomManager roomManager;
    public GameObject roomManagerUI;

    [Header("Room List UI")]
    public GameObject roomNamePrefab;
    public Transform roomListParent;

    private List<RoomInfo> cachedRooms = new();

    void Awake()
    {
        // ← enforce Singleton
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    IEnumerator Start()
    {
        // leave any existing room
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            yield return new WaitUntil(() => !PhotonNetwork.InRoom);
        }

        // connect or wait
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            yield return new WaitUntil(() => PhotonNetwork.IsConnectedAndReady);
        }
    }

    public override void OnConnectedToMaster()
    {
        // join lobby once connected
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        // once in lobby, show room‑list UI
        Debug.Log("✅ Joined Lobby.");
        roomManager.ConnectingScreenUI.SetActive(false);
        roomManager.NickNameUI.SetActive(false);
        roomManagerUI.SetActive(true);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // maintain cache
        foreach (RoomInfo room in roomList)
        {
            int idx = cachedRooms.FindIndex(r => r.Name == room.Name);
            if (idx >= 0)
            {
                if (room.RemovedFromList)
                    cachedRooms.RemoveAt(idx);
                else
                    cachedRooms[idx] = room;
            }
            else if (!room.RemovedFromList)
            {
                cachedRooms.Add(room);
            }
        }

        UpdateRoomListUI();
    }

    void UpdateRoomListUI()
    {
        // clear old entries
        foreach (Transform child in roomListParent)
            Destroy(child.gameObject);

        // instantiate new entries
        foreach (RoomInfo room in cachedRooms)
        {
            GameObject item = Instantiate(roomNamePrefab, roomListParent);
            item.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = room.Name;
            item.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{room.PlayerCount}/10";
            item.GetComponent<RoomButton>()._roomName = room.Name;
        }
    }

    // called by RoomButton when a room‑entry is clicked
    public void JoinRoomByName(string roomName)
    {
        roomManager.SetRoomName(roomName);
        roomManagerUI.SetActive(true);
        gameObject.SetActive(false);
    }
}
