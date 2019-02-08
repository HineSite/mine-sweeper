using System.Drawing;

namespace OpenTkPlay
{
   public static partial class Extensions
   {
      public static bool IntersectsWith(this Rectangle rect, Point point)
      {
         return
            point.X >= rect.Location.X && point.Y >= rect.Location.Y &&
            point.X <= (rect.Location.X + rect.Width) && point.Y <= (rect.Location.Y + rect.Height);
      }

      public static bool IntersectsWith(this RectangleF rect, Point point)
      {
         return
            point.X >= rect.Location.X && point.Y >= rect.Location.Y &&
            point.X <= (rect.Location.X + rect.Width) && point.Y <= (rect.Location.Y + rect.Height);
      }

      public static bool IntersectsWith(this RectangleF rect, PointF point)
      {
         return
            point.X >= rect.Location.X && point.Y >= rect.Location.Y &&
            point.X <= (rect.Location.X + rect.Width) && point.Y <= (rect.Location.Y + rect.Height);
      }

      public static PointF Normalize(this PointF point, float minXValue, float maxXValue, float minYValue, float maxYValue)
      {
         return new PointF(
            -(1 - ((point.X - minXValue) * 2 / (maxXValue - minYValue))),
            (1 - ((point.Y - minYValue) * 2 / (maxYValue - minYValue)))
         );
      }

      public static PointF Offset(this PointF point, float offsetX, float offsetY)
      {
         return new PointF(point.X + offsetX, point.Y + offsetY);
      }
   }
}
