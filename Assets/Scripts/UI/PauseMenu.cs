using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            EnablePauseMenu(!pauseMenu.activeSelf);
    }

    private void EnablePauseMenu(bool Enable)
    {
        if (Enable)
            SelectionManager.Instance.DeselectAll();

        pauseMenu.SetActive(Enable);

        Time.timeScale = Enable ? 0 : 1;
    }

    public void Resume()
    {
        EnablePauseMenu(false);
    }

    public void Options()
    {

    }

    public void Exit()
    {
		Time.timeScale=1;
		Fade.Instance.AddEventOnEndFade(()=>{SceneManager.LoadScene(0);Fade.Instance.FadeOut();});
		Fade.Instance.FadeIn();
    }
}
