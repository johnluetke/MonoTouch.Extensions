/// 
/// MonoTouch.Extensions
/// Extensions.cs 
///
/// Provides extension methods to MonoTouch classes
/// 
/// Author: John Luetke <john@johnluetke.net>
/// 
/// Licensed under the Creative Commons Attribution Non-Commercial license
/// http://creativecommons.org/licenses/by-nc/3.0/
/// 

using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.MapKit;
using MonoTouch.CoreLocation;

namespace MonoTouch.Extensions {
	
	public static class Extensions {
		
		/// <summary>
		/// Traverses up the Responder heirarchy of a UIView to find the first controller.
		/// </summary>
		/// <remarks>
		/// Thanks to Phil M - http://stackoverflow.com/questions/1340434/get-to-uiviewcontroller-from-uiview-on-iphone
		/// </remarks>
		/// <param name="view">The UIView to start traversing upwards from.</param>
		/// <returns>A UIViewController if one is found. Null if not.</returns>
		public static UIViewController TraverseResponderChainForUIViewController(this UIView view) {
			
			UIResponder r = view.NextResponder;
			
			if (r is UIViewController) return (UIViewController)r;
			else if (r is UIView) return ((UIView)r).TraverseResponderChainForUIViewController();
			else return null;
		}
	
		/// <summary>
		/// Zooms in/out to show all annotations on the screen. Does nothing if there are no annotations
		/// </summary>
		/// <param name="map">The MKMapView to consider</param>
		public static void ZoomToFitAnnotations(this MKMapView map) {
			ZoomToFitAnnotations(map, 1.1f);
		}
		
		/// <summary>
		/// ooms in/out to show all annotations on the screen. Does nothing if there are no annotations
		/// </summary>
		/// <param name="map">The MKMapView to consider</param>
		/// <param name="buffer">A buffer to add onto the left and right sides of the resulting region</param>
		public static void ZoomToFitAnnotations(this MKMapView map, float buffer) {
			if (map.Annotations.Length == 0)
				return;
			
			CLLocationCoordinate2D TopLeft = new CLLocationCoordinate2D(90, -180);
			CLLocationCoordinate2D BottomRight = new CLLocationCoordinate2D(-90, 180);
			
			foreach (NSObject a in map.Annotations) {
				TopLeft.Latitude = Math.Min(TopLeft.Latitude, ((MKAnnotation)a).Coordinate.Latitude);
				TopLeft.Longitude = Math.Max(TopLeft.Longitude, ((MKAnnotation)a).Coordinate.Longitude);
				
				BottomRight.Latitude = Math.Max(BottomRight.Latitude, ((MKAnnotation)a).Coordinate.Latitude);
				BottomRight.Longitude = Math.Min(BottomRight.Longitude, ((MKAnnotation)a).Coordinate.Longitude);
			}
			
			MKCoordinateRegion region;
			
			// We found the bounds, now find the center
			region.Center.Latitude = TopLeft.Latitude - (TopLeft.Latitude - BottomRight.Latitude) * 0.5;
			region.Center.Longitude = TopLeft.Longitude + (BottomRight.Longitude - TopLeft.Longitude) * 0.5;
    
			// Add a little extra space on the sides
			region.Span.LatitudeDelta = Math.Abs(TopLeft.Latitude - BottomRight.Latitude) * buffer; 
			region.Span.LongitudeDelta = Math.Abs(BottomRight.Longitude - TopLeft.Longitude) * buffer;
    
			region = map.RegionThatFits(region);
			map.SetRegion(region, true);
		}
		
		/// <summary>
		/// Removes all annotations from the MKMapView
		/// </summary>
		/// <param name="map">The MKMapView to consider</param>
		public static void RemoveAllAnnotations(this MKMapView map) {
			foreach (NSObject a in map.Annotations)
				map.RemoveAnnotation((MKAnnotation)a);
		}
		
		/// <summary>
		/// Zooms to a given location on the map
		/// </summary>
		/// <param name="map">The MKMapView to consider</param>
		/// <param name="coordinate">The coordinate for the map to center on</param>
		/// <param name="buffer">A buffer to ensure around the center</param>
		/// <param name="animated">Whether or not to animate the zoom</param>
		public static void ZoomTo(this MKMapView map, CLLocationCoordinate2D coordinate, float buffer, bool animated) {
			ZoomTo(map, coordinate, new MKCoordinateSpan(buffer, buffer), animated);
		}
		
		/// <summary>
		/// Zooms to a given location on the map
		/// </summary>
		/// <param name="map">The MKMapView to consider</param>
		/// <param name="coordinate">The coordinate for the map to center on</param>
		/// <param name="span">A span to ensure around the center</param>
		/// <param name="animated">Whether or not to animate the zoom</param>
		public static void ZoomTo(this MKMapView map, CLLocationCoordinate2D coordinate, MKCoordinateSpan span, bool animated) {
			// Calculate a region centered at coordinate and buffered by span on all sides
			ZoomTo(map, new MKCoordinateRegion(coordinate, span), animated);
		}
		
		/// <summary>
		/// Zooms to a given location on the map
		/// </summary>
		/// <param name="map">The MKMapView to consider</param>
		/// <param name="region">A region of the map to zoom to</param>
		/// <param name="animated">Whether or not to animate the zoom</param>
		public static void ZoomTo(this MKMapView map, MKCoordinateRegion region, bool animated) {
			map.SetRegion(region, animated);
		}
	}
}

