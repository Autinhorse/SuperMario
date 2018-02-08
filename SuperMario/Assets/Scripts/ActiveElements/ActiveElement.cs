using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveElement : MonoBehaviour {

	//public const int Type_None = 0;
	//public const int Type_Coin = 1;
	//public const int Type_CoinArrow = 2;


	public const int Type_EnemyMushroom = 10001;



	//protected const int ColliseType_Box = 0;
	//protected const int ColliseType_Round = 1;

	public SpriteRenderer SptMain;

	[HideInInspector]
	public int triggerDir;

	[HideInInspector]
	public int type;
	[HideInInspector]
	public int subtype;
	[HideInInspector]
	public int x;
	[HideInInspector]
	public int y;

	//[HideInInspector]
	//public int colliseType;
	[HideInInspector]
	public float colliseWidth;
		
	[HideInInspector]
	public float colliseHeight;

	//[HideInInspector]
	//public float colliseRadius;

	void Awake() {
		DoAwake ();
	}

	public virtual void DoAwake() {
		triggerDir = MapElement.Dir_None;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//DoFixedUpdate ();	
	}

	public virtual void DoFixedUpdate() {
		
	}

	public virtual void SetColor( Color color ) {
		if(SptMain!=null){
			SptMain.color = color;
		}
	}

	public virtual bool IsCollised( Rect actorRect ) {
		if (transform.position.x-colliseWidth/2 > actorRect.xMax) {
				return false;
		}
		if (transform.position.x+colliseWidth/2 < actorRect.xMin) {
			return false;
		}
		if( transform.position.y+colliseHeight/2 < actorRect.yMin) {
			return false;
		}
		if( transform.position.y-colliseHeight/2 > actorRect.yMax) {
			return false;
		}

		if (this is EnemyMushroom) {
			Debug.Log ("EnemyMushroom:" + (transform.position.x - colliseWidth / 2).ToString () + "-" + (transform.position.x + colliseWidth / 2).ToString () 
				+ "-" + (transform.position.y + colliseHeight / 2).ToString () + "-" + (transform.position.y - colliseHeight / 2).ToString () + " | X-"
			+ actorRect.x.ToString () + " Y-" + actorRect.y.ToString () + " W-" + actorRect.width.ToString () + " H-" + actorRect.height.ToString ()
			+ " MinX-" + actorRect.xMin.ToString () + " MaxX:" + actorRect.xMax.ToString () + " MinY-" + actorRect.yMin.ToString () + " MaxY:" + actorRect.yMax.ToString ());
		}
		return true;
	}

	public virtual void SetSubtype( int vSubtype ) {
	}

	public virtual void DoCollised() {
	}

	public virtual bool IsActive() {
		return true;
	}

	public virtual bool IsAlive() {
		return true;
	}

	public virtual bool IsMovable() {
		return false;
	}

	public virtual bool IsColliseWithOthers() {
		return false;
	}

	public virtual bool IsEnemy() {
		return false;
	}

	public virtual void SetActive() {
	}

	public virtual void Reset() {
	}

	public virtual int GetTriggerDirection() {
		return triggerDir;
	}

	public virtual void FlyToActor() {
	}

}
