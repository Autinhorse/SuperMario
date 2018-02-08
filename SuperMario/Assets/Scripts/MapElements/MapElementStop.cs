using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapElementStop : MapElement {
	public const int StopType_Direct = 0;
	public const int StopType_Tilted = 1;
	int _subtype;

	void Awake() {
		type = Type_Stop;
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetSubtype( int subtype ) {
		_subtype = subtype;
		switch (_subtype) {
		case StopType_Direct:
			SptMain.sprite = GameMap.instance.SptStop01;
			break;
		case StopType_Tilted:
			SptMain.sprite = GameMap.instance.SptStop02;
			break;
		}
		SptMain.transform.localEulerAngles = Vector3.zero;
		SetColor (Color.yellow);
	}

	public void SetRunForward( bool isForward ) {
		if (isForward) {
			SptMain.transform.localEulerAngles = Vector3.zero;
			switch (_subtype) {
			case StopType_Direct:
				SptMain.sprite = GameMap.instance.SptStopArrow01;
				break;
			case StopType_Tilted:
				SptMain.sprite = GameMap.instance.SptStopArrow02;
				break;
			}
		} else {
			switch (_subtype) {
			case StopType_Direct:
				SptMain.sprite = GameMap.instance.SptStopArrow01;
				SptMain.transform.localEulerAngles = new Vector3 (0, 0, 180);
				break;
			case StopType_Tilted:
				SptMain.sprite = GameMap.instance.SptStopArrow02;
				SptMain.transform.localEulerAngles = new Vector3 (0, 0, 90);
				break;
			}
		}		
	}

	public void ReleaseFromStop() {
		SetSubtype (_subtype);
	}

	public int GetSubtype() {
		return _subtype;
	}

	public override int IsBlock( int dir ) {
		return Block_Hard;
	}
}
