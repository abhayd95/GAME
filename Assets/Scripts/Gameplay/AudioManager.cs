using UnityEngine;
using System.Collections.Generic;

namespace FreeFire.Gameplay
{
    public class AudioManager : MonoBehaviour
    {
        [Header("Audio Sources")]
        public AudioSource musicSource;
        public AudioSource sfxSource;
        public AudioSource voiceSource;
        public AudioSource ambientSource;

        [Header("Music")]
        public AudioClip lobbyMusic;
        public AudioClip gameMusic;
        public AudioClip victoryMusic;
        public AudioClip defeatMusic;

        [Header("Sound Effects")]
        public AudioClip[] gunshotSounds;
        public AudioClip[] reloadSounds;
        public AudioClip[] footstepSounds;
        public AudioClip[] jumpSounds;
        public AudioClip[] damageSounds;
        public AudioClip[] deathSounds;
        public AudioClip[] zoneSounds;
        public AudioClip[] lootSounds;

        [Header("Voice Lines")]
        public AudioClip[] killConfirmSounds;
        public AudioClip[] damageTakenSounds;
        public AudioClip[] zoneWarningSounds;
        public AudioClip[] victorySounds;

        [Header("Settings")]
        public float masterVolume = 1f;
        public float musicVolume = 0.7f;
        public float sfxVolume = 1f;
        public float voiceVolume = 0.8f;
        public float ambientVolume = 0.5f;

        // Audio pools for performance
        private Queue<AudioSource> audioSourcePool = new Queue<AudioSource>();
        private List<AudioSource> activeAudioSources = new List<AudioSource>();

        // Singleton instance
        public static AudioManager Instance { get; private set; }

        void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAudioManager();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            LoadAudioSettings();
            PlayLobbyMusic();
        }

        void InitializeAudioManager()
        {
            // Create audio sources if they don't exist
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }

            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
            }

            if (voiceSource == null)
            {
                voiceSource = gameObject.AddComponent<AudioSource>();
                voiceSource.playOnAwake = false;
            }

            if (ambientSource == null)
            {
                ambientSource = gameObject.AddComponent<AudioSource>();
                ambientSource.loop = true;
                ambientSource.playOnAwake = false;
            }

            // Create audio source pool
            CreateAudioSourcePool();
        }

        void CreateAudioSourcePool()
        {
            for (int i = 0; i < 20; i++)
            {
                AudioSource pooledSource = gameObject.AddComponent<AudioSource>();
                pooledSource.playOnAwake = false;
                pooledSource.volume = sfxVolume * masterVolume;
                audioSourcePool.Enqueue(pooledSource);
            }
        }

        AudioSource GetPooledAudioSource()
        {
            if (audioSourcePool.Count > 0)
            {
                AudioSource source = audioSourcePool.Dequeue();
                activeAudioSources.Add(source);
                return source;
            }
            else
            {
                // Create new source if pool is empty
                AudioSource newSource = gameObject.AddComponent<AudioSource>();
                newSource.playOnAwake = false;
                newSource.volume = sfxVolume * masterVolume;
                activeAudioSources.Add(newSource);
                return newSource;
            }
        }

        void ReturnAudioSourceToPool(AudioSource source)
        {
            if (activeAudioSources.Contains(source))
            {
                activeAudioSources.Remove(source);
                source.Stop();
                source.clip = null;
                audioSourcePool.Enqueue(source);
            }
        }

        // Music Methods
        public void PlayLobbyMusic()
        {
            PlayMusic(lobbyMusic);
        }

        public void PlayGameMusic()
        {
            PlayMusic(gameMusic);
        }

        public void PlayVictoryMusic()
        {
            PlayMusic(victoryMusic);
        }

        public void PlayDefeatMusic()
        {
            PlayMusic(defeatMusic);
        }

        void PlayMusic(AudioClip clip)
        {
            if (musicSource != null && clip != null)
            {
                musicSource.clip = clip;
                musicSource.volume = musicVolume * masterVolume;
                musicSource.Play();
            }
        }

        public void StopMusic()
        {
            if (musicSource != null)
            {
                musicSource.Stop();
            }
        }

        public void PauseMusic()
        {
            if (musicSource != null)
            {
                musicSource.Pause();
            }
        }

        public void ResumeMusic()
        {
            if (musicSource != null)
            {
                musicSource.UnPause();
            }
        }

        // Sound Effect Methods
        public void PlayGunshotSound(Vector3 position)
        {
            if (gunshotSounds.Length > 0)
            {
                AudioClip randomClip = gunshotSounds[Random.Range(0, gunshotSounds.Length)];
                PlaySoundAtPosition(randomClip, position, sfxVolume);
            }
        }

        public void PlayReloadSound()
        {
            if (reloadSounds.Length > 0)
            {
                AudioClip randomClip = reloadSounds[Random.Range(0, reloadSounds.Length)];
                PlaySound(randomClip, sfxVolume);
            }
        }

        public void PlayFootstepSound(Vector3 position)
        {
            if (footstepSounds.Length > 0)
            {
                AudioClip randomClip = footstepSounds[Random.Range(0, footstepSounds.Length)];
                PlaySoundAtPosition(randomClip, position, sfxVolume * 0.5f);
            }
        }

        public void PlayJumpSound()
        {
            if (jumpSounds.Length > 0)
            {
                AudioClip randomClip = jumpSounds[Random.Range(0, jumpSounds.Length)];
                PlaySound(randomClip, sfxVolume * 0.7f);
            }
        }

        public void PlayDamageSound()
        {
            if (damageSounds.Length > 0)
            {
                AudioClip randomClip = damageSounds[Random.Range(0, damageSounds.Length)];
                PlaySound(randomClip, sfxVolume);
            }
        }

        public void PlayDeathSound(Vector3 position)
        {
            if (deathSounds.Length > 0)
            {
                AudioClip randomClip = deathSounds[Random.Range(0, deathSounds.Length)];
                PlaySoundAtPosition(randomClip, position, sfxVolume);
            }
        }

        public void PlayZoneSound()
        {
            if (zoneSounds.Length > 0)
            {
                AudioClip randomClip = zoneSounds[Random.Range(0, zoneSounds.Length)];
                PlaySound(randomClip, sfxVolume);
            }
        }

        public void PlayLootSound(Vector3 position)
        {
            if (lootSounds.Length > 0)
            {
                AudioClip randomClip = lootSounds[Random.Range(0, lootSounds.Length)];
                PlaySoundAtPosition(randomClip, position, sfxVolume * 0.8f);
            }
        }

        // Voice Methods
        public void PlayKillConfirmSound()
        {
            if (killConfirmSounds.Length > 0)
            {
                AudioClip randomClip = killConfirmSounds[Random.Range(0, killConfirmSounds.Length)];
                PlayVoice(randomClip);
            }
        }

        public void PlayDamageTakenSound()
        {
            if (damageTakenSounds.Length > 0)
            {
                AudioClip randomClip = damageTakenSounds[Random.Range(0, damageTakenSounds.Length)];
                PlayVoice(randomClip);
            }
        }

        public void PlayZoneWarningSound()
        {
            if (zoneWarningSounds.Length > 0)
            {
                AudioClip randomClip = zoneWarningSounds[Random.Range(0, zoneWarningSounds.Length)];
                PlayVoice(randomClip);
            }
        }

        public void PlayVictorySound()
        {
            if (victorySounds.Length > 0)
            {
                AudioClip randomClip = victorySounds[Random.Range(0, victorySounds.Length)];
                PlayVoice(randomClip);
            }
        }

        // Core Audio Methods
        void PlaySound(AudioClip clip, float volume)
        {
            if (clip != null)
            {
                sfxSource.PlayOneShot(clip, volume * masterVolume);
            }
        }

        void PlaySoundAtPosition(AudioClip clip, Vector3 position, float volume)
        {
            if (clip != null)
            {
                AudioSource.PlayClipAtPoint(clip, position, volume * masterVolume);
            }
        }

        void PlayVoice(AudioClip clip)
        {
            if (voiceSource != null && clip != null)
            {
                voiceSource.PlayOneShot(clip, voiceVolume * masterVolume);
            }
        }

        public void PlaySoundWithPool(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f)
        {
            if (clip != null)
            {
                AudioSource source = GetPooledAudioSource();
                source.clip = clip;
                source.volume = volume * masterVolume;
                source.pitch = pitch;
                source.transform.position = position;
                source.Play();

                // Return to pool when finished
                StartCoroutine(ReturnSourceToPoolWhenFinished(source, clip.length / pitch));
            }
        }

        System.Collections.IEnumerator ReturnSourceToPoolWhenFinished(AudioSource source, float duration)
        {
            yield return new WaitForSeconds(duration);
            ReturnAudioSourceToPool(source);
        }

        // Volume Control Methods
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            UpdateAllVolumes();
            SaveAudioSettings();
        }

        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            if (musicSource != null)
            {
                musicSource.volume = musicVolume * masterVolume;
            }
            SaveAudioSettings();
        }

        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            if (sfxSource != null)
            {
                sfxSource.volume = sfxVolume * masterVolume;
            }
            SaveAudioSettings();
        }

        public void SetVoiceVolume(float volume)
        {
            voiceVolume = Mathf.Clamp01(volume);
            if (voiceSource != null)
            {
                voiceSource.volume = voiceVolume * masterVolume;
            }
            SaveAudioSettings();
        }

        public void SetAmbientVolume(float volume)
        {
            ambientVolume = Mathf.Clamp01(volume);
            if (ambientSource != null)
            {
                ambientSource.volume = ambientVolume * masterVolume;
            }
            SaveAudioSettings();
        }

        void UpdateAllVolumes()
        {
            if (musicSource != null)
                musicSource.volume = musicVolume * masterVolume;
            if (sfxSource != null)
                sfxSource.volume = sfxVolume * masterVolume;
            if (voiceSource != null)
                voiceSource.volume = voiceVolume * masterVolume;
            if (ambientSource != null)
                ambientSource.volume = ambientVolume * masterVolume;

            // Update pooled sources
            foreach (AudioSource source in activeAudioSources)
            {
                source.volume = sfxVolume * masterVolume;
            }
        }

        // Settings Management
        void SaveAudioSettings()
        {
            PlayerPrefs.SetFloat("MasterVolume", masterVolume);
            PlayerPrefs.SetFloat("MusicVolume", musicVolume);
            PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
            PlayerPrefs.SetFloat("VoiceVolume", voiceVolume);
            PlayerPrefs.SetFloat("AmbientVolume", ambientVolume);
            PlayerPrefs.Save();
        }

        void LoadAudioSettings()
        {
            masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            voiceVolume = PlayerPrefs.GetFloat("VoiceVolume", 0.8f);
            ambientVolume = PlayerPrefs.GetFloat("AmbientVolume", 0.5f);

            UpdateAllVolumes();
        }

        // Public getters
        public float GetMasterVolume() => masterVolume;
        public float GetMusicVolume() => musicVolume;
        public float GetSFXVolume() => sfxVolume;
        public float GetVoiceVolume() => voiceVolume;
        public float GetAmbientVolume() => ambientVolume;

        // Utility methods
        public void MuteAll()
        {
            SetMasterVolume(0f);
        }

        public void UnmuteAll()
        {
            SetMasterVolume(1f);
        }

        public void FadeOutMusic(float duration)
        {
            StartCoroutine(FadeMusic(0f, duration));
        }

        public void FadeInMusic(float duration)
        {
            StartCoroutine(FadeMusic(musicVolume, duration));
        }

        System.Collections.IEnumerator FadeMusic(float targetVolume, float duration)
        {
            if (musicSource == null) yield break;

            float startVolume = musicSource.volume;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float currentVolume = Mathf.Lerp(startVolume, targetVolume * masterVolume, elapsedTime / duration);
                musicSource.volume = currentVolume;
                yield return null;
            }

            musicSource.volume = targetVolume * masterVolume;
        }
    }
}
