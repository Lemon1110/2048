using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour {
	
    public void ShowHelpPanel()
    {
        GameObject.Find("UI Root/HelpPanel").GetComponent<TweenPosition>().PlayForward();
    }
    public void HelpPanelBack()
    {
        GameObject.Find("UI Root/HelpPanel").GetComponent<TweenPosition>().PlayReverse();
    }
    public void ShowStatisPanel()
    {
        GameObject.Find("UI Root/StatisPanel").GetComponent<TweenPosition>().PlayForward();
    }
    public void StatisPanelBack()
    {
        GameObject.Find("UI Root/StatisPanel").GetComponent<TweenPosition>().PlayReverse();
    }
    public void btnStartClick()
    {
        SceneManager.LoadScene(1);
    }
}
