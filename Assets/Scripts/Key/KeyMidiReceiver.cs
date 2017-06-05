using UnityEngine;

public class KeyMidiReceiver : MonoBehaviour {
    private KeySound keySound;
    private bool pressed;

    // Use this for initialization
    void Start () {
        keySound = gameObject.GetComponent<KeySound>();
    }
	
	// Update is called once per frame
	void Update () {
        var transform = gameObject.GetComponent<Transform>();

        //var rotation_x = (Mathf.Sin(Time.time * 20.0f) * 3.0f) - 87.0f;

        var rotation_x = pressed ? -84.5f : -90.0f;

        var rotation = Quaternion.Euler(rotation_x, 0, 0);

        transform.SetPositionAndRotation(transform.position, rotation);
	}

    public void SetPressed(float velocity)
    {
        // TODO: fix and enable later
        //if (pressed)
        //{
        //    return;
        //}

        pressed = true;

        if (keySound != null)
        {
            keySound.Play();
        }

        //Debug.Log("Pressed Note=" + gameObject.name + " Velocity= " + velocity);
    }

    public void SetReleased()
    {
        if (!pressed)
        {
            return;
        }

        pressed = false;

        if (keySound != null)
        {
            keySound.Pause();
        }

        //Debug.Log("Release Note=" + gameObject.name);
    }
}
