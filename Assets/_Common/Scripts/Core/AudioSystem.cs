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
        [SerializeField] private List<AudioSource> _effectsPlayers = new List<AudioSource>();
        [NonReorderable][SerializeField] private List<AudioTrack> _clips;
        public float LowPitchRange = 0.95f;
        public float HighPitchRange = 1.05f;

        [Header("BG Music")]
        [NonReorderable][SerializeField] private List<AudioTrack> _musics;
        [SerializeField] private AudioSource MusicSource;
        //Don't play if MAX is set
        [SerializeField] private string PlayOnAwake;

        private string[] _audioBackgroundClipNames;
        private float _audioBackgroundVolume;
        private AudioTrack _nextToPlay;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                MusicSource.loop = false;
                if(PlayOnAwake != "") PlayMusic(PlayOnAwake, 1);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        #region ExternalInterface
            public static void PlaySample(string clipName, float volume = 1.0f, bool randomPitch = false){
                if(Guard.IsValid(Instance)){
                    Instance.PlayEffect(clipName, volume, randomPitch);
                }
            }
            public static void PlayBackground(string clipName, float volume){
                if(Guard.IsValid(Instance)){
                    Instance.PlayMusic(clipName, volume);
                }
            }

            public static void PlayBackgrounds(string[] clipName, float volume){
                if(Guard.IsValid(Instance)){
                    Instance.PlayMusic(clipName, volume);
                }
            }

            public static void ChangeBackgroundMusicVolume(float volume){
                if(Guard.IsValid(Instance)){
                    Instance._audioBackgroundVolume = volume;
                }
            }

            public static float GetSoundDuration(string clipName){
                if(Guard.IsValid(Instance)) return Instance.GetSoundDurationInternal(clipName);
                return 0;
            }

        #endregion

        private float GetSoundDurationInternal(string clipName){
            List<AudioTrack> sounds = new List<AudioTrack>();
            sounds.AddRange(_musics);
            sounds.AddRange(_clips);
            for(int i = 0; i < sounds.Count; i++){
                AudioTrack track = sounds[i];
                if(track._name == clipName && track._clip != null){
                    return track._clip.length;
                }
            }

            return 0;
        }


        public void PlayEffect(string clipName, float volume = 1.0f, bool randomPitch=false)
        {
            bool clipSelected = false;
            foreach( AudioTrack at in _clips){
//                Debug.Log(at._callName + " " + clipName);
                if( at._name == clipName){
                    clipSelected = true;

                    for( int i = 0; i < _effectsPlayers.Count; i++){
                        if( _effectsPlayers[i].isPlaying ) continue;
                        
                        if( randomPitch ){
                            float pitch = UnityEngine.Random.Range(LowPitchRange, HighPitchRange);
                            _effectsPlayers[i].pitch = pitch;
                        }
                        _effectsPlayers[i].clip   = at._clip;
                        _effectsPlayers[i].volume = volume;
                        _effectsPlayers[i].Play();
                        return;
                    }

                //    Debug.Log(at._callName + " " + clipName + " found but not played");
                }
            }

            if( !clipSelected ){
                UnityEngine.Debug.LogError("Didn't found Oneshot Sound=" + clipName );
            }
        }

        public void PlayMusic(string[] clipNames, float volume){
            _audioBackgroundClipNames = clipNames;
            _audioBackgroundVolume    = volume;
            PlayMusicInternal();
        }

        private void PlayMusicInternal(){
            int randomSelectedBackgroundClip = UnityEngine.Random.Range(0, _audioBackgroundClipNames.Length);
            string nextToPlay = _audioBackgroundClipNames[randomSelectedBackgroundClip];

            for(int i = 0; i < _musics.Count; i++){
                AudioTrack track = _musics[i];
                if(nextToPlay == track._name){
                    _nextToPlay = track;
                }
            }
        }

        public void PlayMusic(string clipName, float volume)
        {
//            Debug.Log("PlayMusicCalled");
            _audioBackgroundClipNames = new string[] {clipName};
            _audioBackgroundVolume    = volume;
            PlayMusicInternal();
        }

        private void AdjustTheVolume(float targetVolume){
            float volumeDifference = targetVolume - MusicSource.volume;
            float musicChange = Mathf.Sign(targetVolume - MusicSource.volume) * 2f * Time.deltaTime;
            if(Mathf.Abs(volumeDifference) <= Mathf.Abs(musicChange)){
                MusicSource.volume = targetVolume;
                return;
            }

            MusicSource.volume += musicChange;
        }


        void Update() {
            //Debug.Log(MusicSource.isPlaying);
            if(_nextToPlay == null) return;
            if(!MusicSource.isPlaying){
                if(MusicSource.clip == _nextToPlay._clip) PlayMusicInternal();

                MusicSource.clip = _nextToPlay._clip;
                MusicSource.Play();
            }else{
                if(MusicSource.clip == _nextToPlay._clip){
                    if(MusicSource.volume != _audioBackgroundVolume) AdjustTheVolume(_audioBackgroundVolume);
                }else{
                    if(MusicSource.volume != 0) AdjustTheVolume(0);
                    else{
                        MusicSource.clip = _nextToPlay._clip;
                        MusicSource.Play();
                    }
                }
            }
        }
    }
