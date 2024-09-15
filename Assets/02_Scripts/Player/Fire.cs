using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;

public class Fire : MonoBehaviourPun
{
    public Transform firePos;
    public GameObject bulletPref;
    private ParticleSystem muzzleFlash;
    private PhotonView pv = null;
    private bool isMouseClick => Input.GetMouseButtonDown(0);

    void Start()
    {
        pv = GetComponent<PhotonView>();
        firePos = transform.GetChild(2).GetChild(0).transform;
        bulletPref = Resources.Load<GameObject>("Bullet");
        muzzleFlash = firePos.transform.GetChild(0).GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        if (pv.IsMine && isMouseClick)
        {
            FireBullet(pv.Owner.ActorNumber);
            pv.RPC(nameof(FireBullet), RpcTarget.Others, pv.Owner.ActorNumber);
        }
    }

    [PunRPC]
    private void FireBullet(int actorNo)
    {
        if (!muzzleFlash.isPlaying)
            muzzleFlash.Play(true);
        GameObject bullet = Instantiate(bulletPref, firePos.position, firePos.rotation);
        bullet.GetComponent<Bullet>().actorNumber = actorNo;
    }
}
