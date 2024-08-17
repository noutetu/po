using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState
{
    START,
    PLAYERACTION,//行動選択
    PLAYERMOVE,//技選択
    ENEMYMOVE,
    BUSY,
}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleHud enemyHud;
    [SerializeField] BattleDialogBox dialogBox;

    int currentAction;// 0:Fight, 1:Run
    int currentMove;// 0:左上, 1:右上, 2:左下, 3:右下
    BattleState state;

    public void Start()
    {
        StartCoroutine(SetUpBattle());
    }

    IEnumerator SetUpBattle()
    {
        state = BattleState.START;
        //モンスターの生成と描画
        playerUnit.SetUp();
        enemyUnit.SetUp();
        //Hudの描画
        playerHud.SetData(playerUnit.Pokemon);
        enemyHud.SetData(enemyUnit.Pokemon);
        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);

        yield return dialogBox.TypeDialog
        ($"A wild {enemyUnit.Pokemon.Base.Name} appeard");

        yield return new WaitForSeconds(1);
        PlayerAction();
    }

    private void PlayerAction()
    {
        state = BattleState.PLAYERACTION;
        dialogBox.EnableActionSelector(true);

        StartCoroutine(dialogBox.TypeDialog("Choose an action"));
    }

    private void PlayerMove()
    {
        Debug.Log("playermove");
        state = BattleState.PLAYERMOVE;
        dialogBox.EnableDialogText(false);
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableMoveSelector(true);

    }

    private void Update()
    {
        if(state == BattleState.PLAYERACTION)
        {
            HundleActionSelection();
        }
        else if(state == BattleState.PLAYERMOVE)
        {
            HundleMoveSelection();
        }
    }

    public void HundleActionSelection()
    {

        //下を入力するとRun,上を入力するとFightになる
        // 0:Fight, 1:Run
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentAction < 1)
            {
                currentAction++;
            }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentAction > 0)
            {
                currentAction--;
            }
        }

        //色をつけてどちらを選んでいるか管理する

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                PlayerMove();
            }
        }
    }

    public void HundleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentMove < playerUnit.Pokemon.Moves.Count -1)
            {
                currentMove++;
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentMove > 0)
            {
                currentMove--;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentMove < playerUnit.Pokemon.Moves.Count -2)
            {
                currentMove += 2;
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentMove > 1)
            {
                currentMove -=2;
            }
        }
        dialogBox.UpdateMoveSelection(currentMove,playerUnit.Pokemon.Moves[currentMove]);
    }
}
