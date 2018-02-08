using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class CoinElement : ActiveMoveElement {

	public const int CoinType_Normal = 0;

	bool _touched;

	void Awake() {
		DoAwake ();
	}

	public override void DoAwake() {
		base.DoAwake ();
		type = MapElement.Type_Coin;

		colliseWidth = 0.8f;
		colliseHeight = 0.8f;
		_touched = false;

		_status = Status_Alive;
	}

	public override void DoFixedUpdate() {
		switch (_status) {
		case Status_FlyToActor:
			{
				DoFixedUpdateFlyToActor ();
				break;
			}
		}
	}

	public override void SetSubtype( int vSubtype ) {
		subtype = vSubtype;
		switch (subtype) {
		case CoinType_Normal:
			SetColor (GameMap.instance.MapElementMainColor);
			break;
		}
	}

	public override void DoCollised() {
		if (_touched == true) {
			return;
		}
		_touched = true;

		GameMap.instance.ChangeCoin (1);


		DOTween.Play (SptMain.transform.DOMoveY (transform.position.y + 2.5f, 1.0f).SetEase( Ease.InOutBack));
		DOTween.Play (SptMain.DOFade (0, 1.5f).OnComplete (() => {
			GameMap.instance.RemoveActiveElement( this );
			GameObject.Destroy( gameObject );
		}));
	}

	public override void FlyToActor() {
		_status = Status_FlyToActor;
	}
}
