using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] Text dialogText;
    [SerializeField] int letterPerSecond;

    public UnityAction OnshowDialog;
    public UnityAction OnCloseDialog;
    public UnityAction OnDialogFinished;
    bool isTyping;
    public bool IsShowing {get; private set;}

    public static DialogManager Instance { get ; private set ; }


    Dialog dialog;
    int currenLine = 0;

    private void Awake() {
        Instance = this;
    }
    //会話の表示をする
    public IEnumerator ShowDialog(Dialog dialog,UnityAction onFinished = null)
    {
        //フレーム終わりまで待つ
        OnDialogFinished = onFinished;
        yield return new WaitForEndOfFrame();
        IsShowing = true;
        OnshowDialog?.Invoke();
        dialogBox.SetActive(true);
        this.dialog = dialog;
        StartCoroutine(TypeDialog(dialog.Lines[currenLine]));
    }

    public void HundleUpdate()
    {
        if(Input.GetKeyDown(KeyCode.Z) && isTyping == false)
        {
            currenLine ++;
            if(currenLine < dialog.Lines.Count)
            {
                StartCoroutine(TypeDialog(dialog.Lines[currenLine]));   
            }
            else
            {
                IsShowing = false;
                currenLine = 0;
                dialogBox.SetActive(false);
                OnDialogFinished?.Invoke();
                OnCloseDialog?.Invoke();
            }
        }
    }

    IEnumerator TypeDialog(string lines)
    {
        isTyping = true;
        dialogText.text = "";
        foreach(char letter in lines.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f/letterPerSecond);
        }
        isTyping = false;
    }
}
