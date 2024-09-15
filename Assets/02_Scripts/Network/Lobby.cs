using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using ExitGames.Client.Photon;

public class Lobby : MonoBehaviourPunCallbacks
{
    public string version = "1.0";
    public GameObject UICanvas;
    public TMP_InputField UserID;
    public TMP_InputField RoomName;
    public Button Login;
    public Button RoomJoin;

    private string userID;
    private string roomName;

    void Awake()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.GameVersion = version;
            Debug.Log(PhotonNetwork.SendRate);
            PhotonNetwork.ConnectUsingSettings();
            Login.interactable = false;
            RoomJoin.interactable = false;
        }
    }

    void Start()
    {
        userID = PlayerPrefs.GetString("UserID", $"User{Random.Range(1,21):00}");
        UserID.text = userID;
        PhotonNetwork.NickName = userID;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.NetworkClientState);
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        Debug.Log(PhotonNetwork.NetworkClientState);
        Login.interactable = true;
        RoomJoin.interactable = true;
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"OnJoinRandomFailed() : ({returnCode}) {message}");
        OnMakeRoomClick();
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"OnJoinRoomFailed() : ({returnCode}) {message}");
        Login.interactable = true;
        RoomJoin.interactable = true;
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.NetworkClientState);
        if (PhotonNetwork.IsMasterClient)
            StartCoroutine(LoadScene());
        //PhotonNetwork.LoadLevel("MainScene");
    }


    public void OnMakeRoomClick()
    {
        RoomOptions roomOptions = new()
        {
            MaxPlayers = 20,
            IsOpen = true,
            IsVisible = true
        };
        PhotonNetwork.CreateRoom(SetRoomName(), roomOptions);
    }

    public void OnLoginClick()
    {
        Login.interactable = false;
        RoomJoin.interactable = false;
        SetUserID();
        PhotonNetwork.JoinRandomRoom();
    }


    public void SetUserID()
    {
        if (string.IsNullOrEmpty(UserID.text))
            userID = $"User{Random.Range(1, 21):00}";
        else
            userID = UserID.text;
        PlayerPrefs.SetString("UserID", userID);
        PhotonNetwork.NickName = userID;
    }

    public string SetRoomName()
    {
        if (string.IsNullOrEmpty(RoomName.text))
            RoomName.text = $"Room{Random.Range(1, 99):00}";
        return RoomName.text;
    }

    public void OnClickJoinRoom()
    {
        Login.interactable = false;
        RoomJoin.interactable = false;
        SetUserID();
        PhotonNetwork.JoinRoom(SetRoomName());
    }

    private IEnumerator LoadScene()
    {
        PhotonNetwork.IsMessageQueueRunning = false;
        AsyncOperation async = SceneManager.LoadSceneAsync("MainScene");
        yield return async;
    }

}
