 /* Created by SharpDevelop.
 * User: Jake
 * Date: 5/3/2013
 * Time: 4:13 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Xml;
using System.Diagnostics;
using SFML;
using SFML.Window;
using Punk.Graphics;
using Punk.Masks;
using System.Collections.Generic;
using Punk.Utils;
using Punk.Utils.Reflect;
using Fasterflect;
using SF = SFML.Graphics;

namespace Punk
{
	/// <summary>
	/// Main game Entity class updated by World.
	/// </summary>
	public class Entity : Tweener
	{
		/**
		 * If the Entity should render.
		 */
		public bool Visible = true;
		
		/**
		 * If the Entity should respond to collision checks.
		 */
		public bool Collidable = true;
		
		/**
		 * X position of the Entity in the World.
		 */
		public float X = 0;
		
		/**
		 * Y position of the Entity in the World.
		 */
		public float Y = 0;
		
		/**
		 * Width of the Entity's hitbox.
		 */
		public int Width;
		
		/**
		 * Height of the Entity's hitbox.
		 */
		public int Height;
		
		/**
		 * X origin of the Entity's hitbox.
		 */
		public float OriginX;
		
		/**
		 * Y origin of the Entity's hitbox.
		 */
		public float OriginY;
		
		/// <summary>
		/// Constructor. Can be usd to place the Entity and assign a graphic and mask.
		/// </summary>
		/// <param name="x">X position to place the Entity (default 0).</param>
		/// <param name="y">Y position to place the Entity (default 0).</param>
		/// <param name="graphic">Graphic to assign to the Entity (default null).</param>
		/// <param name="mask">Mask to assign to the Entity (default null).</param>
		public Entity(float x = 0, float y = 0, Graphic graphic = null, Mask mask = null)
		{
			X = x;
			Y = y;
			
			_graphic = graphic;
			_mask = mask;
			
			HITBOX.AssignTo(this);
			
			_logic = new List<Logic>();
			_logicAdd = new List<Logic>();
			_logicRemove = new List<Logic>();
			_responses = new Dictionary<string, Entity.MessageResponse>();
		}
		
		/// <summary>
		/// Override this, called when the Entity is added to a World.
		/// </summary>
		public virtual void Added()
		{
		}
		
		/// <summary>
		/// Override this, called when the Entity is removed from a World.
		/// </summary>
		public virtual void Removed()
		{
		}
		
		/// <summary>
		/// Updates the Entity.
		/// </summary>
		override public void Update()
		{
		}
		
		/// <summary>
		/// Renders the Entity. If you override this for special behaviour,
		/// remember to call base.render() to render the Entity's graphic.
		/// </summary>
		public virtual void Render()
		{
			if (_graphic != null && _graphic.Visible)
			{
				if (_graphic.Relative)
				{
					_graphic.Render(X, Y, _world.Camera);
				}
				else
				{
					_graphic.Render(0, 0, _world.Camera);
				}
			}
		}
		
		#region Messaging
		/// <summary>
		/// Callback type for message responses.
		/// </summary>
		public delegate void MessageResponse(params object[] arguments);
		
		/// <summary>
		/// Called when this recieves a message broadcast from the World
		/// </summary>
		/// <param name="message">The name of the message.</param>
		/// <param name="arguments">A set of arguments.</param>
		public virtual void OnMessage(string message, params object[] arguments)
		{
			foreach (Logic logic in _logic)
			{
				logic.OnMessage(message, arguments);
			}
			
			if (_responses.ContainsKey(message))
			{
				_responses[message](arguments);
			}
		}
		
		/// <summary>
		/// Add a message response.
		/// </summary>
		/// <param name="message">The message type.</param>
		/// <param name="response">The response callback to add.</param>
		public void AddResponse(string message, MessageResponse response)
		{
			if (_responses.ContainsKey(message))
			{
				_responses[message] += response;
			}
			else
			{
				_responses.Add(message, delegate {});
				_responses[message] += response;
			}
		}
		
		/// <summary>
		/// Remove a message response.
		/// </summary>
		/// <param name="message">The message type to remove the response from.</param>
		/// <param name="response">The response callback to remove.</param>
		public void RemoveResponse(string message, MessageResponse response)
		{
			if (_responses.ContainsKey(message))
			{
				if ((_responses[message] -= response) == null)
				{
					_responses.Remove(message);
				}
			}
		}
		#endregion
		
		/// <summary>
		/// Override this to specify behavior when this is being loaded from an Ogmo XML Entity.
		/// </summary>
		/// <param name="node">The XML node that describes the entity.</param>
		public virtual void Load(XmlNode node)
		{
		}
		
		#region Collision
		
		/// <summary>
		/// Checks for a collision against an Entity type.
		/// </summary>
		/// <param name="type">The Entity type to check for.</param>
		/// <param name="x">Virtual x position to place this Entity.</param>
		/// <param name="y">Virtual y position to place this Entity.</param>
		/// <returns>The first Entity collided with, or null if none were collided.</returns>
		public Entity Collide(string type, float x, float y)
		{
			if (_world == null)
			{
				return null;
			}
			
			if (!_world._typeFirst.ContainsKey(type))
			{
				return null;
			}
			
			Entity e = _world._typeFirst[type];
			
			//	store the real position
			_x = X; _y = Y;
			X = x; Y = y;
			
			if (_mask == null)
			{
				while (e != null)
				{
					if (e.Collidable && !ReferenceEquals(e, this)
					&& x - OriginX + Width > e.X - e.OriginX
					&& y - OriginY + Height > e.Y - e.OriginY
					&& x - OriginX < e.X - e.OriginX + e.Width
					&& y - OriginY < e.Y - e.OriginY + e.Height)
					{
						if (e._mask == null || e._mask.Collide(HITBOX))
						{
							X = _x;
							Y = _y;
							return e;
						}
					}
					e = e._typeNext;
				}
				X = _x;
				Y = _y;
				return null;
			}
			
			while (e != null)
			{
				if (e.Collidable && !ReferenceEquals(e, this)
				&& x - OriginX + Width > e.X - e.OriginX
				&& y - OriginY + Height > e.Y - e.OriginY
				&& x - OriginX < e.X - e.OriginX + e.Width
				&& y - OriginY < e.Y - e.OriginY + e.Height)
				{
					if (_mask.Collide(e._mask != null ? e._mask : e.HITBOX))
					{
						X = _x; Y = _y;
						return e;
					}
				}
				e = e._typeNext;
			}
			
			X = _x; Y = _y;
			return null;
		}
		
		/// <summary>
		/// Checks for collision against multiple Entity types.
		/// </summary>
		/// <param name="types">A collection (array, List, etc) of Entity types to check for.</param>
		/// <param name="x">Virtual x position to place this Entity.</param>
		/// <param name="y">Virtual y position to place this Entity.</param>
		/// <returns>The first Entity collided with, or null if none were collided.</returns>
		public Entity CollideTypes(IEnumerable<string> types, float x, float y)
		{
			if (_world == null)
			{
				return null;
			}
			
			foreach (string type in types)
			{
				Entity e = Collide(type, x, y);
				if (e != null) return e;
			}
			
			return null;
		}
		
		/// <summary>
		/// Checks if this Entity collides with a specific Entity.
		/// </summary>
		/// <param name="e">The Entity to collide against.</param>
		/// <param name="x">Virtual x position to place this Entity.</param>
		/// <param name="y">Virtual y position to place this Entity.</param>
		/// <returns>The Entity if they overlap, or null if they don't.</returns>
		public Entity CollideWith(Entity e, float x, float y)
		{
			_x = X; _y = Y;
			X = x; Y = y;
			
			if (e.Collidable
			&& x - OriginX + Width > e.X - e.OriginX
			&& y - OriginY + Height > e.Y - e.OriginY
			&& x - OriginX < e.X - e.OriginX + e.Width
			&& y - OriginY < e.Y - e.OriginY + e.Height)
			{
				if (_mask == null)
				{
					if (e._mask == null || e._mask.Collide(HITBOX))
					{
						this.X = _x; this.Y = _y;
						return e;
					}
					this.X = _x; this.Y = _y;
					return null;
				}
				if (_mask.Collide(e._mask != null ? e._mask : e.HITBOX))
				{
					this.X = _x; this.Y = _y;
					return e;
				}
			}
			this.X = _x; this.Y = _y;
			return null;
		}
		
		/// <summary>
		/// Checks if this Entity overlaps the specified rectangle.
		/// </summary>
		/// <param name="x">Virtual x position to place this Entity.</param>
		/// <param name="y">Virtual y position to place this Entity.</param>
		/// <param name="rX">X position of the rectangle.</param>
		/// <param name="rY">Y position of the rectangle.</param>
		/// <param name="rWidth">Width of the rectangle.</param>
		/// <param name="rHeight">Height of the rectangle.</param>
		/// <returns>If they overlap.</returns>
		public bool CollideRect(float x, float y, float rX, float rY, float rWidth, float rHeight)
		{
			if (x - OriginX + Width >= rX && y - OriginY + Height >= rY
			&& x - OriginX <= rX + rWidth && y - OriginY <= rY + rHeight)
			{
				if (_mask == null)
				{
					return true;
				}
				
				_x = X; _y = Y;
				Y = x; Y = y;
				
				Entity entity = new Entity();
				entity.X = rX;
				entity.Y = rY;
				entity.Width = (int) rWidth;
				entity.Height = (int) rHeight;
				
				if (_mask.Collide(entity.HITBOX))
				{
					X = _x; Y = _y;
					return true;
				}
				X = _x; Y = _y;
				return false;
			}
			return false;
		}
		
		/// <summary>
		/// Checks if this Entity overlaps the specified position.
		/// </summary>
		/// <param name="x">Virtual x position to place this Entity.</param>
		/// <param name="y">Virtual y position to place this Entity.</param>
		/// <param name="pX">X position.</param>
		/// <param name="pY">Y position.</param>
		/// <returns>If the Entity intersects with the position.</returns>
		public bool CollidePoint(float x, float y, float pX, float pY)
		{
			if (pX >= x - OriginX && pY >= y - OriginY
			&& pX < x - OriginX + Width && pY < y - OriginY + Height)
			{
				if (_mask == null)
				{
					return true;
				}
				
				_x = this.X; _y = this.Y;
				this.X = x; this.Y = y;
				
				var entity = new Entity();
				entity.X = pX;
				entity.Y = pY;
				entity.Width = 1;
				entity.Height = 1;
				
				if (_mask.Collide(entity.HITBOX))
				{
					this.X = _x; this.Y = _y;
					return true;
				}
				this.X = _x; this.Y = _y;
				return false;
			}
			return false;
		}
		
		/// <summary>
		/// Populates an array with all collided Entities of a type.
		/// </summary>
		/// <param name="type">The Entity type to check for.</param>
		/// <param name="x">Virtual x position to place this Entity.</param>
		/// <param name="y">Virtual y position to place this Entity.</param>
		/// <param name="array">The Array or Vector object to populate.</param>
		public void CollideInto(string type, float x, float y, List<Entity> array = null)
		{
			if (_world == null) return;
			
			var e = _world._typeFirst[type];
			if (e == null) return;
			
			if (array == null)
			{
				array = new List<Entity>();
			}
			
			_x = X; _y = Y;
			X = x; Y = y;
			
			if (_mask == null)
			{
				while (e != null)
				{
					if (e.Collidable && !ReferenceEquals(e, this)
					&& x - OriginX + Width > e.X - e.OriginX
					&& y - OriginY + Height > e.Y - e.OriginY
					&& x - OriginX < e.X - e.OriginX + e.Width
					&& y - OriginY < e.Y - e.OriginY + e.Height)
					{
						if (e._mask == null|| e._mask.Collide(HITBOX))
						{
							array.Add(e);
						}
					}
					e = e._typeNext;
				}
				X = _x; Y = _y;
				return;
			}
			
			while (e != null)
			{
				if (e.Collidable && !ReferenceEquals(e, this)
				&& x - OriginX + Width > e.X - e.OriginX
				&& y - OriginY + Height > e.Y - e.OriginY
				&& x - OriginX < e.X - e.OriginX + e.Width
				&& y - OriginY < e.Y - e.OriginY + e.Height)
				{
					if (_mask.Collide(e._mask != null ? e._mask : e.HITBOX))
					{
						array.Add(e);
					}
				}
				e = e._typeNext;
			}
			X = _x; Y = _y;
		}
		
		/// <summary>
		/// Populates a List with all collided Entities of multiple types.
		/// </summary>
		/// <param name="types">A collection (array, List, etc) of Entity types to check for.</param>
		/// <param name="x">Virtual x position to place this Entity.</param>
		/// <param name="y">Virtual y position to place this Entity.</param>
		/// <param name="array">The List to populate.</param>
		/// <returns></returns>
		public List<Entity> CollideTypesInto(IEnumerable<string> types, float x, float y, List<Entity> array)
		{
			if (_world == null) return array;
			
			foreach (string type in types)
			{
				CollideInto(type, x, y, array);
			}
			
			return array;
		}
		#endregion
		
		#region Logic components
		
		/// <summary>
		/// Add a Logic component to the Entity.
		/// </summary>
		/// <param name="l">The Logic component to remove.</param>
		/// <returns>The added Logic component.</returns>
		public Logic AddLogic(Logic l)
		{
			if (l.Parent != null)
			{
				throw new Exception("A logic component can only be added to one entity at once!");
			}
			
			_logicAdd.Add(l);
			
			return l;
		}
		
		/// <summary>
		/// Removes a Logic component to the Entity.
		/// </summary>
		/// <param name="l">The Logic component to remove.</param>
		/// <returns>The removed Logic component.</returns>
		public Logic RemoveLogic(Logic l)
		{
			if (ReferenceEquals(l.Parent, this))
			{
				_logicRemove.Add(l);
			}
			
			return l;
		}
		
		#endregion
		
		/// <summary>
		/// Adds the graphic to the Entity via a Graphiclist.
		/// </summary>
		/// <param name="g">Graphic to add.</param>
		/// <returns>The added Graphic.</returns>
		public Graphic AddGraphic(Graphic g)
		{
			var list = Graphic as Graphiclist;
			
			if (list != null)
			{
				list.Add(g);
			}
			else
			{
				list = new Graphiclist();
				if (Graphic != null)
				{
					list.Add(Graphic);
				}
				
				list.Add(g);
				Graphic = list;
			}
			
			return g;
		}
		
		/// <summary>
		/// Sets the Entity's hitbox properties.
		/// </summary>
		/// <param name="width">Width of the hitbox.</param>
		/// <param name="height">Height of the hitbox.</param>
		/// <param name="originX">X origin of the hitbox.</param>
		/// <param name="originY">Y origin of the hitbox.</param>
		public void SetHitbox(int width = 0, int height = 0, int originX = 0, int originY= 0)
		{
			Width = width;
			Height = height;
			OriginX = originX;
			OriginY = originY;
		}
		
		/// <summary>
		/// Sets the Entity's hitbox to match that of the provided object.
		/// </summary>
		/// <param name="o">The object defining the hitbox (eg. an Image or Rectangle).</param>
		public void SetHitboxTo(object o)
		{
			if (o is Image)
			{
				var i = o as Image;
				SetHitbox((int) i.Width, (int) i.Height, (int) -i.X, (int) -i.Y);
			}
			else if (o is SF.FloatRect)
			{
				var rect = (SF.FloatRect) o;
				SetHitbox((int) rect.Width, (int) rect.Height, (int) -rect.Left, (int) -rect.Top);
			}
			else if (o is SF.IntRect)
			{
				var rect = (SF.IntRect) o;
				SetHitbox((int) rect.Width, (int) rect.Height, (int) -rect.Left, (int) -rect.Top);
			}
			else
			{
				if (o.HasOwnProperty("Width"))
				{
					Width = o.GetProp<int>("Width");
				}
				
				if (o.HasOwnProperty("Height"))
				{
					Height = o.GetProp<int>("Height");
				}
				
				if (o.HasOwnProperty("OriginX") && !(o is Graphic))
				{
					OriginX = o.GetProp<int>("OriginX");
				}
				else if (o.HasOwnProperty("X"))
				{
					OriginX = -o.GetProp<int>("OriginX");
				}
				
				if (o.HasOwnProperty("OriginY") && !(o is Graphic))
				{
					OriginX = o.GetProp<int>("OriginY");
				}
				else if (o.HasOwnProperty("Y"))
				{
					OriginX = -o.GetProp<int>("OriginY");
				}
			}
		}
		
		/// <summary>
		/// Sets the origin of the Entity.
		/// </summary>
		/// <param name="x">X origin.</param>
		/// <param name="y">Y origin.</param>
		public void SetOrigin(int x = 0, int y = 0)
		{
			OriginX = x;
			OriginY = y;
		}
		
		/// <summary>
		/// Center's the Entity's origin (half width and height).
		/// </summary>
		public void CenterOrigin()
		{
			OriginX = Width / 2;
			OriginY = Height / 2;
		}
		
		/// <summary>
		/// Calculates the distance from another Entity.
		/// </summary>
		/// <param name="e">The other Entity.</param>
		/// <param name="useHitboxes">If hitboxes should be used to determine the distance. If not, the Entities' x/y positions are used.</param>
		/// <returns>The distance.</returns>
		public float DistanceFrom(Entity e, bool useHitboxes = false)
		{
			if (!useHitboxes)
			{
				return (float) Math.Sqrt((X - e.X) * (X - e.X) + (Y - e.Y) * (Y - e.Y));
			}
			
			return FP.DistanceRects(X - OriginX, Y - OriginY, Width, Height, e.X - e.OriginX, e.Y - e.OriginY, e.Width, e.Height);
		}
		
		/// <summary>
		/// Calculates the distance from this Entity to the point.
		/// </summary>
		/// <param name="px">X position.</param>
		/// <param name="py">Y position.</param>
		/// <param name="useHitbox">If hitboxes should be used to determine the distance. If not, the Entities' x/y positions are used.</param>
		/// <returns>The distance.</returns>
		public float DistanceToPoint(float px, float py, bool useHitbox = false)
		{
			if (!useHitbox)
			{
				return (float) Math.Sqrt((X - px) * (X - px) + (Y - py) * (Y - py));
			}
			
			return FP.DistanceRectPoint(px, py, X - OriginX, Y - OriginY, Width, Height);
		}
		
		/// <summary>
		/// Calculates the distance from this Entity to the rectangle.
		/// </summary>
		/// <param name="rx">X position of the rectangle.</param>
		/// <param name="ry">Y position of the rectangle.</param>
		/// <param name="rwidth">Width of the rectangle.</param>
		/// <param name="rheight">Height of the rectangle.</param>
		/// <returns>The distance.</returns>
		public float DistanceToRect(float rx, float ry, float rwidth, float rheight)
		{
			return FP.DistanceRects(rx, ry, rwidth, rheight, X - OriginX, Y - OriginY, Width, Height);
		}
		
		/// <summary>
		/// Gets the class name as a string.
		/// </summary>
		/// <returns>A string representing the class name.</returns>
		public override string ToString()
		{
			return GetType().Name;
		}
		
		/// <summary>
		/// Moves the Entity by the amount, retaining integer values for its x and y.
		/// </summary>
		/// <param name="x">Horizontal offset.</param>
		/// <param name="y">Vertical offset.</param>
		/// <param name="solidType">An optional collision type (or array of types) to stop flush against upon collision.</param>
		/// <param name="sweep">If sweeping should be used (prevents fast-moving objects from going through solidType).</param>
		public void MoveBy(float x, float y, object solidType = null, bool sweep = false)
		{
			IEnumerable<string> solids = null;
			if (solidType == null)
			{
				solids = new string[] {};
			}
			else if (solidType is string)
			{
				solids = new string[] {solidType as string};
			}
			else
			{
				solids = solidType as IEnumerable<string>;
				Debug.Assert(solids != null, "solidType must be a string or an IEnumerable<string>");
			}
			
			x = (float) Math.Round(x);
			y = (float) Math.Round(y);
			
			
			if (solidType != null)
			{
				if (x != 0)
				{
					if (sweep || (CollideTypes(solids, X + x, Y) != null))
					{
						int sign = x > 0 ? 1 : -1;
						
						while (x != 0)
						{
							Entity e = CollideTypes(solids, X + sign, Y);
							if (e != null)
							{
								if (MoveCollideX(e))
								{
									break;
								}
								else
								{
									X += sign;
								}
							}
							else
							{
								X += sign;
							}
							
							x -= sign;
						}
					}
					else
					{
						X += x;
					}
				}
				
				if (y != 0)
				{
					if (sweep || (CollideTypes(solids, X, Y + y) != null))
					{
						int sign = y > 0 ? 1 : -1;
						while (y != 0)
						{
							Entity e = CollideTypes(solids, X, Y + sign);
							if (e != null)
							{
								if (MoveCollideY(e))
								{
									break;
								}
								else
								{
									Y += sign;
								}
							}
							else
							{
								Y += sign;
							}
							
							y -= sign;
						}
					}
					else
					{
						Y += y;
					}
				}
			}
			else
			{
				X += x;
				Y += y;
			}
		}
		
		/// <summary>
		/// Moves the Entity to the position, retaining integer values for its x and y.
		/// </summary>
		/// <param name="x">X position.</param>
		/// <param name="y">Y position.</param>
		/// <param name="solidType">An optional collision type (or array of types) to stop flush against upon collision.</param>
		/// <param name="sweep">If sweeping should be used (prevents fast-moving objects from going through solidType).</param>
		public void MoveTo(float x, float y, object solidType = null, bool sweep = false)
		{
			MoveBy(x - X, y - Y, solidType, sweep);
		}
		
		/// <summary>
		/// Moves towards the target position, retaining integer values for its x and y.
		/// </summary>
		/// <param name="x">X target.</param>
		/// <param name="y">Y target.</param>
		/// <param name="amount">Amount to move.</param>
		/// <param name="solidType">An optional collision type (or array of types) to stop flush against upon collision.</param>
		/// <param name="sweep">If sweeping should be used (prevents fast-moving objects from going through solidType).</param>
		public void MoveTowards(float x, float y, float amount, object solidType = null, bool sweep = false)
		{
			var point = new Vector2f(x - X, y - Y);
			
			if (point.X * point.X + point.Y * point.Y > amount * amount)
			{
				//	FIXME:	Normalize is a bit broken.
				point = point.Normalized(amount);
			}
			
			MoveBy(point.X, point.Y, solidType, sweep);
		}
		
		/// <summary>
		/// When this collide with an Entity on the x-axis with moveTo() or moveBy().
		/// </summary>
		/// <param name="e">The Entity this collided with.</param>
		/// <returns>Whether movement should be halted.</returns>
		public virtual bool MoveCollideX(Entity e)
		{
			return true;
		}
		
		/// <summary>
		/// When this collide with an Entity on the y-axis with moveTo() or moveBy().
		/// </summary>
		/// <param name="e">The Entity this collided with.</param>
		/// <returns>Whether movement should be halted.</returns>
		public virtual bool MoveCollideY(Entity e)
		{
			return true;
		}
		
		/// <summary>
		/// Clamps the Entity's hitbox on the x-axis.
		/// </summary>
		/// <param name="left">Left bounds.</param>
		/// <param name="right">Right bounds.</param>
		/// <param name="padding">Optional padding on the clamp.</param>
		public void ClampHorizontal(float left, float right, float padding = 0)
		{
			if (X - OriginX < left + padding)
			{
				X = left + OriginX + padding;
			}
			
			if (X - OriginX + Width > right - padding)
			{
				X = right - Width + OriginX - padding;
			}
		}
		
		/// <summary>
		/// Clamps the Entity's hitbox on the y axis.
		/// </summary>
		/// <param name="top">Min bounds.</param>
		/// <param name="bottom">Max bounds.</param>
		/// <param name="padding">Optional padding on the clamp.</param>
		public void ClampVertical(float top, float bottom, float padding = 0)
		{
			if (Y - OriginY < top + padding)
			{
				Y = top + OriginY + padding;
			}
			
			if (Y - OriginY + Height > bottom - padding)
			{
				Y = bottom - Height + OriginY - padding;
			}
		}
		
		/// <summary>
		/// If the Entity collides with the camera rectangle.
		/// </summary>
		public bool OnCamera
		{
			get
			{
				//	TODO:	OnCamera checks
				return true;
			}
		}
		
		/// <summary>
		/// The World object this Entity has been added to.
		/// </summary>
		public World World
		{
			get
			{
				return _world;
			}
		}
		
		
		
		/// <summary>
		/// Half the Entity's width.
		/// </summary>
		public float HalfWidth
		{
			get
			{
				return Width / 2;
			}
		}
		
		/// <summary>
		/// Half the Entity's height.
		/// </summary>
		public float HalfHeight
		{
			get
			{
				return Height / 2;
			}
		}
		
		/// <summary>
		/// The center x position of the Entity's hitbox.
		/// </summary>
		public float CenterX
		{
			get
			{
				return X - OriginX + Width / 2;
			}
		}
		
		/// <summary>
		/// The center y position of the Entity's hitbox.
		/// </summary>
		public float CenterY
		{
			get
			{
				return Y - OriginY + Height / 2;
			}
		}
		
		/// <summary>
		/// The leftmost position of the Entity's hitbox.
		/// </summary>
		public float Left
		{
			get
			{
				return X - OriginX;
			}
		}
		
		/// <summary>
		/// The rightmost position of the Entity's hitbox.
		/// </summary>
		public float Right
		{
			get
			{
				return X - OriginX + Width;
			}
		}
		
		/// <summary>
		/// The topmost position of the Entity's hitbox.
		/// </summary>
		public float Top
		{
			get
			{
				return Y - OriginY;
			}
		}
		
		/// <summary>
		/// The bottommost position of the Entity's hitbox.
		/// </summary>
		public float Bottom
		{
			get
			{
				return Y - OriginY + Height;
			}
		}
		
		/// <summary>
		/// The rendering layer of this Entity. Higher layers are rendered first.
		/// </summary>
		public int Layer
		{
			get
			{
				return _layer;
			}
			
			set
			{
				if (_layer == value) return;
				
				if (_world == null)
				{
					_layer = value;
					return;
				}
				
				_world.RemoveRender(this);
				_layer = value;
				_world.AddRender(this);
			}
		}
		
		/// <summary>
		/// The collision type, used for collision checking.
		/// </summary>
		public string Type
		{
			get
			{
				return _type;
			}
			
			set
			{
				if (_type == value) return;
				if (_world == null)
				{
					_type = value;
					return;
				}
				
				if (_type != null)
				{
					_world.RemoveType(this);
				}
				
				_type = value;
				if (value != null) _world.AddType(this);
			}
		}
		
		/// <summary>
		/// An optional Mask component, used for specialized collision. If this is
		/// not assigned, collision checks will use the Entity's hitbox by default.
		/// </summary>
		public Mask Mask
		{
			get
			{
				return _mask;
			}
			
			set
			{
				if (ReferenceEquals(_mask, value))
				{
					return;
				}
				
				if (_mask != null)
				{
					_mask.AssignTo(null);
				}
				
				_mask = value;
				
				if (value != null)
				{
					_mask.AssignTo(this);
				}
			}
		}
		
		/// <summary>
		/// Graphical component to render to the screen.
		/// </summary>
		public Graphic Graphic
		{
			get
			{
				return _graphic;
			}
			
			set
			{
				if (ReferenceEquals(_graphic, value))
				{
					return;
				}
				
				_graphic = value;
				if (value != null && value._assign != null)
				{
					value._assign();
				}
			}
		}
		
		/// <summary>
		/// The Entity's instance name. Use this to uniquely identify single
		/// game Entities, which can then be looked-up with World.GetInstance().
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			
			set
			{
				if (_name == value)
				{
					return;
				}
				
				if (!String.IsNullOrEmpty(_name) && _world != null)
				{
					_world.UnregisterName(this);
				}
				
				_name = value;
				
				if ((!String.IsNullOrEmpty(_name)) && _world != null)
				{
					_world.RegisterName(this);
				}
			}
		}
		
		#region Entity information.
		internal World _world;
		internal string _type;
		internal string _name;
		internal int _layer;
		internal Entity _updatePrev;
		internal Entity _updateNext;
		internal Entity _renderPrev;
		internal Entity _renderNext;
		internal Entity _typePrev;
		internal Entity _typeNext;
		internal List<Logic> _logic;
		internal List<Logic> _logicAdd, _logicRemove;
		private Dictionary<string, MessageResponse> _responses;
		#endregion
		
		#region Collision information.
		private readonly Mask HITBOX = new Mask();
		private Mask _mask;
		private float _x;
		private float _y;
		#endregion
		
		#region Rendering information.
		internal Graphic _graphic;
		#endregion
	}
}
