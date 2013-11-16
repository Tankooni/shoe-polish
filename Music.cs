using System;
using SFML;
using SFML.Audio;
using System.Collections.Generic;
using System.Threading;

namespace Punk
{
	/// <summary>
	/// A wrapper for SFML.Music, which provides OnComplete callback
	/// functionality, and a few other helpful features.
	/// </summary>
    public class Music : SFML.Audio.Music
    {
        /// <summary>
        /// Delegate for optional callback function for when the sound finishes playing.
        /// </summary>
        public delegate void OnComplete();
        
        /// <summary>
        /// Optional callback function for when the sound finishes playing.
        /// </summary>
        public OnComplete Complete;
        
        private bool _loop = false;

        internal Music(string filename) : base(filename)
        {
            ActiveJams.Add(this);
        }

        /// <summary>
        /// If the music is paused.
        /// </summary>
        public bool IsPaused
        {
            get { return Status == SoundStatus.Paused; }
        }

        /// <summary>
        /// If the music is playing.
        /// </summary>
        public bool IsPlaying
        {
            get { return Status == SoundStatus.Playing; }
        }

        /// <summary>
        /// If the music is stopped (not playing, not paused).
        /// </summary>
        public bool IsStopped
        {
            get { return Status == SoundStatus.Stopped; }
        }
        
        /// <summary>
        /// Plays the music and loops it infinitely when it finshes.
        /// </summary>
        public new void Loop()
        {
        	base.Loop = true;
        	_loop = true;
        	Play();
        }
        
        /// <summary>
        /// Stops the playback of the music.
        /// </summary>
        public new void Stop()
        {
        	base.Loop = _loop ? false : base.Loop;
        	_loop = false;
        	
        	base.Stop();
        }

        internal static void UpdateJams()
        {
            foreach (Punk.Music jam in ActiveJams)
            {
                if (jam.PlayingOffset >= jam.Duration)
                {
                    if (jam.Complete != null)
                    {
                        jam.Complete();
                    }
                }
            }
        }

        #region Stored Punk.Music objects.
        static List<Punk.Music> ActiveJams = new List<Punk.Music>();
        #endregion
    }
}
