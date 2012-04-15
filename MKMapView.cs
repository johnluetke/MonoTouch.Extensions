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
	
	public static class MKMapViewExtensions {
	
		/// <summary>
		/// Zooms in/out to show all annotations on the screen. Does nothing if there are no annotations
		/// </summary>
		/// <param name="map">The MKMapView to consider</param>
		public static void ZoomToFitAnnotations(this MonoTouch.MapKit.MKMapView map) {
			ZoomToFitAnnotations(map, 1.1f);
		}
		
		/// <summary>
		/// Zooms in/out to show all annotations on the screen. Does nothing if there are no annotations
		/// </summary>
		/// <param name="map">The MKMapView to consider</param>
		/// <param name="buffer">A buffer to add onto the left and right sides of the resulting region</param>
		public static void ZoomToFitAnnotations(this MonoTouch.MapKit.MKMapView map, float buffer) {
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
		public static void RemoveAllAnnotations(this MonoTouch.MapKit.MKMapView map) {
			foreach (NSObject a in map.Annotations)
				map.RemoveAnnotation((MKAnnotation)a);
		}
		
		public static void RemoveAllOverlays(this MonoTouch.MapKit.MKMapView map) {
			if (map.Overlays == null) return;
			foreach (NSObject a in map.Overlays)
				map.RemoveOverlay(a);
		}
		
		/// <summary>
		/// Zooms to a given location on the map
		/// </summary>
		/// <param name="map">The MKMapView to consider</param>
		/// <param name="coordinate">The coordinate for the map to center on</param>
		/// <param name="buffer">A buffer to ensure around the center</param>
		/// <param name="animated">Whether or not to animate the zoom</param>
		public static void ZoomTo(this MonoTouch.MapKit.MKMapView map, CLLocationCoordinate2D coordinate, float buffer, bool animated) {
			ZoomTo(map, coordinate, new MKCoordinateSpan(buffer, buffer), animated);
		}
		
		/// <summary>
		/// Zooms to a given location on the map
		/// </summary>
		/// <param name="map">The MKMapView to consider</param>
		/// <param name="coordinate">The coordinate for the map to center on</param>
		/// <param name="span">A span to ensure around the center</param>
		/// <param name="animated">Whether or not to animate the zoom</param>
		public static void ZoomTo(this MonoTouch.MapKit.MKMapView map, CLLocationCoordinate2D coordinate, MKCoordinateSpan span, bool animated) {
			// Calculate a region centered at coordinate and buffered by span on all sides
			ZoomTo(map, new MKCoordinateRegion(coordinate, span), animated);
		}
		
		/// <summary>
		/// Zooms to a given location on the map
		/// </summary>
		/// <param name="map">The MKMapView to consider</param>
		/// <param name="region">A region of the map to zoom to</param>
		/// <param name="animated">Whether or not to animate the zoom</param>
		public static void ZoomTo(this MonoTouch.MapKit.MKMapView map, MKCoordinateRegion region, bool animated) {
			map.SetRegion(region, animated);
		}
		
		public static void ZoomToFitOverlay(this MonoTouch.MapKit.MKMapView map, NSObject overlay) {
			if (overlay is MKCircle) {
				MKCircle circle = overlay as MKCircle;
				map.ZoomTo(circle.Coordinate, .01f, true);
				//map.SetCenterCoordinate(circle.Coordinate, true); // doesn't work?
				//map.CenterCoordinate.Latitude = circle.Coordinate.Latitude;
				//map.CenterCoordinate.Longitude = circle.Coordinate.Longitude;
				//map.CenterCoordinate = circle.Coordinate;
				//CLLocationCoordinate2D center = circle.Coordinate;
				//MKMapPoint point = MKMapPoint.FromCoordinate(center);
				//MKMapRect rect = circle.BoundingMap;
				//MKCoordinateSpan span = new MKCoordinateSpan(0, 0);
				//MKCircleView cv = new MKCircleView(circle);
				//cv.MapRectForRect()
				//map.ZoomTo(center, 0.0135f, true);
			}
		} 
	}
}

