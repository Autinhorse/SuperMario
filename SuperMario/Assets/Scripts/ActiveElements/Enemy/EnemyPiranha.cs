using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class EnemyPiranha : ActiveElement {

	public SpriteRenderer SptSpark;

	public float AnimationDuration;

	const int Status_Safe = 0;
	const int Status_Kill = 1;
	const int Status_Delay = 2;

	float _delay;
	float _hideDuration;
	float _showDuration;

	float _timer;
	int _status;

	int _dir;

	void Awake() {
		DoAwake ();
	}

	public override void DoAwake() {
		base.DoAwake ();
		type = MapElement.Type_Piranha;

		colliseWidth = 1.2f;
		colliseHeight = 2.0f;

		_status = Status_Delay;

	}

	public void SetData( float delay, float hideDuration, float showDuration, int dir ) {
		_delay = delay;
		_hideDuration = hideDuration;
		_showDuration = showDuration;
		_dir = dir;

		_timer = _delay;

		switch (_dir) {
		case MapElement.Dir_Up:
			SptMain.transform.localPosition = new Vector3 (0.5f, -0.5f, 0);
			break;
		case MapElement.Dir_Down:
			SptMain.transform.localPosition = new Vector3 (0.5f, 0.5f, 0);
			SptMain.transform.localEulerAngles = new Vector3 (0, 0, 180);
			break;
		case MapElement.Dir_Left:
			SptMain.transform.localPosition = new Vector3 (0.5f, -0.5f, 0);
			SptMain.transform.localEulerAngles = new Vector3 (0, 0, 90);
			break;
		case MapElement.Dir_Right:
			SptMain.transform.localPosition = new Vector3 (0.5f, -0.5f, 0);
			SptMain.transform.localEulerAngles = new Vector3 (0, 0, 270);
			break;
		}
	}

	public override void DoFixedUpdate() {
		_timer -= Time.fixedDeltaTime;
		if (_timer < 0) {
			switch (_status) {
			case Status_Safe:
				DOTween.Play (SptSpark.transform.DOLocalMoveY (1.9f, 0.2f).OnComplete (() => {
					_status = Status_Kill;
				}));
				_timer = _showDuration;
				break;
			case Status_Kill:
				DOTween.Play (SptSpark.transform.DOLocalMoveY (-0.1f, 0.2f));
				_status = Status_Safe;
				_timer = _hideDuration;
				break;
			case Status_Delay:
				_status = Status_Safe;
				_timer = _hideDuration;
				break;
			}
		}
	}

	public override bool IsCollised( Rect actorRect ) {
		if (_status != Status_Kill) {
			return false;
		}

		switch (_dir) {
		case MapElement.Dir_Up:
			if (transform.position.x - colliseWidth / 2 > actorRect.xMax) {
				return false;
			}
			if (transform.position.x + colliseWidth / 2 < actorRect.xMin) {
				return false;
			}
			if (transform.position.y+2 + colliseHeight / 2 < actorRect.yMin) {
				return false;
			}
			if (transform.position.y+2 - colliseHeight / 2 > actorRect.yMax) {
				return false;
			}
			break;
		case MapElement.Dir_Down:
			if (transform.position.x - colliseWidth / 2 > actorRect.xMax) {
				return false;
			}
			if (transform.position.x + colliseWidth / 2 < actorRect.xMin) {
				return false;
			}
			if (transform.position.y-2 + colliseHeight / 2 < actorRect.yMin) {
				return false;
			}
			if (transform.position.y-2 - colliseHeight / 2 > actorRect.yMax) {
				return false;
			}
			break;
		case MapElement.Dir_Left:
			if (transform.position.x-2 - colliseWidth / 2 > actorRect.xMax) {
				return false;
			}
			if (transform.position.x-2 + colliseWidth / 2 < actorRect.xMin) {
				return false;
			}
			if (transform.position.y + colliseHeight / 2 < actorRect.yMin) {
				return false;
			}
			if (transform.position.y - colliseHeight / 2 > actorRect.yMax) {
				return false;
			}
			break;
		case MapElement.Dir_Right:
			if (transform.position.x+2 - colliseWidth / 2 > actorRect.xMax) {
				return false;
			}
			if (transform.position.x+2 + colliseWidth / 2 < actorRect.xMin) {
				return false;
			}
			if (transform.position.y + colliseHeight / 2 < actorRect.yMin) {
				return false;
			}
			if (transform.position.y - colliseHeight / 2 > actorRect.yMax) {
				return false;
			}
			break;
		}

		return true;
	}

	public override bool IsEnemy() {
		return true;
	}

	public override void Reset() {
		SetData (_delay, _hideDuration, _showDuration, _dir);
	}
}
