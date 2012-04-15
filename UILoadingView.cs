using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace MonoTouch.Extensions {
	
	// Shamelessly copied from http://mymonotouch.wordpress.com/2011/01/27/modal-loading-dialog/
	public class UILoadingView : UIAlertView {
		
		private UIActivityIndicatorView _activityView;
		
		public UILoadingView (string title) {
			Title = title;
		}
		
		public new void Show() {
			base.Show();
			// Spinner – add after Show() or we have no Bounds.
			_activityView = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge);
			_activityView.Frame = new RectangleF((Bounds.Width / 2) - 15, Bounds.Height - 50, 30, 30);
			_activityView.StartAnimating();
			//Subviews[2] = _activityView;
			AddSubview(_activityView);
		}
		
		public void Hide() {
			DismissWithClickedButtonIndex(0, true);
		}
	}
}

