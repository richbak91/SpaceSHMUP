using UnityEngine;
using System.Collections;

public class Hero : MonoBehaviour {
	static public Hero S; 

	public float speed = 30;
	public float rollMult = -45;
	public float pitchMult = 30;
	public float gameRestartDelay = 2f;
	public Weapon[] weapons;

	public Bounds bounds;
	
	public delegate void WeaponFireDelegate();
	public WeaponFireDelegate fireDelegate;

	void Awake() {
		S = this;
		bounds = Utils.CombineBoundsOfChildren(this.gameObject);
	}

	void Start() {
		ClearWeapons();
		weapons[0].SetType(WeaponType.blaster);
	}

	void Update () {
		//Get user inputs
		float xAxis = Input.GetAxis("Horizontal"); 
		float yAxis = Input.GetAxis("Vertical"); 

		//Change the position transform based off of user input
		Vector3 pos = transform.position;

		bounds.center = transform.position; 
		Vector3 off = Utils.ScreenBoundsCheck(bounds, BoundsTest.onScreen); 
		if ( off != Vector3.zero ) {
			pos -= off;
			transform.position = pos;
		}

		pos.x += xAxis * speed * Time.deltaTime;
		pos.y += yAxis * speed * Time.deltaTime;
		transform.position = pos;

		//Rotate ship for aesthetic purposes
		transform.rotation = Quaternion.Euler(yAxis*pitchMult,xAxis*rollMult,0);

		//Jump is the spacebar. Also checks for preset
		if (Input.GetAxis("Jump") == 1 && fireDelegate != null) { 
			fireDelegate();
		}
	}

	public GameObject lastTriggerGo = null;
	void OnTriggerEnter(Collider other) {

		GameObject go = Utils.FindTaggedParent(other.gameObject);

		if (go != null) {
			if (go == lastTriggerGo) { 
				return;
			}

			lastTriggerGo = go; 
			if (go.tag == "Enemy") {
				shieldLevel--;
				Destroy(go); 
			} 
			else if (go.tag == "PowerUp") {
				AbsorbPowerUp(go);
			}
			else {
				print("Triggered: "+go.name); 
			}
		} else {
			print("Triggered: "+other.gameObject.name); 
		}
	}

	public void AbsorbPowerUp( GameObject go ) {
		PowerUp pu = go.GetComponent<PowerUp>();
		switch (pu.type) {
		case WeaponType.shield: 
			shieldLevel++;
			break;
		default: 

			if (pu.type == weapons[0].type) {

				Weapon w = GetEmptyWeaponSlot(); 
				if (w != null) {
					w.SetType(pu.type);
				}
			} else {
				ClearWeapons();
				weapons[0].SetType(pu.type);
			}
			break;
		}
		pu.AbsorbedBy( this.gameObject );
	}

	Weapon GetEmptyWeaponSlot() {
		for (int i=0; i<weapons.Length; i++) {
			if ( weapons[i].type == WeaponType.none ) {
				return( weapons[i] );
			}
		}
		return( null );
	}

	void ClearWeapons() {
		foreach (Weapon w in weapons) {
			w.SetType(WeaponType.none);
		}
	}

	[SerializeField]
	private float _shieldLevel = 1;

	public float shieldLevel {
		get {
			return( _shieldLevel ); 
		}
		set {
			_shieldLevel = Mathf.Min( value, 4 ); 
			if (value < 0) {
				Destroy(this.gameObject);
				Main.S.DelayedRestart( gameRestartDelay );
			}
		}
	}

}