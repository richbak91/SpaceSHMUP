using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour {
	public float rotationsPerSecond = 0.1f;

	public int levelShown = 0;
	void Update () {

		// Read the current shield level from the Hero Singleton
		int currLevel = Mathf.FloorToInt( Hero.S.shieldLevel ); // 1

		// If the current shield level is different than the shown one, change it.
		if (levelShown != currLevel) {
			levelShown = currLevel;
			Material mat = this.renderer.material;
			//Change the shield
			mat.mainTextureOffset = new Vector2( 0.2f*levelShown, 0 ); // 2
		}

		// Rotate the shield over time
		float rZ = (rotationsPerSecond*Time.time*360) % 360f; // 3
		transform.rotation = Quaternion.Euler( 0, 0, rZ );
	}
}