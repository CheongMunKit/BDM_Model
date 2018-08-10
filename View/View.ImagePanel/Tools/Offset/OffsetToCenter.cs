using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Vision.View.ImagePanel
{
	public class OffsetToCenter
	{
		public Point Offset(Point original, Point Center)
		{
			Point PointAfterOffset =
				new Point((original.X + Center.X), (original.Y + Center.Y));
			return PointAfterOffset;
		}
	}
}
