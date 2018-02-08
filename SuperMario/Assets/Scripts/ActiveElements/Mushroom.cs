using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class Mushroom : ActiveMoveElement {

	public const int Subtype_AwardStar = 0;
	public const int Subtype_AwardLife = 1;


	bool _touched;

	int _subtype;


	void Awake() {
		DoAwake ();
	}

	public override void DoAwake() {
		base.DoAwake ();
		type = MapElement.Type_CoinEject;

		_width = 0.8f;
		_height = 0.8f;

		colliseWidth = 0.8f;
		colliseHeight = 0.8f;

		_detectDelta = 0.0f;

		_touched = false;

		Gravity = -24.0f;
	}

	public void SetSubtype( int subtype ) {
		_subtype = subtype;
		switch (_subtype) {
		case Subtype_AwardStar:
			SptMain.sprite = GameMap.instance.SptAwardStar;
			break;
		case Subtype_AwardLife:
			SptMain.sprite = GameMap.instance.SptAwardLife;
			break;
		}
	}

	void FixedUpdate() {
		//DoFixedUpdate ();
	}

	public override void DoFixedUpdate() {
		if ((_status == Status_Dead) || (_status == Status_OnGround)) {
			return;
		}

		base.DoFixedUpdate ();



		switch (_status) {
		case Status_FlyingUp:
			
			Vector3 pos = transform.position;
			pos += _speed * Time.fixedDeltaTime;
			MapElement mapElement;

			_timer -= Time.fixedDeltaTime;

			mapElement = GetTopMiddleMapElement (pos);
			if (mapElement != null) {
				if (mapElement.IsBlock (MapElement.Dir_Down) != MapElement.Block_None) {
					// 上面遇到障碍
					pos.y = mapElement.transform.position.y-0.5f-colliseHeight/2;
					_speed.y *= -1;
				}
			}

			mapElement = GetFrontMapElement (pos);
			if (mapElement != null) {
				int detectDir = MapElement.Dir_Left;
				if (_speed.x < 0) {
					detectDir = MapElement.Dir_Right;
				}

				if (mapElement.IsBlock (detectDir) != MapElement.Block_None) {
					// 前面遇到障碍
					if (_speed.x > 0) {
						pos.x = mapElement.transform.position.x - 0.5f - colliseWidth / 2;
					}
					else {
						pos.x = mapElement.transform.position.x + 0.5f + colliseWidth / 2;
					}
					_speed.x *= -1;
				}
			}

			_speed.y += Gravity * Time.fixedDeltaTime;
			if (_speed.y < 0) {
				_status = Status_FallDown;
			}
			transform.position = pos;
			break;
		
		}


	}

	public override void DoCollised() {
		if (_touched == true) {
			return;
		}
		_touched = true;

		_status = Status_Dead;


		DOTween.Play (SptMain.transform.DOMoveY (transform.position.y + 2.5f, 1.0f).SetEase( Ease.InOutBack));
		DOTween.Play (SptMain.DOFade (0, 1.5f).OnComplete (() => {
			GameMap.instance.RemoveActiveElement( this );
			GameObject.Destroy( gameObject );
		}));
	}

	public void EjectOut( int sign ) {
		_speed = new Vector3 (ActorController.instance.ActorSpeedX*1.25f*sign, 2.0f, 0);
		_status = Status_FlyingUp;

	}
}
