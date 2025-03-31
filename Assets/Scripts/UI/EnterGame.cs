using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnterGame : MonoBehaviour
{
    public GameObject select;
    public GameObject next;
    public Button exitBtn;
    Animator click;
    Button nextBtn;
    

    void Start()
    {
        click = next.GetComponent<Animator>();
        nextBtn = next.GetComponent<Button>();//��ð�ť���
        nextBtn.onClick.AddListener(Clicked);//����¼�
        exitBtn.onClick.AddListener(Exit);
    }

    public void Clicked()
    {
        click.Play("start");
        AnimatorStateInfo info = click.GetCurrentAnimatorStateInfo(0);
        Invoke("Next", info.length);
    }

    void Next()
    {
        gameObject.SetActive(false);
        select.SetActive(true);
    }

    void Exit()
    {
        Debug.Log("exit");
        Application.Quit();
    }
}
