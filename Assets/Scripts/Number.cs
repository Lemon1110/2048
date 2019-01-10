using UnityEngine;

public class Number : MonoBehaviour
{
    public int value;//数字值
    public int positionx;//在方格中的位置 横 
    public int positiony;//纵
                         /*  0 1 2 3
                          *  0 1 2 3
                          *  0 1 2 3
                          *  0 1 2 3 (0,0)(0,1)(0,2)(0,3)
                          * */
    public TweenPosition Tp;
    bool IsMoving = false;//数字是否在移动
    bool ToDestory;//数字是否要被销毁
    public    bool hasMixed=false;//数字是否完成过合成
    public AudioSource AudioCombine;//合成音效
    private void Awake()
    {
        AudioCombine = GameObject.Find("Main Camera").GetComponent<AudioSource>();
    }
    void Start()
    {
        Tp = GetComponent<TweenPosition>();
        do
        {
            positiony = Random.Range(0, 4);
            positionx = Random.Range(0, 4);
        }
        while (GameController._Instance.numbers[positionx, positiony] != null);
        transform.parent = GameObject.Find("UI Root/Bg/PlayBg").transform;
        transform.localPosition = new Vector2(-300 + positionx * 200, -300 + positiony * 200);//对应（0，0）位置
        transform.localScale = Vector3.one;
        GameController._Instance.numbers[positionx, positiony] = this;
        int k = Random.Range(1, 3);
        value = 2 * k;
        GetComponent<UISprite>().spriteName = k * 2 + "";
        Tp.from = new Vector2(-300 + positionx * 200, -300 + positiony * 200);
        Tp.to = new Vector2(-300 + positionx * 200, -300 + positiony * 200);
    }

    // Update is called once per frame
    void Update()
    {

        if (!IsMoving)
        {
            if (transform.localPosition != new Vector3(-300 + positionx * 200, -300 + positiony * 200, 0))
            {
                IsMoving = true;
                Tp.from = transform.localPosition;
                Tp.to = new Vector3(-300 + positionx * 200, -300 + positiony * 200, 0);
                Tp.ResetToBeginning();
                Tp.PlayForward();

            }
        }                  
    }

    public void Move(int x, int y)
    {
       
        if (x == 1)//向右
        {
            //print("right");
            int Index = 1;
            while (GameController._Instance.IsEmpty(positionx + Index, positiony))//判断右边格子为不为空
            {
                Index++;
            }
            if (Index > 1)//右边格子为空
            {
                if (!GameController._Instance.IsMovingNums.Contains(this))
                {
                    GameController._Instance.IsMovingNums.Add(this);//正在移动中的对象添加，用来播放动画
                }
                GameController._Instance.HasMove = true;//已经移动过了
                GameController._Instance.numbers[positionx, positiony] = null;//当前位置设为空
                positionx = positionx + Index - 1;
                GameController._Instance.numbers[positionx, positiony] = this;//对象转换
            }

            if (positionx < 3 && value == GameController._Instance.numbers[positionx + 1, positiony].value&& 
                !GameController._Instance.numbers[positionx + 1, positiony].hasMixed)//判断是否左右方向上相邻是否相等
            {
                AudioCombine.Play();//播放声音
                if (!GameController._Instance.IsMovingNums.Contains(this))
                {
                    GameController._Instance.IsMovingNums.Add(this);//正在移动中的对象添加，用来播放动画
                }
                  GameController._Instance.numbers[positionx + 1, positiony].hasMixed = true;//已经合并过一次了
                GameController._Instance.HasMove = true;//已经移动过了
                ToDestory = true;//准备销毁对象物体
                GameController._Instance.numbers[positionx + 1, positiony].value *= 2;//相等则右边数值加倍
                GameController._Instance .Score += GameController._Instance.numbers[positionx + 1, positiony].value;//加分
                GameController._Instance.numbers[positionx, positiony] = null;//当前对象设为空
                positionx += 1;//当前位置加1,播放动画
            }
            /* 这一段的逻辑为
             * 例如，若从下往上第0行也就是positionx=0，从左往右的数字情况分别为2 2 0 4；(0代表为空)
             * x=1,执行往右移动 若当前对象numbers[0,1],则它的value为2，先判断右边为不为空，右边只有一个空，因此Index为2 执行第一个if,完成对象转换，
             * 同理numbers[0,0]也是如此，因此此时numbers二维数组的情况为0 2 2 4；
             * 完成对象转换之后，判断当前和当前的右边相邻数字是否相等，positionx=3时说明在最右边，因此positionx<3，相等则播放声音，实现合并，数值翻倍，
             * 当前的2与右边的4不相等，因此不合并,但对象numbers[0,1](上面对象转换过了)的2，与右边的2相等，因此完成合并操作
             * 然后完成移动，最终格子的情况为0 0 4 4
             * 同理，下面的向左、上、下情况类似
             */
        }
        else if (x == -1)//向左
        {
            // print("向左");
            int Index = 1;
            while (GameController._Instance.IsEmpty(positionx - Index, positiony))
            {
                Index++;
            }
            if (Index > 1)
            {
                GameController._Instance.HasMove = true;
                if (!GameController._Instance.IsMovingNums.Contains(this))
                {
                    GameController._Instance.IsMovingNums.Add(this);
                }
                GameController._Instance.numbers[positionx, positiony] = null;
                positionx = positionx - Index + 1;
                GameController._Instance.numbers[positionx, positiony] = this;
            }
            if (positionx > 0 && value == GameController._Instance.numbers[positionx - 1, positiony].value && !GameController._Instance.numbers[positionx - 1, positiony].hasMixed)
            {
                AudioCombine.Play();
                if (!GameController._Instance.IsMovingNums.Contains(this))
                {
                    GameController._Instance.IsMovingNums.Add(this);
                }
                GameController._Instance.numbers[positionx - 1, positiony].hasMixed = true;
                GameController._Instance.HasMove = true;
                ToDestory = true;
                GameController._Instance.numbers[positionx - 1, positiony].value *= 2;
                GameController._Instance.Score += GameController._Instance.numbers[positionx - 1, positiony].value;
                GameController._Instance.numbers[positionx, positiony] = null;
                positionx -= 1;
               
            }

        }
        else if (y == 1)//向上
        {
            // print("向上");
            int Index = 1;
            while (GameController._Instance.IsEmpty(positionx, positiony + Index))//判断上方格子是否为空
            {
                Index++;
            }
            if (Index > 1)//格子为空
            {
                if (!GameController._Instance.IsMovingNums.Contains(this))
                {
                    GameController._Instance.IsMovingNums.Add(this);
                }
                GameController._Instance.HasMove = true;
                GameController._Instance.numbers[positionx, positiony] = null;
                positiony = positiony + Index - 1;
                GameController._Instance.numbers[positionx, positiony] = this;
            }
            if (positiony < 3 && value == GameController._Instance.numbers[positionx, positiony + 1].value && !GameController._Instance.numbers[positionx , positiony + 1].hasMixed)
            {
                AudioCombine.Play();
                if (!GameController._Instance.IsMovingNums.Contains(this))
                {
                    GameController._Instance.IsMovingNums.Add(this);
                }
                GameController._Instance.numbers[positionx, positiony + 1].hasMixed = true;
                GameController._Instance.HasMove = true;
                ToDestory = true;
                GameController._Instance.numbers[positionx, positiony + 1].value *= 2;
                GameController._Instance.Score += GameController._Instance.numbers[positionx, positiony + 1].value;
                GameController._Instance.numbers[positionx, positiony] = null;
                positiony += 1;
                
            }

        }
        else if (y == -1)//向下
        {
            //print("向下");
            int Index = 1;
            while (GameController._Instance.IsEmpty(positionx, positiony - Index))//判断xia方格子是否为空
            {
                Index++;
            }
            if (Index > 1)//格子为空
            {
                if (!GameController._Instance.IsMovingNums.Contains(this))
                {
                    GameController._Instance.IsMovingNums.Add(this);
                }
                GameController._Instance.HasMove = true;
                GameController._Instance.numbers[positionx, positiony] = null;
                positiony = positiony - Index + 1;
                GameController._Instance.numbers[positionx, positiony] = this;
            }
            if (positiony > 0 && value == GameController._Instance.numbers[positionx, positiony - 1].value && !GameController._Instance.numbers[positionx , positiony -1].hasMixed)
            {
                AudioCombine.Play();
                if (!GameController._Instance.IsMovingNums.Contains(this))
                {
                    GameController._Instance.IsMovingNums.Add(this);
                }
                GameController._Instance.numbers[positionx, positiony - 1].hasMixed = true;
                GameController._Instance.HasMove = true;
                ToDestory = true;
                GameController._Instance.numbers[positionx, positiony - 1].value *= 2;
                GameController._Instance.Score += GameController._Instance.numbers[positionx, positiony - 1].value;
                GameController._Instance.numbers[positionx, positiony] = null;
                positiony -= 1;
               
            }
        }

    }
    public void MoveOver()//移动结束，给TweenPosition的OnFinished绑定这个方法
    {
        IsMoving = false;
        if (ToDestory)
        {
            Destroy(gameObject);
            GameController._Instance.numbers[positionx, positiony].GetComponent<UISprite>().spriteName =
                 GameController._Instance.numbers[positionx, positiony].value + "";
        }
        if(GameController._Instance.numbers[positionx, positiony].value == 2048)
        {
            GameController._Instance.GameWinAndOver(true);
        }

        GameController._Instance.IsMovingNums.Remove(this);


    }
}
