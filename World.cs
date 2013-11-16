/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 5/3/2013
 * Time: 12:47 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Xml;

using Punk.Graphics;
using Punk.Masks;
using Punk.Utils;
using SFML.Window;

namespace Punk
{
	/// <summary>
	/// Updated by Engine, main game container that holds all currently active Entities.
	/// Useful for organization, eg. "Menu", "Level1", etc.
	/// </summary>
	public class World : Tweener
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public World()
		{
			Camera = new Camera();
			
			_remove = new List<Entity>();
			_add = new List<Entity>();
			
			_typeFirst = new Dictionary<string, Entity>();
			_typeCount = new Dictionary<string, uint>();
			_renderFirst = new Dictionary<int, Entity>();
			_renderLast = new Dictionary<int, Entity>();
			_layerCount = new Dictionary<int, int>();
			_layerList = new List<int>();
			_entityNames = new Dictionary<string, Entity>();
			
			_types = new Dictionary<string, OgmoClass>();
			_gridTypes = new Dictionary<string, GridSize>();
			
			Visible = true;
			CanLoad = true;
		}
		
		/// <summary>
		/// Override this; called when the World is switched to, and set to the currently active world.
		/// </summary>
		public virtual void Begin()
		{
		}
		
		/// <summary>
		/// Override this; called when World is changed, and the active world is no longer this.
		/// </summary>
		public virtual void End()
		{
		}
		
		/// <summary>
		/// Performed by the game loop, updates all contained Entities.
		/// If you override this to give your World update code, remember
		/// to call base.update() or your Entities will not be updated.
		/// </summary>
		public override void Update()
		{
			base.Update();
			
			CanLoad = true;
			
			Entity e = _updateFirst;
			while (e != null)
			{
				if (e.Active)
				{
					if (e._tween != null)
					{
						e.UpdateTweens();
					}
					
					if (e._logicAdd.Count > 0)
					{
						var addBuffer = new List<Logic>(e._logicAdd);
						
						foreach (Logic l in addBuffer)
						{
							e._logicAdd.Remove(l);
							e._logic.Add(l);
							
							l.Parent = e;
							l.Added();
						}
					}
					
					if (e._logicRemove.Count > 0)
					{
						var removeBuffer = new List<Logic>(e._logicRemove);
						
						foreach (Logic l in removeBuffer)
						{
							e._logicRemove.Remove(l);
							e._logic.Remove(l);
							
							l.Removed();
							l.Parent = null;
						}
					}
					
					foreach (Logic logic in e._logic)
					{
						logic.Update();
					}
					
					e.Update();
				}
				
				if (e._graphic != null && e._graphic.Active)
				{
					e._graphic.Update();
				}
				
				e = e._updateNext;
			}
		}
		
		/// <summary>
		/// Performed by the game loop, updates all contained Entities.
		/// If you override this to give your World update code, remember
		/// to call base.update() or your Entities will not be updated.
		/// </summary>
		public void Render()
		{
			// render the entities in order of depth
			Entity e;
			int i = _layerList.Count;
			
			while (i-- > 0)
			{
				e = _renderLast[_layerList[i]];
				while (e != null)
				{
					if (e.Visible)
					{
						e.Render();
					}
					
					e = e._renderPrev;
				}
			}
		}
		
		/// <summary>
		/// The world camera.
		/// </summary>
		public Camera Camera;
		
		/// <summary>
		/// X position of the mouse in the World.
		/// </summary>
		public int MouseX
		{
			get
			{
				//	TODO:	get actual position
				return (int) (Input.MouseX + Camera.X - FP.HalfWidth);
			}
		}
		
		/// <summary>
		/// Y position of the mouse in the World.
		/// </summary>
		public int MouseY
		{
			get
			{
				//	TODO:	get actual position
				return (int) (Input.MouseY + Camera.Y - FP.HalfHeight);
			}
		}
		
		/// <summary>
		/// Adds the Entity to the World at the end of the frame
		/// </summary>
		/// <param name="e">Entity object you want to add.</param>
		/// <returns>The added Entity object.</returns>
		public Entity Add(Entity e)
		{
			_add.Add(e);
			return e;
		}
		
		/// <summary>
		/// Removes the Entity from the World at the end of the frame.
		/// </summary>
		/// <param name="e">Entity object you want to remove.</param>
		/// <returns>The removed Entity object.</returns>
		public Entity Remove(Entity e)
		{
			_remove.Add(e);
			return e;
		}
		
		/// <summary>
		/// Removes all Entities from the World at the end of the frame.
		/// </summary>
		public void RemoveAll()
		{
			Entity e = _updateFirst;
			while (e != null)
			{
				_remove.Add(e);
				e = e._updateNext;
			}
		}
		
		/// <summary>
		/// Adds multiple Entities to the world.
		/// </summary>
		/// <param name="list">Several Entities as arguments</param>
		public void AddList(params Entity[] list)
		{
			foreach (Entity e in list)
			{
				Add(e);
			}
		}
		
		/// <summary>
		/// Adds multiple Entities to the world.
		/// </summary>
		/// <param name="list">A List of Entities to add.</param>
		public void AddList(List<Entity> list)
		{
			foreach (Entity e in list)
			{
				Add(e);
			}
		}
		
		/// <summary>
		/// Removes multiple Entities from the world.
		/// </summary>
		/// <param name="list">Several Entities as arguments.</param>
		public void RemoveList(params Entity[] list)
		{
			foreach (Entity e in list)
			{
				Remove(e);
			}
		}
		
		/// <summary>
		/// Removes multiple Entities to the world.
		/// </summary>
		/// <param name="list">A List of Entities to remove.</param>
		public void RemoveList(List<Entity> list)
		{
			foreach (Entity e in list)
			{
				Remove(e);
			}
		}
		
		/// <summary>
		/// Adds an Entity to the World with the Graphic object.
		/// </summary>
		/// <param name="graphic">Graphic to assign the Entity.</param>
		/// <param name="layer">Layer of the Entity.</param>
		/// <param name="x">X position of the Entity.</param>
		/// <param name="y">Y position of the Entity.</param>
		/// <returns>The Entity that was added.</returns>
		public Entity AddGraphic(Graphic graphic, int layer = 0, float x = 0, float y = 0)
		{
			var e = new Entity(x, y, graphic);
			if (layer != 0)
			{
				e.Layer = layer;
			}
			
			e.Active = false;
			return Add(e);
		}
		
		/// <summary>
		/// Adds an Entity to the World with the Mask object.
		/// </summary>
		/// <param name="mask">Mask to assign the Entity.</param>
		/// <param name="type">Collision type of the Entity.</param>
		/// <param name="x">X position of the Entity.</param>
		/// <param name="y">Y position of the Entity.</param>
		/// <returns>The Entity that was added.</returns>
		public Entity AddMask(Mask mask, string type, int x = 0, int y = 0)
		{
			var e = new Entity(x, y, null, mask);
			if (type.Length > 0)
			{
				e.Type = type;
			}
			
			e.Active = e.Visible = false;
			return Add(e);
		}
		
		//	TODO:	FP has create/recycle methods. I don't think we can do this, and we never use them anyway
		
		#region Ogmo loading
		
		public void RegisterGridType(string name, int cellWidth, int cellHeight)
		{
			_gridTypes.Add(name, new GridSize(cellWidth, cellHeight));
		}
		
		public void RegisterClass<T>(string name) where T : Entity
		{
			_types.Add(name, new OgmoClass(name, typeof(T)));
		}
		
//		public void RegisterClass<T>(string name, params string[] arguments) where T : Entity
//		{
//			_types.Add(name, new OgmoClass(name, typeof(T), arguments));
//		}
		
		/// <summary>
		/// Load a world from an Ogmo level file.
		/// </summary>
		/// <param name="filename">The filename of the level.</param>
		public void BuildWorld(string filename)
        {
            if (!CanLoad)
            {
                return;
            }
			
            RemoveAll();
           
            AddList(BuildWorldAsArray(filename));
           
            CanLoad = false;
        }
		
		/// <summary>
		/// Load Entities from an Ogmo level file into an array.
		/// </summary>
		/// <param name="filename">The filename of the level.</param>
		/// <returns>An array containing all Entities loaded.</returns>
		public Entity[] BuildWorldAsArray(string filename)
		{
			var layerIndices = new Dictionary<string, int>();
			var level = Library.GetXml(filename);
            var result = new List<Entity>();
            var layerNodes = new List<XmlNode>();
            numLayers = 0;
            
			//	Get a list of the layer nodes
			foreach (XmlNode layerNode in level["level"].ChildNodes)
			{
				layerNodes.Add(layerNode);
				layerIndices.Add(layerNode.Name, 0);
			}
			
			//	Reverse the list so the layers will be in the right order
			layerNodes.Reverse();
			
			foreach (XmlNode layer in layerNodes)
			{
			    layerIndices[layer.Name] = ++numLayers;
				
			    if (layer.Attributes["tileset"] != null)
				{
					result.Add(OperateOnTilemapLayer(layer));
				}
			    else if (layer.Attributes["exportMode"] != null)
				{
					result.Add(OperateOnGridLayer(layer));
				}
				else
				{
					result.AddRange(OperateOnEntityLayer(layer));
				}
			}
            
            return result.ToArray();
		}
		
		private Entity OperateOnGridLayer(XmlNode layer)
		{
			var type = _gridTypes[layer.Name];

			var contents = layer.InnerText.Split(new string[] {"\n" }, StringSplitOptions.None);
			
			var gridSize = new Vector2i(contents.Length, contents.Length > 0 ? contents[0].Length : 0);
			
			var grid = new Grid(gridSize.X * type.CellWidth, gridSize.Y * type.CellHeight, type.CellWidth, type.CellHeight);
			grid.LoadFromString(layer.InnerText, "", "\n");
			
			var e = new Entity(0, 0, null, grid);
			e.Type = layer.Name;
//			e.Active = e.Visible = false;
			
			return e;
		}
		
		private Entity OperateOnTilemapLayer(XmlNode layer) 
		{
//			var tileset:Tileset = tilesets[new String(layer.@tileset)];
//			
//			if (tileset == null)
//			{
//				return null;
//			}
//			
//			var mapSize:Point = new Point();
//			var lastRow:int = 0;
//			
//			for each (var item:XML in layer.tile)
//			{
//				mapSize.x = Math.max(mapSize.x, item.@x);
//				mapSize.y = Math.max(mapSize.y, item.@y);
//			}
//			
//			++mapSize.x;
//			mapSize.x *= tileset.tileWidth;
//			++mapSize.y;
//			mapSize.y *= tileset.tileHeight;
//			
//			var tilemap:Tilemap = new Tilemap(Library.getImage(tileset.image).bitmapData, mapSize.x, mapSize.y, tileset.tileWidth, tileset.tileHeight);
//			
//			for each (var tile:XML in layer.tile)
//			{
//				tilemap.setTile(tile.@x, tile.@y, tile.@id);
//			}
//			
//			var ent:Entity = new Entity(0, 0, tilemap);
//			ent.layer = numLayers;
//			
//			return ent;
			
			return null;	//	TODO:	Implement this once Tilemap is in.
		}
		
		private Entity[] OperateOnEntityLayer(XmlNode layer)
		{
			var result = new List<Entity>();
			
			foreach (XmlNode entity in layer)
			{
				if (_types.ContainsKey(entity.Name))
				{
					var oclass = _types[entity.Name];
					var e = oclass.CreateInstance(entity);
					e.Layer = numLayers;
					result.Add(e);
				}
			}
			
			return result.ToArray();
		}
		
		#endregion
		
		#region Message broadcasting
		
		/// <summary>
		/// Broadcasts a message to all entities in the world.
		/// </summary>
		/// <param name="message">The name of the message.</param>
		/// <param name="arguments">A set of arguments to pass to the message reciever.</param>
		public void BroadcastMessage(string message, params object[] arguments)
		{
			var e = _updateFirst;
			
			while (e != null)
			{
				e.OnMessage(message, arguments);
				e = e._updateNext;
			}
		}
		
		/// <summary>
		/// Callback type for the BroadcastMessageIf function.
		/// </summary>
		public delegate bool EntityCondition(Entity e);
		
		/// <summary>
		/// Broadcasts a message to all entities that meet a certain criteria.
		/// </summary>
		/// <param name="condition">A callback that returns true if the entity passed to it will recieve the message.</param>
		/// <param name="message">The name of the message.</param>
		/// <param name="arguments">A set of arguments to pass to the message reciever.</param>
		public void BroadcastMessageIf(EntityCondition condition, string message, params object[] arguments)
		{
			var e = _updateFirst;
			
			while (e != null)
			{
				if (condition(e))
				{
					e.OnMessage(message, arguments);
				}
				
				e = e._updateNext;
			}
		}
		
		/// <summary>
		/// Broadcasts a message to all entities with a certain type.
		/// </summary>
		/// <param name="type">The type of entity to recieve the message.</param>
		/// <param name="message">The name of the message.</param>
		/// <param name="arguments">A set of arguments to pass to the message reciever.</param>
		public void BroadcastMessageToType(string type, string message, params object[] arguments)
		{
			if (!_typeFirst.ContainsKey(type))
			{
				return;
			}
			
			var e = _typeFirst[type];
			
			while (e != null)
			{
				e.OnMessage(message, arguments);
				
				e = e._typeNext;
			}
			
		}
		
		/// <summary>
		/// Broadcasts a message to all entities within a certain distance of a position.
		/// </summary>
		/// <param name="x">The X coordinate of the center of the circle.</param>
		/// <param name="y">The Y coordinate of the center of the circle.</param>
		/// <param name="radius">The radius of the circle.</param>
		/// <param name="message">The name of the message.</param>
		/// <param name="arguments">A set of arguments to pass to the message reciever.</param>
		public void BroadcastMessageInCircle(float x, float y, float radius, string message, params object[] arguments)
		{
			var e = _updateFirst;
			
			while (e != null)
			{
				if (e.DistanceToPoint(x, y) <= radius)
				{
					e.OnMessage(message, arguments);
				}
				
				e = e._updateNext;
			}
		}
		
		/// <summary>
		/// Broadcasts a message to all entities within a rectangle.
		/// </summary>
		/// <param name="x">The left of the rectangle.</param>
		/// <param name="y">The top of the rectangle.</param>
		/// <param name="width">The width of the rectangle.</param>
		/// /// <param name="height">The height of the rectangle.</param>
		/// <param name="message">The name of the message.</param>
		/// <param name="arguments">A set of arguments to pass to the message reciever.</param>
		public void BroadcastMessageInRect(float x, float y, float width, float height, string message, params object[] arguments)
		{
			var e = _updateFirst;
			
			while (e != null)
			{
				if (e.CollideRect(e.X, e.Y, x, y, width, height))
				{
					e.OnMessage(message, arguments);
				}
				
				e = e._updateNext;
			}
		}
		
		/// <summary>
		/// Broadcasts a message to all entities of a certain class
		/// </summary>
		/// <param name="message">The name of the message.</param>
		/// <param name="arguments">A set of arguments to pass to the message reciever.</param>
		public void BroadcastMessageToClass<T>(string message, params object[] arguments)
		where T : Entity
		{
			var e = _updateFirst;
			while (e != null)
			{
				if (e is T)
				{
					e.OnMessage(message, arguments);
				}
				
				e = e._updateNext;
			}
		}
		
		
		#endregion
		
		#region Layers
		
		/// <summary>
		/// Brings the Entity to the front of its contained layer.
		/// </summary>
		/// <param name="e">The Entity to shift.</param>
		/// <returns>If the Entity changed position.</returns>
		public bool BringToFront(Entity e)
		{
			if (!ReferenceEquals(this, e._world) || e._renderPrev == null)
			{
				return false;
			}
			
			// pull from list
			e._renderPrev._renderNext = e._renderNext;
			if (e._renderNext != null)
			{
				e._renderNext._renderPrev = e._renderPrev;
			}
			else
			{
				_renderLast[e._layer] = e._renderPrev;
			}
			
			// place at the start
			e._renderNext = _renderFirst[e._layer];
			e._renderNext._renderPrev = e;
			_renderFirst[e._layer] = e;
			e._renderPrev = null;
			return true;
		}
		
		/// <summary>
		/// Sends the Entity to the back of its contained layer.
		/// </summary>
		/// <param name="e">The Entity to shift.</param>
		/// <returns>If the Entity changed position.</returns>
		public bool SendToBack(Entity e)
		{
			if (!ReferenceEquals(this, e._world) || e._renderNext == null)
			{
				return false;
			}
			
			// pull from list
			e._renderNext._renderPrev = e._renderPrev;
			if (e._renderPrev != null)
			{
				e._renderPrev._renderNext = e._renderNext;
			}
			else
			{
				_renderFirst[e._layer] = e._renderNext;
			}
			
			// place at the end
			e._renderPrev = _renderLast[e._layer];
			e._renderPrev._renderNext = e;
			_renderLast[e._layer] = e;
			e._renderNext = null;
			return true;
		}
		
		/// <summary>
		/// Shifts the Entity one place towards the front of its contained layer.
		/// </summary>
		/// <param name="e">The Entity to shift.</param>
		/// <returns>If the Entity changed position.</returns>
		public bool BringForward(Entity e)
		{
			if (!ReferenceEquals(this, e._world) || e._renderPrev == null)
			{
				return false;
			}
			
			// pull from list
			e._renderPrev._renderNext = e._renderNext;
			if (e._renderNext != null)
			{
				e._renderNext._renderPrev = e._renderPrev;
			}
			else
			{
				_renderLast[e._layer] = e._renderPrev;
			}
			
			// shift towards the front
			e._renderNext = e._renderPrev;
			e._renderPrev = e._renderPrev._renderPrev;
			e._renderNext._renderPrev = e;
			if (e._renderPrev != null)
			{
				e._renderPrev._renderNext = e;
			}
			else
			{
				_renderFirst[e._layer] = e;
			}
			return true;
		}
		
		/// <summary>
		/// Shifts the Entity one place towards the back of its contained layer.
		/// </summary>
		/// <param name="e">The Entity to shift.</param>
		/// <returns>If the Entity changed position.</returns>
		public bool SendBackward(Entity e)
		{
			if (!ReferenceEquals(this, e._world) || e._renderNext == null)
			{
				return false;
			}
			
			// pull from list
			e._renderNext._renderPrev = e._renderPrev;
			if (e._renderPrev != null)
			{
				e._renderPrev._renderNext = e._renderNext;
			}
			else
			{
				_renderFirst[e._layer] = e._renderNext;
			}
			
			// shift towards the back
			e._renderPrev = e._renderNext;
			e._renderNext = e._renderNext._renderNext;
			e._renderPrev._renderNext = e;
			if (e._renderNext != null)
			{
				e._renderNext._renderPrev = e;
			}
			else
			{
				_renderLast[e._layer] = e;
			}
			
			return true;
		}
		
		/// <summary>
		/// If the Entity as at the front of its layer.
		/// </summary>
		/// <param name="e">The Entity to check.</param>
		/// <returns>Whether the entity is at front</returns>
		public bool IsAtFront(Entity e)
		{
			return e._renderPrev == null;
		}
		
		/// <summary>
		/// If the Entity as at the back of its layer.
		/// </summary>
		/// <param name="e">The Entity to check.</param>
		/// <returns>Whether the entity is at back</returns>
		public bool IsAtBack(Entity e)
		{
			return e._renderNext == null;
		}
		
		#endregion
		
		#region Collision
		
		/// <summary>
		/// Returns the first Entity that collides with the rectangular area.
		/// </summary>
		/// <param name="type">The Entity type to check for.</param>
		/// <param name="rX">X position of the rectangle.</param>
		/// <param name="rY">Y position of the rectangle.</param>
		/// <param name="rWidth">Width of the rectangle.</param>
		/// <param name="rHeight">Height of the rectangle.</param>
		/// <returns>The first Entity to collide, or null if none collide.</returns>
		public Entity CollideRect(string type, float rX, float rY, float rWidth, float rHeight)
		{
            if (!_typeFirst.ContainsKey(type))
            {
                return null;
            }
			Entity e = _typeFirst[type];
			while (e != null)
			{
				if (e.CollideRect(e.X, e.Y, rX, rY, rWidth, rHeight))
				{
					return e;
				}
				
				e = e._typeNext;
			}
			return null;
		}
		
		/// <summary>
		/// Returns the first Entity found that collides with the position.
		/// </summary>
		/// <param name="type">The Entity type to check for.</param>
		/// <param name="pX">X position.</param>
		/// <param name="pY">Y position.</param>
		/// <returns>The collided Entity, or null if none collide.</returns>
		public Entity CollidePoint(string type, float pX, float pY)
		{
			Entity e = _typeFirst[type];
			while (e != null)
			{
				if (e.CollidePoint(e.X, e.Y, pX, pY))
				{
					return e;
				}
				
				e = e._typeNext;
			}
			return null;
		}
		
		/// <summary>
		/// Returns the first Entity found that collides with the line.
		/// </summary>
		/// <param name="type">The Entity type to check for.</param>
		/// <param name="fromX">Start x of the line.</param>
		/// <param name="fromY">Start y of the line.</param>
		/// <param name="toX">End x of the line.</param>
		/// <param name="toY">End y of the line.</param>
		/// <param name="precision">No clue what this does</param>	//	TODO:	figure out what this does
		/// <returns>The collided Entity, or null if none collide</returns>
		public Entity CollideLine(string type, int fromX, int fromY, int toX, int toY, uint precision = 1)
		{
			// If the distance is less than precision, do the short sweep.
			if (precision < 1) precision = 1;
			if (FP.Distance(fromX, fromY, toX, toY) < precision)
			{
				return CollidePoint(type, fromX, toY);
			}
			
			// Get information about the line we're about to raycast.
			int xDelta = Math.Abs(toX - fromX);
			int yDelta = Math.Abs(toY - fromY);
			float xSign = toX > fromX ? precision : -precision;
			float ySign = toY > fromY ? precision : -precision;
			float x = fromX;
			float y = fromY;
			Entity e;
			
			// Do a raycast from the start to the end point.
			if (xDelta > yDelta)
			{
				ySign *= yDelta / xDelta;
				if (xSign > 0)
				{
					while (x < toX)
					{
						if ((e = CollidePoint(type, x, y)) != null)
						{
							return e;
						}
						
						x += xSign; y += ySign;
					}
				}
				else
				{
					while (x > toX)
					{
						if ((e = CollidePoint(type, x, y)) != null)
						{
							return e;
						}
						
						x += xSign; y += ySign;
					}
				}
			}
			else
			{
				xSign *= xDelta / yDelta;
				if (ySign > 0)
				{
					while (y < toY)
					{
						if ((e = CollidePoint(type, x, y)) != null)
						{
							return e;
						}
						
						x += xSign; y += ySign;
					}
				}
				else
				{
					while (y > toY)
					{
						if ((e = CollidePoint(type, x, y)) != null)
						{
							return e;
						}
						
						x += xSign; y += ySign;
					}
				}
			}
			
			// Check the last position.
			if (precision > 1)
			{
				return CollidePoint(type, toX, toY);
			}
			
			return null;
		}
		
		/// <summary>
		/// Populates an array with all Entities that collide with the rectangle. This
		/// function does not empty the array, that responsibility is left to the user.
		/// </summary>
		/// <param name="type">The Entity type to check for.</param>
		/// <param name="rX">X position of the rectangle.</param>
		/// <param name="rY">Y position of the rectangle.</param>
		/// <param name="rWidth">Width of the rectangle.</param>
		/// <param name="rHeight">Height of the rectangle.</param>
		/// <param name="into">The List to populate with collided Entities.</param>
		public void CollideRectInto(string type, float rX, float rY, float rWidth, float rHeight, List<Entity> into)
		{
			Entity e = _typeFirst[type];
			
			while (e != null)
			{
				if (e.CollideRect(e.X, e.Y, rX, rY, rWidth, rHeight))
				{
					into.Add(e);
				}
				
				e = e._typeNext;
			}
		}
		
		/// <summary>
		/// Populates an array with all Entities that collide with the rectangle. This
		/// function does not empty the array, that responsibility is left to the user.
		/// </summary>
		/// <param name="type">The Entity type to check for.</param>
		/// <param name="pX">X position to check nearby.</param>
		/// <param name="pY">Y position to check nearby.</param>
		/// <param name="into">The List to populate with collided Entities.</param>
		public void CollidePointInto(string type, float pX, float pY, List<Entity> into)
		{
			Entity e = _typeFirst[type];
			int n = into.Count;
			
			while (e != null)
			{
				if (e.CollidePoint(e.X, e.Y, pX, pY))
				{
					into.Add(e);
				}
				
				e = e._typeNext;
			}
		}
		
		#endregion
		
		/// <summary>
		/// Finds the Entity nearest to the rectangle.
		/// </summary>
		/// <param name="type">The Entity type to check for.</param>
		/// <param name="x">X position of the rectangle.</param>
		/// <param name="y">Y position of the rectangle.</param>
		/// <param name="width">Width of the rectangle.</param>
		/// <param name="height">Height of the rectangle.</param>
		/// <param name="ignore">Ignore this entity.</param>
		/// <returns>The nearest Entity to the rectangle.</returns>
		public Entity NearestToRect(string type, float x, float y, float width, float height, Entity ignore = null)
		{
			Entity n = _typeFirst[type];
			float nearDist = float.MaxValue;
			Entity near = null;
			float dist;
			
			while (n != null)
			{
				if (!ReferenceEquals(n, ignore))
				{
					dist = SquareRects(x, y, width, height, n.X - n.OriginX, n.Y - n.OriginY, n.Width, n.Height);
					
					if (dist < nearDist)
					{
						nearDist = dist;
						near = n;
					}
				}
				n = n._typeNext;
			}
			
			return near;
		}
		
		/// <summary>
		/// Finds the Entity nearest to another.
		/// </summary>
		/// <param name="type">The Entity type to check for.</param>
		/// <param name="e">The Entity to find the nearest to.</param>
		/// <param name="useHitboxes">If the Entities' hitboxes should be used to determine the distance. If false, their x/y coordinates are used.</param>
		/// <returns>The nearest Entity to e.</returns>
		public Entity NearestToEntity(string type, Entity e, bool useHitboxes = false)
		{
			if (useHitboxes)
			{
				return NearestToRect(type, e.X - e.OriginX, e.Y - e.OriginY, e.Width, e.Height);
			}
			
			Entity n = _typeFirst[type];
			float nearDist = float.MaxValue;
			Entity near = null;
			float dist;
			float x = e.X - e.OriginX;
			float y = e.Y - e.OriginY;
			
			while (n != null)
			{
				if (!ReferenceEquals(n, e))
				{
					dist = (x - n.X) * (x - n.X) + (y - n.Y) * (y - n.Y);
					if (dist < nearDist)
					{
						nearDist = dist;
						near = n;
					}
				}
				n = n._typeNext;
			}
			
			return near;
		}
		
		/// <summary>
		/// Finds the Entity nearest to the position.
		/// </summary>
		/// <param name="type">The Entity type to check for.</param>
		/// <param name="x">X position.</param>
		/// <param name="y">Y position.</param>
		/// <param name="useHitboxes">If the Entities' hitboxes should be used to determine the distance. If false, their x/y coordinates are used.</param>
		/// <returns>The nearest Entity to the position.</returns>
		public Entity NearestToPoint(string type, float x, float y, bool useHitboxes = false)
		{
			Entity n = _typeFirst[type];
			float nearDist = float.MaxValue;
			Entity near = null;
			float dist;
			
			if (useHitboxes)
			{
				while (n != null)
				{
					dist = SquarePointRect(x, y, n.X - n.OriginX, n.Y - n.OriginY, n.Width, n.Height);
					if (dist < nearDist)
					{
						nearDist = dist;
						near = n;
					}
					n = n._typeNext;
				}
				
				return near;
			}
			
			while (n != null)
			{
				dist = (x - n.X) * (x - n.X) + (y - n.Y) * (y - n.Y);
				if (dist < nearDist)
				{
					nearDist = dist;
					near = n;
				}
				n = n._typeNext;
			}
			return near;
		}
		
		#region Count
		
		/// <summary>
		/// How many Entities are in the World.
		/// </summary>
		public uint Count
		{
			get
			{
				return _count;
			}
		}
		
		/// <summary>
		/// Returns the amount of Entities of the type are in the World.
		/// </summary>
		/// <param name="type">The type to count.</param>
		/// <returns>How many Entities of type exist in the World.</returns>
		public uint TypeCount(string type)
		{
			if (!_typeCount.ContainsKey(type))
				return 0;
			
			return _typeCount[type];
		}
		
		//	TODO:	FP has classCount. Can we have this? Do we need it?
		
		/// <summary>
		/// Returns the amount of Entities are on the layer in the World.
		/// </summary>
		/// <param name="layer">The layer to count Entities on.</param>
		/// <returns>How many Entities are on the layer.</returns>
		public uint LayerCount(int layer)
		{
			if (!_layerCount.ContainsKey(layer))
				return 0;
			
			return (uint) _layerCount[layer];
		}
		#endregion
		
		/// <summary>
		/// The first Entity in the World.
		/// </summary>
		public Entity First
		{
			get
			{
				return _updateFirst;
			}
		}
		
		/// <summary>
		/// How many Entity layers the World has.
		/// </summary>
		public uint Layers
		{
			get
			{
				return (uint) _layerList.Count;
			}
		}
		
		/// <summary>
		/// The first Entity of the type.
		/// </summary>
		/// <param name="type">The type to check.</param>
		/// <returns>The Entity.</returns>
		public Entity TypeFirst(string type)
		{
			if (_updateFirst == null)
			{
				return null;
			}
			
			if (!_typeFirst.ContainsKey(type))
				return null;
			
			return _typeFirst[type] as Entity;
		}
		
		//	TODO:	FP has ClassFirst. Can/Need etc
		
		/// <summary>
		/// The first Entity on the layer.
		/// </summary>
		/// <param name="layer">The layer to check.</param>
		/// <returns>The Entity.</returns>
		public Entity LayerFirst(int layer)
		{
			if (_updateFirst == null)
			{
				return null;
			}
			
			if (!_renderFirst.ContainsKey(layer))
				return null;
			
			return _renderFirst[layer] as Entity;
		}
		
		/// <summary>
		/// The last Entity on the layer.
		/// </summary>
		/// <param name="layer">The layer to check.</param>
		/// <returns>The Entity.</returns>
		public Entity LayerLast(int layer)
		{
			if (_updateFirst == null)
			{
				return null;
			}
			
			if (!_renderLast.ContainsKey(layer))
				return null;
			
			return _renderLast[layer] as Entity;
		}
		
		/// <summary>
		/// The Entity that will be rendered first by the World.
		/// </summary>
		public Entity Farthest
		{
			get
			{
				if (_updateFirst == null)
				{
					return null;
				}
				
				return _renderLast[(int) _layerList[_layerList.Count - 1]] as Entity;
			}
		}
		
		/// <summary>
		/// The Entity that will be rendered last by the World.
		/// </summary>
		public Entity Nearest
		{
			get
			{
				if (_updateFirst == null)
				{
					return null;
				}
				
				return _renderFirst[(int) _layerList[0]] as Entity;
			}
		}
		
		/// <summary>
		/// How many different types have been added to the World.
		/// </summary>
		public uint UniqueTypes
		{
			get
			{
				//	TODO:	return the type count
				return 0;
			}
		}
		
		/// <summary>
		/// Pushes all Entities in the World of the type into the Array or Vector.
		/// </summary>
		/// <param name="type">The type to check.</param>
		/// <param name="into">The List to populate</param>
		public void GetType(string type, List<Entity> into)
		{
			if (!_typeFirst.ContainsKey(type))
				return;
			
			Entity e = _typeFirst[type];
			
			while (e != null)
			{
				into.Add(e);
				e = e._typeNext;
			}
		}
		
		//	TODO:	GetClass, can, need etc
		
		/// <summary>
		/// Pushes all Entities in the World on the layer into the Array or Vector.
		/// </summary>
		/// <param name="layer">The layer to check.</param>
		/// <param name="into">The List to populate</param>
		public void GetLayer(int layer, List<Entity> into)
		{
			if (!_renderLast.ContainsKey(layer))
				return;
			
			Entity e = _renderLast[layer];
			
			while (e != null)
			{
				into.Add(e);
				e = e._renderPrev;
			}
		}
		
		/// <summary>
		/// Pushes all Entities in the World into the Array or Vector.
		/// </summary>
		/// <param name="into">The List to populate</param>
		public void GetAll(List<Entity> into)
		{
			Entity e = _updateFirst;
			
			while (e != null)
			{
				into.Add(e);
				e = e._updateNext;
			}
		}
		
		/// <summary>
		/// Returns the Entity with the instance name, or null if none exists.
		/// </summary>
		/// <param name="name">Instance name of the Entity.</param>
		/// <returns>The named Entity, or null if it doesn't exist.</returns>
		public Entity GetInstance(string name)
		{
			if (_entityNames.ContainsKey(name))
			{
				return _entityNames[name];
			}
			
			return null;
		}
		
		
		/// <summary>
		/// Updates the add/remove lists at the end of the frame.
		/// </summary>
		public void UpdateLists()
		{
			var removeBuffer = new List<Entity>(_remove);
			var addBuffer = new List<Entity>(_add);
			
			// remove entities
			if (removeBuffer.Count > 0)
			{
				foreach (Entity e in removeBuffer)
				{
					if (e._world == null)
					{
						if(_add.IndexOf(e) >= 0)
							_add.Remove(e);
						
						continue;
					}
					if (!ReferenceEquals(e._world, this))
						continue;
					
					e.Removed();
					e._world = null;
					
					RemoveUpdate(e);
					RemoveRender(e);
					_remove.Remove(e);
					if (!String.IsNullOrEmpty(e._type)) RemoveType(e);
					if (!String.IsNullOrEmpty(e._name)) UnregisterName(e);
					if (e.AutoClear && e._tween != null) e.ClearTweens();
				}
				
				removeBuffer.Clear();
			}
			
			// add entities
			if (addBuffer.Count > 0)
			{
				foreach (Entity e in addBuffer)
				{
					if (e._world != null)
						continue;
					
					AddUpdate(e);
					AddRender(e);
					if (!String.IsNullOrEmpty(e._type)) AddType(e);
					if (!String.IsNullOrEmpty(e._name)) RegisterName(e);
					
					e._world = this;
					e.Added();
					_add.Remove(e);
				}
				
				addBuffer.Clear();
			}
			
			//	TODO: FP does recycling stuff here, but we don't need that yet
			
			// sort the depth list
			if (_layerSort)
			{
				if (_layerList.Count > 1) _layerList.Sort();	//	FIXME:	Make sure this sorts ascending
				_layerSort = false;
			}
		}
		
		/// <summary>
		/// Adds an Entity to the update list.
		/// </summary>
		/// <param name="e">The Entity to add.</param>
		private void AddUpdate(Entity e)
		{
			// add to update list
			if (_updateFirst != null)
			{
				_updateFirst._updatePrev = e;
				e._updateNext = _updateFirst;
			}
			else
			{
				e._updateNext = null;
			}
			
			e._updatePrev = null;
			_updateFirst = e;
			_count++;
		}
		
		/// <summary>
		/// Removes an Entity from the update list.
		/// </summary>
		/// <param name="e">The Entity to remove.</param>
		private void RemoveUpdate(Entity e)
		{
			// remove from the update list
			if (ReferenceEquals(_updateFirst, e))
			{
				_updateFirst = e._updateNext;
			}
			
			if (e._updateNext != null)
			{
				e._updateNext._updatePrev = e._updatePrev;
			}
			
			if (e._updatePrev != null)
			{
				e._updatePrev._updateNext = e._updateNext;
			}
			
			e._updateNext = e._updatePrev = null;
			
			_count --;
		}
		
		/// <summary>
		/// Adds Entity to the render
		/// </summary>
		/// <param name="e">The Entity to add</param>
		internal void AddRender(Entity e)
		{
			if (!_renderFirst.ContainsKey(e._layer))
			{
				_renderFirst.Add(e._layer, null);
			}
			
			if (!_renderLast.ContainsKey(e._layer))
			{
				_renderLast.Add(e._layer, null);
			}
			
			if (!_layerCount
			    .ContainsKey(e._layer))
			{
				_layerCount.Add(e._layer, 0);
			}
					
			
			Entity f = _renderFirst[e._layer];
			
			if (f != null)
			{
				// Append entity to existing layer.
				e._renderNext = f;
				f._renderPrev = e;
				_layerCount[e._layer]++;
			}
			else
			{
				// Create new layer with entity.
				_renderLast[e._layer] = e;
				_layerList.Add(e._layer);
				_layerSort = true;
				e._renderNext = null;
				_layerCount[e._layer] = 1;
			}
			_renderFirst[e._layer] = e;
			e._renderPrev = null;
		}
	
		/// <summary>
		/// Removes Entity from the render list.
		/// </summary>
		/// <param name="e">The Entity to remove</param>
		internal void RemoveRender(Entity e)
		{
			if (e._renderNext != null)
			{
				e._renderNext._renderPrev = e._renderPrev;
			}
			else
			{
				_renderLast[e._layer] = e._renderPrev;
			}
			
			if (e._renderPrev != null)
			{
				e._renderPrev._renderNext = e._renderNext;
			}
			else
			{
				// Remove this entity from the layer.
				_renderFirst[e._layer] = e._renderNext;
				
				if (e._renderNext == null)
				{
					// Remove the layer from the layer list if this was the last entity.
					if (_layerList.Count > 1)
					{
						_layerList.Remove(e._layer);
						//	FIXME:	Make sure I did this right	_layerList[_layerList.IndexOf(e._layer)] = _layerList[_layerList.Count - 1];
						_layerSort = true;
					}
				}
			}
			_layerCount[e._layer]--;
			e._renderNext = e._renderPrev = null;
		}
		
		/// <summary>
		/// Adds Entity to the type list.
		/// </summary>
		/// <param name="e">The Entity to add</param>
		internal void AddType(Entity e)
		{
			// add to type list
			if (_typeFirst.ContainsKey(e.Type))
			{
				_typeFirst[e._type]._typePrev = e;
				e._typeNext = _typeFirst[e._type];
				_typeCount[e._type] ++;
			}
			else
			{
				e._typeNext = null;
				_typeCount[e._type] = 1;
			}
			
			e._typePrev = null;
			_typeFirst[e._type] = e;
		}
		
		/// <summary>
		/// Removes Entity from the type list.
		/// </summary>
		/// <param name="e">The Entity to remove</param>
		internal void RemoveType(Entity e)
		{
			// remove from the type list
			if (_typeFirst[e._type] == e) _typeFirst[e._type] = e._typeNext;
			if (e._typeNext != null) e._typeNext._typePrev = e._typePrev;
			if (e._typePrev != null) e._typePrev._typeNext = e._typeNext;
			e._typeNext = e._typePrev = null;
			uint count = --_typeCount[e._type];
			
			if (count <= 0)
			{
				_typeFirst.Remove(e._type);
			}
		}
		
		/// <summary>
		/// Register's the Entity's instance name.
		/// </summary>
		/// <param name="e"></param>
		internal void RegisterName(Entity e)
		{
			_entityNames.Add(e._name, e);
		}
		
		/// <summary>
		/// Unregister's the Entity's instance name.
		/// </summary>
		/// <param name="e"></param>
		internal void UnregisterName(Entity e)
		{
			_entityNames.Remove(e._name);
		}
		
		/// <summary>
		/// Calculates the squared distance between two rectangles.
		/// </summary>
		/// <param name="x1"></param>
		/// <param name="y1"></param>
		/// <param name="w1"></param>
		/// <param name="h1"></param>
		/// <param name="x2"></param>
		/// <param name="y2"></param>
		/// <param name="w2"></param>
		/// <param name="h2"></param>
		/// <returns></returns>
		private static float SquareRects(float x1, float y1, float w1, float h1, float x2, float y2, float w2, float h2)
		{
			if (x1 < x2 + w2 && x2 < x1 + w1)
			{
				if (y1 < y2 + h2 && y2 < y1 + h1) return 0;
				if (y1 > y2) return (y1 - (y2 + h2)) * (y1 - (y2 + h2));
				return (y2 - (y1 + h1)) * (y2 - (y1 + h1));
			}
			if (y1 < y2 + h2 && y2 < y1 + h1)
			{
				if (x1 > x2) return (x1 - (x2 + w2)) * (x1 - (x2 + w2));
				return (x2 - (x1 + w1)) * (x2 - (x1 + w1));
			}
			if (x1 > x2)
			{
				if (y1 > y2) return SquarePoints(x1, y1, (x2 + w2), (y2 + h2));
				return SquarePoints(x1, y1 + h1, x2 + w2, y2);
			}
			if (y1 > y2) return SquarePoints(x1 + w1, y1, x2, y2 + h2);
			return SquarePoints(x1 + w1, y1 + h1, x2, y2);
		}
		
		/// <summary>
		/// Calculates the squared distance between two points.
		/// </summary>
		/// <param name="x1"></param>
		/// <param name="y1"></param>
		/// <param name="x2"></param>
		/// <param name="y2"></param>
		/// <returns></returns>
		private static float SquarePoints(float x1, float y1, float x2, float y2)
		{
			return (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2);
		}
		
		/// <summary>
		/// Calculates the squared distance between a rectangle and a point.
		/// </summary>
		/// <param name="px"></param>
		/// <param name="py"></param>
		/// <param name="rx"></param>
		/// <param name="ry"></param>
		/// <param name="rw"></param>
		/// <param name="rh"></param>
		/// <returns></returns>
		private static float SquarePointRect(float px, float py, float rx, float ry, float rw, float rh)
		{
			if (px >= rx && px <= rx + rw)
			{
				if (py >= ry && py <= ry + rh) return 0;
				if (py > ry) return (py - (ry + rh)) * (py - (ry + rh));
				return (ry - py) * (ry - py);
			}
			if (py >= ry && py <= ry + rh)
			{
				if (px > rx) return (px - (rx + rw)) * (px - (rx + rw));
				return (rx - px) * (rx - px);
			}
			if (px > rx)
			{
				if (py > ry) return SquarePoints(px, py, rx + rw, ry + rh);
				return SquarePoints(px, py, rx + rw, ry);
			}
			if (py > ry) return SquarePoints(px, py, rx, ry + rh);
			return SquarePoints(px, py, rx, ry);
		}
		
		#region Adding and removal.
		private List<Entity> _add;
		private List<Entity> _remove;
		//	TODO:	if we need it	private List<Entity> _recycle;
		#endregion
		
		#region Update information.
		private Entity _updateFirst;
		private uint _count;
		#endregion
		
		#region Render information.
		internal Dictionary<string, Entity> _typeFirst;
		internal Dictionary<string, uint> _typeCount;
		private Dictionary<int, Entity> _renderFirst;
		private Dictionary<int, Entity> _renderLast;
		private List<int> _layerList;
		private Dictionary<int, int> _layerCount;
		private bool _layerSort;
		#endregion
		
		internal Dictionary<string, Entity> _entityNames;
		
		#region Ogmo stuff
		internal Dictionary<string, OgmoClass> _types;
		internal Dictionary<string, GridSize> _gridTypes;
		internal int numLayers;
		#endregion
		
		/// <summary>
		/// Whether to render the world.
		/// </summary>
		public bool Visible;
		
		/// <summary>
		/// Whether Ogmo can load the world (to avoid loading entities twice).
		/// </summary>
		private bool CanLoad;
	}
}
