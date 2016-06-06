
namespace System {
	namespace Drawing {

		public class _SizeBase<T> {
			public T Width;
			public T Height;

			public _SizeBase() { }
			public _SizeBase(T width, T height) {
				Width = width;
				Height = height;
			}
		}

		public class Size : _SizeBase<int> {
			public Size() {
				Width = 0;
				Height = 0;
			}
		}

		public class SizeF : _SizeBase<float> {
			public SizeF() {
				Width = 0f;
				Height = 0f;
			}
		}


		public class _PointBase<T> {
			public T X;
			public T Y;

			public _PointBase() { }
			public _PointBase(T x, T y) {
				X = x;
				Y = y;
			}
		}

		public class Point : _PointBase<int> {
		}

		public class PointF : _PointBase<float> {
		}

		public class _RectBase<T, TPoint, TSize> 
			where TPoint : _PointBase<T>, new()
			where TSize : _SizeBase<T>, new()
		{
			private TPoint point_; 
			private TSize size_;

			public _RectBase() { }
			public _RectBase(T x, T y, T w, T h) {
				point_ = new TPoint {
					X = x,
					Y = y,
				};
				size_ = new TSize {
					Width = w,
					Height = h,
				};
			}
		}

		public class Rectangle : _RectBase<int, Point, Size> {
		}

		public class RectangleF : _RectBase<float, PointF, SizeF> {
		}

		public class Color {

		}

	}
}