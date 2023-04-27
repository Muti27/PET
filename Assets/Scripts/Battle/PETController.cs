using Cinemachine;
using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[ExecuteInEditMode]
[RequireComponent(typeof(InputSystem))]
[RequireComponent(typeof(PlayerInput))]
public class PETController : MonoBehaviour, IPunObservable
{
    private PhotonView photonView;                  //Photon連線必要
    private InputSystem input;                      //輸入系統(可統一)
    private PlayerInput playerInput;                //新Input系統
    private CinemachineVirtualCamera vcamera;       //虛擬攝影機    

    [SerializeField] private GameObject pet;
    [SerializeField] private Weapon weapon;

    [SerializeField] private float moveSpeed = 1.0f;

    private PETData petData;
    private Animator animator;                      //角色動畫
    private Vector3 lookPosition;
    private bool isDead = false;
    private bool isPlayAnim = false;
    private int currentAnimId;

    //
    private int animationMove;
    private int animationMoveLeft;
    private int animationMoveRight;
    private int animationMoveBack;
    private int animationRoll;
    private int animationAttack;
    private int animationDead;

    private void Awake()
    {
        if (photonView == null)
        {
            photonView = gameObject.GetComponent<PhotonView>();
        }

        if (input == null)
        {
            input = gameObject.GetComponent<InputSystem>();
        }

        if (playerInput == null)
        {
            playerInput = gameObject.GetComponent<PlayerInput>();
            playerInput.enabled = photonView.IsMine;
        }

        if (vcamera == null)
        {
            vcamera = gameObject.GetComponentInChildren<CinemachineVirtualCamera>();
            vcamera.enabled = photonView.IsMine;
        }

        if (weapon == null)
            weapon = gameObject.GetComponentInChildren<Weapon>();

        petData = new PETData();
        animator = gameObject.GetComponentInChildren<Animator>();

        SetAnimatorID();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
            return;

        CheckAttack();
        CheckRolling();
        Look();
        Move();
    }

    private void SetAnimatorID()
    {
        animationMove = Animator.StringToHash("move");
        animationMoveLeft = Animator.StringToHash("moveleft");
        animationMoveRight = Animator.StringToHash("moveright");
        animationMoveBack = Animator.StringToHash("moveback");

        animationRoll = Animator.StringToHash("roll");
        animationAttack = Animator.StringToHash("attack");
        animationDead = Animator.StringToHash("dead");
    }

    private void CheckAttack()
    {
        if (input.isAttack)
        {
            if (PhotonNetwork.OfflineMode)
                Attack();
            else
                photonView.RPC("Attack", RpcTarget.AllViaServer);
        }
    }

    [PunRPC]
    public void Attack()
    {
        if (isPlayAnim)
        {
            input.isAttack = false;
            return;
        }

        Debug.Log($"Attack");

        isPlayAnim = true;

        animator.SetTrigger(animationAttack);

        weapon.SetColliderActive(true);

        input.isAttack = false;

        StartCoroutine(DelayCall(animator.GetCurrentAnimatorStateInfo(0).length, () => { weapon.SetColliderActive(false); isPlayAnim = false; }));
    }

    private void CheckRolling()
    {
        if (input.isRolling)
        {
            if (PhotonNetwork.OfflineMode)
                Rolling();
            else
                photonView.RPC("Rolling", RpcTarget.AllViaServer);
        }
    }

    [PunRPC]
    private void Rolling()
    {
        if (input.isRolling)
        {
            Debug.Log($"Rolling");

            animator.SetTrigger(animationRoll);

            input.isRolling = false;
        }
    }

    private void Look()
    {
        if (input.look != Vector2.zero)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(input.look);
            if (Physics.Raycast(ray, out hit))
            {
                lookPosition = hit.point - pet.transform.position;

                pet.transform.rotation = Quaternion.LookRotation(new Vector3(lookPosition.x, 0, lookPosition.z));
            }
        }
    }

    private void Move()
    {
        if (input.move != Vector2.zero)
        {
            Vector3 direction = new Vector3(input.move.x, 0, input.move.y);
            float param = 1f;

            float angle = Quaternion.Angle(Quaternion.LookRotation(new Vector3(lookPosition.x, 0, lookPosition.z)), Quaternion.LookRotation(direction));
            //forward
            if (angle < 45f)
            {
                //animator.SetBool(animationMove, true);
                ChangeAnimation(animationMove, true);
            }
            //left or right
            else if (angle < 135)
            {
                param = 0.5f;
                var vectorCross = Vector3.Cross(new Vector3(lookPosition.x, 0, lookPosition.z).normalized, direction);

                //判斷左右
                if (vectorCross.y < 0)
                    //animator.SetBool(animationMoveLeft, true);
                    ChangeAnimation(animationMoveLeft, true);
                else
                    //animator.SetBool(animationMoveRight, true);
                    ChangeAnimation(animationMoveRight, true);
            }
            //back
            else
            {
                param = 0.5f;
                ChangeAnimation(animationMoveBack, true);                
            }

            transform.Translate(direction * moveSpeed * param * Time.deltaTime);

            //Debug.Log($"{angle}");
        }
        else
        {
            //animator.SetBool(animationMove, false);
            ChangeAnimation();
        }
    }

    public void OnHit(int damage)
    {
        if (isDead)
            return;

        petData.HP -= damage;

        Debug.Log($"{petData.HP}");

        if (petData.HP <= 0)
        {
            if (PhotonNetwork.OfflineMode)
                OnDead();
            else
                photonView.RPC("OnDead", RpcTarget.AllViaServer);
        }
    }

    [PunRPC]
    private void OnDead()
    {
        isDead = true;

        animator.SetBool(animationDead, true);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //stream.SendNext(CurrentDistance);
            //stream.SendNext(CurrentSpeed);
            //stream.SendNext(m_input);
        }
        else
        {
            //if (this.m_firstTake)
            //{
            //    this.m_firstTake = false;
            //}

            //this.CurrentDistance = (float)stream.ReceiveNext();
            //this.CurrentSpeed = (float)stream.ReceiveNext();
            //this.m_input = (float)stream.ReceiveNext();
        }
    }

    private void ChangeAnimation(int animationId = 0, bool active = false)
    {
        if (animationId != 0)
        {
            animator.SetBool(currentAnimId, false);
            currentAnimId = animationId;
        }

        animator.SetBool(currentAnimId, active);
    }

    private IEnumerator DelayCall(float time, Action callback)
    {
        yield return new WaitForSeconds(time);

        callback?.Invoke();
    }
}
