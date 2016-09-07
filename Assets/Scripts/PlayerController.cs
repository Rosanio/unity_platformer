using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	[HideInInspector] public bool jump;

	public float moveForce = 365f;
	public float jumpForce = 1000f;
	public float maxSpeed = 5f;
	public Transform groundCheck;

	private bool grounded = false;
	private Rigidbody2D rb2d;

	void Awake () {
		rb2d = GetComponent<Rigidbody2D>();
	}

	void Update() {
		grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));

		if(Input.GetButtonDown("Jump") && grounded) {
			jump = true;
		}
	}

	void FixedUpdate() {
		float h = Input.GetAxis("Horizontal");

		if(h*rb2d.velocity.x < maxSpeed) {
			rb2d.AddForce(Vector2.right * h * moveForce);
		}

		if(Mathf.Abs(rb2d.velocity.x) > maxSpeed) {
			rb2d.velocity = new Vector2(Mathf.Sign(rb2d.velocity.x)*maxSpeed, rb2d.velocity.y);
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
	}
}
