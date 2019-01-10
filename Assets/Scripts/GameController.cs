using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController _Instance;
    [HideInInspector]
    public Number[,] numbers = new Number[4, 4];//保存方格中数组 
    public GameObject NumberPrefab;
    private GameObject PlayBg;
    [HideInInspector]
    public List<Number> IsMovingNums = new List<Number>();//在移动中的number
    [HideInInspector]
    public bool HasMove = false;//是否有num发生移动
    [HideInInspector]
    public bool CanControl = false;
    public int Score = 0;
    public GameObject GameOver;
    [HideInInspector]
    public bool IsOver = false;
    public bool IsTough;
    private void Awake()
    {
        PlayBg = GameObject.Find("UI Root/Bg/PlayBg");
        _Instance = this;
    }
    private void Start()
    {
        for (int i = 0; i < 2; i++)
        {
            Instantiate(NumberPrefab);

        }
        CanControl = true;
        GameObject.Find("UI Root/Bg/ScoreSpr/HightestScore").GetComponent<UILabel>().text = PlayerPrefs.GetInt("HighScore") + "";
    }
    private void Update()
    {
        if (CanControl && IsMovingNums.Count == 0)
        {
            int x = 0, y = 0;
            //按键
            if (!IsTough)
            {
                if (Input.GetKeyDown("up"))
                {
                    y = 1;
                }
                if (Input.GetKeyDown("down"))
                {
                    y = -1;
                }
                if (Input.GetKeyDown("left"))
                {
                    x = -1;
                }
                if (Input.GetKeyDown("right"))
                {
                    x = 1;
                }
                if (x != 0 || y != 0)
                    MoveNumber(x, y);
            }
            // 触摸
            else if (IsTough)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _activeInput = true;
                    _mousePos = Input.mousePosition;
                }
                if (Input.GetMouseButton(0) && _activeInput)
                {
                    //计算鼠标的向量
                    //当前鼠标坐标减去鼠标按下时的坐标就可以得到鼠标滑动方向的向量
                    Vector3 vec = Input.mousePosition - _mousePos;
                    //判断向量长度，避免鼠标刚刚滑动就计算他的方向，给一定的长度能够准确判断鼠标滑动方向
                    if (vec.magnitude > 20)
                    {
                        //在然后Mathf.Rad2（弧度转为度数）
                        var angleY = Mathf.Acos(Vector3.Dot(vec.normalized, Vector2.up)) * Mathf.Rad2Deg;
                        var angleX = Mathf.Acos(Vector3.Dot(vec.normalized, Vector2.right)) * Mathf.Rad2Deg;
                        if (angleY <= 45)
                        {
                            y = 1;
                        }
                        else if (angleY >= 135)
                        {
                            y = -1;
                        }
                        else if (angleX <= 45)
                        {
                            x = 1;
                        }
                        else if (angleX >= 135)
                        {
                            x = -1;
                        }
                        _activeInput = false;
                    }
                }
                if (x != 0 || y != 0)
                    MoveNumber(x, y);
            }
           
        }
        if (HasMove && IsMovingNums.Count == 0)//每一次完成移动之后
        {
            Instantiate(NumberPrefab);
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (numbers[i, j])
                    {
                        numbers[i, j].hasMixed = false;
                    }
                }
            }
            HasMove = false;
            //判断游戏是否结束
            GameObject.Find("UI Root/Bg/ScoreSpr/NowScore").GetComponent<UILabel>().text = Score + "";
            if (Score > PlayerPrefs.GetInt("HighScore"))
                PlayerPrefs.SetInt("HighScore", Score);
            GameObject.Find("UI Root/Bg/ScoreSpr/HightestScore").GetComponent<UILabel>().text = PlayerPrefs.GetInt("HighScore") + "";
            CheckGameIsOver();

        }
    }
    public void MoveNumber(int dirX, int dirY)
    {
        if (dirX == 1)
        {
            //print("right");
            for (int y = 0; y < 4; y++)
            {
                for (int x = 2; x >= 0; x--)//最右边一列不移动
                {
                    if (numbers[x, y] != null)
                    {
                        numbers[x, y].Move(dirX, dirY);
                    }
                }
            }
        }
        if (dirX == -1)
        {
            //print("left");
            for (int y = 0; y < 4; y++)
            {
                for (int x = 1; x < 4; x++)//最left边一列不移动
                {
                    if (numbers[x, y] != null)
                    {
                        numbers[x, y].Move(dirX, dirY); ;
                    }
                }
            }
        }
        if (dirY == 1)
        {
            //print("up");
            for (int x = 0; x < 4; x++)
            {
                for (int y = 2; y >= 0; y--)//最up边一列不移动
                {
                    if (numbers[x, y] != null)
                    {
                        numbers[x, y].Move(dirX, dirY);
                    }
                }
            }
        }
        if (dirY == -1)
        {
            // print("down");
            for (int x = 0; x < 4; x++)
            {
                for (int y = 1; y < 4; y++)//最down边一列不移动
                {
                    if (numbers[x, y] != null)
                    {
                        numbers[x, y].Move(dirX, dirY);
                    }
                }
            }
        }
    }
    public bool IsEmpty(int x, int y)
    {
        if (x < 0 || x >= 4 || y < 0 || y >= 4)
        {
            return false;
        }
        else if (numbers[x, y] != null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public void Restart()//重新开始
    {
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                if (numbers[x, y] != null)
                {
                    Destroy(numbers[x, y].gameObject);
                    numbers[x, y] = null;
                }
            }
        }
        Instantiate(NumberPrefab);
        Instantiate(NumberPrefab);

        if (Score > PlayerPrefs.GetInt("HighScore"))
            PlayerPrefs.SetInt("HighScore", Score);
        Score = 0;
        GameObject.Find("UI Root/Bg/ScoreSpr/NowScore").GetComponent<UILabel>().text = Score + "";
    }
    public void GameWinAndOver(bool IsWin)
    {
        CanControl = false;
        GameOver.GetComponent<TweenAlpha>().PlayForward();

        if (IsWin)
        {
            GameOver.transform.Find("Label").GetComponent<UILabel>().text = "You Win!";
        }

        else
        {
            GameOver.transform.Find("Label").GetComponent<UILabel>().text = "You lose!";
        }
        GameOver.transform.Find("LabelScore").GetComponent<UILabel>().text = "" + Score;

        if (Score > PlayerPrefs.GetInt("HighScore"))
            PlayerPrefs.SetInt("HighScore", Score);
        Score = 0;

    }
    public void Continue()//继续游戏
    {
        CanControl = true;
        GameOver.GetComponent<TweenAlpha>().PlayReverse();
        Restart();
    }
    public void CheckGameIsOver()//检查游戏是否结束
    {
        //int j = 0;
        //bool IsFull = false;
        //for (int x = 0; x < 4; x++)
        //{
        //    for (int y = 0; y < 3; y++)
        //    {
        //        j++;
        //        if (numbers[x,y]==null)
        //        {
        //            IsFull = false;
        //        }
        //    }
        //}
        print(PlayBg.transform.childCount);
        if (PlayBg.transform.childCount == 17)
        {
            bool IsSame = false;
            for (int x = 0; x < 4; x++)//4列
            {
                for (int y = 0; y < 3; y++)//3行
                {
                    if (numbers[x, y] && numbers[x, y + 1])
                    {
                        if (numbers[x, y].value == numbers[x, y + 1].value)
                        {
                            IsSame = true;
                        }
                    }
                }
            }
            print("四列三行" + IsSame);
            for (int x = 0; x < 3; x++)//三列
            {
                for (int y = 0; y < 4; y++)//四行
                {
                    if (numbers[x, y] && numbers[x + 1, y])
                    {
                        if (numbers[x, y].value == numbers[x + 1, y].value)
                        {
                            IsSame = true;
                        }
                    }
                }
            }
            print("三列四行" + IsSame);
            if (IsSame == false)
            {
                //游戏结束！
                print("游戏结束");
                GameWinAndOver(false);
            }
        }
    }

    bool _activeInput;
    Vector3 _mousePos;

}

