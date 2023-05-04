using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private Collider collider;
    private int attack = 10;

    private void Awake()
    {
        collider = GetComponent<Collider>();
    }

    public void SetColliderActive(bool active)
    {
        collider.enabled = active;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log($"{other.gameObject.name}");

            var hit = other.GetComponent<PETController>();
            hit.OnHit(attack);
        }

        if (other.tag == "Monster")
        {
            var hit = other.GetComponent<BaseEnemyAI>();
            hit.OnHit(attack);
        }
    }
}
