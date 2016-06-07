using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Foundation;
using CoreAnimation;

namespace Reactive.Bindings.Extensions
{
	public static class ObserveEveryValueChangedExtensions
	{
		/// <summary>
		/// Publish target property when value is changed. If source is destructed, publish OnCompleted.
		/// </summary>
		public static IObservable<TProperty> ObserveEveryValueChanged<TSource, TProperty>(this TSource source, Func<TSource, TProperty> propertySelector, IEqualityComparer<TProperty> comparer = null)
			where TSource : class
		{
			if (source == null) return Observable.Empty<TProperty>();
			comparer = comparer ?? EqualityComparer<TProperty>.Default;

			var reference = new WeakReference(source);
			source = null;

			return Observable.Create<TProperty>(observer =>
			{
				var currentValue = default(TProperty);
				var prevValue = default(TProperty);

				var t = reference.Target;
				if (t != null)
				{
					try
					{
						currentValue = propertySelector((TSource)t);
					}
					catch (Exception ex)
					{
						observer.OnError(ex);
					}
					finally
					{
						t = null;
					}
				}
				else
				{
					observer.OnCompleted();
					return Disposable.Empty;
				}

				observer.OnNext(currentValue);
				prevValue = currentValue;

				var link = CADisplayLink.Create(() => {
					var target = reference.Target;
					if (target != null)
					{
						try
						{
							currentValue = propertySelector((TSource)target);
						}
						catch (Exception ex)
						{
							observer.OnError(ex);
						}
						finally
						{
							target = null;
						}
					}
					else
					{
						observer.OnCompleted();
					}

					if (!comparer.Equals(currentValue, prevValue))
					{
						observer.OnNext(currentValue);
						prevValue = currentValue;
					}
				});
				link.AddToRunLoop(NSRunLoop.Current, NSRunLoop.NSRunLoopCommonModes);

				return new DisposableWrapper(link);
			});
		}

		private class DisposableWrapper : IDisposable
		{
			private CADisplayLink link;

			public DisposableWrapper(CADisplayLink link)
			{
				this.link = link;
			}

			public void Dispose()
			{
				if(link != null)
				{
					link.Invalidate();
					link = null;
				}
			}
		}
	}
}

