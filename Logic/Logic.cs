/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 5/24/2013
 * Time: 6:37 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace Punk
{
	/// <summary>
	/// Simple Logical component. Can be added to an Entity to compartmentalize update logic.
	/// </summary>
	public class Logic
	{
		/// <summary>
		/// The Entity that this Logic belongs to.
		/// </summary>
		public Entity Parent;
		
		/// <summary>
		/// The name of this Logic; defaults to the name of the class.
		/// </summary>
        public string Name;
        
        /// <summary>
        /// How many times this Logic has updated since being created.
        /// </summary>
        public uint Timer = 0;
        
        /// <summary>
        /// 
        /// </summary>
        public bool Active = true;
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public Logic()
        {
        	Name = GetType().Name;
        	_responses = new Dictionary<string, Entity.MessageResponse>();
        }
        
        #region Messaging
        /// <summary>
		/// Called when this recieves a message broadcast from the World
		/// </summary>
		/// <param name="message">The name of the message.</param>
		/// <param name="arguments">A set of arguments.</param>
		public virtual void OnMessage(string message, params object[] arguments)
		{
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
		public void AddResponse(string message, Entity.MessageResponse response)
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
		public void RemoveResponse(string message, Entity.MessageResponse response)
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
        /// Updates the Logic.
        /// </summary>
        public virtual void Update()
        {
            if (!Active)
            {
            	return;
            }
             
            Timer++;
        }
        
        /// <summary>
        /// Override this to render debug information in the debug console.
        /// </summary>
        public virtual void RenderDebug()
        {
        }
        
        /// <summary>
        /// Override this; called when this Logic is added to an Entity.
        /// </summary>
        public virtual void Added()
        {
        }
        
		/// <summary>
        /// Override this; called when this Logic is removed from an Entity.
        /// </summary>        
        public virtual void Removed()
        {
        }
        
        /// <summary>
        /// A textual representation of this Logic. Defaults to the name of the class.
        /// </summary>
        /// <returns>A textual representation of this Logic.</returns>
        public override string ToString()
		{
        	return string.Format("[Logic {0}]", Name);
		}
        
        private Dictionary<string, Entity.MessageResponse> _responses;

	}
}
