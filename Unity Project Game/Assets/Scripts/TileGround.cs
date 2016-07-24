using UnityEngine;
using System.Collections;

public class TileGround : MonoBehaviour {
	public int widthTile = 4;
	public int heightTile = 4;
	public GameObject tileObject;
	public float seconds = 5;

	float secondsTmp;
	GameObject[] animAr;
	void Awake () {	
		secondsTmp = seconds;
		animAr = new GameObject[widthTile * heightTile];

		for (int x = 0; x < widthTile; x++)
			for (int y = 0; y < heightTile; y++) {
				float newX = -15.8f + x * 10.5f;
				float newY = 7.5f - y * 5;
				GameObject tile = (GameObject) Instantiate (tileObject, new Vector3(newX, newY, 0), Quaternion.identity);
				animAr[x*widthTile + y] = tile;
			}

		for (int z = 0; z < animAr.Length; z++) {
			GameObject go  = animAr[z];
			//Debug.Log ("Anim go:" + go);
			Animator anim = go.GetComponent<Animator>();
			//anim.Stop ();
			//anim.StopPlayback ();
			anim.enabled = false;
		}
	}

	void Update () {
		//Debug.Log ("secondsTmp:" +secondsTmp);
		secondsTmp -= Time.deltaTime;
		if (secondsTmp <= 0) {
			PlaySomeAnimate ();
			secondsTmp = seconds;
		}
	}

	Animator prevAnim;
	void PlaySomeAnimate()
	{
		if (prevAnim != null)
			prevAnim.enabled = false;

		int randN = Random.Range (0, animAr.Length - 1);
		GameObject go  = animAr[randN];
		//Debug.Log ("Anim PlaySomeAnimate go:" + go);
		Animator anim = go.GetComponent<Animator>();
		//anim.Play(UnityEngine.Experimental.Director.Playable.Connect);
		anim.enabled = true;
		//anim.StartPlayback();
		//anim.StopPlayback ();
		prevAnim = anim;
	}
}
