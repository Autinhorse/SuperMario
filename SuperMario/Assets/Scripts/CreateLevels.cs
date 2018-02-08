using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateLevels  {

	List<LevelDef> _localLevels;

	public LevelDef GetLevelDef( int index ) {
		return _localLevels[0];
	}

	public CreateLevels() {
		_localLevels = new List<LevelDef> ();

		// ************* 创建第一关 **************************************************************
		LevelDef levelDef = new LevelDef ();
		LevelSegment segment;
		LevelElementQuestion elementQuestion;
		LevelElementPillar elementPillar;
		LevelElementCoinArrow elementCoinArrow;
		LevelElementStop elementStop;
		LevelElementEnemyMushroon enemyMushroom;


		// ************* 第一段 ********************************
		/*
		LevelSegment segment = new LevelSegment();

		string[] segametData01 = {
			// 0
			//0        10	    20        30        40        50		60
			"0000000000000000000000000000000000000000000000000000000000000",
			"0000000000000000000000000000000000000000000000000000000000000",
			"0000000000000000000000000000000000000000000000000000000000000",
			"0000000000000000000000000000000000000000000000000000000000000",
			"0000000000000000000000000000000000000000000000000000000000000",
		   //0        10	    20        30        40        50		60
			// 5
		   //0        10	    20        30        40        50		60
			"0000000000000000000000000000000000000000000000000000000000000",
			"0000000000000000000000000000000000000000000000000000000000000",
			"0000000000000000000000000000000000000000000000000000000000000",
			"0100000000000000000000000000000000000000000000000000000000000",
			"0100000000000000000000000000000000000000000000000000000000000",
		   //0        10	    20        30        40        50		60
			// 10
		   //0        10	    20        30        40        50		60
			"0100000000000000000000000000000000000000000000000000000000001",
			"0100000000000000000000000000000000000000000000000000000000001",
			"0100000000000000000000000000000000000000000000000000000000001",
			"0100000000000000000000000000000000000000000000000000000000001",
			"0100000000000000000000000000000000000000000000000000000000001",
		   //0        10	    20        30        40        50		60
			// 15
		   //0        10	    20        30        40        50		60
			"0100000000000000000000000000000000000000000000000000000000001",
			"0100000000000000000000000000000000000000000000000000000000001",
			"0100000000000000000000000001111111000000000000000000000000001",
			"0100000000000000000000000001000000000000000000000000000000001",
			"0100000000000000000000000001099999990000000000000000000000001",
		   //0        10	    20        30        40        50		60
			// 20
		   //0        10	    20        30        40        50		60
			"0100000000000000000000000001000000000000000000000000000000001",
			"0100000000000000000000000001000000000000000000000000000000001",
			"0100000000000000000000000001009000000000000000000000000000001",
			"1111111111111111111111111HI1111111111111111111111111111111111",
			"1111111111111111111111111001111111111111111111111111111111111",
		   //0        10	    20        30        40        50		60
			// 25
		   //0        10	    20        30        40        50		60
			"1111111111111111111111GGGG111111110001111111111111G1111111111",
			"1111111111111111111111111111111111000111111111111111110000111"
		};
		segment.data = segametData01;

		segment.width = segment.data[0].Length;
		segment.height = segment.data.Length;

		segment.segmentStartPos = new MapPoint (0, 0);
		segment.startPoint = new MapPoint (4, 22);
		segment.endPoint = new MapPoint (60, 22);

		segment.lockScreenLeft = true;
		segment.lockScreenRight = false;
		segment.lockScreenTop = true;
		segment.lockScreenBottom = true;
		segment.CalculateBorder ();

		LevelElementQuestion elementQuestion = new LevelElementQuestion ( 10, 17, LevelElementQuestion.Type_AwardStar, LevelElementQuestion.Sprite_Question );
		segment.elements.Add (elementQuestion);

		elementQuestion = new LevelElementQuestion ( 18, 17, LevelElementQuestion.Type_AwardLife, LevelElementQuestion.Sprite_Question );
		segment.elements.Add (elementQuestion);

		LevelElementEnemyPiranha elementPiranha = new LevelElementEnemyPiranha (20, 21, MapElement.Dir_Up, 0.5f, 3.0f, 4.0f);
		segment.elements.Add (elementPiranha);

		elementPiranha = new LevelElementEnemyPiranha (16, 17, MapElement.Dir_Right, 1.0f, 3.0f, 4.0f);
		segment.elements.Add (elementPiranha);

		elementPiranha = new LevelElementEnemyPiranha (24, 17, MapElement.Dir_Left, 1.5f, 4.0f, 3.0f);
		segment.elements.Add (elementPiranha);

		elementPiranha = new LevelElementEnemyPiranha (20, 13, MapElement.Dir_Down, 2.0f, 4.0f, 3.0f);
		segment.elements.Add (elementPiranha);




		LevelElementCoinArrow elementCoinArrow = new LevelElementCoinArrow ( 24, 21, MapElement.Dir_Right );
		segment.elements.Add (elementCoinArrow);

		LevelElementPillar elementPillar = new LevelElementPillar ( 54, 19, MapElement.Dir_Up, 8 );
		//segment.elements.Add (elementPillar);


		LevelElementEnemyMushroon enemyMushroom = new LevelElementEnemyMushroon (23, 20, MapElement.Dir_Left, MapElement.Dir_Left,0);
		segment.elements.Add (enemyMushroom);

		enemyMushroom = new LevelElementEnemyMushroon (25, 20, MapElement.Dir_Left, MapElement.Dir_Left,0);
		segment.elements.Add (enemyMushroom);

		enemyMushroom = new LevelElementEnemyMushroon (27, 20, MapElement.Dir_Left, MapElement.Dir_Left,0);
		segment.elements.Add (enemyMushroom);

		enemyMushroom = new LevelElementEnemyMushroon (29, 20, MapElement.Dir_Left, MapElement.Dir_Left,0);
		segment.elements.Add (enemyMushroom);

		enemyMushroom = new LevelElementEnemyMushroon (31, 20, MapElement.Dir_Left, MapElement.Dir_Left,0);
		segment.elements.Add (enemyMushroom);

		enemyMushroom = new LevelElementEnemyMushroon (33, 20, MapElement.Dir_Left, MapElement.Dir_Left,0);
		segment.elements.Add (enemyMushroom);

		enemyMushroom = new LevelElementEnemyMushroon (35, 20, MapElement.Dir_Left, MapElement.Dir_Left,0);
		segment.elements.Add (enemyMushroom);

		levelDef.segments.Add (segment);
*/
		// ************* 第一段 ********************************
	    segment = new LevelSegment();
		string[] segametData01 = {
		   // 0
		   //0        10	    20        30        40        50		60
			"00000000000000000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000000000000000000000000000000000",
		   //0        10	    20        30        40        50		60
    	   // 5
		   //0        10	    20        30        40        50		60
			"00000000000000000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000900000000000000000000000000000",
		   //0        10	    20        30        40        50		60
		   // 10
		   //0        10	    20        30        40        50		60
			"00000000000000000000000000000000000090000000000000000000000000000",
			"00000000000000000000000000000000000009000000000000000000000000000",
			"00000000000000000000000000000000000000010000000000000000000000000",
			"00000000000000000000000000000000000000010000000000000000000000000",
			"00000000000000000000000000000000000000010000000000000000000000000",

		   //0        10	    20        30        40        50		60
		   // 15
		   //0        10	    20        30        40        50		60
			"00000000000000000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000009000000000000000000000000000",
			"00000000000000000000000000000000000090000000000000000000000000000",
			"00000000000000000000000000000000000090000000000000000000000000000",
			"00000000000000000000000000000000000900000000000000000000000000000",
		   //0        10	    20        30        40        50		60
		   // 20
		   //0        10	    20        30        40        50		60
			//"0000000666676666600000000000000900000000000000060000000000000",
			"00000000000000000000009900000000000111111111100000000000000000000",
			"00000000000000000000099111111111111111111111100000000000000000000",
			"11111111111111111111111111111111111111111111111111111111110000111",
			"11111111111111111111111111111111111111111111111111111111110000111",
			"11111111111111111111111111111111111111111111111111111111110000111",
		   //0        10	    20        30        40        50		60
		   // 25
		   //0        10	    20        30        40        50		60
			"11111111111111111111111111111111111111111111111111111111110000111",
			"11111111111111111111111111111111111111111111111111111111110000111"
		};
		segment.data = segametData01;

		segment.width = segment.data[0].Length;
		segment.height = segment.data.Length;

		segment.segmentStartPos = new MapPoint (0, 0);
		segment.startPoint = new MapPoint (2, 21);
		segment.endPoint = new MapPoint (60, 22);

		segment.lockScreenLeft = true;
		segment.lockScreenRight = false;
		segment.lockScreenTop = true;
		segment.lockScreenBottom = true;
		segment.CalculateBorder ();

		elementQuestion = new LevelElementQuestion ( 11, 19, LevelElementQuestion.Type_CoinOne, LevelElementQuestion.Sprite_Question );
		segment.elements.Add (elementQuestion);

		elementQuestion = new LevelElementQuestion ( 12, 19, LevelElementQuestion.Type_CoinOne, LevelElementQuestion.Sprite_Question );
		segment.elements.Add (elementQuestion);

		elementPillar = new LevelElementPillar ( 58, 20, MapElement.Dir_Up, 7 );
		segment.elements.Add (elementPillar);

		/*
		elementCoinArrow = new LevelElementCoinArrow ( 24, 21, MapElement.Dir_LeftUp );
		segment.elements.Add (elementCoinArrow);




	    elementStop = new LevelElementStop ( 35, 21, MapElementStop.StopType_Direct );
		segment.elements.Add (elementStop);



		enemyMushroom = new LevelElementEnemyMushroon (33, 18, MapElement.Dir_Left, MapElement.Dir_Left, 0);
		segment.elements.Add (enemyMushroom);
*/
		levelDef.segments.Add (segment); 

		// ************* 第二段 ********************************
		segment = new LevelSegment();

		string[] segametData02 = {
			// 0
		   //0        10	    20        30        40        50		60
			"00000000000000000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000000000000000000000000000000000",
		   //0        10	    20        30        40        50		60
			// 5
		   //0        10	    20        30        40        50		60
			"00000000000000000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000000000000000000000000009990000",
		   //0        10	    20        30        40        50		60
			// 10
		   //0        10	    20        30        40        50		60
			"0000000000000000000000000000000000000000000000000000000000AAA0000",
			"00000000000000000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000000000000000000000000000000000",
		   //0        10	    20        30        40        50		60
			// 15
		   //0        10	    20        30        40        50		60
			"00000000000000000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000068600000000111111114E00000000000",
		   //0        10	    20        30        40        50		60
			// 20
		   //0        10	    20        30        40        50		60
			"0000000000000000000000000000000000000000000011111111114E000000000",
			"000000000000990000009900000000000000000000001111111111114E0000000",
			"0000000003111111111111111014E000000000000011111111111111114E00000",
			"11111111111111111111111111111111111111110011111111111111111111111",
			"11111111111111111111111111111111111111110011111111111111111111111",
		   //0        10	    20        30        40        50		60
			// 25
		   //0        10	    20        30        40        50		60
			"11111111111111111111111111111111111111110011111111111100001111111",
			"11111111111111111111111111111111111111110011111111111100001111111"
		};
		segment.data = segametData02;

		segment.width = segment.data[0].Length;
		segment.height = segment.data.Length;

		segment.segmentStartPos = new MapPoint (64, 0);
		segment.startPoint = new MapPoint (2, 22);
		segment.endPoint = new MapPoint (63, 22);

		segment.lockScreenLeft = false;
		segment.lockScreenRight = false;
		segment.lockScreenTop = true;
		segment.lockScreenBottom = true;
		segment.CalculateBorder ();

		elementQuestion = new LevelElementQuestion ( 7, 19, LevelElementQuestion.Type_CoinFive, LevelElementQuestion.Sprite_Brick );
		segment.elements.Add (elementQuestion);

		elementStop = new LevelElementStop ( 25, 22, MapElementStop.StopType_Tilted );
		segment.elements.Add (elementStop);

		elementQuestion = new LevelElementQuestion ( 32, 19, LevelElementQuestion.Type_CoinOne, LevelElementQuestion.Sprite_Brick );
		segment.elements.Add (elementQuestion);

		elementQuestion = new LevelElementQuestion ( 34, 19, LevelElementQuestion.Type_CoinOne, LevelElementQuestion.Sprite_Brick );
		segment.elements.Add (elementQuestion);

		elementQuestion = new LevelElementQuestion ( 36, 19, LevelElementQuestion.Type_CoinOne, LevelElementQuestion.Sprite_Brick );
		segment.elements.Add (elementQuestion);

		elementQuestion = new LevelElementQuestion ( 33, 15, LevelElementQuestion.Type_CoinOne, LevelElementQuestion.Sprite_Question );
		segment.elements.Add (elementQuestion);

		elementQuestion = new LevelElementQuestion ( 34, 15, LevelElementQuestion.Type_AwardLife, LevelElementQuestion.Sprite_Question );
		segment.elements.Add (elementQuestion);

		elementQuestion = new LevelElementQuestion ( 35, 15, LevelElementQuestion.Type_CoinFive, LevelElementQuestion.Sprite_Question );
		segment.elements.Add (elementQuestion);

		elementQuestion = new LevelElementQuestion ( 52, 15, LevelElementQuestion.Type_CoinFive, LevelElementQuestion.Sprite_Question );
		segment.elements.Add (elementQuestion);

		enemyMushroom = new LevelElementEnemyMushroon (18, 21, MapElement.Dir_Left, MapElement.Dir_Left, 0);
		segment.elements.Add (enemyMushroom);

		enemyMushroom = new LevelElementEnemyMushroon (26, 21, MapElement.Dir_Left, MapElement.Dir_Left, 0);
		segment.elements.Add (enemyMushroom);

		levelDef.segments.Add (segment);

		// ************* 第三段 ********************************
		segment = new LevelSegment();

		string[] segametData03 = {
			// 0
		   //0        10	    20        30        40        50		60
			"00000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000000000000000000000",
		   //0        10	    20        30        40        50		60
			// 5
		   //0        10	    20        30        40        50		60
			"00000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000000000000000000000",
		   //0        10	    20        30        40        50		60
			// 10
		   //0        10	    20        30        40        50		60
			"00000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000006666666600000000000",
			"00000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000000000000000000000",
		   //0        10	    20        30        40        50		60
			// 15
		   //0        10	    20        30        40        50		60
			"00000000000000000000000000000000000000000000000000000",
			"00000000000000000000077777077770000000000000000000000",
			"00000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000000000000000000000",
			"00000000000090909090900000000000000000000011111100000",
		   //0        10	    20        30        40        50		60
			// 20
		   //0        10	    20        30        40        50		60
			"00000000000777777777777700000000000000010011111100000",
			"000000000000000000000000000000002D1111110011111100000",
			"0000000000000000000000000000002D111111110011111100000",
			"11111111111111100111111111111111111111110011111111111",
			"11111111111111100111111111111111111111110011111111111",
		   //0        10	    20        30        40        50		60
			// 25
		   //0        10	    20        30        40        50		60
			"11111111111111100111111111111111111111110011111111111",
			"11111111111111100111111111111111111111110011111111111"
		};
		segment.data = segametData03;

		segment.width = segment.data[0].Length;
		segment.height = segment.data.Length;

		segment.segmentStartPos = new MapPoint (128, 0);
		segment.startPoint = new MapPoint (2, 22);
		segment.endPoint = new MapPoint (51, 22);

		segment.lockScreenLeft = false;
		segment.lockScreenRight = false;
		segment.lockScreenTop = true;
		segment.lockScreenBottom = true;
		segment.CalculateBorder ();

		elementQuestion = new LevelElementQuestion ( 38, 8, LevelElementQuestion.Type_AwardLife, LevelElementQuestion.Sprite_Brick );
		segment.elements.Add (elementQuestion);

		elementStop = new LevelElementStop ( 26, 16, MapElementStop.StopType_Tilted );
		segment.elements.Add (elementStop);

		elementQuestion = new LevelElementQuestion ( 45, 15, LevelElementQuestion.Type_CoinFive, LevelElementQuestion.Sprite_Brick );
		segment.elements.Add (elementQuestion);

		elementCoinArrow = new LevelElementCoinArrow ( 13, 22, MapElement.Dir_RightUp );
		segment.elements.Add (elementCoinArrow);

		elementCoinArrow = new LevelElementCoinArrow ( 22, 19, MapElement.Dir_RightUp );
		segment.elements.Add (elementCoinArrow);

		elementCoinArrow = new LevelElementCoinArrow ( 30, 15, MapElement.Dir_RightDown );
		segment.elements.Add (elementCoinArrow);

		enemyMushroom = new LevelElementEnemyMushroon (9, 22, MapElement.Dir_Left, MapElement.Dir_Left, 0);
		segment.elements.Add (enemyMushroom);

		enemyMushroom = new LevelElementEnemyMushroon (15, 19, MapElement.Dir_Left, MapElement.Dir_Left, 0);
		segment.elements.Add (enemyMushroom);

		enemyMushroom = new LevelElementEnemyMushroon (22, 19, MapElement.Dir_Left, MapElement.Dir_Left, 0);
		segment.elements.Add (enemyMushroom);

		enemyMushroom = new LevelElementEnemyMushroon (29, 15, MapElement.Dir_Left, MapElement.Dir_Left, 0);
		segment.elements.Add (enemyMushroom);

		enemyMushroom = new LevelElementEnemyMushroon (34, 20, MapElement.Dir_Left, MapElement.Dir_Left, 0);
		segment.elements.Add (enemyMushroom);

		enemyMushroom = new LevelElementEnemyMushroon (39, 19, MapElement.Dir_Left, MapElement.Dir_Left, 0);
		segment.elements.Add (enemyMushroom);

		enemyMushroom = new LevelElementEnemyMushroon (41, 10, MapElement.Dir_Left, MapElement.Dir_Left, 0);
		segment.elements.Add (enemyMushroom);

		enemyMushroom = new LevelElementEnemyMushroon (48, 12, MapElement.Dir_Left, MapElement.Dir_Left, 0);
		segment.elements.Add (enemyMushroom);

		levelDef.segments.Add (segment);

		// ************* 第四段 ********************************
		segment = new LevelSegment();

		string[] segametData04 = {
			// 0
		   //0        10	    20        30        40        50		60
			"00000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000000000000000000000",
		   //0        10	    20        30        40        50		60
			// 5
		   //0        10	    20        30        40        50		60
			"00000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000000009990000000000",
			"00000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000009900009900000000",
			"00000000000000000000000000000006000006AAA0AAA00000000",
		   //0        10	    20        30        40        50		60
			// 10
		   //0        10	    20        30        40        50		60
			"00000000000000000000000000000006000000000000000000000",
			"00000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000006000000000000000",
			"00000000000000000000000000000000000000000000000000000",
		   //0        10	    20        30        40        50		60
			// 15
		   //0        10	    20        30        40        50		60
			"00000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000000000000000000000",
			"00000000000000000000000000000000000000000000000000000",
			"00000000000000000000000077770777777777700000000000000",
		   //0        10	    20        30        40        50		60
			// 20
		   //0        10	    20        30        40        50		60
			"00000000000000000000000000000000000000000000000000000",
			"00000000990000990000090000000000000000000000000000000",
			"00000009009009009009009000000000000000000000000000000",
			"11111111001111001111001111111111111111111111001111111",
			"11111111001111001111001111111111111111111111001111111",
		   //0        10	    20        30        40        50		60
			// 25
		   //0        10	    20        30        40        50		60
			"11111111001111001111001111111111111111111111001111111",
			"11111111001111001111001111111111111111111111001111111"
		};
		segment.data = segametData04;

		segment.width = segment.data[0].Length;
		segment.height = segment.data.Length;

		segment.segmentStartPos = new MapPoint (180, 0);
		segment.startPoint = new MapPoint (2, 22);
		segment.endPoint = new MapPoint (51, 22);

		segment.lockScreenLeft = false;
		segment.lockScreenRight = false;
		segment.lockScreenTop = true;
		segment.lockScreenBottom = true;
		segment.CalculateBorder ();

		elementPillar = new LevelElementPillar (  8, 22, MapElement.Dir_Up, 5 );
		segment.elements.Add (elementPillar);

		elementPillar = new LevelElementPillar ( 14, 22, MapElement.Dir_Up, 5 );
		segment.elements.Add (elementPillar);

		elementPillar = new LevelElementPillar ( 20, 22, MapElement.Dir_Up, 5 );
		segment.elements.Add (elementPillar);

		elementCoinArrow = new LevelElementCoinArrow ( 20, 21, MapElement.Dir_RightUp );
		segment.elements.Add (elementCoinArrow);

		elementStop = new LevelElementStop ( 28, 19, MapElementStop.StopType_Direct );
		segment.elements.Add (elementStop);

		elementQuestion = new LevelElementQuestion ( 25, 16, LevelElementQuestion.Type_CoinOne, LevelElementQuestion.Sprite_Question );
		segment.elements.Add (elementQuestion);

		elementQuestion = new LevelElementQuestion ( 41, 4, LevelElementQuestion.Type_AwardStar, LevelElementQuestion.Sprite_Question );
		segment.elements.Add (elementQuestion);


		elementCoinArrow = new LevelElementCoinArrow ( 32, 10, MapElement.Dir_RightUp );
		segment.elements.Add (elementCoinArrow);

		enemyMushroom = new LevelElementEnemyMushroon (28, 22, MapElement.Dir_Left, MapElement.Dir_Left, 0);
		segment.elements.Add (enemyMushroom);

		enemyMushroom = new LevelElementEnemyMushroon (34, 22, MapElement.Dir_Left, MapElement.Dir_Left, 0);
		segment.elements.Add (enemyMushroom);


		levelDef.segments.Add (segment);

		// ************* 第五段 ********************************
		segment = new LevelSegment();

		string[] segametData05 = {
			// 0
		   //0        10	    20        30        40        50		60
			"0000000000000000000000000000000000000000000000000000000",
			"0000000000000000000000000000000000000000000000000000000",
			"0000000000000000000000000000000000000000000000000000000",
			"0000000000000000000000000000000000000000000000000000000",
			"0000000000000000000000000000000000000000000000000000000",
		   //0        10	    20        30        40        50		60
			// 5
		   //0        10	    20        30        40        50		60
			"0000000000000000000000000000000000000000000000000000000",
			"0000000000000000000000000000000000000000000000000000000",
			"0000000000000000000000000000000000000000000000000000000",
			"0000000000000000000000000000000000000000000000000000000",
			"0000000000000000000000000000000000000000000000000000000",
		   //0        10	    20        30        40        50		60
			// 10
		   //0        10	    20        30        40        50		60
			"0000000000000000000000000000000000000000000000000000000",
			"0000000000000000000000000000000000000000000000000000000",
			"0000000000000000000000000000000000000000000000000000000",
			"0000000000000000000000000000000000000000000000000000000",
			"0000000000000000000000000000000000000000000000000000000",
		   //0        10	    20        30        40        50		60
			// 15
		   //0        10	    20        30        40        50		60
			"0000000000000000000000000000000000000000000000000000000",
			"0000000000000000000000000000000000000000000000000000000",
			"0000000000000000000000999990000000000000000999000000000",
			"0000000000009999000000000000000000000000009000900000000",
			"0000000000000000000777777777777770000000000000000000000",
		   //0        10	    20        30        40        50		60
			// 20
		   //0        10	    20        30        40        50		60
			"0000990000000000000000999990000000000007777777777700000",
			"00000000002D11111114E0000000000000000000000000000000000",
			"000000002D11111111111111111114E000000000000000000000000",
			"1111111111111111111111111111111111111111111111111111111",
			"1111111111111111111111111111111111111111111111111111111",
		   //0        10	    20        30        40        50		60
			// 25
		   //0        10	    20        30        40        50		60
			"1111111111111111111111111111111111111111111111111111111",
			"1111111111111111111111111111111111111111111111111111111"
		};
		segment.data = segametData05;

		segment.width = segment.data[0].Length;
		segment.height = segment.data.Length;

		segment.segmentStartPos = new MapPoint (233, 0);
		segment.startPoint = new MapPoint (2, 22);
		segment.endPoint = new MapPoint (54, 22);

		segment.lockScreenLeft = false;
		segment.lockScreenRight = false;
		segment.lockScreenTop = true;
		segment.lockScreenBottom = true;
		segment.CalculateBorder ();


		elementCoinArrow = new LevelElementCoinArrow ( 20, 21, MapElement.Dir_RightDown );
		segment.elements.Add (elementCoinArrow);


		enemyMushroom = new LevelElementEnemyMushroon (12, 20, MapElement.Dir_Left, MapElement.Dir_Left, 0);
		segment.elements.Add (enemyMushroom);

		enemyMushroom = new LevelElementEnemyMushroon (18, 20, MapElement.Dir_Left, MapElement.Dir_Left, 0);
		segment.elements.Add (enemyMushroom);

		enemyMushroom = new LevelElementEnemyMushroon (23, 18, MapElement.Dir_Left, MapElement.Dir_Left, 0);
		segment.elements.Add (enemyMushroom);

		enemyMushroom = new LevelElementEnemyMushroon (29, 18, MapElement.Dir_Left, MapElement.Dir_Left, 0);
		segment.elements.Add (enemyMushroom);

		enemyMushroom = new LevelElementEnemyMushroon (33, 22, MapElement.Dir_Left, MapElement.Dir_Left, 0);
		segment.elements.Add (enemyMushroom);

		enemyMushroom = new LevelElementEnemyMushroon (39, 22, MapElement.Dir_Left, MapElement.Dir_Left, 0);
		segment.elements.Add (enemyMushroom);

		enemyMushroom = new LevelElementEnemyMushroon (45, 19, MapElement.Dir_Left, MapElement.Dir_Left, 0);
		segment.elements.Add (enemyMushroom);

		enemyMushroom = new LevelElementEnemyMushroon (51, 22, MapElement.Dir_Left, MapElement.Dir_Left, 0);
		segment.elements.Add (enemyMushroom);

		levelDef.segments.Add (segment);

		// ************* 第六段 ********************************
		segment = new LevelSegment();

		string[] segametData06 = {
			// 0
		   //0        10	    20        30        40        50		60
			"0000000000000000000000000000000000000000000000000000000",
			"0000000000000000000000000000000000000000000000000000000",
			"0000000000000000000000000000000000000000000000000000000",
			"0000000000000000000000000000000000000000000000000000000",
			"0000000000000000000000000000000000000000000000000000000",
		   //0        10	    20        30        40        50		60
			// 5
		   //0        10	    20        30        40        50		60
			"0000000000000000000000000000000000000000000000000000000",
			"0000000000000000000000000000000000000000000000000000000",
			"0000000000000000000000000000000000000000000000000000000",
			"0000000000000000000000000000000000000000000000000000000",
			"0000000000000000000000000000000000000000000000000000000",
		   //0        10	    20        30        40        50		60
			// 10
		   //0        10	    20        30        40        50		60
			"0000000000000000000000000000000000099000000000000000000",
			"0000000000000000000000000000000000900000000000000000000",
			"0000000000000000000000000000000009000000000000000000000",
			"0000000000000000000000000000000090000000000000000000000",
			"0000000000000000000000000000000900000000000000000000000",
		   //0        10	    20        30        40        50		60
			// 15
		   //0        10	    20        30        40        50		60
			"0000000000000000000000900000009000000000000000000000000",
			"0000000000000099000009000777777000000000000000000000000",
			"0000000000000900900090000000000000000000000000000000000",
			"0000000000009000900900000000000000000000000000000000000",
			"0000000000090000099000000000000000000000000000000000000",
		   //0        10	    20        30        40        50		60
			// 20
		   //0        10	    20        30        40        50		60
			"0000000000900077777777700000000000000000000000000000000",
			"0000000009000000000000000000000000000000000000000000000",
			"0000009090000000000000000000000000000000000000000000000",
			"1111111111111111111111111111111111111111111111111111111",
			"1111111111111111111111111111111111111111111111111111111",
		   //0        10	    20        30        40        50		60
			// 25
		   //0        10	    20        30        40        50		60
			"1111111111111111111111111111111111111111111111111111111",
			"1111111111111111111111111111111111111111111111111111111"
		};
		segment.data = segametData06;

		segment.width = segment.data[0].Length;
		segment.height = segment.data.Length;

		segment.segmentStartPos = new MapPoint (288, 0);
		segment.startPoint = new MapPoint (2, 22);
		segment.endPoint = new MapPoint (42, 22);

		segment.lockScreenLeft = false;
		segment.lockScreenRight = false;
		segment.lockScreenTop = true;
		segment.lockScreenBottom = true;
		segment.CalculateBorder ();


		enemyMushroom = new LevelElementEnemyMushroon (10, 22, MapElement.Dir_Left, MapElement.Dir_Left, 0);
		segment.elements.Add (enemyMushroom);

		enemyMushroom = new LevelElementEnemyMushroon (16, 19, MapElement.Dir_Left, MapElement.Dir_Left, 0);
		segment.elements.Add (enemyMushroom);

		enemyMushroom = new LevelElementEnemyMushroon (22, 19, MapElement.Dir_Left, MapElement.Dir_Left, 0);
		segment.elements.Add (enemyMushroom);

		enemyMushroom = new LevelElementEnemyMushroon (27, 15, MapElement.Dir_Left, MapElement.Dir_Left, 0);
		segment.elements.Add (enemyMushroom);

		enemyMushroom = new LevelElementEnemyMushroon (34, 22, MapElement.Dir_Left, MapElement.Dir_Left, 0);
		segment.elements.Add (enemyMushroom);

		enemyMushroom = new LevelElementEnemyMushroon (40, 22, MapElement.Dir_Left, MapElement.Dir_Left, 0);
		segment.elements.Add (enemyMushroom);


		levelDef.segments.Add (segment);

		_localLevels.Add (levelDef);
	}
}
