using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CharacterController : MonoBehaviour {
	public float maxSpeed = 10f;
	public float incrementCoeff = 0.05f;
	public float leftBorder = -20;
	public float rightBorder = 20;

	AudioSource bounceAudioSource;
	Rigidbody2D rg;
	void Awake () {
		rg = GetComponent<Rigidbody2D>();
		if (Network.isServer) rg.velocity = new Vector2 (Random.Range(-3,3) + Random.value, rg.velocity.y);

		bounceAudioSource = (AudioSource)gameObject.AddComponent<AudioSource> ();
		AudioClip clip = (AudioClip) Resources.Load ("Audio/bounce");
		bounceAudioSource.clip = clip;
	}

	void FixedUpdate()
	{
		if (!NavCam.isPause && Network.isServer) {
			Vector3 target = new Vector3 (transform.position.x, transform.position.y - maxSpeed, transform.position.z);
			float step = incrementCoeff * Bat.level;
			transform.position = Vector3.MoveTowards (transform.position, target, step);

			if (transform.position.x <= leftBorder) { 
				rg.velocity = new Vector2 (-rg.velocity.x, rg.velocity.y);
				bounceAudioSource.Play ();
			}
			if (transform.position.x >= rightBorder) {
				rg.velocity = new Vector2 (-rg.velocity.x, rg.velocity.y);
				bounceAudioSource.Play ();
			}
		}
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.name == "Bottom" && Network.isServer) {
			EndGame ();
		}
	}

	void OnGUI()
	{
		//GUI.Box (new Rect (150, 0, 200, 30), " V:" + rg.velocity + " rg.X:"+rg.position.x);
	}

	void EndGame()
	{		
		NavCam.EndGame ();
	}
}


