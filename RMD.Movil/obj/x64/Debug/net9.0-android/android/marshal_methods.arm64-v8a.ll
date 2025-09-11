; ModuleID = 'marshal_methods.arm64-v8a.ll'
source_filename = "marshal_methods.arm64-v8a.ll"
target datalayout = "e-m:e-i8:8:32-i16:16:32-i64:64-i128:128-n32:64-S128"
target triple = "aarch64-unknown-linux-android21"

%struct.MarshalMethodName = type {
	i64, ; uint64_t id
	ptr ; char* name
}

%struct.MarshalMethodsManagedClass = type {
	i32, ; uint32_t token
	ptr ; MonoClass klass
}

@assembly_image_cache = dso_local local_unnamed_addr global [412 x ptr] zeroinitializer, align 8

; Each entry maps hash of an assembly name to an index into the `assembly_image_cache` array
@assembly_image_cache_hashes = dso_local local_unnamed_addr constant [1236 x i64] [
	i64 u0x001e58127c546039, ; 0: lib_System.Globalization.dll.so => 42
	i64 u0x0024d0f62dee05bd, ; 1: Xamarin.KotlinX.Coroutines.Core.dll => 353
	i64 u0x006b9d7c1c7e1c42, ; 2: de/Microsoft.Data.SqlClient.resources => 358
	i64 u0x0071cf2d27b7d61e, ; 3: lib_Xamarin.AndroidX.SwipeRefreshLayout.dll.so => 305
	i64 u0x01109b0e4d99e61f, ; 4: System.ComponentModel.Annotations.dll => 13
	i64 u0x02123411c4e01926, ; 5: lib_Xamarin.AndroidX.Navigation.Runtime.dll.so => 290
	i64 u0x022e81ea9c46e03a, ; 6: lib_CommunityToolkit.Maui.Core.dll.so => 177
	i64 u0x022f31be406de945, ; 7: Microsoft.Extensions.Options.ConfigurationExtensions => 197
	i64 u0x02827b47e97f2378, ; 8: System.Security.Cryptography.Pkcs.dll => 227
	i64 u0x0284512fad379f7e, ; 9: System.Runtime.Handles => 105
	i64 u0x029b2c18aaa0996c, ; 10: lib-ko-Microsoft.Data.SqlClient.resources.dll.so => 363
	i64 u0x02a4c5a44384f885, ; 11: Microsoft.Extensions.Caching.Memory => 184
	i64 u0x02abedc11addc1ed, ; 12: lib_Mono.Android.Runtime.dll.so => 171
	i64 u0x02f55bf70672f5c8, ; 13: lib_System.IO.FileSystem.DriveInfo.dll.so => 48
	i64 u0x032267b2a94db371, ; 14: lib_Xamarin.AndroidX.AppCompat.dll.so => 238
	i64 u0x03621c804933a890, ; 15: System.Buffers => 7
	i64 u0x037861ca1435352a, ; 16: OneSignalSDK.DotNet.Android.Core.Binding => 213
	i64 u0x0399610510a38a38, ; 17: lib_System.Private.DataContractSerialization.dll.so => 86
	i64 u0x043032f1d071fae0, ; 18: ru/Microsoft.Maui.Controls.resources => 394
	i64 u0x044440a55165631e, ; 19: lib-cs-Microsoft.Maui.Controls.resources.dll.so => 372
	i64 u0x046eb1581a80c6b0, ; 20: vi/Microsoft.Maui.Controls.resources => 400
	i64 u0x0470607fd33c32db, ; 21: Microsoft.IdentityModel.Abstractions.dll => 201
	i64 u0x047408741db2431a, ; 22: Xamarin.AndroidX.DynamicAnimation => 260
	i64 u0x0517ef04e06e9f76, ; 23: System.Net.Primitives => 71
	i64 u0x051a3be159e4ef99, ; 24: Xamarin.GooglePlayServices.Tasks => 343
	i64 u0x0565d18c6da3de38, ; 25: Xamarin.AndroidX.RecyclerView => 294
	i64 u0x0580fcad5f8dc5ac, ; 26: lib_Xamarin.Google.Dagger.dll.so => 335
	i64 u0x0581db89237110e9, ; 27: lib_System.Collections.dll.so => 12
	i64 u0x05989cb940b225a9, ; 28: Microsoft.Maui.dll => 209
	i64 u0x05a1c25e78e22d87, ; 29: lib_System.Runtime.CompilerServices.Unsafe.dll.so => 102
	i64 u0x05d8ca8ee551619f, ; 30: zh-Hant/Microsoft.Data.SqlClient.resources => 369
	i64 u0x06073ed944b92dc4, ; 31: lib-tr-Microsoft.Data.SqlClient.resources.dll.so => 367
	i64 u0x06076b5d2b581f08, ; 32: zh-HK/Microsoft.Maui.Controls.resources => 401
	i64 u0x06388ffe9f6c161a, ; 33: System.Xml.Linq.dll => 156
	i64 u0x06600c4c124cb358, ; 34: System.Configuration.dll => 19
	i64 u0x067f95c5ddab55b3, ; 35: lib_Xamarin.AndroidX.Fragment.Ktx.dll.so => 265
	i64 u0x0680a433c781bb3d, ; 36: Xamarin.AndroidX.Collection.Jvm => 246
	i64 u0x069fff96ec92a91d, ; 37: System.Xml.XPath.dll => 161
	i64 u0x070b0847e18dab68, ; 38: Xamarin.AndroidX.Emoji2.ViewsHelper.dll => 262
	i64 u0x0739448d84d3b016, ; 39: lib_Xamarin.AndroidX.VectorDrawable.dll.so => 308
	i64 u0x07469f2eecce9e85, ; 40: mscorlib.dll => 167
	i64 u0x07c57877c7ba78ad, ; 41: ru/Microsoft.Maui.Controls.resources.dll => 394
	i64 u0x07dcdc7460a0c5e4, ; 42: System.Collections.NonGeneric => 10
	i64 u0x08015600dcbf6dc7, ; 43: it/Microsoft.Data.SqlClient.resources.dll => 361
	i64 u0x08122e52765333c8, ; 44: lib_Microsoft.Extensions.Logging.Debug.dll.so => 195
	i64 u0x088610fc2509f69e, ; 45: lib_Xamarin.AndroidX.VectorDrawable.Animated.dll.so => 309
	i64 u0x08881a0a9768df86, ; 46: lib_Azure.Core.dll.so => 174
	i64 u0x08a7c865576bbde7, ; 47: System.Reflection.Primitives => 96
	i64 u0x08c9d051a4a817e5, ; 48: Xamarin.AndroidX.CustomView.PoolingContainer.dll => 257
	i64 u0x08f3c9788ee2153c, ; 49: Xamarin.AndroidX.DrawerLayout => 259
	i64 u0x09076b821f278db5, ; 50: Xamarin.Google.Dagger => 335
	i64 u0x09138715c92dba90, ; 51: lib_System.ComponentModel.Annotations.dll.so => 13
	i64 u0x0919c28b89381a0b, ; 52: lib_Microsoft.Extensions.Options.dll.so => 196
	i64 u0x092266563089ae3e, ; 53: lib_System.Collections.NonGeneric.dll.so => 10
	i64 u0x095cacaf6b6a32e4, ; 54: System.Memory.Data => 226
	i64 u0x098b50f911ccea8d, ; 55: lib_Xamarin.GooglePlayServices.Basement.dll.so => 340
	i64 u0x09ac681cd3340a4c, ; 56: lib_Xamarin.AndroidX.Room.Runtime.dll.so => 297
	i64 u0x09d144a7e214d457, ; 57: System.Security.Cryptography => 127
	i64 u0x09da6dfc3439e851, ; 58: lib_Xamarin.Firebase.Components.dll.so => 319
	i64 u0x09e2b9f743db21a8, ; 59: lib_System.Reflection.Metadata.dll.so => 95
	i64 u0x0a805f95d98f597b, ; 60: lib_Microsoft.Extensions.Caching.Abstractions.dll.so => 183
	i64 u0x0abb3e2b271edc45, ; 61: System.Threading.Channels.dll => 140
	i64 u0x0adeb6c0f5699d33, ; 62: Microsoft.Data.SqlClient.dll => 182
	i64 u0x0b06b1feab070143, ; 63: System.Formats.Tar => 39
	i64 u0x0b3b632c3bbee20c, ; 64: sk/Microsoft.Maui.Controls.resources => 395
	i64 u0x0b521223ae889727, ; 65: Xamarin.Firebase.Messaging => 328
	i64 u0x0b6aff547b84fbe9, ; 66: Xamarin.KotlinX.Serialization.Core.Jvm => 356
	i64 u0x0be2e1f8ce4064ed, ; 67: Xamarin.AndroidX.ViewPager => 311
	i64 u0x0c3ca6cc978e2aae, ; 68: pt-BR/Microsoft.Maui.Controls.resources => 391
	i64 u0x0c59ad9fbbd43abe, ; 69: Mono.Android => 172
	i64 u0x0c65741e86371ee3, ; 70: lib_Xamarin.Android.Glide.GifDecoder.dll.so => 232
	i64 u0x0c74af560004e816, ; 71: Microsoft.Win32.Registry.dll => 5
	i64 u0x0c7790f60165fc06, ; 72: lib_Microsoft.Maui.Essentials.dll.so => 210
	i64 u0x0c83c82812e96127, ; 73: lib_System.Net.Mail.dll.so => 67
	i64 u0x0cce4bce83380b7f, ; 74: Xamarin.AndroidX.Security.SecurityCrypto => 300
	i64 u0x0d13cd7cce4284e4, ; 75: System.Security.SecureString => 130
	i64 u0x0d3b5ab8b2766190, ; 76: lib_Microsoft.Bcl.AsyncInterfaces.dll.so => 180
	i64 u0x0d63f4f73521c24f, ; 77: lib_Xamarin.AndroidX.SavedState.SavedState.Ktx.dll.so => 299
	i64 u0x0e04e702012f8463, ; 78: Xamarin.AndroidX.Emoji2 => 261
	i64 u0x0e14e73a54dda68e, ; 79: lib_System.Net.NameResolution.dll.so => 68
	i64 u0x0e5f1ca7132adebb, ; 80: Xamarin.Firebase.Installations.dll => 325
	i64 u0x0ee0094c4d11c3a0, ; 81: lib_OneSignalSDK.DotNet.Android.InAppMessages.Binding.dll.so => 214
	i64 u0x0f37dd7a62ae99af, ; 82: lib_Xamarin.AndroidX.Collection.Ktx.dll.so => 247
	i64 u0x0f5e7abaa7cf470a, ; 83: System.Net.HttpListener => 66
	i64 u0x0fbe06392ef90569, ; 84: lib-ja-Microsoft.Data.SqlClient.resources.dll.so => 362
	i64 u0x1001f97bbe242e64, ; 85: System.IO.UnmanagedMemoryStream => 57
	i64 u0x102861e4055f511a, ; 86: Microsoft.Bcl.AsyncInterfaces.dll => 180
	i64 u0x102a31b45304b1da, ; 87: Xamarin.AndroidX.CustomView => 256
	i64 u0x1065c4cb554c3d75, ; 88: System.IO.IsolatedStorage.dll => 52
	i64 u0x108cf0e0ba098a51, ; 89: es/Microsoft.Data.SqlClient.resources => 359
	i64 u0x10f6cfcbcf801616, ; 90: System.IO.Compression.Brotli => 43
	i64 u0x114443cdcf2091f1, ; 91: System.Security.Cryptography.Primitives => 125
	i64 u0x11a603952763e1d4, ; 92: System.Net.Mail => 67
	i64 u0x11a70d0e1009fb11, ; 93: System.Net.WebSockets.dll => 81
	i64 u0x11f26371eee0d3c1, ; 94: lib_Xamarin.AndroidX.Lifecycle.Runtime.Ktx.dll.so => 278
	i64 u0x11fbe62d469cc1c8, ; 95: Microsoft.VisualStudio.DesignTools.TapContract.dll => 406
	i64 u0x12128b3f59302d47, ; 96: lib_System.Xml.Serialization.dll.so => 158
	i64 u0x123639456fb056da, ; 97: System.Reflection.Emit.Lightweight.dll => 92
	i64 u0x12521e9764603eaa, ; 98: lib_System.Resources.Reader.dll.so => 99
	i64 u0x125b7f94acb989db, ; 99: Xamarin.AndroidX.RecyclerView.dll => 294
	i64 u0x126ee4b0de53cbfd, ; 100: Microsoft.IdentityModel.Protocols.OpenIdConnect.dll => 205
	i64 u0x12d3b63863d4ab0b, ; 101: lib_System.Threading.Overlapped.dll.so => 141
	i64 u0x134085e8fa0a1a65, ; 102: Xamarin.AndroidX.Sqlite.dll => 302
	i64 u0x134eab1061c395ee, ; 103: System.Transactions => 151
	i64 u0x138567fa954faa55, ; 104: Xamarin.AndroidX.Browser => 243
	i64 u0x13a01de0cbc3f06c, ; 105: lib-fr-Microsoft.Maui.Controls.resources.dll.so => 378
	i64 u0x13beedefb0e28a45, ; 106: lib_System.Xml.XmlDocument.dll.so => 162
	i64 u0x13f1e5e209e91af4, ; 107: lib_Java.Interop.dll.so => 169
	i64 u0x13f1e880c25d96d1, ; 108: he/Microsoft.Maui.Controls.resources => 379
	i64 u0x1403071365bcd83a, ; 109: Xamarin.Firebase.Annotations => 317
	i64 u0x143a1f6e62b82b56, ; 110: Microsoft.IdentityModel.Protocols.OpenIdConnect => 205
	i64 u0x143d8ea60a6a4011, ; 111: Microsoft.Extensions.DependencyInjection.Abstractions => 189
	i64 u0x1497051b917530bd, ; 112: lib_System.Net.WebSockets.dll.so => 81
	i64 u0x14b78ce3adce0011, ; 113: Microsoft.VisualStudio.DesignTools.TapContract => 406
	i64 u0x14c7136b1f3e05a2, ; 114: OneSignalSDK.DotNet.Core.dll => 218
	i64 u0x14d612a531c79c05, ; 115: Xamarin.JSpecify.dll => 346
	i64 u0x14e68447938213b7, ; 116: Xamarin.AndroidX.Collection.Ktx.dll => 247
	i64 u0x152a448bd1e745a7, ; 117: Microsoft.Win32.Primitives => 4
	i64 u0x1557de0138c445f4, ; 118: lib_Microsoft.Win32.Registry.dll.so => 5
	i64 u0x159cc6c81072f00e, ; 119: lib_System.Diagnostics.EventLog.dll.so => 224
	i64 u0x15bdc156ed462f2f, ; 120: lib_System.IO.FileSystem.dll.so => 51
	i64 u0x15e300c2c1668655, ; 121: System.Resources.Writer.dll => 101
	i64 u0x162be8a76b00cd97, ; 122: lib-de-Microsoft.Data.SqlClient.resources.dll.so => 358
	i64 u0x16bf2a22df043a09, ; 123: System.IO.Pipes.dll => 56
	i64 u0x16ca6be5d217e21e, ; 124: lib_RMD.Movil.dll.so => 0
	i64 u0x16ea2b318ad2d830, ; 125: System.Security.Cryptography.Algorithms => 120
	i64 u0x16eeae54c7ebcc08, ; 126: System.Reflection.dll => 98
	i64 u0x17125c9a85b4929f, ; 127: lib_netstandard.dll.so => 168
	i64 u0x1716866f7416792e, ; 128: lib_System.Security.AccessControl.dll.so => 118
	i64 u0x174f71c46216e44a, ; 129: Xamarin.KotlinX.Coroutines.Core => 353
	i64 u0x1752c12f1e1fc00c, ; 130: System.Core => 21
	i64 u0x1752d90fe9bcded5, ; 131: RMD.Shared.Utils.dll => 410
	i64 u0x17b56e25558a5d36, ; 132: lib-hu-Microsoft.Maui.Controls.resources.dll.so => 382
	i64 u0x17f9358913beb16a, ; 133: System.Text.Encodings.Web => 137
	i64 u0x1809fb23f29ba44a, ; 134: lib_System.Reflection.TypeExtensions.dll.so => 97
	i64 u0x18402a709e357f3b, ; 135: lib_Xamarin.KotlinX.Serialization.Core.Jvm.dll.so => 356
	i64 u0x18a9befae51bb361, ; 136: System.Net.WebClient => 77
	i64 u0x18f0ce884e87d89a, ; 137: nb/Microsoft.Maui.Controls.resources.dll => 388
	i64 u0x193d7a04b7eda8bc, ; 138: lib_Xamarin.AndroidX.Print.dll.so => 292
	i64 u0x19777fba3c41b398, ; 139: Xamarin.AndroidX.Startup.StartupRuntime.dll => 304
	i64 u0x19a4c090f14ebb66, ; 140: System.Security.Claims => 119
	i64 u0x1a6fceea64859810, ; 141: Azure.Identity => 175
	i64 u0x1a91866a319e9259, ; 142: lib_System.Collections.Concurrent.dll.so => 8
	i64 u0x1a9a69dd443930f5, ; 143: OneSignalSDK.DotNet.Android.InAppMessages.Binding.dll => 214
	i64 u0x1aac34d1917ba5d3, ; 144: lib_System.dll.so => 165
	i64 u0x1aad60783ffa3e5b, ; 145: lib-th-Microsoft.Maui.Controls.resources.dll.so => 397
	i64 u0x1aea8f1c3b282172, ; 146: lib_System.Net.Ping.dll.so => 70
	i64 u0x1b4b7a1d0d265fa2, ; 147: Xamarin.Android.Glide.DiskLruCache => 231
	i64 u0x1bbdb16cfa73e785, ; 148: Xamarin.AndroidX.Lifecycle.Runtime.Ktx.Android => 279
	i64 u0x1bc766e07b2b4241, ; 149: Xamarin.AndroidX.ResourceInspection.Annotation.dll => 295
	i64 u0x1c292b1598348d77, ; 150: Microsoft.Extensions.Diagnostics.dll => 190
	i64 u0x1c753b5ff15bce1b, ; 151: Mono.Android.Runtime.dll => 171
	i64 u0x1c7cbc0ecd18cdaf, ; 152: Xamarin.Firebase.Encoders.dll => 321
	i64 u0x1cd47467799d8250, ; 153: System.Threading.Tasks.dll => 145
	i64 u0x1d23eafdc6dc346c, ; 154: System.Globalization.Calendars.dll => 40
	i64 u0x1da4110562816681, ; 155: Xamarin.AndroidX.Security.SecurityCrypto.dll => 300
	i64 u0x1db6820994506bf5, ; 156: System.IO.FileSystem.AccessControl.dll => 47
	i64 u0x1dbb0c2c6a999acb, ; 157: System.Diagnostics.StackTrace => 30
	i64 u0x1e3d87657e9659bc, ; 158: Xamarin.AndroidX.Navigation.UI => 291
	i64 u0x1e71143913d56c10, ; 159: lib-ko-Microsoft.Maui.Controls.resources.dll.so => 386
	i64 u0x1e7c31185e2fb266, ; 160: lib_System.Threading.Tasks.Parallel.dll.so => 144
	i64 u0x1eb3fdd1d3192a5f, ; 161: Xamarin.AndroidX.Work.Work.Runtime.Ktx => 316
	i64 u0x1ed8fcce5e9b50a0, ; 162: Microsoft.Extensions.Options.dll => 196
	i64 u0x1f055d15d807e1b2, ; 163: System.Xml.XmlSerializer => 163
	i64 u0x1f1ed22c1085f044, ; 164: lib_System.Diagnostics.FileVersionInfo.dll.so => 28
	i64 u0x1f61df9c5b94d2c1, ; 165: lib_System.Numerics.dll.so => 84
	i64 u0x1f750bb5421397de, ; 166: lib_Xamarin.AndroidX.Tracing.Tracing.dll.so => 306
	i64 u0x1f7fcab141728c45, ; 167: Xamarin.AndroidX.Legacy.Support.Core.UI => 267
	i64 u0x1fc1a401201d7356, ; 168: Xamarin.Google.Dagger.dll => 335
	i64 u0x20237ea48006d7a8, ; 169: lib_System.Net.WebClient.dll.so => 77
	i64 u0x203c381a1b2c2f25, ; 170: Xamarin.AndroidX.Room.Common => 296
	i64 u0x209375905fcc1bad, ; 171: lib_System.IO.Compression.Brotli.dll.so => 43
	i64 u0x20fab3cf2dfbc8df, ; 172: lib_System.Diagnostics.Process.dll.so => 29
	i64 u0x2110167c128cba15, ; 173: System.Globalization => 42
	i64 u0x21419508838f7547, ; 174: System.Runtime.CompilerServices.VisualC => 103
	i64 u0x2174319c0d835bc9, ; 175: System.Runtime => 117
	i64 u0x2198e5bc8b7153fa, ; 176: Xamarin.AndroidX.Annotation.Experimental.dll => 236
	i64 u0x2199f06354c82d3b, ; 177: System.ClientModel.dll => 222
	i64 u0x219ea1b751a4dee4, ; 178: lib_System.IO.Compression.ZipFile.dll.so => 45
	i64 u0x21cc7e445dcd5469, ; 179: System.Reflection.Emit.ILGeneration => 91
	i64 u0x21f31ab89ce62e02, ; 180: lib_OneSignalSDK.DotNet.dll.so => 219
	i64 u0x220fd4f2e7c48170, ; 181: th/Microsoft.Maui.Controls.resources => 397
	i64 u0x224538d85ed15a82, ; 182: System.IO.Pipes => 56
	i64 u0x22908438c6bed1af, ; 183: lib_System.Threading.Timer.dll.so => 148
	i64 u0x22fbc14e981e3b45, ; 184: lib_Microsoft.VisualStudio.DesignTools.MobileTapContracts.dll.so => 405
	i64 u0x2347c268e3e4e536, ; 185: Xamarin.GooglePlayServices.Basement.dll => 340
	i64 u0x237be844f1f812c7, ; 186: System.Threading.Thread.dll => 146
	i64 u0x23852b3bdc9f7096, ; 187: System.Resources.ResourceManager => 100
	i64 u0x23986dd7e5d4fc01, ; 188: System.IO.FileSystem.Primitives.dll => 49
	i64 u0x23f599165f90dd7a, ; 189: lib-cs-Microsoft.Data.SqlClient.resources.dll.so => 357
	i64 u0x2407aef2bbe8fadf, ; 190: System.Console => 20
	i64 u0x240abe014b27e7d3, ; 191: Xamarin.AndroidX.Core.dll => 252
	i64 u0x242bf4514622c837, ; 192: Xamarin.AndroidX.Legacy.Support.V4.dll => 269
	i64 u0x247619fe4413f8bf, ; 193: System.Runtime.Serialization.Primitives.dll => 114
	i64 u0x24a77624feb83011, ; 194: lib_Xamarin.AndroidX.Sqlite.dll.so => 302
	i64 u0x24de8d301281575e, ; 195: Xamarin.Android.Glide => 229
	i64 u0x24ea225f38dba051, ; 196: Xamarin.AndroidX.Room.Runtime => 297
	i64 u0x252073cc3caa62c2, ; 197: fr/Microsoft.Maui.Controls.resources.dll => 378
	i64 u0x256b8d41255f01b1, ; 198: Xamarin.Google.Crypto.Tink.Android => 334
	i64 u0x2643615a57920257, ; 199: Xamarin.AndroidX.Legacy.Support.V4 => 269
	i64 u0x2662c629b96b0b30, ; 200: lib_Xamarin.Kotlin.StdLib.dll.so => 347
	i64 u0x268c1439f13bcc29, ; 201: lib_Microsoft.Extensions.Primitives.dll.so => 198
	i64 u0x26a670e154a9c54b, ; 202: System.Reflection.Extensions.dll => 94
	i64 u0x26cdccfaaa010ebc, ; 203: OneSignalSDK.DotNet.Android.dll => 217
	i64 u0x26d077d9678fe34f, ; 204: System.IO.dll => 58
	i64 u0x270a44600c921861, ; 205: System.IdentityModel.Tokens.Jwt => 225
	i64 u0x273f3515de5faf0d, ; 206: id/Microsoft.Maui.Controls.resources.dll => 383
	i64 u0x2742545f9094896d, ; 207: hr/Microsoft.Maui.Controls.resources => 381
	i64 u0x2759af78ab94d39b, ; 208: System.Net.WebSockets => 81
	i64 u0x27b2b16f3e9de038, ; 209: Xamarin.Google.Crypto.Tink.Android.dll => 334
	i64 u0x27b410442fad6cf1, ; 210: Java.Interop.dll => 169
	i64 u0x27b97e0d52c3034a, ; 211: System.Diagnostics.Debug => 26
	i64 u0x27c1227907352196, ; 212: lib_Xamarin.Firebase.Installations.InterOp.dll.so => 326
	i64 u0x27eb21c6eb99d774, ; 213: Xamarin.Kotlin.StdLib.Jdk8.dll => 349
	i64 u0x2801845a2c71fbfb, ; 214: System.Net.Primitives.dll => 71
	i64 u0x286835e259162700, ; 215: lib_Xamarin.AndroidX.ProfileInstaller.ProfileInstaller.dll.so => 293
	i64 u0x28e52865585a1ebe, ; 216: Microsoft.Extensions.Diagnostics.Abstractions.dll => 191
	i64 u0x2949f3617a02c6b2, ; 217: Xamarin.AndroidX.ExifInterface => 263
	i64 u0x29c819b69129cfb7, ; 218: lib_Xamarin.AndroidX.Sqlite.Framework.dll.so => 303
	i64 u0x29f947844fb7fc11, ; 219: Microsoft.Maui.Controls.HotReload.Forms => 404
	i64 u0x2a128783efe70ba0, ; 220: uk/Microsoft.Maui.Controls.resources.dll => 399
	i64 u0x2a3b095612184159, ; 221: lib_System.Net.NetworkInformation.dll.so => 69
	i64 u0x2a6507a5ffabdf28, ; 222: System.Diagnostics.TraceSource.dll => 33
	i64 u0x2ad156c8e1354139, ; 223: fi/Microsoft.Maui.Controls.resources => 377
	i64 u0x2ad5d6b13b7a3e04, ; 224: System.ComponentModel.DataAnnotations.dll => 14
	i64 u0x2af298f63581d886, ; 225: System.Text.RegularExpressions.dll => 139
	i64 u0x2af615542f04da50, ; 226: System.IdentityModel.Tokens.Jwt.dll => 225
	i64 u0x2afc1c4f898552ee, ; 227: lib_System.Formats.Asn1.dll.so => 38
	i64 u0x2b148910ed40fbf9, ; 228: zh-Hant/Microsoft.Maui.Controls.resources.dll => 403
	i64 u0x2b6989d78cba9a15, ; 229: Xamarin.AndroidX.Concurrent.Futures.dll => 248
	i64 u0x2beed069f537ac18, ; 230: lib_RMD.Shared.Models.dll.so => 409
	i64 u0x2c4c3b619f1b41f8, ; 231: Xamarin.AndroidX.AsyncLayoutInflater.dll => 242
	i64 u0x2c517f906db0a191, ; 232: Xamarin.Google.Android.DataTransport.TransportBackendCct.dll => 330
	i64 u0x2c8bd14bb93a7d82, ; 233: lib-pl-Microsoft.Maui.Controls.resources.dll.so => 390
	i64 u0x2cbd9262ca785540, ; 234: lib_System.Text.Encoding.CodePages.dll.so => 134
	i64 u0x2cc9e1fed6257257, ; 235: lib_System.Reflection.Emit.Lightweight.dll.so => 92
	i64 u0x2cd723e9fe623c7c, ; 236: lib_System.Private.Xml.Linq.dll.so => 88
	i64 u0x2ce66f4c8733e883, ; 237: pt-BR/Microsoft.Data.SqlClient.resources.dll => 365
	i64 u0x2d169d318a968379, ; 238: System.Threading.dll => 149
	i64 u0x2d47774b7d993f59, ; 239: sv/Microsoft.Maui.Controls.resources.dll => 396
	i64 u0x2d5ffcae1ad0aaca, ; 240: System.Data.dll => 24
	i64 u0x2db915caf23548d2, ; 241: System.Text.Json.dll => 138
	i64 u0x2dcaa0bb15a4117a, ; 242: System.IO.UnmanagedMemoryStream.dll => 57
	i64 u0x2e2cd95ef05fc714, ; 243: OneSignalSDK.DotNet.Android => 217
	i64 u0x2e5a40c319acb800, ; 244: System.IO.FileSystem => 51
	i64 u0x2e6f1f226821322a, ; 245: el/Microsoft.Maui.Controls.resources.dll => 375
	i64 u0x2e7ae36a08a1dbb3, ; 246: Xamarin.Firebase.Encoders => 321
	i64 u0x2f02f94df3200fe5, ; 247: System.Diagnostics.Process => 29
	i64 u0x2f2e98e1c89b1aff, ; 248: System.Xml.ReaderWriter => 157
	i64 u0x2f40b2521deba305, ; 249: lib_Microsoft.SqlServer.Server.dll.so => 212
	i64 u0x2f5911d9ba814e4e, ; 250: System.Diagnostics.Tracing => 34
	i64 u0x2f84070a459bc31f, ; 251: lib_System.Xml.dll.so => 164
	i64 u0x2feb4d2fcda05cfd, ; 252: Microsoft.Extensions.Caching.Abstractions.dll => 183
	i64 u0x2ff49de6a71764a1, ; 253: lib_Microsoft.Extensions.Http.dll.so => 192
	i64 u0x301713b5054e4388, ; 254: Xamarin.GooglePlayServices.Stats.dll => 342
	i64 u0x3053de11b4c0cb63, ; 255: lib_Xamarin.Firebase.Measurement.Connector.dll.so => 327
	i64 u0x30829702a4057a12, ; 256: Xamarin.Google.Android.DataTransport.TransportBackendCct => 330
	i64 u0x309ee9eeec09a71e, ; 257: lib_Xamarin.AndroidX.Fragment.dll.so => 264
	i64 u0x309f2bedefa9a318, ; 258: Microsoft.IdentityModel.Abstractions => 201
	i64 u0x30c6dda129408828, ; 259: System.IO.IsolatedStorage => 52
	i64 u0x31195fef5d8fb552, ; 260: _Microsoft.Android.Resource.Designer.dll => 411
	i64 u0x312c8ed623cbfc8d, ; 261: Xamarin.AndroidX.Window.dll => 313
	i64 u0x31496b779ed0663d, ; 262: lib_System.Reflection.DispatchProxy.dll.so => 90
	i64 u0x315f08d19390dc36, ; 263: Xamarin.Google.ErrorProne.TypeAnnotations => 337
	i64 u0x31f946282b3de50c, ; 264: Xamarin.Firebase.Datatransport.dll => 320
	i64 u0x3200b1eae7efaded, ; 265: lib_Xamarin.Firebase.Encoders.JSON.dll.so => 322
	i64 u0x32243413e774362a, ; 266: Xamarin.AndroidX.CardView.dll => 244
	i64 u0x3235427f8d12dae1, ; 267: lib_System.Drawing.Primitives.dll.so => 35
	i64 u0x329753a17a517811, ; 268: fr/Microsoft.Maui.Controls.resources => 378
	i64 u0x32aa989ff07a84ff, ; 269: lib_System.Xml.ReaderWriter.dll.so => 157
	i64 u0x33829542f112d59b, ; 270: System.Collections.Immutable => 9
	i64 u0x33a31443733849fe, ; 271: lib-es-Microsoft.Maui.Controls.resources.dll.so => 376
	i64 u0x341abc357fbb4ebf, ; 272: lib_System.Net.Sockets.dll.so => 76
	i64 u0x348d598f4054415e, ; 273: Microsoft.SqlServer.Server => 212
	i64 u0x3496c1e2dcaf5ecc, ; 274: lib_System.IO.Pipes.AccessControl.dll.so => 55
	i64 u0x34dfd74fe2afcf37, ; 275: Microsoft.Maui => 209
	i64 u0x34e292762d9615df, ; 276: cs/Microsoft.Maui.Controls.resources.dll => 372
	i64 u0x34f550615634aa64, ; 277: lib_Xamarin.Firebase.Encoders.dll.so => 321
	i64 u0x3508234247f48404, ; 278: Microsoft.Maui.Controls => 207
	i64 u0x353590da528c9d22, ; 279: System.ComponentModel.Annotations => 13
	i64 u0x3549870798b4cd30, ; 280: lib_Xamarin.AndroidX.ViewPager2.dll.so => 312
	i64 u0x355282fc1c909694, ; 281: Microsoft.Extensions.Configuration => 185
	i64 u0x3552fc5d578f0fbf, ; 282: Xamarin.AndroidX.Arch.Core.Common => 240
	i64 u0x355c649948d55d97, ; 283: lib_System.Runtime.Intrinsics.dll.so => 109
	i64 u0x35ea9d1c6834bc8c, ; 284: Xamarin.AndroidX.Lifecycle.ViewModel.Ktx.dll => 283
	i64 u0x3628ab68db23a01a, ; 285: lib_System.Diagnostics.Tools.dll.so => 32
	i64 u0x364703ab05867b92, ; 286: Xamarin.Firebase.Components => 319
	i64 u0x3673b042508f5b6b, ; 287: lib_System.Runtime.Extensions.dll.so => 104
	i64 u0x36740f1a8ecdc6c4, ; 288: System.Numerics => 84
	i64 u0x36b2b50fdf589ae2, ; 289: System.Reflection.Emit.Lightweight => 92
	i64 u0x36cada77dc79928b, ; 290: System.IO.MemoryMappedFiles => 53
	i64 u0x37377625566120b1, ; 291: lib_RMD.Movil.Core.dll.so => 408
	i64 u0x374ef46b06791af6, ; 292: System.Reflection.Primitives.dll => 96
	i64 u0x376bf93e521a5417, ; 293: lib_Xamarin.Jetbrains.Annotations.dll.so => 345
	i64 u0x37bc29f3183003b6, ; 294: lib_System.IO.dll.so => 58
	i64 u0x380134e03b1e160a, ; 295: System.Collections.Immutable.dll => 9
	i64 u0x38049b5c59b39324, ; 296: System.Runtime.CompilerServices.Unsafe => 102
	i64 u0x382b20286945c849, ; 297: lib_Microsoft.Bcl.Cryptography.dll.so => 181
	i64 u0x385c17636bb6fe6e, ; 298: Xamarin.AndroidX.CustomView.dll => 256
	i64 u0x38869c811d74050e, ; 299: System.Net.NameResolution.dll => 68
	i64 u0x38e93ec1c057cdf6, ; 300: Microsoft.IdentityModel.Protocols => 204
	i64 u0x39251dccb84bdcaa, ; 301: lib_System.Configuration.ConfigurationManager.dll.so => 223
	i64 u0x393c226616977fdb, ; 302: lib_Xamarin.AndroidX.ViewPager.dll.so => 311
	i64 u0x395e37c3334cf82a, ; 303: lib-ca-Microsoft.Maui.Controls.resources.dll.so => 371
	i64 u0x39f54e3c93ae3e6a, ; 304: Xamarin.AndroidX.Room.Runtime.dll => 297
	i64 u0x3ab5859054645f72, ; 305: System.Security.Cryptography.Primitives.dll => 125
	i64 u0x3ad75090c3fac0e9, ; 306: lib_Xamarin.AndroidX.ResourceInspection.Annotation.dll.so => 295
	i64 u0x3ae44ac43a1fbdbb, ; 307: System.Runtime.Serialization => 116
	i64 u0x3b3a441a55ff7e2e, ; 308: lib_Xamarin.AndroidX.Work.Work.Runtime.Ktx.dll.so => 316
	i64 u0x3b860f9932505633, ; 309: lib_System.Text.Encoding.Extensions.dll.so => 135
	i64 u0x3bea9ebe8c027c01, ; 310: lib_Microsoft.IdentityModel.Tokens.dll.so => 206
	i64 u0x3c3aafb6b3a00bf6, ; 311: lib_System.Security.Cryptography.X509Certificates.dll.so => 126
	i64 u0x3c4049146b59aa90, ; 312: System.Runtime.InteropServices.JavaScript => 106
	i64 u0x3c5f19e4acdcebd8, ; 313: lib_Microsoft.Data.SqlClient.dll.so => 182
	i64 u0x3c7c495f58ac5ee9, ; 314: Xamarin.Kotlin.StdLib => 347
	i64 u0x3c7e5ed3d5db71bb, ; 315: System.Security => 131
	i64 u0x3cd9d281d402eb9b, ; 316: Xamarin.AndroidX.Browser.dll => 243
	i64 u0x3d1c50cc001a991e, ; 317: Xamarin.Google.Guava.ListenableFuture.dll => 338
	i64 u0x3d2b1913edfc08d7, ; 318: lib_System.Threading.ThreadPool.dll.so => 147
	i64 u0x3d46f0b995082740, ; 319: System.Xml.Linq => 156
	i64 u0x3d8a8f400514a790, ; 320: Xamarin.AndroidX.Fragment.Ktx.dll => 265
	i64 u0x3d9c2a242b040a50, ; 321: lib_Xamarin.AndroidX.Core.dll.so => 252
	i64 u0x3dbb6b9f5ab90fa7, ; 322: lib_Xamarin.AndroidX.DynamicAnimation.dll.so => 260
	i64 u0x3e0b360b2840f096, ; 323: it/Microsoft.Data.SqlClient.resources => 361
	i64 u0x3e250296f3b5714d, ; 324: Xamarin.Firebase.Measurement.Connector => 327
	i64 u0x3e5441657549b213, ; 325: Xamarin.AndroidX.ResourceInspection.Annotation => 295
	i64 u0x3e57d4d195c53c2e, ; 326: System.Reflection.TypeExtensions => 97
	i64 u0x3e616ab4ed1f3f15, ; 327: lib_System.Data.dll.so => 24
	i64 u0x3f1d226e6e06db7e, ; 328: Xamarin.AndroidX.SlidingPaneLayout.dll => 301
	i64 u0x3f3c8f45ab6f28c7, ; 329: Microsoft.Identity.Client.Extensions.Msal.dll => 200
	i64 u0x3f510adf788828dd, ; 330: System.Threading.Tasks.Extensions => 143
	i64 u0x407740ff2e914d86, ; 331: Xamarin.AndroidX.Print.dll => 292
	i64 u0x407a10bb4bf95829, ; 332: lib_Xamarin.AndroidX.Navigation.Common.dll.so => 288
	i64 u0x407ac43dee26bd5a, ; 333: lib_Azure.Identity.dll.so => 175
	i64 u0x40c98b6bd77346d4, ; 334: Microsoft.VisualBasic.dll => 3
	i64 u0x415e36f6b13ff6f3, ; 335: System.Configuration.ConfigurationManager.dll => 223
	i64 u0x41833cf766d27d96, ; 336: mscorlib => 167
	i64 u0x41cab042be111c34, ; 337: lib_Xamarin.AndroidX.AppCompat.AppCompatResources.dll.so => 239
	i64 u0x4216fdf45ceff571, ; 338: Xamarin.AndroidX.Sqlite.Framework.dll => 303
	i64 u0x423a9ecc4d905a88, ; 339: lib_System.Resources.ResourceManager.dll.so => 100
	i64 u0x423bf51ae7def810, ; 340: System.Xml.XPath => 161
	i64 u0x42462ff15ddba223, ; 341: System.Resources.Reader.dll => 99
	i64 u0x426347ffcb8732cf, ; 342: Xamarin.AndroidX.Lifecycle.Service => 280
	i64 u0x4291015ff4e5ef71, ; 343: Xamarin.AndroidX.Core.ViewTree.dll => 254
	i64 u0x42a31b86e6ccc3f0, ; 344: System.Diagnostics.Contracts => 25
	i64 u0x42dd14640913f65e, ; 345: lib_RMD.Shared.Utils.dll.so => 410
	i64 u0x430e95b891249788, ; 346: lib_System.Reflection.Emit.dll.so => 93
	i64 u0x432dbba8b0560810, ; 347: OneSignalSDK.DotNet.Android.InAppMessages.Binding => 214
	i64 u0x43375950ec7c1b6a, ; 348: netstandard.dll => 168
	i64 u0x434c4e1d9284cdae, ; 349: Mono.Android.dll => 172
	i64 u0x43505013578652a0, ; 350: lib_Xamarin.AndroidX.Activity.Ktx.dll.so => 234
	i64 u0x437d06c381ed575a, ; 351: lib_Microsoft.VisualBasic.dll.so => 3
	i64 u0x4382363bad1b4039, ; 352: OneSignalSDK.DotNet.dll => 219
	i64 u0x43950f84de7cc79a, ; 353: pl/Microsoft.Maui.Controls.resources.dll => 390
	i64 u0x43e8ca5bc927ff37, ; 354: lib_Xamarin.AndroidX.Emoji2.ViewsHelper.dll.so => 262
	i64 u0x448bd33429269b19, ; 355: Microsoft.CSharp => 1
	i64 u0x4499fa3c8e494654, ; 356: lib_System.Runtime.Serialization.Primitives.dll.so => 114
	i64 u0x4515080865a951a5, ; 357: Xamarin.Kotlin.StdLib.dll => 347
	i64 u0x4545802489b736b9, ; 358: Xamarin.AndroidX.Fragment.Ktx => 265
	i64 u0x454b4d1e66bb783c, ; 359: Xamarin.AndroidX.Lifecycle.Process => 275
	i64 u0x458d2df79ac57c1d, ; 360: lib_System.IdentityModel.Tokens.Jwt.dll.so => 225
	i64 u0x4598f1adbd9c8129, ; 361: OneSignalSDK.DotNet.Android.Location.Binding => 215
	i64 u0x45c40276a42e283e, ; 362: System.Diagnostics.TraceSource => 33
	i64 u0x45d443f2a29adc37, ; 363: System.AppContext.dll => 6
	i64 u0x46a4213bc97fe5ae, ; 364: lib-ru-Microsoft.Maui.Controls.resources.dll.so => 394
	i64 u0x47358bd471172e1d, ; 365: lib_System.Xml.Linq.dll.so => 156
	i64 u0x4787a936949fcac2, ; 366: System.Memory.Data.dll => 226
	i64 u0x47daf4e1afbada10, ; 367: pt/Microsoft.Maui.Controls.resources => 392
	i64 u0x480c0a47dd42dd81, ; 368: lib_System.IO.MemoryMappedFiles.dll.so => 53
	i64 u0x488d293220a4fe37, ; 369: Xamarin.AndroidX.Legacy.Support.Core.Utils.dll => 268
	i64 u0x49e952f19a4e2022, ; 370: System.ObjectModel => 85
	i64 u0x49f6ab815e178ca9, ; 371: lib_Xamarin.Firebase.Common.dll.so => 318
	i64 u0x49f9e6948a8131e4, ; 372: lib_Xamarin.AndroidX.VersionedParcelable.dll.so => 310
	i64 u0x4a5667b2462a664b, ; 373: lib_Xamarin.AndroidX.Navigation.UI.dll.so => 291
	i64 u0x4a7a18981dbd56bc, ; 374: System.IO.Compression.FileSystem.dll => 44
	i64 u0x4aa5c60350917c06, ; 375: lib_Xamarin.AndroidX.Lifecycle.LiveData.Core.Ktx.dll.so => 274
	i64 u0x4b07a0ed0ab33ff4, ; 376: System.Runtime.Extensions.dll => 104
	i64 u0x4b576d47ac054f3c, ; 377: System.IO.FileSystem.AccessControl => 47
	i64 u0x4b7b6532ded934b7, ; 378: System.Text.Json => 138
	i64 u0x4b8f8ea3c2df6bb0, ; 379: System.ClientModel => 222
	i64 u0x4c7755cf07ad2d5f, ; 380: System.Net.Http.Json.dll => 64
	i64 u0x4cad38a03a928144, ; 381: lib_Xamarin.Firebase.Encoders.Proto.dll.so => 323
	i64 u0x4cc5f15266470798, ; 382: lib_Xamarin.AndroidX.Loader.dll.so => 285
	i64 u0x4cf6f67dc77aacd2, ; 383: System.Net.NetworkInformation.dll => 69
	i64 u0x4d3183dd245425d4, ; 384: System.Net.WebSockets.Client.dll => 80
	i64 u0x4d479f968a05e504, ; 385: System.Linq.Expressions.dll => 59
	i64 u0x4d55a010ffc4faff, ; 386: System.Private.Xml => 89
	i64 u0x4d5cbe77561c5b2e, ; 387: System.Web.dll => 154
	i64 u0x4d6001db23f8cd87, ; 388: lib_System.ClientModel.dll.so => 222
	i64 u0x4d77512dbd86ee4c, ; 389: lib_Xamarin.AndroidX.Arch.Core.Common.dll.so => 240
	i64 u0x4d7793536e79c309, ; 390: System.ServiceProcess => 133
	i64 u0x4d95fccc1f67c7ca, ; 391: System.Runtime.Loader.dll => 110
	i64 u0x4dcf44c3c9b076a2, ; 392: it/Microsoft.Maui.Controls.resources.dll => 384
	i64 u0x4dd9247f1d2c3235, ; 393: Xamarin.AndroidX.Loader.dll => 285
	i64 u0x4e2aeee78e2c4a87, ; 394: Xamarin.AndroidX.ProfileInstaller.ProfileInstaller => 293
	i64 u0x4e32f00cb0937401, ; 395: Mono.Android.Runtime => 171
	i64 u0x4e40e8cc9dee99de, ; 396: RMD.Movil => 0
	i64 u0x4e5eea4668ac2b18, ; 397: System.Text.Encoding.CodePages => 134
	i64 u0x4e742b34f53425d7, ; 398: Xamarin.Firebase.Encoders.Proto.dll => 323
	i64 u0x4ebd0c4b82c5eefc, ; 399: lib_System.Threading.Channels.dll.so => 140
	i64 u0x4ee8eaa9c9c1151a, ; 400: System.Globalization.Calendars => 40
	i64 u0x4f21ee6ef9eb527e, ; 401: ca/Microsoft.Maui.Controls.resources => 371
	i64 u0x4f27ca9d6e02176c, ; 402: cs/Microsoft.Data.SqlClient.resources => 357
	i64 u0x4fdc964ec1888e25, ; 403: lib_Microsoft.Extensions.Configuration.Binder.dll.so => 187
	i64 u0x4ffd65baff757598, ; 404: Microsoft.IdentityModel.Tokens => 206
	i64 u0x50320f2a19424f3f, ; 405: lib-it-Microsoft.Data.SqlClient.resources.dll.so => 361
	i64 u0x5037f0be3c28c7a3, ; 406: lib_Microsoft.Maui.Controls.dll.so => 207
	i64 u0x50bd8cfae0f07500, ; 407: lib_Xamarin.GooglePlayServices.Stats.dll.so => 342
	i64 u0x50c3a29b21050d45, ; 408: System.Linq.Parallel.dll => 60
	i64 u0x5112ed116d87baf8, ; 409: CommunityToolkit.Mvvm => 178
	i64 u0x5116b21580ae6eb0, ; 410: Microsoft.Extensions.Configuration.Binder.dll => 187
	i64 u0x5131bbe80989093f, ; 411: Xamarin.AndroidX.Lifecycle.ViewModel.Android.dll => 282
	i64 u0x5146d4e23aed3198, ; 412: ja/Microsoft.Data.SqlClient.resources => 362
	i64 u0x516324a5050a7e3c, ; 413: System.Net.WebProxy => 79
	i64 u0x516d6f0b21a303de, ; 414: lib_System.Diagnostics.Contracts.dll.so => 25
	i64 u0x516ef4cb3aa9654a, ; 415: OneSignalSDK.DotNet.Core => 218
	i64 u0x51bb8a2afe774e32, ; 416: System.Drawing => 36
	i64 u0x5247c5c32a4140f0, ; 417: System.Resources.Reader => 99
	i64 u0x526bb15e3c386364, ; 418: Xamarin.AndroidX.Lifecycle.Runtime.Ktx.dll => 278
	i64 u0x526ce79eb8e90527, ; 419: lib_System.Net.Primitives.dll.so => 71
	i64 u0x52829f00b4467c38, ; 420: lib_System.Data.Common.dll.so => 22
	i64 u0x5290402954d7bce0, ; 421: zh-Hans/Microsoft.Data.SqlClient.resources => 368
	i64 u0x529ffe06f39ab8db, ; 422: Xamarin.AndroidX.Core => 252
	i64 u0x52ff996554dbf352, ; 423: Microsoft.Maui.Graphics => 211
	i64 u0x535f7e40e8fef8af, ; 424: lib-sk-Microsoft.Maui.Controls.resources.dll.so => 395
	i64 u0x53978aac584c666e, ; 425: lib_System.Security.Cryptography.Cng.dll.so => 121
	i64 u0x53a96d5c86c9e194, ; 426: System.Net.NetworkInformation => 69
	i64 u0x53be1038a61e8d44, ; 427: System.Runtime.InteropServices.RuntimeInformation.dll => 107
	i64 u0x53c3014b9437e684, ; 428: lib-zh-HK-Microsoft.Maui.Controls.resources.dll.so => 401
	i64 u0x53e450ebd586f842, ; 429: lib_Xamarin.AndroidX.LocalBroadcastManager.dll.so => 286
	i64 u0x5435e6f049e9bc37, ; 430: System.Security.Claims.dll => 119
	i64 u0x54795225dd1587af, ; 431: lib_System.Runtime.dll.so => 117
	i64 u0x547a34f14e5f6210, ; 432: Xamarin.AndroidX.Lifecycle.Common.dll => 270
	i64 u0x556e8b63b660ab8b, ; 433: Xamarin.AndroidX.Lifecycle.Common.Jvm.dll => 271
	i64 u0x5588627c9a108ec9, ; 434: System.Collections.Specialized => 11
	i64 u0x55a898e4f42e3fae, ; 435: Microsoft.VisualBasic.Core.dll => 2
	i64 u0x55fa0c610fe93bb1, ; 436: lib_System.Security.Cryptography.OpenSsl.dll.so => 124
	i64 u0x56442b99bc64bb47, ; 437: System.Runtime.Serialization.Xml.dll => 115
	i64 u0x56a5d2c17db41bcb, ; 438: Xamarin.Google.Android.DataTransport.TransportRuntime => 331
	i64 u0x56a8b26e1aeae27b, ; 439: System.Threading.Tasks.Dataflow => 142
	i64 u0x56f932d61e93c07f, ; 440: System.Globalization.Extensions => 41
	i64 u0x56fbdd49dd4e9a1e, ; 441: Xamarin.AndroidX.Lifecycle.Service.dll => 280
	i64 u0x571c5cfbec5ae8e2, ; 442: System.Private.Uri => 87
	i64 u0x576499c9f52fea31, ; 443: Xamarin.AndroidX.Annotation => 235
	i64 u0x579a06fed6eec900, ; 444: System.Private.CoreLib.dll => 173
	i64 u0x57c542c14049b66d, ; 445: System.Diagnostics.DiagnosticSource => 27
	i64 u0x581a8bd5cfda563e, ; 446: System.Threading.Timer => 148
	i64 u0x584ac38e21d2fde1, ; 447: Microsoft.Extensions.Configuration.Binder => 187
	i64 u0x58601b2dda4a27b9, ; 448: lib-ja-Microsoft.Maui.Controls.resources.dll.so => 385
	i64 u0x58688d9af496b168, ; 449: Microsoft.Extensions.DependencyInjection.dll => 188
	i64 u0x588c167a79db6bfb, ; 450: lib_Xamarin.Google.ErrorProne.Annotations.dll.so => 336
	i64 u0x5906028ae5151104, ; 451: Xamarin.AndroidX.Activity.Ktx => 234
	i64 u0x595a356d23e8da9a, ; 452: lib_Microsoft.CSharp.dll.so => 1
	i64 u0x59f9e60b9475085f, ; 453: lib_Xamarin.AndroidX.Annotation.Experimental.dll.so => 236
	i64 u0x5a70033ca9d003cb, ; 454: lib_System.Memory.Data.dll.so => 226
	i64 u0x5a745f5101a75527, ; 455: lib_System.IO.Compression.FileSystem.dll.so => 44
	i64 u0x5a89a886ae30258d, ; 456: lib_Xamarin.AndroidX.CoordinatorLayout.dll.so => 251
	i64 u0x5a8f6699f4a1caa9, ; 457: lib_System.Threading.dll.so => 149
	i64 u0x5ae8e4f3eae4d547, ; 458: Xamarin.AndroidX.Legacy.Support.Core.Utils => 268
	i64 u0x5ae9cd33b15841bf, ; 459: System.ComponentModel => 18
	i64 u0x5b54391bdc6fcfe6, ; 460: System.Private.DataContractSerialization => 86
	i64 u0x5b5f0e240a06a2a2, ; 461: da/Microsoft.Maui.Controls.resources.dll => 373
	i64 u0x5b755276902c8414, ; 462: Xamarin.GooglePlayServices.Base => 339
	i64 u0x5b8109e8e14c5e3e, ; 463: System.Globalization.Extensions.dll => 41
	i64 u0x5bddd04d72a9e350, ; 464: Xamarin.AndroidX.Lifecycle.LiveData.Core.Ktx => 274
	i64 u0x5bdf16b09da116ab, ; 465: Xamarin.AndroidX.Collection => 245
	i64 u0x5bff6a70194300bd, ; 466: lib_Xamarin.Kotlin.StdLib.Jdk8.dll.so => 349
	i64 u0x5c019d5266093159, ; 467: lib_Xamarin.AndroidX.Lifecycle.Runtime.Ktx.Android.dll.so => 279
	i64 u0x5c30a4a35f9cc8c4, ; 468: lib_System.Reflection.Extensions.dll.so => 94
	i64 u0x5c393624b8176517, ; 469: lib_Microsoft.Extensions.Logging.dll.so => 193
	i64 u0x5c53c29f5073b0c9, ; 470: System.Diagnostics.FileVersionInfo => 28
	i64 u0x5c87463c575c7616, ; 471: lib_System.Globalization.Extensions.dll.so => 41
	i64 u0x5d0a4a29b02d9d3c, ; 472: System.Net.WebHeaderCollection.dll => 78
	i64 u0x5d33da2f84c1de97, ; 473: lib-pt-BR-Microsoft.Data.SqlClient.resources.dll.so => 365
	i64 u0x5d40c9b15181641f, ; 474: lib_Xamarin.AndroidX.Emoji2.dll.so => 261
	i64 u0x5d6ca10d35e9485b, ; 475: lib_Xamarin.AndroidX.Concurrent.Futures.dll.so => 248
	i64 u0x5d7960d446a1890e, ; 476: lib-pl-Microsoft.Data.SqlClient.resources.dll.so => 364
	i64 u0x5d7ec76c1c703055, ; 477: System.Threading.Tasks.Parallel => 144
	i64 u0x5db0cbbd1028510e, ; 478: lib_System.Runtime.InteropServices.dll.so => 108
	i64 u0x5db30905d3e5013b, ; 479: Xamarin.AndroidX.Collection.Jvm.dll => 246
	i64 u0x5dd10b96620cce23, ; 480: Xamarin.Firebase.Iid.Interop => 324
	i64 u0x5e467bc8f09ad026, ; 481: System.Collections.Specialized.dll => 11
	i64 u0x5e5173b3208d97e7, ; 482: System.Runtime.Handles.dll => 105
	i64 u0x5ea92fdb19ec8c4c, ; 483: System.Text.Encodings.Web.dll => 137
	i64 u0x5eb8046dd40e9ac3, ; 484: System.ComponentModel.Primitives => 16
	i64 u0x5ec272d219c9aba4, ; 485: System.Security.Cryptography.Csp.dll => 122
	i64 u0x5eee1376d94c7f5e, ; 486: System.Net.HttpListener.dll => 66
	i64 u0x5f36ccf5c6a57e24, ; 487: System.Xml.ReaderWriter.dll => 157
	i64 u0x5f4294b9b63cb842, ; 488: System.Data.Common => 22
	i64 u0x5f4fa8b9ffd0e2c5, ; 489: lib_Xamarin.Google.Android.DataTransport.TransportApi.dll.so => 329
	i64 u0x5f9a2d823f664957, ; 490: lib-el-Microsoft.Maui.Controls.resources.dll.so => 375
	i64 u0x5fa6da9c3cd8142a, ; 491: lib_Xamarin.KotlinX.Serialization.Core.dll.so => 355
	i64 u0x5fac98e0b37a5b9d, ; 492: System.Runtime.CompilerServices.Unsafe.dll => 102
	i64 u0x609f4b7b63d802d4, ; 493: lib_Microsoft.Extensions.DependencyInjection.dll.so => 188
	i64 u0x60cd4e33d7e60134, ; 494: Xamarin.KotlinX.Coroutines.Core.Jvm => 354
	i64 u0x60ee72f71c4b3113, ; 495: Xamarin.Firebase.Iid.Interop.dll => 324
	i64 u0x60f62d786afcf130, ; 496: System.Memory => 63
	i64 u0x61bb78c89f867353, ; 497: System.IO => 58
	i64 u0x61be8d1299194243, ; 498: Microsoft.Maui.Controls.Xaml => 208
	i64 u0x61d2cba29557038f, ; 499: de/Microsoft.Maui.Controls.resources => 374
	i64 u0x61d88f399afb2f45, ; 500: lib_System.Runtime.Loader.dll.so => 110
	i64 u0x6219beeff33faa04, ; 501: cs/Microsoft.Data.SqlClient.resources.dll => 357
	i64 u0x622eef6f9e59068d, ; 502: System.Private.CoreLib => 173
	i64 u0x625def565caafc1c, ; 503: tr/Microsoft.Data.SqlClient.resources.dll => 367
	i64 u0x62812a93be7f3265, ; 504: Microsoft.Bcl.Cryptography => 181
	i64 u0x63cdbd66ac39bb46, ; 505: lib_Microsoft.VisualStudio.DesignTools.XamlTapContract.dll.so => 407
	i64 u0x63d5e3aa4ef9b931, ; 506: Xamarin.KotlinX.Coroutines.Android.dll => 352
	i64 u0x63f1f6883c1e23c2, ; 507: lib_System.Collections.Immutable.dll.so => 9
	i64 u0x6400f68068c1e9f1, ; 508: Xamarin.Google.Android.Material.dll => 332
	i64 u0x640e3b14dbd325c2, ; 509: System.Security.Cryptography.Algorithms.dll => 120
	i64 u0x64587004560099b9, ; 510: System.Reflection => 98
	i64 u0x6468947914df5c9c, ; 511: Microsoft.Bcl.Cryptography.dll => 181
	i64 u0x646bbda4b2219cc2, ; 512: lib_Xamarin.Firebase.Datatransport.dll.so => 320
	i64 u0x64b1529a438a3c45, ; 513: lib_System.Runtime.Handles.dll.so => 105
	i64 u0x6565fba2cd8f235b, ; 514: Xamarin.AndroidX.Lifecycle.ViewModel.Ktx => 283
	i64 u0x658f524e4aba7dad, ; 515: CommunityToolkit.Maui.dll => 176
	i64 u0x65ecac39144dd3cc, ; 516: Microsoft.Maui.Controls.dll => 207
	i64 u0x65ece51227bfa724, ; 517: lib_System.Runtime.Numerics.dll.so => 111
	i64 u0x661722438787b57f, ; 518: Xamarin.AndroidX.Annotation.Jvm.dll => 237
	i64 u0x6679b2337ee6b22a, ; 519: lib_System.IO.FileSystem.Primitives.dll.so => 49
	i64 u0x6692e924eade1b29, ; 520: lib_System.Console.dll.so => 20
	i64 u0x669b2e9020ef2b4e, ; 521: Xamarin.Firebase.Datatransport => 320
	i64 u0x66a4e5c6a3fb0bae, ; 522: lib_Xamarin.AndroidX.Lifecycle.ViewModel.Android.dll.so => 282
	i64 u0x66d13304ce1a3efa, ; 523: Xamarin.AndroidX.CursorAdapter => 255
	i64 u0x674303f65d8fad6f, ; 524: lib_System.Net.Quic.dll.so => 72
	i64 u0x6756ca4cad62e9d6, ; 525: lib_Xamarin.AndroidX.ConstraintLayout.Core.dll.so => 250
	i64 u0x67bca11acb828d8d, ; 526: lib_Xamarin.Google.Android.DataTransport.TransportBackendCct.dll.so => 330
	i64 u0x67c0802770244408, ; 527: System.Windows.dll => 155
	i64 u0x68100b69286e27cd, ; 528: lib_System.Formats.Tar.dll.so => 39
	i64 u0x68558ec653afa616, ; 529: lib-da-Microsoft.Maui.Controls.resources.dll.so => 373
	i64 u0x6872ec7a2e36b1ac, ; 530: System.Drawing.Primitives.dll => 35
	i64 u0x68bb2c417aa9b61c, ; 531: Xamarin.KotlinX.AtomicFU.dll => 350
	i64 u0x68fbbbe2eb455198, ; 532: System.Formats.Asn1 => 38
	i64 u0x69063fc0ba8e6bdd, ; 533: he/Microsoft.Maui.Controls.resources.dll => 379
	i64 u0x69a3e26c76f6eec4, ; 534: Xamarin.AndroidX.Window.Extensions.Core.Core.dll => 314
	i64 u0x69a49b7740c938f4, ; 535: Xamarin.AndroidX.Work.Runtime => 315
	i64 u0x6a4d7577b2317255, ; 536: System.Runtime.InteropServices.dll => 108
	i64 u0x6ace3b74b15ee4a4, ; 537: nb/Microsoft.Maui.Controls.resources => 388
	i64 u0x6afcedb171067e2b, ; 538: System.Core.dll => 21
	i64 u0x6b6385bc3b5020d0, ; 539: lib_OneSignalSDK.DotNet.Android.dll.so => 217
	i64 u0x6bef98e124147c24, ; 540: Xamarin.Jetbrains.Annotations => 345
	i64 u0x6ce874bff138ce2b, ; 541: Xamarin.AndroidX.Lifecycle.ViewModel.dll => 281
	i64 u0x6d0a12b2adba20d8, ; 542: System.Security.Cryptography.ProtectedData.dll => 228
	i64 u0x6d12bfaa99c72b1f, ; 543: lib_Microsoft.Maui.Graphics.dll.so => 211
	i64 u0x6d3b7628f8253e93, ; 544: pl/Microsoft.Data.SqlClient.resources => 364
	i64 u0x6d70755158ca866e, ; 545: lib_System.ComponentModel.EventBasedAsync.dll.so => 15
	i64 u0x6d79993361e10ef2, ; 546: Microsoft.Extensions.Primitives => 198
	i64 u0x6d7eeca99577fc8b, ; 547: lib_System.Net.WebProxy.dll.so => 79
	i64 u0x6d8515b19946b6a2, ; 548: System.Net.WebProxy.dll => 79
	i64 u0x6d86d56b84c8eb71, ; 549: lib_Xamarin.AndroidX.CursorAdapter.dll.so => 255
	i64 u0x6d9bea6b3e895cf7, ; 550: Microsoft.Extensions.Primitives.dll => 198
	i64 u0x6dd9bf4083de3f6a, ; 551: Xamarin.AndroidX.DocumentFile.dll => 258
	i64 u0x6e25a02c3833319a, ; 552: lib_Xamarin.AndroidX.Navigation.Fragment.dll.so => 289
	i64 u0x6e79c6bd8627412a, ; 553: Xamarin.AndroidX.SavedState.SavedState.Ktx => 299
	i64 u0x6e838d9a2a6f6c9e, ; 554: lib_System.ValueTuple.dll.so => 152
	i64 u0x6e9965ce1095e60a, ; 555: lib_System.Core.dll.so => 21
	i64 u0x6f304d1dec1019e9, ; 556: lib_Xamarin.Firebase.Installations.dll.so => 325
	i64 u0x6fd2265da78b93a4, ; 557: lib_Microsoft.Maui.dll.so => 209
	i64 u0x6fdfc7de82c33008, ; 558: cs/Microsoft.Maui.Controls.resources => 372
	i64 u0x6ffc4967cc47ba57, ; 559: System.IO.FileSystem.Watcher.dll => 50
	i64 u0x701cd46a1c25a5fe, ; 560: System.IO.FileSystem.dll => 51
	i64 u0x709688f4ec2831e9, ; 561: lib_Xamarin.AndroidX.Media.dll.so => 287
	i64 u0x70e99f48c05cb921, ; 562: tr/Microsoft.Maui.Controls.resources.dll => 398
	i64 u0x70fd3deda22442d2, ; 563: lib-nb-Microsoft.Maui.Controls.resources.dll.so => 388
	i64 u0x71485e7ffdb4b958, ; 564: System.Reflection.Extensions => 94
	i64 u0x7162a2fce67a945f, ; 565: lib_Xamarin.Android.Glide.Annotations.dll.so => 230
	i64 u0x717530326f808838, ; 566: lib_Microsoft.Extensions.Diagnostics.Abstractions.dll.so => 191
	i64 u0x71a495ea3761dde8, ; 567: lib-it-Microsoft.Maui.Controls.resources.dll.so => 384
	i64 u0x71ad672adbe48f35, ; 568: System.ComponentModel.Primitives.dll => 16
	i64 u0x71bc142d620e986a, ; 569: lib_System.Security.Cryptography.Pkcs.dll.so => 227
	i64 u0x720f102581a4a5c8, ; 570: Xamarin.AndroidX.Core.ViewTree => 254
	i64 u0x725f5a9e82a45c81, ; 571: System.Security.Cryptography.Encoding => 123
	i64 u0x72b1fb4109e08d7b, ; 572: lib-hr-Microsoft.Maui.Controls.resources.dll.so => 381
	i64 u0x72e0300099accce1, ; 573: System.Xml.XPath.XDocument => 160
	i64 u0x730bfb248998f67a, ; 574: System.IO.Compression.ZipFile => 45
	i64 u0x732b2d67b9e5c47b, ; 575: Xamarin.Google.ErrorProne.Annotations.dll => 336
	i64 u0x734b76fdc0dc05bb, ; 576: lib_GoogleGson.dll.so => 179
	i64 u0x73a6be34e822f9d1, ; 577: lib_System.Runtime.Serialization.dll.so => 116
	i64 u0x73e4ce94e2eb6ffc, ; 578: lib_System.Memory.dll.so => 63
	i64 u0x743a1eccf080489a, ; 579: WindowsBase.dll => 166
	i64 u0x746cf89b511b4d40, ; 580: lib_Microsoft.Extensions.Diagnostics.dll.so => 190
	i64 u0x755a91767330b3d4, ; 581: lib_Microsoft.Extensions.Configuration.dll.so => 185
	i64 u0x75c326eb821b85c4, ; 582: lib_System.ComponentModel.DataAnnotations.dll.so => 14
	i64 u0x76012e7334db86e5, ; 583: lib_Xamarin.AndroidX.SavedState.dll.so => 298
	i64 u0x76ca07b878f44da0, ; 584: System.Runtime.Numerics.dll => 111
	i64 u0x7736c8a96e51a061, ; 585: lib_Xamarin.AndroidX.Annotation.Jvm.dll.so => 237
	i64 u0x7762034089fe3533, ; 586: OneSignalSDK.DotNet.Android.Notifications.Binding.dll => 216
	i64 u0x778a805e625329ef, ; 587: System.Linq.Parallel => 60
	i64 u0x779290cc2b801eb7, ; 588: Xamarin.KotlinX.AtomicFU.Jvm => 351
	i64 u0x77f8a4acc2fdc449, ; 589: System.Security.Cryptography.Cng.dll => 121
	i64 u0x780bc73597a503a9, ; 590: lib-ms-Microsoft.Maui.Controls.resources.dll.so => 387
	i64 u0x782c5d8eb99ff201, ; 591: lib_Microsoft.VisualBasic.Core.dll.so => 2
	i64 u0x783606d1e53e7a1a, ; 592: th/Microsoft.Maui.Controls.resources.dll => 397
	i64 u0x784b4ff3eed363ff, ; 593: Xamarin.Firebase.Common => 318
	i64 u0x78a0275e998ae4ef, ; 594: Xamarin.Firebase.Measurement.Connector.dll => 327
	i64 u0x78a45e51311409b6, ; 595: Xamarin.AndroidX.Fragment.dll => 264
	i64 u0x78d5c74e565733ea, ; 596: Xamarin.Google.Android.DataTransport.TransportRuntime.dll => 331
	i64 u0x78ed4ab8f9d800a1, ; 597: Xamarin.AndroidX.Lifecycle.ViewModel => 281
	i64 u0x793546dbadd324b1, ; 598: Xamarin.Google.Android.DataTransport.TransportApi => 329
	i64 u0x79eb916f2d11e1f0, ; 599: zh-Hans/Microsoft.Data.SqlClient.resources.dll => 368
	i64 u0x7a25bdb29108c6e7, ; 600: Microsoft.Extensions.Http => 192
	i64 u0x7a39601d6f0bb831, ; 601: lib_Xamarin.KotlinX.AtomicFU.dll.so => 350
	i64 u0x7a5207a7c82d30b4, ; 602: lib_Xamarin.JSpecify.dll.so => 346
	i64 u0x7a7e7eddf79c5d26, ; 603: lib_Xamarin.AndroidX.Lifecycle.ViewModel.dll.so => 281
	i64 u0x7a9a57d43b0845fa, ; 604: System.AppContext => 6
	i64 u0x7ad0f4f1e5d08183, ; 605: Xamarin.AndroidX.Collection.dll => 245
	i64 u0x7adb8da2ac89b647, ; 606: fi/Microsoft.Maui.Controls.resources.dll => 377
	i64 u0x7b13d9eaa944ade8, ; 607: Xamarin.AndroidX.DynamicAnimation.dll => 260
	i64 u0x7b4927e421291c41, ; 608: Microsoft.IdentityModel.JsonWebTokens.dll => 202
	i64 u0x7bef86a4335c4870, ; 609: System.ComponentModel.TypeConverter => 17
	i64 u0x7c0820144cd34d6a, ; 610: sk/Microsoft.Maui.Controls.resources.dll => 395
	i64 u0x7c2a0bd1e0f988fc, ; 611: lib-de-Microsoft.Maui.Controls.resources.dll.so => 374
	i64 u0x7c41d387501568ba, ; 612: System.Net.WebClient.dll => 77
	i64 u0x7c482cd79bd24b13, ; 613: lib_Xamarin.AndroidX.ConstraintLayout.dll.so => 249
	i64 u0x7cb95ad2a929d044, ; 614: Xamarin.GooglePlayServices.Basement => 340
	i64 u0x7cc637f941f716d0, ; 615: CommunityToolkit.Maui.Core => 177
	i64 u0x7cd2ec8eaf5241cd, ; 616: System.Security.dll => 131
	i64 u0x7cf9ae50dd350622, ; 617: Xamarin.Jetbrains.Annotations.dll => 345
	i64 u0x7d649b75d580bb42, ; 618: ms/Microsoft.Maui.Controls.resources.dll => 387
	i64 u0x7d8ee2bdc8e3aad1, ; 619: System.Numerics.Vectors => 83
	i64 u0x7df5df8db8eaa6ac, ; 620: Microsoft.Extensions.Logging.Debug => 195
	i64 u0x7dfc3d6d9d8d7b70, ; 621: System.Collections => 12
	i64 u0x7e2e564fa2f76c65, ; 622: lib_System.Diagnostics.Tracing.dll.so => 34
	i64 u0x7e302e110e1e1346, ; 623: lib_System.Security.Claims.dll.so => 119
	i64 u0x7e4465b3f78ad8d0, ; 624: Xamarin.KotlinX.Serialization.Core.dll => 355
	i64 u0x7e571cad5915e6c3, ; 625: lib_Xamarin.AndroidX.Lifecycle.Process.dll.so => 275
	i64 u0x7e6b1ca712437d7d, ; 626: Xamarin.AndroidX.Emoji2.ViewsHelper => 262
	i64 u0x7e946809d6008ef2, ; 627: lib_System.ObjectModel.dll.so => 85
	i64 u0x7ea0272c1b4a9635, ; 628: lib_Xamarin.Android.Glide.dll.so => 229
	i64 u0x7eb4f0dc47488736, ; 629: lib_Xamarin.GooglePlayServices.Tasks.dll.so => 343
	i64 u0x7ecc13347c8fd849, ; 630: lib_System.ComponentModel.dll.so => 18
	i64 u0x7f00ddd9b9ca5a13, ; 631: Xamarin.AndroidX.ViewPager.dll => 311
	i64 u0x7f9351cd44b1273f, ; 632: Microsoft.Extensions.Configuration.Abstractions => 186
	i64 u0x7fae0ef4dc4770fe, ; 633: Microsoft.Identity.Client => 199
	i64 u0x7fbd557c99b3ce6f, ; 634: lib_Xamarin.AndroidX.Lifecycle.LiveData.Core.dll.so => 273
	i64 u0x8076a9a44a2ca331, ; 635: System.Net.Quic => 72
	i64 u0x80b7e726b0280681, ; 636: Microsoft.VisualStudio.DesignTools.MobileTapContracts => 405
	i64 u0x80ca3d88b0159b22, ; 637: lib_Xamarin.AndroidX.Legacy.Support.Core.UI.dll.so => 267
	i64 u0x80da183a87731838, ; 638: System.Reflection.Metadata => 95
	i64 u0x8101b635bac5d3fb, ; 639: RMD.Shared.Utils => 410
	i64 u0x812c069d5cdecc17, ; 640: System.dll => 165
	i64 u0x81381be520a60adb, ; 641: Xamarin.AndroidX.Interpolator.dll => 266
	i64 u0x81657cec2b31e8aa, ; 642: System.Net => 82
	i64 u0x81ab745f6c0f5ce6, ; 643: zh-Hant/Microsoft.Maui.Controls.resources => 403
	i64 u0x825767256f457674, ; 644: lib_Xamarin.AndroidX.Legacy.Support.V4.dll.so => 269
	i64 u0x8277f2be6b5ce05f, ; 645: Xamarin.AndroidX.AppCompat => 238
	i64 u0x828f06563b30bc50, ; 646: lib_Xamarin.AndroidX.CardView.dll.so => 244
	i64 u0x82920a8d9194a019, ; 647: Xamarin.KotlinX.AtomicFU.Jvm.dll => 351
	i64 u0x82b399cb01b531c4, ; 648: lib_System.Web.dll.so => 154
	i64 u0x82df8f5532a10c59, ; 649: lib_System.Drawing.dll.so => 36
	i64 u0x82f0b6e911d13535, ; 650: lib_System.Transactions.dll.so => 151
	i64 u0x82f6403342e12049, ; 651: uk/Microsoft.Maui.Controls.resources => 399
	i64 u0x83a7afd2c49adc86, ; 652: lib_Microsoft.IdentityModel.Abstractions.dll.so => 201
	i64 u0x83c14ba66c8e2b8c, ; 653: zh-Hans/Microsoft.Maui.Controls.resources => 402
	i64 u0x846ce984efea52c7, ; 654: System.Threading.Tasks.Parallel.dll => 144
	i64 u0x84ae73148a4557d2, ; 655: lib_System.IO.Pipes.dll.so => 56
	i64 u0x84b01102c12a9232, ; 656: System.Runtime.Serialization.Json.dll => 113
	i64 u0x850c5ba0b57ce8e7, ; 657: lib_Xamarin.AndroidX.Collection.dll.so => 245
	i64 u0x851d02edd334b044, ; 658: Xamarin.AndroidX.VectorDrawable => 308
	i64 u0x8528b82bdbc15371, ; 659: ko/Microsoft.Data.SqlClient.resources => 363
	i64 u0x85440c605de848f2, ; 660: lib_Xamarin.AndroidX.Lifecycle.Service.dll.so => 280
	i64 u0x85c919db62150978, ; 661: Xamarin.AndroidX.Transition.dll => 307
	i64 u0x8662aaeb94fef37f, ; 662: lib_System.Dynamic.Runtime.dll.so => 37
	i64 u0x86a909228dc7657b, ; 663: lib-zh-Hant-Microsoft.Maui.Controls.resources.dll.so => 403
	i64 u0x86b3e00c36b84509, ; 664: Microsoft.Extensions.Configuration.dll => 185
	i64 u0x86b62cb077ec4fd7, ; 665: System.Runtime.Serialization.Xml => 115
	i64 u0x8706ffb12bf3f53d, ; 666: Xamarin.AndroidX.Annotation.Experimental => 236
	i64 u0x872a5b14c18d328c, ; 667: System.ComponentModel.DataAnnotations => 14
	i64 u0x872fb9615bc2dff0, ; 668: Xamarin.Android.Glide.Annotations.dll => 230
	i64 u0x87c69b87d9283884, ; 669: lib_System.Threading.Thread.dll.so => 146
	i64 u0x87f6569b25707834, ; 670: System.IO.Compression.Brotli.dll => 43
	i64 u0x8842b3a5d2d3fb36, ; 671: Microsoft.Maui.Essentials => 210
	i64 u0x88926583efe7ee86, ; 672: Xamarin.AndroidX.Activity.Ktx.dll => 234
	i64 u0x88b16a1a7051ebe2, ; 673: Xamarin.Firebase.Annotations.dll => 317
	i64 u0x88ba6bc4f7762b03, ; 674: lib_System.Reflection.dll.so => 98
	i64 u0x88bda98e0cffb7a9, ; 675: lib_Xamarin.KotlinX.Coroutines.Core.Jvm.dll.so => 354
	i64 u0x8930322c7bd8f768, ; 676: netstandard => 168
	i64 u0x897a606c9e39c75f, ; 677: lib_System.ComponentModel.Primitives.dll.so => 16
	i64 u0x89911a22005b92b7, ; 678: System.IO.FileSystem.DriveInfo.dll => 48
	i64 u0x89c5188089ec2cd5, ; 679: lib_System.Runtime.InteropServices.RuntimeInformation.dll.so => 107
	i64 u0x8a19e3dc71b34b2c, ; 680: System.Reflection.TypeExtensions.dll => 97
	i64 u0x8a399a706fcbce4b, ; 681: Microsoft.Extensions.Caching.Abstractions => 183
	i64 u0x8a9b37be263c8614, ; 682: Xamarin.AndroidX.Sqlite.Framework => 303
	i64 u0x8ad229ea26432ee2, ; 683: Xamarin.AndroidX.Loader => 285
	i64 u0x8b4ff5d0fdd5faa1, ; 684: lib_System.Diagnostics.DiagnosticSource.dll.so => 27
	i64 u0x8b541d476eb3774c, ; 685: System.Security.Principal.Windows => 128
	i64 u0x8b8d01333a96d0b5, ; 686: System.Diagnostics.Process.dll => 29
	i64 u0x8b9278242f21e276, ; 687: Xamarin.Firebase.Encoders.JSON.dll => 322
	i64 u0x8b9ceca7acae3451, ; 688: lib-he-Microsoft.Maui.Controls.resources.dll.so => 379
	i64 u0x8c156fe7f184f137, ; 689: tr/Microsoft.Data.SqlClient.resources => 367
	i64 u0x8c53ae18581b14f0, ; 690: Azure.Core => 174
	i64 u0x8cb8f612b633affb, ; 691: Xamarin.AndroidX.SavedState.SavedState.Ktx.dll => 299
	i64 u0x8cdfdb4ce85fb925, ; 692: lib_System.Security.Principal.Windows.dll.so => 128
	i64 u0x8cdfe7b8f4caa426, ; 693: System.IO.Compression.FileSystem => 44
	i64 u0x8d0f420977c2c1c7, ; 694: Xamarin.AndroidX.CursorAdapter.dll => 255
	i64 u0x8d52f7ea2796c531, ; 695: Xamarin.AndroidX.Emoji2.dll => 261
	i64 u0x8d7b8ab4b3310ead, ; 696: System.Threading => 149
	i64 u0x8da188285aadfe8e, ; 697: System.Collections.Concurrent => 8
	i64 u0x8e623fec9635e28f, ; 698: Syncfusion.Maui.Toolkit.resources.dll => 221
	i64 u0x8e937db395a74375, ; 699: lib_Microsoft.Identity.Client.dll.so => 199
	i64 u0x8ed3cdd722b4d782, ; 700: System.Diagnostics.EventLog => 224
	i64 u0x8ed807bfe9858dfc, ; 701: Xamarin.AndroidX.Navigation.Common => 288
	i64 u0x8ee08b8194a30f48, ; 702: lib-hi-Microsoft.Maui.Controls.resources.dll.so => 380
	i64 u0x8ef7601039857a44, ; 703: lib-ro-Microsoft.Maui.Controls.resources.dll.so => 393
	i64 u0x8efbc0801a122264, ; 704: Xamarin.GooglePlayServices.Tasks.dll => 343
	i64 u0x8f32c6f611f6ffab, ; 705: pt/Microsoft.Maui.Controls.resources.dll => 392
	i64 u0x8f44b45eb046bbd1, ; 706: System.ServiceModel.Web.dll => 132
	i64 u0x8f8829d21c8985a4, ; 707: lib-pt-BR-Microsoft.Maui.Controls.resources.dll.so => 391
	i64 u0x8fab1ee0e9c09e0b, ; 708: RMD.Shared.Models => 409
	i64 u0x8fbf5b0114c6dcef, ; 709: System.Globalization.dll => 42
	i64 u0x8fcc8c2a81f3d9e7, ; 710: Xamarin.KotlinX.Serialization.Core => 355
	i64 u0x90263f8448b8f572, ; 711: lib_System.Diagnostics.TraceSource.dll.so => 33
	i64 u0x903101b46fb73a04, ; 712: _Microsoft.Android.Resource.Designer => 411
	i64 u0x90393bd4865292f3, ; 713: lib_System.IO.Compression.dll.so => 46
	i64 u0x905e2b8e7ae91ae6, ; 714: System.Threading.Tasks.Extensions.dll => 143
	i64 u0x90634f86c5ebe2b5, ; 715: Xamarin.AndroidX.Lifecycle.ViewModel.Android => 282
	i64 u0x907b636704ad79ef, ; 716: lib_Microsoft.Maui.Controls.Xaml.dll.so => 208
	i64 u0x90e9efbfd68593e0, ; 717: lib_Xamarin.AndroidX.Lifecycle.LiveData.dll.so => 272
	i64 u0x91418dc638b29e68, ; 718: lib_Xamarin.AndroidX.CustomView.dll.so => 256
	i64 u0x9157bd523cd7ed36, ; 719: lib_System.Text.Json.dll.so => 138
	i64 u0x91a74f07b30d37e2, ; 720: System.Linq.dll => 62
	i64 u0x91cb86ea3b17111d, ; 721: System.ServiceModel.Web => 132
	i64 u0x91fa41a87223399f, ; 722: ca/Microsoft.Maui.Controls.resources.dll => 371
	i64 u0x92054e486c0c7ea7, ; 723: System.IO.FileSystem.DriveInfo => 48
	i64 u0x928614058c40c4cd, ; 724: lib_System.Xml.XPath.XDocument.dll.so => 160
	i64 u0x92a698e6d582778f, ; 725: Xamarin.Firebase.Components.dll => 319
	i64 u0x92b138fffca2b01e, ; 726: lib_Xamarin.AndroidX.Arch.Core.Runtime.dll.so => 241
	i64 u0x92dd6c6033393bf7, ; 727: Syncfusion.Maui.Toolkit.resources => 221
	i64 u0x92dfc2bfc6c6a888, ; 728: Xamarin.AndroidX.Lifecycle.LiveData => 272
	i64 u0x933da2c779423d68, ; 729: Xamarin.Android.Glide.Annotations => 230
	i64 u0x93489853b6098685, ; 730: es/Microsoft.Data.SqlClient.resources.dll => 359
	i64 u0x9388aad9b7ae40ce, ; 731: lib_Xamarin.AndroidX.Lifecycle.Common.dll.so => 270
	i64 u0x93a39f9bbc3b3ca4, ; 732: Xamarin.GooglePlayServices.CloudMessaging.dll => 341
	i64 u0x93cfa73ab28d6e35, ; 733: ms/Microsoft.Maui.Controls.resources => 387
	i64 u0x941c00d21e5c0679, ; 734: lib_Xamarin.AndroidX.Transition.dll.so => 307
	i64 u0x944077d8ca3c6580, ; 735: System.IO.Compression.dll => 46
	i64 u0x948cffedc8ed7960, ; 736: System.Xml => 164
	i64 u0x948d746a7702861f, ; 737: Microsoft.IdentityModel.Logging.dll => 203
	i64 u0x94c8990839c4bdb1, ; 738: lib_Xamarin.AndroidX.Interpolator.dll.so => 266
	i64 u0x9502fd818eed2359, ; 739: lib_Microsoft.IdentityModel.Protocols.OpenIdConnect.dll.so => 205
	i64 u0x9564283c37ed59a9, ; 740: lib_Microsoft.IdentityModel.Logging.dll.so => 203
	i64 u0x965d480cfb8de46d, ; 741: pl/Microsoft.Data.SqlClient.resources.dll => 364
	i64 u0x967fc325e09bfa8c, ; 742: es/Microsoft.Maui.Controls.resources => 376
	i64 u0x9686161486d34b81, ; 743: lib_Xamarin.AndroidX.ExifInterface.dll.so => 263
	i64 u0x96e49b31fe33d427, ; 744: Microsoft.Identity.Client.Extensions.Msal => 200
	i64 u0x9732d8dbddea3d9a, ; 745: id/Microsoft.Maui.Controls.resources => 383
	i64 u0x978be80e5210d31b, ; 746: Microsoft.Maui.Graphics.dll => 211
	i64 u0x979ab54025cc1c7f, ; 747: lib_Xamarin.GooglePlayServices.Base.dll.so => 339
	i64 u0x97b8c771ea3e4220, ; 748: System.ComponentModel.dll => 18
	i64 u0x97e144c9d3c6976e, ; 749: System.Collections.Concurrent.dll => 8
	i64 u0x97e55f96df4ddd72, ; 750: lib_Xamarin.Firebase.Annotations.dll.so => 317
	i64 u0x984184e3c70d4419, ; 751: GoogleGson => 179
	i64 u0x9843944103683dd3, ; 752: Xamarin.AndroidX.Core.Core.Ktx => 253
	i64 u0x98d720cc4597562c, ; 753: System.Security.Cryptography.OpenSsl => 124
	i64 u0x991d510397f92d9d, ; 754: System.Linq.Expressions => 59
	i64 u0x996ceeb8a3da3d67, ; 755: System.Threading.Overlapped.dll => 141
	i64 u0x999cb19e1a04ffd3, ; 756: CommunityToolkit.Mvvm.dll => 178
	i64 u0x99a00ca5270c6878, ; 757: Xamarin.AndroidX.Navigation.Runtime => 290
	i64 u0x99cdc6d1f2d3a72f, ; 758: ko/Microsoft.Maui.Controls.resources.dll => 386
	i64 u0x9a01b1da98b6ee10, ; 759: Xamarin.AndroidX.Lifecycle.Runtime.dll => 276
	i64 u0x9a0cc42c6f36dfc9, ; 760: lib_Microsoft.IdentityModel.Protocols.dll.so => 204
	i64 u0x9a5ccc274fd6e6ee, ; 761: Jsr305Binding.dll => 333
	i64 u0x9ae6940b11c02876, ; 762: lib_Xamarin.AndroidX.Window.dll.so => 313
	i64 u0x9b211a749105beac, ; 763: System.Transactions.Local => 150
	i64 u0x9b8734714671022d, ; 764: System.Threading.Tasks.Dataflow.dll => 142
	i64 u0x9bc6aea27fbf034f, ; 765: lib_Xamarin.KotlinX.Coroutines.Core.dll.so => 353
	i64 u0x9bd8cc74558ad4c7, ; 766: Xamarin.KotlinX.AtomicFU => 350
	i64 u0x9c244ac7cda32d26, ; 767: System.Security.Cryptography.X509Certificates.dll => 126
	i64 u0x9c465f280cf43733, ; 768: lib_Xamarin.KotlinX.Coroutines.Android.dll.so => 352
	i64 u0x9c4dd9126a77a711, ; 769: Xamarin.Firebase.Installations.InterOp => 326
	i64 u0x9c8f6872beab6408, ; 770: System.Xml.XPath.XDocument.dll => 160
	i64 u0x9ce01cf91101ae23, ; 771: System.Xml.XmlDocument => 162
	i64 u0x9d128180c81d7ce6, ; 772: Xamarin.AndroidX.CustomView.PoolingContainer => 257
	i64 u0x9d5dbcf5a48583fe, ; 773: lib_Xamarin.AndroidX.Activity.dll.so => 233
	i64 u0x9d74dee1a7725f34, ; 774: Microsoft.Extensions.Configuration.Abstractions.dll => 186
	i64 u0x9de64d56c52fba95, ; 775: Xamarin.Firebase.Encoders.Proto => 323
	i64 u0x9e4534b6adaf6e84, ; 776: nl/Microsoft.Maui.Controls.resources => 389
	i64 u0x9e4b95dec42769f7, ; 777: System.Diagnostics.Debug.dll => 26
	i64 u0x9eaf1efdf6f7267e, ; 778: Xamarin.AndroidX.Navigation.Common.dll => 288
	i64 u0x9ef542cf1f78c506, ; 779: Xamarin.AndroidX.Lifecycle.LiveData.Core => 273
	i64 u0x9ffbb6b1434ad2df, ; 780: Microsoft.Identity.Client.dll => 199
	i64 u0xa00832eb975f56a8, ; 781: lib_System.Net.dll.so => 82
	i64 u0xa0ad78236b7b267f, ; 782: Xamarin.AndroidX.Window => 313
	i64 u0xa0d8259f4cc284ec, ; 783: lib_System.Security.Cryptography.dll.so => 127
	i64 u0xa0e17ca50c77a225, ; 784: lib_Xamarin.Google.Crypto.Tink.Android.dll.so => 334
	i64 u0xa0ff9b3e34d92f11, ; 785: lib_System.Resources.Writer.dll.so => 101
	i64 u0xa12fbfb4da97d9f3, ; 786: System.Threading.Timer.dll => 148
	i64 u0xa1373aa12c58883e, ; 787: lib_OneSignalSDK.DotNet.Core.dll.so => 218
	i64 u0xa1440773ee9d341e, ; 788: Xamarin.Google.Android.Material => 332
	i64 u0xa1b9d7c27f47219f, ; 789: Xamarin.AndroidX.Navigation.UI.dll => 291
	i64 u0xa2572680829d2c7c, ; 790: System.IO.Pipelines.dll => 54
	i64 u0xa26597e57ee9c7f6, ; 791: System.Xml.XmlDocument.dll => 162
	i64 u0xa308401900e5bed3, ; 792: lib_mscorlib.dll.so => 167
	i64 u0xa395572e7da6c99d, ; 793: lib_System.Security.dll.so => 131
	i64 u0xa3c64c49e90a9987, ; 794: System.Security.Cryptography.Pkcs => 227
	i64 u0xa3e683f24b43af6f, ; 795: System.Dynamic.Runtime.dll => 37
	i64 u0xa4145becdee3dc4f, ; 796: Xamarin.AndroidX.VectorDrawable.Animated => 309
	i64 u0xa46aa1eaa214539b, ; 797: ko/Microsoft.Maui.Controls.resources => 386
	i64 u0xa4a372eecb9e4df0, ; 798: Microsoft.Extensions.Diagnostics => 190
	i64 u0xa4d20d2ff0563d26, ; 799: lib_CommunityToolkit.Mvvm.dll.so => 178
	i64 u0xa4edc8f2ceae241a, ; 800: System.Data.Common.dll => 22
	i64 u0xa5494f40f128ce6a, ; 801: System.Runtime.Serialization.Formatters.dll => 112
	i64 u0xa54b74df83dce92b, ; 802: System.Reflection.DispatchProxy => 90
	i64 u0xa579ed010d7e5215, ; 803: Xamarin.AndroidX.DocumentFile => 258
	i64 u0xa5b7152421ed6d98, ; 804: lib_System.IO.FileSystem.Watcher.dll.so => 50
	i64 u0xa5c3844f17b822db, ; 805: lib_System.Linq.Parallel.dll.so => 60
	i64 u0xa5ce5c755bde8cb8, ; 806: lib_System.Security.Cryptography.Csp.dll.so => 122
	i64 u0xa5e599d1e0524750, ; 807: System.Numerics.Vectors.dll => 83
	i64 u0xa5f1ba49b85dd355, ; 808: System.Security.Cryptography.dll => 127
	i64 u0xa61975a5a37873ea, ; 809: lib_System.Xml.XmlSerializer.dll.so => 163
	i64 u0xa6593e21584384d2, ; 810: lib_Jsr305Binding.dll.so => 333
	i64 u0xa66cbee0130865f7, ; 811: lib_WindowsBase.dll.so => 166
	i64 u0xa67dbee13e1df9ca, ; 812: Xamarin.AndroidX.SavedState.dll => 298
	i64 u0xa684b098dd27b296, ; 813: lib_Xamarin.AndroidX.Security.SecurityCrypto.dll.so => 300
	i64 u0xa68a420042bb9b1f, ; 814: Xamarin.AndroidX.DrawerLayout.dll => 259
	i64 u0xa6d26156d1cacc7c, ; 815: Xamarin.Android.Glide.dll => 229
	i64 u0xa71fe7d6f6f93efd, ; 816: Microsoft.Data.SqlClient => 182
	i64 u0xa75386b5cb9595aa, ; 817: Xamarin.AndroidX.Lifecycle.Runtime.Android => 277
	i64 u0xa763fbb98df8d9fb, ; 818: lib_Microsoft.Win32.Primitives.dll.so => 4
	i64 u0xa78ce3745383236a, ; 819: Xamarin.AndroidX.Lifecycle.Common.Jvm => 271
	i64 u0xa7a08e0f3bd00c3e, ; 820: Xamarin.Firebase.Messaging.dll => 328
	i64 u0xa7c31b56b4dc7b33, ; 821: hu/Microsoft.Maui.Controls.resources => 382
	i64 u0xa7eab29ed44b4e7a, ; 822: Mono.Android.Export => 170
	i64 u0xa8195217cbf017b7, ; 823: Microsoft.VisualBasic.Core => 2
	i64 u0xa843f6095f0d247d, ; 824: Xamarin.GooglePlayServices.Base.dll => 339
	i64 u0xa859a95830f367ff, ; 825: lib_Xamarin.AndroidX.Lifecycle.ViewModel.Ktx.dll.so => 283
	i64 u0xa8b52f21e0dbe690, ; 826: System.Runtime.Serialization.dll => 116
	i64 u0xa8c84ce526c2b4bd, ; 827: Microsoft.VisualStudio.DesignTools.XamlTapContract.dll => 407
	i64 u0xa8e6320dd07580ef, ; 828: lib_Microsoft.IdentityModel.JsonWebTokens.dll.so => 202
	i64 u0xa8ee4ed7de2efaee, ; 829: Xamarin.AndroidX.Annotation.dll => 235
	i64 u0xa95590e7c57438a4, ; 830: System.Configuration => 19
	i64 u0xa964304b5631e28a, ; 831: CommunityToolkit.Maui.Core.dll => 177
	i64 u0xaa05f1b7b312c742, ; 832: OneSignalSDK.DotNet.Android.Notifications.Binding => 216
	i64 u0xaa2219c8e3449ff5, ; 833: Microsoft.Extensions.Logging.Abstractions => 194
	i64 u0xaa443ac34067eeef, ; 834: System.Private.Xml.dll => 89
	i64 u0xaa52de307ef5d1dd, ; 835: System.Net.Http => 65
	i64 u0xaa9a7b0214a5cc5c, ; 836: System.Diagnostics.StackTrace.dll => 30
	i64 u0xaaaf86367285a918, ; 837: Microsoft.Extensions.DependencyInjection.Abstractions.dll => 189
	i64 u0xaaf84bb3f052a265, ; 838: el/Microsoft.Maui.Controls.resources => 375
	i64 u0xab9af77b5b67a0b8, ; 839: Xamarin.AndroidX.ConstraintLayout.Core => 250
	i64 u0xab9c1b2687d86b0b, ; 840: lib_System.Linq.Expressions.dll.so => 59
	i64 u0xac2af3fa195a15ce, ; 841: System.Runtime.Numerics => 111
	i64 u0xac5376a2a538dc10, ; 842: Xamarin.AndroidX.Lifecycle.LiveData.Core.dll => 273
	i64 u0xac5acae88f60357e, ; 843: System.Diagnostics.Tools.dll => 32
	i64 u0xac79c7e46047ad98, ; 844: System.Security.Principal.Windows.dll => 128
	i64 u0xac98d31068e24591, ; 845: System.Xml.XDocument => 159
	i64 u0xacd46e002c3ccb97, ; 846: ro/Microsoft.Maui.Controls.resources => 393
	i64 u0xacda2fab67639416, ; 847: Xamarin.Firebase.Installations => 325
	i64 u0xacdd9e4180d56dda, ; 848: Xamarin.AndroidX.Concurrent.Futures => 248
	i64 u0xacf42eea7ef9cd12, ; 849: System.Threading.Channels => 140
	i64 u0xad7e82ed3b0f16d0, ; 850: lib_Xamarin.AndroidX.DocumentFile.dll.so => 258
	i64 u0xad89c07347f1bad6, ; 851: nl/Microsoft.Maui.Controls.resources.dll => 389
	i64 u0xadbb53caf78a79d2, ; 852: System.Web.HttpUtility => 153
	i64 u0xadc90ab061a9e6e4, ; 853: System.ComponentModel.TypeConverter.dll => 17
	i64 u0xadca1b9030b9317e, ; 854: Xamarin.AndroidX.Collection.Ktx => 247
	i64 u0xadd8eda2edf396ad, ; 855: Xamarin.Android.Glide.GifDecoder => 232
	i64 u0xadf4cf30debbeb9a, ; 856: System.Net.ServicePoint.dll => 75
	i64 u0xadf511667bef3595, ; 857: System.Net.Security => 74
	i64 u0xae0aaa94fdcfce0f, ; 858: System.ComponentModel.EventBasedAsync.dll => 15
	i64 u0xae282bcd03739de7, ; 859: Java.Interop => 169
	i64 u0xae53579c90db1107, ; 860: System.ObjectModel.dll => 85
	i64 u0xaeb080014622ef84, ; 861: Xamarin.JavaX.Inject => 344
	i64 u0xaec7c0c7e2ed4575, ; 862: lib_Xamarin.KotlinX.AtomicFU.Jvm.dll.so => 351
	i64 u0xaf4829c0b3e740ae, ; 863: lib_Syncfusion.Maui.Toolkit.resources.dll.so => 221
	i64 u0xaf732d0b2193b8f5, ; 864: System.Security.Cryptography.OpenSsl.dll => 124
	i64 u0xaf8d8726109d6bee, ; 865: Xamarin.AndroidX.Work.Runtime.dll => 315
	i64 u0xaf9c4f550f1dfc36, ; 866: Xamarin.AndroidX.AsyncLayoutInflater => 242
	i64 u0xafdb94dbccd9d11c, ; 867: Xamarin.AndroidX.Lifecycle.LiveData.dll => 272
	i64 u0xafe29f45095518e7, ; 868: lib_Xamarin.AndroidX.Lifecycle.ViewModelSavedState.dll.so => 284
	i64 u0xb03ae931fb25607e, ; 869: Xamarin.AndroidX.ConstraintLayout => 249
	i64 u0xb05cc42cd94c6d9d, ; 870: lib-sv-Microsoft.Maui.Controls.resources.dll.so => 396
	i64 u0xb0ac21bec8f428c5, ; 871: Xamarin.AndroidX.Lifecycle.Runtime.Ktx.Android.dll => 279
	i64 u0xb0bb43dc52ea59f9, ; 872: System.Diagnostics.Tracing.dll => 34
	i64 u0xb19b5004e406fd10, ; 873: Xamarin.AndroidX.Work.Work.Runtime.Ktx.dll => 316
	i64 u0xb1dd05401aa8ee63, ; 874: System.Security.AccessControl => 118
	i64 u0xb220631954820169, ; 875: System.Text.RegularExpressions => 139
	i64 u0xb2376e1dbf8b4ed7, ; 876: System.Security.Cryptography.Csp => 122
	i64 u0xb2a1959fe95c5402, ; 877: lib_System.Runtime.InteropServices.JavaScript.dll.so => 106
	i64 u0xb2a3f67f3bf29fce, ; 878: da/Microsoft.Maui.Controls.resources => 373
	i64 u0xb3005ac9c8a035c5, ; 879: lib_Xamarin.JavaX.Inject.dll.so => 344
	i64 u0xb3011a0a57f7ffb2, ; 880: Microsoft.VisualStudio.DesignTools.MobileTapContracts.dll => 405
	i64 u0xb363b43eeec5798e, ; 881: lib_Xamarin.Firebase.Iid.Interop.dll.so => 324
	i64 u0xb3874072ee0ecf8c, ; 882: Xamarin.AndroidX.VectorDrawable.Animated.dll => 309
	i64 u0xb398860d6ed7ba2f, ; 883: System.Security.Cryptography.ProtectedData => 228
	i64 u0xb3f0a0fcda8d3ebc, ; 884: Xamarin.AndroidX.CardView => 244
	i64 u0xb46be1aa6d4fff93, ; 885: hi/Microsoft.Maui.Controls.resources => 380
	i64 u0xb477491be13109d8, ; 886: ar/Microsoft.Maui.Controls.resources => 370
	i64 u0xb4bd7015ecee9d86, ; 887: System.IO.Pipelines => 54
	i64 u0xb4c53d9749c5f226, ; 888: lib_System.IO.FileSystem.AccessControl.dll.so => 47
	i64 u0xb4ff710863453fda, ; 889: System.Diagnostics.FileVersionInfo.dll => 28
	i64 u0xb5c38bf497a4cfe2, ; 890: lib_System.Threading.Tasks.dll.so => 145
	i64 u0xb5c7fcdafbc67ee4, ; 891: Microsoft.Extensions.Logging.Abstractions.dll => 194
	i64 u0xb5e2ea1bb00704d6, ; 892: Xamarin.Kotlin.StdLib.Jdk7.dll => 348
	i64 u0xb5ea31d5244c6626, ; 893: System.Threading.ThreadPool.dll => 147
	i64 u0xb7212c4683a94afe, ; 894: System.Drawing.Primitives => 35
	i64 u0xb7b7753d1f319409, ; 895: sv/Microsoft.Maui.Controls.resources => 396
	i64 u0xb81a2c6e0aee50fe, ; 896: lib_System.Private.CoreLib.dll.so => 173
	i64 u0xb872c26142d22aa9, ; 897: Microsoft.Extensions.Http.dll => 192
	i64 u0xb8b0a9b3dfbc5cb7, ; 898: Xamarin.AndroidX.Window.Extensions.Core.Core => 314
	i64 u0xb8c60af47c08d4da, ; 899: System.Net.ServicePoint => 75
	i64 u0xb8e68d20aad91196, ; 900: lib_System.Xml.XPath.dll.so => 161
	i64 u0xb9185c33a1643eed, ; 901: Microsoft.CSharp.dll => 1
	i64 u0xb9b8001adf4ed7cc, ; 902: lib_Xamarin.AndroidX.SlidingPaneLayout.dll.so => 301
	i64 u0xb9f64d3b230def68, ; 903: lib-pt-Microsoft.Maui.Controls.resources.dll.so => 392
	i64 u0xb9fc3c8a556e3691, ; 904: ja/Microsoft.Maui.Controls.resources => 385
	i64 u0xba4670aa94a2b3c6, ; 905: lib_System.Xml.XDocument.dll.so => 159
	i64 u0xba48785529705af9, ; 906: System.Collections.dll => 12
	i64 u0xba965b8c86359996, ; 907: lib_System.Windows.dll.so => 155
	i64 u0xbb286883bc35db36, ; 908: System.Transactions.dll => 151
	i64 u0xbb65706fde942ce3, ; 909: System.Net.Sockets => 76
	i64 u0xbb8c8d165ef11460, ; 910: lib_Microsoft.Identity.Client.Extensions.Msal.dll.so => 200
	i64 u0xbba28979413cad9e, ; 911: lib_System.Runtime.CompilerServices.VisualC.dll.so => 103
	i64 u0xbbd180354b67271a, ; 912: System.Runtime.Serialization.Formatters => 112
	i64 u0xbc260cdba33291a3, ; 913: Xamarin.AndroidX.Arch.Core.Common.dll => 240
	i64 u0xbcd22b365b764643, ; 914: lib-zh-Hans-Microsoft.Data.SqlClient.resources.dll.so => 368
	i64 u0xbd0aaf9dbfcc3376, ; 915: fr/Microsoft.Data.SqlClient.resources.dll => 360
	i64 u0xbd0e2c0d55246576, ; 916: System.Net.Http.dll => 65
	i64 u0xbd3c2d7a8325e11b, ; 917: lib-fr-Microsoft.Data.SqlClient.resources.dll.so => 360
	i64 u0xbd3fbd85b9e1cb29, ; 918: lib_System.Net.HttpListener.dll.so => 66
	i64 u0xbd437a2cdb333d0d, ; 919: Xamarin.AndroidX.ViewPager2 => 312
	i64 u0xbd4aef17dbfb0390, ; 920: ru/Microsoft.Data.SqlClient.resources => 366
	i64 u0xbd4f572d2bd0a789, ; 921: System.IO.Compression.ZipFile.dll => 45
	i64 u0xbd5d0b88d3d647a5, ; 922: lib_Xamarin.AndroidX.Browser.dll.so => 243
	i64 u0xbd877b14d0b56392, ; 923: System.Runtime.Intrinsics.dll => 109
	i64 u0xbd960c672af2c51e, ; 924: Xamarin.GooglePlayServices.Stats => 342
	i64 u0xbe50876298b688e8, ; 925: Xamarin.AndroidX.Room.Common.dll => 296
	i64 u0xbe65a49036345cf4, ; 926: lib_System.Buffers.dll.so => 7
	i64 u0xbee38d4a88835966, ; 927: Xamarin.AndroidX.AppCompat.AppCompatResources => 239
	i64 u0xbef9919db45b4ca7, ; 928: System.IO.Pipes.AccessControl => 55
	i64 u0xbf0fa68611139208, ; 929: lib_Xamarin.AndroidX.Annotation.dll.so => 235
	i64 u0xbfc1e1fb3095f2b3, ; 930: lib_System.Net.Http.Json.dll.so => 64
	i64 u0xbfc455f43efd8f21, ; 931: Xamarin.AndroidX.Legacy.Support.Core.UI.dll => 267
	i64 u0xc040a4ab55817f58, ; 932: ar/Microsoft.Maui.Controls.resources.dll => 370
	i64 u0xc07cadab29efeba0, ; 933: Xamarin.AndroidX.Core.Core.Ktx.dll => 253
	i64 u0xc0d928351ab5ca77, ; 934: System.Console.dll => 20
	i64 u0xc0f5a221a9383aea, ; 935: System.Runtime.Intrinsics => 109
	i64 u0xc111030af54d7191, ; 936: System.Resources.Writer => 101
	i64 u0xc12b8b3afa48329c, ; 937: lib_System.Linq.dll.so => 62
	i64 u0xc1347413e524ff69, ; 938: lib_Syncfusion.Maui.Toolkit.dll.so => 220
	i64 u0xc183ca0b74453aa9, ; 939: lib_System.Threading.Tasks.Dataflow.dll.so => 142
	i64 u0xc1ff9ae3cdb6e1e6, ; 940: Xamarin.AndroidX.Activity.dll => 233
	i64 u0xc26c064effb1dea9, ; 941: System.Buffers.dll => 7
	i64 u0xc278de356ad8a9e3, ; 942: Microsoft.IdentityModel.Logging => 203
	i64 u0xc28c50f32f81cc73, ; 943: ja/Microsoft.Maui.Controls.resources.dll => 385
	i64 u0xc2902f6cf5452577, ; 944: lib_Mono.Android.Export.dll.so => 170
	i64 u0xc2a3bca55b573141, ; 945: System.IO.FileSystem.Watcher => 50
	i64 u0xc2bcfec99f69365e, ; 946: Xamarin.AndroidX.ViewPager2.dll => 312
	i64 u0xc30b52815b58ac2c, ; 947: lib_System.Runtime.Serialization.Xml.dll.so => 115
	i64 u0xc36d7d89c652f455, ; 948: System.Threading.Overlapped => 141
	i64 u0xc396b285e59e5493, ; 949: GoogleGson.dll => 179
	i64 u0xc3c86c1e5e12f03d, ; 950: WindowsBase => 166
	i64 u0xc41fdb231b5de3c0, ; 951: OneSignalSDK.DotNet => 219
	i64 u0xc421b61fd853169d, ; 952: lib_System.Net.WebSockets.Client.dll.so => 80
	i64 u0xc4628cb75a5204b4, ; 953: lib_Xamarin.GooglePlayServices.CloudMessaging.dll.so => 341
	i64 u0xc463e077917aa21d, ; 954: System.Runtime.Serialization.Json => 113
	i64 u0xc4d3858ed4d08512, ; 955: Xamarin.AndroidX.Lifecycle.ViewModelSavedState.dll => 284
	i64 u0xc4d69851fe06342f, ; 956: lib_Microsoft.Extensions.Caching.Memory.dll.so => 184
	i64 u0xc50fded0ded1418c, ; 957: lib_System.ComponentModel.TypeConverter.dll.so => 17
	i64 u0xc519125d6bc8fb11, ; 958: lib_System.Net.Requests.dll.so => 73
	i64 u0xc5293b19e4dc230e, ; 959: Xamarin.AndroidX.Navigation.Fragment => 289
	i64 u0xc5325b2fcb37446f, ; 960: lib_System.Private.Xml.dll.so => 89
	i64 u0xc535cb9a21385d9b, ; 961: lib_Xamarin.Android.Glide.DiskLruCache.dll.so => 231
	i64 u0xc583d8477b5d3bac, ; 962: zh-Hant/Microsoft.Data.SqlClient.resources.dll => 369
	i64 u0xc5a0f4b95a699af7, ; 963: lib_System.Private.Uri.dll.so => 87
	i64 u0xc5cdcd5b6277579e, ; 964: lib_System.Security.Cryptography.Algorithms.dll.so => 120
	i64 u0xc5ec286825cb0bf4, ; 965: Xamarin.AndroidX.Tracing.Tracing => 306
	i64 u0xc6706bc8aa7fe265, ; 966: Xamarin.AndroidX.Annotation.Jvm => 237
	i64 u0xc7c01e7d7c93a110, ; 967: System.Text.Encoding.Extensions.dll => 135
	i64 u0xc7ce851898a4548e, ; 968: lib_System.Web.HttpUtility.dll.so => 153
	i64 u0xc809d4089d2556b2, ; 969: System.Runtime.InteropServices.JavaScript.dll => 106
	i64 u0xc858a28d9ee5a6c5, ; 970: lib_System.Collections.Specialized.dll.so => 11
	i64 u0xc8ac7c6bf1c2ec51, ; 971: System.Reflection.DispatchProxy.dll => 90
	i64 u0xc9c62c8f354ac568, ; 972: lib_System.Diagnostics.TextWriterTraceListener.dll.so => 31
	i64 u0xc9e54b32fc19baf3, ; 973: lib_CommunityToolkit.Maui.dll.so => 176
	i64 u0xca32340d8d54dcd5, ; 974: Microsoft.Extensions.Caching.Memory.dll => 184
	i64 u0xca3a723e7342c5b6, ; 975: lib-tr-Microsoft.Maui.Controls.resources.dll.so => 398
	i64 u0xca5801070d9fccfb, ; 976: System.Text.Encoding => 136
	i64 u0xcab3493c70141c2d, ; 977: pl/Microsoft.Maui.Controls.resources => 390
	i64 u0xcab69b9a31439815, ; 978: lib_Xamarin.Google.ErrorProne.TypeAnnotations.dll.so => 337
	i64 u0xcacfddc9f7c6de76, ; 979: ro/Microsoft.Maui.Controls.resources.dll => 393
	i64 u0xcadbc92899a777f0, ; 980: Xamarin.AndroidX.Startup.StartupRuntime => 304
	i64 u0xcba1cb79f45292b5, ; 981: Xamarin.Android.Glide.GifDecoder.dll => 232
	i64 u0xcbb5f80c7293e696, ; 982: lib_System.Globalization.Calendars.dll.so => 40
	i64 u0xcbd4fdd9cef4a294, ; 983: lib__Microsoft.Android.Resource.Designer.dll.so => 411
	i64 u0xcc15da1e07bbd994, ; 984: Xamarin.AndroidX.SlidingPaneLayout => 301
	i64 u0xcc182c3afdc374d6, ; 985: Microsoft.Bcl.AsyncInterfaces => 180
	i64 u0xcc2876b32ef2794c, ; 986: lib_System.Text.RegularExpressions.dll.so => 139
	i64 u0xcc5c3bb714c4561e, ; 987: Xamarin.KotlinX.Coroutines.Core.Jvm.dll => 354
	i64 u0xcc76886e09b88260, ; 988: Xamarin.KotlinX.Serialization.Core.Jvm.dll => 356
	i64 u0xcc9fa2923aa1c9ef, ; 989: System.Diagnostics.Contracts.dll => 25
	i64 u0xccf25c4b634ccd3a, ; 990: zh-Hans/Microsoft.Maui.Controls.resources.dll => 402
	i64 u0xcd10a42808629144, ; 991: System.Net.Requests => 73
	i64 u0xcdca1b920e9f53ba, ; 992: Xamarin.AndroidX.Interpolator => 266
	i64 u0xcdd0c48b6937b21c, ; 993: Xamarin.AndroidX.SwipeRefreshLayout => 305
	i64 u0xcde1fa22dc303670, ; 994: Microsoft.VisualStudio.DesignTools.XamlTapContract => 407
	i64 u0xceb28d385f84f441, ; 995: Azure.Core.dll => 174
	i64 u0xcf140ed700bc8e66, ; 996: Microsoft.SqlServer.Server.dll => 212
	i64 u0xcf1f7a2359f1a539, ; 997: Xamarin.JavaX.Inject.dll => 344
	i64 u0xcf23d8093f3ceadf, ; 998: System.Diagnostics.DiagnosticSource.dll => 27
	i64 u0xcf5ff6b6b2c4c382, ; 999: System.Net.Mail.dll => 67
	i64 u0xcf8fc898f98b0d34, ; 1000: System.Private.Xml.Linq => 88
	i64 u0xd04b5f59ed596e31, ; 1001: System.Reflection.Metadata.dll => 95
	i64 u0xd063299fcfc0c93f, ; 1002: lib_System.Runtime.Serialization.Json.dll.so => 113
	i64 u0xd07eb0f20de63ff6, ; 1003: Xamarin.Firebase.Encoders.JSON => 322
	i64 u0xd0de8a113e976700, ; 1004: System.Diagnostics.TextWriterTraceListener => 31
	i64 u0xd0fc33d5ae5d4cb8, ; 1005: System.Runtime.Extensions => 104
	i64 u0xd1194e1d8a8de83c, ; 1006: lib_Xamarin.AndroidX.Lifecycle.Common.Jvm.dll.so => 271
	i64 u0xd12beacdfc14f696, ; 1007: System.Dynamic.Runtime => 37
	i64 u0xd16fd7fb9bbcd43e, ; 1008: Microsoft.Extensions.Diagnostics.Abstractions => 191
	i64 u0xd198e7ce1b6a8344, ; 1009: System.Net.Quic.dll => 72
	i64 u0xd22a0c4630f2fe66, ; 1010: lib_System.Security.Cryptography.ProtectedData.dll.so => 228
	i64 u0xd23f43eaa6c25c64, ; 1011: Xamarin.AndroidX.Media.dll => 287
	i64 u0xd2dffb59201927bd, ; 1012: de/Microsoft.Data.SqlClient.resources.dll => 358
	i64 u0xd3144156a3727ebe, ; 1013: Xamarin.Google.Guava.ListenableFuture => 338
	i64 u0xd333d0af9e423810, ; 1014: System.Runtime.InteropServices => 108
	i64 u0xd33a415cb4278969, ; 1015: System.Security.Cryptography.Encoding.dll => 123
	i64 u0xd3426d966bb704f5, ; 1016: Xamarin.AndroidX.AppCompat.AppCompatResources.dll => 239
	i64 u0xd3651b6fc3125825, ; 1017: System.Private.Uri.dll => 87
	i64 u0xd373685349b1fe8b, ; 1018: Microsoft.Extensions.Logging.dll => 193
	i64 u0xd3801faafafb7698, ; 1019: System.Private.DataContractSerialization.dll => 86
	i64 u0xd3e4c8d6a2d5d470, ; 1020: it/Microsoft.Maui.Controls.resources => 384
	i64 u0xd3edcc1f25459a50, ; 1021: System.Reflection.Emit => 93
	i64 u0xd4645626dffec99d, ; 1022: lib_Microsoft.Extensions.DependencyInjection.Abstractions.dll.so => 189
	i64 u0xd4fa0abb79079ea9, ; 1023: System.Security.Principal.dll => 129
	i64 u0xd5507e11a2b2839f, ; 1024: Xamarin.AndroidX.Lifecycle.ViewModelSavedState => 284
	i64 u0xd5858610826f1c08, ; 1025: lib-ru-Microsoft.Data.SqlClient.resources.dll.so => 366
	i64 u0xd5d04bef8478ea19, ; 1026: Xamarin.AndroidX.Tracing.Tracing.dll => 306
	i64 u0xd60815f26a12e140, ; 1027: Microsoft.Extensions.Logging.Debug.dll => 195
	i64 u0xd65786d27a4ad960, ; 1028: lib_Microsoft.Maui.Controls.HotReload.Forms.dll.so => 404
	i64 u0xd6694f8359737e4e, ; 1029: Xamarin.AndroidX.SavedState => 298
	i64 u0xd6949e129339eae5, ; 1030: lib_Xamarin.AndroidX.Core.Core.Ktx.dll.so => 253
	i64 u0xd6d21782156bc35b, ; 1031: Xamarin.AndroidX.SwipeRefreshLayout.dll => 305
	i64 u0xd6de019f6af72435, ; 1032: Xamarin.AndroidX.ConstraintLayout.Core.dll => 250
	i64 u0xd6f697a581fc6fe3, ; 1033: Xamarin.Google.ErrorProne.TypeAnnotations.dll => 337
	i64 u0xd70956d1e6deefb9, ; 1034: Jsr305Binding => 333
	i64 u0xd72329819cbbbc44, ; 1035: lib_Microsoft.Extensions.Configuration.Abstractions.dll.so => 186
	i64 u0xd72c760af136e863, ; 1036: System.Xml.XmlSerializer.dll => 163
	i64 u0xd753f071e44c2a03, ; 1037: lib_System.Security.SecureString.dll.so => 130
	i64 u0xd7b3764ada9d341d, ; 1038: lib_Microsoft.Extensions.Logging.Abstractions.dll.so => 194
	i64 u0xd7f0088bc5ad71f2, ; 1039: Xamarin.AndroidX.VersionedParcelable => 310
	i64 u0xd8fb25e28ae30a12, ; 1040: Xamarin.AndroidX.ProfileInstaller.ProfileInstaller.dll => 293
	i64 u0xda1dfa4c534a9251, ; 1041: Microsoft.Extensions.DependencyInjection => 188
	i64 u0xdad05a11827959a3, ; 1042: System.Collections.NonGeneric.dll => 10
	i64 u0xdadcb03a9169362c, ; 1043: OneSignalSDK.DotNet.Android.Location.Binding.dll => 215
	i64 u0xdaee64b908353553, ; 1044: Xamarin.AndroidX.Sqlite => 302
	i64 u0xdaefdfe71aa53cf9, ; 1045: System.IO.FileSystem.Primitives => 49
	i64 u0xdb5383ab5865c007, ; 1046: lib-vi-Microsoft.Maui.Controls.resources.dll.so => 400
	i64 u0xdb58816721c02a59, ; 1047: lib_System.Reflection.Emit.ILGeneration.dll.so => 91
	i64 u0xdbec20ec3340df05, ; 1048: RMD.Movil.Core.dll => 408
	i64 u0xdbeda89f832aa805, ; 1049: vi/Microsoft.Maui.Controls.resources.dll => 400
	i64 u0xdbf2a779fbc3ac31, ; 1050: System.Transactions.Local.dll => 150
	i64 u0xdbf9607a441b4505, ; 1051: System.Linq => 62
	i64 u0xdbfc90157a0de9b0, ; 1052: lib_System.Text.Encoding.dll.so => 136
	i64 u0xdc75032002d1a212, ; 1053: lib_System.Transactions.Local.dll.so => 150
	i64 u0xdca8be7403f92d4f, ; 1054: lib_System.Linq.Queryable.dll.so => 61
	i64 u0xdce2c53525640bf3, ; 1055: Microsoft.Extensions.Logging => 193
	i64 u0xdd2b722d78ef5f43, ; 1056: System.Runtime.dll => 117
	i64 u0xdd67031857c72f96, ; 1057: lib_System.Text.Encodings.Web.dll.so => 137
	i64 u0xdd70765ad6162057, ; 1058: Xamarin.JSpecify => 346
	i64 u0xdd92e229ad292030, ; 1059: System.Numerics.dll => 84
	i64 u0xdddcdd701e911af1, ; 1060: lib_Xamarin.AndroidX.Legacy.Support.Core.Utils.dll.so => 268
	i64 u0xdde30e6b77aa6f6c, ; 1061: lib-zh-Hans-Microsoft.Maui.Controls.resources.dll.so => 402
	i64 u0xde110ae80fa7c2e2, ; 1062: System.Xml.XDocument.dll => 159
	i64 u0xde4726fcdf63a198, ; 1063: Xamarin.AndroidX.Transition => 307
	i64 u0xde572c2b2fb32f93, ; 1064: lib_System.Threading.Tasks.Extensions.dll.so => 143
	i64 u0xde8769ebda7d8647, ; 1065: hr/Microsoft.Maui.Controls.resources.dll => 381
	i64 u0xdee075f3477ef6be, ; 1066: Xamarin.AndroidX.ExifInterface.dll => 263
	i64 u0xdf4b773de8fb1540, ; 1067: System.Net.dll => 82
	i64 u0xdfa254ebb4346068, ; 1068: System.Net.Ping => 70
	i64 u0xdfd2736047072591, ; 1069: lib_Xamarin.AndroidX.Room.Common.dll.so => 296
	i64 u0xe0142572c095a480, ; 1070: Xamarin.AndroidX.AppCompat.dll => 238
	i64 u0xe020c74e3723dc6f, ; 1071: Syncfusion.Maui.Toolkit.dll => 220
	i64 u0xe021eaa401792a05, ; 1072: System.Text.Encoding.dll => 136
	i64 u0xe02f89350ec78051, ; 1073: Xamarin.AndroidX.CoordinatorLayout.dll => 251
	i64 u0xe03ac59b394077a7, ; 1074: lib_Xamarin.Firebase.Messaging.dll.so => 328
	i64 u0xe0496b9d65ef5474, ; 1075: Xamarin.Android.Glide.DiskLruCache.dll => 231
	i64 u0xe0ea30f1ac5b7731, ; 1076: ko/Microsoft.Data.SqlClient.resources.dll => 363
	i64 u0xe0ee2e61123c1478, ; 1077: lib-es-Microsoft.Data.SqlClient.resources.dll.so => 359
	i64 u0xe0f282c49dd6bd0a, ; 1078: Xamarin.AndroidX.Media => 287
	i64 u0xe10b760bb1462e7a, ; 1079: lib_System.Security.Cryptography.Primitives.dll.so => 125
	i64 u0xe12265280d0b036d, ; 1080: fr/Microsoft.Data.SqlClient.resources => 360
	i64 u0xe14ff8e84737288a, ; 1081: Xamarin.Firebase.Installations.InterOp.dll => 326
	i64 u0xe1566bbdb759c5af, ; 1082: Microsoft.Maui.Controls.HotReload.Forms.dll => 404
	i64 u0xe192a588d4410686, ; 1083: lib_System.IO.Pipelines.dll.so => 54
	i64 u0xe1a08bd3fa539e0d, ; 1084: System.Runtime.Loader => 110
	i64 u0xe1a77eb8831f7741, ; 1085: System.Security.SecureString.dll => 130
	i64 u0xe1b52f9f816c70ef, ; 1086: System.Private.Xml.Linq.dll => 88
	i64 u0xe1e199c8ab02e356, ; 1087: System.Data.DataSetExtensions.dll => 23
	i64 u0xe1ecfdb7fff86067, ; 1088: System.Net.Security.dll => 74
	i64 u0xe2252a80fe853de4, ; 1089: lib_System.Security.Principal.dll.so => 129
	i64 u0xe22fa4c9c645db62, ; 1090: System.Diagnostics.TextWriterTraceListener.dll => 31
	i64 u0xe2420585aeceb728, ; 1091: System.Net.Requests.dll => 73
	i64 u0xe26692647e6bcb62, ; 1092: Xamarin.AndroidX.Lifecycle.Runtime.Ktx => 278
	i64 u0xe29b73bc11392966, ; 1093: lib-id-Microsoft.Maui.Controls.resources.dll.so => 383
	i64 u0xe2ad448dee50fbdf, ; 1094: System.Xml.Serialization => 158
	i64 u0xe2d920f978f5d85c, ; 1095: System.Data.DataSetExtensions => 23
	i64 u0xe2e426c7714fa0bc, ; 1096: Microsoft.Win32.Primitives.dll => 4
	i64 u0xe2f1495353abf5e7, ; 1097: lib_OneSignalSDK.DotNet.Android.Location.Binding.dll.so => 215
	i64 u0xe332bacb3eb4a806, ; 1098: Mono.Android.Export.dll => 170
	i64 u0xe3811d68d4fe8463, ; 1099: pt-BR/Microsoft.Maui.Controls.resources.dll => 391
	i64 u0xe3b684b69d602994, ; 1100: OneSignalSDK.DotNet.Android.Core.Binding.dll => 213
	i64 u0xe3b7cbae5ad66c75, ; 1101: lib_System.Security.Cryptography.Encoding.dll.so => 123
	i64 u0xe4292b48f3224d5b, ; 1102: lib_Xamarin.AndroidX.Core.ViewTree.dll.so => 254
	i64 u0xe494f7ced4ecd10a, ; 1103: hu/Microsoft.Maui.Controls.resources.dll => 382
	i64 u0xe4a9b1e40d1e8917, ; 1104: lib-fi-Microsoft.Maui.Controls.resources.dll.so => 377
	i64 u0xe4f74a0b5bf9703f, ; 1105: System.Runtime.Serialization.Primitives => 114
	i64 u0xe5434e8a119ceb69, ; 1106: lib_Mono.Android.dll.so => 172
	i64 u0xe55703b9ce5c038a, ; 1107: System.Diagnostics.Tools => 32
	i64 u0xe57013c8afc270b5, ; 1108: Microsoft.VisualBasic => 3
	i64 u0xe57d22ca4aeb4900, ; 1109: System.Configuration.ConfigurationManager => 223
	i64 u0xe5a86e0c11dca2ba, ; 1110: lib_Xamarin.AndroidX.AsyncLayoutInflater.dll.so => 242
	i64 u0xe62913cc36bc07ec, ; 1111: System.Xml.dll => 164
	i64 u0xe7bea09c4900a191, ; 1112: Xamarin.AndroidX.VectorDrawable.dll => 308
	i64 u0xe7e03cc18dcdeb49, ; 1113: lib_System.Diagnostics.StackTrace.dll.so => 30
	i64 u0xe7e147ff99a7a380, ; 1114: lib_System.Configuration.dll.so => 19
	i64 u0xe8397cf3948e7cb7, ; 1115: lib_Microsoft.Extensions.Options.ConfigurationExtensions.dll.so => 197
	i64 u0xe83ddbccfc6aff3f, ; 1116: Xamarin.Kotlin.StdLib.Jdk7 => 348
	i64 u0xe86b0df4ba9e5db8, ; 1117: lib_Xamarin.AndroidX.Lifecycle.Runtime.Android.dll.so => 277
	i64 u0xe896622fe0902957, ; 1118: System.Reflection.Emit.dll => 93
	i64 u0xe89a2a9ef110899b, ; 1119: System.Drawing.dll => 36
	i64 u0xe8c5f8c100b5934b, ; 1120: Microsoft.Win32.Registry => 5
	i64 u0xe957c3976986ab72, ; 1121: lib_Xamarin.AndroidX.Window.Extensions.Core.Core.dll.so => 314
	i64 u0xe968ab252d3dda82, ; 1122: lib_Xamarin.Google.Android.DataTransport.TransportRuntime.dll.so => 331
	i64 u0xe98163eb702ae5c5, ; 1123: Xamarin.AndroidX.Arch.Core.Runtime => 241
	i64 u0xe994f23ba4c143e5, ; 1124: Xamarin.KotlinX.Coroutines.Android => 352
	i64 u0xe9b9c8c0458fd92a, ; 1125: System.Windows => 155
	i64 u0xe9d166d87a7f2bdb, ; 1126: lib_Xamarin.AndroidX.Startup.StartupRuntime.dll.so => 304
	i64 u0xea008206567504c4, ; 1127: Syncfusion.Maui.Toolkit => 220
	i64 u0xea5a4efc2ad81d1b, ; 1128: Xamarin.Google.ErrorProne.Annotations => 336
	i64 u0xeb2313fe9d65b785, ; 1129: Xamarin.AndroidX.ConstraintLayout.dll => 249
	i64 u0xeb6e275e78cb8d42, ; 1130: Xamarin.AndroidX.LocalBroadcastManager.dll => 286
	i64 u0xeb710a05c0467d46, ; 1131: Xamarin.GooglePlayServices.CloudMessaging => 341
	i64 u0xeb9973cda26e858f, ; 1132: Xamarin.Firebase.Common.dll => 318
	i64 u0xed19c616b3fcb7eb, ; 1133: Xamarin.AndroidX.VersionedParcelable.dll => 310
	i64 u0xed60c6fa891c051a, ; 1134: lib_Microsoft.VisualStudio.DesignTools.TapContract.dll.so => 406
	i64 u0xed6ef763c6fb395f, ; 1135: System.Diagnostics.EventLog.dll => 224
	i64 u0xeda5321b85525c6d, ; 1136: lib_OneSignalSDK.DotNet.Android.Notifications.Binding.dll.so => 216
	i64 u0xedc4817167106c23, ; 1137: System.Net.Sockets.dll => 76
	i64 u0xedc632067fb20ff3, ; 1138: System.Memory.dll => 63
	i64 u0xedc8e4ca71a02a8b, ; 1139: Xamarin.AndroidX.Navigation.Runtime.dll => 290
	i64 u0xee81f5b3f1c4f83b, ; 1140: System.Threading.ThreadPool => 147
	i64 u0xeeb7ebb80150501b, ; 1141: lib_Xamarin.AndroidX.Collection.Jvm.dll.so => 246
	i64 u0xeefc635595ef57f0, ; 1142: System.Security.Cryptography.Cng => 121
	i64 u0xef03b1b5a04e9709, ; 1143: System.Text.Encoding.CodePages.dll => 134
	i64 u0xef432781d5667f61, ; 1144: Xamarin.AndroidX.Print => 292
	i64 u0xef602c523fe2e87a, ; 1145: lib_Xamarin.Google.Guava.ListenableFuture.dll.so => 338
	i64 u0xef72742e1bcca27a, ; 1146: Microsoft.Maui.Essentials.dll => 210
	i64 u0xefd0396433f04886, ; 1147: pt-BR/Microsoft.Data.SqlClient.resources => 365
	i64 u0xefd1e0c4e5c9b371, ; 1148: System.Resources.ResourceManager.dll => 100
	i64 u0xefe8f8d5ed3c72ea, ; 1149: System.Formats.Tar.dll => 39
	i64 u0xefec0b7fdc57ec42, ; 1150: Xamarin.AndroidX.Activity => 233
	i64 u0xf0042b29a537c57d, ; 1151: lib_Xamarin.AndroidX.Work.Runtime.dll.so => 315
	i64 u0xf00c29406ea45e19, ; 1152: es/Microsoft.Maui.Controls.resources.dll => 376
	i64 u0xf038e4700d91eb2d, ; 1153: RMD.Movil.Core => 408
	i64 u0xf09e47b6ae914f6e, ; 1154: System.Net.NameResolution => 68
	i64 u0xf0ac2b489fed2e35, ; 1155: lib_System.Diagnostics.Debug.dll.so => 26
	i64 u0xf0bb49dadd3a1fe1, ; 1156: lib_System.Net.ServicePoint.dll.so => 75
	i64 u0xf0de2537ee19c6ca, ; 1157: lib_System.Net.WebHeaderCollection.dll.so => 78
	i64 u0xf1099338c6c6b7a6, ; 1158: RMD.Shared.Models.dll => 409
	i64 u0xf1138779fa181c68, ; 1159: lib_Xamarin.AndroidX.Lifecycle.Runtime.dll.so => 276
	i64 u0xf11b621fc87b983f, ; 1160: Microsoft.Maui.Controls.Xaml.dll => 208
	i64 u0xf161f4f3c3b7e62c, ; 1161: System.Data => 24
	i64 u0xf16eb650d5a464bc, ; 1162: System.ValueTuple => 152
	i64 u0xf18a15e13fe9dc9d, ; 1163: lib_OneSignalSDK.DotNet.Android.Core.Binding.dll.so => 213
	i64 u0xf1c4b4005493d871, ; 1164: System.Formats.Asn1.dll => 38
	i64 u0xf2039b1a33e63e8e, ; 1165: Xamarin.Google.Android.DataTransport.TransportApi.dll => 329
	i64 u0xf238bd79489d3a96, ; 1166: lib-nl-Microsoft.Maui.Controls.resources.dll.so => 389
	i64 u0xf2a69492c6bd46b0, ; 1167: lib_Xamarin.Kotlin.StdLib.Jdk7.dll.so => 348
	i64 u0xf2feea356ba760af, ; 1168: Xamarin.AndroidX.Arch.Core.Runtime.dll => 241
	i64 u0xf300e085f8acd238, ; 1169: lib_System.ServiceProcess.dll.so => 133
	i64 u0xf34e52b26e7e059d, ; 1170: System.Runtime.CompilerServices.VisualC.dll => 103
	i64 u0xf37221fda4ef8830, ; 1171: lib_Xamarin.Google.Android.Material.dll.so => 332
	i64 u0xf3ad9b8fb3eefd12, ; 1172: lib_System.IO.UnmanagedMemoryStream.dll.so => 57
	i64 u0xf3ddfe05336abf29, ; 1173: System => 165
	i64 u0xf408654b2a135055, ; 1174: System.Reflection.Emit.ILGeneration.dll => 91
	i64 u0xf4103170a1de5bd0, ; 1175: System.Linq.Queryable.dll => 61
	i64 u0xf42d20c23173d77c, ; 1176: lib_System.ServiceModel.Web.dll.so => 132
	i64 u0xf4c1dd70a5496a17, ; 1177: System.IO.Compression => 46
	i64 u0xf4ecf4b9afc64781, ; 1178: System.ServiceProcess.dll => 133
	i64 u0xf4eeeaa566e9b970, ; 1179: lib_Xamarin.AndroidX.CustomView.PoolingContainer.dll.so => 257
	i64 u0xf518f63ead11fcd1, ; 1180: System.Threading.Tasks => 145
	i64 u0xf5e59d7ac34b50aa, ; 1181: Microsoft.IdentityModel.Protocols.dll => 204
	i64 u0xf5fc7602fe27b333, ; 1182: System.Net.WebHeaderCollection => 78
	i64 u0xf6077741019d7428, ; 1183: Xamarin.AndroidX.CoordinatorLayout => 251
	i64 u0xf61ade9836ad4692, ; 1184: Microsoft.IdentityModel.Tokens.dll => 206
	i64 u0xf6742cbf457c450b, ; 1185: Xamarin.AndroidX.Lifecycle.Runtime.Android.dll => 277
	i64 u0xf6c0e7d55a7a4e4f, ; 1186: Microsoft.IdentityModel.JsonWebTokens => 202
	i64 u0xf6f893f692f8cb43, ; 1187: Microsoft.Extensions.Options.ConfigurationExtensions.dll => 197
	i64 u0xf70c0a7bf8ccf5af, ; 1188: System.Web => 154
	i64 u0xf77b20923f07c667, ; 1189: de/Microsoft.Maui.Controls.resources.dll => 374
	i64 u0xf7be8a85d06b4b64, ; 1190: ru/Microsoft.Data.SqlClient.resources.dll => 366
	i64 u0xf7e2cac4c45067b3, ; 1191: lib_System.Numerics.Vectors.dll.so => 83
	i64 u0xf7e74930e0e3d214, ; 1192: zh-HK/Microsoft.Maui.Controls.resources.dll => 401
	i64 u0xf83775f330791063, ; 1193: ja/Microsoft.Data.SqlClient.resources.dll => 362
	i64 u0xf84773b5c81e3cef, ; 1194: lib-uk-Microsoft.Maui.Controls.resources.dll.so => 399
	i64 u0xf8aac5ea82de1348, ; 1195: System.Linq.Queryable => 61
	i64 u0xf8b77539b362d3ba, ; 1196: lib_System.Reflection.Primitives.dll.so => 96
	i64 u0xf8cd217ba1bbfdc8, ; 1197: lib-zh-Hant-Microsoft.Data.SqlClient.resources.dll.so => 369
	i64 u0xf8e045dc345b2ea3, ; 1198: lib_Xamarin.AndroidX.RecyclerView.dll.so => 294
	i64 u0xf915dc29808193a1, ; 1199: System.Web.HttpUtility.dll => 153
	i64 u0xf92ff76df00a2c3e, ; 1200: RMD.Movil.dll => 0
	i64 u0xf96c777a2a0686f4, ; 1201: hi/Microsoft.Maui.Controls.resources.dll => 380
	i64 u0xf9be54c8bcf8ff3b, ; 1202: System.Security.AccessControl.dll => 118
	i64 u0xf9eec5bb3a6aedc6, ; 1203: Microsoft.Extensions.Options => 196
	i64 u0xfa0e82300e67f913, ; 1204: lib_System.AppContext.dll.so => 6
	i64 u0xfa2fdb27e8a2c8e8, ; 1205: System.ComponentModel.EventBasedAsync => 15
	i64 u0xfa3f278f288b0e84, ; 1206: lib_System.Net.Security.dll.so => 74
	i64 u0xfa5ed7226d978949, ; 1207: lib-ar-Microsoft.Maui.Controls.resources.dll.so => 370
	i64 u0xfa645d91e9fc4cba, ; 1208: System.Threading.Thread => 146
	i64 u0xfad4d2c770e827f9, ; 1209: lib_System.IO.IsolatedStorage.dll.so => 52
	i64 u0xfb06dd2338e6f7c4, ; 1210: System.Net.Ping.dll => 70
	i64 u0xfb087abe5365e3b7, ; 1211: lib_System.Data.DataSetExtensions.dll.so => 23
	i64 u0xfb846e949baff5ea, ; 1212: System.Xml.Serialization.dll => 158
	i64 u0xfbad3e4ce4b98145, ; 1213: System.Security.Cryptography.X509Certificates => 126
	i64 u0xfbf0a31c9fc34bc4, ; 1214: lib_System.Net.Http.dll.so => 65
	i64 u0xfc61ddcf78dd1f54, ; 1215: Xamarin.AndroidX.LocalBroadcastManager => 286
	i64 u0xfc6b7527cc280b3f, ; 1216: lib_System.Runtime.Serialization.Formatters.dll.so => 112
	i64 u0xfc719aec26adf9d9, ; 1217: Xamarin.AndroidX.Navigation.Fragment.dll => 289
	i64 u0xfc82690c2fe2735c, ; 1218: Xamarin.AndroidX.Lifecycle.Process.dll => 275
	i64 u0xfc93fc307d279893, ; 1219: System.IO.Pipes.AccessControl.dll => 55
	i64 u0xfcd302092ada6328, ; 1220: System.IO.MemoryMappedFiles.dll => 53
	i64 u0xfd22f00870e40ae0, ; 1221: lib_Xamarin.AndroidX.DrawerLayout.dll.so => 259
	i64 u0xfd49b3c1a76e2748, ; 1222: System.Runtime.InteropServices.RuntimeInformation => 107
	i64 u0xfd536c702f64dc47, ; 1223: System.Text.Encoding.Extensions => 135
	i64 u0xfd583f7657b6a1cb, ; 1224: Xamarin.AndroidX.Fragment => 264
	i64 u0xfd8dd91a2c26bd5d, ; 1225: Xamarin.AndroidX.Lifecycle.Runtime => 276
	i64 u0xfda36abccf05cf5c, ; 1226: System.Net.WebSockets.Client => 80
	i64 u0xfdbe4710aa9beeff, ; 1227: CommunityToolkit.Maui => 176
	i64 u0xfddbe9695626a7f5, ; 1228: Xamarin.AndroidX.Lifecycle.Common => 270
	i64 u0xfeae9952cf03b8cb, ; 1229: tr/Microsoft.Maui.Controls.resources => 398
	i64 u0xfebe1950717515f9, ; 1230: Xamarin.AndroidX.Lifecycle.LiveData.Core.Ktx.dll => 274
	i64 u0xff270a55858bac8d, ; 1231: System.Security.Principal => 129
	i64 u0xff9b54613e0d2cc8, ; 1232: System.Net.Http.Json => 64
	i64 u0xffb5607c2db1b7e8, ; 1233: Xamarin.Kotlin.StdLib.Jdk8 => 349
	i64 u0xffdb7a971be4ec73, ; 1234: System.ValueTuple.dll => 152
	i64 u0xfff40914e0b38d3d ; 1235: Azure.Identity.dll => 175
], align 8

@assembly_image_cache_indices = dso_local local_unnamed_addr constant [1236 x i32] [
	i32 42, i32 353, i32 358, i32 305, i32 13, i32 290, i32 177, i32 197,
	i32 227, i32 105, i32 363, i32 184, i32 171, i32 48, i32 238, i32 7,
	i32 213, i32 86, i32 394, i32 372, i32 400, i32 201, i32 260, i32 71,
	i32 343, i32 294, i32 335, i32 12, i32 209, i32 102, i32 369, i32 367,
	i32 401, i32 156, i32 19, i32 265, i32 246, i32 161, i32 262, i32 308,
	i32 167, i32 394, i32 10, i32 361, i32 195, i32 309, i32 174, i32 96,
	i32 257, i32 259, i32 335, i32 13, i32 196, i32 10, i32 226, i32 340,
	i32 297, i32 127, i32 319, i32 95, i32 183, i32 140, i32 182, i32 39,
	i32 395, i32 328, i32 356, i32 311, i32 391, i32 172, i32 232, i32 5,
	i32 210, i32 67, i32 300, i32 130, i32 180, i32 299, i32 261, i32 68,
	i32 325, i32 214, i32 247, i32 66, i32 362, i32 57, i32 180, i32 256,
	i32 52, i32 359, i32 43, i32 125, i32 67, i32 81, i32 278, i32 406,
	i32 158, i32 92, i32 99, i32 294, i32 205, i32 141, i32 302, i32 151,
	i32 243, i32 378, i32 162, i32 169, i32 379, i32 317, i32 205, i32 189,
	i32 81, i32 406, i32 218, i32 346, i32 247, i32 4, i32 5, i32 224,
	i32 51, i32 101, i32 358, i32 56, i32 0, i32 120, i32 98, i32 168,
	i32 118, i32 353, i32 21, i32 410, i32 382, i32 137, i32 97, i32 356,
	i32 77, i32 388, i32 292, i32 304, i32 119, i32 175, i32 8, i32 214,
	i32 165, i32 397, i32 70, i32 231, i32 279, i32 295, i32 190, i32 171,
	i32 321, i32 145, i32 40, i32 300, i32 47, i32 30, i32 291, i32 386,
	i32 144, i32 316, i32 196, i32 163, i32 28, i32 84, i32 306, i32 267,
	i32 335, i32 77, i32 296, i32 43, i32 29, i32 42, i32 103, i32 117,
	i32 236, i32 222, i32 45, i32 91, i32 219, i32 397, i32 56, i32 148,
	i32 405, i32 340, i32 146, i32 100, i32 49, i32 357, i32 20, i32 252,
	i32 269, i32 114, i32 302, i32 229, i32 297, i32 378, i32 334, i32 269,
	i32 347, i32 198, i32 94, i32 217, i32 58, i32 225, i32 383, i32 381,
	i32 81, i32 334, i32 169, i32 26, i32 326, i32 349, i32 71, i32 293,
	i32 191, i32 263, i32 303, i32 404, i32 399, i32 69, i32 33, i32 377,
	i32 14, i32 139, i32 225, i32 38, i32 403, i32 248, i32 409, i32 242,
	i32 330, i32 390, i32 134, i32 92, i32 88, i32 365, i32 149, i32 396,
	i32 24, i32 138, i32 57, i32 217, i32 51, i32 375, i32 321, i32 29,
	i32 157, i32 212, i32 34, i32 164, i32 183, i32 192, i32 342, i32 327,
	i32 330, i32 264, i32 201, i32 52, i32 411, i32 313, i32 90, i32 337,
	i32 320, i32 322, i32 244, i32 35, i32 378, i32 157, i32 9, i32 376,
	i32 76, i32 212, i32 55, i32 209, i32 372, i32 321, i32 207, i32 13,
	i32 312, i32 185, i32 240, i32 109, i32 283, i32 32, i32 319, i32 104,
	i32 84, i32 92, i32 53, i32 408, i32 96, i32 345, i32 58, i32 9,
	i32 102, i32 181, i32 256, i32 68, i32 204, i32 223, i32 311, i32 371,
	i32 297, i32 125, i32 295, i32 116, i32 316, i32 135, i32 206, i32 126,
	i32 106, i32 182, i32 347, i32 131, i32 243, i32 338, i32 147, i32 156,
	i32 265, i32 252, i32 260, i32 361, i32 327, i32 295, i32 97, i32 24,
	i32 301, i32 200, i32 143, i32 292, i32 288, i32 175, i32 3, i32 223,
	i32 167, i32 239, i32 303, i32 100, i32 161, i32 99, i32 280, i32 254,
	i32 25, i32 410, i32 93, i32 214, i32 168, i32 172, i32 234, i32 3,
	i32 219, i32 390, i32 262, i32 1, i32 114, i32 347, i32 265, i32 275,
	i32 225, i32 215, i32 33, i32 6, i32 394, i32 156, i32 226, i32 392,
	i32 53, i32 268, i32 85, i32 318, i32 310, i32 291, i32 44, i32 274,
	i32 104, i32 47, i32 138, i32 222, i32 64, i32 323, i32 285, i32 69,
	i32 80, i32 59, i32 89, i32 154, i32 222, i32 240, i32 133, i32 110,
	i32 384, i32 285, i32 293, i32 171, i32 0, i32 134, i32 323, i32 140,
	i32 40, i32 371, i32 357, i32 187, i32 206, i32 361, i32 207, i32 342,
	i32 60, i32 178, i32 187, i32 282, i32 362, i32 79, i32 25, i32 218,
	i32 36, i32 99, i32 278, i32 71, i32 22, i32 368, i32 252, i32 211,
	i32 395, i32 121, i32 69, i32 107, i32 401, i32 286, i32 119, i32 117,
	i32 270, i32 271, i32 11, i32 2, i32 124, i32 115, i32 331, i32 142,
	i32 41, i32 280, i32 87, i32 235, i32 173, i32 27, i32 148, i32 187,
	i32 385, i32 188, i32 336, i32 234, i32 1, i32 236, i32 226, i32 44,
	i32 251, i32 149, i32 268, i32 18, i32 86, i32 373, i32 339, i32 41,
	i32 274, i32 245, i32 349, i32 279, i32 94, i32 193, i32 28, i32 41,
	i32 78, i32 365, i32 261, i32 248, i32 364, i32 144, i32 108, i32 246,
	i32 324, i32 11, i32 105, i32 137, i32 16, i32 122, i32 66, i32 157,
	i32 22, i32 329, i32 375, i32 355, i32 102, i32 188, i32 354, i32 324,
	i32 63, i32 58, i32 208, i32 374, i32 110, i32 357, i32 173, i32 367,
	i32 181, i32 407, i32 352, i32 9, i32 332, i32 120, i32 98, i32 181,
	i32 320, i32 105, i32 283, i32 176, i32 207, i32 111, i32 237, i32 49,
	i32 20, i32 320, i32 282, i32 255, i32 72, i32 250, i32 330, i32 155,
	i32 39, i32 373, i32 35, i32 350, i32 38, i32 379, i32 314, i32 315,
	i32 108, i32 388, i32 21, i32 217, i32 345, i32 281, i32 228, i32 211,
	i32 364, i32 15, i32 198, i32 79, i32 79, i32 255, i32 198, i32 258,
	i32 289, i32 299, i32 152, i32 21, i32 325, i32 209, i32 372, i32 50,
	i32 51, i32 287, i32 398, i32 388, i32 94, i32 230, i32 191, i32 384,
	i32 16, i32 227, i32 254, i32 123, i32 381, i32 160, i32 45, i32 336,
	i32 179, i32 116, i32 63, i32 166, i32 190, i32 185, i32 14, i32 298,
	i32 111, i32 237, i32 216, i32 60, i32 351, i32 121, i32 387, i32 2,
	i32 397, i32 318, i32 327, i32 264, i32 331, i32 281, i32 329, i32 368,
	i32 192, i32 350, i32 346, i32 281, i32 6, i32 245, i32 377, i32 260,
	i32 202, i32 17, i32 395, i32 374, i32 77, i32 249, i32 340, i32 177,
	i32 131, i32 345, i32 387, i32 83, i32 195, i32 12, i32 34, i32 119,
	i32 355, i32 275, i32 262, i32 85, i32 229, i32 343, i32 18, i32 311,
	i32 186, i32 199, i32 273, i32 72, i32 405, i32 267, i32 95, i32 410,
	i32 165, i32 266, i32 82, i32 403, i32 269, i32 238, i32 244, i32 351,
	i32 154, i32 36, i32 151, i32 399, i32 201, i32 402, i32 144, i32 56,
	i32 113, i32 245, i32 308, i32 363, i32 280, i32 307, i32 37, i32 403,
	i32 185, i32 115, i32 236, i32 14, i32 230, i32 146, i32 43, i32 210,
	i32 234, i32 317, i32 98, i32 354, i32 168, i32 16, i32 48, i32 107,
	i32 97, i32 183, i32 303, i32 285, i32 27, i32 128, i32 29, i32 322,
	i32 379, i32 367, i32 174, i32 299, i32 128, i32 44, i32 255, i32 261,
	i32 149, i32 8, i32 221, i32 199, i32 224, i32 288, i32 380, i32 393,
	i32 343, i32 392, i32 132, i32 391, i32 409, i32 42, i32 355, i32 33,
	i32 411, i32 46, i32 143, i32 282, i32 208, i32 272, i32 256, i32 138,
	i32 62, i32 132, i32 371, i32 48, i32 160, i32 319, i32 241, i32 221,
	i32 272, i32 230, i32 359, i32 270, i32 341, i32 387, i32 307, i32 46,
	i32 164, i32 203, i32 266, i32 205, i32 203, i32 364, i32 376, i32 263,
	i32 200, i32 383, i32 211, i32 339, i32 18, i32 8, i32 317, i32 179,
	i32 253, i32 124, i32 59, i32 141, i32 178, i32 290, i32 386, i32 276,
	i32 204, i32 333, i32 313, i32 150, i32 142, i32 353, i32 350, i32 126,
	i32 352, i32 326, i32 160, i32 162, i32 257, i32 233, i32 186, i32 323,
	i32 389, i32 26, i32 288, i32 273, i32 199, i32 82, i32 313, i32 127,
	i32 334, i32 101, i32 148, i32 218, i32 332, i32 291, i32 54, i32 162,
	i32 167, i32 131, i32 227, i32 37, i32 309, i32 386, i32 190, i32 178,
	i32 22, i32 112, i32 90, i32 258, i32 50, i32 60, i32 122, i32 83,
	i32 127, i32 163, i32 333, i32 166, i32 298, i32 300, i32 259, i32 229,
	i32 182, i32 277, i32 4, i32 271, i32 328, i32 382, i32 170, i32 2,
	i32 339, i32 283, i32 116, i32 407, i32 202, i32 235, i32 19, i32 177,
	i32 216, i32 194, i32 89, i32 65, i32 30, i32 189, i32 375, i32 250,
	i32 59, i32 111, i32 273, i32 32, i32 128, i32 159, i32 393, i32 325,
	i32 248, i32 140, i32 258, i32 389, i32 153, i32 17, i32 247, i32 232,
	i32 75, i32 74, i32 15, i32 169, i32 85, i32 344, i32 351, i32 221,
	i32 124, i32 315, i32 242, i32 272, i32 284, i32 249, i32 396, i32 279,
	i32 34, i32 316, i32 118, i32 139, i32 122, i32 106, i32 373, i32 344,
	i32 405, i32 324, i32 309, i32 228, i32 244, i32 380, i32 370, i32 54,
	i32 47, i32 28, i32 145, i32 194, i32 348, i32 147, i32 35, i32 396,
	i32 173, i32 192, i32 314, i32 75, i32 161, i32 1, i32 301, i32 392,
	i32 385, i32 159, i32 12, i32 155, i32 151, i32 76, i32 200, i32 103,
	i32 112, i32 240, i32 368, i32 360, i32 65, i32 360, i32 66, i32 312,
	i32 366, i32 45, i32 243, i32 109, i32 342, i32 296, i32 7, i32 239,
	i32 55, i32 235, i32 64, i32 267, i32 370, i32 253, i32 20, i32 109,
	i32 101, i32 62, i32 220, i32 142, i32 233, i32 7, i32 203, i32 385,
	i32 170, i32 50, i32 312, i32 115, i32 141, i32 179, i32 166, i32 219,
	i32 80, i32 341, i32 113, i32 284, i32 184, i32 17, i32 73, i32 289,
	i32 89, i32 231, i32 369, i32 87, i32 120, i32 306, i32 237, i32 135,
	i32 153, i32 106, i32 11, i32 90, i32 31, i32 176, i32 184, i32 398,
	i32 136, i32 390, i32 337, i32 393, i32 304, i32 232, i32 40, i32 411,
	i32 301, i32 180, i32 139, i32 354, i32 356, i32 25, i32 402, i32 73,
	i32 266, i32 305, i32 407, i32 174, i32 212, i32 344, i32 27, i32 67,
	i32 88, i32 95, i32 113, i32 322, i32 31, i32 104, i32 271, i32 37,
	i32 191, i32 72, i32 228, i32 287, i32 358, i32 338, i32 108, i32 123,
	i32 239, i32 87, i32 193, i32 86, i32 384, i32 93, i32 189, i32 129,
	i32 284, i32 366, i32 306, i32 195, i32 404, i32 298, i32 253, i32 305,
	i32 250, i32 337, i32 333, i32 186, i32 163, i32 130, i32 194, i32 310,
	i32 293, i32 188, i32 10, i32 215, i32 302, i32 49, i32 400, i32 91,
	i32 408, i32 400, i32 150, i32 62, i32 136, i32 150, i32 61, i32 193,
	i32 117, i32 137, i32 346, i32 84, i32 268, i32 402, i32 159, i32 307,
	i32 143, i32 381, i32 263, i32 82, i32 70, i32 296, i32 238, i32 220,
	i32 136, i32 251, i32 328, i32 231, i32 363, i32 359, i32 287, i32 125,
	i32 360, i32 326, i32 404, i32 54, i32 110, i32 130, i32 88, i32 23,
	i32 74, i32 129, i32 31, i32 73, i32 278, i32 383, i32 158, i32 23,
	i32 4, i32 215, i32 170, i32 391, i32 213, i32 123, i32 254, i32 382,
	i32 377, i32 114, i32 172, i32 32, i32 3, i32 223, i32 242, i32 164,
	i32 308, i32 30, i32 19, i32 197, i32 348, i32 277, i32 93, i32 36,
	i32 5, i32 314, i32 331, i32 241, i32 352, i32 155, i32 304, i32 220,
	i32 336, i32 249, i32 286, i32 341, i32 318, i32 310, i32 406, i32 224,
	i32 216, i32 76, i32 63, i32 290, i32 147, i32 246, i32 121, i32 134,
	i32 292, i32 338, i32 210, i32 365, i32 100, i32 39, i32 233, i32 315,
	i32 376, i32 408, i32 68, i32 26, i32 75, i32 78, i32 409, i32 276,
	i32 208, i32 24, i32 152, i32 213, i32 38, i32 329, i32 389, i32 348,
	i32 241, i32 133, i32 103, i32 332, i32 57, i32 165, i32 91, i32 61,
	i32 132, i32 46, i32 133, i32 257, i32 145, i32 204, i32 78, i32 251,
	i32 206, i32 277, i32 202, i32 197, i32 154, i32 374, i32 366, i32 83,
	i32 401, i32 362, i32 399, i32 61, i32 96, i32 369, i32 294, i32 153,
	i32 0, i32 380, i32 118, i32 196, i32 6, i32 15, i32 74, i32 370,
	i32 146, i32 52, i32 70, i32 23, i32 158, i32 126, i32 65, i32 286,
	i32 112, i32 289, i32 275, i32 55, i32 53, i32 259, i32 107, i32 135,
	i32 264, i32 276, i32 80, i32 176, i32 270, i32 398, i32 274, i32 129,
	i32 64, i32 349, i32 152, i32 175
], align 4

@marshal_methods_number_of_classes = dso_local local_unnamed_addr constant i32 0, align 4

@marshal_methods_class_cache = dso_local local_unnamed_addr global [0 x %struct.MarshalMethodsManagedClass] zeroinitializer, align 8

; Names of classes in which marshal methods reside
@mm_class_names = dso_local local_unnamed_addr constant [0 x ptr] zeroinitializer, align 8

@mm_method_names = dso_local local_unnamed_addr constant [1 x %struct.MarshalMethodName] [
	%struct.MarshalMethodName {
		i64 u0x0000000000000000, ; name: 
		ptr @.MarshalMethodName.0_name; char* name
	} ; 0
], align 8

; get_function_pointer (uint32_t mono_image_index, uint32_t class_index, uint32_t method_token, void*& target_ptr)
@get_function_pointer = internal dso_local unnamed_addr global ptr null, align 8

; Functions

; Function attributes: memory(write, argmem: none, inaccessiblemem: none) "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" uwtable willreturn
define void @xamarin_app_init(ptr nocapture noundef readnone %env, ptr noundef %fn) local_unnamed_addr #0
{
	%fnIsNull = icmp eq ptr %fn, null
	br i1 %fnIsNull, label %1, label %2

1: ; preds = %0
	%putsResult = call noundef i32 @puts(ptr @.str.0)
	call void @abort()
	unreachable 

2: ; preds = %1, %0
	store ptr %fn, ptr @get_function_pointer, align 8, !tbaa !3
	ret void
}

; Strings
@.str.0 = private unnamed_addr constant [40 x i8] c"get_function_pointer MUST be specified\0A\00", align 1

;MarshalMethodName
@.MarshalMethodName.0_name = private unnamed_addr constant [1 x i8] c"\00", align 1

; External functions

; Function attributes: noreturn "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8"
declare void @abort() local_unnamed_addr #2

; Function attributes: nofree nounwind
declare noundef i32 @puts(ptr noundef) local_unnamed_addr #1
attributes #0 = { memory(write, argmem: none, inaccessiblemem: none) "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" "target-cpu"="generic" "target-features"="+fix-cortex-a53-835769,+neon,+outline-atomics,+v8a" uwtable willreturn }
attributes #1 = { nofree nounwind }
attributes #2 = { noreturn "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" "target-cpu"="generic" "target-features"="+fix-cortex-a53-835769,+neon,+outline-atomics,+v8a" }

; Metadata
!llvm.module.flags = !{!0, !1, !7, !8, !9, !10}
!0 = !{i32 1, !"wchar_size", i32 4}
!1 = !{i32 7, !"PIC Level", i32 2}
!llvm.ident = !{!2}
!2 = !{!".NET for Android remotes/origin/release/9.0.1xx @ 9abff7703206541fdb83ffa80fe2c2753ad1997b"}
!3 = !{!4, !4, i64 0}
!4 = !{!"any pointer", !5, i64 0}
!5 = !{!"omnipotent char", !6, i64 0}
!6 = !{!"Simple C++ TBAA"}
!7 = !{i32 1, !"branch-target-enforcement", i32 0}
!8 = !{i32 1, !"sign-return-address", i32 0}
!9 = !{i32 1, !"sign-return-address-all", i32 0}
!10 = !{i32 1, !"sign-return-address-with-bkey", i32 0}
