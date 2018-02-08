using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class MapElementQuestionBlock : MapElement {
	int _subtype;
	int _spriteType;

	bool _isBroken;

	bool _hitForward;


	void Awake() {
		type = Type_Question;
		_isBroken = false;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetData( int vSubtype, int vSpriteType ){
		_subtype = vSubtype;
		_spriteType = vSpriteType;

		if (_spriteType == LevelElementQuestion.Sprite_Question) {
			SptMain.sprite = GameMap.instance.SptQuestionBlock;
		} else {
			SptMain.sprite = GameMap.instance.SptQuestionBrick;
		}

	}

	public override void HitBottom(bool hitForward) {
		if (_isBroken == true) {
			return;
		}

		_hitForward = hitForward;

		GameObject go;
		Vector3 pos;

		_isBroken = true;

		Sequence seq = DOTween.Sequence ();
		seq.Append (SptMain.transform.DOMoveY (SptMain.transform.position.y + 0.5f, 0.1f).OnComplete( ()=> {
			SptMain.sprite = GameMap.instance.SptWallFixed;
			switch (_subtype) {
			case LevelElementQuestion.Type_CoinOne:
				go = (GameObject)GameObject.Instantiate (GameMap.instance.GoMapElementCoin);
				go.transform.SetParent (GameMap.instance.GoGameMap.transform);
				go.transform.position = transform.position;
				go.SetActive (true);

				CoinElement element = (CoinElement)go.GetComponent<CoinElement> ();
				element.SetSubtype (0);

				DOTween.Play (go.transform.DOMoveY (transform.position.y + 5.5f, 2.2f).OnComplete (() => {
					GameObject.Destroy (go);
				}));
				break;
			case LevelElementQuestion.Type_CoinFive:
				for (int m = 0; m < 5; m++) {
					go = (GameObject)GameObject.Instantiate (GameMap.instance.GoMapElementCoinEject);
					go.transform.SetParent (GameMap.instance.GoGameMap.transform);
					pos = transform.position;
					pos.y+=0.8f;
					go.transform.position = pos;
					go.SetActive (true);

					CoinEject coinEject = (CoinEject)go.GetComponent<CoinEject> ();
					int sign = 1;
					if(_hitForward==false){
						sign = -1;
					}
					coinEject.SetColor(GameMap.instance.MapElementMainColor);
					coinEject.EjectOut( sign );

					GameMap.instance.AddActiveElement( coinEject );
				}
				break;
			case LevelElementQuestion.Type_AwardLife:
			case LevelElementQuestion.Type_AwardStar:{
					go = (GameObject)GameObject.Instantiate (GameMap.instance.GoMapElementBounceAward);
					go.transform.SetParent (GameMap.instance.GoGameMap.transform);
					pos = transform.position;
					pos.y+=0.8f;
					go.transform.position = pos;
					go.SetActive (true);

					BounceAward bounceAward = (BounceAward)go.GetComponent<BounceAward> ();
					switch( _subtype ) {

					case LevelElementQuestion.Type_AwardLife:
						bounceAward.SetSubtype (BounceAward.Subtype_AwardLife);
						break;
					case LevelElementQuestion.Type_AwardStar:
						bounceAward.SetSubtype (BounceAward.Subtype_AwardStar);
						break;
					}
					bounceAward.SetColor(GameMap.instance.MapElementMainColor);
					int sign = 1;
					if(_hitForward==false){
						sign = -1;
					}
					bounceAward.EjectOut( sign );

					GameMap.instance.AddActiveElement( bounceAward );
					break;
				}
			case LevelElementQuestion.Type_Mushroom:{
					go = (GameObject)GameObject.Instantiate (GameMap.instance.GoMapElementMushroom);
					go.transform.SetParent (GameMap.instance.GoGameMap.transform);
					pos = transform.position;
					pos.y+=0.8f;
					go.transform.position = pos;
					go.SetActive (true);

					Mushroom mushroom = (Mushroom)go.GetComponent<Mushroom> ();
					mushroom.SetColor(GameMap.instance.MapElementMainColor);
					int sign = 1;
					if(_hitForward==false){
						sign = -1;
					}
					mushroom.EjectOut( sign );

					GameMap.instance.AddActiveElement( mushroom );
					break;
				}
			}
		} ) );
		seq.Append (SptMain.transform.DOMoveY (SptMain.transform.position.y, 0.3f) );

		DOTween.Play (seq);
	}

	public override int IsBlock( int dir ) {
		if ((dir == MapElement.Dir_Down)&&(_isBroken==false)) {
			return Block_Soft;
		}

		return Block_Hard;
	}
}
