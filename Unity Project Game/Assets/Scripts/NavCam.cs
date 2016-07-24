using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class NavCam : MonoBehaviour {
	public float widthFieldGame = 200f;
	public float heightFieldGame = 200f;
	public Bat scriptBat;
	public static float wdFieldGame;
	public static float htFieldGame;
	public bool isStart = false;
	public static bool isPause = false;
	public bool isConnect = false;
	public bool isConnectDialog = false;

	private string serverIP = "IP:localhost";
	private int prevLevel = 1;
	private AudioSource menuAudioSource;
	private AudioSource soundAudioSource;
	private bool isMute = false;

	void Start () {
		wdFieldGame = widthFieldGame;
		htFieldGame = heightFieldGame;

		menuAudioSource = (AudioSource)gameObject.AddComponent<AudioSource> ();
		menuAudioSource.clip = (AudioClip) Resources.Load ("Audio/Menu");
		menuAudioSource.loop = true;
		menuAudioSource.Play ();

		int r = Random.Range (0, 10);
		soundAudioSource = (AudioSource)gameObject.AddComponent<AudioSource> ();
		soundAudioSource.clip = (AudioClip) Resources.Load ("Audio/Sound"+(r >= 5 ? "1" : "2"));
		soundAudioSource.loop = true;
		//soundAudioSource.Play ();
	}

	void Update () {
		if (Bat.level != prevLevel)
		{
			prevLevel = Bat.level;
			soundAudioSource.pitch += 0.1f;
			if(Bat.level == 1) soundAudioSource.pitch = 1;
		}

		if (Input.GetKey (KeyCode.R)) {
			SceneManager.LoadScene(SceneManager.GetSceneAt(0).name);
		}
	}

	void OnGUI()
	{
		float menuX = 100f;
		float menuY = 80f;
		float width = 100f;

		menuX = (Screen.width - width) / 2;
		menuY = (Screen.height - 60) / 2;

		if (isStart) {
			if (GUI.Button (new Rect (0, 0, width, 30F), "Pause")) {
				isPause = !isPause;
				if (Time.timeScale == 0) {
					Time.timeScale = 1;
				} else {				
					Time.timeScale = 0;
				}
			}
			if (GUI.Button (new Rect (0, 30, width, 30F), (isMute ? "SoundOn" : "Mute"))) {
				isMute = !isMute;
				soundAudioSource.volume = (isMute ? 0 : 1);
			}
		}

		if (!isStart && !isConnectDialog && !isConnect) {
			if (GUI.Button (new Rect (menuX, menuY, width, 30F), "Start")) {
				CreateServer ();
			}
			if (GUI.Button (new Rect (menuX, menuY + 30, width, 30F), "Connect")) {
				CreateConnectDialog ();
			}
			if (GUI.Button (new Rect (menuX, menuY + 60, width, 30F), "Exit")) {
				Application.Quit ();
			}
		}

		if (isConnectDialog) {
			serverIP = GUI.TextField (new Rect (menuX, menuY, 200, 30), serverIP, 20);
			if (GUI.Button (new Rect (menuX, menuY + 30, 100f, 30F), "Connect")) {
				CreateConnect ();
			}
		}
	}

	// Start & play game
	void CreateServer() 
	{
		isStart = true;
		Time.timeScale = 1;
		Bat.level = 1;
		scriptBat.StartGame ();
		scriptBat.NewBall (true);
		scriptBat.StartServer ();
		menuAudioSource.Stop ();
		soundAudioSource.Play ();
	}

	// Create & connect to serverIP
	void CreateConnectDialog()
	{
		isConnectDialog = true;
		Time.timeScale = 1;
	}
	void CreateConnect()
	{
		isConnect = true;
		isConnectDialog = false;
		Time.timeScale = 1;
		serverIP = serverIP.Replace ("IP:", "");
		scriptBat.StartConnect(serverIP);
		scriptBat.NewBall (true);
		menuAudioSource.Stop ();
		soundAudioSource.Play ();
	}

	public static void EndGame()
	{
		SceneManager.LoadScene(SceneManager.GetSceneAt(0).name);
	}
}
