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
	public const int Type_WallBoard = 7;     			// 这个是可以从下方跳上去的平板
	public const int Type_BrickCoin = 8;				// 这个是藏了一个金币的砖块
	public const int Type_Coin = 9;						// 这个是一个金币
	public const int Type_Cloud = 10;					// A 这个是一个云朵
	//public const int Type_EnemyMushroom = 11;  	 	// B 这个是一个蘑菇敌人
	public const int Type_End = 12;   					// C 这个是结束
	public const int Type_WallSlope01Follow = 13; 		// D 这个是1:2上坡后面一个
	public const int Type_WallSlope03Follow = 14; 		// E 这个是1:2下坡后面一个
	public const int Type_EndFollow = 15; 				// F 这个是结束位置上面一个
	public const int Type_WallSlopeBottom = 16;			// G 斜坡下面
	public const int Type_Spring = 17;					// H 弹簧
	public const int Type_SpringFollow = 18;			// I 弹簧其余位置


	public const int Type_CoinArrow = 10001;			// 这个是一个金币产生器，沿指定方向产生四个金币
	public const int Type_Pillar = 10002;				// 这个是一个柱子
	public const int Type_Stop = 10003;					// 这个是一个停止位置
	//public const int Type_AwardLife = 10004;			// 这个是藏在问题方块里面的，加一条命
	public const int Type_EnemyMushroom = 10005;		// 敌人，普通蘑菇
	public const int Type_Question = 10006;				// 问题方块
	public const int Type_CoinEject = 10007;			// 撞了方块以后弹出来的金币
	public const int Type_Mushroom = 10008;				// 撞了出现的蘑菇，吃了可以变大
	public const int Type_Piranha = 10009;				// 食人花
	public const int Type_BounceAward = 10010;			// 撞了方块以后出现的星星，吃了一段时间内可以吸引附近的金币或无敌
	public const int Type_PiranhaFollow = 10011;		// 食人花的占位元素

	public SpriteRenderer SptMain;
	public SpriteRenderer SptBorder;

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

	public void SetColor( Color mainColor ) {
		if (SptMain != null) {
			SptMain.color = mainColor;
		}
	}

	public void SetColor( Color mainColor, Color borderColor ) {
		if (SptMain != null) {
			SptMain.color = mainColor;
		}

		if (SptBorder != null) {
			SptBorder.color = borderColor;
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

	public virtual void HitBottom(bool hitForward) {
	}
}
