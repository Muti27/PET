using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PETData
{
    private int lv;
    private int hp;
    private int atk;
    private int def;

    public int Lv { get { return lv; } }
    public int HP { get { return hp; } set { hp = value; } }
    public int Atk { get { return atk; } }
    public int Def { get { return def; } }

    public PETData()
    {
        lv = 1;
        hp = 100;
        atk = 10;
        def = 5;
    }
}
