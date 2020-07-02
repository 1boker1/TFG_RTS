using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOverMenu;

	private void Start()
	{
		GameObjectives.OnObjectivesCompleted+=EnableGameOver;
	}

	public void EnableGameOver()
	{
		if(gameOverMenu!=null)
		{
			if(SelectionManager.Instance!=null)
				SelectionManager.Instance.DeselectAll();
			Time.timeScale=0;
			gameOverMenu.SetActive(true);
		}
	}

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
		EnablePauseMenu(false);

		Time.timeScale=1;

		Fade.Instance.AddEventOnEndFade(()=>{SceneManager.LoadScene(0);Fade.Instance.FadeOut();});
		Fade.Instance.FadeIn();
    }
}
