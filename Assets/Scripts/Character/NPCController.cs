using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour,Iinteractable
{
    Character character;
    [SerializeField] Dialog dialog;
    NPCState state;
    float idleTimer;//アイドル状態の時間
    [SerializeField] float timeBetweenPattern;

    [SerializeField] List<Vector2> movePattern;
    int currentPattern;

    private void Awake() {
        currentPattern = 0;
        state = NPCState.Idle;
        character = GetComponent<Character>();
    }
    void Update()
    {
        
        //一定間隔で右に移動
        if(state == NPCState.Idle)
        {
            idleTimer += Time.deltaTime;
            if(idleTimer > timeBetweenPattern)
            {
                idleTimer = 0;
                StartCoroutine(Walk());
            }
        }
        character.HundleUpdate();
    }

    IEnumerator Walk()
    {
        state = NPCState.Walk;
        yield return character.Move(movePattern[currentPattern]);
        currentPattern = (currentPattern + 1) % movePattern.Count;
        state = NPCState.Idle;
    }

    //話しかけられた時に実行
    public void Interact()
    {
       if(state == NPCState.Idle)
       {
            state = NPCState.Dialog;
            StartCoroutine(DialogManager.Instance.ShowDialog(dialog,OnDialogFinished));

    }

    void OnDialogFinished()
    {
        state = NPCState.Idle;
    }
}

public enum NPCState
{
    Idle,
    Walk,
    Dialog,
}
}