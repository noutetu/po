using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] LayerMask solidObjectsLayer;//壁判定のレイヤー
    [SerializeField] LayerMask interactableLayer;//壁判定のレイヤー
    [SerializeField] LayerMask longGrassLayer;//草むら判定
    [SerializeField] LayerMask playerLayer;//草むら判定


    //どこからでも使えるようにしたい
    public static GameLayers Instance {get;set;}

    public LayerMask SolidObjectsLayer {get => solidObjectsLayer;}
    public LayerMask InteractableLayer {get => interactableLayer;}
    public LayerMask LongGrassLayer {get => longGrassLayer;}
    public LayerMask PlayerLayer {get => playerLayer;}

    private void Awake()
    {
        Instance = this;
    }

}
