using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{	public Animator animator;

	private string levelToLoad;
	

	public void FadeToLevel (string level)
	{
		levelToLoad = level;
		animator.SetTrigger("fadeOut");
	}

	public void OnFadeComplete ()
	{
		SceneManager.LoadScene(levelToLoad);
	}
}