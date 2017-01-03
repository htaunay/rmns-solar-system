using UnityEngine;
using System.Collections;

public class FlyNavigator : MonoBehaviour
{
	public enum NavigationMode { Manual, Automatic };

	[SerializeField]
	private NavigationMode Mode = NavigationMode.Manual;

	[SerializeField]
	private float translationSpeed = 1;

	[SerializeField]
	private float rotationSpeed = 50;

	private float lastMouseRoll = 0;

	[SerializeField]
	private float mouseWheelPotency = 1f;

	private GUIStyle style = new GUIStyle();

	private void Start()
	{
		style.fontSize = 36;
		style.normal.textColor = Color.yellow;
	}

	private void OnGUI ()
	{
		GUI.Label(new Rect(20, 30, 100, 50), (translationSpeed / 150.0f) + " AU/s", style);
	}

	private Vector3 GetRotation()
	{
		Vector3 rotation = Vector3.zero;

		rotation += Vector3.up * Input.GetAxis("Mouse X");
		rotation += Vector3.left * Input.GetAxis("Mouse Y");

		return rotation * rotationSpeed * Mathf.Clamp(Time.deltaTime, 0.0f, 20.0f);
	}
	
	private Vector3 GetTranslation()
	{
		Vector3 translation = Vector3.zero;

		if (Input.GetKey (KeyCode.A)) translation += Vector3.left;
		if (Input.GetKey (KeyCode.D)) translation += Vector3.right;

		if(Input.GetKey (KeyCode.LeftShift))
		{
			if (Input.GetKey (KeyCode.W)) translation += Vector3.up;
			if (Input.GetKey (KeyCode.S)) translation += Vector3.down;
		}
		else
		{
			if (Input.GetKey (KeyCode.W)) translation += Vector3.forward;
			if (Input.GetKey (KeyCode.S)) translation += Vector3.back;
		}

		return translation.normalized * translationSpeed * Time.deltaTime;
	}

	public void SetTranslationSpeed(float speed)
	{
		translationSpeed = speed;
	}

	private void Update()
	{
		transform.eulerAngles += GetRotation();
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0.0f);

		transform.Translate(GetTranslation());

		if (Input.GetKeyDown (KeyCode.M))
			Mode = (Mode == NavigationMode.Manual) ? NavigationMode.Automatic : NavigationMode.Manual;

		// Update speed
		if (Mode == NavigationMode.Automatic) {
			Server.Instance.UpdateTranslationSpeed (transform.position, this);
		}
		else {
			float speedVariation = (Input.GetAxis ("Mouse ScrollWheel") * mouseWheelPotency) + 1;

			translationSpeed *= speedVariation;
			translationSpeed = Mathf.Clamp (translationSpeed, 0.1f, 100f);
		}
	}
}