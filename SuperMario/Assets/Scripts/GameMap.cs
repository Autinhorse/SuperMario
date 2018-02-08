using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using UnityEngine.UI;

using DG.Tweening;

class MapElementSegmentData {
	public LevelSegment segment;
	public int x;
	public int y;
}

public class GameMap : MonoBehaviour {

	public const int Status_MainUI = 0;
	public const int Status_Starting = 1;
	public const int Status_Playing = 2;
	public const int Status_GameOverWin = 3;
	public const int Status_GameOverDead = 4;

	public static GameMap instance {
		get {
			return _instance;
		}
	}
	private static GameMap _instance;

	[Header("----------Game Control----------")]
	public bool IsAlwaysForward;

	[Header("----------Game Object----------")]
	public Camera MainCamera;
	public GameObject GoGameMap;
	public ActorController Actor;

	public SpriteRenderer SptScreenCoverTop;
	public SpriteRenderer SptScreenCoverLeft;
	public SpriteRenderer SptScreenCoverCenter;
	public SpriteRenderer SptScreenCoverRight;
	public SpriteRenderer SptScreenCoverBottom;


	[Header("----------MapElement----------")]
	public GameObject GoMapElementEmpty;
	public GameObject GoMapElementWallFixed;
	public GameObject GoMapElementWallBoard;
	public GameObject GoMapElementCloud;
	public GameObject GoMapElementCoin;
	public GameObject GoMapElementBrick;
	public GameObject GoMapElementCoinArrow;
	public GameObject GoMapElementStop;
	public GameObject GoMapElementWallSlopeUp01;
	public GameObject GoMapElementWallSlopeUp01Follow;
	public GameObject GoMapElementWallSlopeUp02;
	public GameObject GoMapElementWallSlopeDown01;
	public GameObject GoMapElementWallSlopeDown01Follow;
	public GameObject GoMapElementWallSlopeDown02;
	public GameObject GoMapElementWallSlopeBottom;
	public GameObject GoMapElementEnd;
	public GameObject GoMapElementEndFollow;
	public GameObject GoMapElementSpring;
	public GameObject GoMapElementSpringFollow;
	public GameObject GoMapElementPiranhaFollow;

	public GameObject GoMapElementEnemyMushroom;

	public GameObject GoMapElementPlaceHolder;
	public GameObject GoMapElementPillarTop;
	public GameObject GoMapElementPillarBar;
	public GameObject GoMapElementQuestion;
	public GameObject GoMapElementMushroom;
	public GameObject GoMapElementBounceAward;
	public GameObject GoMapElementCoinEject;
	public GameObject GoMapElementPiranha;


	[Header("----------Colors----------")]
	public Color MapElementMainColor;

	[Header("----------Sprites----------")]
	public Sprite SptCoinArrow01;
	public Sprite SptCoinArrow02;

	public Sprite SptStop01;
	public Sprite SptStop02;
	public Sprite SptStopArrow01;
	public Sprite SptStopArrow02;

	public Sprite SptQuestionBlock;
	public Sprite SptQuestionBrick;
	public Sprite SptWallFixed;

	public Sprite SptAwardStar;
	public Sprite SptAwardLife;

	[Header("----------Duration----------")]
	public float ScreenCoverMoveDuration;

	[Header("----------UI----------")]
	public RectTransform[] RectSelectUIWorlds;
	public Image ImgSelectUIBackground;
	public RectTransform RectPlayUITopBar;
	public RectTransform RectPlayUIPausePanel;
	public Text TxtPlayUIGo;
	public Text TxtPlayUILifeValue;
	public Text TxtPlayUITimeValue;
	public Text TxtPlayUICoinValue;
	public Image[] ImgPlayUIExtraCoin;
	public Image ImgTopCover;

	CreateLevels _levels;

	MapElement[,] _mapElements;
	MapElementSegmentData[,] _mapElementSegment;
	List<ActiveElement> _activeElements;

	List<ActiveElement> _tempElements;

	int _levelWidth;
	int _levelHeight;

	int _status;

	int _lives;
	int _coins;
	float _timeValue;

	LevelSegment _currentSegment;

	public int LeftBorder {
		get {
			return (int) MainCamera.transform.position.x;
		}
	}

	public int RightBorder {
		get {
			return (int) (MainCamera.transform.position.x+_screenWidth);
		}
	}
		
	public int TopBorder {
		get {
			return (int) (-1*MainCamera.transform.position.y);
		}
	}

	public int BottomBorder {
		get {
			return (int) (-1*MainCamera.transform.position.y+_screenHeight);
		}
	}

	int _screenWidth;
	int _screenHeight;

	int _screenPixelWidth;
	int _screenPixelHeight;

	float _timer;
	int _actorMapPosX;
	int _actorMapPosY;

	void Awake() {
		_instance = this;

		_screenHeight = 27;

		float screenRate = 1.0f * Screen.width / Screen.height;
		if (screenRate > 0.72f) {
			// 3:4 screen
			_screenWidth = 18;
		} else if (screenRate > 0.6f) {
			// 2:3 screen 
			_screenWidth = 16;
		} else if (screenRate > 0.65f) {
			// 2:3 screen 
			_screenWidth = 16;
		} else if (screenRate > 0.56f) {
			// 9:16 or 10:16 screen 
			_screenWidth = 15;
		} else {
			// 9:18 or even less
			_screenWidth = 12;
		}

		_screenPixelHeight = 1920;
		_screenPixelWidth = (int)(_screenPixelHeight * screenRate);

		_activeElements = new List<ActiveElement> ();

		_tempElements= new List<ActiveElement> ();

		_levels = new CreateLevels ();

	}

	// Use this for initialization
	void Start () {
		
		RectPlayUIPausePanel.localPosition = new Vector3 (0, _screenPixelHeight, 0);
		RectPlayUITopBar.localPosition = new Vector3 (0, _screenPixelHeight/2+128, 0);

		TxtPlayUIGo.gameObject.SetActive (false);

		Actor.gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate() {
		if (_status == Status_Playing) {
			for (int m = 0; m < _activeElements.Count; m++) {
				if (_activeElements [m].IsActive ()) {
					_activeElements [m].DoFixedUpdate ();
				}
			}
			Vector3 centerM;
			Vector3 centerN;
			ActiveMoveElement moveElementM;
			ActiveMoveElement moveElementN;
			Vector3[] cornerPoints = new Vector3[4];

			for (int m = 0; m < _activeElements.Count; m++) {
				if ((_activeElements [m].IsColliseWithOthers())&&(_activeElements [m].IsActive())&&(_activeElements [m].IsAlive())) {
					moveElementM = (ActiveMoveElement)_activeElements [m];
					centerM = moveElementM.transform.position;
					cornerPoints [0] = new Vector2 (centerM.x - moveElementM.colliseWidth / 2, centerM.y - moveElementM.colliseHeight / 2);
					cornerPoints [1] = new Vector2 (centerM.x + moveElementM.colliseWidth / 2, centerM.y - moveElementM.colliseHeight / 2);
					cornerPoints [2] = new Vector2 (centerM.x - moveElementM.colliseWidth / 2, centerM.y + moveElementM.colliseHeight / 2);
					cornerPoints [3] = new Vector2 (centerM.x + moveElementM.colliseWidth / 2, centerM.y + moveElementM.colliseHeight / 2);
					for (int n = m+1; n < _activeElements.Count; n++) {
						if ((_activeElements [n].IsColliseWithOthers())&&(_activeElements [n].IsActive())&&(_activeElements [n].IsAlive())) {
							moveElementN = (ActiveMoveElement)_activeElements [n];
							centerN = moveElementN.transform.position;
							if (_activeElements [n].IsColliseWithOthers ()) {
								bool isCollised = false;
								Rect boxN = new Rect (centerN.x - moveElementN.colliseWidth/2, centerN.y - moveElementN.colliseHeight/2, moveElementN.colliseWidth, moveElementN.colliseHeight);
								// 判断碰撞
								if ((boxN.Contains (cornerPoints [0])) || (boxN.Contains (cornerPoints [1])) || (boxN.Contains (cornerPoints [2])) || (boxN.Contains (cornerPoints [3]))) {
									// 有碰撞
									moveElementM.CollisePushBack (false);
									moveElementN.CollisePushBack (true);
								} 
							}
						}
					}
				}
			}

			int oldTime = (int)_timeValue;
			_timeValue -= Time.fixedDeltaTime;
			if (oldTime != (int)_timeValue) {
				ShowTimeValue ();
			}
		}
	}

	void ShowTimeValue() {
		string timeValue = "";
		if (_timeValue < 10) {
			timeValue = "00" +((int) _timeValue).ToString ();
		} else if (_timeValue < 100) {
			timeValue = "0" + ((int)_timeValue).ToString ();
		} else {
			timeValue = ((int)_timeValue).ToString ();
		}
		TxtPlayUITimeValue.text = timeValue;
	}

	public bool ChangeLife( int deltaLife ) {
		_lives += deltaLife;
		TxtPlayUILifeValue.text = _lives.ToString ();

		return _lives <= 0;
	}


	public void ChangeCoin( int deltaCoin ) {
		_coins += deltaCoin;

		if (_coins < 0) {
			_coins = 0;
		}

		string coins = "";

		if (_coins < 10) {
			coins = "00" + _coins.ToString ();
		} else if (_coins < 100) {
			coins = "0" + _coins.ToString ();
		} else {
			coins = _coins.ToString ();
		}
		TxtPlayUICoinValue.text = coins;

	}

	public void CreateLevel( LevelDef levelDef ) {

		if (levelDef == null) {
			Debug.LogError ("LevelDef data is null, cannot create level!");
			return;
		}

		_activeElements.Clear ();

		// Calculate width and height of level
		_levelWidth = 0;
		_levelHeight = 0;
		for (int m = 0; m < levelDef.segments.Count; m++) {
			if (_levelWidth < levelDef.segments [m].segmentStartPos.x + levelDef.segments [m].width) {
				_levelWidth = levelDef.segments [m].segmentStartPos.x + levelDef.segments [m].width;
			}
			if (_levelHeight < levelDef.segments [m].segmentStartPos.y + levelDef.segments [m].height) {
				_levelHeight = levelDef.segments [m].segmentStartPos.y + levelDef.segments [m].height;
			}
		}

		_mapElements = new MapElement[_levelWidth, _levelHeight];
		_mapElementSegment = new MapElementSegmentData[_levelWidth, _levelHeight];

		// Create Level Elements
		for (int m = 0; m < levelDef.segments.Count; m++) {
			LevelSegment segment = levelDef.segments [m];
			MapPoint mapPos = new MapPoint(0,0);
			int elementType;
			GameObject targetGO;

			ActiveElement activeElement;

			for (int x = 0; x < segment.width; x++) {
				for (int y = 0; y < segment.height; y++) {
					mapPos.x = segment.segmentStartPos.x + x;
					mapPos.y = segment.segmentStartPos.y + y;

					activeElement = null;

					elementType = Encoding.Default.GetBytes(segment.data[y].Substring(x,1))[0];

					if (elementType <= 57) {
						elementType -= 48;
					}
					else if (elementType <= 90) {
						elementType -= 55;
					}
					else if (elementType <= 122) {
						elementType -= 61;
					}

					switch (elementType) {
					case MapElement.Type_None:
						_mapElements [mapPos.x, mapPos.y] = null;
						/*
						targetGO = (GameObject)GameObject.Instantiate (GoMapElementEmpty);
						targetGO.transform.SetParent (GoGameMap.transform);
						targetGO.transform.localPosition = GetWorldPositionByMapPoint (mapPos.x , mapPos.y);
						targetGO.SetActive (true);

						_mapElements [mapPos.x, mapPos.y] = (MapElement)targetGO.GetComponent<MapElement> ();*/
						break;
					case MapElement.Type_WallFixed:
						targetGO = (GameObject)GameObject.Instantiate (GoMapElementWallFixed);
						targetGO.transform.SetParent (GoGameMap.transform);
						targetGO.transform.localPosition = GetWorldPositionByMapPoint (mapPos.x , mapPos.y);
						targetGO.SetActive (true);

						_mapElements [mapPos.x, mapPos.y] = (MapElement)targetGO.GetComponent<MapElement> ();
						_mapElements [mapPos.x, mapPos.y].SetColor (MapElementMainColor);
						break;
					case MapElement.Type_WallBoard:
						targetGO = (GameObject)GameObject.Instantiate (GoMapElementWallBoard);
						targetGO.transform.SetParent (GoGameMap.transform);
						targetGO.transform.localPosition = GetWorldPositionByMapPoint (mapPos.x , mapPos.y);
						targetGO.SetActive (true);

						_mapElements [mapPos.x, mapPos.y] = (MapElement)targetGO.GetComponent<MapElement> ();
						_mapElements [mapPos.x, mapPos.y].SetColor (MapElementMainColor);
						break;
					case MapElement.Type_Cloud:
						targetGO = (GameObject)GameObject.Instantiate (GoMapElementCloud);
						targetGO.transform.SetParent (GoGameMap.transform);
						targetGO.transform.localPosition = GetWorldPositionByMapPoint (mapPos.x , mapPos.y);
						targetGO.SetActive (true);

						_mapElements [mapPos.x, mapPos.y] = (MapElement)targetGO.GetComponent<MapElement> ();
						_mapElements [mapPos.x, mapPos.y].SetColor (MapElementMainColor);
						break;
					case MapElement.Type_Coin:
						activeElement = CreateCoin (mapPos.x, mapPos.y, 0);
						break;
					case MapElement.Type_Brick:
						targetGO = (GameObject)GameObject.Instantiate (GoMapElementBrick);
						targetGO.transform.SetParent (GoGameMap.transform);
						targetGO.transform.localPosition = GetWorldPositionByMapPoint (mapPos.x , mapPos.y);
						targetGO.SetActive (true);

						_mapElements [mapPos.x, mapPos.y] = (MapElement)targetGO.GetComponent<MapElement> ();
						_mapElements [mapPos.x, mapPos.y].SetColor (MapElementMainColor);
						break;
					case MapElement.Type_BrickCoin:
						targetGO = (GameObject)GameObject.Instantiate (GoMapElementBrick);
						targetGO.transform.SetParent (GoGameMap.transform);
						targetGO.transform.localPosition = GetWorldPositionByMapPoint (mapPos.x , mapPos.y);
						targetGO.SetActive (true);

						_mapElements [mapPos.x, mapPos.y] = (MapElement)targetGO.GetComponent<MapElement> ();
						_mapElements [mapPos.x, mapPos.y].SetColor (MapElementMainColor);
						break;
					case MapElement.Type_WallSlope01:
						targetGO = (GameObject)GameObject.Instantiate (GoMapElementWallSlopeUp01);
						targetGO.transform.SetParent (GoGameMap.transform);
						targetGO.transform.localPosition = GetWorldPositionByMapPoint (mapPos.x , mapPos.y);
						targetGO.SetActive (true);

						_mapElements [mapPos.x, mapPos.y] = (MapElement)targetGO.GetComponent<MapElement> ();
						_mapElements [mapPos.x, mapPos.y].SetColor (MapElementMainColor);
						break;
					case MapElement.Type_WallSlope02:
						targetGO = (GameObject)GameObject.Instantiate (GoMapElementWallSlopeUp02);
						targetGO.transform.SetParent (GoGameMap.transform);
						targetGO.transform.localPosition = GetWorldPositionByMapPoint (mapPos.x , mapPos.y);
						targetGO.SetActive (true);

						_mapElements [mapPos.x, mapPos.y] = (MapElement)targetGO.GetComponent<MapElement> ();
						_mapElements [mapPos.x, mapPos.y].SetColor (MapElementMainColor);
						break;
					case MapElement.Type_WallSlope03:
						targetGO = (GameObject)GameObject.Instantiate (GoMapElementWallSlopeDown01);
						targetGO.transform.SetParent (GoGameMap.transform);
						targetGO.transform.localPosition = GetWorldPositionByMapPoint (mapPos.x , mapPos.y);
						targetGO.SetActive (true);

						_mapElements [mapPos.x, mapPos.y] = (MapElement)targetGO.GetComponent<MapElement> ();
						_mapElements [mapPos.x, mapPos.y].SetColor (MapElementMainColor);
						break;
					case MapElement.Type_WallSlope04:
						targetGO = (GameObject)GameObject.Instantiate (GoMapElementWallSlopeDown02);
						targetGO.transform.SetParent (GoGameMap.transform);
						targetGO.transform.localPosition = GetWorldPositionByMapPoint (mapPos.x , mapPos.y);
						targetGO.SetActive (true);

						_mapElements [mapPos.x, mapPos.y] = (MapElement)targetGO.GetComponent<MapElement> ();
						_mapElements [mapPos.x, mapPos.y].SetColor (MapElementMainColor);
						break;
					case MapElement.Type_WallSlope01Follow:
						targetGO = (GameObject)GameObject.Instantiate (GoMapElementWallSlopeUp01Follow);
						targetGO.transform.SetParent (GoGameMap.transform);
						targetGO.transform.localPosition = GetWorldPositionByMapPoint (mapPos.x , mapPos.y);
						targetGO.SetActive (true);

						_mapElements [mapPos.x, mapPos.y] = (MapElement)targetGO.GetComponent<MapElement> ();
						_mapElements [mapPos.x, mapPos.y].SetColor (MapElementMainColor);
						break;
					case MapElement.Type_WallSlope03Follow:
						targetGO = (GameObject)GameObject.Instantiate (GoMapElementWallSlopeDown01Follow);
						targetGO.transform.SetParent (GoGameMap.transform);
						targetGO.transform.localPosition = GetWorldPositionByMapPoint (mapPos.x , mapPos.y);
						targetGO.SetActive (true);

						_mapElements [mapPos.x, mapPos.y] = (MapElement)targetGO.GetComponent<MapElement> ();
						_mapElements [mapPos.x, mapPos.y].SetColor (MapElementMainColor);
						break;

					case MapElement.Type_WallSlopeBottom:
						targetGO = (GameObject)GameObject.Instantiate (GoMapElementWallSlopeBottom);
						targetGO.transform.SetParent (GoGameMap.transform);
						targetGO.transform.localPosition = GetWorldPositionByMapPoint (mapPos.x , mapPos.y);
						targetGO.SetActive (true);

						_mapElements [mapPos.x, mapPos.y] = (MapElement)targetGO.GetComponent<MapElement> ();
						_mapElements [mapPos.x, mapPos.y].SetColor (MapElementMainColor);
						break;

					
					case MapElement.Type_Spring:
						targetGO = (GameObject)GameObject.Instantiate (GoMapElementSpring);
						targetGO.transform.SetParent (GoGameMap.transform);
						targetGO.transform.localPosition = GetWorldPositionByMapPoint (mapPos.x , mapPos.y);
						targetGO.SetActive (true);

						_mapElements [mapPos.x, mapPos.y] = (MapElement)targetGO.GetComponent<MapElement> ();
						_mapElements [mapPos.x, mapPos.y].SetColor (MapElementMainColor);

						break;

					case MapElement.Type_SpringFollow:
						targetGO = (GameObject)GameObject.Instantiate (GoMapElementSpringFollow);
						targetGO.transform.SetParent (GoGameMap.transform);
						targetGO.transform.localPosition = GetWorldPositionByMapPoint (mapPos.x , mapPos.y);
						targetGO.SetActive (true);

						_mapElements [mapPos.x, mapPos.y] = (MapElement)targetGO.GetComponent<MapElement> ();
						_mapElements [mapPos.x, mapPos.y].SetColor (MapElementMainColor);

						break;

					}

					_mapElementSegment [mapPos.x, mapPos.y] = new MapElementSegmentData ();
					_mapElementSegment [mapPos.x, mapPos.y].segment = segment;
					_mapElementSegment [mapPos.x, mapPos.y].x = x + segment.segmentStartPos.x;
					_mapElementSegment [mapPos.x, mapPos.y].y = y + segment.segmentStartPos.y;

					if (_mapElements [mapPos.x, mapPos.y] != null) {
						_mapElements [mapPos.x, mapPos.y].x = _mapElementSegment [mapPos.x, mapPos.y].x;
						_mapElements [mapPos.x, mapPos.y].y = _mapElementSegment [mapPos.x, mapPos.y].y;
					}

					if (activeElement != null) {
						activeElement.x = _mapElementSegment [mapPos.x, mapPos.y].x;
						activeElement.y = _mapElementSegment [mapPos.x, mapPos.y].y;
					}

					_mapElementSegment [mapPos.x, mapPos.y].x = x + segment.segmentStartPos.x;
					_mapElementSegment [mapPos.x, mapPos.y].y = y + segment.segmentStartPos.y;
				}
			}

			for (int n = 0; n < segment.elements.Count; n++) {
				LevelElement element = segment.elements [n];

				switch (element.type) {
				case MapElement.Type_Pillar:
					LevelElementPillar pillarElement = (LevelElementPillar)element;
					int pillarPosX = segment.segmentStartPos.x + pillarElement.x;
					int pillarPosY = segment.segmentStartPos.y + pillarElement.y;
					if (pillarElement.dir == MapElement.Dir_Up) {
						targetGO = (GameObject)GameObject.Instantiate (GoMapElementPillarTop);
						targetGO.transform.SetParent (GoGameMap.transform);
						targetGO.transform.localPosition = GetWorldPositionByMapPoint (pillarPosX, pillarPosY);
						targetGO.SetActive (true);

						_mapElements [pillarPosX, pillarPosY] = (MapElement)targetGO.GetComponent<MapElement> ();
						_mapElements [pillarPosX, pillarPosY].SetColor (MapElementMainColor);

						targetGO = (GameObject)GameObject.Instantiate (GoMapElementPlaceHolder);
						targetGO.transform.SetParent (GoGameMap.transform);
						targetGO.transform.localPosition = GetWorldPositionByMapPoint (pillarPosX + 1, pillarPosY);
						targetGO.SetActive (true);

						_mapElements [pillarPosX + 1, pillarPosY] = (MapElement)targetGO.GetComponent<MapElement> ();

						for (int i = 1; i < pillarElement.length; i++) {
							targetGO = (GameObject)GameObject.Instantiate (GoMapElementPillarBar);
							targetGO.transform.SetParent (GoGameMap.transform);
							targetGO.transform.localPosition = GetWorldPositionByMapPoint (pillarPosX, pillarPosY + i);
							targetGO.SetActive (true);

							_mapElements [pillarPosX, pillarPosY + i] = (MapElement)targetGO.GetComponent<MapElement> ();
							_mapElements [pillarPosX, pillarPosY + i].SetColor (MapElementMainColor);

							targetGO = (GameObject)GameObject.Instantiate (GoMapElementPlaceHolder);
							targetGO.transform.SetParent (GoGameMap.transform);
							targetGO.transform.localPosition = GetWorldPositionByMapPoint (pillarPosX + 1, pillarPosY + i);
							targetGO.SetActive (true);

							_mapElements [pillarPosX + 1, pillarPosY + i] = (MapElement)targetGO.GetComponent<MapElement> ();

						}
					} else {
						targetGO = (GameObject)GameObject.Instantiate (GoMapElementPillarTop);
						targetGO.transform.SetParent (GoGameMap.transform);
						Vector3 tempPos = GetWorldPositionByMapPoint (pillarPosX, pillarPosY);
						tempPos.x += 1;
						targetGO.transform.localPosition = tempPos;
						targetGO.transform.localEulerAngles = new Vector3 (0, 0, 180);
						targetGO.SetActive (true);

						_mapElements [pillarPosX, pillarPosY] = (MapElement)targetGO.GetComponent<MapElement> ();
						_mapElements [pillarPosX, pillarPosY].SetColor (MapElementMainColor);

						targetGO = (GameObject)GameObject.Instantiate (GoMapElementPlaceHolder);
						targetGO.transform.SetParent (GoGameMap.transform);
						targetGO.transform.localPosition = GetWorldPositionByMapPoint (pillarPosX+1, pillarPosY);
						targetGO.SetActive (true);

						_mapElements [pillarPosX+1, pillarPosY] = (MapElement)targetGO.GetComponent<MapElement> ();

						for (int i = 1; i < pillarElement.length; i++) {
							targetGO = (GameObject)GameObject.Instantiate (GoMapElementPillarBar);
							targetGO.transform.SetParent (GoGameMap.transform);
							targetGO.transform.localPosition = GetWorldPositionByMapPoint (pillarPosX, pillarPosY-i);
							targetGO.SetActive (true);

							_mapElements [pillarPosX, pillarPosY-i] = (MapElement)targetGO.GetComponent<MapElement> ();
							_mapElements [pillarPosX, pillarPosY-i].SetColor (MapElementMainColor);

							targetGO = (GameObject)GameObject.Instantiate (GoMapElementPlaceHolder);
							targetGO.transform.SetParent (GoGameMap.transform);
							targetGO.transform.localPosition = GetWorldPositionByMapPoint (pillarPosX+1, pillarPosY-i);
							targetGO.SetActive (true);

							_mapElements [pillarPosX+1, pillarPosY-i] = (MapElement)targetGO.GetComponent<MapElement> ();

						}
					}

					break;

				case MapElement.Type_CoinArrow:
					LevelElementCoinArrow coinArrowElement = (LevelElementCoinArrow)element;
					targetGO = (GameObject)GameObject.Instantiate (GoMapElementCoinArrow);
					targetGO.transform.SetParent (GoGameMap.transform);
					targetGO.transform.localPosition = GetWorldPositionByMapPoint (coinArrowElement.x + segment.segmentStartPos.x, coinArrowElement.y + segment.segmentStartPos.y);
					targetGO.SetActive (true);

					CoinArrowElement coinArrow = (CoinArrowElement)targetGO.GetComponent<CoinArrowElement> ();
					coinArrow.SetSubtype (coinArrowElement.dir);

					coinArrow.x = coinArrowElement.x + segment.segmentStartPos.x;
					coinArrow.y = coinArrowElement.y + segment.segmentStartPos.y;

					_activeElements.Add (coinArrow);
					break;

				case MapElement.Type_Stop:
					LevelElementStop stopElement = (LevelElementStop)element;
					targetGO = (GameObject)GameObject.Instantiate (GoMapElementStop);
					targetGO.transform.SetParent (GoGameMap.transform);
					targetGO.transform.localPosition = GetWorldPositionByMapPoint (stopElement.x+ segment.segmentStartPos.x, stopElement.y+ segment.segmentStartPos.y);
					targetGO.SetActive (true);

					MapElementStop stop = (MapElementStop)targetGO.GetComponent<MapElementStop> ();
					stop.SetSubtype (stopElement.dir);

					stop.x = stopElement.x + segment.segmentStartPos.x;
					stop.y = stopElement.y + segment.segmentStartPos.y;

					_mapElements [stop.x, stop.y] = stop;

					break;

				case MapElement.Type_EnemyMushroom:
					LevelElementEnemyMushroon enemyMushroom = (LevelElementEnemyMushroon)element;
					targetGO = (GameObject)GameObject.Instantiate (GoMapElementEnemyMushroom);
					targetGO.transform.SetParent (GoGameMap.transform);
					targetGO.transform.localPosition = GetWorldPositionByMapPoint (enemyMushroom.x + segment.segmentStartPos.x, enemyMushroom.y + segment.segmentStartPos.y);
					targetGO.SetActive (true);

					EnemyMushroom mushroom = (EnemyMushroom)targetGO.GetComponent<EnemyMushroom> ();
					mushroom.SetColor (MapElementMainColor);
					mushroom.triggerDir = enemyMushroom.triggerDir;

					mushroom.x = enemyMushroom.x + segment.segmentStartPos.x;
					mushroom.y = enemyMushroom.y + segment.segmentStartPos.y;

					mushroom.StartPos = targetGO.transform.localPosition;
					_activeElements.Add (mushroom);

					break;

				case MapElement.Type_Question:
					LevelElementQuestion levelQuestion = (LevelElementQuestion)element;
					targetGO = (GameObject)GameObject.Instantiate (GoMapElementQuestion);
					targetGO.transform.SetParent (GoGameMap.transform);
					targetGO.transform.localPosition = GetWorldPositionByMapPoint (levelQuestion.x + segment.segmentStartPos.x, levelQuestion.y + segment.segmentStartPos.y);
					targetGO.SetActive (true);

					MapElementQuestionBlock question = (MapElementQuestionBlock)targetGO.GetComponent<MapElementQuestionBlock> ();
					question.SetColor (MapElementMainColor);
					question.SetData (levelQuestion.elementType, levelQuestion.spriteType);

					question.x = levelQuestion.x + segment.segmentStartPos.x;
					question.y = levelQuestion.y + segment.segmentStartPos.y;

					_mapElements [question.x, question.y] = question;
					break;

				case MapElement.Type_Piranha:
					LevelElementEnemyPiranha levelPiranha = (LevelElementEnemyPiranha)element;
					targetGO = (GameObject)GameObject.Instantiate (GoMapElementPiranha);
					targetGO.transform.SetParent (GoGameMap.transform);
					targetGO.transform.localPosition = GetWorldPositionByMapPoint (levelPiranha.x + segment.segmentStartPos.x, levelPiranha.y + segment.segmentStartPos.y);
					targetGO.SetActive (true);

					EnemyPiranha piranha = (EnemyPiranha)targetGO.GetComponent<EnemyPiranha> ();
					piranha.SetColor (MapElementMainColor);
					piranha.SetData (levelPiranha.delay, levelPiranha.hideDuration, levelPiranha.showDuration, levelPiranha.dir);

					piranha.x = levelPiranha.x + segment.segmentStartPos.x;
					piranha.y = levelPiranha.y + segment.segmentStartPos.y;

					_activeElements.Add (piranha);

					for (int i = 0; i < 2; i++) {
						for (int j = 0; j < 2; j++) {
							targetGO = (GameObject)GameObject.Instantiate (GoMapElementPiranhaFollow);
							targetGO.transform.SetParent (GoGameMap.transform);
							targetGO.transform.localPosition = GetWorldPositionByMapPoint (piranha.x + i, piranha.y + j);
							targetGO.SetActive (true);

							_mapElements [piranha.x + i, piranha.y + j] = (MapElement)targetGO.GetComponent<MapElement> ();
						}
					}
					break;
				}
			}

			if (m == levelDef.segments.Count - 1) {
				int endX = segment.segmentStartPos.x + segment.endPoint.x;
				int endY = segment.segmentStartPos.y + segment.endPoint.y;

				targetGO = (GameObject)GameObject.Instantiate (GoMapElementEnd);
				targetGO.transform.SetParent (GoGameMap.transform);
				targetGO.transform.localPosition = GetWorldPositionByMapPoint (endX , endY);
				targetGO.SetActive (true);

				_mapElements [endX , endY] = (MapElement)targetGO.GetComponent<MapElement> ();
				_mapElements [endX , endY].SetColor (MapElementMainColor);

				for (int n = endY - 1; n >= 0; n--) {
					if (_mapElements [endX, n] != null) {
						break;
					}
					 
					targetGO = (GameObject)GameObject.Instantiate (GoMapElementEndFollow);
					targetGO.transform.SetParent (GoGameMap.transform);
					targetGO.transform.localPosition = GetWorldPositionByMapPoint (endX, n);
					targetGO.SetActive (true);

					_mapElements [endX, n] = (MapElement)targetGO.GetComponent<MapElement> ();
				}
				break;
			}
		}



		_actorMapPosX = levelDef.segments [0].segmentStartPos.x + levelDef.segments [0].startPoint.x;
		_actorMapPosY = levelDef.segments [0].segmentStartPos.y + levelDef.segments [0].startPoint.y;

		Vector3 actorPos = GetWorldPositionByMapPoint (_actorMapPosX, _actorMapPosY);
		actorPos.z = -1;

		Actor.transform.position = actorPos;

		SetCameraPosition ();
	}

	public Vector3 GetWorldPositionByMapPoint( int x, int y ) {
		return new Vector3 (x-7, 12-y, 0);
	}

	public MapPoint GetMapPointOfWorldPosition( Vector3 pos ) {
 		return new MapPoint ((int)(pos.x + 7.4999f), (int)(12.4999f - pos.y)); 
	}

	public Vector3 GetActorCurrentPosition() {
		return Actor.transform.position;
	}

	public MapElement GetMapElementByMapPoint( int x, int y ) {
		if((x>=0)&&(x<_levelWidth)&&(y>=0)&&(y<_levelHeight)) {
			return _mapElements [x, y];
		}
		return null;
	}


	public MapElement GetMapElementByWorldPosition( Vector2 pos ) {
		MapPoint mapPoint = GetMapPointOfWorldPosition (pos);

		return GetMapElementByMapPoint (mapPoint.x, mapPoint.y);
	}

	public void ShowScreenCover( Vector3 centerPos, bool withAnimation, FuncCallback callback ) {
		SptScreenCoverTop.gameObject.SetActive (true);
		SptScreenCoverLeft.gameObject.SetActive (true);
		SptScreenCoverCenter.gameObject.SetActive (true);
		SptScreenCoverRight.gameObject.SetActive (true);
		SptScreenCoverBottom.gameObject.SetActive (true);

		if (withAnimation == false) {
			SptScreenCoverCenter.transform.localScale = Vector3.zero;
			SptScreenCoverCenter.transform.position = centerPos;

			Vector3 pos = centerPos;
			pos.z = -40;
			pos.x -= 40;
			SptScreenCoverLeft.transform.position = pos;
			pos = centerPos;
			pos.z = -40;
			pos.x += 40;
			SptScreenCoverRight.transform.position = pos;
			pos = centerPos;
			pos.z = -40;
			pos.y -= 40;
			SptScreenCoverBottom.transform.position = pos;
			pos = centerPos;
			pos.z = -40;
			pos.y += 40;
			SptScreenCoverTop.transform.position = pos;
		} else {
			float maxScale = (15 + Mathf.Abs (centerPos.y)) * 1.5f;

			SptScreenCoverCenter.transform.localScale = Vector3.one*maxScale*2;
			SptScreenCoverCenter.transform.position = centerPos;

			Vector3 pos = centerPos;
			pos.x -= (40+maxScale);
			pos.z = -40;
			SptScreenCoverLeft.transform.position = pos;
			pos = centerPos;
			pos.x += 40+maxScale;
			pos.z = -40;
			SptScreenCoverRight.transform.position = pos;
			pos = centerPos;
			pos.y -= (40+maxScale);
			pos.z = -40;
			SptScreenCoverBottom.transform.position = pos;
			pos = centerPos;
			pos.y += (40+maxScale);
			pos.z = -40;
			SptScreenCoverTop.transform.position = pos;

			pos = centerPos;
			DOTween.Play (SptScreenCoverLeft.transform.DOLocalMoveX (pos.x - 40, ScreenCoverMoveDuration));
			DOTween.Play (SptScreenCoverRight.transform.DOLocalMoveX (pos.x + 40, ScreenCoverMoveDuration));
			DOTween.Play (SptScreenCoverTop.transform.DOLocalMoveY (pos.y + 40, ScreenCoverMoveDuration));
			DOTween.Play (SptScreenCoverBottom.transform.DOLocalMoveY (pos.y - 40, ScreenCoverMoveDuration));

			DOTween.Play (SptScreenCoverCenter.transform.DOScale (Vector3.zero, ScreenCoverMoveDuration).OnComplete (() => {
				if(callback!=null){
					callback();
				}
			}));
		}
	}

	public void HideScreenCover( Vector3 centerPos, FuncCallback callback ) {
		float maxScale = (15 + Mathf.Abs (centerPos.y)) * 1.5f;

		SptScreenCoverCenter.transform.localScale = Vector3.zero;
		SptScreenCoverCenter.transform.position = centerPos;

		Vector3 pos = centerPos;
		pos.z = -40;
		pos.x -= 40;
		SptScreenCoverLeft.transform.position = pos;
		pos = centerPos;
		pos.z = -40;
		pos.x += 40;
		SptScreenCoverRight.transform.position = pos;
		pos = centerPos;
		pos.z = -40;
		pos.y -= 40;
		SptScreenCoverBottom.transform.position = pos;
		pos = centerPos;
		pos.z = -40;
		pos.y += 40;
		SptScreenCoverTop.transform.position = pos;

	    pos = centerPos;
		DOTween.Play (SptScreenCoverLeft.transform.DOLocalMoveX (pos.x - 40-maxScale, ScreenCoverMoveDuration).SetEase(Ease.InCirc));
		DOTween.Play (SptScreenCoverRight.transform.DOLocalMoveX (pos.x + 40+maxScale, ScreenCoverMoveDuration).SetEase(Ease.InCirc));
		DOTween.Play (SptScreenCoverTop.transform.DOLocalMoveY (pos.y + 40+maxScale, ScreenCoverMoveDuration).SetEase(Ease.InCirc));
		DOTween.Play (SptScreenCoverBottom.transform.DOLocalMoveY (pos.y - 40-maxScale, ScreenCoverMoveDuration).SetEase(Ease.InCirc));

		DOTween.Play (SptScreenCoverCenter.transform.DOScale (Vector3.one*maxScale*2, ScreenCoverMoveDuration).SetEase(Ease.InCirc).OnComplete (() => {
			SptScreenCoverTop.gameObject.SetActive (false);
			SptScreenCoverLeft.gameObject.SetActive (false);
			SptScreenCoverCenter.gameObject.SetActive (false);
			SptScreenCoverRight.gameObject.SetActive (false);
			SptScreenCoverBottom.gameObject.SetActive (false);
			
			if(callback!=null){
				callback();
			}
		}));
	}

	public void SetCameraPosition() {
		Vector3 cameraPos = MainCamera.transform.position;

		Vector3 pos = Actor.transform.position;
		cameraPos.x=pos.x+3;

		if (pos.y - cameraPos.y > 5) {
			cameraPos.y = pos.y - 5;
		}
		if (cameraPos.y - pos.y > 5) {
			cameraPos.y = pos.y + 5;
		}

		//if (_mapElements [_actorMapPosX, _actorMapPosY] != null) {
		_currentSegment = _mapElementSegment [_actorMapPosX, _actorMapPosY].segment;

		if (_currentSegment.lockScreenLeft) {
			if (cameraPos.x < _currentSegment.leftTopPosition.x + _screenWidth / 2+1) {
				cameraPos.x = _currentSegment.leftTopPosition.x + _screenWidth / 2+1;
				}
			}
		if(_currentSegment.lockScreenRight){
			if (cameraPos.x > _currentSegment.rightBottomPosition.x - _screenWidth / 2) {
				cameraPos.x = _currentSegment.rightBottomPosition.x - _screenWidth / 2;
				}
			}
		if (_currentSegment.lockScreenTop) {
			if (cameraPos.y > _currentSegment.leftTopPosition.y - _screenHeight / 2) {
				cameraPos.y = _currentSegment.leftTopPosition.y - _screenHeight / 2;
				}
			}
		if(_currentSegment.lockScreenBottom){
			if (cameraPos.y < _currentSegment.rightBottomPosition.y + _screenHeight / 2 + 1) {
				cameraPos.y = _currentSegment.rightBottomPosition.y + _screenHeight / 2 + 1;
				}
			}
		//}

		MainCamera.transform.position = cameraPos;
	}

	public void StartGame() {
		for (int m = 0; m < _tempElements.Count; m++) {
			GameObject.Destroy (_tempElements [m].gameObject);
		}
		_tempElements.Clear ();

		_timeValue = 60;
		_lives = 3;
		_coins = 0;

		ResumeGame ();
	}

	public void ResumeGame() {

		string coinValue = "";
		if (_timeValue < 10) {
			coinValue = "00" +((int) _coins).ToString ();
		} else if (_timeValue < 100) {
			coinValue = "0" + ((int)_coins).ToString ();
		} else {
			coinValue = ((int)_coins).ToString ();
		}
		TxtPlayUICoinValue.text = coinValue;

		TxtPlayUILifeValue.text = "x   "+_lives.ToString ();

		DOTween.Play (RectPlayUITopBar.DOLocalMoveY (_screenPixelHeight / 2 - 64, 1.0f).SetEase (Ease.InOutQuad));

		Actor.StartGame ();

		_status = Status_Playing;
	}

	public void KeyPressed( bool flag ) {
		if (_status == Status_Playing) {
			Actor.KeyPressed (flag);
		}
	}

	public List<ActiveElement> GetActiveElementList() {
		return _activeElements;
	}

	public LevelSegment GetMapSegmentByPoint( MapPoint point ) {
		if ((point.x >= 0) && (point.x < _levelWidth) && (point.y >= 0) && (point.y < _levelHeight)) {
			return _mapElementSegment [point.x, point.y].segment;
		}

		return null;
	}

	public void AddActiveElement( ActiveElement element ) {
		_activeElements.Add (element);
	}

	public void RemoveActiveElement( ActiveElement element ) {
		_activeElements.Remove (element);
	}

	public ActiveElement CreateCoin( int x, int y, int subtype ) {
		GameObject targetGO = (GameObject)GameObject.Instantiate (GoMapElementCoin);
		targetGO.transform.SetParent (GoGameMap.transform);
		targetGO.transform.localPosition = GetWorldPositionByMapPoint (x, y);
		targetGO.SetActive (true);

		CoinElement element = (CoinElement)targetGO.GetComponent<CoinElement> ();
		element.SetSubtype (subtype);

		_activeElements.Add (element);

		element.x = x;
		element.y = y;

		return element;
	}

	public void GameOver(Vector3 centerPos, bool isWin ) {
		Debug.LogError ("Game Over!!!");



		DOTween.Play (RectPlayUITopBar.DOLocalMoveY (_screenPixelHeight / 2 + 64, 0.15f).SetEase (Ease.InOutQuad));

		ShowScreenCover (centerPos, true, DoGameOver );

		if (isWin) {
			_status = Status_GameOverWin;
		}
		else{
			ChangeLife (-1);
			_status = Status_GameOverDead;
		}
	}

	void DoGameOver() {
		if (_status == Status_GameOverDead) {
			if (_lives <= 0) {
				Debug.LogError ("Game over, no more life!");
			}
			else {
				// Restart
				for (int m = 0; m < _activeElements.Count; m++) {
					_activeElements [m].Reset ();
				}

				Vector3 pos = GetWorldPositionByMapPoint(_currentSegment.startPoint.x+_currentSegment.segmentStartPos.x, _currentSegment.startPoint.y+_currentSegment.segmentStartPos.y);
				pos.z = -25;
				Actor.transform.localScale = Vector3.one;
				Actor.transform.localEulerAngles = Vector3.zero;
				Actor.transform.position = pos;
				HideScreenCover( pos, ResumeGame );
			}
		}
	}

	public bool IsDeadMapPoint( MapPoint point ) {
		bool isDead = false;

		MapPoint cameraPoint = GetMapPointOfWorldPosition (MainCamera.transform.position);


		if ((point.y > cameraPoint.y + _screenHeight*2/3)||(point.y< cameraPoint.y-_screenHeight*2/3) ||(point.x<cameraPoint.x-_screenWidth)||(point.x>cameraPoint.x+_screenWidth)){
			isDead = true;
		}

		return isDead;
	}

	public void OnButtonPause(){
	}

	public void OnButtonRestart() {
	}

	public void OnButtonResume() {
	}

	public void OnButtonExit() {
	}

	public void OnButtonSelectLevel( int level ) {

		CreateLevel (_levels.GetLevelDef (0));

		Vector3 actorPos = GetActorCurrentPosition ();
		actorPos.z = -25;

		Color color = ImgTopCover.color;
		color.a = 0;
		ImgTopCover.color = color;
		ImgTopCover.gameObject.SetActive (true);
		DOTween.Play (ImgTopCover.DOColor (color, 0.2f).OnComplete (() => {
			for(int m=0;m<RectSelectUIWorlds.Length;m++){
				RectSelectUIWorlds[m].localPosition = new Vector3( 0, _screenPixelHeight/2+200, 0 );
			}
			ImgSelectUIBackground.gameObject.SetActive( false );
				
			ShowScreenCover (actorPos, false, null);
			HideScreenCover( actorPos, StartGame );
			Actor.gameObject.SetActive( true );
		}));

	}
}
