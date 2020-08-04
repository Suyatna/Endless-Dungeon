using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShopKeeper : MonoBehaviour {

	public GameObject m_UICanvas;
	public String[] itemName;
	public int[] itemCost;
	private Player m_player;
	private int m_gemCount;
	private int currentSellection;
	private int currentItemCost;

	void Awake()
	{
		if (m_UICanvas.activeSelf) 
		{
			m_UICanvas.SetActive(false);
		}

	}

	void OnTriggerEnter2D(Collider2D other)
	{
		m_player = other.gameObject.GetComponent<Player>();

		if (m_player != null) {
			m_gemCount = m_player.Diamonds;
			m_UICanvas.SetActive(true);
			
			UIManager.Instance.UI_UpDateGems(UIManager.Instance.playerGemCountText, m_gemCount);
			
			m_player.canAttack = false;
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		m_player = other.gameObject.GetComponent<Player>();

		if (m_player != null) 
		{
			m_UICanvas.SetActive(false);
			
			m_player.canAttack = true;
		}
	}
}
