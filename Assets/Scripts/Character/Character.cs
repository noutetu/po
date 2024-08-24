using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    CharacterAnimator animator;
    public CharacterAnimator Animator { get => animator;}

    [SerializeField] float moveSpeed;

    public bool IsMoving { get; set; }

    //移動アニメーションを管理する

    void Awake()
    {
        animator = GetComponent<CharacterAnimator>();
    }



    //コルーチンを使って徐々に目的に近づける
    public IEnumerator Move(Vector2 moveVec,UnityAction OnMoveover = null)
    {
        //向きを変えたい
        animator.MoveX = moveVec.x;
        animator.MoveY = moveVec.y;
        Vector3 targetPos = transform.position;
        targetPos += (Vector3)moveVec;
        if (!IsWalkable(targetPos))
        {
            yield break;
        }
        //移動中は入力を受け付けない
        IsMoving = true;
        //targetPosとの差があるなら繰り返す。
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            //targetPosに近づける
            transform.position = Vector3.MoveTowards(
            transform.position,//現在の場所
            targetPos,//目的地
            moveSpeed * Time.deltaTime);

            yield return null;
        }

        transform.position = targetPos;
        IsMoving = false;
        OnMoveover?.Invoke();
    }
    
    public void HundleUpdate()
    {
        animator.IsMoving = IsMoving;
    }

    //targetPosに移動可能か調べる関数
    bool IsWalkable(Vector2 targetPos)
    {
        //targetPosに半径0.2の円のrayを飛ばして、ぶつからなかったらtrue
        return !Physics2D.OverlapCircle(targetPos, 0.05f, GameLayers.Instance.SolidObjectsLayer | GameLayers.Instance.InteractableLayer);
    }
}
