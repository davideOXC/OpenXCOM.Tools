using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;

using PckView.Args;
using PckView.Panels;

using XCom;
using XCom.Interfaces;


namespace PckView
{
	internal delegate void MouseClickedEventHandler(object sender, PckViewMouseEventArgs e);
	internal delegate void MouseMovedEventHandler(int pixels);


	internal sealed class ViewPck
		:
			Panel
	{
		private XCImageCollection _collection;

		private const int Pad = 2;

//		private Color _goodColor = Color.FromArgb(204, 204, 255);
//		private SolidBrush _goodBrush = new SolidBrush(Color.FromArgb(204, 204, 255));

		private int _moveX;
		private int _moveY;
		private int _startY;

		private readonly List<ViewPckItem> _selectedItems;

		internal event MouseClickedEventHandler MouseClickedEvent;
		internal event MouseMovedEventHandler MouseMovedEvent;


		internal ViewPck()
		{
			Paint += OnPaint;

			SetStyle(ControlStyles.OptimizedDoubleBuffer
				   | ControlStyles.AllPaintingInWmPaint
				   | ControlStyles.UserPaint
				   | ControlStyles.ResizeRedraw, true);

			MouseDown += OnMouseDown;
			MouseMove += OnMouseMove;

			_startY = 0;

			_selectedItems = new List<ViewPckItem>();
		}


/*		/// <summary>
		/// Saves a bitmap as an 8-bit image.
		/// </summary>
		/// <param name="file"></param>
		/// <param name="pal"></param>
		public void SaveBMP(string file, Palette pal)
		{
			Bmp.SendToSaver(file, _collection, pal, numAcross(), 1);
		} */

/*		internal void Hq2x()
		{
			_collection.HQ2X();
		} */

		internal Palette Pal
		{
			get { return _collection.Pal; }
			set
			{
				if (_collection != null)
					_collection.Pal = value;
			}
		}

		internal int StartY
		{
			set
			{
				_startY = value;
				Refresh();
			}
		}

		internal int PreferredHeight
		{
			get { return (_collection != null) ? CalculateHeight()
											   : 0; }
		}

		internal XCImageCollection Collection
		{
			get { return _collection; }
			set
			{
				if ((_collection = value) != null)
					Height = CalculateHeight();

				OnMouseDown(null, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
				OnMouseMove(null, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));

				Refresh();
			}
		}

		internal ReadOnlyCollection<ViewPckItemImage> SelectedItems
		{
			get
			{
				if (_collection != null)
				{
					var list = new List<ViewPckItemImage>();
					foreach (var selectedItem in _selectedItems)
					{
						var item = new ViewPckItemImage();
						item.Item = selectedItem;
						item.Image = _collection[GetId(selectedItem)];
						list.Add(item);
					}
					return list.AsReadOnly();
				}
				return null;
			}
		}

		internal void ChangeItem(int id, XCImage image)
		{
			_collection[id] = image;
		}

		private void OnMouseMove(object sender, MouseEventArgs e)
		{
			if (_collection != null)
			{
				int x =  e.X / GetSpecialWidth(_collection.ImageFile.ImageSize.Width);
				int y = (e.Y - _startY) / (_collection.ImageFile.ImageSize.Height + Pad * 2);

				if (x != _moveX || y != _moveY)
				{
					_moveX = x;
					_moveY = y;

					if (_moveX >= PixelsAcross())
						_moveX  = PixelsAcross() - 1;

					if (MouseMovedEvent != null)
						MouseMovedEvent(_moveY * PixelsAcross() + _moveX);
				}
			}
		}

		private void OnMouseDown(object sender, MouseEventArgs e)
		{
			if (_collection != null)
			{
				var x =  e.X / GetSpecialWidth(_collection.ImageFile.ImageSize.Width);
				var y = (e.Y - _startY) / (_collection.ImageFile.ImageSize.Height + Pad * 2);

				if (x >= PixelsAcross())
					x = PixelsAcross() - 1;

				var id = y * PixelsAcross() + x;

				var selected = new ViewPckItem();
				selected.X = x;
				selected.Y = y;
				selected.Index = id;

				if (id < Collection.Count)
				{
					if (ModifierKeys == Keys.Control)
					{
						ViewPckItem existingItem = null;
						foreach (var item in _selectedItems)
						{
							if (item.X == x && item.Y == y)
								existingItem = item;
						}

						if (existingItem != null)
						{
							_selectedItems.Remove(existingItem);
						}
						else
							_selectedItems.Add(selected);
					}
					else
					{
						_selectedItems.Clear();
						_selectedItems.Add(selected);
					}

					Refresh();

					if (MouseClickedEvent != null)
					{
						var args = new PckViewMouseEventArgs(e, id);
						MouseClickedEvent(this, args);
					}
				}
			}
		}

		private void OnPaint(object sender, PaintEventArgs e)
		{
			if (_collection != null && _collection.Count != 0)
			{
				var g = e.Graphics;

				var specialWidth = GetSpecialWidth(_collection.ImageFile.ImageSize.Width);

				foreach (var selectedItem in _selectedItems)
				{
//					if (_collection.ImageFile.FileOptions.BitDepth == 8 && _collection[0].Palette.Transparent.A == 0)
//					{
//						g.FillRectangle(
//									_goodBrush,
//									selectedItem.X * specialWidth - Pad,
//									_startY + selectedItem.Y * (_collection.ImageFile.ImageSize.Height + Pad * 2) - Pad,
//									specialWidth,
//									_collection.ImageFile.ImageSize.Height + Pad * 2);
//					}
//					else
//					{
					g.FillRectangle(
								Brushes.Red,
								selectedItem.X * specialWidth - Pad,
								_startY + selectedItem.Y * (_collection.ImageFile.ImageSize.Height + Pad * 2) - Pad,
								specialWidth,
								_collection.ImageFile.ImageSize.Height + Pad * 2);
//					}
				}

				int across = PixelsAcross();

				for (int i = 0; i < across + 1; ++i)
					g.DrawLine(
							Pens.Black,
							new Point(i * specialWidth - Pad,          _startY),
							new Point(i * specialWidth - Pad, Height - _startY));

				for (int i = 0; i < _collection.Count / across + 1; ++i)
					g.DrawLine(
							Pens.Black,
							new Point(0,     _startY + i * (_collection.ImageFile.ImageSize.Height + Pad * 2) - Pad),
							new Point(Width, _startY + i * (_collection.ImageFile.ImageSize.Height + Pad * 2) - Pad));

				for (int i = 0; i < _collection.Count; ++i)
				{
					int x = i % across;
					int y = i / across;
					try
					{
						g.DrawImage(
								_collection[i].Image, x * specialWidth,
								_startY + y * (_collection.ImageFile.ImageSize.Height + Pad * 2));
					}
					catch {} // TODO: that.
				}
			}
		}

		internal void RemoveSelected()
		{
			if (SelectedItems.Count != 0)
			{
				var lowestIndex = int.MaxValue;

				var indexList = new List<int>();
				foreach (var item in SelectedItems)
					indexList.Add(item.Item.Index);

				indexList.Sort();
				indexList.Reverse();

				foreach (var index in indexList)
				{
					if (index < lowestIndex)
						lowestIndex = index;

					Collection.Remove(index);
				}

				if (lowestIndex > 0 && lowestIndex == Collection.Count)
					lowestIndex = Collection.Count - 1;

				ClearSelection(lowestIndex);
			}
		}

		private void ClearSelection(int lowestIndex)
		{
			_selectedItems.Clear();

			if (Collection.Count != 0)
			{
				var selected = new ViewPckItem();
				selected.Y = lowestIndex / PixelsAcross();
				selected.X = lowestIndex - selected.Y;
				selected.Index = selected.Y * PixelsAcross() + selected.X;

				_selectedItems.Add(selected);
			}
		}

		private int PixelsAcross()
		{
			return Math.Max(
						1,
						(Width - 8) / (_collection.ImageFile.ImageSize.Width + Pad * 2));
		}

		private int CalculateHeight()
		{
			return (_collection.Count / PixelsAcross()) * (_collection.ImageFile.ImageSize.Height + Pad * 2) + 60;
		}

		private int GetId(ViewPckItem selectedItem)
		{
			return selectedItem.X + selectedItem.Y * PixelsAcross();
		}

		private static int GetSpecialWidth(int width)
		{
			return width + Pad * 2;
		}
	}
}
