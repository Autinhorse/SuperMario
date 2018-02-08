using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class CoinArrowElement : ActiveElement {
	bool _touched;
	int _coinCreateIndex;

	void Awake() {
		DoAwake ();	
	}

	public override void DoAwake() {
		type = MapElement.Type_CoinArrow;
		colliseWidth = 1.0f;
		colliseHeight = 1.0f;
		_touched = false;
		_coinCreateIndex = -1;
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (_coinCreateIndex > 0) {
			int deltaX = 0;
			int deltaY = 0;
			switch (subtype) {
			case MapElement.Dir_Left:
				deltaX = -1;
				deltaY = 0;
				break;
			case MapElement.Dir_LeftUp:
				deltaX = -1;
				deltaY = -1;
				break;
			case MapElement.Dir_Up:
				deltaX = 0;
				deltaY = -1;
				break;
			case MapElement.Dir_RightUp:
				deltaX = 1;
				deltaY = -1;
				break;
			case MapElement.Dir_Right:
				deltaX = 1;
				deltaY = 0;
				break;
			case MapElement.Dir_RightDown:
				deltaX = 1;
				deltaY = 1;
				break;
			case MapElement.Dir_Down:
				deltaX = 0;
				deltaY = 1;
				break;
			case MapElement.Dir_LeftDown:
				deltaX = -1;
				deltaY = 1;
				break;
			}
			int index = 5 - _coinCreateIndex;
			_coinCreateIndex--;
			GameMap.instance.CreateCoin (x + deltaX * index,y + deltaY * index, CoinElement.CoinType_Normal);
		}
	}

	public override void DoCollised() {
		if (_touched == true) {
			return;
		}
		_touched = true;
		_coinCreateIndex=4;


		DOTween.Play (SptMain.DOFade (0.2f, 0.5f).OnComplete (() => {
			GameMap.instance.RemoveActiveElement( this );
			GameObject.Destroy( gameObject );
		}));
	}

	public override void SetSubtype( int vSubtype ) {
		subtype = vSubtype;
		switch (subtype) {
		case MapElement.Dir_Left:
			SptMain.sprite = GameMap.instance.SptCoinArrow01;
			SptMain.transform.localEulerAngles = new Vector3 (0, 0, 180);
			break;
		case MapElement.Dir_Right:
			SptMain.sprite = GameMap.instance.SptCoinArrow01;
			SptMain.transform.localEulerAngles = new Vector3 (0, 0, 0);
			break;
		case MapElement.Dir_Up:
			SptMain.sprite = GameMap.instance.SptCoinArrow01;
			SptMain.transform.localEulerAngles = new Vector3 (0, 0, 90);
			break;
		case MapElement.Dir_Down:
			SptMain.sprite = GameMap.instance.SptCoinArrow01;
			SptMain.transform.localEulerAngles = new Vector3 (0, 0, 270);
			break;
		case MapElement.Dir_RightUp:
			SptMain.sprite = GameMap.instance.SptCoinArrow02;
			SptMain.transform.localEulerAngles = new Vector3 (0, 0, 0);
			break;
		case MapElement.Dir_LeftUp:
			SptMain.sprite = GameMap.instance.SptCoinArrow02;
			SptMain.transform.localEulerAngles = new Vector3 (0, 0, 90);
			break;
		case MapElement.Dir_LeftDown:
			SptMain.sprite = GameMap.instance.SptCoinArrow02;
			SptMain.transform.localEulerAngles = new Vector3 (0, 0, 180);
			break;
		case MapElement.Dir_RightDown:
			SptMain.sprite = GameMap.instance.SptCoinArrow02;
			SptMain.transform.localEulerAngles = new Vector3 (0, 0, 270);
			break;
		}

		SetColor (GameMap.instance.MapElementMainColor);
	}
}
