using System;
using SFML;
using SFML.Audio;
using System.Collections.Generic;
using System.Threading;

namespace Punk 
{
	/// <summary>
	/// A wrapper for SFML.Sound, which provides OnComplete callback
	/// functionality, and a few other helpful features.
	/// </summary>
    public class Sfx : Sound
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
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="source">The SFML Sound object.</param>
		/// <param name="complete">Optional callback function for when the sound finishes playing.</param>
        public Sfx(Sound source, OnComplete complete = null) : base(source) 
        {
            this.Complete = complete;

            ActiveSounds.Add(this);
        }
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="source">The SFML SoundBuffer object. Can be retrieved with Library.GetBuffer().</param>
		/// <param name="complete">Optional callback function for when the sound finishes playing.</param>
        public Sfx(SoundBuffer source, OnComplete complete = null)
            : this(new Sound(source), complete) { }
        
		protected override void Destroy(bool disposing)
		{
			base.Destroy(disposing);
			ActiveSounds.Remove(this);
		}


        /// <summary>
        /// If the sound is paused.
        /// </summary>
        public bool IsPaused
        {
            get { return Status == SoundStatus.Paused; }
        }

        /// <summary>
        /// If the sound is playing.
        /// </summary>
        public bool IsPlaying
        {
            get { return Status == SoundStatus.Playing; }
        }

        /// <summary>
        /// If the sound is stopped (not playing and not paused).
        /// </summary>
        public bool IsStopped
        {
            get { return Status == SoundStatus.Stopped; }
        }
        
        /// <summary>
        /// Plays the sound and loops it infinitely when it finshes.
        /// </summary>
        public new void Loop()
        {
        	base.Loop = true;
        	_loop = true;
        	Play();
        }
        
        /// <summary>
        /// Stops the playback of the sound.
        /// </summary>
        public new void Stop()
        {
        	base.Loop = _loop ? false : base.Loop;
        	_loop = false;
        	
        	base.Stop();
        }

        internal static void UpdateSounds()
        {
            foreach (Sfx sfx in ActiveSounds)
            {
                if (sfx.PlayingOffset.Milliseconds >= sfx.SoundBuffer.Duration)
                {
                    if (sfx.Complete != null)
                    {
                        sfx.Complete();
                    }
                }
            }
        }

        #region Stored Sound objects.
        static List<Sfx> ActiveSounds = new List<Sfx>();
        #endregion
    }
}
