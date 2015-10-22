using UnityEngine;
using System.Collections;

public class Enemy_2 : Enemy {

	public Vector3[] points;
	public float birthTime;
	public float lifeTime = 10;

	public float sinEccentricity = 0.6f;
	void Start () {

		points = new Vector3[2];

		Vector3 cbMin = Utils.camBounds.min;
		Vector3 cbMax = Utils.camBounds.max;
		Vector3 v = Vector3.zero;

		// Pick any point on the left side of the screen
		v.x = cbMin.x - Main.S.enemySpawnPadding;
		v.y = Random.Range( cbMin.y, cbMax.y );
		points[0] = v;

		// Pick any point on the right side of the screen
		v = Vector3.zero;
		v.x = cbMax.x + Main.S.enemySpawnPadding;
		v.y = Random.Range( cbMin.y, cbMax.y );
		points[1] = v;

		// Change the direction
		if (Random.value < 0.5f) {
			points[0].x *= -1;
			points[1].x *= -1;
		}

		birthTime = Time.time;
	}
	public override void Move() {
		// Bézier curves work based on a u value between 0 & 1
		float u = (Time.time - birthTime) / lifeTime;
		if (u > 1) {
			Destroy( this.gameObject );
			return;
		}

		// Adjust u by adding an easing curve based on a Sine wave
		u = u + sinEccentricity*(Mathf.Sin(u*Mathf.PI*2));
		// Interpolate the two linear interpolation points
		pos = (1-u)*points[0] + u*points[1];
	}
}
