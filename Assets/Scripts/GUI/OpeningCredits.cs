using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OpeningCredits : MonoBehaviour {

	public Image[] panels;
	public float[] timeAtEachPanel;

	public float timer;
	public float lerpTimer = 0f;
	private float lerpDuration = 0.25f;
	public int currentPanel = 0;
	public bool isLerping;

	void Update () {
		if( currentPanel >= panels.Length ){
			return;
		} else {
			if( !isLerping ) {
				if( timer >= timeAtEachPanel[currentPanel] ) {
					timer = 0f;
					isLerping = true;
				}
				timer += Time.deltaTime;
			} else {
				if( lerpTimer >= lerpDuration ) {
					panels[currentPanel].gameObject.SetActive( false );
					isLerping = false;
					lerpTimer = 0f;
					currentPanel++;
					return;
				}
				Color temp = panels[currentPanel].color;
				float newAlpha = Mathf.Lerp( 1f, 0f, lerpTimer/lerpDuration );
				Image[] images = panels[currentPanel].GetComponentsInChildren<Image>();
				foreach( Image image in images ) {
					image.color = new Color( temp.r, temp.g, temp.b, newAlpha );
				}
				lerpTimer += Time.deltaTime;
			}
		}
	}
}
