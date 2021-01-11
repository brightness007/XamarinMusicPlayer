using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;

using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using LottoMore.Droid.Implementations;
using LottoMore.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(AudioPlayerService))]

namespace LottoMore.Droid.Implementations
{
    class AudioPlayerService : IAudioPlayerService
    {

        private MediaPlayer _mediaPlayer;

        public AudioPlayerService()
        {

        }
        public Action OnFinishPlaying { get; set; }

        public void Pause()
        {
            if (_mediaPlayer.IsPlaying)
            {
                _mediaPlayer.Pause();
            }
            else
            {
                _mediaPlayer.Start();
            }
            //throw new NotImplementedException();
        }

        public void Play(string pathToAudioFIle)
        {
            // Check if _audioPlayer is currently playing  
            if (_mediaPlayer != null)
            {
                if (_mediaPlayer.IsPlaying)
                {
                    _mediaPlayer.Completion -= MediaPlayer_Completion;
                    _mediaPlayer.Stop();
                }
                else
                {
                    _mediaPlayer.Start();
                    return;
                }
            }

            var fullPath = pathToAudioFIle;

            Android.Content.Res.AssetFileDescriptor afd = null;

#if true // true - play embedded audio, false - play audio from network
            try
            {
                afd = MainActivity.CONTEXT.Assets.OpenFd(fullPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error openfd: " + ex);
            }
#endif
            //if (afd != null)
            {
                //System.Diagnostics.Debug.WriteLine("Length " + afd.Length);
                if (_mediaPlayer == null)
                {
                    _mediaPlayer = new MediaPlayer();
                    _mediaPlayer.Prepared += (sender, args) =>
                    {
                        _mediaPlayer.Start();
                        _mediaPlayer.Completion += MediaPlayer_Completion;
                    };
                }

                _mediaPlayer.Reset();
                _mediaPlayer.SetVolume(1.0f, 1.0f);

                if (afd != null)
                    _mediaPlayer.SetDataSource(afd.FileDescriptor, afd.StartOffset, afd.Length);
                else
                    _mediaPlayer.SetDataSource("http://192.168.1.26:3000/Despacito%20(Remix).flac");
                _mediaPlayer.Prepare();
                _mediaPlayer.Start();
            }
        }

        private void MediaPlayer_Completion(object sender, EventArgs e)
        {
            OnFinishPlaying?.Invoke();
        }

        public void Play()
        {
            OnFinishPlaying?.Invoke();
        }
    }
}