using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerController : MonoBehaviour, Iinteractable
{
    Character character;
    [SerializeField] GameObject exclamation;
    [SerializeField] Dialog dialog;
    [SerializeField] Dialog afterDialog;

    [SerializeField] GameObject view;
    [SerializeField] new string name;
    [SerializeField] Sprite sprite;

    public string Name { get => name; }
    public Sprite Sprite { get => sprite; }

    //戦闘終了したトレーナーとは戦わない
    bool battleLost;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Start()
    {
        SetViewRotation(character.Animator.DefaultDirection);
    }

    //トレーナーバトルを開始する；プレイヤーが視界に入った時
    public IEnumerator TriggerTrainerBattle(PlayerController player)
    {
        exclamation.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclamation.SetActive(false);
        var diff = player.transform.position - transform.position;
        var MoveVec = diff - diff.normalized;//playerとtrainerの差
        MoveVec = new Vector2(Mathf.Round(MoveVec.x), Mathf.Round(MoveVec.y));
        yield return character.Move(MoveVec);
        //話しかける
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog, StartBattle));
    }

    void StartBattle()
    {
        GameController.Instance.StartTrainerBattle(this);
    }

    private void Update() {
        character.HundleUpdate();
    }

    void SetViewRotation(faceDirection dir)
    {

        float angels = 0;
        switch (dir)
        {
            case faceDirection.Left:
                angels = -90;
                break;
            case faceDirection.Right:
                angels = 90;
                break;
            case faceDirection.Up:
                angels = 180;
                break;
            case faceDirection.Down:
                angels = 0;
                break;
        }
        view.transform.eulerAngles = new Vector3(0, 0, angels);
    }

    public void BattleLost()
    {
        battleLost = true;
        view.SetActive(false);
    }

    public void Interact(Vector3 initiator)
    {
        if (battleLost)
        {
            //こちらを向いて
            character.LookToward(initiator);
            StartCoroutine(DialogManager.Instance.ShowDialog(afterDialog));
            return;
        }
        //こちらを向いて
        character.LookToward(initiator);
        //話しかける
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog, StartBattle));
    }
}
