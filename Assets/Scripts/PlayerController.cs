using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	[HideInInspector] public bool jump;
	[HideInInspector] public bool wallJump;

	public float moveForce;
	public float jumpForce;
	public float maxSpeed;
	public float friction;
	public float maxSlideSpeed;
	public Transform groundCheck1;
	public Transform groundCheck2;
	public Transform wallCheck;

	private bool grounded = false;
	private bool sliding = false;
	private Rigidbody2D rb2d;

	void Awake () {
		rb2d = GetComponent<Rigidbody2D>();
	}

	void Update() {
		grounded = Physics2D.Linecast(transform.position, groundCheck1.position, 1 << LayerMask.NameToLayer("Ground")) || Physics2D.Linecast(transform.position, groundCheck2.position, 1 << LayerMask.NameToLayer("Ground"));

		sliding = Physics2D.Linecast(transform.position, wallCheck.position, 1 << LayerMask.NameToLayer("Wall"));

		if(Input.GetButtonDown("Jump") && grounded) {
			jump = true;
		}

		// if(Input.GetButtonDown("Jump") && sliding) {
		// 	Debug.Log("wall jumping");
		// 	wallJump = true;
		// }
	}

	void FixedUpdate() {
		float h = Input.GetAxis("Horizontal");

		if(h*rb2d.velocity.x < maxSpeed) {
			rb2d.AddForce(Vector2.right * h * moveForce);
		}

		if((Mathf.Abs(rb2d.velocity.x) > maxSpeed) && grounded) {
			rb2d.velocity = new Vector2(Mathf.Sign(rb2d.velocity.x)*maxSpeed, rb2d.velocity.y);
		}

		if(sliding) {
			if(rb2d.velocity.y < -maxSlideSpeed ) {
				Vector2 vel = rb2d.velocity;
				vel.y = -maxSlideSpeed;
				rb2d.velocity = vel;
			}
			//Add different options for wall jumping based on player input, such as in platformer tutorial video
			if(Input.GetButtonDown("Jump")) {
				rb2d.AddForce(new Vector2(-2*jumpForce, jumpForce));

			}
		}

		if(jump) {
			rb2d.AddForce(new Vector2(0f, jumpForce));
			jump = false;
		}

		if(rb2d.velocity.y > 0) {
			Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Ground"), true);
		} else {
			Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Ground"), false);
		}

		// if(rb2d.velocity.y == 0) {
		// 	if(!grounded) {
		// 		sliding = true;
		// 	} else {
		// 		sliding = false;
		// 	}
		// }

		if(Mathf.Abs(h) < 0.75) {
			Vector2 vel = rb2d.velocity;
			vel.x = rb2d.velocity.x * friction;
			rb2d.velocity = vel;
		}
	}
}
