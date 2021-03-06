using System;
using UnityEngine;

namespace Utage
{

	/// <summary>
	/// サウンド再生ストリーム
	/// 基本はシステム内部で使うので外から呼ばないこと
	/// </summary>
	[AddComponentMenu("Utage/Lib/Sound/SoundStream")]
	internal class SoundStream : MonoBehaviour
	{
		/// <summary>
		/// 状態
		/// </summary>
		enum SoundStreamStatus
		{
			None,
			Ready,
			Play,
			FadeIn,
			FadeOut,
		};
		SoundStreamStatus status = SoundStreamStatus.None;
		SoundStreamStatus Status { get { return status; } }

		public AudioSource AudioSource { get { return this.audioSource; } }
		AudioSource audioSource;
		AudioClip clip;

		//再生時に指定されたボリューム
		public float RequestVolume { get { return requestVolume; } }
		float requestVolume = 0;


		/// <summary>
		/// ループするかどうか
		/// </summary>
		public bool IsLoop { get { return isLoop; } }
		bool isLoop;

		/// <summary>
		/// ストリーム再生か
		/// </summary>
		public bool IsStreaming { get { return isStreaming; } }
		bool isStreaming;

		float fadeInTime;
		LinearValue fadeInValue = new LinearValue();
		LinearValue fadeOutValue = new LinearValue();
		Action callbackEnd;
		//マスターボリュームを取得するコールバック
		Func<float> callbackMasterVolume;

		/// <summary>
		/// ファイル名
		/// </summary>
		public string FileName
		{
			get
			{
				AssetFileReference file = this.GetComponent<AssetFileReference>();
				if (null != file)
				{
					return file.File.FileName;
				}
				else
				{
					if( AssetFileManager.ContainsStaticAsset(this.clip) )
					{
						return this.clip.name;
					}
				}
				return "";
			}
		}

		void Awake()
		{
			this.audioSource = this.gameObject.AddComponent<AudioSource>();
		}

		/// <summary>
		/// 再生するための準備
		/// </summary>
		/// <param name="clip">オーディクリップ</param>
		/// <param name="masterVolume">マスターボリューム</param>
		/// <param name="volume">再生ボリューム</param>
		/// <param name="isLoop">ループ再生するか</param>
		/// <param name="isStreaming">ストリーム再生するか</param>
		/// <param name="callBackEnd">再生終了時に呼ばれるコールバック</param>
		public void Ready(AudioClip clip, float fadeInTime, float volume, bool isLoop, bool isStreaming, Func<float> callbackMasterVolume, Action callbackEnd)
		{
			this.clip = clip;
			this.fadeInTime = fadeInTime;
			this.requestVolume = volume;
			this.isLoop = isLoop;
			this.isStreaming = isStreaming;
			this.callbackMasterVolume = callbackMasterVolume;
			this.callbackEnd = callbackEnd;
			status = SoundStreamStatus.Ready;
		}

		/// <summary>
		/// 再生準備中か
		/// </summary>
		/// <returns></returns>
		public bool IsReady()
		{
			return (SoundStreamStatus.Ready == status);
		}

		/// <summary>
		/// 再生
		/// </summary>
		/// <param name="clip">オーディクリップ</param>
		/// <param name="masterVolume">マスターボリューム</param>
		/// <param name="volume">再生ボリューム</param>
		/// <param name="isLoop">ループ再生するか</param>
		/// <param name="callBackEnd">再生終了時に呼ばれるコールバック</param>
		public void Play(AudioClip clip, float fadeInTime, float volume, bool isLoop, bool isStreaming, Func<float> callbackMasterVolume, Action callbackEnd)
		{
			Ready(clip, fadeInTime, volume, isLoop, isStreaming, callbackMasterVolume, callbackEnd);
			Play();
		}

		/// <summary>
		/// 再生
		/// </summary>
		public void Play()
		{
/*			if (!WrapperUnityVersion.IsReadyPlayAudioClip(clip))
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.SoundNotReadyToPlay,clip.name));
			}*/
			audioSource.clip = clip;
			audioSource.loop = IsLoop;
			audioSource.volume = requestVolume * callbackMasterVolume();
			audioSource.Play();
			status = SoundStreamStatus.Play;

			if (fadeInTime > 0)
			{
				float volume = 0;
				if (status == SoundStreamStatus.FadeOut) {
					volume = fadeOutValue.GetValue ();
				}
				status = SoundStreamStatus.FadeIn;
				fadeInValue.Init (fadeInTime, volume, 1);
			}
		}

		//終了
		public void End()
		{
			audioSource.Stop();
			if (null != callbackEnd) callbackEnd();
			GameObject.Destroy(this.gameObject);
		}

		//指定のサウンドかどうか
		public bool IsEqualClip(AudioClip clip)
		{
			return (audioSource.clip == clip);
		}

		//指定のサウンドが鳴っているか
		public bool IsPlaying(AudioClip clip)
		{
			if (IsEqualClip(clip))
			{
				return (status == SoundStreamStatus.Play);
			}
			return false;
		}
		//サウンドが鳴っているか
		public bool IsPlaying()
		{
			switch (status)
			{
				case SoundStreamStatus.FadeIn:
				case SoundStreamStatus.Play:
					return true;
				default:
					return false;
			}
		}
/*
		//指定時間フェードイン
		internal void FadeIn(float fadeTime)
		{
			Play();
			float volume = 0;
			if (status == SoundStreamStatus.FadeOut)
			{
				volume = fadeOutValue.GetValue();
			}
			status = SoundStreamStatus.FadeIn;
			fadeInValue.Init(fadeTime, 1, volume);
		}
*/

		//指定時間フェードアウトして終了
		public void FadeOut(float fadeTime)
		{
			CancelInvoke();
			if (fadeTime > 0 && !IsEnd())
			{
				float volume = 1;
				if (status == SoundStreamStatus.FadeIn)
				{
					volume = fadeOutValue.GetValue();
				}
				status = SoundStreamStatus.FadeOut;
				fadeOutValue.Init(fadeTime, volume, 0);
			}
			else
			{
				End();
			}
		}

		//曲が終わっているか
		public bool IsEnd()
		{
			return (SoundStreamStatus.None == status);
		}

		public void Update()
		{
			switch (status)
			{
				case SoundStreamStatus.Play:
					UpdatePlay();
					break;
				case SoundStreamStatus.FadeIn:
					UpdateFadeIn();
					break;
				case SoundStreamStatus.FadeOut:
					UpdateFadeOut();
					break;
				default:
					break;
			}
		}

		//通常再生
		void UpdatePlay()
		{
			//再生終了
			if (!audioSource.isPlaying)
			{
				//ループしないなら終了
				if (!isLoop)
				{
					End();
					return;
				}
				else
				{
					audioSource.Play();
				}
			}
			audioSource.volume = requestVolume * callbackMasterVolume();
		}

		//フェードイン処理
		void UpdateFadeIn()
		{
			fadeInValue.IncTime();
			audioSource.volume = fadeInValue.GetValue() * requestVolume * callbackMasterVolume();
			if (fadeInValue.IsEnd())
			{
				status = SoundStreamStatus.Play;
			}
		}

		//フェードアウト処理
		void UpdateFadeOut()
		{
			fadeOutValue.IncTime();
			audioSource.volume = fadeOutValue.GetValue() * requestVolume * callbackMasterVolume();
			if (fadeOutValue.IsEnd())
			{
				End();
			}
		}

		//現在鳴っているボリュームを取得
		public float GetCurrentSamplesVolume()
		{
			if (audioSource.isPlaying)
			{
				return GetSamplesVolume(audioSource);
			}
			else
			{
				return 0;
			}
		}

		// オーディオのボリュームを取得
		const int smaples = 256;
		const int chanel = 0;
		float[] waveData = new float[smaples];
		float GetSamplesVolume(AudioSource audio)
		{
			audio.GetOutputData(waveData, chanel);
			float sum = 0;
			foreach (float s in waveData)
			{
				sum += Mathf.Abs(s);
			}
			return (sum / smaples);
		}
	};
}
