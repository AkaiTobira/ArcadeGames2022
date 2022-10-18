using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioTrack{
    public AudioTrack(){}
    public AudioTrack(string name, AudioClip obj){
        _name = name;
        _clip = obj;
    }
    public string _name;
    public AudioClip _clip;
    [HideInInspector] public float Volume;
    [HideInInspector] public float TargetVolume;
    [HideInInspector] public float Pitch;
}

public class AudioSystem : MonoBehaviour
{
    public static AudioSystem Instance = null;

    [Header("Oneshots")]
    [SerializeField] private List<AudioSource> _effectsPLayers = new List<AudioSource>();
    [NonReorderable][SerializeField] private List<AudioTrack> _clips;
    public float LowPitchRange = 0.95f;
	public float HighPitchRange = 1.05f;

    [Header("BG Music")]
    [NonReorderable][SerializeField] private List<AudioTrack> _musics;
	[SerializeField] private AudioSource MusicSource;


	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
	    	DontDestroyOnLoad(gameObject);
    	}
		else if (Instance != this)
		{
			Destroy(gameObject);
		}
	}

    private List<AudioTrack> _BGMusicToPlay = new List<AudioTrack>();

	public void PlayEffect(string clipName, float volume, bool randomPitch=false)
	{
        if( string.IsNullOrEmpty(clipName) ) return;

        bool clipSelected = false;
        foreach( AudioTrack at in _clips){

            if( at._name == clipName){
                clipSelected = true;

                for( int i = 0; i < _effectsPLayers.Count; i++){
                    if( _effectsPLayers[i].isPlaying ) continue;
                    
                    if( randomPitch ){
                        float pitch = Random.Range(LowPitchRange, HighPitchRange);
		                _effectsPLayers[i].pitch = pitch;
                    }
                	_effectsPLayers[i].clip   = at._clip;
                    _effectsPLayers[i].volume = volume;
                    _effectsPLayers[i].Play();
                    return;
                }
            }
        }
        if( !clipSelected ){
            Debug.LogError("Didn't found Oneshot Sound=" + clipName );
        }
	}

	public void PlayMusic(string clipName, float volume)
	{
        if( string.IsNullOrEmpty(clipName) ) return;

        bool clipSelected = false;
        foreach( AudioTrack at in _musics){
            if( at._name == clipName){
                clipSelected = true;

                if( MusicSource.isPlaying ){
                    MusicSource.clip   = at._clip;
                    at.TargetVolume = volume;
                    _BGMusicToPlay.Add( at );
                }else{
                    MusicSource.clip   = at._clip;
                    MusicSource.volume = volume;
                    MusicSource.Play();
                }
            }
        }
        if( !clipSelected ){
            Debug.LogError("Didn't found BGMusic=" + clipName );
        }
	}


    void Update() {
        if( _BGMusicToPlay.Count != 0 && MusicSource.isPlaying){
            if( _BGMusicToPlay[0]._clip == MusicSource.clip ){
                float musicChange = Mathf.Sign(_BGMusicToPlay[0].TargetVolume - MusicSource.volume) * 2f * Time.deltaTime;

                MusicSource.volume = (musicChange < 0) ?
                    Mathf.Max( _BGMusicToPlay[0].TargetVolume, MusicSource.volume + musicChange ) :
                    Mathf.Min( _BGMusicToPlay[0].TargetVolume, MusicSource.volume + musicChange );

                if( Mathf.Abs( _BGMusicToPlay[0].TargetVolume - MusicSource.volume ) <= 0.001f ){
                    if(_BGMusicToPlay[0].TargetVolume <= 0.01f) MusicSource.Stop();
                    _BGMusicToPlay.RemoveAt(0);
                }
            }else{
                if( MusicSource.volume > 0){
                    MusicSource.volume = Mathf.Max( 0, MusicSource.volume - (2f* Time.deltaTime) );
                }else{
                    MusicSource.clip   = _BGMusicToPlay[0]._clip;
                    MusicSource.volume = 0;
                    MusicSource.Play();
                }
            }
        
        }
    }
}