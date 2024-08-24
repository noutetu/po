using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] Text dialogText;
    [SerializeField] int letterPerSecond;

    public UnityAction OnshowDialog;
    public UnityAction CloseDialog;

    public static DialogManager Instance { get ; private set ; }

    private void Awake() {
        Instance = this;
    }
    //会話の表示をする
    public void ShowDialog(Dialog dialog)
    {
        OnshowDialog?.Invoke();
        dialogBox.SetActive(true);
        dialogText.text = dialog.Lines[0];
        StartCoroutine(TypeDialog(dialog.Lines[0]));
    }

    public void HundleUpdate()
    {

    }

    IEnumerator TypeDialog(string lines)
    {
        dialogText.text = "";
        foreach(char letter in lines.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f/letterPerSecond);
        }
    }
}
