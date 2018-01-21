using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class LevelDef  {
	public int _width;
	public int _height;
	public int width {
		get {
			return _width;
		}
	}
	public int height {
		get {
			return _height;
		}
	}

	public List<LevelSegment> segments;

	public LevelDef() {
		segments = new List<LevelSegment> ();
	}
}

public class LevelSegment {
	public MapPoint segmentStartPos;

	public MapPoint startPoint;
	public MapPoint endPoint;
	public int width;
	public int height;

	public bool lockScreenLeft;
	public bool lockScreenRight;
	public bool lockScreenTop;
	public bool lockScreenBottom;

	public Vector2 leftTopPosition;
	public Vector2 rightBottomPosition;


	public string[] data;

	public List<LevelElement> elements;

	public LevelSegment() {
		elements = new List<LevelElement> ();
	}

	public void CalculateBorder() {
		MapPoint bottomRightPoint = new MapPoint (segmentStartPos.x + width, segmentStartPos.y + height);

		leftTopPosition =(Vector2) GameMap.instance.GetWorldPositionByMapPoint (segmentStartPos.x, segmentStartPos.y);
		rightBottomPosition =(Vector2) GameMap.instance.GetWorldPositionByMapPoint (bottomRightPoint.x, bottomRightPoint.y);
	}
}


public class LevelElement {
	public int type;
	public int x;
	public int y;

	public LevelElement(int vT, int vX, int vY ) {
		type = vT;
		x = vX;
		y = vY;
	}
}

public class LevelElementCoinArrow : LevelElement{
	public int dir;

	public LevelElementCoinArrow( int vX, int vY, int vD ) :base( MapElement.Type_CoinArrow, vX, vY ) {
		dir = vD;
	}
}

public class LevelElementPillar : LevelElement{
	public int dir;
	public int length;

	public LevelElementPillar( int vX, int vY, int vD, int vL ) :base( MapElement.Type_Pillar, vX, vY ) {
		dir = vD;
		length = vL;
	}
}

public class LevelElementStop : LevelElement{
	public int dir;

	public LevelElementStop( int vX, int vY, int vD ) :base( MapElement.Type_Stop, vX, vY ) {
		dir = vD;
	}
}

public class LevelElementQuestion : LevelElement{
	public int elementType;

	public LevelElementQuestion( int vX, int vY, int vT ) :base( MapElement.Type_Stop, vX, vY ) {
		elementType = vT;
	}
}