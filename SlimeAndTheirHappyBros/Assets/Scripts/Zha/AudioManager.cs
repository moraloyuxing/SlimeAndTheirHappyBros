using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource effectAudio, bgmAudio;

    Dictionary<string, AudioClip> soundDictionary = new Dictionary<string, AudioClip>();

    AudioClip nextMusic;

    public AudioClip shoppingMusic;
    public AudioClip[] battleMusic;
    public SoundClip[] soundClips;
    public AudioClip bossMusic;
    public AudioClip[] shootClips;
    public AudioClip[] correctClips;
    public AudioClip[] wrongClips;

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
        singletonInScene.effectAudio = transform.Find("EffectAudio").GetComponent<AudioSource>();
        singletonInScene.bgmAudio = transform.Find("BGMAudio").GetComponent<AudioSource>();

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

    public void PauseBGM() {
        bgmAudio.Pause();
    }

    public void ChangeBGM(bool shopping, int curRound) {
        
        if (shopping) {
            bgmAudio.Pause();
            bgmAudio.clip = shoppingMusic;
            StartCoroutine(OnChangingBGM());
        }
        else {
            if (curRound < 0) bgmAudio.clip = bossMusic;  // boss 定curRound為-1
            else if (curRound < 3) bgmAudio.clip = battleMusic[0];
            else if (curRound < 5) bgmAudio.clip = battleMusic[1];
            else bgmAudio.clip = battleMusic[2];
            bgmAudio.Play();
        }
        
    }

    IEnumerator OnChangingBGM() {
        yield return new WaitForSeconds(1.5f);
        bgmAudio.Play();
    }

    public void PlaySound2D(string _name, float volume)
    {
        if (soundDictionary.ContainsKey(_name))
        {
            effectAudio.PlayOneShot(soundDictionary[_name], volume);
        }
        else Debug.Log("沒有這個音檔");
    }
    public void PlaySound2D(string _name, float volume, float pitch)
    {
        if (soundDictionary.ContainsKey(_name))
        {
            effectAudio.pitch = pitch;
            effectAudio.PlayOneShot(soundDictionary[_name], volume);
        }
        else Debug.Log("沒有這個音檔");
    }

    public void PlayRandomShoot(float volume) {
        int r = Random.Range(0, 100) % shootClips.Length;
        effectAudio.PlayOneShot(shootClips[r], volume);
    }
    public void PlayRandomCorrect(float volume)
    {
        int r = Random.Range(0,100) % correctClips.Length;
        effectAudio.PlayOneShot(correctClips[r], volume);
    }
    public void PlayRandomWrong(float volume)
    {
        int r = Random.Range(0, 100) % wrongClips.Length;
        effectAudio.PlayOneShot(wrongClips[r], volume);
    }

    IEnumerator ReturnPitch() {

        yield return null;
        effectAudio.pitch = 1.0f;

    }
}


[System.Serializable]
public class SoundClip
{
    public string clipNmae;
    public AudioClip clip;
}