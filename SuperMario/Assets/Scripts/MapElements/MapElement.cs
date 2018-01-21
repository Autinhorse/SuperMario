using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPoint {
	public int x;
	public int y;

	public MapPoint( int vX, int vY ){
		x = vX;
		y = vY;
	}

	public override string ToString ()
	{
		string result = "MapPoint(" + x.ToString () + "," + y.ToString () + ")";
		return result;
	}
}

public class MapElement : MonoBehaviour {
	// 用于表示主角从某个方向进入这个方块是不是会被阻挡
	public const int Block_None = 0;	// 不会被阻挡
	public const int Block_Hard = 1;	// 会被硬阻挡
	public const int Block_Soft = 2;	// 会被软阻挡（有弹性）

	public const int Dir_None = 0;
	public const int Dir_Left = 1;
	public const int Dir_LeftUp = 2;
	public const int Dir_Up = 3;
	public const int Dir_RightUp = 4;
	public const int Dir_Right = 5;
	public const int Dir_RightDown = 6;
	public const int Dir_Down = 7;
	public const int Dir_LeftDown = 8;


	public const int Type_None = 0;
	public const int Type_WallFixed = 1;				// 固定的方块
	public const int Type_WallSlope01 = 2;				// 1:2向上的斜坡
	public const int Type_WallSlope02 = 3;				// 1:1（45度）向上的斜坡
	public const int Type_WallSlope03 = 4;				// 1:2向下的斜坡			
	public const int Type_WallSlope04 = 5;				// 1:1（45度）向下的斜坡
	public const int Type_Brick = 6;					// 这个是普通什么都没有的砖块
	public const int Type_Question = 7;     			// 这个是普通的只有一个金币的问号方块
	public const int Type_BrickCoin = 8;				// 这个是藏了一个金币的砖块
	public const int Type_Coin = 9;						// 这个是一个金币
	public const int Type_Cloud = 10;					// a 这个是一个云朵
	public const int Type_EnemyMushroom = 11;  	 		// b 这个是一个蘑菇敌人
	public const int Type_End = 12;   					// c 这个是结束
	public const int Type_WallSlope01Follow = 13; 		// d 这个是结束
	public const int Type_WallSlope03Follow = 14; 		// e 这个是结束


	public const int Type_CoinArrow = 10001;	// 这个是一个金币产生器，沿指定方向产生四个金币
	public const int Type_Pillar = 10002;		// 这个是一个柱子
	public const int Type_Stop = 10003;			// 这个是一个停止位置
	public const int Type_Life = 10004;			// 这个是藏在问题方块里面的，加一条命
	public const int Type_Mushroom = 10005;		// 这个是藏在问题方块里面的，加一条命

	public SpriteRenderer SptMain;

	[HideInInspector]
	public int type;
	[HideInInspector]
	public int x;
	[HideInInspector]
	public int y;

	[HideInInspector]
	public LevelSegment masterSegment;

	void Awake() {
		type = Type_None;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Create( LevelSegment segemnt, LevelElement element ) {
		
	}

	public void SetColor( Color color ) {
		if (SptMain != null) {
			SptMain.color = color;
		}
	}

	public override string ToString ()
	{
		string result = "MapElement(" + x.ToString () + "," + y.ToString () + ")--Type:"+type.ToString();
		return result;
	}

	public virtual int IsBlock( int dir ) {
		return Block_None;
	}

	public virtual void HitBottom() {
	}
}
