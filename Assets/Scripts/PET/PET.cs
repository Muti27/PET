using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PET : MonoBehaviour
{
    [SerializeField] private Transform statusTrans;

    private PETData petData;
    private string petName;

    private PETStatus petStatus;

    public string PetName { get { return petName; } }

    public void Init(string name)
    {
        petData = new PETData();
        petName = name;

        transform.localPosition = new Vector3(0, 0, -7);
        transform.localRotation = Quaternion.LookRotation(Camera.main.transform.position);
        transform.localScale = Vector3.one;

        var status = Instantiate(Resources.Load($"PETStatus"), UIManager.Instance.transform) as GameObject;
        petStatus = status.GetComponent<PETStatus>();
        petStatus.UpdateInfo(petData, petName);
    }

    private void Update()
    {
        if (petStatus != null)
        {
            petStatus.transform.position = Camera.main.WorldToScreenPoint(statusTrans.position);
        }
    }
}


