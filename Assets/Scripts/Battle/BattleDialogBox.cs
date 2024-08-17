using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BattleDialogBox : MonoBehaviour
{   
    //DialogのTextを取得して変更する   
    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject moveDetails;
     [SerializeField] GameObject moveSelector;

    [SerializeField] List<Text> moveTexts;
    [SerializeField] List<Text> actionTexts;

    [SerializeField] Text ppText;
    [SerializeField] Text typeText;
    [SerializeField] Text dialogText;

    [SerializeField] int letterPerSecond;//1文字あたりの時間

    [SerializeField] Color highlightColor;

    //変更するための関数
    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }

    //タイプ形式で文字を表示する
    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        foreach(char letter in dialog)
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f/letterPerSecond);
        }
    }

    //UIの表示/非表示

    //dialogTextの表示
    public void EnableDialogText(bool enabled)
    {
        dialogText.enabled = enabled;
    }

    //actionSelectorの表示
    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }

    //dialogTextの表示
    public void EnableMoveSelector(bool enabled)
    {
        moveSelector.SetActive(enabled);
        moveDetails.SetActive(enabled);
    }

    //選択中のアクションの色を変える
    public void UpdateActionSelection(int selectAction)
    {
        //selectActionが０の時はactionTexts[0]の色を青にし、それ以外を黒にする
        //selectActionが1の時はactionTexts[1]の色を青にし、それ以外を黒にする

        for(int i = 0; i < actionTexts.Count; i++)
        {
            if(selectAction == i)
            {
                actionTexts[i].color = highlightColor;
            }
            else
            {
                actionTexts[i].color = Color.black;
            }
        }
        
    }
}
