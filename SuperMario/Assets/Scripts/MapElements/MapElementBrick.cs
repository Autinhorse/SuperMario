using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class MapElementBrick : MapElement {
	int _special;
	bool _isBroken;

	void Awake() {
		type = Type_Brick;
		_isBroken = false;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void HitBottom() {
		if (_isBroken == true) {
			return;
		}

		_isBroken = true;

		DOTween.Play( SptMain.DOFade( 0, 0.3f ) );
		DOTween.Play (SptMain.transform.DOMoveY (SptMain.transform.position.y + 0.5f, 0.3f));
			
	}

	public override int IsBlock( int dir ) {
		if (_isBroken == true) {
			return Block_None;
		}

		if(dir==MapElement.Dir_Down) {
			return Block_Soft;
		}

		return Block_Hard;

	}
}
