using System;
using System.Runtime.InteropServices;
using MonoTouch.UIKit;

namespace MonoTouch.Extensions {
	
	[Flags]
	public enum Device {
		//                            UID   3G? Type
		iPhone           = 0x0101, // 0001 0000 0001
		iPhone3G         = 0x0211, // 0010 0001 0001
		iPhone3GS        = 0x0311, // 0011 0001 0001
		iPhone4          = 0x0412, // 0100 0001 0010
		iPod1G           = 0x0502, // 0101 0000 0010
		iPod2G           = 0x0602, // 0110 0000 0010
		iPod3G           = 0x0702, // 0111 0000 0010
		iPod4G           = 0x0802, // 1000 0000 0010
		iPad             = 0x0904, // 1001 0000 0100
		iPhoneSimulator  = 0x0A08, // 1010 0000 1000
		iPhone4Simulator = 0x0B08, // 1011 0000 1000
		iPadSimulator    = 0x0C08, // 1100 0000 1000
		Unknown          = 0x0D00  // 1101 0000 0000
	}
	
	public class Hardware {
	
		public const string HardwareProperty = "hw.machine";
		
		// Changing the constant to "/usr/bin/libSystem.dylib" allows this P/Invoke to work on Mac OS X
		// Using "hw.model" as property gives Macintosh model, "hw.machine" kernel arch (ppc, ppc64, i386, x86_64)
		[DllImport(MonoTouch.Constants.SystemLibrary)]
		internal static extern int sysctlbyname( [MarshalAs(UnmanagedType.LPStr)] string property, // name of the property
			                                        IntPtr output, // output
			                                        IntPtr oldLen, // IntPtr.Zero
			                                        IntPtr newp, // IntPtr.Zero
			                                        uint newlen // 0
		                                        );
		
		public static int ScreenWidth() {
			return ScreenWidth(Name);
		}
		
		public static int ScreenWidth(Device d) {
			return ScreenWidth(d, UIInterfaceOrientation.LandscapeLeft);
		}
		
		public static int ScreenWidth(Device d, UIInterfaceOrientation o) {
			switch (d) {
				case Device.iPad:
				case Device.iPadSimulator:
					return o == UIInterfaceOrientation.LandscapeLeft || o == UIInterfaceOrientation.LandscapeRight ? 1024 : 768;
					break;
				case Device.iPhone:
				case Device.iPhone3G:
				case Device.iPhone3GS:
				case Device.iPod1G:
				case Device.iPod2G:
				case Device.iPod3G:
				case Device.iPod4G:
				case Device.iPhoneSimulator:
					return o == UIInterfaceOrientation.LandscapeLeft || o == UIInterfaceOrientation.LandscapeRight ? 480 : 320;
				case Device.iPhone4:
				case Device.iPhone4Simulator:
					return o == UIInterfaceOrientation.LandscapeLeft || o == UIInterfaceOrientation.LandscapeRight ? 960 : 640;
				default:
					return 0;
			}
		}
		
		public static int ScreenHeight() {
			return ScreenWidth(Name);
		}
		
		public static int ScreenHeight(Device d) {
			return ScreenHeight(d, UIInterfaceOrientation.LandscapeLeft);
		}
		
		public static int ScreenHeight(Device d, UIInterfaceOrientation o) {
			switch (d) {
				case Device.iPad:
				case Device.iPadSimulator:
					return o == UIInterfaceOrientation.LandscapeLeft || o == UIInterfaceOrientation.LandscapeRight ? 768 : 1024;
					break;
				case Device.iPhone:
				case Device.iPhone3G:
				case Device.iPhone3GS:
				case Device.iPod1G:
				case Device.iPod2G:
				case Device.iPod3G:
				case Device.iPod4G:
				case Device.iPhoneSimulator:
					return o == UIInterfaceOrientation.LandscapeLeft || o == UIInterfaceOrientation.LandscapeRight ? 320 : 480;
				case Device.iPhone4:
				case Device.iPhone4Simulator:
					return o == UIInterfaceOrientation.LandscapeLeft || o == UIInterfaceOrientation.LandscapeRight ? 640 : 960;
				default:
					return 0;
			}
		}
		
		public static Device Name {
			get {
				// get the length of the string that will be returned
				var pLen = Marshal.AllocHGlobal(sizeof(int));
				sysctlbyname(Hardware.HardwareProperty, IntPtr.Zero, pLen, IntPtr.Zero, 0);
		
				var length = Marshal.ReadInt32(pLen);
		
				// check to see if we got a length
				if (length == 0) {
					Marshal.FreeHGlobal(pLen);
					return Device.Unknown;
				}
		
				// get the hardware string
				var pStr = Marshal.AllocHGlobal(length);
				sysctlbyname(Hardware.HardwareProperty, pStr, pLen, IntPtr.Zero, 0);
		
				// convert the native string into a C# string
				var hardwareStr = Marshal.PtrToStringAnsi(pStr);
				var ret = Device.Unknown;
		
				// determine which hardware we are running
				if (hardwareStr == "iPhone1,1")
					ret = Device.iPhone;
				else if (hardwareStr == "iPhone1,2")
					ret = Device.iPhone3G;
				else if (hardwareStr == "iPhone2,1")
					ret = Device.iPhone3GS;
				else if (hardwareStr == "iPhone3,1")
					ret = Device.iPhone4;
				else if (hardwareStr == "iPad1,1")
					ret = Device.iPad;
				else if (hardwareStr == "iPod1,1")
					ret = Device.iPod1G;
				else if (hardwareStr == "iPod2,1")
					ret = Device.iPod2G;
				else if (hardwareStr == "iPod3,1")
					ret = Device.iPod3G;
				else if (hardwareStr == "iPod4,1")
					ret = Device.iPod3G;
				else if (hardwareStr == "i386" || hardwareStr == "x86_64") {
					if (UIDevice.CurrentDevice.Model.Contains("iPhone"))
						ret = UIScreen.MainScreen.Bounds.Height * UIScreen.MainScreen.Scale == 960 || UIScreen.MainScreen.Bounds.Width * UIScreen.MainScreen.Scale == 960 ? Device.iPhone4Simulator : Device.iPhoneSimulator;
					else
						ret = Device.iPadSimulator;
					}
				else ret = Device.Unknown;
		
				// cleanup
				Marshal.FreeHGlobal(pLen);
				Marshal.FreeHGlobal(pStr);
		
				return ret;
			}
		}
	}
}

