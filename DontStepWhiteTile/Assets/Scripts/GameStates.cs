using UnityEngine;
using System.Collections;

public class GameStates : MonoBehaviour{
	public static int state;// 0 for start scene, 1 for game scene, 2 for end scene
	public static int mode;// 0 for TileAttack, 1 for LongRun, 2 for StoppingJust
	public static int step;
	public static float time;
	public static bool victory = false;	

	public static float[] highTimeAttack = new float[10];
	public static int[] highLongRun = new int[10];
	public static int[] highStoppingJust = new int[10];

	public const int TIME_ATTACK_STEPS = 50;
	public const float LONG_RUN_TIME = 10.0f;
	public const float STOPPING_JUST_TIME = 10.0f;
	
	public static void SaveGame(){
		for (int i = 0; i < 10; i++){
			PlayerPrefs.SetFloat ("HighTimeAttack" + i.ToString(), highTimeAttack[i]);
			PlayerPrefs.SetInt ("HighLongRun" + i.ToString(), highLongRun[i]);
			PlayerPrefs.SetInt ("HighStoppingJust" + i.ToString(), highStoppingJust[i]);
		}
	}
	
	public static void LoadGame(){
		for (int i = 0; i < 10; i++){
			highTimeAttack[i] = PlayerPrefs.GetFloat ("HighTimeAttack" + i.ToString());
			highLongRun[i] = PlayerPrefs.GetInt ("HighLongRun" + i.ToString());
			highStoppingJust[i] = PlayerPrefs.GetInt ("HighStoppingJust" + i.ToString());
		}
	}
}
