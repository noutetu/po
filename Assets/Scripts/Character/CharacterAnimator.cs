using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    //　Prameters
    public float MoveX { get; set; }
    public float MoveY { get; set; }
    public bool IsMoving { get; set; }

    //状態
    SpriteAnimator walkDownAnim;
    SpriteAnimator walkUpAnim;
    SpriteAnimator walkRightAnim;
    SpriteAnimator walkLeftAnim;

    SpriteAnimator currentAnim;

    bool wasPreviouslyMoving;

    //各状態のframes
    [SerializeField] List<Sprite> walkDownSprites;
    [SerializeField] List<Sprite> walkUpSprites;
    [SerializeField] List<Sprite> walkRightSprites;
    [SerializeField] List<Sprite> walkLeftSprites;
    [SerializeField] faceDirection defaultDirection;
    public faceDirection DefaultDirection { get => defaultDirection;}

    //SpriteRenderer
    SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        walkDownAnim = new SpriteAnimator(spriteRenderer, walkDownSprites);
        walkUpAnim = new SpriteAnimator(spriteRenderer, walkUpSprites);
        walkRightAnim = new SpriteAnimator(spriteRenderer, walkRightSprites);
        walkLeftAnim = new SpriteAnimator(spriteRenderer, walkLeftSprites);
        SetFaceDirection(defaultDirection);
        currentAnim = walkDownAnim;
    }

    private void Update()
    {
        SpriteAnimator prevAnim = currentAnim;

        if (MoveX == 1)
        {
            currentAnim = walkRightAnim;
        }
        else if (MoveX == -1)
        {
            currentAnim = walkLeftAnim;
        }
        else if (MoveY == 1)
        {
            currentAnim = walkUpAnim;
        }
        else if (MoveY == -1)
        {
            currentAnim = walkDownAnim;
        }

        //前のアニメーションと状態が違う,もしくは止まってる状態から動いたならstartを実行
        if (prevAnim != currentAnim || wasPreviouslyMoving != IsMoving)
        {
            currentAnim.Start();
        }

        if (IsMoving)
        {
            //動いてないなら
            currentAnim.HundleUpdate();
        }
        else
        {
            //動いてないなら最初のフレームを表示
            spriteRenderer.sprite = currentAnim.Frames[0];
        }
        wasPreviouslyMoving = IsMoving;
    }

    public void SetFaceDirection(faceDirection dir)
    {
        switch (dir)
        {
            case faceDirection.Left:
                MoveX = -1;
                break;
            case faceDirection.Right:
                MoveX = 1;
                break;
            case faceDirection.Up:
                MoveY = 1;
                break;
            case faceDirection.Down:
                MoveY = -1;
                break;
        }
    }
}

public enum faceDirection
{
    Up,
    Down,
    Left,
    Right,
}
