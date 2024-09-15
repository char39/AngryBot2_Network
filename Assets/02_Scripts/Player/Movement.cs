using UnityEngine;
using Cinemachine;
using Photon.Pun;
using Photon.Realtime;

public class Movement : MonoBehaviourPunCallbacks, IPunObservable
{
    private CharacterController charCtrl;
    private new Transform transform;
    private Animator ani;
    private new Camera camera;
    private Plane plane;                        // 가상의 Plane에 RayCast 접근을 위한 변수.
    private Ray ray;
    private Vector3 hitPoint;
    internal float moveSpeed = 10.0f;

    private CinemachineVirtualCamera vir;
    private PhotonView pv = null;

    private Vector3 curPos;
    private Quaternion curRot;
    private float forward;
    private float strafe;

    void Start()
    {
        transform = GetComponent<Transform>();
        charCtrl = GetComponent<CharacterController>();
        ani = GetComponent<Animator>();
        vir = FindObjectOfType<CinemachineVirtualCamera>();
        pv = GetComponent<PhotonView>();
        camera = Camera.main;
        plane = new Plane(transform.up, transform.position);

        pv.Synchronization = ViewSynchronization.ReliableDeltaCompressed;
        pv.ObservedComponents[0] = this;
        if (pv.IsMine)
        {
            vir.Follow = transform;
            vir.LookAt = transform;
        }
    }

    void Update()
    {
        if (pv.IsMine)
        {
            Move();
            Turn();
        }
        else if (!pv.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 10.0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, curRot, Time.deltaTime * 10.0f);
            SetFloat("Forward", forward);
            SetFloat("Strafe", strafe);
        }
    }

    private float H => Input.GetAxis("Horizontal");
    private float V => Input.GetAxis("Vertical");

    private void Move()
    {
        if (!charCtrl.enabled) return;
        Vector3 cameraForward = camera.transform.forward;           // 카메라의 전방 벡터.
        Vector3 cameraRight = camera.transform.right;               // 카메라의 오른쪽 벡터.
        cameraForward.y = 0;                                        // y축 회전을 막기 위해 y값을 0으로 설정.
        cameraRight.y = 0;                                          // y축 회전을 막기 위해 y값을 0으로 설정.

        Vector3 moveDir = cameraForward * V + cameraRight * H;      // 이동 방향을 계산.
        moveDir.Set(moveDir.x, 0f, moveDir.z);                      // y축 회전을 막기 위해 y값을 0으로 설정.
        charCtrl.SimpleMove(moveDir * moveSpeed);                   // 캐릭터를 이동.

        forward = Vector3.Dot(moveDir, transform.forward);    // 전후 이동 값 계산.
        strafe = Vector3.Dot(moveDir, transform.right);       // 좌우 이동 값 계산.

        SetFloat("Forward", forward);                               // 전후 이동 값을 애니메이터에 전달.
        SetFloat("Strafe", strafe);                                 // 좌우 이동 값을 애니메이터에 전달.
    }

    private void SetFloat(string name, float value)
    {
        ani.SetFloat(name, value);
    }

    private void Turn()
    {
        ray = camera.ScreenPointToRay(Input.mousePosition);         // 마우스 위치를 Ray로 변환.
        float enter = 0.0f;                                         // Ray와 Plane의 충돌 지점을 저장할 변수.
        plane.Raycast(ray, out enter);                              // Ray와 Plane의 충돌 여부를 확인.
        hitPoint = ray.GetPoint(enter);                             // 충돌 지점을 저장.

        Vector3 lookDir = hitPoint - transform.position;            // 캐릭터가 바라볼 방향을 계산.
        lookDir.y = 0f;                                             // y축 회전을 막기 위해 y값을 0으로 설정.

        transform.localRotation = Quaternion.LookRotation(lookDir); // 캐릭터가 바라볼 방향으로 회전.
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(forward);
            stream.SendNext(strafe);
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
            curRot = (Quaternion)stream.ReceiveNext();
            forward = (float)stream.ReceiveNext();
            strafe = (float)stream.ReceiveNext();
        }
    }
}