using System;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using UIKit;
using CoreAnimation;
using Foundation;

namespace TouchSample
{
	public partial class ViewController : UIViewController
	{
		public ViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			// Perform any additional setup after loading the view, typically from a nib.
			slider.ObserveEveryValueChanged(x => x.Value).Subscribe(x => label.Text = x.ToString());
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}

