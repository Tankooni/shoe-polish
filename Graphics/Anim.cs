/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 5/18/2013
 * Time: 11:35 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Punk.Graphics
{
	/// <summary>
	/// Template used by Spritemap to define animations. Don't create
	/// these yourself, instead you can fetch them with Spritemap's Add().
	/// </summary>
	public class Anim
	{
		internal Anim(string name, int[] frames, float frameRate = 0, bool loop = true)
		{
			Name = name;
			Frames = frames;
			FrameRate = frameRate;
			Loop = loop;
			FrameCount = frames.Length;
			Parent = null;
		}
		
		/// <summary>
		/// Plays the animation.
		/// </summary>
		/// <param name="reset">If the animation should force-restart if it is already playing.</param>
		public void Play(bool reset = false)
		{
			Parent.Play(Name, reset);
		}
		
		/// <summary>
		/// The name of the animation
		/// </summary>
		public string Name { get; internal set; }
		
		/// <summary>
		/// Array of frame indices to animate.
		/// </summary>
		public int[] Frames { get; internal set; }
		
		/// <summary>
		/// Animation speed.
		/// </summary>
		public float FrameRate { get; internal set; }
		
		/// <summary>
		/// If the animation loops.
		/// </summary>
		public bool Loop { get; internal set; }
		
		/// <summary>
		/// Amount of frames in the animation.
		/// </summary>
		public int FrameCount { get; internal set; }
		
		internal Spritemap Parent;
	}
}
