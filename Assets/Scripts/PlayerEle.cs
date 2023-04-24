using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerEle : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;

    public void UpdateInfo(string name)
    {
        nameText.text = name;
    }
}
