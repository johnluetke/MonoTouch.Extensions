using System;
using MonoTouch.UIKit;

namespace MonoTouch.Extended {
	
	public static class UIView {
		
		/// <summary>
		/// Traverses up the Responder heirarchy of a UIView to find the first controller.
		/// </summary>
		/// <remarks>
		/// Thanks to Phil M - http://stackoverflow.com/questions/1340434/get-to-uiviewcontroller-from-uiview-on-iphone
		/// </remarks>
		/// <param name="view">The UIView to start traversing upwards from.</param>
		/// <returns>A UIViewController if one is found. Null if not.</returns>
		public static UIViewController TraverseResponderChainForUIViewController(this MonoTouch.UIKit.UIView view) {
			
			UIResponder r = view.NextResponder;
			
			if (r is UIViewController) return (UIViewController)r;
			else if (r is MonoTouch.UIKit.UIView) return ((MonoTouch.UIKit.UIView)r).TraverseResponderChainForUIViewController();
			else return null;
		}
	}
}

