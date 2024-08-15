using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    //ポケモンが実際に使う時の技データ
    //技のマスターデータを持つ
    //使いやすいようにPPも持つ

    public MoveBase Base;
    public int PP {get;set;}

    public Move(MoveBase pBase)
    {
        Base = pBase;
        PP = pBase.PP;
    }
}
