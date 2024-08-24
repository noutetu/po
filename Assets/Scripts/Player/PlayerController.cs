using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class PlayerController : MonoBehaviour
{

    Animator animator;
    [SerializeField] float moveSpeed;
    [SerializeField] LayerMask solidObjectsLayer;//壁判定のレイヤー
    [SerializeField] LayerMask interactableLayer;//壁判定のレイヤー
    [SerializeField] LayerMask longGrassLayer;//草むら判定

    public UnityAction OnEncounted;

    bool isMoving;

    Vector2 input;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void HundleUpdate()
    {
        if (!isMoving)
        {
            //キーボードの入力方向に動く
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            //斜め移動の対策
            if (input.x != 0)
            {
                input.y = 0;
            }
            //入力があったら
            if (input != Vector2.zero)
            {
                //向きを変えたい
                animator.SetFloat("MoveX", input.x);
                animator.SetFloat("MoveY", input.y);
                Vector2 targetPos = transform.position;
                targetPos += input;
                if (IsWalkable(targetPos))
                {
                    StartCoroutine(Move(targetPos));
                }

            }
        }

        animator.SetBool("IsMoving", isMoving);
    }
    //コルーチンを使って徐々に目的に近づける
    IEnumerator Move(Vector3 targetPos)
    {
        //移動中は入力を受け付けない
        isMoving = true;
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
        isMoving = false;
        CheckForEncounters();
    }
    //targetPosに移動可能か調べる関数
    bool IsWalkable(Vector2 targetPos)
    {
        //targetPosに半径0.2の円のrayを飛ばして、ぶつからなかったらtrue
        return !Physics2D.OverlapCircle(targetPos, 0.05f, solidObjectsLayer|interactableLayer);
    }

    //自分の場所から円のRayを飛ばして、草むらに当たったらランダムエンカウント
    void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, longGrassLayer))
        {
            if (Random.Range(0, 100) < 10)
            {
                Debug.Log("野生のポケモンが現れた！！！");
                animator.SetBool("IsMoving", false);

                OnEncounted();
            }
        }
    }
}