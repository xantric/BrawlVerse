using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Cinemachine;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance { get; private set; }

    [Header("Player Setup")]
    public GameObject playerPrefab;
    public Transform spawnPoint;

    [Header("Cameras")]
    [Tooltip("Your old lobby camera GameObject (if any)")]
    public GameObject lobbyCamera;
    [Tooltip("Your in‑room camera GameObject (the one you now want disabled)")]
    public GameObject roomCamera;
    [Tooltip("Your Cinemachine FreeLook vcam to follow the player")]
    public CinemachineFreeLook freeLookCamera;

    [Header("UI")]
    public GameObject NickNameUI;
    public GameObject ConnectingScreenUI;

    [HideInInspector] public string roomName = "default";
    private string nickname = "Unnamed";
    private bool joinRequested = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Make sure the FreeLook is off until after join
        if (freeLookCamera != null)
            freeLookCamera.gameObject.SetActive(false);
    }

    public void SetNickname(string name) => nickname = name;
    public void SetRoomName(string name)     => roomName = name;

    public void OnJoinButtonPressed()
    {
        Debug.Log("Join Button Pressed. State: " + PhotonNetwork.NetworkClientState);
        joinRequested = true;

        NickNameUI.SetActive(false);
        ConnectingScreenUI.SetActive(true);

        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();
        else
            PhotonNetwork.JoinLobby();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server.");
        if (joinRequested)
            PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby. Now creating/joining room: " + roomName);
        if (joinRequested)
        {
            PhotonNetwork.JoinOrCreateRoom(
                roomName,
                new RoomOptions { MaxPlayers = 10 },
                TypedLobby.Default
            );
            joinRequested = false;
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room: " + roomName);

        // 1) Hide the connecting UI
        if (ConnectingScreenUI != null)
            ConnectingScreenUI.SetActive(false);

        // 2) Spawn the player (wires up freeLookCamera targets)
        SpawnPlayer();

        // 3) Disable your old lobby camera (if you still use it)
        if (lobbyCamera != null)
            lobbyCamera.SetActive(false);

        // 4) **Disable the roomCamera** that you no longer want active
        if (roomCamera != null)
            roomCamera.SetActive(false);

        // 5) Activate the FreeLook vcam to follow the player
        if (freeLookCamera != null)
            freeLookCamera.gameObject.SetActive(true);
    }

    public void SpawnPlayer()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("[RoomManager] playerPrefab not assigned!");
            return;
        }

        Vector3 pos = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        Quaternion rot = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;

        GameObject player = PhotonNetwork.Instantiate(
            playerPrefab.name,
            pos,
            rot
        );
        PhotonNetwork.LocalPlayer.NickName = nickname;

        if (player.TryGetComponent<PhotonView>(out var view) && view.IsMine)
        {
            if (player.TryGetComponent<PlayerHealth>(out var h))
                h.isLocalPlayer = true;

            // Attach your FreeLook vcam to the player
            Transform head = player.transform.childCount > 1
                ? player.transform.GetChild(1)
                : player.transform;

            freeLookCamera.Follow = head;
            freeLookCamera.LookAt   = head;
        }
    }
}
