using System.Collections;
using Photon.Pun;
using UnityEngine;
using Player = Photon.Realtime.Player;

public class Damage : MonoBehaviourPunCallbacks
{
    private Renderer[] renderers;
    private int initialHP = 100;
    public int curHP = 0;
    private Animator ani;
    private CharacterController charCtrl;
    private readonly int hashDie = Animator.StringToHash("Die");
    private readonly int hashRespawn = Animator.StringToHash("Respawn");
    private GameManager gameManager;
    
    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        ani = GetComponent<Animator>();
        charCtrl = GetComponent<CharacterController>();
        curHP = initialHP;
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnCollisionEnter(Collision col)
    {
        if (curHP > 0 && col.collider.CompareTag("BULLET"))
        {
            curHP -= 20;
            if (curHP <= 0)
            {
                if (photonView.IsMine)
                {
                    var actorNo = col.collider.GetComponent<Bullet>().actorNumber;
                    Player lastShootPlayer = PhotonNetwork.CurrentRoom.GetPlayer(actorNo);
                    string msg = $"{lastShootPlayer.NickName}님이 {photonView.Owner.NickName}님을 죽였습니다.";
                    // photonView.RPC("KillMessage", RpcTarget.AllBufferedViaServer, msg);
                }
                StartCoroutine(PlayerDie());
            }
        }
    }

    private IEnumerator PlayerDie()
    {
        charCtrl.enabled = false;                   // 캐릭터 컨트롤러 비활성화
        ani.SetBool(hashRespawn, false);            // 리스폰 애니메이션 비활성화
        ani.SetTrigger(hashDie);                    // 죽는 애니메이션 활성화
        yield return new WaitForSeconds(3.0f);      // 3초 대기

        ani.SetBool(hashRespawn, true);             // 리스폰 애니메이션 활성화
        SetPlayerVisible(false);                    // 플레이어 비활성화
        yield return new WaitForSeconds(1.5f);      // 1.5초 대기

        Transform[] points = GameObject.Find("SpawnPoints").GetComponentsInChildren<Transform>();
        int index = Random.Range(1, points.Length);
        transform.position = points[index].position;

        curHP = initialHP;
        charCtrl.enabled = true;
        SetPlayerVisible(true);
    }

    private void SetPlayerVisible(bool isVisible)
    {
        foreach (Renderer renderer in renderers)
            renderer.enabled = isVisible;
    }

    [PunRPC]
    private void KillMessage(string msg)
    {
        //gameManager.msgList.text += msg;
    }
}
