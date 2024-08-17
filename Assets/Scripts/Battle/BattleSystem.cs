using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;    
    [SerializeField] BattleUnit enemyUnit;    
    [SerializeField] BattleHud playerHud;    
    [SerializeField] BattleHud enemyHud;   
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField]


    public void Start()
    {
        StartCoroutine(SetUpBattle());
    }

    IEnumerator SetUpBattle()
    {
        //モンスターの生成と描画
        playerUnit.SetUp();
        enemyUnit.SetUp();
        //Hudの描画
        playerHud.SetData(playerUnit.Pokemon);
        enemyHud.SetData(enemyUnit.Pokemon);
        
        yield return dialogBox.TypeDialog
        ($"A wild {enemyUnit.Pokemon.Base.Name} appeard");

        yield return new WaitForSeconds(1);
        dialogBox.EnableActionSelector(true);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableDialogText(false);
            dialogBox.EnableActionSelector(false);
            dialogBox.EnableMoveSelector(true);
        }
    }
}
