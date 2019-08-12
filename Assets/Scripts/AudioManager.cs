using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{

    public AudioClip[] sounds;


    void PlaySound(int blockType)
    {
        GetComponent<AudioSource>().PlayOneShot(sounds[blockType]);
    }

    void OnEnable()
    {
        VoxelChunk.OnEventBlockChanged += PlaySound;
    }

    void OnDisable()
    {
        VoxelChunk.OnEventBlockChanged += PlaySound;
    }
}
