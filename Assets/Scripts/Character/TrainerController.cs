using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerController : MonoBehaviour
{
    [SerializeField] Character character;
    [SerializeField] GameObject exclamation;

    private void Awake() {
        character = GetComponent<Character>();
    }

    //トレーナーバトルを開始する；プレイヤーが視界に入った時
    public IEnumerator TriggerTrainerBattle(PlayerController player)
    {
        exclamation.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclamation.SetActive(false);
        var diff =  player.transform.position - transform.position;
        var MoveVec = diff - diff.normalized;//playerとtrainerの差
        MoveVec = new Vector2(Mathf.Round(MoveVec.x), Mathf.Round(MoveVec.y));
        yield return character.Move(MoveVec);
    }
}
