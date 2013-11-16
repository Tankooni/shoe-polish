/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 6/6/2013
 * Time: 5:14 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Xml;
using Punk.Graphics.Spine;
using SFML.Graphics;
using SFML.Window;
using Spine;

namespace Punk.Graphics
{
	/// <summary>
	/// Skeletal animation. Displays animations created with Spine by Esoteric Software.
	/// </summary>
	public class SpineAnimation : Graphic
	{
		/// <summary>
		/// Callback type for when animations complete.
		/// </summary>
		public delegate void OnComplete();
		
		static SpineAnimation()
		{
			Bone.yDown = true;
		}
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="animationFolder">The folder the animation, skeleton, and spritesheet data are found in.</param>
		/// <param name="skin">An optional skin to use in the animation, if supported.</param>
		public SpineAnimation(string animationFolder, string skin = null)
		{
			animationFolder.TrimEnd('/');
			var atlas = new Atlas(new StringReader(Library.GetText(string.Format("{0}/skeleton.atlas", animationFolder))), ".", new PunkTextureLoader(animationFolder));
			var json = new SkeletonJson(atlas);
			var skeletonData = json.ReadSkeletonData(string.Format("{0}/skeleton.json", animationFolder));
			
			var stateData = new AnimationStateData(skeletonData);
			
			Rate = 1;
			Active = true;
			
			_vertexArray = new VertexArray(PrimitiveType.Quads, (uint) skeletonData.Bones.Count * 4);
			
			_skeleton = new Skeleton(skeletonData);
			_state = new AnimationState(stateData);
			
			if (!string.IsNullOrEmpty(skin))
			{
				_skeleton.SetSkin(skin);
			}
			
			_skeleton.SetSlotsToSetupPose();
			_skeleton.SetToSetupPose();
			_skeleton.UpdateWorldTransform();
			
			_vertexPositions = new float[8];	//	cache this for performance
			_transform = new Transformable();
		}
		
		/// <summary>
		/// Update the animation.
		/// </summary>
		public override void Update()
		{
			base.Update();
			_skeleton.Update(FP.Elapsed);
			_state.Update(FP.Elapsed * Rate);
			_state.Apply(_skeleton);
			_skeleton.UpdateWorldTransform();
			
			if (_state.Animation != null)
			{
				if ((_time >= 0) && (_time += FP.Elapsed) >= _state.Animation.Duration)
				{
					if (Callback != null)
					{
						Callback();
					}
					
					_time = _state.Loop ? 0 : -1;
				}
			}
		}
		
		/// <summary>
		/// Renders the graphic to the screen buffer.
		/// </summary>
		public override void Render(float x, float y, Camera camera)
		{
			_vertexArray.Clear();
				
			Texture texture = null;
			
			_transform.Scale = new Vector2f(ScaleX * Scale, ScaleY * Scale);
			_transform.Rotation = -Angle;	//	TODO:	is reversing angles ideal?
			_transform.Position = new Vector2f(X + x - (camera.X - FP.HalfWidth) * ScrollX, Y + y - (camera.Y - FP.HalfHeight) * ScrollY);
			_transform.Origin = new Vector2f(OriginX, OriginY);
			
			foreach (Slot slot in _skeleton.Slots)
			{
				Attachment attachment = slot.Attachment;
				RegionAttachment regionAttachment = attachment as RegionAttachment;
				if (regionAttachment == null)
				{
					continue;
				}
				
				regionAttachment.ComputeVertices(slot.Skeleton.X, slot.Skeleton.Y, slot.Bone, _vertexPositions);
				
				float R = _skeleton.R * slot.R * 255;
				float G = _skeleton.G * slot.G * 255;
				float B = _skeleton.B * slot.B * 255;
				float A = _skeleton.A * slot.A * 255;
				
				Vertex[] vertices = new Vertex[]
				{
					new Vertex(),
					new Vertex(),
					new Vertex(),
					new Vertex()
				};
				
				unchecked	//	screw you C#
				{
					vertices[0].Color.R = (byte) R;
					vertices[0].Color.G = (byte) G;
					vertices[0].Color.B = (byte) B;
					vertices[0].Color.A = (byte) A;
					
					vertices[1].Color.R = (byte) R;
					vertices[1].Color.G = (byte) G;
					vertices[1].Color.B = (byte) B;
					vertices[1].Color.A = (byte) A;
					
					vertices[2].Color.R = (byte) R;
					vertices[2].Color.G = (byte) G;
					vertices[2].Color.B = (byte) B;
					vertices[2].Color.A = (byte) A;
					
					vertices[3].Color.R = (byte) R;
					vertices[3].Color.G = (byte) G;
					vertices[3].Color.B = (byte) B;
					vertices[3].Color.A = (byte) A;
				}
				
				vertices[0].Position.X = _vertexPositions[RegionAttachment.X1];
				vertices[0].Position.Y = _vertexPositions[RegionAttachment.Y1];
				vertices[1].Position.X = _vertexPositions[RegionAttachment.X2];
				vertices[1].Position.Y = _vertexPositions[RegionAttachment.Y2];
				vertices[2].Position.X = _vertexPositions[RegionAttachment.X3];
				vertices[2].Position.Y = _vertexPositions[RegionAttachment.Y3];
				vertices[3].Position.X = _vertexPositions[RegionAttachment.X4];
				vertices[3].Position.Y = _vertexPositions[RegionAttachment.Y4];
				
				// SMFL doesn't handle batching for us, so we'll just force a single texture per skeleton.
				texture = (Texture) regionAttachment.RendererObject;
				
				Vector2u size = texture.Size;
				vertices[0].TexCoords.X = regionAttachment.UVs[RegionAttachment.X1] * size.X;
				vertices[0].TexCoords.Y = regionAttachment.UVs[RegionAttachment.Y1] * size.Y;
				vertices[1].TexCoords.X = regionAttachment.UVs[RegionAttachment.X2] * size.X;
				vertices[1].TexCoords.Y = regionAttachment.UVs[RegionAttachment.Y2] * size.Y;
				vertices[2].TexCoords.X = regionAttachment.UVs[RegionAttachment.X3] * size.X;
				vertices[2].TexCoords.Y = regionAttachment.UVs[RegionAttachment.Y3] * size.Y;
				vertices[3].TexCoords.X = regionAttachment.UVs[RegionAttachment.X4] * size.X;
				vertices[3].TexCoords.Y = regionAttachment.UVs[RegionAttachment.Y4] * size.Y;
				
				_vertexArray.Append(vertices[0]);
				_vertexArray.Append(vertices[1]);
				_vertexArray.Append(vertices[2]);
				_vertexArray.Append(vertices[3]);
			}
			
			var states = new RenderStates();
			states.Texture = texture;
			states.Transform = _transform.Transform;
			
			FP.Screen.Draw(_vertexArray, states);
		}
		
		/// <summary>
		/// Plays an animation.
		/// </summary>
		/// <param name="name">Name of the animation to play.</param>
		/// <param name="loop">If the animation should loop (defaults to true).</param>
		/// <param name="reset">If the animation should be restarted if it is already playing (defaults to false).</param>
		public void Play(string name, bool loop = true, bool reset = false)
		{
			if (reset || name != CurrentAnim)
			{
				_time = 0;
				_state.SetAnimation(name, loop);
			}
		}
		
		/// <summary>
		/// Rotation of the image, in degrees.
		/// </summary>
		public float Angle = 0;
		
		/// <summary>
		/// Scale of the image, affects both x and y scale.
		/// </summary>
		public float Scale = 1;
		
		/// <summary>
		/// X scale of the image.
		/// </summary>
		public float ScaleX = 1;
		
		/// <summary>
		/// Y scale of the image.
		/// </summary>
		public float ScaleY = 1;
		
		/// <summary>
		/// X origin of the image, determines transformation point.
		/// </summary>
		public float OriginX = 0;
		
		/// <summary>
		/// Y origin of the image, determines transformation point.
		/// </summary>
		public float OriginY = 0;
		
		/// <summary>
		/// If you want to draw the image horizontally flipped.
		/// This will flip the texture without moving it, as would happen if you set ScaleX to -1.
		/// </summary>
		public bool FlippedX
		{
			get
			{
				return _skeleton.FlipX;
			}
			
			set
			{
				_skeleton.FlipX = value;
			}
		}
		
		/// <summary>
		/// If the animation has stopped.
		/// </summary>
		public bool Complete
		{
			get
			{
				return _time == -1;
			}
		}
		
		/// <summary>
		/// If you want to draw the image vertically flipped.
		/// This will flip the texture without moving it, as would happen if you set ScaleY to -1.
		/// </summary>
		public bool FlippedY
		{
			get
			{
				return _skeleton.FlipY;
			}
			
			set
			{
				_skeleton.FlipY = value;
			}
		}
		
		/// <summary>
		/// The currently playing animation.
		/// </summary>
		public string CurrentAnim
		{
			get
			{
				return _state.Animation != null ? _state.Animation.Name : "";
			}
		}
		
		/// <summary>
		/// The skin of the animation.
		/// </summary>
		public string Skin
		{
			get
			{
				return _skeleton.Skin != null ? _skeleton.Skin.Name : "";
			}
			
			set
			{
				_skeleton.SetSkin(value);
			}
		}
		
		/// <summary>
		/// Optional callback function for animation end.
		/// </summary>
		public OnComplete Callback;
		
		/// <summary>
		/// Animation speed factor, alter this to speed up/slow down all animations.
		/// </summary>
		public float Rate;
		
		private VertexArray _vertexArray;
		float[] _vertexPositions;
		Transformable _transform;
		private float _time = 0;
		private Skeleton _skeleton;
		private AnimationState _state;
	}
}


