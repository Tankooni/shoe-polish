/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 5/7/2013
 * Time: 12:33 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Xml;
using Drawing = System.Drawing;
using System.Resources;
using SFML.Graphics;
using SFML.Audio;
using System.Collections.Generic;
using System.IO;

namespace Punk
{
	/// <summary>
	/// Content management and caching.
	/// </summary>
	public static class Library
	{
		internal static void LoadEmbeddedAssets()
		{
			ResourceManager manager = new ResourceManager("Punk.Resources.Embeds", typeof(Engine).Assembly);
			
			var embed = "Punk.Embeds/";
			
			using (MemoryStream stream = new MemoryStream((byte[]) manager.GetObject("DefaultFont")))
			{
				Fonts.Add(embed + "DefaultFont.ttf", new Font(new MemoryStream(stream.ToArray())));
			}
			
			var images = new string[]
			{
				"console_debug",
				"console_logo",
				"console_output",
				"console_pause",
				"console_play",
				"console_step"
			};
			
			foreach (string image in images)
			{
				using (MemoryStream stream = new MemoryStream())
			    {
					Drawing.Bitmap img = (Drawing.Bitmap) manager.GetObject(image);
			        img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
			        stream.Close();
						
			        Textures.Add(embed + image + ".png", new Texture(new MemoryStream(stream.ToArray())));	//	wat
			    }
			}
			
		}
		
		/// <summary>
		/// Load an XML document.
		/// </summary>
		/// <param name="filename">The filename of the document to load.</param>
		/// <returns>The loaded document.</returns>
		public static XmlDocument GetXml(string filename)
		{
			ValidateFilename(filename);
			
			if (XmlDocs.ContainsKey(filename))
			{
				return XmlDocs[filename];
			}
			
			var xml = new XmlDocument();
			xml.Load(filename);
			XmlDocs.Add(filename, xml);
			return xml;
		}
		
		/// <summary>
		/// Load a texture.
		/// </summary>
		/// <param name="filename">The filename of the texture to load.</param>
		/// <returns>The loaded texture.</returns>
		public static Texture GetTexture(string filename)
		{
			ValidateFilename(filename);
			
			if (Textures.ContainsKey(filename))
			{
				return Textures[filename];
			}
			
			Texture texture = new Texture(filename);
			Textures.Add(filename, texture);
			return texture;
		}
		
		/// <summary>
		/// Load sound data.
		/// </summary>
		/// <param name="filename">The filename of the sound file to load.</param>
		/// <returns>The loaded sound file.</returns>
        public static SoundBuffer GetBuffer(string filename)
        {
        	ValidateFilename(filename);
        	
            if (SoundBuffers.ContainsKey(filename))
            {
                return SoundBuffers[filename];
            }

            SoundBuffer buffer = new SoundBuffer(filename);
            SoundBuffers.Add(filename, buffer);
            return buffer;
        }
        
        /// <summary>
        /// Loads a "music" which, internally is a stream.
        /// </summary>
        /// <param name="filename">The filename of the music to load.</param>
        /// <returns>The loaded music file.</returns>
        public static Punk.Music GetMusic(string filename)
        {
        	ValidateFilename(filename);
        	
            if (Songs.ContainsKey(filename))
            {
                return Songs[filename];
            }

            Punk.Music jam = new Punk.Music(filename);
            Songs.Add(filename, jam);
            return jam;
        }
        
        /// <summary>
        /// Load a font.
        /// </summary>
        /// <param name="filename">The filename of the font to load.</param>
        /// <returns>The loaded font.</returns>
        public static Font GetFont(string filename)
        {
        	ValidateFilename(filename);
        	
        	if (Fonts.ContainsKey(filename))
        	{
        		return Fonts[filename];
        	}
        	
        	Font font = new Font(filename);
        	Fonts.Add(filename, font);
        	return font;
        }
        
        /// <summary>
        /// Load a text file and return its contents
        /// </summary>
        /// <param name="filename">The name of the file</param>
        /// <returns>The loaded text.</returns>
        public static string GetText(string filename)
        {
        	ValidateFilename(filename);
        	
        	if (Texts.ContainsKey(filename))
        	{
        		return Texts[filename];
        	}
        	
        	var text = File.ReadAllText(filename);
        	Texts.Add(filename, text);
        	return text;
        }

        internal static Dictionary<string, Texture> Textures = new Dictionary<string, Texture>();
        internal static Dictionary<string, SoundBuffer> SoundBuffers = new Dictionary<string, SoundBuffer>();
        internal static Dictionary<string, Punk.Music> Songs = new Dictionary<string, Punk.Music>();
        internal static Dictionary<string, Font> Fonts = new Dictionary<string, Font>();
        internal static Dictionary<string, XmlDocument> XmlDocs = new Dictionary<string, XmlDocument>();
        internal static Dictionary<string, string> Texts = new Dictionary<string, string>();
		
		public static void Reload()
		{
			var textures = Textures.Keys;
			var xmls = new List<string>(XmlDocs.Keys);
			var texts = new List<string>(Texts.Keys);
			
			foreach (var name in textures)
			{
				try
				{
					var tex = new Texture(name);
					Textures[name].Update(tex.CopyToImage());
				}
				catch (Exception)
				{
					//	got an embedded file; ignore the error
					continue;
				}
			}
			
			foreach (var name in xmls)
			{
				var xml = new XmlDocument();
				xml.Load(name);
				XmlDocs[name] = xml;
			}
			
			foreach (var name in texts)
			{
				Texts[name] = File.ReadAllText(name);
			}
		}
		
		private static void ValidateFilename(string filename)
		{
			if (filename.Contains(@"\"))
			{
				throw new PlatformNotSupportedException("You used a backslash to load assets! BAD! BAAAAD!");
			}
			
		}

	}
}
