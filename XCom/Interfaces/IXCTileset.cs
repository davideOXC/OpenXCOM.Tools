using System;
using System.IO;

using XCom.Interfaces.Base;


namespace XCom.Interfaces
{
	public class IXCTileset // TODO: cTor has inheritors and calls a virtual function.
		:
		ITileset
	{
		protected Palette myPal;
		public Palette Palette
		{
			get { return myPal; }
			set { myPal = value; }
		}

		protected string rootPath;
		public string MapPath
		{
			get { return rootPath; }
			set { rootPath = value; }
		}

		protected string rmpPath;
		public string RmpPath
		{
			get { return rmpPath; }
			set { rmpPath = value; }
		}

		protected string blankPath;
		public string BlankPath
		{
			get { return blankPath; }
			set { blankPath = value; }
		}

		protected string[] groundMaps;
		public string[] Ground // TODO: return a collection or make it a method.
		{
			get { return groundMaps; }
		}

		protected bool underwater;
		public bool Underwater
		{
			get { return underwater; }
		}

		protected bool baseStyle;
		public bool BaseStyle
		{
			get { return baseStyle; }
		}

		protected int mapDepth;
		public int Depth
		{
			get { return mapDepth; }
		}

		protected MapSize mapSize;
		public MapSize Size
		{
			get { return mapSize; }
		}

		protected string scanFile;

		protected string loftFile;



		protected IXCTileset(string name)
			:
			base(name)
		{
			myPal = GameInfo.DefaultPalette;
			mapSize = new MapSize(60, 60, 4);
			mapDepth = 0;
			underwater = true;
			baseStyle = false;
		}

		protected IXCTileset(string name, StreamReader sr, VarCollection vars)
			:
			this(name)
		{
			//LogFile.WriteLine("");
			//LogFile.WriteLine("[7]IXCTileset cTor");
			while (sr.Peek() != -1)
			{
				string line = VarCollection.ReadLine(sr, vars);
				//LogFile.WriteLine(". [7]line= " + line);

				if (line.ToUpperInvariant() == "END")
				{
					//LogFile.WriteLine(". . [7]Exit.");
					return;
				}

				int pos    = line.IndexOf(':');
				string key = line.Substring(0, pos);
				string val = line.Substring(pos + 1);

				//LogFile.WriteLine(". [7]pos= " + pos);
				//LogFile.WriteLine(". [7]key= " + key);
				//LogFile.WriteLine(". [7]val= " + val);

				switch (key.ToUpperInvariant())
				{
					case "PALETTE":
						switch (val.ToUpperInvariant())
						{
							case "UFO":  myPal = Palette.UFOBattle;       break;
							case "TFTD": myPal = Palette.TFTDBattle;      break;
							default:     myPal = Palette.GetPalette(val); break;
						}
						break;

					case "DLL":
						string dll = val.Substring(val.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
						Console.WriteLine(name + " is in dll " + dll);
						break;

					case "ROOTPATH":
						rootPath = val;
						break;

					case "RMPPATH":
						rmpPath = val;
						break;

					case "BASESTYLE":
						baseStyle = true;
						break;

					case "GROUND":
						groundMaps = val.Split(' ');
						break;

					case "SIZE":
						string[] dim = val.Split(',');
						int rows   = int.Parse(dim[0], System.Globalization.CultureInfo.InvariantCulture);
						int cols   = int.Parse(dim[1], System.Globalization.CultureInfo.InvariantCulture);
						int height = int.Parse(dim[2], System.Globalization.CultureInfo.InvariantCulture);

						mapSize = new MapSize(rows, cols, height);
						break;

					case "LANDMAP":
						underwater = false;
						break;

					case "DEPTH":
						mapDepth = int.Parse(val, System.Globalization.CultureInfo.InvariantCulture);
						break;

					case "BLANKPATH":
						blankPath = val;
						break;

					case "SCANG":
						scanFile = val;
						break;

					case "LOFTEMP":
						loftFile = val;
						break;

					default:
						// user-defined keyword
						ParseLine(key, val, sr, vars); // FIX: "Virtual member call in a constructor."
						break;
				}
			}
		}


		public virtual void Save(StreamWriter sw, VarCollection vars)
		{}

		public virtual void ParseLine(
				string key,
				string line,
				StreamReader sr,
				VarCollection vars)
		{}

		public virtual void AddMap(string name, string subset)
		{}

		public virtual void AddMap(XCMapDesc desc, string subset)
		{}

		public virtual XCMapDesc RemoveMap(string name, string subset)
		{
			return null;
		}
	}
}
