using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(InputSystem))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PETController : MonoBehaviour
{
    private InputSystem input;
    private CharacterController controller;
    private PlayerInput playerInput;    
    private Animator animator;

    [SerializeField] private GameObject pet;

    [SerializeField] private float moveSpeed = 1.0f;

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

        if (controller == null)
        {
            controller = gameObject.GetComponent<CharacterController>();
        }

        if (playerInput == null)
        {
            playerInput = gameObject.GetComponent<PlayerInput>();
            playerInput.camera = Camera.main;
        }
                
        animator = gameObject.GetComponentInChildren<Animator>();

        SetAnimatorID();
    }

    // Update is called once per frame
    void Update()
    {
        Attack();
        Roll();
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
            Debug.Log($"Attack");

            animator.SetTrigger(animationAttack);

            input.isAttack = false;
        }
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

    private void Move()
    {
        if (input.move != Vector2.zero)
        {
            Debug.Log($"{input.move}");
            pet.transform.rotation = Quaternion.LookRotation(new Vector3(input.move.x, 0, input.move.y));
            controller.Move(new Vector3(input.move.x, 0, input.move.y) * moveSpeed * Time.deltaTime);

            animator.SetBool(animationMove, true);
        }
        else
        {
            animator.SetBool(animationMove, false);
        }
    }
}
