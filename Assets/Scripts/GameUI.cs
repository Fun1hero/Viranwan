using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {

	public Image fadePlane;
	public GameObject gameOverUI;
	
	void Start () {
		FindObjectOfType<Player>().onDeath += OnGameOver;
	}
	
	void OnGameOver (){
		gameOverUI.SetActive(true);
		StartCoroutine(Fade(Color.clear, Color.black, 1));
	}

	IEnumerator Fade (Color fromColor, Color toColor, float time){
		float speed = 1/time;
		float percent = 0;

		while(percent < 1){
			percent += Time.deltaTime * speed;
			fadePlane.color = Color.Lerp(fromColor, toColor, percent);
			yield return null;
		}
	}

	//UI

	public void StartNewGame (){
		UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
	}
}
