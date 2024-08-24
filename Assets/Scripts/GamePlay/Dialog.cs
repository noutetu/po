using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//会話ログを出せる物全てに使える
[System.Serializable]
public class Dialog
{
    [SerializeField] List<string> lines;
    public List<string> Lines { get => lines;}
}
