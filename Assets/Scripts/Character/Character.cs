using System.Collections;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    CharacterAnimator animator;
    public CharacterAnimator Animator { get => animator; }

    [SerializeField] float moveSpeed;

    public bool IsMoving { get; set; }

    //移動アニメーションを管理する

    void Awake()
    {
        animator = GetComponent<CharacterAnimator>();
    }



    //コルーチンを使って徐々に目的に近づける
    public IEnumerator Move(Vector2 moveVec, UnityAction OnMoveover = null)
    {
        //向きを変えたい
        animator.MoveX = Mathf.Clamp(moveVec.x, -1f, 1f);
        animator.MoveY = Mathf.Clamp(moveVec.y, -1f, 1f);
        Vector3 targetPos = transform.position;
        targetPos += (Vector3)moveVec;
        if (!isPathClear(targetPos))
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

    bool isPathClear(Vector3 targetPos)
    {
        Vector3 diff = targetPos - transform.position;
        Vector3 dir = diff.normalized;//正規化lk
        return Physics2D.BoxCast(transform.position + dir, new Vector2(0.2f, 0.2f), 0, dir, diff.magnitude - 1,
        GameLayers.Instance.SolidObjectsLayer | GameLayers.Instance.InteractableLayer | GameLayers.Instance.PlayerLayer) == false;
    }


    public void LookToward(Vector3 targetPos)
    {
        float xDiff = Mathf.Floor(targetPos.x) - Mathf.Floor(transform.position.x);
        float yDiff = Mathf.Floor(targetPos.y) - Mathf.Floor(transform.position.y);
        //向きを変えたい

        if (xDiff == 0 || yDiff == 0)
        {

        }

        animator.MoveX = Mathf.Clamp(xDiff, -1f, 1f);
        animator.MoveY = Mathf.Clamp(yDiff, -1f, 1f);
    }
}