using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
	//Singleton Class UIManager

	private static UIManager _instance;
	public static UIManager Instance{
		
		get{
			if(_instance == null){
				Debug.LogError("UI Manager is Null!");
			}
			return _instance;
		}
	}

	public Text playerGemCountText;
	public Text hudGemCountText;
	public Image[] healthBars;

	void Awake(){
		_instance = this;
	}

	//Updates the players gems count when they enter the shop
	public void UI_UpDateGems(Text UIEllement, int gemCount)
	{
		UIEllement.text = gemCount.ToString();

	}

	public void UpDatePlayerHealth(int livesRemaining){
		//loop through livesRemaining
		for (int i = 0; i <= livesRemaining; i++) {
			if (i == livesRemaining) {
				healthBars[i].enabled = false;
			} 
		}
	}
}
