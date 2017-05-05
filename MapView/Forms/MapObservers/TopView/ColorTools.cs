using System;
using System.Drawing;


namespace MapView.Forms.MapObservers.TopViews
{
	internal sealed class ColorTools
		:
			IDisposable
	{
		#region Fields
		private readonly Pen _pen;
		private readonly Pen _penLight;
		private readonly SolidBrush _brush;
		private readonly SolidBrush _brushLight;
		#endregion

		#region Properties
		internal Pen Pen
		{
			get { return _pen; }
		}

		internal Pen LightPen
		{
			get { return _penLight; }
		}

		internal Brush Brush
		{
			get { return _brush; }
		}

		internal Brush LightBrush
		{
			get { return _brushLight; }
		}
		#endregion


		#region cTors
		/// <summary>
		/// cTors.
		/// </summary>
		/// <param name="pen"></param>
		internal ColorTools(Pen pen)
		{
			_pen      = pen;
			_penLight = new Pen(Color.FromArgb(70, pen.Color), pen.Width);

			_brush      = new SolidBrush(pen.Color);
			_brushLight = new SolidBrush(Color.FromArgb(70, pen.Color));
		}
		internal ColorTools(SolidBrush brush, float width)
		{
			_pen       = new Pen(brush.Color);
			_pen.Width = width;
			_penLight  = new Pen(Color.FromArgb(50, brush.Color), width);

			_brush      = brush;
			_brushLight = new SolidBrush(Color.FromArgb(50, brush.Color));
		}
		#endregion


		/// <summary>
		/// This isn't really necessary since the Pens and Brushes last the
		/// lifetime of the app. But FxCop gets antsy ....
		/// NOTE: Dispose() is never called. cf DrawBlobService.
		/// </summary>
		public void Dispose()
		{
			_pen.Dispose();
			_penLight.Dispose();
			_brush.Dispose();
			_brushLight.Dispose();

			GC.SuppressFinalize(this);
		}
	}
}
