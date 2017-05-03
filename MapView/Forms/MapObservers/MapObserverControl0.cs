using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

//using DSShared.Windows;

using XCom.Interfaces.Base;


namespace MapView
{
	/// <summary>
	/// Parent for TopView, TileView, RouteView.
	/// </summary>
	internal class MapObserverControl0
		:
			UserControl,
			IMapObserver
	{
		#region IMapObserver requirements

		private readonly Dictionary<string, IMapObserver> _observersDictionary = new Dictionary<string, IMapObserver>();
		public Dictionary<string, IMapObserver> MoreObservers
		{
			get { return _observersDictionary; }
		}

		private XCMapBase _mapBase;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual XCMapBase MapBase
		{
			get { return _mapBase; }
			set
			{
				_mapBase = value;
				Refresh();
			}
		}

//		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
//		public RegistryInfo RegistryInfo
//		{ get; set; }

/*		private RegistryInfo _regInfo;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RegistryInfo RegistryInfo
		{
			get { return _regInfo; } // NOTE: not used. Only satisfies IMapObserver requirement.
			set
			{
				_regInfo = value;
//				value.RegistryLoadEvent += (sender, e) => OnExtraRegistrySettingsLoad(e);
//				value.RegistrySaveEvent += (sender, e) => OnExtraRegistrySettingsSave(e);
			}
		} */

		/// <summary>
		/// Satisfies IMapObserver. Used by RouteView.
		/// </summary>
		/// <param name="args"></param>
		public virtual void OnLocationSelected_Observer(LocationSelectedEventArgs args)
		{
			//XCom.LogFile.WriteLine("");
			//XCom.LogFile.WriteLine("MapObserverControl0.OnLocationSelected_Observer");

			Refresh();
		}

		/// <summary>
		/// Satisfied IMapObserver.
		/// </summary>
		/// <param name="args"></param>
		public virtual void OnLevelChanged_Observer(LevelChangedEventArgs args)
		{
			Refresh();
		}

		#endregion


		internal Settings Settings
		{ get; set; }


		/// <summary>
		/// Invoked by TopView, TileView, RouteView.
		/// </summary>
		public MapObserverControl0()
		{
			Settings = new Settings();
		}


		internal protected virtual void LoadControl0Settings()
		{}

/*		/// <summary>
		/// Currently implemented only to load TopView's visible quadrants menu.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnExtraRegistrySettingsLoad(RegistryEventArgs e)
		{}
		/// <summary>
		/// Currently implemented only to save TopView's visible quadrants menu.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnExtraRegistrySettingsSave(RegistryEventArgs e)
		{} */

		/// <summary>
		/// Scrolls the z-axis for TopView and RouteView.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			if      (e.Delta < 0) _mapBase.Up();
			else if (e.Delta > 0) _mapBase.Down();
		}
	}
}
