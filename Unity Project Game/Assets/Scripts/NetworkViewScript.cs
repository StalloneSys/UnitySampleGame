using UnityEngine;
using System.Collections;

public class NetworkViewScript : MonoBehaviour {
	public static string NETWORK_SERVER_IP = "localhost";
	public static int NETWORK_PORT = 4585;
	public Rigidbody2D rg;
	public Rigidbody2D rgBall;
	public Vector2 syncStartPosition = Vector2.zero;
	public Vector2 syncEndPosition = Vector2.zero;
	public Vector2 syncStartPositionBall = Vector2.zero;
	public Vector2 syncEndPositionBall = Vector2.zero;
	public int levelCurrent = 0;
	public float syncDelay = 0f;
	public float syncTime = 0f;

	float lastSynchronizationTime;
	NetworkView netView;
	string log = "";
	int count = 0;
	bool isConnectedProcess = false;

	void Awake () {
		netView = gameObject.AddComponent (typeof(NetworkView)) as NetworkView;
		netView.viewID = Network.AllocateViewID();
		netView.observed = this;
		netView.stateSynchronization = NetworkStateSynchronization.Unreliable;
	}

	public void SetCallRPC()
	{
		netView.RPC ("CatchBall", RPCMode.Others);
	}

	void OnGUI()
	{
		//if (count > 0)
		//	GUI.Box (new Rect (0, 280, 630, 30), "log:" + log);
	}

	public void StartServer ()
	{
		if (!Network.isServer) {
			Network.InitializeSecurity ();
			Network.InitializeServer (20, NETWORK_PORT, false);
		}
	}

	public void StartConnect(string serverIP)
	{
		//Debug.Log ("BAT StartConnect serverIP:" + serverIP);
		isConnectedProcess = true;
		Network.Connect ((serverIP.Length > 6 && serverIP.Contains(".") ? serverIP : NETWORK_SERVER_IP), NETWORK_PORT);
	}

	public void CloseConnect()
	{
		//Debug.Log ("BAT CloseConnect");
		isConnectedProcess = false;
		Network.Disconnect();
		NavCam.EndGame ();
	}

	void OnFailedToConnect(NetworkConnectionError error)
	{
		//Debug.Log ("Error connect error:" +error.ToString());
		NavCam.EndGame ();
	}

	void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		if (Network.isClient) {
			Debug.Log ("Disconnected from server:" + info.ToString ());
		} else {
			Debug.Log ("Connection closed");
		}
		NavCam.EndGame ();
	}

	void OnConnectedToServer(){
		//Debug.Log ("Connected to server");
		GetComponent<SpriteRenderer> ().enabled = true;
	}

	void OnServerInitialized()
	{
		Debug.Log ("ServerInitialized");
	}

	void OnPlayerConnected(NetworkPlayer player)
	{
		Debug.Log ("onPlayerConnected:" + player.ipAddress+" port:"+player.port);
	}
	void OnPlayerDisconnected(NetworkPlayer player)
	{
		Debug.Log ("onPlayerDiconnected:" + player.ipAddress+" port:"+player.port);
	}

	void OnSerializeNetworkView(BitStream stream)
	{
		//log = "c:" + count + " s.isWr:"+stream.isWriting;
		//count++;
		Vector3 syncPosition = Vector3.zero;
		Vector3 syncVelocity = Vector3.zero;

		Vector3 syncPositionBall = Vector3.zero;
		Vector3 syncVelocityBall = Vector3.zero;

		int level = 0;

		if (stream.isWriting) {
			syncPosition = new Vector3(rg.position.x, rg.position.y, 0f);
			stream.Serialize (ref syncPosition);

			syncVelocity = new Vector3(rg.velocity.x, rg.velocity.y, 0f);
			stream.Serialize (ref syncVelocity);

			syncPositionBall = new Vector3(rgBall.position.x, rgBall.position.y, 0f);
			stream.Serialize (ref syncPositionBall);

			syncVelocityBall = new Vector3(rgBall.velocity.x, rgBall.velocity.y, 0f);
			stream.Serialize (ref syncVelocityBall);

			level = Bat.level;
			stream.Serialize (ref level);

		} else {
			stream.Serialize (ref syncPosition);
			stream.Serialize (ref syncVelocity);

			stream.Serialize (ref syncPositionBall);
			stream.Serialize (ref syncVelocityBall);

			stream.Serialize (ref level);

			//stream.Serialize (ref syncRotation);

			syncTime = 0f;
			syncDelay = Time.time - lastSynchronizationTime;
			lastSynchronizationTime = Time.time;

			Vector2 syncPos = new Vector2 (syncPosition.x, syncPosition.y);
			Vector2 syncVel = new Vector2 (syncVelocity.x, syncVelocity.y);

			Vector2 syncPosBall = new Vector2 (syncPositionBall.x, syncPositionBall.y);
			Vector2 syncVelBall = new Vector2 (syncVelocityBall.x, syncVelocityBall.y);

			//syncEndPosition = syncPosition + syncVelocity * syncDelay;
			syncEndPosition = syncPos + syncVel * syncDelay;
			syncStartPosition = rg.position;

			Vector2 pos = syncPos;
			//syncEndPosition = pos;
			syncEndPosition = syncEndPosition;
			syncEndPositionBall = syncPosBall;
			levelCurrent = level;
			Bat.level = level;
			rg.position = syncEndPosition;
			rgBall.position = syncEndPositionBall;

			//log = "c:" + count + "__isWr:"+stream.isWriting+" pos.x:"+pos.x;
		}
	}
}
