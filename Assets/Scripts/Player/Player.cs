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
	// variable for move speed
	[SerializeField]
	private float mSpeed = 5f;
	// variable for jump force
	[SerializeField]
	private float mJumpForce = 5f;
	// layer of ground objects used for detection
	[SerializeField]
	private LayerMask mGroundLayer;

	// get handle for rigidbody
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

	public int sceneIndexLoad;

	// Use this for initialization
	void Start()
	{
		Health = mHealth;
		Diamonds = mDiamonds;
		Debug.Log("Player Health = " + Health.ToString());
		Debug.Log("Player Diamonds = " + Diamonds.ToString());
		
		// assign handle of rigidbody
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
		
		// horizontal input for left/right
		float move = Input.GetAxisRaw("Horizontal");

		// check if player is grounded
		_mIsGrounded = IsGrounded();

		// Flip Sprite in X if move is < 0
		if (move > 0) {
			FlipSprite(true);
		} else 
		if (move < 0) {
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
			if (faceRight == true) {
				_mPlayerSprite.flipX = false;
				_mSwordArcSprite.flipX = false;
				_mSwordArcSprite.flipY = false;

				Vector3 newPos = _mSwordArcSprite.transform.localPosition;
				newPos.x = Mathf.Abs(newPos.x);
				_mSwordArcSprite.transform.localPosition = newPos;

				// faceing left
			} 
			else if (faceRight == false) 
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
		if (hitInfo.collider != null) {
			if (!_mResetJump) {
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
		
		//Deduct 1 from health
		Health--;
		
		//Update UI Health
		UIManager.Instance.UpDatePlayerHealth(Health);
		
		Debug.Log("Player Health = " + Health.ToString());
		
		//Check for dead
		if(Health < 1)
		{
			_mIsDead = true;
			
			//Play death animation
			_mPlayerAnim.Death();
			
			GameplayManager.Manager.diedCanvas.SetActive(true);
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
			GameplayManager.Manager.diedCanvas.SetActive(true);
		}
		
		if (other.CompareTag("Gate"))
		{
			MenuManager.Manager.Loading(SceneManager.GetActiveScene().buildIndex + 1);
		}
		
		if (other.CompareTag("EndGate"))
		{
			MenuManager.Manager.Loading(0);
		}
	}

	private void SaveGame()
	{
		SaveSystem.SavePlayer(this);
	}

	private void LoadGame()
	{
		PlayerData data = SaveSystem.LoadPlayer();

		mHealth = data.health;
		sceneIndexLoad = data.sceneIndexLoad;
		
		MenuManager.Manager.Loading(sceneIndexLoad);

		Vector3 position;
		position.x = data.position[0];
		position.y = data.position[1];
		position.z = data.position[2];

		var transform1 = transform;
		transform1.position = position;

	}

	public void SaveSlot(string slot)
	{
		SaveSystem.SaveSlot = "/save" + slot + ".bel";
		SaveGame();
	}

	public void LoadSlot(string slot)
	{
		SaveSystem.LoadSlot = "/save" + slot + ".bel";
		LoadGame();
	}
}
