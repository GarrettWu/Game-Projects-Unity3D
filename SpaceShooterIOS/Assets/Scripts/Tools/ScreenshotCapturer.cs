using UnityEngine;
using System.Collections;

public class ScreenshotCapturer : MonoBehaviour {

	private int screenshotCount = 0;

	void Update(){
		ScreenshotCapture();
	}

	void ScreenshotCapture(){
		if (Input.GetKeyDown(KeyCode.R)){
			Application.CaptureScreenshot("Screenshots/Screenshot" + screenshotCount.ToString() + ".png");
			screenshotCount++;
		}
	}
}
