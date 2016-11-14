using UnityEngine;
using System.Collections;

public class DizzyEffect : MonoBehaviour {
    public GameObject star;
    private GameObject[] stars;
    public float spinSpeed = 5;
    public float radius = 2;

    // Use this for initialization
    void Start () {
        stars = new GameObject[5];
        for (int i = 0; i < 5; i++)
        {
            stars[i] = Instantiate(star, transform) as GameObject;
            stars[i].transform.localPosition = new Vector3(radius * Mathf.Sin( Mathf.Deg2Rad*i*72), 0, radius * Mathf.Cos(Mathf.Deg2Rad*i * 72));
        }
	}
	
	// Update is called once per frame
	void Update () {

        //transform.up = Vector3.up;
        transform.Rotate(0, spinSpeed,0,Space.World);
        if (transform.rotation.eulerAngles.x != 0 || transform.rotation.eulerAngles.z != 0)
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        foreach (GameObject s in stars)
            if(Camera.current != null)
                s.transform.forward = Camera.current.transform.forward;
    }
}
