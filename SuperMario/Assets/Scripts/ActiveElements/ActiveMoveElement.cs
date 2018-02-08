using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveMoveElement : ActiveElement {
	public const int Status_Dead = 10001;

	public float LineSpeed;

	public const int Status_Inactive = 0;
	public const int Status_Run = 1;
	public const int Status_FallDown = 2;
	public const int Status_RunSlope01 = 3;
	public const int Status_RunSlope02 = 4;
	public const int Status_RunSlope03 = 5;
	public const int Status_RunSlope04 = 6;
	public const int Status_BounceUp = 7;
	public const int Status_BounceDown = 8;
	public const int Status_FlyingUp = 9;
	public const int Status_OnGround = 10;
	public const int Status_FlyToActor = 11;
	public const int Status_Alive = 12;

	protected Vector3 _speed;
	protected int _status;
	protected int _dir;

	protected float _width;
	protected float _height;

	protected float _detectDelta;

	protected GameMap _gameMap;
	protected float _speedXBeforeBlock;

	protected const float Slope01Angle = 26.4774f;
	protected const float Slope02Angle = 45.0f;

	protected float _timer;

	protected float Gravity;

	[HideInInspector]
	public Vector3 StartPos;


	void Awake(){
		DoAwake ();
	}

 	public override void DoAwake ()
	{
		base.DoAwake ();
		_dir = MapElement.Dir_Left;
		_speed = Vector3.zero;
		_gameMap = GameMap.instance;

		_width = 1;
		_height = 1;

		_detectDelta = 0.05f;

		Gravity = -20.0f;
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetDirection( int dir ) {
		_dir = dir;
	}

	public override void SetActive() {
		if (IsActive ()) {
			return;
		}



		MapElement detectElement = GetFootBottomMiddleMapElement (transform.position);
		bool isEmpty = true;

		if (detectElement != null) {
			if (detectElement.IsBlock (MapElement.Dir_Up)!=MapElement.Block_None) {
				isEmpty = false;
			}
		}

		if (_dir == MapElement.Dir_Left) {
			_speed.x = -1 * LineSpeed;
		} else {
			_speed.x = LineSpeed;
		}

		if (isEmpty) {
			_speed.y = -0.5f;
			SetStatus (Status_FallDown);
		} else {
			SetStatus (Status_Run);
		}
	}

	public override bool IsActive() {
		return _status != Status_Inactive;
	}

	public virtual bool JumpOnHead() {
		return false;
	}

	void FixedUpdate() {
		//DoFixedUpdate ();
	}

	public override void DoFixedUpdate() {

		if(_status!=Status_Dead) {
			if (_gameMap.IsDeadMapPoint (_gameMap.GetMapPointOfWorldPosition (transform.position)) == true) {
				FalldownToDie ();
				return;
			}
		}

		switch (_status) {
		case Status_Inactive:
			break;
		case Status_BounceUp:
			{
				Vector3 pos = transform.position;
				pos += _speed * Time.fixedDeltaTime;
				transform.position = pos;

				_timer -= Time.fixedDeltaTime;
				if (_timer < 0) {
					_speed.y = -0.5f;
					SetStatus (Status_FallDown);
				}
				break;
			}
		case Status_Run: {
				Vector3 pos = transform.position;
				pos += _speed * Time.fixedDeltaTime;

				int detectDir = MapElement.Dir_Right;
				if (_speed.x > 0) {
					detectDir = MapElement.Dir_Left;
				}

				MapElement detectElement = GetFrontMapElement (pos);
				if (detectElement != null) {
					if (detectElement.IsBlock (detectDir) != MapElement.Block_None) {
						if (_speed.x < 0) {
							pos.x = _gameMap.GetWorldPositionByMapPoint (detectElement.x, detectElement.y).x + 1.0f;
						} else {
							pos.x = _gameMap.GetWorldPositionByMapPoint (detectElement.x, detectElement.y).x - 1.0f;
						}
						_speed.x *= -1;
					} else {
						if (_speed.x < 0) {
							if (detectElement.type == MapElement.Type_WallSlope03Follow) {
								pos.x = Mathf.Round (pos.x) - 0.3f;
								pos.y = Mathf.Round (pos.y) - 0.1f;
								SetRunSlopeDown01 (false);
							} else if (detectElement.type == MapElement.Type_WallSlope04) {
								pos.x = Mathf.Round (pos.x) - 0.2f;
								pos.y = Mathf.Round (pos.y) - 0.2f;
								SetRunSlopeDown02 (false);
							}
						} else if (_speed.x > 0) {
							if (detectElement.type == MapElement.Type_WallSlope01) {
								pos.x = Mathf.Round (pos.x)+ 0.3f;
								pos.y = Mathf.Round (pos.y)- 0.1f;
								SetRunSlopeUp01 (true);
							} else if (detectElement.type == MapElement.Type_WallSlope02) {
								pos.x = Mathf.Round (pos.x)+ 0.2f;
								pos.y = Mathf.Round (pos.y)- 0.2f;
								SetRunSlopeUp02 (true);
							}
						}

					}
				} else {
					detectElement = GetFootBottomMiddleMapElement (pos);
					bool isEmptyBelow = false;
					if (detectElement == null) {
						isEmptyBelow = true;
					} else {
						if (detectElement.type == MapElement.Type_WallSlope01Follow) {
							pos = _gameMap.GetWorldPositionByMapPoint (detectElement.x, detectElement.y);
							pos.x += 0.4f;
							pos.y += 1.0f;
							SetRunSlopeUp01 (false);
						} else if (detectElement.type == MapElement.Type_WallSlope02) {
							pos = _gameMap.GetWorldPositionByMapPoint (detectElement.x, detectElement.y);
							pos.x += 0.4f;
							pos.y += 1.0f;
							SetRunSlopeUp02 (false);
						} else if (detectElement.type == MapElement.Type_WallSlope03) {
							pos = _gameMap.GetWorldPositionByMapPoint (detectElement.x, detectElement.y);
							pos.x -= 0.4f;
							pos.y += 1.0f;
							SetRunSlopeDown01 (true);
						} else if (detectElement.type == MapElement.Type_WallSlope04) {
							pos = _gameMap.GetWorldPositionByMapPoint (detectElement.x, detectElement.y);
							pos.x -= 0.4f;
							pos.y += 1.0f;
							SetRunSlopeDown02 (true);
						} else if (detectElement.IsBlock (MapElement.Dir_Up) == MapElement.Block_None) {
							isEmptyBelow = true;
						}
					}

					if (isEmptyBelow == true) {
						_speed.y = -0.5f;
						SetStatus (Status_FallDown);
					}
				}

				transform.position = pos;
				break;
			}

		case Status_RunSlope01:
		case Status_RunSlope02:
		case Status_RunSlope03:
		case Status_RunSlope04: {
				Vector3 pos = transform.position;
				pos += _speed * Time.fixedDeltaTime;
				float angle = (45-transform.localEulerAngles.z)*3.1415927f/180;

				Vector3 detectPos = pos;

				if (_speed.x < 0) {
					detectPos.x -= ((_width / 2 + _detectDelta) * 1.414f * Mathf.Sin (angle));
					detectPos.y -= (_height / 2 - _detectDelta) * 1.414f * Mathf.Cos (angle);
				} else {
					detectPos.x += ((_width / 2 + _detectDelta) * 1.414f * Mathf.Cos (angle));
					detectPos.y -= (_height / 2 - _detectDelta) * 1.414f * Mathf.Sin (angle);
				}

				MapPoint detectMapPoint = _gameMap.GetMapPointOfWorldPosition (detectPos);
				MapElement detectElement = _gameMap.GetMapElementByMapPoint (detectMapPoint.x, detectMapPoint.y);

				if (detectElement != null) {
					if ((((_status == Status_RunSlope01) || (_status == Status_RunSlope02)) && (_speed.x < 0)) || (((_status == Status_RunSlope03) || (_status == Status_RunSlope04)) && (_speed.x > 0))) {
						if ((detectElement.type != MapElement.Type_WallSlope01) && (detectElement.type != MapElement.Type_WallSlope01Follow) && (detectElement.type != MapElement.Type_WallSlope02)
						    && (detectElement.type != MapElement.Type_WallSlope03) && (detectElement.type != MapElement.Type_WallSlope03Follow) && (detectElement.type != MapElement.Type_WallSlope04)
						    && (detectElement.type != MapElement.Type_WallSlopeBottom)) {
							pos = SetSlopeToRunDown (detectMapPoint);
						}
					}
				}
				detectPos = pos;

				if (_speed.x < 0) {
					detectPos.x -= ((_width / 2 + _detectDelta) * 1.414f * Mathf.Sin (angle));
					detectPos.y -= ((_height / 2 + _detectDelta) * 1.414f * Mathf.Cos (angle)+_detectDelta);
				} else {
					detectPos.x += ((_width / 2 + _detectDelta) * 1.414f * Mathf.Cos (angle));
					detectPos.y -= ((_height / 2 + _detectDelta) * 1.414f * Mathf.Sin (angle)+_detectDelta);
				}

			 	detectMapPoint = _gameMap.GetMapPointOfWorldPosition (detectPos);
				detectElement = _gameMap.GetMapElementByMapPoint (detectMapPoint.x, detectMapPoint.y);

				if (detectElement == null) {
					pos = SetSlopeToRunUp (detectMapPoint);
				}

				transform.position = pos;
				break;
			}

		case Status_FallDown:
			{
				Vector3 pos = transform.position;
				pos += _speed * Time.fixedDeltaTime;

				_speed.y += Gravity * Time.fixedDeltaTime;

				MapElement detectElement = GetFrontTopMapElement (pos);

				bool isBlocked = false;
				int detectDir = MapElement.Dir_Right;
				if (_speed.x > 0) {
					detectDir = MapElement.Dir_Left;
				}

				if (detectElement != null) {
					if (detectElement.IsBlock (detectDir)!=MapElement.Block_None) {
						if (_speed.x < 0) {
							pos.x = _gameMap.GetWorldPositionByMapPoint (detectElement.x, detectElement.y).x + 1.0f;
						}
						else {
							pos.x = _gameMap.GetWorldPositionByMapPoint (detectElement.x, detectElement.y).x - 1.0f;
						}
						_speed.x *= -1;
						isBlocked = true;
					}
				}

				if (isBlocked == false) {
					detectElement = GetFrontMapElement (pos);
					if (detectElement != null) {
						if (detectElement.IsBlock (detectDir)!=MapElement.Block_None) {
							if (_speed.x < 0) {
								pos.x = _gameMap.GetWorldPositionByMapPoint (detectElement.x, detectElement.y).x + 1.0f;
							}
							else {
								pos.x = _gameMap.GetWorldPositionByMapPoint (detectElement.x, detectElement.y).x - 1.0f;
							}
							_speed.x *= -1;
							isBlocked = true;
						}
					}
				}

				bool isMoveForward = _speed.x > 0;
				Vector3 detectPos = pos;
				detectPos.y -=  (_height/2+_detectDelta);

				MapPoint detectMapPoint = _gameMap.GetMapPointOfWorldPosition (detectPos);

				detectElement = _gameMap.GetMapElementByMapPoint (detectMapPoint.x, detectMapPoint.y);
				if (detectElement != null) {
					// 下面有障碍，判断是不是落在斜坡上
					if ((detectElement.type == MapElement.Type_WallSlope01)||(detectElement.type == MapElement.Type_WallSlope01Follow)) {
						Vector3 deltaPos = pos;
						deltaPos.x += 0.2f;
						deltaPos.x = deltaPos.x - (int)deltaPos.x;
						if (detectElement.type == MapElement.Type_WallSlope01) {
							deltaPos.y = deltaPos.x / 2;
						} else {
							Debug.Log ("Add 0.5.");
							deltaPos.y = deltaPos.x / 2 + 0.5f;
						}
						deltaPos.x += detectElement.transform.position.x - 0.5f;
						deltaPos.y += detectElement.transform.position.y - 0.5f;

						float angle = (45 - Slope01Angle) * 3.1415927f / 180;
						if (isMoveForward) {
							pos.x = deltaPos.x - 0.707f * Mathf.Cos (angle);
							pos.y = deltaPos.y + 0.707f * Mathf.Sin (angle);
							SetRunSlopeUp01 (isMoveForward);
						} else {
							// 向后走，前角在低处
							pos.x = deltaPos.x + 0.707f * Mathf.Sin (angle);
							pos.y = deltaPos.y + 0.707f * Mathf.Cos (angle);
							SetRunSlopeUp01 (isMoveForward);
						}
					} else if (detectElement.type == MapElement.Type_WallSlope02) {
						Vector3 deltaPos = pos;
						deltaPos.x += 0.2f;
						deltaPos.x = deltaPos.x - (int)deltaPos.x;
						deltaPos.y = deltaPos.x;

						deltaPos.x += detectElement.transform.position.x - 0.5f;
						deltaPos.y += detectElement.transform.position.y - 0.5f;


						float angle = (45 - Slope02Angle) * 3.1415927f / 180;
						if (isMoveForward) {
							pos.x = deltaPos.x - 0.707f * Mathf.Cos (angle);
							pos.y = deltaPos.y + 0.707f * Mathf.Sin (angle);
							SetRunSlopeUp02 (isMoveForward);
						} else {
							// 向后走，前角在低处
							pos.x = deltaPos.x + 0.707f * Mathf.Sin (angle);
							pos.y = deltaPos.y + 0.707f * Mathf.Cos (angle);
							SetRunSlopeUp02 (isMoveForward);
						}
					} else if ((detectElement.type == MapElement.Type_WallSlope03)||(detectElement.type == MapElement.Type_WallSlope03Follow)) {
						// 碰到了斜坡
						Vector3 deltaPos = pos;
						deltaPos.x += 0.5f;
						deltaPos.x = deltaPos.x - (int)deltaPos.x;
						if (detectElement.type == MapElement.Type_WallSlope03) {
							deltaPos.y = deltaPos.x / 2;
						} else {
							deltaPos.y = deltaPos.x / 2 + 0.5f;
						}
						deltaPos.x += detectElement.transform.position.x - 0.5f;
						deltaPos.y = detectElement.transform.position.y + 0.5f - deltaPos.y;

						float angle = (Slope01Angle-315) * 3.1415927f / 180;
						if (isMoveForward) {
							pos.x = deltaPos.x - 0.707f * Mathf.Cos (angle);
							pos.y = deltaPos.y + 0.707f * Mathf.Sin (angle);
							SetRunSlopeDown01 (isMoveForward);
						} else {
							// 向后走，前角在低处
							pos.x = deltaPos.x + 0.707f * Mathf.Sin (angle);
							pos.y = deltaPos.y + 0.707f * Mathf.Cos (angle);
							SetRunSlopeDown01 (isMoveForward);
						}
					} else if (detectElement.type == MapElement.Type_WallSlope04) {
						// 碰到了斜坡
						Vector3 deltaPos = pos;
						deltaPos.x += 0.4f;
						deltaPos.x = deltaPos.x - (int)deltaPos.x;
						deltaPos.y = deltaPos.x;

						deltaPos.x += detectElement.transform.position.x - 0.5f;
						deltaPos.y = detectElement.transform.position.y + 0.5f - deltaPos.y;


						float angle = (Slope02Angle-315) * 3.1415927f / 180;
						if (isMoveForward) {
							pos.x = deltaPos.x - 0.707f * Mathf.Cos (angle);
							pos.y = deltaPos.y + 0.707f * Mathf.Sin (angle);
							SetRunSlopeDown02 (isMoveForward);
						} else {
							// 向后走，前角在低处
							pos.x = deltaPos.x + 0.707f * Mathf.Sin (angle);
							pos.y = deltaPos.y + 0.707f * Mathf.Cos (angle);
							SetRunSlopeDown02 (isMoveForward);
						}
					} else if (detectElement.IsBlock (MapElement.Dir_Up) != MapElement.Block_None) {
						_speed.y = 0;
						pos.y = detectElement.transform.position.y + 0.5f + _height / 2;
						SetStatus (Status_Run);

						if (this is Mushroom) {
							_speed.x = ActorController.instance.ActorSpeedX;
						}

					}
				}

				transform.position = pos;
				break;
			}

		case Status_FlyToActor:
			{
				
				DoFixedUpdateFlyToActor ();
				break;
			}
		}
	}

	protected void DoFixedUpdateFlyToActor() {
		Vector3 targetPos = ActorController.instance.transform.position;
		float deltaX = targetPos.x - transform.position.x;
		float deltaY = targetPos.y - transform.position.y;

		float dis = (Mathf.Abs (deltaX) + Mathf.Abs (deltaY));
		if (dis < 0.5f) {
			DoCollised ();
		} else {
			float flySpeed = 12;
			float speedX = deltaX * flySpeed * Mathf.Abs (deltaX) / dis;
			float speedY = deltaY * flySpeed * Mathf.Abs (deltaY) / dis;

			Vector3 pos = transform.position;
			pos.x += speedX * Time.fixedDeltaTime;
			pos.y += speedY * Time.fixedDeltaTime;

			transform.position = pos;
		}
	}

	protected void SetStatus( int status ) {
		_status = status;
	}

	protected MapElement GetTopMiddleMapElement(Vector3 pos) {
		Vector3 detectPos = pos;

		detectPos.y +=  (colliseHeight/2+_detectDelta);

		MapPoint detectMapPoint = _gameMap.GetMapPointOfWorldPosition (detectPos);
		return _gameMap.GetMapElementByMapPoint (detectMapPoint.x, detectMapPoint.y);
	}


	protected MapElement GetFootBottomMiddleMapElement(Vector3 pos) {
		Vector3 detectPos = pos;
		float angle = transform.localEulerAngles.z*3.1415927f/180;
		detectPos.x +=  (colliseWidth/2+_detectDelta)*Mathf.Sin(angle);
		detectPos.y -=  (colliseHeight/2+_detectDelta)*Mathf.Cos(angle);

		MapPoint detectMapPoint = _gameMap.GetMapPointOfWorldPosition (detectPos);
		return _gameMap.GetMapElementByMapPoint (detectMapPoint.x, detectMapPoint.y);
	}

	protected MapElement GetFrontMapElement(Vector3 pos) {
		Vector3 detectPos = pos;

		if (_speed.x > 0) {
			detectPos.x += (colliseWidth/2+_detectDelta);
		} else {
			detectPos.x -= (colliseWidth/2+_detectDelta);
		}

		MapPoint detectMapPoint = _gameMap.GetMapPointOfWorldPosition (detectPos);
		return _gameMap.GetMapElementByMapPoint (detectMapPoint.x, detectMapPoint.y);
	}

	protected MapElement GetFrontTopMapElement(Vector3 pos) {
		Vector3 detectPos = pos;

		if (_speed.x > 0) {
			detectPos.x += (colliseWidth/2+_detectDelta);
		} else {
			detectPos.x -= (colliseWidth/2+_detectDelta);
		}
		detectPos.y += (colliseHeight/2+_detectDelta);

		MapPoint detectMapPoint = _gameMap.GetMapPointOfWorldPosition (detectPos);
		return _gameMap.GetMapElementByMapPoint (detectMapPoint.x, detectMapPoint.y);
	}
	protected MapElement GetFrontBottomMapElement(Vector3 pos) {
		Vector3 detectPos = pos;

		if (_speed.x > 0) {
			detectPos.x += (colliseWidth/2+_detectDelta);
		} else {
			detectPos.x -= (colliseWidth/2+_detectDelta);
		}
		detectPos.y -= (colliseHeight/2+_detectDelta);

		MapPoint detectMapPoint = _gameMap.GetMapPointOfWorldPosition (detectPos);
		return _gameMap.GetMapElementByMapPoint (detectMapPoint.x, detectMapPoint.y);
	}

	void SetRunSlopeUp01( bool isMoveForward ) {
		if (isMoveForward) {
			_speed.x *= 0.9f;
			_speed.y = _speed.x / 2;
		}
		else {
			_speed.x *= 1.1f;
			_speed.y = _speed.x / 2;
		}
		transform.localEulerAngles = new Vector3(0,0,Slope01Angle);
		SetStatus (Status_RunSlope01);
	}

	void SetRunSlopeUp02( bool isMoveForward ) {
		if (isMoveForward) {
			_speed.x *= 0.75f;
			_speed.y = _speed.x;
		}
		else {
			_speed.x *= 1.2f;
			_speed.y = _speed.x;
		}
		transform.localEulerAngles = new Vector3 (0, 0,Slope02Angle);
		SetStatus (Status_RunSlope02);
	}

	void SetRunSlopeDown01( bool isMoveForward ) {
		if (isMoveForward) {
			_speed.x *= 1.1f;
			_speed.y = _speed.x / -2;
		}
		else {
			_speed.x *= 0.9f;
			_speed.y = _speed.x / -2;
		}
		transform.localEulerAngles = new Vector3(0,0,-1*Slope01Angle);
		SetStatus (Status_RunSlope03);
	}

	void SetRunSlopeDown02( bool isMoveForward ) {
		if (isMoveForward) {
			_speed.x *= 1.2f;
			_speed.y = _speed.x * -1;
		}
		else {
			_speed.x *= 0.75f;
			_speed.y = _speed.x * -1;
		}
		transform.localEulerAngles = new Vector3 (0, 0,-1*Slope02Angle);
		SetStatus (Status_RunSlope04);
	}

	Vector3 SetSlopeToRunUp(MapPoint detectPoint) {
		Vector3 targetPos = _gameMap.GetWorldPositionByMapPoint (detectPoint.x, detectPoint.y);
		targetPos.x = targetPos.x - 0.5f * Mathf.Sign (_speed.x);
		_speed.x = LineSpeed * Mathf.Sign (_speed.x);
		_speed.y = 0;
		SetStatus (Status_Run);

		transform.localEulerAngles = Vector3.zero;

		return targetPos;
	}

	Vector3 SetSlopeToRunDown(MapPoint detectPoint) {
		Vector3 targetPos = _gameMap.GetWorldPositionByMapPoint (detectPoint.x, detectPoint.y);
		targetPos.x = targetPos.x - 0.5f * Mathf.Sign (_speed.x);
		targetPos.y = targetPos.y+1.0f;
		_speed.x = LineSpeed * Mathf.Sign (_speed.x);
		_speed.y = 0;
		SetStatus (Status_Run);

		transform.localEulerAngles = Vector3.zero;

		return targetPos;
	}

	// 这个可移动物体会不会和其他可移动物体碰撞
	public virtual bool ShouldCollisedWithOthers() {
		return true;
	}

	// 和其他移动物体碰撞，反向推开
	public virtual void CollisePushBack( bool pushBack) {
		Vector3 pos;
		switch (_status) {
		case Status_Run:
		case Status_RunSlope01:
		case Status_RunSlope02:
		case Status_RunSlope03:
		case Status_RunSlope04:
			// 直线运动发生碰撞，转向
			_speed *= -1;
			if (pushBack == true) {
				DoFixedUpdate ();
			}
			break;
		case Status_FallDown:
			// 直线运动发生碰撞，转向
			_speed.y = 3.5f;
			_timer = 0.5f;
			SetStatus (Status_BounceUp);
			DoFixedUpdate();

			break;
		}
	}


	public override bool IsMovable() {
		if (IsActive () == false) {
			return false;
		}
		return true;
	}

	public virtual void FalldownToDie() {
		_status = Status_Dead;

		gameObject.SetActive (false);
	}

	public override bool IsCollised( Rect actorRect ) {
		if (IsAlive () == false) {
			return false;
		}

		return base.IsCollised (actorRect);
	}

	public virtual bool IsAlive() {
		if (IsActive () == false) {
			return false;
		}
		if (_status != Status_Dead) {
			return true;
		}
		return false;
	}
}
