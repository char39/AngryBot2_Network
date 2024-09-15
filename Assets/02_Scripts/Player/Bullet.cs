using Photon.Pun;
using UnityEngine;

public class Bullet : MonoBehaviourPun
{
    public GameObject effect;
    public int actorNumber;     // 총알을 발사한 플레이어의 고유번호

    void Start()
    {
        GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 3000f);
        
        Destroy(gameObject, 3.0f);
    }

    private void OnCollisionEnter(Collision col)
    {
        var contact = col.GetContact(0);
        var obj = Instantiate(effect, contact.point, Quaternion.LookRotation(-contact.normal));
        Destroy(obj, 2.0f);
        Destroy(gameObject);
    }


}
