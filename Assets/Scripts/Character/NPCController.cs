using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour,Iinteractable
{
    [SerializeField] Dialog dialog;
    //話しかけられた時に実行
    public void Interact()
    {
        DialogManager.Instance.ShowDialog(dialog);
    }
}
