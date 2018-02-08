using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void FuncCallback();

public class GameController : MonoBehaviour {

	// 触摸相关处理
	public struct TouchInfo
	{
		public TouchPhase phase;
		public Vector2 position;
		public Vector2 deltaPosition;
		public float deltaTime;
		public int fingerId;
	}

	int _touchCount;
	TouchInfo[] _touches;
	float _lastMouseTime;
	Vector2 _lastMousePosition;
	bool _mousePressed;



	GameMap _gameMap;

	public const int Status_MainPage = 0;
	public const int Status_GameStarting = 1; 
	public const int Status_Playing = 2;

	int _status;

	void Awake() {
		Application.targetFrameRate = 60;

		_touches = new TouchInfo[8];
		for (int m = 0; m < 8; m++) {
			_touches [m] = new TouchInfo ();
		}
	}

	// Use this for initialization
	void Start () {
		

		_mousePressed = false;

		_gameMap = GameMap.instance;


	}

	// Update is called once per frame
	void Update () {
		// Touch control
		if (Application.platform == RuntimePlatform.Android ||
			Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_touchCount = Input.touchCount;
			if (_touchCount > 0)
			{
				for (int i=0; i<_touchCount; i++)
				{
					_touches[i].phase = Input.touches[i].phase;
					_touches[i].fingerId = Input.touches[i].fingerId;
					_touches[i].position = new Vector2(Input.touches[i].position.x, Screen.height - Input.touches[i].position.y);
					_touches[i].deltaPosition = new Vector2(Input.touches[i].deltaPosition.x, -Input.touches[i].deltaPosition.y);
					_touches[i].deltaTime = Input.touches[i].deltaTime;
				}
			}

		}
		else
		{
			if (Input.GetMouseButtonDown(0))
			{
				_touchCount = 1;
				_touches[0].phase = TouchPhase.Began;
				_touches[0].fingerId = 1;
				_touches[0].position = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);

				_mousePressed = true;
				_lastMouseTime = Time.time;
			}
			else if (_mousePressed && Input.GetMouseButtonUp(0))
			{
				_touchCount = 1;
				_touches[0].phase = TouchPhase.Ended;
				_touches[0].fingerId = 1;
				_touches[0].position = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
				_touches[0].deltaPosition = _touches[0].position - _lastMousePosition;
				_touches[0].deltaTime = Time.time - _lastMouseTime;
				_lastMousePosition = _touches[0].position;

				_mousePressed = false;
			}
			else if (_mousePressed && Input.GetMouseButton(0))
			{
				_touchCount = 1;
				_touches[0].phase = TouchPhase.Moved;
				_touches[0].fingerId = 1;
				_touches[0].position = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
				_touches[0].deltaPosition = _touches[0].position - _lastMousePosition;
				_touches[0].deltaTime = Time.time - _lastMouseTime;
				_lastMousePosition = _touches[0].position;

				_lastMouseTime = Time.time;
			}
			else
			{
				if (_mousePressed)
				{
					_touchCount = 1;
					_touches[0].phase = TouchPhase.Ended;
					_touches[0].fingerId = 1;
					_touches[0].position = _lastMousePosition;
					_touches[0].deltaPosition = Vector2.zero;
					_touches[0].deltaTime = 0;

					_mousePressed = false;
				}
				else
				{
					_touchCount = 0;
				}
			}
			/* 键盘的处理可以在这里。
            if (Input.GetKeyDown(KeyCode.A))
            {
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
            }
            */
		}

		if (_touchCount == 1) {
			switch (_touches [0].phase) {
			case TouchPhase.Began:
				_gameMap.KeyPressed (true);
				break;
			case TouchPhase.Ended:
				_gameMap.KeyPressed (false);
				break;
			}
		}

	}

	public void HideScreenCoverCompleted() {
		_gameMap.StartGame ();
	}
}
