using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckFinishButton : MonoBehaviour
{
    private Button button;
    private Image image;

    private void Awake()
    {
        button = this.GetComponent<Button>();
        image = this.GetComponent<Image>();
    }

    public void Show(bool show)
    {
        button.enabled = show;
        image.color = show ? Color.white : Color.gray;
    }




    public void FinishButtonClicked()
    {
        SaveSystem.Instance.Save();
        WBTransition.SceneManager.LoadScene("Lobby");
    }
}
