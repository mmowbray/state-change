using UnityEngine;
using System.Collections;

public class BackgroundMusic : MonoBehaviour
{
    public AudioSource puzzleMusic;
    public AudioSource battleMusic;
	// Use this for initialization
	void Start ()
    {
        puzzleMusic.Play();
	}
	
    public void playBattleMusic()
    {
        if (puzzleMusic.isPlaying)
        {
            puzzleMusic.Stop();
            battleMusic.Play();
        }
    }

    public void playPuzzleMusic()
    {
        if (battleMusic.isPlaying)
        {
            puzzleMusic.Play();
            battleMusic.Stop();
        }
    }
}
