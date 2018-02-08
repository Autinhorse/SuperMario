using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class ActorController : MonoBehaviour {

	public const int Status_StandBy = 0;
	public const int Status_Run = 1;
	public const int Status_Blocked = 2;
	public const int Status_JumpupBlocked = 3;
	public const int Status_JumpUp = 4;
	public const int Status_JumpTop = 5;
	public const int Status_FallDown = 6;
	public const int Status_SlideDownFront = 9;
	public const int Status_SlideDownBack = 10;
	public const int Status_JumpHitTop = 11;
	public const int Status_RunSlope01 = 12;
	public const int Status_RunSlope02 = 13;
	public const int Status_RunSlope03 = 14;
	public const int Status_RunSlope04 = 15;
	public const int Status_Stop = 16;
	public const int Status_GameOver = 17;
	public const int Status_SpringUp = 18;

	public float ActorSpeedX;					// 水平速度
	public float ActorClimbDuration;			// 前面右一层障碍的时候，翻上障碍需要的时间

	public float ActorJumpUpSpeedY;				// 向上跳起开始的时候的垂直速度
	public float ActorJumpUpDuration;			// 向上运动的最少时间，这段时间的状态是Status_JumpUp
	public float ActorJumpUpTopDuration;		// 在跳起的高点水平运动的时间，这段时间主角先升高在降低，状态是Status_JumpTop
	public float ActorJumpUpPressDuration;		// 在跳起以后，可以保持连续按下一直上升的时间，注意这个时间需要保证小于ActorJumpUpDuration+ActorJumpUpPressDuration*ActorJumpUpPressTimeRate
	public float ActorJumpUpTopHeight;			// 在进入状态是Status_JumpTop状态后，可以上升的高度
	public float ActorJumpUpPressTimeRate;		// 
	public float ActorJumpUpFallSpeedRate;		// 经过状态是Status_JumpTop状态后开始下降，下降的速度相对于上升速度的比例
	public float ActorBounceUpDuration;			// 踩中敌人后自动弹起向上跳的时间

	public float ActorBlockedDuration;			// 前方被阻挡后，在原地跳动，一次跳动的时间
	public float ActorBlockedJumpHeight;		// 前方被阻挡后，在原地跳动，一次跳动的高度

	public float ActorSlideSpeedY;				// 跳到墙上，贴墙下滑的速度

	public float ActorSlideBeginDelay;
	public float ActorSlideTurnArroundDelay;

	public const int Ani_Idle = 0;
	public const int Ani_Run = 1;
	public const int Ani_JumpUp = 2;
	public const int Ani_JumpTop = 3;
	public const int Ani_FallDown = 4;
	public const int Ani_SlideDown = 5;
	public const int Ani_RunSlope01 = 6;
	public const int Ani_RunSlope02 = 7;
	public const int Ani_RunSlope03 = 8;
	public const int Ani_RunSlope04 = 9;
	public const int Ani_Blocked = 10;
	public const int Ani_JumpHitTop = 11;
	public const int Ani_JumpupBlocked = 12;
	public const int Ani_Stop = 13;

	public static ActorController instance {
		get {
			return _instance;
		}
	}
	static ActorController _instance;

	float Gravity;
		

	const float Slope01Angle = 26.4774f;
	const float Slope02Angle = 45.0f;


	int _status;
	int _animation;

	float _magnetTimer;

	GameMap _gameMap;

	float DetectAdvanceSpaceX;					// 碰撞判断提前量
	float DetectAdvanceSpaceY;					// 碰撞判断提前量

	public Vector3 pos {
		get {
			return gameObject.transform.position;
		}
	}

	public bool isWaitingAnimation {
		get {
			return _isWaitingAnimation;
		}
	}
	bool _isWaitingAnimation;

	float _width;
	float _height;

	Vector3 _speed;
	//bool _isForward;

	float _timer;
	float _jumpUpTimer;
	float _stopTimer;
	bool _jumpUpPressed;
	bool _jumpUpPressTreated;
	float _jumpUpTopRate;

	bool _isLastRunForward;

	float _targetY;

	MapElementStop _stopElement;

	void Awake() {
		_instance = this;

		_isWaitingAnimation = false;

		_speed = Vector3.zero;

		_width = 0.9f;
		_height = 1.6f;
		DetectAdvanceSpaceX = 0.065f;
		DetectAdvanceSpaceY = 0.05f;
	}

	// Use this for initialization
	void Start () {
		SetStatus( Status_StandBy );
		SetAnimation (Ani_Idle);

		_gameMap = GameMap.instance;

		_jumpUpPressed = false;
		_jumpUpPressTreated = false;

		_isLastRunForward = true;

		//_isForward = true;

		_animation = Ani_Idle;

		_stopTimer = -1;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		MapElement detectElement;

		bool isGroundBelow;
		bool isSlopeBelow;
		bool isHeadHit;
		bool isSlideDown;
		bool isMoveForward;
		bool isStopElement;
		int detectDir;

		float deltaTime = Time.fixedDeltaTime;
		if (deltaTime > 0.02f) {
			deltaTime = 0.02f;
		}

		// 判断和关卡中的运动物体关系
		CheckActiveElements();

		Vector3 pos;
		pos = transform.position;
		Debug.Log ("Update In - Status" + _status.ToString () + "  Time:" + Time.fixedDeltaTime.ToString ()+":"+deltaTime.ToString() + " x:" + pos.x.ToString ()+ " y:" + pos.y.ToString () +"  Speed-x:"+_speed.x.ToString()+" y:"+_speed.y.ToString() );
		pos += _speed * deltaTime;

		isMoveForward = _speed.x > 0;

		if(_status!=Status_GameOver) {
			detectElement = GetFootMapElement (pos);
			if (detectElement != null) {
				if ((detectElement.type == MapElement.Type_End) || (detectElement.type == MapElement.Type_EndFollow)) {
					SetGameOver (true);
					return;
				}
			}

			if (_gameMap.IsDeadMapPoint (_gameMap.GetMapPointOfWorldPosition (pos)) == true) {
				SetGameOver (false);
				return;
			}
		}

		if (_magnetTimer > 0) {
			_magnetTimer -= deltaTime;
		}

		switch (_status) {
		case Status_Run:
			if (_timer >= 0) {
				_timer -= deltaTime;
				if (_timer < 0) {
					_speed = GetDefaultSpeed ();
				} else {
					break;
				}
			}

			if (_stopTimer > 0) {
				_stopTimer -= deltaTime;
			}

			if (isMoveForward == true) {
				detectDir = MapElement.Dir_Left;
			} else {
				detectDir = MapElement.Dir_Right;
			}

			isSlopeBelow = false;
			// 判断前面是不是被堵住
			detectElement = GetFrontMapElement (pos, isMoveForward);
			if (detectElement != null) {
				if (detectElement.IsBlock (detectDir) != MapElement.Block_None) {
					// 前面的方块不能进入，被堵住了
					SetStatus (Status_Blocked);
					SetAnimation (Ani_Blocked);
					_isLastRunForward = isMoveForward;
					_timer = ActorBlockedDuration;
					_targetY = pos.y + ActorBlockedJumpHeight;
					pos.x -= _speed.x * deltaTime;
					Debug.LogWarning ("Run block front: TargetY:" + _targetY.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
					break;
				}	
				// 判断前面是不是斜坡
				if (detectElement.type == MapElement.Type_WallSlope01) {
					pos.x = Mathf.Round (pos.x) + 0.5f;
					isSlopeBelow = true;
					SetRunSlopeUp01 (isMoveForward);
					Debug.LogWarning ("Run to slope 01: Speed:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());

				} else if (detectElement.type == MapElement.Type_WallSlope02) {
					pos.x = Mathf.Round (pos.x) + 0.35f;
					isSlopeBelow = true;
					SetRunSlopeUp02 (isMoveForward);
					Debug.LogWarning ("Run to slope 02: Speed:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());

				}
				else if (detectElement.type == MapElement.Type_WallSlope03Follow) {
					pos.x = Mathf.Round (pos.x) - 0.5f;
					isSlopeBelow = true;
					SetRunSlopeDown01 (isMoveForward);
					Debug.LogWarning ("Run to slope 03: Speed:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());

				} else if (detectElement.type == MapElement.Type_WallSlope04) {
					pos.x = Mathf.Round (pos.x) - 0.5f;
					isSlopeBelow = true;
					SetRunSlopeDown02 (isMoveForward);
					Debug.LogWarning ("Run to slope 04: Speed:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());

				}
			}


			detectElement = GetFrontHeadMapElement (pos, isMoveForward);
			if (detectElement != null) {
				if (detectElement.IsBlock (detectDir) != MapElement.Block_None) {
					// 前面的方块不能进入，被堵住了
					SetStatus (Status_Blocked);
					SetAnimation (Ani_Blocked);
					_isLastRunForward = isMoveForward;
					_timer = ActorBlockedDuration;
					_targetY = pos.y + ActorBlockedJumpHeight;
					pos.x -= _speed.x * deltaTime;
					Debug.LogWarning ("Run block front head: TargetY:" + _targetY.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
					break;
				}	
			}

			// 判断下面是不是空了
			detectElement = GetFootBottomMapElement (pos);
			if ((_status == Status_Run)&&(detectElement!=null)) {
				if ((detectElement.type == MapElement.Type_WallSlope03) || (detectElement.type == MapElement.Type_WallSlope03Follow)) {
					_speed = GetDefaultSpeed ();
					if (isMoveForward) {
						_speed.x *= 1.1f;
						_speed.y = -1 * Mathf.Abs (_speed.x / 2);
					} else {
						_speed.x *= 0.9f;
						_speed.y = -1 * Mathf.Abs (_speed.x / 2);
					}
					pos.x = Mathf.Round (pos.x) - 0.35f;
					SetAnimation (Ani_RunSlope03);
					SetStatus (Status_RunSlope03);
					isSlopeBelow = true;
					Debug.LogWarning ("Run slope 03: SpeedY:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
				} else if (detectElement.type == MapElement.Type_WallSlope04) {
					_speed = GetDefaultSpeed ();
					if (isMoveForward) {
						_speed.x *= 1.2f;
						_speed.y = -1 * _speed.x;
					} else {
						_speed.x *= 0.75f;
						_speed.y = -1 * _speed.x;
					}
					pos.x = Mathf.Round (pos.x) - 0.35f;
					SetAnimation (Ani_RunSlope04);
					SetStatus (Status_RunSlope04);
					isSlopeBelow = true;
					Debug.LogWarning ("Run slope 04: SpeedY:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
				} else if ((detectElement.type == MapElement.Type_WallSlope01) || (detectElement.type == MapElement.Type_WallSlope01Follow)) {
					_speed = GetDefaultSpeed ();
					if (isMoveForward) {
						_speed.x *= 1.1f;
						_speed.y = _speed.x / 2;
					} else {
						_speed.x *= 0.9f;
						_speed.y = _speed.x / 2;
					}	
					pos.x = Mathf.Round (pos.x) + 0.35f;
					SetAnimation (Ani_RunSlope01);
					SetStatus (Status_RunSlope02);
					isSlopeBelow = true;
					Debug.LogWarning ("Run slope 01: SpeedY:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
				} else if (detectElement.type == MapElement.Type_WallSlope02) {
					_speed = GetDefaultSpeed ();
					if (isMoveForward) {
						_speed.x *= 1.2f;
						_speed.y = _speed.x;
					} else {
						_speed.x *= 0.75f;
						_speed.y = _speed.x;
					}
					pos.x = Mathf.Round (pos.x) + 0.34f;
					SetAnimation (Ani_RunSlope02);
					SetStatus (Status_RunSlope02);
					isSlopeBelow = true;
					Debug.LogWarning ("Run slope 02: SpeedY:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
				} else if (detectElement.type == MapElement.Type_Stop) {
					if (_stopTimer < 0) {
						// 判断是不是踩到了停止
						_stopElement = (MapElementStop)detectElement;
						pos = SetStop ();
					}
				}

			}

			bool isEmpty = false;
			if (detectElement == null) {
				isEmpty = true;
			} else {
				if (detectElement.IsBlock (MapElement.Dir_Up) == MapElement.Block_None) {
					// 下面是空的
					isEmpty = true;
				}	
			}
			if ((isEmpty == true)&&(isSlopeBelow==false)) {
				SetStatus (Status_FallDown);
				SetAnimation (Ani_FallDown);
				_speed.y = -1 * ActorJumpUpSpeedY * ActorJumpUpFallSpeedRate;
				Debug.LogWarning ("Run fall down: SpeedY:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
			} 


			break;
		case Status_Blocked:
			
			// 前面被堵住的状态，在原地跳动
			_timer -= deltaTime;
			//pos.x -= _speed.x * Time.deltaTime;
			pos.x = Mathf.Round(pos.x);
			if (_timer > ActorBlockedDuration / 2) {
				pos.y = _targetY - ActorBlockedJumpHeight * (_timer - ActorBlockedDuration / 2) / (ActorBlockedDuration / 2);
			} else if (_timer < 0) {
				pos.y = _targetY-ActorBlockedJumpHeight;
				_timer = ActorBlockedDuration;

			} else {
				pos.y = _targetY - ActorBlockedJumpHeight * ( ActorBlockedDuration / 2-_timer) / (ActorBlockedDuration / 2);
			}
			break;
		
		case Status_JumpUp:
			if (isMoveForward == true) {
				detectDir = MapElement.Dir_Left;
			}
			else {
				detectDir = MapElement.Dir_Right;
			}
			// 向上跳起
			if (_jumpUpPressed == true) {
				// 玩家没有松开按键，保持上升
				_timer += deltaTime * ActorJumpUpPressTimeRate;
				_jumpUpTimer -= deltaTime;
				if (_jumpUpTimer <= 0) {
					_jumpUpPressed = false;
					_jumpUpTopRate = 2.0f;
				}
			}
			_timer -= deltaTime;
			if (_timer <= 0) {
				// 超过按键允许时间
				SetStatus (Status_JumpTop);
				SetAnimation (Ani_JumpTop);
				_timer += ActorJumpUpTopDuration * _jumpUpTopRate;
				_targetY = pos.y + ActorJumpUpTopHeight * _jumpUpTopRate;
				pos.y = CalculateJumpUpTopHeight (_timer);

				Debug.LogWarning ("Jumpup to JumpTop: _targetY:" + _targetY.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
			}

			// 判断头上是不是顶到了东西
			detectElement = GetHeadTopMapElement (pos);
			if (detectElement != null) {
				switch (detectElement.IsBlock (MapElement.Dir_Down)) {
				case  MapElement.Block_None:
					break;
				case  MapElement.Block_Soft:
					detectElement.HitBottom (_speed.x>0);
					_timer = ActorJumpUpTopDuration;
					_targetY = pos.y + ActorJumpUpTopHeight;
					_jumpUpTopRate = 1.0f;
					pos.y = CalculateJumpUpTopHeight (_timer);
					SetStatus (Status_JumpHitTop);
					SetAnimation (Ani_JumpHitTop);
					Debug.LogWarning ("Jumpup Hit top hard: SpeedY:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
					break;
				case  MapElement.Block_Hard:
					pos.y = Mathf.Round (pos.y);
					_speed.y = -1 * ActorJumpUpSpeedY * ActorJumpUpFallSpeedRate;
					SetStatus (Status_FallDown);
					SetAnimation (Ani_FallDown);
					Debug.LogWarning ("Jumpup Hit top hard: SpeedY:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
					break;
				}
			}

			// 判断前面是不是被堵住
			isSlideDown = false;

			// 先判断主角的头部有没有撞上障碍
			detectElement = GetFrontTopMapElement (pos,isMoveForward);
			if (detectElement != null) {
				if (detectElement.IsBlock (detectDir) != MapElement.Block_None) {
					pos = SetSlideDown (pos,isMoveForward,deltaTime);
					// 被堵住了，下滑
					isSlideDown = true;

					Debug.LogWarning ("Jumpup to Slide 01:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
				}
			}

			// 头部没有被堵住，判断中部
			if (isSlideDown == false) {
				detectElement = GetFrontMiddleMapElement (pos,isMoveForward);
				if (detectElement != null) {
					if (detectElement.IsBlock (detectDir) != MapElement.Block_None) {
						// 中间被堵住了
						// 被堵住了，下滑
						pos = SetSlideDown (pos,isMoveForward,deltaTime);
						isSlideDown = true;
						Debug.LogWarning ("Jumpup to Slide 02:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
					}
				}
			}

			// 头部，中部都没有被堵住，判断脚部
			if (isSlideDown == false) {
				detectElement = GetFrontBottomMapElement (pos,isMoveForward);
				if (detectElement != null) {
					if (detectElement.IsBlock (detectDir) != MapElement.Block_None) {
						// 判断脚下是不是正好是斜坡，在斜坡起跳有时候会撞到前面斜坡下的墙，所以是的话忽略前面的障碍
						detectElement = GetFootMapElement (pos);
						isSlopeBelow = false;
						if (detectElement != null) {
							if ((detectElement.type == MapElement.Type_WallSlope01) || (detectElement.type == MapElement.Type_WallSlope01Follow)
							    || (detectElement.type == MapElement.Type_WallSlope02)
							    || (detectElement.type == MapElement.Type_WallSlope03) || (detectElement.type == MapElement.Type_WallSlope03Follow)
							    || (detectElement.type == MapElement.Type_WallSlope04)) {
								isSlopeBelow = true;
							}
						}
						if(isSlopeBelow==false) {
							// 下边沿被堵住了
							// 判断主角是不是超过对面障碍半个身高，是的话可以越过
							if (pos.y - (int)pos.y > 0.5f) {
								// 没有被阻挡，上到障碍上面继续跑
								pos = SetJumpToRun (pos);
								Debug.LogWarning ("Jumpup to Run:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
							} else {
								// 被堵住了
								pos = SetSlideDown (pos, isMoveForward, deltaTime);
								isSlideDown = true;
								Debug.LogWarning ("Jumpup to Slide 03:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
								break;
							}
						}
					}	
				}
			}

			break;

		case Status_JumpupBlocked:
			isMoveForward = GetDefaultSpeed ().x > 0;
			if (isMoveForward == true) {
				detectDir = MapElement.Dir_Left;
			} else {
				detectDir = MapElement.Dir_Right;
			}
			// 前面有阻碍，向上跳起
			if (_jumpUpPressed == true) {
				_timer += deltaTime * ActorJumpUpPressTimeRate;
				_jumpUpTimer -= deltaTime;
				if (_jumpUpTimer <= 0) {
					_jumpUpPressed = false;
					_jumpUpTopRate = 2.0f;
				}
			}

			isSlideDown = false;
			_timer -= deltaTime;
			if (_timer <= 0) {
				//SetStatus (Status_FallDown);
				//_speed.y = -1 * ActorJumpUpSpeedY * ActorJumpUpFallSpeedRate;
				pos = SetSlideDown (pos,isMoveForward,deltaTime);
				isSlideDown = true;
				Debug.LogWarning ("JumpupBlocked to Slide: Speed:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
			}

			if(isSlideDown==false) {
				isHeadHit = false;

				// 判断头上是不是顶到了东西
				detectElement = GetHeadTopMapElement (pos);
				if (detectElement != null) {
					switch (detectElement.IsBlock (MapElement.Dir_Down)) {
					case  MapElement.Block_None:
						break;
					case  MapElement.Block_Soft:
						detectElement.HitBottom (_speed.x>0);
						_timer = ActorJumpUpTopDuration;
						_targetY = pos.y + ActorJumpUpTopHeight;
						_jumpUpTopRate = 1.0f;
						pos.y = CalculateJumpUpTopHeight (_timer);
						SetStatus (Status_JumpHitTop);
						SetAnimation (Ani_JumpHitTop);
						isHeadHit = true;
						Debug.LogWarning ("JumpupBlocked Hit top soft: SpeedY:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
						break;
					case  MapElement.Block_Hard:
						pos.y = Mathf.Round (pos.y);
						_speed.y = -1 * ActorJumpUpSpeedY * ActorJumpUpFallSpeedRate;
						SetStatus (Status_FallDown);
						SetAnimation (Ani_FallDown);
						isHeadHit = true;
						Debug.LogWarning ("JumpupBlocked Hit top hard: SpeedY:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
						break;
					}
				}

				if (isHeadHit == false) {
					// 判断前面是不是空了
					detectElement = GetFrontFootMapElement (pos, isMoveForward);

					isEmpty = false;
					if (detectElement == null) {
						isEmpty = true;
					}
					if (detectElement != null) {
						if (detectElement.IsBlock (detectDir) == MapElement.Block_None) {
							isEmpty = true;
						}
					}

					if (isEmpty == true) {
						//  判断头上前面是不是空的
						detectElement = GetFrontTopMapElement (pos, isMoveForward);
						isEmpty = false;
						if (detectElement == null) {
							isEmpty = true;
						}
						if (detectElement != null) {
							if (detectElement.IsBlock (detectDir) == MapElement.Block_None) {
								isEmpty = true;
							}
						}
						if (isEmpty) {
							_speed = GetDefaultSpeed ();
							_speed.x /= 3;
							_speed.y = ActorJumpUpSpeedY;
							pos.y = (int)(pos.y + 0.5f);
							SetStatus (Status_JumpUp);
							SetAnimation (Ani_JumpUp);
							Debug.LogWarning ("JumpupBlocked to jumpup: Speed:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
						}
					}
				}
			}
			break;

		case Status_JumpTop:
			if (isMoveForward == true) {
				detectDir = MapElement.Dir_Left;
			}
			else {
				detectDir = MapElement.Dir_Right;
			}

			_timer -= deltaTime;
			pos.y = CalculateJumpUpTopHeight (_timer);
			if (_timer < 0) {
				SetStatus (Status_FallDown);
				SetAnimation (Ani_FallDown);
				_speed.y = -1 * ActorJumpUpSpeedY * ActorJumpUpFallSpeedRate;
			}

			// 判断下面是不是地面
			isGroundBelow = false;
			detectElement = GetFootBottomMapElement (pos);
			if (detectElement != null) {
				if (detectElement.IsBlock (MapElement.Dir_Up) != MapElement.Block_None) {
					pos = SetJumpToRun(pos);


					Debug.LogWarning ("JumpTop to Run:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
				}
			}

			// 判断有没有踩在Stop上面
		    isStopElement = false;
			if (detectElement != null) {
				if (detectElement.type == MapElement.Type_Stop) {
					_stopElement = (MapElementStop)detectElement;
					isStopElement = true;
				} else {
					detectElement = GetFootFrontMapElement (pos, isMoveForward);
					if (detectElement != null) {
						if (detectElement.type == MapElement.Type_Stop) {
							_stopElement = (MapElementStop)detectElement;
							isStopElement = true;
						} else {
							detectElement = GetFootFrontMapElement (pos, !isMoveForward);
							if (detectElement != null) {
								if (detectElement.type == MapElement.Type_Stop) {
									_stopElement = (MapElementStop)detectElement;
									isStopElement = true;
								}
							}
						}
					}
				}
			}
			if (isStopElement == true) {
				pos = SetStop ();
				isGroundBelow = true;
				break;
			}

			// 判断头上是不是顶到了东西
			detectElement = GetHeadTopMapElement (pos);
			if (detectElement != null) {
				switch (detectElement.IsBlock (MapElement.Dir_Down)) {
				case  MapElement.Block_None:
					break;
				case  MapElement.Block_Soft:
					detectElement.HitBottom (_speed.x>0);
					pos.y = CalculateJumpUpTopHeight (_timer);
					SetStatus (Status_JumpHitTop);
					SetAnimation (Ani_JumpHitTop);
					Debug.LogWarning ("JumpTop Hittop Soft:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
					break;
				case  MapElement.Block_Hard:
					pos.y = Mathf.Round (pos.y);
					_speed.y = -1 * ActorJumpUpSpeedY * ActorJumpUpFallSpeedRate;
					SetStatus (Status_FallDown);
					SetAnimation (Ani_FallDown);
					Debug.LogWarning ("JumpTop Hittop Hard:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
					break;
				}
			}

			// 判断前面是不是障碍物
			// 先判断前面上面是否撞上障碍
			isSlideDown = false;
			detectElement = GetFrontTopMapElement (pos,isMoveForward);
			if (detectElement != null) {
				if (detectElement.IsBlock (detectDir) != MapElement.Block_None) {
					// 前面是障碍，只能滑下去了
					if (isGroundBelow) {
						SetStatus (Status_Blocked);
						SetAnimation (Ani_Blocked);
						_isLastRunForward = isMoveForward;
						pos.y = Mathf.Round (pos.y-_speed.y*deltaTime);
						_timer = ActorBlockedDuration;
						_targetY = pos.y + ActorBlockedJumpHeight;
						pos.x -= _speed.x * deltaTime;
						Debug.LogWarning ("JumpTop To block 01:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
					} else {
						pos = SetSlideDown (pos,isMoveForward,deltaTime);
						isSlideDown = true;
						Debug.LogWarning ("JumpTop to slide 01:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
					}
					if (_jumpUpPressed) {
						DoKeyPressed (true);
					}
				}
			}

			// 前面中间有没有障碍
			if (isSlideDown == false) {
				detectElement = GetFrontMiddleMapElement (pos,isMoveForward);
				if (detectElement != null) {
					if (detectElement.IsBlock (detectDir) != MapElement.Block_None) {
						// 前面是障碍，只能滑下去了
						if (isGroundBelow) {
							SetStatus (Status_Blocked);
							SetAnimation (Ani_Blocked);
							_isLastRunForward = isMoveForward;
							pos.y = Mathf.Round (pos.y-_speed.y*deltaTime);
							_timer = ActorBlockedDuration;
							_targetY = pos.y + ActorBlockedJumpHeight;
							pos.x -= _speed.x * deltaTime;
							Debug.LogWarning ("JumpTop To block 02:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
						} else {
							pos = SetSlideDown (pos,isMoveForward,deltaTime);
							isSlideDown = true;
							Debug.LogWarning ("JumpTop to slide 02:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
						}
						if (_jumpUpPressed) {
							DoKeyPressed (true);
						}
					}
				}
			}

			// 前面上面,中间都没有障碍，判断下面
			if(isSlideDown==false) {
				detectElement = GetFrontBottomMapElement(pos,isMoveForward);
				if (detectElement != null) {
					if (detectElement.IsBlock (detectDir) != MapElement.Block_None) {
						// 上边沿没有被堵住，判断主角是不是超过对面障碍半个身高，是的话可以越过ß
						if (pos.y - (int)pos.y > 0.5f) {
							// 没有被阻挡，上到障碍上面继续跑
							pos = SetJumpToRun( pos );

							Debug.LogWarning ("JumpTop to run on top:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
						} else {
							if (isGroundBelow) {
								// 下面是地面，主角被堵住
								SetStatus (Status_Blocked);
								SetAnimation (Ani_Blocked);
								_isLastRunForward = isMoveForward;
								pos.y = Mathf.Round (pos.y-_speed.y*deltaTime);
								_timer = ActorBlockedDuration;
								_targetY = pos.y + ActorBlockedJumpHeight;
								pos.x -= _speed.x * deltaTime;
								Debug.LogWarning ("JumpTop to block 03:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());

							} else {
								// 被堵住了,下面不是地面，下滑
								pos = SetSlideDown (pos,isMoveForward,deltaTime);
								Debug.LogWarning ("JumpTop to slide  03:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
							}
						}
						if (_jumpUpPressed) {
							DoKeyPressed (true);
						}
					}
				}
			}

			break;

		case Status_JumpHitTop:
			// 头上顶到了可以顶碎的物体,继续上升一小段然后开始下落
			_timer -= deltaTime;
			pos.y = CalculateJumpUpTopHeight (_timer);
			if (_timer < ActorJumpUpTopDuration/2) {
				_speed.y = -1 * ActorJumpUpSpeedY;
				SetStatus( Status_FallDown);
				SetAnimation (Ani_FallDown);

				Debug.LogWarning( "JumpHittop to fall down:"+_speed.ToString()+ "PosX:"+pos.x.ToString()+" PosY:"+pos.y.ToString() );
			}

			break;

		case Status_FallDown:
			if (isMoveForward == true) {
				detectDir = MapElement.Dir_Left;
			} else {
				detectDir = MapElement.Dir_Right;
			}

			// 判断下面是不是地面
			isGroundBelow = false;
			isSlopeBelow = false;
			detectElement = GetFootBottomMapElement (pos);
			if (detectElement != null) {
				if (detectElement.IsBlock (MapElement.Dir_Up) != MapElement.Block_None) {
					pos = SetJumpToRun (pos);
					isGroundBelow = true;
					Debug.LogWarning ("falldown to run on ground:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
				}
				if ((detectElement.type == MapElement.Type_WallSlope01)||(detectElement.type == MapElement.Type_WallSlope01Follow)
					||(detectElement.type == MapElement.Type_WallSlope02)
					||(detectElement.type == MapElement.Type_WallSlope03)||(detectElement.type == MapElement.Type_WallSlope03Follow)
					||(detectElement.type == MapElement.Type_WallSlope04))	{
					isSlopeBelow = true;
				}
			}

			if( isSlopeBelow==false ) {
				// 判断有没有踩在Stop上面
			    isStopElement = false;
				if (detectElement != null) {
					if (detectElement.type == MapElement.Type_Stop) {
						_stopElement = (MapElementStop)detectElement;
						isStopElement = true;
					} else {
						detectElement = GetFootFrontMapElement (pos, isMoveForward);
						if (detectElement != null) {
							if (detectElement.type == MapElement.Type_Stop) {
								_stopElement = (MapElementStop)detectElement;
								isStopElement = true;
							} else {
								detectElement = GetFootFrontMapElement (pos, !isMoveForward);
								if (detectElement != null) {
									if (detectElement.type == MapElement.Type_Stop) {
										_stopElement = (MapElementStop)detectElement;
										isStopElement = true;
									}
								}
							}
						}
					}
				}
				if (isStopElement == true) {
					pos = SetStop ();
					break;
				}
			}


			if (isGroundBelow == false) {
				// 判断前角是不是进入了斜坡

				detectElement = GetFootFrontMapElement (pos, isMoveForward);
				if (detectElement != null) {
					if ((detectElement.type == MapElement.Type_WallSlope01) || (detectElement.type == MapElement.Type_WallSlope01Follow)) {
						// 碰到了斜坡
						Vector3 deltaPos = pos;
						deltaPos.x += 1.0f;
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
							Debug.LogWarning ("falldown to slope 01-1 Speed:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
						} else {
							// 向后走，前角在低处
							pos.x = deltaPos.x + 0.707f * Mathf.Sin (angle);
							pos.y = deltaPos.y + 0.707f * Mathf.Cos (angle);
							SetRunSlopeUp01 (isMoveForward);
							Debug.LogWarning ("falldown to slope 01-2 Speed:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
						}

						isSlopeBelow = true;

					} else if (detectElement.type == MapElement.Type_WallSlope02) {
						// 碰到了斜坡
						Vector3 deltaPos = pos;
						deltaPos.x += 1.0f;
						deltaPos.x = deltaPos.x - (int)deltaPos.x;
						deltaPos.y = deltaPos.x;

						deltaPos.x += detectElement.transform.position.x - 0.5f;
						deltaPos.y += detectElement.transform.position.y - 0.5f;


						float angle = (45 - Slope02Angle) * 3.1415927f / 180;
						if (isMoveForward) {
							pos.x = deltaPos.x - 0.707f * Mathf.Cos (angle);
							pos.y = deltaPos.y + 0.707f * Mathf.Sin (angle);
							SetRunSlopeUp02 (isMoveForward);
							Debug.LogWarning ("falldown to slope 02-1 Speed:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
						} else {
							// 向后走，前角在低处
							pos.x = deltaPos.x + 0.707f * Mathf.Sin (angle);
							pos.y = deltaPos.y + 0.707f * Mathf.Cos (angle);
							SetRunSlopeUp02 (isMoveForward);
							Debug.LogWarning ("falldown to slope 02-2 Speed:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
						}

						isSlopeBelow = true;
					}
					else if ((detectElement.type == MapElement.Type_WallSlope03) || (detectElement.type == MapElement.Type_WallSlope03Follow)) {
						// 碰到了斜坡
						Vector3 deltaPos = pos;
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
							Debug.LogWarning ("falldown to slope 03-1 Speed:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
						} else {
							// 向后走，前角在低处
							pos.x = deltaPos.x + 0.707f * Mathf.Sin (angle);
							pos.y = deltaPos.y + 0.707f * Mathf.Cos (angle);
							SetRunSlopeDown01 (isMoveForward);
							Debug.LogWarning ("falldown to run down 01-2 Speed:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
						}

						isSlopeBelow = true;

					} else if (detectElement.type == MapElement.Type_WallSlope04) {
						// 碰到了斜坡
						Vector3 deltaPos = pos;
						deltaPos.x = deltaPos.x - (int)deltaPos.x;
						deltaPos.y = deltaPos.x;

						deltaPos.x += detectElement.transform.position.x - 0.5f;
						deltaPos.y = detectElement.transform.position.y + 0.5f - deltaPos.y;


						float angle = (Slope02Angle-315) * 3.1415927f / 180;
						if (isMoveForward) {
							pos.x = deltaPos.x - 0.707f * Mathf.Cos (angle);
							pos.y = deltaPos.y + 0.707f * Mathf.Sin (angle);
							SetRunSlopeDown02 (isMoveForward);
							Debug.LogWarning ("falldown to slope 04-1 Speed:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
						} else {
							// 向后走，前角在低处
							pos.x = deltaPos.x + 0.707f * Mathf.Sin (angle);
							pos.y = deltaPos.y + 0.707f * Mathf.Cos (angle);
							SetRunSlopeDown02 (isMoveForward);
							Debug.LogWarning ("falldown to slope 04-2 Speed:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
						}

						isSlopeBelow = true;
					}
				}
			}


			if(isSlopeBelow==false) {
				

				// 判断前面是不是障碍物
				// 先判断头部
				isSlideDown = false;
				detectElement = GetFrontTopMapElement (pos,isMoveForward);
				if (detectElement != null) {
					if (detectElement.IsBlock (detectDir) != MapElement.Block_None) {
						if (isGroundBelow == true) {
							// 下面是地面，被阻挡住
							SetStatus (Status_Blocked);
							SetAnimation (Ani_Blocked);
							_isLastRunForward = isMoveForward;
							pos.y = Mathf.Round (pos.y-_speed.y*deltaTime);
							_timer = ActorBlockedDuration;
							_targetY = pos.y + ActorBlockedJumpHeight;
							pos.x -= _speed.x * deltaTime;
							Debug.LogWarning ("falldown to block 01:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
						} else {
							// 下面不是地面，下滑
							pos = SetSlideDown (pos, isMoveForward, deltaTime);
							isSlideDown = true;
							Debug.LogWarning ("falldown to slide  01:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
						}

					}
				}

				if (isSlideDown == false) {
					detectElement = GetFrontMiddleMapElement(pos,isMoveForward);
					if (detectElement != null) {
						if (detectElement.IsBlock (detectDir) != MapElement.Block_None) {
							// 下边撞到，判断上边
							if (isGroundBelow == false) {
								// 被阻挡了，下面是空的，下滑
								pos = SetSlideDown (pos,isMoveForward,deltaTime);
								isSlideDown = true;
								Debug.LogWarning ("falldown to slide  02:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
							} else {
								// 被阻挡了而且下面不是空的，Block
								SetStatus (Status_Blocked);
								SetAnimation (Ani_Blocked);
								_isLastRunForward = isMoveForward;
								pos.y = Mathf.Round (pos.y-_speed.y*deltaTime);
								_timer = ActorBlockedDuration;
								_targetY = pos.y + ActorBlockedJumpHeight;
								pos.x -= _speed.x * deltaTime;
								Debug.LogWarning ("falldown to block 02:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
							} 
							if (_jumpUpPressed) {
								DoKeyPressed (true);
							}
						}
					}
				}

				if (isSlideDown == false) {
					detectElement = GetFrontBottomMapElement (pos, isMoveForward);
					if (detectElement != null) {
						if (detectElement.IsBlock (detectDir) != MapElement.Block_None) {
							// 下边撞到，判断上边
							// 上边没有障碍，判断主角现在是不是高出了半个高度
							if (pos.y - (int)pos.y > 0.5f) {
								// 没有被阻挡，上到障碍上面继续跑
								pos = SetJumpToRun (pos);
								Debug.LogWarning ("falldown to run on top:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
							} else {
								if (isGroundBelow == false) {
									// 被阻挡了，下面是空的，下滑
									pos = SetSlideDown (pos, isMoveForward, deltaTime);
									isSlideDown = true;
									Debug.LogWarning ("falldown to slide  02:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
								} else {
									// 被阻挡了而且下面不是空的，Block
									SetStatus (Status_Blocked);
									SetAnimation (Ani_Blocked);
									_isLastRunForward = isMoveForward;
									pos.y = Mathf.Round (pos.y - _speed.y * deltaTime);
									_timer = ActorBlockedDuration;
									_targetY = pos.y + ActorBlockedJumpHeight;
									pos.x -= _speed.x * deltaTime;
									Debug.LogWarning ("falldown to block 03:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
								}
							} 
							if (_jumpUpPressed) {
								DoKeyPressed (true);
							}
						}
					}
				}
			}

			break;
		case Status_SlideDownFront:
		case Status_SlideDownBack:

			if (_timer >= 0) {
				_timer -= deltaTime;
				if (_timer < 0) {
					_speed.y = ActorSlideSpeedY;
				} else {
					break;
				}
			}
				// 前面是障碍，顺着滑下来
			// 判断前面是不是障碍物
			isEmpty = false;
			detectDir = MapElement.Dir_Left;
			isMoveForward = true;
			if (_status == Status_SlideDownBack) {
				detectDir = MapElement.Dir_Right;
				isMoveForward = false;
			}
				
			detectElement = GetFrontHeadMapElement (pos, isMoveForward);
			if (detectElement == null) {
				isEmpty = true;
			} else {
				if (detectElement.IsBlock (detectDir) == MapElement.Block_None) {
					isEmpty = true;
				}
			}

			if (isEmpty) {
				isEmpty = false;
				detectElement = GetFrontMiddleMapElement (pos, isMoveForward);
				if (detectElement == null) {
					isEmpty = true;
				} else {
					if (detectElement.IsBlock (detectDir) == MapElement.Block_None) {
						isEmpty = true;
					}
				}
				if (isEmpty) {
					detectElement = GetFrontBottomMapElement (pos, isMoveForward);
					if (detectElement == null) {
						isEmpty = true;
					} else {
						if (detectElement.IsBlock (detectDir) == MapElement.Block_None) {
							isEmpty = true;
						}
					}

					if (isEmpty) {
						isEmpty = false;
						// 如果前面是空的了，改为向前掉落
						_speed = GetDefaultSpeed ();
						_speed.x *= 0.6f;
						_speed.y = -1 * ActorJumpUpSpeedY;
						SetStatus (Status_FallDown);
						SetAnimation (Ani_FallDown);
						Debug.LogWarning ("slide down to falldown:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
					}
				}
			}

			// 判断下面是不是地面
			detectElement = GetFootBottomMapElement (pos);
			if (detectElement != null) {
				if (detectElement.IsBlock (MapElement.Dir_Up) != MapElement.Block_None) {
					// 下面是地面。滑下来后
					if (isEmpty == false) {
						// 前面不是空的，被阻挡
						SetStatus (Status_Blocked);
						SetAnimation (Ani_Blocked);
						_isLastRunForward = isMoveForward;
						_timer = ActorBlockedDuration;
						pos.y = Mathf.Round (pos.y - _speed.y * deltaTime);
						_targetY = pos.y + ActorBlockedJumpHeight;
						pos.x -= _speed.x * deltaTime;
						if (_jumpUpPressed) {
							DoKeyPressed (true);
						}
						Debug.LogWarning ("slide down to block:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
					}

					// 落地后，需要判断缺省运动方向有没有障碍，因为滑的墙的方向和缺省运动方向可能不一样
					bool isDefaultForward = GetDefaultSpeed().x>0;
					isEmpty = false;
					detectElement = GetFrontTopMapElement (pos, isDefaultForward);
					if (isDefaultForward == true) {
						detectDir = MapElement.Dir_Left;
					}
					else {
						detectDir = MapElement.Dir_Right;
					}
					if (detectElement == null) {
						isEmpty = true;
					} else {
						if (detectElement.IsBlock (detectDir)==MapElement.Block_None) {
							isEmpty = true;
						}
					}

					if (isEmpty == true) {
						isEmpty = false;
						detectElement = GetFrontBottomMapElement (pos, isDefaultForward);
						if (detectElement == null) {
							isEmpty = true;
						} else {
							if (detectElement.IsBlock (detectDir)==MapElement.Block_None) {
								isEmpty = true;
							}
						}
					}
					if (isEmpty == true) {
						// 前面是空的，往前跑
						pos = SetJumpToRun(pos);
						Debug.LogWarning ("slide down to run on ground:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
					}
				}
			}
			break;
		case Status_RunSlope01:
		case Status_RunSlope02:
				// 判断脚下踩的是不是还是斜坡
			detectElement = GetFootFrontMapElement (pos, isMoveForward);
			isEmpty = false;
			if (detectElement == null) {
				isEmpty = true;
			} else {
				if (detectElement.type==MapElement.Type_None){
					isEmpty = true;
				}
			}

			if (isMoveForward) {
				if (isEmpty == true) {
					detectElement = GetFootBottomMiddleMapElement (pos);
					isEmpty = false;
					if (detectElement == null) {
						isEmpty = true;
					} else {
						if (detectElement.type==MapElement.Type_None){
							isEmpty = true;
						}
					}
					if (isEmpty == true) {
						pos.y = Mathf.Round (pos.y + 0.5f);
						pos.x += 0.5f;
						_speed = GetDefaultSpeed ();
						SetStatus (Status_Run);
						SetAnimation (Ani_Run);

						Debug.LogWarning ("Run Up to run on ground:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
					}
				}
			} else {
				if (detectElement != null) {
					if ((detectElement.type == MapElement.Type_WallFixed) || (detectElement.type == MapElement.Type_Brick)) {
						pos.y = Mathf.Round (pos.y);
						pos.x -= 0.5f;
						_speed = GetDefaultSpeed ();
						SetStatus (Status_Run);
						SetAnimation (Ani_Run);
						Debug.Log ("DetectType:" + detectElement.type);
						Debug.LogWarning ("Run Down to run on ground:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
					}
				}
			}
			break;
		case Status_RunSlope03:
		case Status_RunSlope04:
				// 判断脚下踩的是不是空了
			/*detectElement = GetFootBottomMapElement (pos);
			isEmpty = false;
			if (detectElement == null) {
				isEmpty = true;
			} else {
				switch (detectElement.IsBlock(MapElement.Dir_Up)) {
				case MapElement.Type_WallSlope01:
				case MapElement.Type_WallSlope01Follow:
					break;
				case MapElement.Type_None:
					isEmpty = true;
					break;
				}
			}

			if (isEmpty == true) {
				// 脚下	
			} else {*/
			detectElement = GetFootFrontMapElement (pos, isMoveForward);
			if (isMoveForward) {
				if (detectElement != null) {
					if ((detectElement.type == MapElement.Type_WallFixed) || (detectElement.type == MapElement.Type_Brick)) {
						detectElement = GetFootFrontCornerMapElement (pos, isMoveForward);
						if (detectElement != null) {
							if ((detectElement.type == MapElement.Type_WallFixed) || (detectElement.type == MapElement.Type_Brick)) {
								pos.y = Mathf.Round (pos.y);
								pos.x += 0.5f;
								_speed = GetDefaultSpeed ();
								SetStatus (Status_Run);
								SetAnimation (Ani_Run);
								Debug.Log ("DetectType:" + detectElement.type);
								Debug.LogWarning ("Run Down to run on ground:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
							}
						}
					}
				}
			} else {
				if (detectElement == null) {
					detectElement = GetFootBottomMiddleMapElement (pos);
					isEmpty = false;
					if (detectElement == null) {
						isEmpty = true;
					} else {
						if (detectElement.type==MapElement.Type_None){
							isEmpty = true;
						}
					}

					if (isEmpty == true) {
						pos.y = Mathf.Round (pos.y + 0.5f);
						pos.x -= 0.5f;
						_speed = GetDefaultSpeed ();
						SetStatus (Status_Run);
						SetAnimation (Ani_Run);

						Debug.LogWarning ("Run Up to run on ground:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
					}
				}
			}
			break;

		case Status_SpringUp:
			if (isMoveForward == true) {
				detectDir = MapElement.Dir_Left;
			} else {
				detectDir = MapElement.Dir_Right;
			}

			// 判断头上是不是顶到了东西
			detectElement = GetHeadTopMapElement (pos);
			if (detectElement != null) {
				switch (detectElement.IsBlock (MapElement.Dir_Down)) {
				case  MapElement.Block_None:
					break;
				case  MapElement.Block_Soft:
					detectElement.HitBottom (_speed.x > 0);
					_timer = ActorJumpUpTopDuration;
					_targetY = pos.y + ActorJumpUpTopHeight;
					_jumpUpTopRate = 1.0f;
					pos.y = CalculateJumpUpTopHeight (_timer);
					SetStatus (Status_JumpHitTop);
					SetAnimation (Ani_JumpHitTop);
					Debug.LogWarning ("Jumpup Hit top hard: SpeedY:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
					break;
				case  MapElement.Block_Hard:
					pos.y = Mathf.Round (pos.y);
					_speed.y = -1 * ActorJumpUpSpeedY * ActorJumpUpFallSpeedRate;
					SetStatus (Status_FallDown);
					SetAnimation (Ani_FallDown);
					Debug.LogWarning ("Jumpup Hit top hard: SpeedY:" + _speed.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
					break;
				}
			}

			detectElement = GetFrontMapElement (pos, isMoveForward);
			bool isFrontBlocked = false;
			if (detectElement != null) {
				if (detectElement.IsBlock (detectDir) != MapElement.Block_None) {
					isFrontBlocked = true;
				}	
			}

			if (isFrontBlocked == false) {
				detectElement = GetFrontHeadMapElement (pos, isMoveForward);
				if (detectElement != null) {
					if (detectElement.IsBlock (detectDir) != MapElement.Block_None) {
						isFrontBlocked = true;
						break;
					}	
				}
			}

			if (isFrontBlocked == false) {
				detectElement = GetFrontBottomMapElement (pos, isMoveForward);
				if (detectElement != null) {
					if (detectElement.IsBlock (detectDir) != MapElement.Block_None) {
						isFrontBlocked = true;
						break;
					}	
				}
			}

			if (isFrontBlocked == true) {
				// 前面的方块不能进入，被堵住了
				pos.x = detectElement.transform.position.x + Mathf.Sign (_speed.x) * (0.5f + _width / 2);
				_speed.x = 0;
			}
			
			_speed.y += Gravity * deltaTime;
			if (_speed.y < 0) {
				_jumpUpTopRate = 1.0f;
				SetStatus (Status_JumpTop);
				SetAnimation (Ani_JumpTop);
				_speed.x = GetDefaultSpeed ().x;
				_timer = ActorJumpUpTopDuration * _jumpUpTopRate;
				_targetY = pos.y + ActorJumpUpTopHeight * _jumpUpTopRate;
				pos.y = CalculateJumpUpTopHeight (_timer);

				Debug.LogWarning ("SpringUp to JumpTop: _targetY:" + _targetY.ToString () + "PosX:" + pos.x.ToString () + " PosY:" + pos.y.ToString ());
			}
			break;
		}



		Debug.Log ("Update Out - Status" + _status.ToString () + "  Time:" + _timer.ToString () + " x:" + pos.x.ToString ()+ " y:" + pos.y.ToString () +"  Speed-x:"+_speed.x+" y:"+_speed.y );
		transform.position = pos;

		_gameMap.SetCameraPosition ();


	}

	void SetGameOver(bool isWin) {
		if (_status == Status_GameOver) {
			return;
		}
		SetStatus (Status_GameOver);
		_speed = Vector3.zero;
		_gameMap.GameOver ( transform.position, isWin );
	}

	Rect GetCollisionRect() {
		return new Rect (transform.position.x-_width/2,transform.position.y-_height/2+0.3f,_width,_height);
	}

	void CheckActiveElements() {
		List<ActiveElement> activeElements = _gameMap.GetActiveElementList ();
		ActiveElement element;
		ActiveMoveElement activeMoveElement;
		Rect actorRect = GetCollisionRect ();

		for (int m = 0; m < activeElements.Count; m++) {
			element = activeElements [m];

			if (element.IsCollised (actorRect)) {
				if (element.IsEnemy () == false) {
					// 不是敌人，直接触发
					element.DoCollised ();
				} else if (element is EnemyMoveElement) {
					// 判断是不是踩上去
					bool isDead = true;
					if ((_status == Status_FallDown)
					    || (_status == Status_SlideDownFront) || (_status == Status_SlideDownBack)
					    || ((_status == Status_RunSlope02) && (_speed.x < 0))
					    || ((_status == Status_RunSlope04) && (_speed.x > 0))) {
						if (transform.position.y > element.transform.position.y) {
							EnemyMoveElement enemy = (EnemyMoveElement)element;
							isDead = enemy.JumpOnHead ();
							if (isDead == false) {
								// 如果已经按下了跳
								if (_jumpUpPressed) {
									DoKeyPressed (true);
								} else {
									// 只是弹起来
									_jumpUpTimer = 0;
									_timer = ActorBounceUpDuration;
									_speed.y = ActorJumpUpSpeedY;
									_jumpUpPressed = true;
									SetStatus (Status_JumpUp);
									SetAnimation (Ani_JumpUp);
									_jumpUpPressTreated = true;
								}
							}
						} 
					}

					if (isDead) {
						element.DoCollised ();
						SetGameOver (false);

						return;
					}
				} else {
					element.DoCollised ();
					SetGameOver (false);

					return;
				}
			}

			if (element.IsActive () == false) {
				// 这个元素还没有激活
				if (_speed.x > 0) {
					// 主角在往右跑
					if ((element.x <= _gameMap.RightBorder + 1) && (element.y >= _gameMap.TopBorder) && (element.y <= _gameMap.BottomBorder) && (element.GetTriggerDirection () == MapElement.Dir_Left)) {
						element.SetActive ();
					}
				} else if (_speed.x < 0) {
					// 主角在往右跑
					if ((element.x >= _gameMap.LeftBorder - 1) && (element.y >= _gameMap.TopBorder) && (element.y <= _gameMap.BottomBorder) && (element.GetTriggerDirection () == MapElement.Dir_Right)) {
						element.SetActive ();
					}
				}
				if (_speed.y > 0) {
					if ((element.y >= _gameMap.TopBorder - 1) && (element.x >= _gameMap.LeftBorder) && (element.x <= _gameMap.RightBorder) && (element.GetTriggerDirection () == MapElement.Dir_Up)) {
						element.SetActive ();
					}
				} else if (_speed.y < 0) {
					if ((element.y <= _gameMap.BottomBorder + 1) && (element.x >= _gameMap.LeftBorder) && (element.x <= _gameMap.RightBorder) && (element.GetTriggerDirection () == MapElement.Dir_Down)) {
						element.SetActive ();
					}
				}
			} else if (_magnetTimer>0) {
				if (Vector2.Distance (transform.position, element.transform.position) < 4) {
					element.FlyToActor ();
				}
			}
		}
	}

	Vector3 SetJumpToRun( Vector3 pos ) {
		Vector3 speed = GetDefaultSpeed ();
		if (speed.x * _speed.x < 0) {
			// 方向不同
			_timer = ActorSlideTurnArroundDelay;
			_speed = Vector3.zero;
		} else {
			_timer = -1;
			_speed = GetDefaultSpeed ();
		}
		pos.y = Mathf.Round(pos.y-DetectAdvanceSpaceY + 0.5f);
		SetStatus (Status_Run);
		SetAnimation (Ani_Run);

		if (_jumpUpPressed) {
			DoKeyPressed (true);
		}

		return pos;
	}


	Vector3 SetSlideDown( Vector3 pos, bool isMoveForward,float deltaTime ) {
		if (isMoveForward) {
			SetStatus (Status_SlideDownFront);
			SetAnimation (Ani_SlideDown);
		} else {
			SetStatus (Status_SlideDownBack);
			SetAnimation (Ani_SlideDown);
		}
		pos.x -= _speed.x * deltaTime;
		pos.x = Mathf.Round (pos.x);
		_speed.y = 0;
		_speed.x = 0;
		_timer = ActorSlideBeginDelay;

		if (_jumpUpPressed) {
			DoKeyPressed (true);
		}
		return pos;
	}

	void SetRunSlopeUp01( bool isMoveForward ) {
		_speed = GetDefaultSpeed ();
		if (isMoveForward) {
			_speed.x *= 0.9f;
			_speed.y = _speed.x / 2;
		}
		else {
			_speed.x *= 1.1f;
			_speed.y = _speed.x / 2;
		}
		SetAnimation (Ani_RunSlope01);
		SetStatus (Status_RunSlope01);
	}

	void SetRunSlopeUp02( bool isMoveForward ) {
		_speed = GetDefaultSpeed ();
		if (isMoveForward) {
			_speed.x *= 0.75f;
			_speed.y = _speed.x;
		}
		else {
			_speed.x *= 1.2f;
			_speed.y = _speed.x;
		}
		SetAnimation (Ani_RunSlope02);
		SetStatus (Status_RunSlope02);
	}

	void SetRunSlopeDown01( bool isMoveForward ) {
		_speed = GetDefaultSpeed ();
		if (isMoveForward) {
			_speed.x *= 1.1f;
			_speed.y = _speed.x / -2;
		}
		else {
			_speed.x *= 0.9f;
			_speed.y = _speed.x / -2;
		}
		SetAnimation (Ani_RunSlope03);
		SetStatus (Status_RunSlope03);
	}

	void SetRunSlopeDown02( bool isMoveForward ) {
		_speed = GetDefaultSpeed ();
		if (isMoveForward) {
			_speed.x *= 1.2f;
			_speed.y = _speed.x * -1;
		}
		else {
			_speed.x *= 0.75f;
			_speed.y = _speed.x * -1;
		}
		SetAnimation (Ani_RunSlope04);
		SetStatus (Status_RunSlope04);
	}

	Vector3 SetStop(  ) {
		bool isForward = GetDefaultSpeed ().x > 0;
		_stopElement.SetRunForward (isForward);

		_speed = Vector3.zero;
		SetAnimation (Ani_Stop);
		SetStatus (Status_Stop);

		Vector3 result = _gameMap.GetWorldPositionByMapPoint (_stopElement.x, _stopElement.y);
		result.y += 1;
		return result;
	}

	MapElement GetHeadTopMapElement(Vector3 pos) {
		Vector3 detectPos = pos;
		detectPos.y = pos.y - 0.5f + _height+DetectAdvanceSpaceY;
		MapPoint detectMapPoint = _gameMap.GetMapPointOfWorldPosition (detectPos);
		return _gameMap.GetMapElementByMapPoint (detectMapPoint.x, detectMapPoint.y);
	}

	MapElement GetFrontTopMapElement(Vector3 pos, bool isForward) {
		Vector3 detectPos = pos;
		if (isForward) {
			detectPos.x += _width / 2 + DetectAdvanceSpaceX;
		} else {
			detectPos.x -= (_width / 2 + DetectAdvanceSpaceX);
		}
		detectPos.y = pos.y - 0.5f + _height-0.01f;
		MapPoint detectMapPoint = _gameMap.GetMapPointOfWorldPosition (detectPos);
		return _gameMap.GetMapElementByMapPoint (detectMapPoint.x, detectMapPoint.y);
	}

	MapElement GetFrontMapElement(Vector3 pos, bool isForward) {
		Vector3 detectPos = pos;
		if (isForward) {
			detectPos.x += _width / 2 + DetectAdvanceSpaceX;
		} else {
			detectPos.x -= (_width / 2 + DetectAdvanceSpaceX);
		}
		MapPoint detectMapPoint = _gameMap.GetMapPointOfWorldPosition (detectPos);
		return _gameMap.GetMapElementByMapPoint (detectMapPoint.x, detectMapPoint.y);
	}

	MapElement GetFrontMiddleMapElement(Vector3 pos, bool isForward) {
		Vector3 detectPos = pos;
		if (isForward) {
			detectPos.x += _width / 2 + DetectAdvanceSpaceX;
		} else {
			detectPos.x -= (_width / 2 + DetectAdvanceSpaceX);
		}
		detectPos.y = pos.y - 0.5f + _height/2;
		MapPoint detectMapPoint = _gameMap.GetMapPointOfWorldPosition (detectPos);
		return _gameMap.GetMapElementByMapPoint (detectMapPoint.x, detectMapPoint.y);
	}

	MapElement GetFrontFootMapElement(Vector3 pos, bool isForward) {
		Vector3 detectPos = pos;
		if (isForward) {
			detectPos.x += _width / 2 + DetectAdvanceSpaceX;
		} else {
			detectPos.x -= (_width / 2 + DetectAdvanceSpaceX);
		}
		detectPos.y = pos.y - 0.35f;
		MapPoint detectMapPoint = _gameMap.GetMapPointOfWorldPosition (detectPos);
		return _gameMap.GetMapElementByMapPoint (detectMapPoint.x, detectMapPoint.y);
	}

	MapElement GetFrontHeadMapElement(Vector3 pos, bool isForward) {
		Vector3 detectPos = pos;
		if (isForward) {
			detectPos.x += _width / 2 + DetectAdvanceSpaceX;
		} else {
			detectPos.x -= (_width / 2 + DetectAdvanceSpaceX);
		}
		detectPos.y = pos.y - 0.5f+_height-0.15f;
		MapPoint detectMapPoint = _gameMap.GetMapPointOfWorldPosition (detectPos);
		return _gameMap.GetMapElementByMapPoint (detectMapPoint.x, detectMapPoint.y);
	}

	MapElement GetFrontBottomMapElement(Vector3 pos, bool isForward) {
		Vector3 detectPos = pos;
		if (isForward) {
			detectPos.x += _width / 2 + DetectAdvanceSpaceX;
		} else {
			detectPos.x -= (_width / 2 + DetectAdvanceSpaceX);
		}
		detectPos.y = pos.y - 0.5f+0.01f;
		MapPoint detectMapPoint = _gameMap.GetMapPointOfWorldPosition (detectPos);
		return _gameMap.GetMapElementByMapPoint (detectMapPoint.x, detectMapPoint.y);
	}

	MapElement GetFootBottomMapElement(Vector3 pos) {
		Vector3 detectPos = pos;
		float angle = transform.localEulerAngles.z*3.1415927f/180;
		detectPos.x += (0.5f+ DetectAdvanceSpaceY)*Mathf.Sin(angle);
		detectPos.y -= (0.5f+ DetectAdvanceSpaceY)*Mathf.Cos(angle);

		MapPoint detectMapPoint = _gameMap.GetMapPointOfWorldPosition (detectPos);
		return _gameMap.GetMapElementByMapPoint (detectMapPoint.x, detectMapPoint.y);
	}

	MapElement GetFootBottomMiddleMapElement(Vector3 pos) {
		Vector3 detectPos = pos;
		float angle = transform.localEulerAngles.z*3.1415927f/180;
		detectPos.x += 0.5f*Mathf.Sin(angle);
		detectPos.y -= 0.5f*Mathf.Cos(angle);

		MapPoint detectMapPoint = _gameMap.GetMapPointOfWorldPosition (detectPos);
		return _gameMap.GetMapElementByMapPoint (detectMapPoint.x, detectMapPoint.y);
	}

	MapElement GetFootBottomDownMapElement(Vector3 pos) {
		Vector3 detectPos = pos;
		detectPos.y -= 1.0f;

		MapPoint detectMapPoint = _gameMap.GetMapPointOfWorldPosition (detectPos);
		return _gameMap.GetMapElementByMapPoint (detectMapPoint.x, detectMapPoint.y);
	}

	MapElement GetFootMapElement( Vector3 pos ) {
		Vector3 detectPos = pos;
		float angle = transform.localEulerAngles.z*3.1415927f/180;

		detectPos.x += 0.5f*Mathf.Sin(angle);
		detectPos.y -= 0.5f*Mathf.Cos(angle);

		MapPoint detectMapPoint = _gameMap.GetMapPointOfWorldPosition (detectPos);
		return _gameMap.GetMapElementByMapPoint (detectMapPoint.x, detectMapPoint.y);
	}

	MapElement GetFootFrontMapElement( Vector3 pos, bool isForward ) {
		Vector3 detectPos = pos;
		float angle = (45-transform.localEulerAngles.z)*3.1415927f/180;

		if (isForward) {
			detectPos.x += 0.707f * Mathf.Cos (angle);
			detectPos.y -= 0.707f * Mathf.Sin (angle);
		} else {
			detectPos.x -= 0.707f * Mathf.Sin (angle);
			detectPos.y -= 0.707f * Mathf.Cos (angle);
		}

		MapPoint detectMapPoint = _gameMap.GetMapPointOfWorldPosition (detectPos);
		MapElement result = _gameMap.GetMapElementByMapPoint (detectMapPoint.x, detectMapPoint.y);
		/*if (result != null) {
			Debug.Log ("GetFootFrontMapElement:  POS(" + pos.x.ToString () + "," + pos.y.ToString () + ")  DP(" + detectPos.x.ToString () + "," + detectPos.y.ToString () + ")  MP:" + detectMapPoint + "  Type:" + result.type.ToString ());
		}
		else {
			Debug.Log ("GetFootFrontMapElement:  POS(" + pos.x.ToString () + "," + pos.y.ToString () + ")  DP(" + detectPos.x.ToString () + "," + detectPos.y.ToString () + ")  MP:" + detectMapPoint + "  Type:null");
		}*/

		return result;
	}

	MapElement GetFootFrontCornerMapElement( Vector3 pos, bool isForward ) {
		Vector3 detectPos = pos;
		float angle = (45-transform.localEulerAngles.z)*3.1415927f/180;

		if (isForward) {
			detectPos.x += 0.707f * Mathf.Cos (angle);
			detectPos.y -= 0.707f * Mathf.Sin (angle);
			detectPos.x += 0.2f;
			detectPos.y -= 0.1f;
		} else {
			detectPos.x -= 0.707f * Mathf.Sin (angle);
			detectPos.y -= 0.707f * Mathf.Cos (angle);
			detectPos.x -= 0.2f;
			detectPos.y -= 0.1f;
		}

		MapPoint detectMapPoint = _gameMap.GetMapPointOfWorldPosition (detectPos);
		MapElement result = _gameMap.GetMapElementByMapPoint (detectMapPoint.x, detectMapPoint.y);
		/*if (result != null) {
			Debug.Log ("GetFootFrontMapElement:  POS(" + pos.x.ToString () + "," + pos.y.ToString () + ")  DP(" + detectPos.x.ToString () + "," + detectPos.y.ToString () + ")  MP:" + detectMapPoint + "  Type:" + result.type.ToString ());
		}
		else {
			Debug.Log ("GetFootFrontMapElement:  POS(" + pos.x.ToString () + "," + pos.y.ToString () + ")  DP(" + detectPos.x.ToString () + "," + detectPos.y.ToString () + ")  MP:" + detectMapPoint + "  Type:null");
		}*/

		return result;
	}


	public float CalculateJumpUpTopHeight( float time ) {
		float deltaTime=0;
		float jumpUpTopDuration = ActorJumpUpTopDuration * _jumpUpTopRate;
		if (time >  jumpUpTopDuration/ 2) {
			deltaTime = time - jumpUpTopDuration / 2;
		} else {
			deltaTime = jumpUpTopDuration / 2 - time;
		}

		return _targetY - ActorJumpUpTopHeight*_jumpUpTopRate * deltaTime / (jumpUpTopDuration / 2);
	}


	public void StartGame() {
		_speed.x = ActorSpeedX;
		_magnetTimer = -1;

		SetStatus( Status_Run );
		SetAnimation (Ani_Run);
	}

	public void SetStartFootWorldPosition( Vector3 pos ) {
		
	}

	public void KeyPressed( bool flag ) {
		Debug.Log ("KeyPressed:" + flag);
		_jumpUpPressTreated = false;
		DoKeyPressed (flag);
	}

	void DoKeyPressed( bool flag ) {
		Debug.Log ("DoKeyPressed:" + _jumpUpPressTreated);
		if (flag == true) {
			if (_jumpUpPressTreated == false) {
				switch (_status) {
				case Status_Run:
					{
						MapElement detectElement = GetFootBottomDownMapElement (transform.position);
						if (detectElement != null) {
							if ((detectElement.type == MapElement.Type_Spring) || (detectElement.type == MapElement.Type_SpringFollow)) {
								SetStatus (Status_SpringUp);
								_timer = ActorJumpUpDuration+ActorJumpUpPressDuration+ActorJumpUpDuration;
								_speed.y = ActorJumpUpSpeedY*2;
								Gravity = -50;
								_speed.x *= 1 / 3;
								break;
							}
						}
						_jumpUpTimer = ActorJumpUpPressDuration;
						_timer = ActorJumpUpDuration;
						_speed.y = ActorJumpUpSpeedY;
						_jumpUpPressed = true;
						SetStatus (Status_JumpUp);
						SetAnimation (Ani_JumpUp);
						_jumpUpPressTreated = true;
						Debug.LogWarning ("Key Pressed: Jump Up");
						break;
					}
				case Status_RunSlope01:
				case Status_RunSlope03:
				case Status_RunSlope04:
				case Status_RunSlope02:
					_jumpUpTimer = ActorJumpUpPressDuration;
					_timer = ActorJumpUpDuration;
					_speed.y = ActorJumpUpSpeedY;
					_jumpUpPressed = true;
					SetStatus (Status_JumpUp);
					SetAnimation (Ani_JumpUp);
					_jumpUpPressTreated = true;
					Debug.LogWarning ("Key Pressed: Jump Up");
					break;
				case Status_Blocked:
					{
						MapElement detectElement = GetFootBottomDownMapElement (transform.position);
						if (detectElement != null) {
							if ((detectElement.type == MapElement.Type_Spring) || (detectElement.type == MapElement.Type_SpringFollow)) {
								SetStatus (Status_SpringUp);
								_speed.y = ActorJumpUpSpeedY*2;
								Gravity = -50;
								_speed.x *= 1 / 3;
								break;
							}
						}
						_jumpUpTimer = ActorJumpUpPressDuration;
						_timer = ActorJumpUpDuration;
						_speed.y = ActorJumpUpSpeedY;
						_speed.x = 0;
						_jumpUpPressed = true;
						SetStatus (Status_JumpupBlocked);
						SetAnimation (Ani_JumpupBlocked);
						Debug.LogWarning ("Key Pressed: Jump Up Blocked");
						_jumpUpPressTreated = true;
						break;
					}
				case Status_SlideDownFront:
					_jumpUpTimer = ActorJumpUpPressDuration;
					_timer = ActorJumpUpDuration;
					_speed.y = ActorJumpUpSpeedY;
					_speed.x = -1 * ActorSpeedX;
					_jumpUpPressed = true;
					SetStatus (Status_JumpUp);
					SetAnimation (Ani_JumpUp);
					Debug.LogWarning ("Key Pressed: Jump Up Backward");
					_jumpUpPressTreated = true;
					break;
				case Status_SlideDownBack:
					_jumpUpTimer = ActorJumpUpPressDuration;
					_timer = ActorJumpUpDuration;
					_speed.y = ActorJumpUpSpeedY;
					_speed.x = ActorSpeedX;
					_jumpUpPressed = true;
					SetStatus (Status_JumpUp);
					SetAnimation (Ani_JumpUp);
					Debug.LogWarning ("Key Pressed: Jump Up forward");
					_jumpUpPressTreated = true;
					break;
				case Status_Stop:
					if (_stopElement.GetSubtype () == MapElementStop.StopType_Direct) {
						_stopTimer = 0.15f;
						_speed = GetDefaultSpeed ();
						SetStatus (Status_Run);
						SetAnimation (Ani_Run);
						_jumpUpPressTreated = true;
						Debug.LogWarning ("Key Pressed: run from stop");
						_stopElement.ReleaseFromStop ();
					} else {
						_jumpUpTimer = ActorJumpUpPressDuration;
						_timer = ActorJumpUpDuration;
						_speed = GetDefaultSpeed ();
						_speed.y = ActorJumpUpSpeedY;
						_jumpUpPressed = true;
						SetStatus (Status_JumpUp);
						SetAnimation (Ani_JumpUp);
						_jumpUpPressTreated = true;
						Debug.LogWarning ("Key Pressed: Jump Up from stop");
						_stopElement.ReleaseFromStop ();
					}
					break;
				default:
					_jumpUpPressed = true;
					_jumpUpPressTreated = false;
					break;
				}
			}
		} else {
			_jumpUpPressed = false;
			_jumpUpPressTreated = true;
			_jumpUpTopRate = 2.0f - _jumpUpTimer / ActorJumpUpTopDuration;
		}
	}

	public void SetStatus( int vStatus ) {
		_status = vStatus;
	}

	public void SetAnimation( int vAni ) {
		switch (_animation) {
		case Ani_Idle:
			switch (vAni) {
			case Ani_Idle:
			case Ani_Run:
			case Ani_JumpUp:
			case Ani_JumpTop:
			case Ani_FallDown:
			case Ani_SlideDown:
			case Ani_RunSlope01:
			case Ani_RunSlope02:
			case Ani_RunSlope03:
			case Ani_RunSlope04:
				break;
			}
			break;
		case Ani_Run:
		case Ani_FallDown:
		case Ani_JumpTop:
			switch (vAni) {
			case Ani_Idle:
			case Ani_Run:
			case Ani_JumpUp:
			case Ani_JumpTop:
			case Ani_FallDown:
			case Ani_SlideDown:
				break;
			case Ani_RunSlope01:
				//DOTween.Play (transform.DORotate (new Vector3 (0, 0, 30), 0.06f));
				transform.localEulerAngles = new Vector3(0,0,Slope01Angle);
				break;
			case Ani_RunSlope03:
				transform.localEulerAngles = new Vector3(0,0,-1*Slope01Angle);
				break;
			case Ani_RunSlope02:
				transform.localEulerAngles = new Vector3 (0, 0, Slope02Angle);
				break;
			case Ani_RunSlope04:
				transform.localEulerAngles = new Vector3(0,0,-1*Slope02Angle);
				break;
			}
			break;
		case Ani_RunSlope01:
		case Ani_RunSlope03:
		case Ani_RunSlope02:
		case Ani_RunSlope04:
			switch (vAni) {
			case Ani_Idle:
			case Ani_Run:
			case Ani_JumpUp:
			case Ani_FallDown:
				//DOTween.Play (transform.DORotate (Vector3.zero, 0.06f));
				transform.localEulerAngles = Vector3.zero;
				break;
			case Ani_JumpTop:
			case Ani_SlideDown:
			case Ani_RunSlope01:
			case Ani_RunSlope02:
			case Ani_RunSlope03:
			case Ani_RunSlope04:
				break;
			}
			break;
		}
		_animation = vAni;
	}

	// 这个函数用于取得关卡的缺省速度。马里奥跑酷的大部分关卡缺省是向右跑的，个别关卡是按前面运动的方向。
	// 这里通过这个函数确定缺省运动方向，可以返回
	// 注意这个函数判断前面的运动方向需要使用前面状态和速度，所以必须在修改这两个变量前调用。
	Vector3 GetDefaultSpeed(){
		if (_gameMap.IsAlwaysForward) {
			return new Vector3 (ActorSpeedX, 0, 0);
		}

		if (_status == Status_JumpupBlocked) {
			int sign = 1;
			if (_isLastRunForward == false) {
				sign = -1;
			}

			return new Vector3 (sign * ActorSpeedX, 0, 0);
		}

		return new Vector3 (Mathf.Sign(_speed.x) * ActorSpeedX, 0, 0);
	}

	public void SetMagnet() {
		_magnetTimer = 8;
	}
}
