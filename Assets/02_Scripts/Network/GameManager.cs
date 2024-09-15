using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        // 네트워크 상태 확인 후 플레이어 생성
        if (PhotonNetwork.InRoom)
        {
            CreatePlayer();
        }
        else
        {
            Debug.LogWarning("Cannot create player. Not in a room.");
        }

        PhotonNetwork.IsMessageQueueRunning = true;
        Debug.Log(PhotonNetwork.CurrentRoom?.Name);
    }

    [PunRPC]
    private void CreatePlayer()
    {
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
            Debug.Log($"Player NickName = {player.Value.NickName} : {player.Value.ActorNumber}");
            
        Transform[] points = GameObject.Find("SpawnPoints").GetComponentsInChildren<Transform>();
        int index = Random.Range(1, points.Length);
        PhotonNetwork.Instantiate("Player", points[index].position, points[index].rotation);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
        CreatePlayer();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player {newPlayer.NickName} entered room");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"Player {otherPlayer.NickName} left room");
    }
}