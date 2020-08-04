using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(PlayerAnimation))]

public class Player : MonoBehaviour, IDamageable, ICollectable{

	[SerializeField]
	private int mDiamonds = 0;

	[Range(1,4)]
	[SerializeField]
	public int mHealth = 4;
	
	[SerializeField]
	private float mSpeed = 5f;
	
	[SerializeField]
	private float mJumpForce = 5f;
	
	[SerializeField]
	private LayerMask mGroundLayer;
	
	private Rigidbody2D _mRBody2D;
	private PlayerAnimation _mPlayerAnim;
	private SpriteRenderer _mPlayerSprite;
	private SpriteRenderer _mSwordArcSprite;

	private bool _mResetJump = false;
	private bool _mIsGrounded = false;
	private bool _mIsDead = false;

	public int Health { get; set;}
	public int Diamonds { get; set;}
	
	public bool canAttack = true;

	public int saveSceneIndex;

	// Use this for initialization
	void Start()
	{
		Health = mHealth;
		Diamonds = mDiamonds;
		Debug.Log("Player Health = " + Health.ToString());
		Debug.Log("Player Diamonds = " + Diamonds.ToString());
		
		_mRBody2D = GetComponent<Rigidbody2D>();
		_mPlayerAnim = GetComponent<PlayerAnimation>();
		_mPlayerSprite = GetComponentInChildren<SpriteRenderer>();
		_mSwordArcSprite = transform.GetChild(1).GetComponent<SpriteRenderer>();
		
		UIManager.Instance.UI_UpDateGems(UIManager.Instance.hudGemCountText, Diamonds);

	}
	
	// Update is called once per frame
	void Update()
	{
		Move();
		Attack();
	}

	void Move()
	{
		float move = Input.GetAxisRaw("Horizontal");
		
		_mIsGrounded = IsGrounded();

		// Flip Sprite in X if move is < 0
		if (move > 0) 
		{
			FlipSprite(true);
		}
		else if (move < 0) 
		{
			FlipSprite(false);
		}
		
		if (Input.GetKeyDown("space") && IsGrounded()) {

			_mRBody2D.velocity = new Vector2(_mRBody2D.velocity.x, mJumpForce);
			_mPlayerAnim.Jump(true);
			
			StartCoroutine(ResetJumpRoutine());
		}
		
		_mRBody2D.velocity = new Vector2(move * mSpeed, _mRBody2D.velocity.y);
		_mPlayerAnim.Move(move);
	}

	void Attack()
	{
		if (!canAttack) {
			return;
		}

		if(Input.GetMouseButtonDown(0) && IsGrounded()){
			_mPlayerAnim.Attack();
		}
	}

	void FlipSprite(bool faceRight)
	{
		if (_mPlayerSprite != null) 
		{
			// facing right
			if (faceRight == true) 
			{
				_mPlayerSprite.flipX = false;
				_mSwordArcSprite.flipX = false;
				_mSwordArcSprite.flipY = false;

				Vector3 newPos = _mSwordArcSprite.transform.localPosition;
				newPos.x = Mathf.Abs(newPos.x);
				_mSwordArcSprite.transform.localPosition = newPos;
			} 
			else
			{
				_mPlayerSprite.flipX = true;
				_mSwordArcSprite.flipX = false;
				_mSwordArcSprite.flipY = true;

				var transform1 = _mSwordArcSprite.transform;
				Vector3 newPos = transform1.localPosition;
				newPos.x = -0.8f;
				
				transform1.localPosition = newPos;
			}
		}
	}

	bool IsGrounded()
	{

		RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, Vector2.down, 1f, mGroundLayer);
		Debug.DrawRay(transform.position, Vector2.down, Color.green);
		
		if (hitInfo.collider != null) 
		{
			if (!_mResetJump) 
			{
				_mPlayerAnim.Jump(false);
				return true;
			}
		}

		return false;
	}

	IEnumerator ResetJumpRoutine()
	{
		_mResetJump = true;
		
		yield return new WaitForSeconds(0.2f);
		
		_mResetJump = false;
	}

	public void Damage()
	{
		if (_mIsDead)
		{
			return;
		}
		
		Health--;
		
		UIManager.Instance.UpDatePlayerHealth(Health);
		
		Debug.Log("Player Health = " + Health.ToString());
		
		// Check for dead
		if(Health < 1)
		{
			_mIsDead = true;
			
			_mPlayerAnim.Death();
			
			MenuManager.Manager.deadMenu.SetActive(true);
		}
	}

	public void AddDiamonds(int value)
	{
		Diamonds += value;
		UIManager.Instance.UI_UpDateGems(UIManager.Instance.hudGemCountText, Diamonds);
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Died"))
		{
			MenuManager.Manager.deadMenu.SetActive(true);
		}
		
		if (other.CompareTag("Gate"))
		{
			MenuManager.Manager.Loading(SceneManager.GetActiveScene().buildIndex + 1);
		}
		
		if (other.CompareTag("EndGate"))
		{
			MenuManager.Manager.Loading(4);
			MenuManager.Manager.finishMenu.SetActive(true);
		}
	}

	public void SaveGame()
	{
		SaveSystem.SavePlayer(this);
	}

	public void SaveSlot(string slot)
	{
		SaveSystem.SaveSlot = "/save" + slot + ".bel";
		SaveGame();
	}
}
