using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject dialog_Setting;

    // Use this for initialization
    void Start()
    {
        // 绑定buttons event  
        Button[] buttons = GetComponentsInChildren<Button>(true);
        foreach (var item in buttons)
        {
            Button btn = item;
            btn.onClick.AddListener(delegate ()
            {
                this.OnClick(btn.gameObject);
            });
        }
    }

    public void OnClick(GameObject sender)
    {
        switch (sender.name)
        {
            case "Button_Start":
                break;
            case "Button_Load":
                break;
            case "Button_Setting":
                dialog_Setting.SetActive(true);
                break;
            case "Button_End":
                Application.Quit();
                break;
        }
    }
}