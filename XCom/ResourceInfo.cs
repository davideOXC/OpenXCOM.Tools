using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using DSShared;


namespace XCom
{
	public static class ResourceInfo
	{
		public static TileGroupManager TileGroupInfo
		{ get; private set; }

		private static readonly Dictionary<Palette, Dictionary<string, SpriteCollection>> _palSpritesets
						  = new Dictionary<Palette, Dictionary<string, SpriteCollection>>();


		/// <summary>
		/// Initializes/ loads info about XCOM resources.
		/// </summary>
		/// <param name="pathConfig"></param>
		public static void InitializeResources(PathInfo pathConfig)
		{
			Directory.SetCurrentDirectory(pathConfig.DirectoryPath); // change to /settings dir // TODO: screw settings dir.
//			XConsole.Init(20);

			TileGroupInfo = new TileGroupManager(new TilesetManager(pathConfig.FullPath));

			Directory.SetCurrentDirectory(SharedSpace.Instance.GetShare(SharedSpace.ApplicationDirectory)); // change back to app dir
		}

		/// <summary>
		/// Loads a given spriteset for UFO or TFTD. This could go in Descriptor
		/// except the XCOM cursor-sprites load w/out a descriptor. So do the
		/// 'ExtraSprites'.
		/// </summary>
		/// <param name="file"></param>
		/// <param name="path"></param>
		/// <param name="lenTabOffset"></param>
		/// <param name="pal"></param>
		/// <returns></returns>
		public static SpriteCollection LoadSpriteset(
				string file,
				string path,
				int lenTabOffset,
				Palette pal)
		{
			//LogFile.WriteLine("");
			//LogFile.WriteLine("ResourceInfo.LoadSpriteset");

			if (!String.IsNullOrEmpty(path))
			{
				//LogFile.WriteLine(". path= " + path);
				//LogFile.WriteLine(". file= " + file);

				var pf = Path.Combine(path, file);
				//LogFile.WriteLine(". pf= " + pf);

				string pfePck = pf + SpriteCollection.PckExt;
				string pfeTab = pf + SpriteCollection.TabExt;

				if (!File.Exists(pfePck) || !File.Exists(pfeTab))
				{
					MessageBox.Show(
								"Can't find files for spriteset"
									+ Environment.NewLine + Environment.NewLine
									+ pfePck + Environment.NewLine
									+ pfeTab,
								"Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1,
								0);
				}
				else
				{
					if (!_palSpritesets.ContainsKey(pal))
						_palSpritesets.Add(pal, new Dictionary<string, SpriteCollection>());

					var spritesets = _palSpritesets[pal];
					if (!spritesets.ContainsKey(pf))
					{
						//LogFile.WriteLine(". . pf not found in spriteset dictionary -> add new SpriteCollection");

						using (var strPck = File.OpenRead(pfePck))
						using (var strTab = File.OpenRead(pfeTab))
						{
							spritesets.Add(pf, new SpriteCollection(
																strPck,
																strTab,
																lenTabOffset,
																pal));
						}
					}
					return _palSpritesets[pal][pf];
				}
			}
			return null;
		}
	}
}
