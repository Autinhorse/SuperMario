using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using DG.Tweening;

class MapElementSegmentData {
	public LevelSegment segment;
	public int x;
	public int y;
}

public class GameMap : MonoBehaviour {

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
	public GameObject GoMapElementEnemyMushroom;
	public GameObject GoMapElementMushroom;
	public GameObject GoMapElementPlaceHolder;
	public GameObject GoMapElementPillarTop;
	public GameObject GoMapElementPillarBar;


	[Header("----------Colors----------")]
	public Color MapElementMainColor;

	[Header("----------Duration----------")]
	public float ScreenCoverMoveDuration;

	MapElement[,] _mapElements;
	MapElementSegmentData[,] _mapElementSegment;
	int _levelWidth;
	int _levelHeight;

	int _screenWidth;
	int _screenHeight;

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
	}

	// Use this for initialization
	void Start () {
		
				

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void CreateLevel( LevelDef levelDef ) {

		if (levelDef == null) {
			Debug.LogError ("LevelDef data is null, cannot create level!");
			return;
		}

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

			for (int x = 0; x < segment.width; x++) {
				for (int y = 0; y < segment.height; y++) {
					mapPos.x = segment.segmentStartPos.x + x;
					mapPos.y = segment.segmentStartPos.y + y;

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
					case MapElement.Type_Cloud:
						targetGO = (GameObject)GameObject.Instantiate (GoMapElementCloud);
						targetGO.transform.SetParent (GoGameMap.transform);
						targetGO.transform.localPosition = GetWorldPositionByMapPoint (mapPos.x , mapPos.y);
						targetGO.SetActive (true);

						_mapElements [mapPos.x, mapPos.y] = (MapElement)targetGO.GetComponent<MapElement> ();
						_mapElements [mapPos.x, mapPos.y].SetColor (MapElementMainColor);
						break;
					case MapElement.Type_Coin:
						targetGO = (GameObject)GameObject.Instantiate (GoMapElementCoin);
						targetGO.transform.SetParent (GoGameMap.transform);
						targetGO.transform.localPosition = GetWorldPositionByMapPoint (mapPos.x , mapPos.y);
						targetGO.SetActive (true);

						_mapElements [mapPos.x, mapPos.y] = (MapElement)targetGO.GetComponent<MapElement> ();
						_mapElements [mapPos.x, mapPos.y].SetColor (MapElementMainColor);
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

						/*
					case MapElement.Type_Question:
						targetGO = (GameObject)GameObject.Instantiate (GoMapElementBrick);
						targetGO.transform.SetParent (GoGameMap.transform);
						targetGO.transform.localPosition = GetWorldPositionByMapPoint (mapPos.x , mapPos.y);
						targetGO.SetActive (true);

						_mapElements [mapPos.x, mapPos.y] = (MapElement)targetGO.GetComponent<MapElement> ();
						_mapElements [mapPos.x, mapPos.y].SetColor (MapElementMainColor);
						break;*/
					}

					_mapElementSegment [mapPos.x, mapPos.y] = new MapElementSegmentData ();
					_mapElementSegment [mapPos.x, mapPos.y].segment = segment;
					_mapElementSegment [mapPos.x, mapPos.y].x = x + segment.segmentStartPos.x;
					_mapElementSegment [mapPos.x, mapPos.y].y = y + segment.segmentStartPos.y;

					if (_mapElements [mapPos.x, mapPos.y] != null) {
						_mapElements [mapPos.x, mapPos.y].x = _mapElementSegment [mapPos.x, mapPos.y].x;
						_mapElements [mapPos.x, mapPos.y].y = _mapElementSegment [mapPos.x, mapPos.y].y;
					}
				}

				;
				for( int n=0; n<segment.elements.Count; n++ ) {
					LevelElement element = segment.elements [n];

					switch (element.type) {
					case MapElement.Type_Pillar:
						LevelElementPillar pillarElement = (LevelElementPillar)element;
						if (pillarElement.dir == MapElement.Dir_Up) {
							targetGO = (GameObject)GameObject.Instantiate (GoMapElementPillarTop);
							targetGO.transform.SetParent (GoGameMap.transform);
							targetGO.transform.localPosition = GetWorldPositionByMapPoint (pillarElement.x, pillarElement.y);
							targetGO.SetActive (true);

							_mapElements [pillarElement.x, pillarElement.y] = (MapElement)targetGO.GetComponent<MapElement> ();
							_mapElements [pillarElement.x, pillarElement.y].SetColor (MapElementMainColor);

							targetGO = (GameObject)GameObject.Instantiate (GoMapElementPlaceHolder);
							targetGO.transform.SetParent (GoGameMap.transform);
							targetGO.transform.localPosition = GetWorldPositionByMapPoint (pillarElement.x+1, pillarElement.y);
							targetGO.SetActive (true);

							_mapElements [pillarElement.x+1, pillarElement.y] = (MapElement)targetGO.GetComponent<MapElement> ();

							for (int i = 1; i < pillarElement.length; i++) {
								targetGO = (GameObject)GameObject.Instantiate (GoMapElementPillarBar);
								targetGO.transform.SetParent (GoGameMap.transform);
								targetGO.transform.localPosition = GetWorldPositionByMapPoint (pillarElement.x, pillarElement.y+i);
								targetGO.SetActive (true);

								_mapElements [pillarElement.x, pillarElement.y+i] = (MapElement)targetGO.GetComponent<MapElement> ();
								_mapElements [pillarElement.x, pillarElement.y+i].SetColor (MapElementMainColor);

								targetGO = (GameObject)GameObject.Instantiate (GoMapElementPlaceHolder);
								targetGO.transform.SetParent (GoGameMap.transform);
								targetGO.transform.localPosition = GetWorldPositionByMapPoint (pillarElement.x+1, pillarElement.y+i);
								targetGO.SetActive (true);

								_mapElements [pillarElement.x+1, pillarElement.y+i] = (MapElement)targetGO.GetComponent<MapElement> ();
									
							}
						} else {
						}

						break;
					}
				}
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
 		return new MapPoint ((int)(pos.x + 7.49f), (int)(12.49f - pos.y)); 
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
			pos.x -= 40;
			SptScreenCoverLeft.transform.position = pos;
			pos = centerPos;
			pos.x += 40;
			SptScreenCoverRight.transform.position = pos;
			pos = centerPos;
			pos.y -= 40;
			SptScreenCoverBottom.transform.position = pos;
			pos = centerPos;
			pos.y += 40;
			SptScreenCoverTop.transform.position = pos;
		} else {
			float maxScale = (15 + Mathf.Abs (centerPos.y)) * 1.5f;

			SptScreenCoverCenter.transform.localScale = Vector3.one*maxScale*2;
			SptScreenCoverCenter.transform.position = centerPos;

			Vector3 pos = centerPos;
			pos.x -= (40+maxScale);
			SptScreenCoverLeft.transform.position = pos;
			pos = centerPos;
			pos.x += 40+maxScale;
			SptScreenCoverRight.transform.position = pos;
			pos = centerPos;
			pos.y -= (40+maxScale);
			SptScreenCoverBottom.transform.position = pos;
			pos = centerPos;
			pos.y += (40+maxScale);
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

		Vector3 pos = centerPos;
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
		LevelSegment segment = _mapElementSegment [_actorMapPosX, _actorMapPosY].segment;

			if (segment.lockScreenLeft) {
				if (cameraPos.x < segment.leftTopPosition.x + _screenWidth / 2+1) {
					cameraPos.x = segment.leftTopPosition.x + _screenWidth / 2+1;
				}
			}
			if(segment.lockScreenRight){
				if (cameraPos.x > segment.rightBottomPosition.x - _screenWidth / 2) {
					cameraPos.x = segment.rightBottomPosition.x - _screenWidth / 2;
				}
			}
			if (segment.lockScreenTop) {
				if (cameraPos.y > segment.leftTopPosition.y - _screenHeight / 2) {
					cameraPos.y = segment.leftTopPosition.y - _screenHeight / 2;
				}
			}
			if(segment.lockScreenBottom){
				if (cameraPos.y < segment.rightBottomPosition.y + _screenHeight / 2 + 1) {
					cameraPos.y = segment.rightBottomPosition.y + _screenHeight / 2 + 1;
				}
			}
		//}

		MainCamera.transform.position = cameraPos;
	}

	public void StartGame() {
		Actor.StartGame ();
	}

	public void KeyPressed( bool flag ) {
		Actor.KeyPressed (flag);
	}

}
