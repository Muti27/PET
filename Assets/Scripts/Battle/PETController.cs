using Cinemachine;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Profiling.Memory.Experimental;

[ExecuteInEditMode]
[RequireComponent(typeof(InputSystem))]
[RequireComponent(typeof(PlayerInput))]
public class PETController : MonoBehaviour, IPunObservable
{
    private InputSystem input;
    private PlayerInput playerInput;
    private PhotonView photonView;
    private CinemachineVirtualCamera vcamera;    
    private Animator animator;

    [SerializeField] private GameObject pet;
    [SerializeField] private Weapon weapon;

    [SerializeField] private float moveSpeed = 1.0f;

    private PETData petData;
    private bool isPlayAnim = false;
    //
    private int animationMove;
    private int animationRoll;
    private int animationAttack;

    private void Awake()
    {
        if (input == null)
        {
            input = gameObject.GetComponent<InputSystem>();
        }

        if (playerInput == null)
        {
            playerInput = gameObject.GetComponent<PlayerInput>();
            //playerInput.camera = Camera.main;
        }

        if (photonView == null)
        {
            photonView = gameObject.GetComponent<PhotonView>();
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
        Attack();
        Roll();
        Look();
        Move();
    }

    private void SetAnimatorID()
    {
        animationMove = Animator.StringToHash("move");
        animationRoll = Animator.StringToHash("roll");
        animationAttack = Animator.StringToHash("attack");
    }

    private void Attack()
    {
        if (input.isAttack)
        {
            if (PhotonNetwork.OfflineMode)
            {
                AttackRPC();
            }
            else
            {
                photonView.RPC("AttackRPC", RpcTarget.AllViaServer);
            }
        }
    }

    [PunRPC]
    public void AttackRPC()
    {
        if (isPlayAnim)
            return;

        Debug.Log($"Attack");

        isPlayAnim = true;

        animator.SetTrigger(animationAttack);

        weapon.SetColliderActive(true);

        input.isAttack = false;

        StartCoroutine(DelayCall(animator.GetCurrentAnimatorStateInfo(0).length, () => { weapon.SetColliderActive(false); isPlayAnim = false; }));
    }

    private void Roll()
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
            var ray = Camera.main.ScreenPointToRay(input.look);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 lookPosition = hit.point - pet.transform.position;

                pet.transform.rotation = Quaternion.LookRotation(new Vector3(lookPosition.x, 0, lookPosition.z));
            }
            
        }
    }

    private void Move()
    {
        if (input.move != Vector2.zero)
        {
            transform.Translate(new Vector3(input.move.x, 0, input.move.y) * moveSpeed * Time.deltaTime);

            animator.SetBool(animationMove, true);
        }
        else
        {
            animator.SetBool(animationMove, false);
        }
    }

    public void OnHit(int damage)
    {
        petData.HP -= damage;

        Debug.Log($"{petData.HP}");

        if (petData.HP <= 0)
        {
            Destroy(gameObject);
        }
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

    private IEnumerator DelayCall(float time, Action callback)
    {
        yield return new WaitForSeconds(time);

        callback?.Invoke();
    }
}
