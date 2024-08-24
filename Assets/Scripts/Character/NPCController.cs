using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour,Iinteractable
{
    [SerializeField] string text;
    //話しかけられた時に実行
    public void Interact()
    {
        Debug.Log(text);
    }
}
