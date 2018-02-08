using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class CoinEject : ActiveMoveElement {
	

	bool _touched;


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

		Gravity = -40.0f;
	}

	// Use this for initialization
	void Start () {
		
	}

	void FixedUpdate() {
		//DoFixedUpdate ();
	}
	
	public override void DoFixedUpdate() {
		if ((_status == Status_Dead) || (_status == Status_OnGround)) {
			return;
		}

		Vector3 pos = transform.position;
		pos += _speed * Time.fixedDeltaTime;
		MapElement mapElement;

		switch (_status) {
		case Status_FlyingUp:
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
				
			break;
		case Status_FallDown:
			_speed.y += Gravity * Time.fixedDeltaTime;

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
					} else {
						pos.x = mapElement.transform.position.x + 0.5f + colliseWidth / 2;
					}
					_speed.x *= -1;
				}
			}

			mapElement = GetFootBottomMiddleMapElement (pos);
			if (mapElement != null) {
				if (mapElement.IsBlock (MapElement.Dir_Up) != MapElement.Block_None) {
					pos.y = mapElement.transform.position.y + 0.5f + colliseHeight / 2;
					_speed.y*= -0.5f;
					_speed.x /= 2;
					if (Mathf.Abs (_speed.y) < Mathf.Abs (Gravity)*Time.fixedDeltaTime*1.75f) {
						_speed.x = 0;
						_speed.y = 0;
						_status = Status_OnGround;
					} else {
						_status = Status_FlyingUp;
					}
				}
			}

			if (_gameMap.IsDeadMapPoint (_gameMap.GetMapPointOfWorldPosition (pos)) == true) {
				// 掉下去了
				_gameMap.RemoveActiveElement( this );
				GameObject.Destroy( gameObject );
			}

			break;
		case Status_BounceUp:
			_timer -= Time.fixedDeltaTime;

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
					} else {
						pos.x = mapElement.transform.position.x + 0.5f + colliseWidth / 2;
					}
					_speed.x *= -1;
				}
			}

			if (_timer < 0) {
				_status = Status_BounceDown;
				_speed.y *= -1;
			}
			break;
		case Status_BounceDown:
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
					} else {
						pos.x = mapElement.transform.position.x + 0.5f + colliseWidth / 2;
					}
					_speed.x *= -1;
				}
			}

			mapElement = GetFootBottomMiddleMapElement (pos);
			if (mapElement != null) {
				if (mapElement.IsBlock (MapElement.Dir_Up) != MapElement.Block_None) {
					pos.y = mapElement.transform.position.y + 0.5f + colliseHeight / 2;
					_status = Status_OnGround;
					_speed.x = 0;
					_speed.y = 0;
				}
			}
			break;
		}

		transform.position = pos;
	}

	public override void DoCollised() {
		if (_touched == true) {
			return;
		}
		_touched = true;

		GameMap.instance.ChangeCoin (1);

		_status = Status_Dead;

		DOTween.Play (SptMain.transform.DOMoveY (transform.position.y + 2.5f, 1.0f).SetEase( Ease.InOutBack));
		DOTween.Play (SptMain.DOFade (0, 1.5f).OnComplete (() => {
			GameMap.instance.RemoveActiveElement( this );
			GameObject.Destroy( gameObject );
		}));
	}

	public void EjectOut( int sign ) {
		_speed = new Vector3 (ActorController.instance.ActorSpeedX * UnityEngine.Random.Range (120, 150) / 100.0f*sign, 12.0f, 0);
		_status = Status_FlyingUp;

	}

	public override void FlyToActor() {
		_status = Status_FlyToActor;
	}
}
