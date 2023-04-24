using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PETStatus : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI atkText;
    [SerializeField] private TextMeshProUGUI defText;

    public void UpdateInfo(PETData data, string name)
    {
        nameText.text = name;
        levelText.text = data.Lv.ToString();
        hpText.text = data.HP.ToString();
        atkText.text = data.Atk.ToString();
        defText.text = data.Def.ToString();
    }
}
