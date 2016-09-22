using UnityEngine;
using System;

public class PlayerController : MonoBehaviour {

	[HideInInspector] public bool jump;
	[HideInInspector] public bool wallJump;

	public float moveForce;
	public float jumpForce;
	public float wallJumpingForce;
	public float aerialForce;
	public float maxSpeed;
	public float friction;
	public float maxSlideSpeed;
	public Transform[] groundChecks;
	public Transform wallCheckRight;
	public Transform wallCheckLeft;

	private bool grounded = false;
	private bool sliding = false;
	private bool groundedTranslucent;
	private bool slidingRight;
	private bool walljumping = false;
	private Rigidbody2D rb2d;
	private String[] groundTags = {"Ground", "Translucent"};
	private BoxCollider2D currentPlatformCollider;
	private BoxCollider2D ignoredPlatformCollider;
	private float ignorePlatformCollisionTime = 0.3f;
	private float startIgnorePlatformCollision;

	void Awake () {
		rb2d = GetComponent<Rigidbody2D>();
	}

	void Update() {
		for(int i = 0; i < groundChecks.Length; i++) {
			for(int j = 0; j < groundTags.Length; j++) {
				grounded = Physics2D.Linecast(transform.position, groundChecks[i].position, 1 << LayerMask.NameToLayer(groundTags[j]));
				if(grounded) {
					currentPlatformCollider = (BoxCollider2D) Physics2D.Linecast(transform.position, groundChecks[i].position, 1 << LayerMask.NameToLayer(groundTags[j])).collider;
					if(groundTags[j] == "Translucent") {
						groundedTranslucent = true;
					} else {
						groundedTranslucent = false;
					}
					break;
				} else {
					groundedTranslucent = false;
				}
			}
		}

		sliding = (Physics2D.Linecast(transform.position, wallCheckRight.position, 1 << LayerMask.NameToLayer("Wall")) || Physics2D.Linecast(transform.position, wallCheckLeft.position, 1 << LayerMask.NameToLayer("Wall"))) && !grounded;

		if(sliding) {
			slidingRight = Physics2D.Linecast(transform.position, wallCheckRight.position, 1 << LayerMask.NameToLayer("Wall"));
		}

		if(Input.GetButtonDown("Jump") && grounded) {
			jump = true;
		}
	}

	void FixedUpdate() {
		float h = 0;

		if(grounded || sliding) {
			walljumping = false;
		}

		h = Input.GetAxis("Horizontal");
		if(h*rb2d.velocity.x < maxSpeed) {
			if(walljumping) {
				rb2d.AddForce(Vector2.right * h * wallJumpingForce);
			} else if(!grounded) {
				rb2d.AddForce(Vector2.right * h * aerialForce);
			} else {
				rb2d.AddForce(Vector2.right * h * moveForce);
			}
		}

		if((Mathf.Abs(rb2d.velocity.x) > maxSpeed)) {
			rb2d.velocity = new Vector2(Mathf.Sign(rb2d.velocity.x)*maxSpeed, rb2d.velocity.y);
		}

		if(rb2d.velocity.y > maxSpeed) {
			rb2d.velocity = new Vector2(rb2d.velocity.x, maxSpeed);
		}

		if(sliding) {
			if(rb2d.velocity.y < -maxSlideSpeed ) {
				Vector2 vel = rb2d.velocity;
				vel.y = -maxSlideSpeed;
				rb2d.velocity = vel;
			}

			if(Input.GetButtonDown("Jump")) {
				if(slidingRight) {
					rb2d.AddForce(new Vector2(-jumpForce, jumpForce));
				} else {
					rb2d.AddForce(new Vector2(jumpForce, jumpForce));
				}
				walljumping = true;
			}
		}

		if(jump) {
			if(groundedTranslucent && Input.GetKey(KeyCode.DownArrow)) {
				ignoredPlatformCollider = currentPlatformCollider;
				startIgnorePlatformCollision = Time.time;
				Physics2D.IgnoreCollision(gameObject.GetComponent<BoxCollider2D>(), currentPlatformCollider, true);
			} else {
				rb2d.AddForce(new Vector2(0f, jumpForce));
			}
			jump = false;
		}

		if(ignoredPlatformCollider != null) {
			if(Time.time - startIgnorePlatformCollision > ignorePlatformCollisionTime) {
				Physics2D.IgnoreCollision(gameObject.GetComponent<BoxCollider2D>(), ignoredPlatformCollider, false);
				ignoredPlatformCollider = null;
			}
		}

		if(Mathf.Abs(h) < 0.75 && grounded) {
			Vector2 vel = rb2d.velocity;
			vel.x = rb2d.velocity.x * friction;
			rb2d.velocity = vel;
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		if(other.gameObject.layer == LayerMask.NameToLayer("Translucent")) {
			Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Translucent"), true);
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if(other.gameObject.layer == LayerMask.NameToLayer("Translucent")) {
			Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Translucent"), false);
		}
	}
}
