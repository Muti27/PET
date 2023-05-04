using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAnim : MonoBehaviour
{
    [SerializeField] private BaseEnemyAI bossAI;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AttackEnd(string msg)
    {
        //bossAI.isAttack = false;
        //Debug.LogError("AttackEnd");
    }
}
