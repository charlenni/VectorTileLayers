using SkiaSharp;
using System;

namespace Mapsui.VectorTileLayers.Core.Extensions
{
    public static class SKRectExtensions
    {
		public static SKRect Intersection(this SKRect self, SKRect other) =>
			new SKRect(
				left: Math.Max(self.Left, other.Left),
				top: Math.Max(self.Top, other.Top),
				right: Math.Min(self.Right, other.Right),
				bottom: Math.Min(self.Bottom, other.Bottom)
			);

		public static SKRect Extend(this SKRect self, SKRect other) =>
			new SKRect(
				left: Math.Min(self.Left, other.Left),
				top: Math.Min(self.Top, other.Top),
				right: Math.Max(self.Right, other.Right),
				bottom: Math.Max(self.Bottom, other.Bottom)
			);

		public static bool Intersects(this SKRect self, in SKRect other) =>
			self.Left <= other.Right &&
			self.Top <= other.Bottom &&
			self.Right >= other.Left &&
			self.Bottom >= other.Top;


		public static double Margin(this SKRect self) => Math.Max(self.Right - self.Left, 0) + Math.Max(self.Bottom - self.Top, 0);

		public static double Area(this SKRect self) => Math.Max(self.Right - self.Left, 0) * Math.Max(self.Bottom - self.Top, 0);

	}
}
