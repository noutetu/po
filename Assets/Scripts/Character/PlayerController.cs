using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class PlayerController : MonoBehaviour
{

    public UnityAction OnEncounted;
    public UnityAction<Collider2D> OnEnterTrainersView;

    Character character;

    Vector2 input;

    private void Awake()
    {
        character = GetComponent<Character>();
        
    }

    public void HundleUpdate()
    {
        if (!character.IsMoving)
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
                StartCoroutine(character.Move(input,OnMoveOver));
            }
        }
        character.HundleUpdate();
        if(Input.GetKeyDown(KeyCode.Z))
        {
            Interact();
        }
    }

    void Interact()
    {
        //向いてる方向
        Vector3 faceDirection = new Vector3(character.Animator.MoveX,character.Animator.MoveY);
        //鑑賞する場所
        Vector3 interactPos = transform.position + faceDirection;

        //鑑賞する場所にrayを飛ばす
        Collider2D collider2D =  Physics2D.OverlapCircle(interactPos, 0.3f,GameLayers.Instance.InteractableLayer);
        if(collider2D)
        {
            collider2D.GetComponent<Iinteractable>()?.Interact(transform.position);
        }

    }
    //targetPosに移動可能か調べる関数
    bool IsWalkable(Vector2 targetPos)
    {
        //targetPosに半径0.2の円のrayを飛ばして、ぶつからなかったらtrue
        return !Physics2D.OverlapCircle(targetPos, 0.05f, GameLayers.Instance.SolidObjectsLayer|GameLayers.Instance.InteractableLayer);
    }

    void OnMoveOver()
    {
        CheckForEncounters();
        CheckIfInTrainerView();
    }

    //自分の場所から円のRayを飛ばして、草むらに当たったらランダムエンカウント
    void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.3f, GameLayers.Instance.LongGrassLayer))
        {
            if (Random.Range(0, 100) < 10)
            {
                Debug.Log("野生のポケモンが現れた！！！");
                character.Animator.IsMoving = false;

                OnEncounted();
            }
        }
    }

    //移動先が、トレーナーの視界ならエンカウント
    void CheckIfInTrainerView()
    {
        Collider2D trainerCollider2D = Physics2D.OverlapCircle(transform.position, 0.3f, GameLayers.Instance.TrainerViewLayer);
        if (trainerCollider2D)
        {  
            Debug.Log("トレーナーの視界に入った");
            OnEnterTrainersView?.Invoke(trainerCollider2D);
        }
    }
}