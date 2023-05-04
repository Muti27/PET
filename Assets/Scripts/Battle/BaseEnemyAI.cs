using BLINK;
using JetBrains.Annotations;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling.Memory.Experimental;
using UnityEngineInternal;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class BaseEnemyAI : MonoBehaviour
{
    private Animator animator;
    private PETController target;
    private PhotonView photonView;

    [SerializeField] 
    private float speed = 2f;

    private float hp = 100;
    private float atk = 50;
    private float def = 10;

    private bool isCombat = false;
    private bool isPlayAnim = false;
    private bool isDead = false;
    private bool isOnHit = false;

    private const string animationAttackRight = "attack_r";
    private const string animationAttackLeft = "attack_l";
    private const string animationMove = "move";
    private const string animationSleep = "sleep";
    private const string animationCombat = "combat";
    private const string animationHit = "hit";
    private const string animationDead = "dead";

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {        
        if (isPlayAnim)
            return;

        if (!isCombat)
        {
            SearchTarget();
        }
        else
        {
            Beheavior();
        }
    }

    private void SearchTarget()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players)
        {
            if (Vector3.Distance(player.transform.position, transform.position) <= 50f)
            {
                target = player.GetComponent<PETController>();
                    
                if (animator.GetBool(animationSleep))
                    animator.SetBool(animationSleep, false);

                isCombat = true;
                break;
            }
        }        

        if(target == null)
        {
            animator.SetBool(animationSleep, true);
        }
    }

    private void Beheavior()
    {
        //
        if (Vector3.Distance(target.transform.position, transform.position) <= 3f)
        {
            animator.SetBool(animationMove, false);

            animator.SetTrigger(animationAttackRight);

            isPlayAnim = true;
        }
        //Move
        else
        {
            Vector3 direction = target.transform.position - transform.position;

            transform.rotation = Quaternion.LookRotation(direction);

            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);

            animator.SetBool(animationMove, true);
        }
    }

    public void OnHit(int damage) 
    {
        if (isDead)
            return;

        if (isOnHit)
            return;

        hp -= damage;

        animator.SetTrigger(animationHit);

        isPlayAnim = true;
        isOnHit = true;

        Debug.Log($"{hp}");

        if (hp <= 0)
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

    public void AttackEnd(int type)
    {
        isPlayAnim = false;

        if (type == 1)
        {
            isOnHit = false;
        }
        //Debug.LogError("AttackEnd");
    }
}
