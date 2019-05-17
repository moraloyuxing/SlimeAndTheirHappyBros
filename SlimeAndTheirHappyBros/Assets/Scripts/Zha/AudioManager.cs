using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource soundAudio;

    Dictionary<string, AudioClip> soundDictionary = new Dictionary<string, AudioClip>();

    public SoundClip[] soundClips;

    private static AudioManager singletonInScene;
    public static AudioManager SingletonInScene
    {
        get
        {
            return singletonInScene;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        singletonInScene = this;
        singletonInScene.soundAudio = GetComponent<AudioSource>();

        if (soundClips != null) {
            foreach (SoundClip clip in soundClips) {
                soundDictionary.Add(clip.clipNmae, clip.clip);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySound2D(string _name, float volume)
    {
        if (soundDictionary.ContainsKey(_name))
        {
            soundAudio.PlayOneShot(soundDictionary[_name], volume);
        }
        else Debug.Log("沒有這個音檔");
    }

}


[System.Serializable]
public class SoundClip
{
    public string clipNmae;
    public AudioClip clip;
}