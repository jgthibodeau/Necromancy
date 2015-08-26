using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalScript : MonoBehaviour {
	//Controller stuff
	public static float deadzone = 0.25f;
	public static string Interact = "Interact";
	public static string Cancel = "Cancel";
	public static string LeftStick = "Left";
	public static string RightStick = "Right";
	public static string LeftTrigger = "Trigger Left";
	public static string RightTrigger = "Trigger Right";

	public enum InputType{Button,Axis,Trigger};

	public static bool GetButton(string action){
		return Input.GetButtonDown (action);
	}
	public static float GetTrigger(string action){
		return Input.GetAxis (action);
	}
	public static Vector2 GetAxis(string action){
		Vector2 stickInput = new Vector2 (Input.GetAxis ("Horizontal " + action), Input.GetAxis ("Vertical " + action));
		if (stickInput.magnitude < deadzone)
			stickInput = Vector2.zero;
		else
			stickInput = stickInput.normalized * ((stickInput.magnitude - deadzone) / (1 - deadzone));

//		Vector2 stickInput = new Vector2 (Input.GetAxis ("Horizontal " + action), Input.GetAxis ("Vertical " + action));
//		if(stickInput.magnitude < deadzone)
//			stickInput = Vector2.zero;

		return stickInput;
	}

	//Gamestate stuff
	public enum GameState{InGame, Paused};
	public static GameState currentGameState = GameState.InGame;

	public void ChangeState(GameState state){
		currentGameState = state;
		switch(currentGameState){
		case GameState.InGame:
			break;
		case GameState.Paused:
			break;
		}
	}

	//Raycasting layermasks
	public static int IgnoreInteractableLayerMask = int.MaxValue & ~(1 << LayerMask.NameToLayer("Interactable")) & ~(1 << LayerMask.NameToLayer("Ignore Raycast"));
	public static int InteractableLayerMask = 1 << LayerMask.NameToLayer("Interactable");

	//Color
	public static Color orange = new Color(1F, 0.549F, 0F);

	//Polygon utilities
	public static bool IsPointInPolygon(List<Vector2> polygon, Vector2 testPoint){
		bool result = false;
		int j = polygon.Count - 1;
		for (int i = 0; i < polygon.Count; i++)
		{
			if (polygon[i].y < testPoint.y && polygon[j].y >= testPoint.y || polygon[j].y < testPoint.y && polygon[i].y >= testPoint.y)
			{
				if (polygon[i].x + (testPoint.y - polygon[i].y) / (polygon[j].y - polygon[i].y) * (polygon[j].x - polygon[i].x) < testPoint.x)
				{
					result = !result;
				}
			}
			j = i;
		}
		return result;
	}
	public static bool IsPointInPolygon2(List<Vector2> poly, Vector2 pnt )
	{
		int i, j;
		int nvert = poly.Count;
		bool c = false;
		for (i = 0, j = nvert - 1; i < nvert; j = i++)
		{
			if (((poly[i].y > pnt.y) != (poly[j].y > pnt.y)) &&
			    (pnt.x < (poly[j].x - poly[i].x) * (pnt.y - poly[i].y) / (poly[j].y - poly[i].y) + poly[i].x))
				c = !c; 
		}
		return c;
	}

	//Array utilities
	public static void ShuffleArray<T>(T[] arr) {
		for (int i = arr.Length - 1; i > 0; i--) {
			int r = Random.Range(0, i);
			T tmp = arr[i];
			arr[i] = arr[r];
			arr[r] = tmp;
		}
	}
}
