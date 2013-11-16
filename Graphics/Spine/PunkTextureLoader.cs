/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 6/6/2013
 * Time: 10:00 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using SFML.Window;
using Spine;

namespace Punk.Graphics.Spine
{
	/// <summary>
	/// Description of PunkTextureLoader.
	/// </summary>
	public class PunkTextureLoader : TextureLoader
	{
		private string folder;
		
		public PunkTextureLoader(string folder)
		{
			this.folder = folder;
		}
		
		public void Load(AtlasPage page, string path)
		{
			//	Spine passes a string made with Path.Combine(), which is dumb. We never need to use backslashes. Ever.
			var filename = path.Replace('\\', '/');
			filename = filename.TrimStart('.');
			filename = string.Format("{0}{1}", folder, filename);
			
			var texture = Library.GetTexture(filename);
			texture.Smooth = true;
			page.rendererObject = texture;
			
			var size = texture.Size;
			page.width = (int) size.X;
			page.height = (int) size.Y;
		}
		
		public void Unload(object texture)
		{
			//	do nothing! :D
		}
	}
}
