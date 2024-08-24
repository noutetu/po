using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Iinteractableを継承するクラスは必ずInteract関数を持っている
public interface Iinteractable 
{
    //関数を宣言する
    public void Interact(Vector3 initiator);
}
