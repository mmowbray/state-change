using UnityEngine;
using System.Collections;

public class OpenDoor : MonoBehaviour
{
    private GameObject player;
    public float openDist;
    private bool open;
    private int direction;
    public float moveSpeed;
    public float openHeight;
    private float closeHeight;
    public bool battleDoor; //when a battleDoor opens the music changes to battle music
    private BackgroundMusic music;

    [SerializeField] private AudioSource doorSound;

    // Use this for initialization
    void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        music = GameObject.FindGameObjectWithTag("MusicPlayer").GetComponent<BackgroundMusic>();
        closeHeight = transform.position.y;

    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= openDist)
        {
            open = true;
            if (battleDoor)
                music.playBattleMusic();
            else
                music.playPuzzleMusic();
            //doorSound.Play();
        }
        else
        {
            open = false;
            //doorSound.Play();
        }

        if (open && transform.position.y <= openHeight)
            transform.Translate(new Vector3(0.0f, 1.0f * moveSpeed * Time.deltaTime, 0.0f));
        else if(!open && transform.position.y >= closeHeight)
            transform.Translate(new Vector3(0.0f, -1.0f * moveSpeed * Time.deltaTime, 0.0f));
    }
}
