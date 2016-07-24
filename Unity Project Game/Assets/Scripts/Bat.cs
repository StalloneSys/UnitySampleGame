using UnityEngine;
using System.Collections;

public class Bat : MonoBehaviour {
	public float maxSpeed = 10f;
	public GameObject ballObject;
	public NetworkViewScript networkScript;
	public AudioSource catchAS;
	public static int level = 1;
	public float move = 1;
	public int score = 0;
	public float leftBorder = -10;
	public float rightBorder = 150;
	public float seconds = 30;

	private NetworkView netView;
	private Rigidbody2D rg;
	private GameObject ball;
	private Animator batAnimator;
	private Animation batAnimation;
	private float secondsTmp;
	private bool isStartGame = false;
	private bool isConnectedProcess = false;
	private string checkStr = "1";
	private int count = 0;
	private float widthBat = 1;

	void Awake () {
		rg = GetComponent<Rigidbody2D>();
		widthBat = GetComponent<Collider2D> ().bounds.size.x;
		networkScript = GetComponent<NetworkViewScript> ();
		networkScript.rg = rg;
		leftBorder += widthBat / 2;
		rightBorder += widthBat / 2;

		secondsTmp = seconds;
		GetComponent<SpriteRenderer> ().enabled = false;

		catchAS = GetComponent<AudioSource>();
		catchAS.Stop ();

		batAnimator = GetComponent<Animator> ();
		batAnimation = GetComponent<Animation> ();
		batAnimator.enabled = false;
	}

	void FixedUpdate()
	{		
		if (Network.isServer) {
			if (isStartGame) {
				secondsTmp -= Time.deltaTime;
				if (secondsTmp <= 0) {
					level++;
					secondsTmp = seconds;
				}
			}

			move = Input.GetAxis ("Horizontal");			
			rg.velocity = new Vector2 (move * maxSpeed, rg.velocity.y);
			if (rg.position.x < leftBorder)
				rg.position = new Vector2 (leftBorder, rg.position.y);
			if (rg.position.x + widthBat > rightBorder)
				rg.position = new Vector2 (rightBorder - widthBat, rg.position.y);
		}
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.tag == "Ball") {
			Destroy (col.gameObject);
			NewBall ();
		}
	}

	public void NewBall(bool isRoot = false)
	{
		if (!isRoot) {
			CatchBall ();		
			networkScript.SetCallRPC ();
		}

		float minRange = -10f;
		float maxRange = minRange + NavCam.wdFieldGame;
		isStartGame = true;
		if (ballObject != null) {
			float newX = Random.Range(minRange, maxRange);
			float newY = 12;
			ball = (GameObject) Instantiate (ballObject, new Vector3(newX, newY, 0), Quaternion.identity);
			networkScript.rgBall = ball.GetComponent<Rigidbody2D>();
		}
	}

	[RPC]
	private void CatchBall()
	{
		score++;
		catchAS.Play ();
		batAnimator.enabled = true;
	}

	public void BatAnimationEndClip()
	{
		batAnimator.enabled = false;
	}

	void OnGUI()
	{
		GUI.Box (new Rect (Screen.width - 130, 0, 130, 25), "Score:" + score + " Level:"+level);
		//GUI.Box (new Rect (0, 30, 130, 30), "X:" + rg.position.x + " Y:"+rg.position.y);
		//GUI.Box (new Rect (0, 60, 130, 30), "W:" + Screen.width + " Y:"+ Screen.height);
		//if (isConnectedProcess)
		//	GUI.Box (new Rect (0, 240, 530, 30), "ser:" + Network.isServer + " c:"+count+ (isConnectedProcess ? " conProc":"") + " X:" + networkScript.syncEndPosition.x + " Y:" + networkScript.syncEndPosition.y + " X_BALL:" + networkScript.syncEndPositionBall.x + " Y_BALL:" + networkScript.syncEndPositionBall.y + " Check:"+checkStr);
		//GUI.Box (new Rect (0, 60, 130, 30), "X1:" + rg.transform + " Y:"+ Screen.height);
		//if(count > 0) 
		//	GUI.Box (new Rect (0, 280, 330, 30), "log:" + log);
		
		//if (isConnectedProcess && Network.peerType == NetworkPeerType.Connecting) {
		//	GUI.Box (new Rect (210, 60, 130, 30), "Connecting:" + NETWORK_SERVER_IP);
		//} else if (isConnectedProcess) {
		//	if (GUI.Button (new Rect (0f, 220f, 100f, 30F), "Disconnect")) {
		//		Debug.Log ("Click DISCONNECT button");
		//		CloseConnect ();
		//	}
		//}
	}

	public void StartGame()
	{
		GetComponent<SpriteRenderer> ().enabled = true;
	}

	public void StartServer ()
	{
		networkScript.StartServer ();
	}

	public void StartConnect(string serverIP)
	{
		isConnectedProcess = true;
		networkScript.StartConnect (serverIP);
	}

	public void CloseConnect()
	{
		isConnectedProcess = false;
		Network.Disconnect();
	}

}
