// System.Runtime.InteropServices.Marshal.cs
//
// Sean MacIsaac (macisaac@ximian.com)
// Paolo Molaro (lupus@ximian.com)
// Dietmar Maurer (dietmar@ximian.com)
// Jonathan Chambers (joncham@gmail.com)
//
// (C) 2001-2002 Ximian, Inc.

//
// Copyright (C) 2004 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System;
using System.Security;
using System.Reflection;
using System.Threading;

using System.Runtime.ConstrainedExecution;
#if !FULL_AOT_RUNTIME
using System.Runtime.InteropServices.ComTypes;
using Mono.Interop;
#endif

namespace System.Runtime.InteropServices
{
	public static class Marshal
	{
		/* fields */
		public static readonly int SystemMaxDBCSCharSize = 2; // don't know what this is
		public static readonly int SystemDefaultCharSize = Environment.IsRunningOnWindows ? 2 : 1;

#if !MOBILE
		[MethodImplAttribute (MethodImplOptions.InternalCall)]
		private extern static int AddRefInternal (IntPtr pUnk);
#endif

		public static int AddRef (IntPtr pUnk)
		{
#if !MOBILE
			if (pUnk == IntPtr.Zero)
				throw new ArgumentException ("Value cannot be null.", "pUnk");
			return AddRefInternal (pUnk);
#else
			throw new NotImplementedException ();
#endif
		}

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		public extern static IntPtr AllocCoTaskMem (int cb);

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		[ReliabilityContractAttribute (Consistency.WillNotCorruptState, Cer.MayFail)]
		public extern static IntPtr AllocHGlobal (IntPtr cb);

		[ReliabilityContractAttribute (Consistency.WillNotCorruptState, Cer.MayFail)]
		public static IntPtr AllocHGlobal (int cb)
		{
			return AllocHGlobal ((IntPtr)cb);
		}

		[MonoTODO]
		public static object BindToMoniker (string monikerName)
		{
			throw new NotImplementedException ();
		}

		[MonoTODO]
		public static void ChangeWrapperHandleStrength (object otp, bool fIsWeak)
		{
			throw new NotImplementedException ();
		}

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal extern static void copy_to_unmanaged (Array source, int startIndex,
							       IntPtr destination, int length);

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal extern static void copy_from_unmanaged (IntPtr source, int startIndex,
								 Array destination, int length);

		public static void Copy (byte[] source, int startIndex, IntPtr destination, int length)
		{
			copy_to_unmanaged (source, startIndex, destination, length);
		}

		public static void Copy (char[] source, int startIndex, IntPtr destination, int length)
		{
			copy_to_unmanaged (source, startIndex, destination, length);
		}

		public static void Copy (short[] source, int startIndex, IntPtr destination, int length)
		{
			copy_to_unmanaged (source, startIndex, destination, length);
		}

		public static void Copy (int[] source, int startIndex, IntPtr destination, int length)
		{
			copy_to_unmanaged (source, startIndex, destination, length);
		}

		public static void Copy (long[] source, int startIndex, IntPtr destination, int length)
		{
			copy_to_unmanaged (source, startIndex, destination, length);
		}

		public static void Copy (float[] source, int startIndex, IntPtr destination, int length)
		{
			copy_to_unmanaged (source, startIndex, destination, length);
		}

		public static void Copy (double[] source, int startIndex, IntPtr destination, int length)
		{
			copy_to_unmanaged (source, startIndex, destination, length);
		}

		public static void Copy (IntPtr[] source, int startIndex, IntPtr destination, int length)
		{
			copy_to_unmanaged (source, startIndex, destination, length);
		}

		public static void Copy (IntPtr source, byte[] destination, int startIndex, int length)
		{
			copy_from_unmanaged (source, startIndex, destination, length);
		}

		public static void Copy (IntPtr source, char[] destination, int startIndex, int length)
		{
			copy_from_unmanaged (source, startIndex, destination, length);
		}

		public static void Copy (IntPtr source, short[] destination, int startIndex, int length)
		{
			copy_from_unmanaged (source, startIndex, destination, length);
		}

		public static void Copy (IntPtr source, int[] destination, int startIndex, int length)
		{
			copy_from_unmanaged (source, startIndex, destination, length);
		}

		public static void Copy (IntPtr source, long[] destination, int startIndex, int length)
		{
			copy_from_unmanaged (source, startIndex, destination, length);
		}

		public static void Copy (IntPtr source, float[] destination, int startIndex, int length)
		{
			copy_from_unmanaged (source, startIndex, destination, length);
		}

		public static void Copy (IntPtr source, double[] destination, int startIndex, int length)
		{
			copy_from_unmanaged (source, startIndex, destination, length);
		}

		public static void Copy (IntPtr source, IntPtr[] destination, int startIndex, int length)
		{
			copy_from_unmanaged (source, startIndex, destination, length);
		}

		public static IntPtr CreateAggregatedObject (IntPtr pOuter,
							     object o)
		{
			throw new NotImplementedException ();
		}

#if NET_4_5
		public static IntPtr CreateAggregatedObject<T> (IntPtr pOuter, T o) {
			return CreateAggregatedObject (pOuter, (object)o);
		}
#endif

#if !FULL_AOT_RUNTIME
		public static object CreateWrapperOfType (object o, Type t)
		{
			__ComObject co = o as __ComObject;
			if (co == null)
				throw new ArgumentException ("o must derive from __ComObject", "o");
			if (t == null)
				throw new ArgumentNullException ("t");

			Type[] itfs = o.GetType ().GetInterfaces ();
			foreach (Type itf in itfs) {
				if (itf.IsImport && co.GetInterface (itf) == IntPtr.Zero)
					throw new InvalidCastException ();
			}

			return ComInteropProxy.GetProxy (co.IUnknown, t).GetTransparentProxy ();
		}

#if NET_4_5
		public static TWrapper CreateWrapperOfType<T, TWrapper> (T o) {
			return (TWrapper)CreateWrapperOfType ((object)o, typeof (TWrapper));
		}
#endif
#endif

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		[ComVisible (true)]
		public extern static void DestroyStructure (IntPtr ptr, Type structuretype);

#if NET_4_5
		public static void DestroyStructure<T> (IntPtr ptr) {
			DestroyStructure (ptr, typeof (T));
		}
#endif			

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		public extern static void FreeBSTR (IntPtr ptr);

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		public extern static void FreeCoTaskMem (IntPtr ptr);

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		[ReliabilityContractAttribute (Consistency.WillNotCorruptState, Cer.Success)]
		public extern static void FreeHGlobal (IntPtr hglobal);

		static void ClearBSTR (IntPtr ptr)
		{
			int len = ReadInt32 (ptr, -4);

			for (int i = 0; i < len; i++)
				WriteByte (ptr, i, 0);
		}
		
		public static void ZeroFreeBSTR (IntPtr s)
		{
			ClearBSTR (s);
			FreeBSTR (s);
		}

		static void ClearAnsi (IntPtr ptr)
		{
			for (int i = 0; ReadByte (ptr, i) != 0; i++)
				WriteByte (ptr, i, 0);
		}

		static void ClearUnicode (IntPtr ptr)
		{
			for (int i = 0; ReadInt16 (ptr, i) != 0; i += 2)
				WriteInt16 (ptr, i, 0);
		}
		
		public static void ZeroFreeCoTaskMemAnsi (IntPtr s)
		{
			ClearAnsi (s);
			FreeCoTaskMem (s);
		}

		public static void ZeroFreeCoTaskMemUnicode (IntPtr s)
		{
			ClearUnicode (s);
			FreeCoTaskMem (s);
		}

		public static void ZeroFreeGlobalAllocAnsi (IntPtr s)
		{
			ClearAnsi (s);
			FreeHGlobal (s);
		}

		public static void ZeroFreeGlobalAllocUnicode (IntPtr s)
		{
			ClearUnicode (s);
			FreeHGlobal (s);
		}

#if !FULL_AOT_RUNTIME
		public static Guid GenerateGuidForType (Type type)
		{
			return type.GUID;
		}

		public static string GenerateProgIdForType (Type type)
		{
			IList<CustomAttributeData> attrs = CustomAttributeData.GetCustomAttributes (type);

			foreach (var a in attrs)
			{
				var dt = a.Constructor.DeclaringType;
				string name = dt.Name;
				if (name == "ProgIdAttribute")
				{
					var args = a.ConstructorArguments;
					string text = a.ConstructorArguments[0].Value as string;
					if (text == null)
					{
						text = string.Empty;
					}
					return text;
				}
			}

			return type.FullName;
		}

		[MonoTODO]
		public static object GetActiveObject (string progID)
		{
			throw new NotImplementedException ();
		}

#if !MOBILE
		[MethodImplAttribute (MethodImplOptions.InternalCall)]
		private extern static IntPtr GetCCW (object o, Type T);

		private static IntPtr GetComInterfaceForObjectInternal (object o, Type T)
		{
			if (IsComObject (o))
				return ((__ComObject)o).GetInterface (T);
			else
				return GetCCW (o, T);
		}
#endif

		public static IntPtr GetComInterfaceForObject (object o, Type T)
		{
#if !MOBILE
			IntPtr pItf = GetComInterfaceForObjectInternal (o, T);
			AddRef (pItf);
			return pItf;
#else
			throw new NotImplementedException ();
#endif
		}

#if NET_4_5
		public static IntPtr GetComInterfaceForObject<T, TInterface> (T o) {
			return GetComInterfaceForObject ((object)o, typeof (T));
		}
#endif			

		[MonoTODO]
		public static IntPtr GetComInterfaceForObjectInContext (object o, Type t)
		{
			throw new NotImplementedException ();
		}

		[MonoNotSupportedAttribute ("MSDN states user code should never need to call this method.")]
		public static object GetComObjectData (object obj, object key)
		{
			throw new NotSupportedException ("MSDN states user code should never need to call this method.");
		}

#if !MOBILE
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		private extern static int GetComSlotForMethodInfoInternal (MemberInfo m);
#endif

		public static int GetComSlotForMethodInfo (MemberInfo m)
		{
#if !MOBILE
			if (m == null)
				throw new ArgumentNullException ("m");
			if (!(m is MethodInfo))
				throw new ArgumentException ("The MemberInfo must be an interface method.", "m");
			if (!m.DeclaringType.IsInterface)
				throw new ArgumentException ("The MemberInfo must be an interface method.", "m");
			return GetComSlotForMethodInfoInternal (m);
#else
			throw new NotImplementedException ();
#endif
		}

		[MonoTODO]
		public static int GetEndComSlot (Type t)
		{
			throw new NotImplementedException ();
		}

		[MonoTODO]
		public static int GetExceptionCode()
		{
			throw new NotImplementedException ();
		}

		[MonoTODO]
		[ComVisible (true)]
		public static IntPtr GetExceptionPointers()
		{
			throw new NotImplementedException ();
		}

		public static IntPtr GetHINSTANCE (Module m)
		{
			if (m == null)
				throw new ArgumentNullException ("m");

			return m.GetHINSTANCE ();
		}
#endif // !FULL_AOT_RUNTIME

#if !FULL_AOT_RUNTIME
		[MonoTODO ("SetErrorInfo")]
		public static int GetHRForException (Exception e)
		{
			return e.hresult;
		}

		[MonoTODO]
		[ReliabilityContract (Consistency.WillNotCorruptState, Cer.Success)]
		public static int GetHRForLastWin32Error()
		{
			throw new NotImplementedException ();
		}

		[MethodImplAttribute (MethodImplOptions.InternalCall)]
		private extern static IntPtr GetIDispatchForObjectInternal (object o);

		public static IntPtr GetIDispatchForObject (object o)
		{
			IntPtr pUnk = GetIDispatchForObjectInternal (o);
			// Internal method does not AddRef
			AddRef (pUnk);
			return pUnk;
		}

		[MonoTODO]
		public static IntPtr GetIDispatchForObjectInContext (object o)
		{
			throw new NotImplementedException ();
		}

		[MonoTODO]
		public static IntPtr GetITypeInfoForType (Type t)
		{
			throw new NotImplementedException ();
		}

		[MethodImplAttribute (MethodImplOptions.InternalCall)]
		private extern static IntPtr GetIUnknownForObjectInternal (object o);

		public static IntPtr GetIUnknownForObject (object o)
		{
			IntPtr pUnk = GetIUnknownForObjectInternal (o);
			// Internal method does not AddRef
			AddRef (pUnk);
			return pUnk;
		}

		[MonoTODO]
		public static IntPtr GetIUnknownForObjectInContext (object o)
		{
			throw new NotImplementedException ();
		}

		[MonoTODO]
		[Obsolete ("This method has been deprecated")]
		public static IntPtr GetManagedThunkForUnmanagedMethodPtr (IntPtr pfnMethodToWrap, IntPtr pbSignature, int cbSignature)
		{
			throw new NotImplementedException ();
		}

		[MonoTODO]
		public static MemberInfo GetMethodInfoForComSlot (Type t, int slot, ref ComMemberType memberType)
		{
			throw new NotImplementedException ();
		}

		public static void GetNativeVariantForObject (object obj, IntPtr pDstNativeVariant)
		{
			Variant vt = new Variant();
			vt.SetValue(obj);
			Marshal.StructureToPtr(vt, pDstNativeVariant, false);
		}

#if NET_4_5
		public static void GetNativeVariantForObject<T> (T obj, IntPtr pDstNativeVariant) {
			GetNativeVariantForObject ((object)obj, pDstNativeVariant);
		}
#endif

#if !MOBILE
		[MethodImplAttribute (MethodImplOptions.InternalCall)]
		private static extern object GetObjectForCCW (IntPtr pUnk);
#endif

		public static object GetObjectForIUnknown (IntPtr pUnk)
		{
#if !MOBILE
			object obj = GetObjectForCCW (pUnk);
			// was not a CCW
			if (obj == null) {
				ComInteropProxy proxy = ComInteropProxy.GetProxy (pUnk, typeof (__ComObject));
				obj = proxy.GetTransparentProxy ();
			}
			return obj;
#else
			throw new NotImplementedException ();
#endif
		}

		public static object GetObjectForNativeVariant (IntPtr pSrcNativeVariant)
		{
			Variant vt = (Variant)Marshal.PtrToStructure(pSrcNativeVariant, typeof(Variant));
			return vt.GetValue();
		}

#if NET_4_5
		public static T GetObjectForNativeVariant<T> (IntPtr pSrcNativeVariant) {
			Variant vt = (Variant)Marshal.PtrToStructure(pSrcNativeVariant, typeof(Variant));
			return (T)vt.GetValue();
		}
#endif

		public static object[] GetObjectsForNativeVariants (IntPtr aSrcNativeVariant, int cVars)
		{
			if (cVars < 0)
				throw new ArgumentOutOfRangeException ("cVars", "cVars cannot be a negative number.");
			object[] objects = new object[cVars];
			for (int i = 0; i < cVars; i++)
				objects[i] = GetObjectForNativeVariant ((IntPtr)(aSrcNativeVariant.ToInt64 () +
					i * SizeOf (typeof(Variant))));
			return objects;
		}

#if NET_4_5
		public static T[] GetObjectsForNativeVariants<T> (IntPtr aSrcNativeVariant, int cVars) {
			if (cVars < 0)
				throw new ArgumentOutOfRangeException ("cVars", "cVars cannot be a negative number.");
			T[] objects = new T[cVars];
			for (int i = 0; i < cVars; i++)
				objects[i] = GetObjectForNativeVariant<T> ((IntPtr)(aSrcNativeVariant.ToInt64 () +
					i * SizeOf (typeof(Variant))));
			return objects;
		}
#endif

		[MonoTODO]
		public static int GetStartComSlot (Type t)
		{
			throw new NotImplementedException ();
		}

		[MonoTODO]
		[Obsolete ("This method has been deprecated")]
		public static Thread GetThreadFromFiberCookie (int cookie)
		{
			throw new NotImplementedException ();
		}

		public static object GetTypedObjectForIUnknown (IntPtr pUnk, Type t)
		{
			ComInteropProxy proxy = new ComInteropProxy (pUnk, t);
			__ComObject co = (__ComObject)proxy.GetTransparentProxy ();
			foreach (Type itf in t.GetInterfaces ()) {
				if ((itf.Attributes & TypeAttributes.Import) == TypeAttributes.Import) {
					if (co.GetInterface (itf) == IntPtr.Zero)
						return null;
				}
			}
			return co;
		}

		[MonoTODO]
		public static Type GetTypeForITypeInfo (IntPtr piTypeInfo)
		{
			throw new NotImplementedException ();
		}

#if !FULL_AOT_RUNTIME
		[Obsolete]
		[MonoTODO]
		public static string GetTypeInfoName (UCOMITypeInfo pTI)
		{
			throw new NotImplementedException ();
		}

		public static string GetTypeInfoName (ITypeInfo typeInfo)
		{
			throw new NotImplementedException ();
		}

		[Obsolete]
		[MonoTODO]
		public static Guid GetTypeLibGuid (UCOMITypeLib pTLB)
		{
			throw new NotImplementedException ();
		}

		[MonoTODO]
		public static Guid GetTypeLibGuid (ITypeLib typelib)
		{
			throw new NotImplementedException ();
		}

		[MonoTODO]
		public static Guid GetTypeLibGuidForAssembly (Assembly asm)
		{
			throw new NotImplementedException ();
		}

		[Obsolete]
		[MonoTODO]
		public static int GetTypeLibLcid (UCOMITypeLib pTLB)
		{
			throw new NotImplementedException ();
		}

		[MonoTODO]
		public static int GetTypeLibLcid (ITypeLib typelib)
		{
			throw new NotImplementedException ();
		}

		[Obsolete]
		[MonoTODO]
		public static string GetTypeLibName (UCOMITypeLib pTLB)
		{
			throw new NotImplementedException ();
		}

		[MonoTODO]
		public static string GetTypeLibName (ITypeLib typelib)
		{
			throw new NotImplementedException ();
		}

		[MonoTODO]
		public static void GetTypeLibVersionForAssembly (Assembly inputAssembly, out int majorVersion, out int minorVersion)
		{
			throw new NotImplementedException ();
		}

		public static object GetUniqueObjectForIUnknown (IntPtr unknown)
		{
			throw new NotImplementedException ();
		}
#endif

		[MonoTODO]
		[Obsolete ("This method has been deprecated")]
		public static IntPtr GetUnmanagedThunkForManagedMethodPtr (IntPtr pfnMethodToWrap, IntPtr pbSignature, int cbSignature)
		{
			throw new NotImplementedException ();
		}

#if !MOBILE
		[MethodImplAttribute (MethodImplOptions.InternalCall)]
		public extern static bool IsComObject (object o);
#else
		public static bool IsComObject (object o)
		{
			throw new NotImplementedException ();
		}
#endif		

		[MonoTODO]
		public static bool IsTypeVisibleFromCom (Type t)
		{
			throw new NotImplementedException ();
		}

		[MonoTODO]
		public static int NumParamBytes (MethodInfo m)
		{
			throw new NotImplementedException ();
		}
#endif

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		[ReliabilityContractAttribute (Consistency.WillNotCorruptState, Cer.Success)]
		public static extern int GetLastWin32Error();

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		public extern static IntPtr OffsetOf (Type t, string fieldName);

#if NET_4_5
		public static IntPtr OffsetOf<T> (string fieldName) {
			return OffsetOf (typeof (T), fieldName);
		}
#endif

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		public extern static void Prelink (MethodInfo m);

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		public extern static void PrelinkAll (Type c);

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		public extern static string PtrToStringAnsi (IntPtr ptr);
		
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		public extern static string PtrToStringAnsi (IntPtr ptr, int len);

		public static string PtrToStringAuto (IntPtr ptr)
		{
			return SystemDefaultCharSize == 2
				? PtrToStringUni (ptr) : PtrToStringAnsi (ptr);
		}
		
		public static string PtrToStringAuto (IntPtr ptr, int len)
		{
			return SystemDefaultCharSize == 2
				? PtrToStringUni (ptr, len) : PtrToStringAnsi (ptr, len);
		}

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		public extern static string PtrToStringUni (IntPtr ptr);

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		public extern static string PtrToStringUni (IntPtr ptr, int len);

#if !MOBILE
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		public extern static string PtrToStringBSTR (IntPtr ptr);
#else
		public static string PtrToStringBSTR (IntPtr ptr)
		{
			throw new NotImplementedException ();
		}
#endif
		
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		[ComVisible (true)]
		public extern static void PtrToStructure (IntPtr ptr, object structure);

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		[ComVisible (true)]
		public extern static object PtrToStructure (IntPtr ptr, Type structureType);

#if NET_4_5
		public static void PtrToStructure<T> (IntPtr ptr, T structure) {
			PtrToStructure (ptr, (object)structure);
		}

		public static object PtrToStructure<T> (IntPtr ptr) {
			return PtrToStructure (ptr, typeof (T));
		}
#endif

#if !MOBILE
		[MethodImplAttribute (MethodImplOptions.InternalCall)]
		private extern static int QueryInterfaceInternal (IntPtr pUnk, ref Guid iid, out IntPtr ppv);
#endif

		public static int QueryInterface (IntPtr pUnk, ref Guid iid, out IntPtr ppv)
		{
#if !MOBILE
			if (pUnk == IntPtr.Zero)
				throw new ArgumentException ("Value cannot be null.", "pUnk");
			return QueryInterfaceInternal (pUnk, ref iid, out ppv);
#else
			throw new NotImplementedException ();
#endif
		}

		public static byte ReadByte (IntPtr ptr)
		{
			unsafe {
				return *(byte*)ptr;
			}
		}

		public static byte ReadByte (IntPtr ptr, int ofs) {
			unsafe {
				return *((byte*)ptr + ofs);
			}
		}

		[MonoTODO]
		[SuppressUnmanagedCodeSecurity]
		public static byte ReadByte ([In, MarshalAs (UnmanagedType.AsAny)] object ptr, int ofs)
		{
			throw new NotImplementedException ();
		}

		public unsafe static short ReadInt16 (IntPtr ptr)
		{
			byte *addr = (byte *) ptr;
			
			// The mono JIT can't inline this due to the hight number of calls
			// return ReadInt16 (ptr, 0);
			
			if (((uint)addr & 1) == 0) 
				return *(short*)addr;

			short s;
			String.memcpy ((byte*)&s, (byte*)ptr, 2);
			return s;
		}

		public unsafe static short ReadInt16 (IntPtr ptr, int ofs)
		{
			byte *addr = ((byte *) ptr) + ofs;

			if (((uint) addr & 1) == 0)
				return *(short*)addr;

			short s;
			String.memcpy ((byte*)&s, addr, 2);
			return s;
		}

		[MonoTODO]
		[SuppressUnmanagedCodeSecurity]
		public static short ReadInt16 ([In, MarshalAs(UnmanagedType.AsAny)] object ptr, int ofs)
		{
			throw new NotImplementedException ();
		}

		[ReliabilityContractAttribute (Consistency.WillNotCorruptState, Cer.Success)]
		public unsafe static int ReadInt32 (IntPtr ptr)
		{
			byte *addr = (byte *) ptr;
			
			if (((uint)addr & 3) == 0) 
				return *(int*)addr;

			int s;
			String.memcpy ((byte*)&s, addr, 4);
			return s;
		}

		[ReliabilityContractAttribute (Consistency.WillNotCorruptState, Cer.Success)]
		public unsafe static int ReadInt32 (IntPtr ptr, int ofs)
		{
			byte *addr = ((byte *) ptr) + ofs;
			
			if ((((int) addr) & 3) == 0)
				return *(int*)addr;
			else {
				int s;
				String.memcpy ((byte*)&s, addr, 4);
				return s;
			}
		}

		[ReliabilityContractAttribute (Consistency.WillNotCorruptState, Cer.Success)]
		[MonoTODO]
		[SuppressUnmanagedCodeSecurity]
		public static int ReadInt32 ([In, MarshalAs(UnmanagedType.AsAny)] object ptr, int ofs)
		{
			throw new NotImplementedException ();
		}

		[ReliabilityContractAttribute (Consistency.WillNotCorruptState, Cer.Success)]
		public unsafe static long ReadInt64 (IntPtr ptr)
		{
			byte *addr = (byte *) ptr;
				
			// The real alignment might be 4 on some platforms, but this is just an optimization,
			// so it doesn't matter.
			if (((uint) addr & 7) == 0)
				return *(long*)ptr;

			long s;
			String.memcpy ((byte*)&s, addr, 8);
			return s;
		}

		public unsafe static long ReadInt64 (IntPtr ptr, int ofs)
		{
			byte *addr = ((byte *) ptr) + ofs;

			if (((uint) addr & 7) == 0)
				return *(long*)addr;
			
			long s;
			String.memcpy ((byte*)&s, addr, 8);
			return s;
		}

		[ReliabilityContractAttribute (Consistency.WillNotCorruptState, Cer.Success)]
		[MonoTODO]
		[SuppressUnmanagedCodeSecurity]
		public static long ReadInt64 ([In, MarshalAs (UnmanagedType.AsAny)] object ptr, int ofs)
		{
			throw new NotImplementedException ();
		}

		[ReliabilityContractAttribute (Consistency.WillNotCorruptState, Cer.Success)]
		public static IntPtr ReadIntPtr (IntPtr ptr)
		{
			if (IntPtr.Size == 4)
				return (IntPtr)ReadInt32 (ptr);
			else
				return (IntPtr)ReadInt64 (ptr);
		}
		
		[ReliabilityContractAttribute (Consistency.WillNotCorruptState, Cer.Success)]
		public static IntPtr ReadIntPtr (IntPtr ptr, int ofs)
		{
			if (IntPtr.Size == 4)
				return (IntPtr)ReadInt32 (ptr, ofs);
			else
				return (IntPtr)ReadInt64 (ptr, ofs);
		}

		[ReliabilityContractAttribute (Consistency.WillNotCorruptState, Cer.Success)]
		[MonoTODO]
		public static IntPtr ReadIntPtr ([In, MarshalAs (UnmanagedType.AsAny)] object ptr, int ofs)
		{
			throw new NotImplementedException ();
		}

		[MethodImplAttribute (MethodImplOptions.InternalCall)]
		public extern static IntPtr ReAllocCoTaskMem (IntPtr pv, int cb);

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		public extern static IntPtr ReAllocHGlobal (IntPtr pv, IntPtr cb);

#if !MOBILE
		[ReliabilityContractAttribute (Consistency.WillNotCorruptState, Cer.Success)]
		[MethodImplAttribute (MethodImplOptions.InternalCall)]
		private extern static int ReleaseInternal (IntPtr pUnk);
#endif

		[ReliabilityContract (Consistency.WillNotCorruptState, Cer.Success)]
		public static int Release (IntPtr pUnk)
		{
#if !MOBILE
			if (pUnk == IntPtr.Zero)
				throw new ArgumentException ("Value cannot be null.", "pUnk");

			return ReleaseInternal (pUnk);
#else
			throw new NotImplementedException ();
#endif
		}

#if !FULL_AOT_RUNTIME
		[MethodImplAttribute (MethodImplOptions.InternalCall)]
		private extern static int ReleaseComObjectInternal (object co);

		public static int ReleaseComObject (object o)
		{
			if (o == null)
				throw new ArgumentException ("Value cannot be null.", "o");
			if (!IsComObject (o))
				throw new ArgumentException ("Value must be a Com object.", "o");
			return ReleaseComObjectInternal (o);
		}

		[Obsolete]
		[MonoTODO]
		public static void ReleaseThreadCache()
		{
			throw new NotImplementedException ();
		}

		[MonoNotSupportedAttribute ("MSDN states user code should never need to call this method.")]
		public static bool SetComObjectData (object obj, object key, object data)
		{
			throw new NotSupportedException ("MSDN states user code should never need to call this method.");
		}
#endif

		[ComVisible (true)]
		public static int SizeOf (object structure)
		{
			return SizeOf (structure.GetType ());
		}

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		public extern static int SizeOf (Type t);

#if NET_4_5
		public static int SizeOf<T> () {
			return SizeOf (typeof (T));
		}

		public static int SizeOf<T> (T structure) {
			return SizeOf (structure.GetType ());
		}
#endif

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		public extern static IntPtr StringToBSTR (string s);

		//
		// I believe this is wrong, because in Mono and in P/Invoke
		// we treat "Ansi" conversions as UTF-8 conversions, while
		// this one does not do this
		//
		public static IntPtr StringToCoTaskMemAnsi (string s)
		{
			int length = s.Length + 1;
			IntPtr ctm = AllocCoTaskMem (length);

			byte[] asBytes = new byte[length];
			for (int i = 0; i < s.Length; i++)
				asBytes[i] = (byte)s[i];
			asBytes[s.Length] = 0;

			copy_to_unmanaged (asBytes, 0, ctm, length);
			return ctm;
		}

		public static IntPtr StringToCoTaskMemAuto (string s)
		{
			return SystemDefaultCharSize == 2
				? StringToCoTaskMemUni (s) : StringToCoTaskMemAnsi (s);
		}

		public static IntPtr StringToCoTaskMemUni (string s)
		{
			int length = s.Length + 1;
			IntPtr ctm = AllocCoTaskMem (length * 2);
			
			char[] asChars = new char[length];
			s.CopyTo (0, asChars, 0, s.Length);
			asChars[s.Length] = '\0';

			copy_to_unmanaged (asChars, 0, ctm, length);
			return ctm;
		}

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		public extern static IntPtr StringToHGlobalAnsi (string s);

		public static IntPtr StringToHGlobalAuto (string s)
		{
			return SystemDefaultCharSize == 2
				? StringToHGlobalUni (s) : StringToHGlobalAnsi (s);
		}

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		public extern static IntPtr StringToHGlobalUni (string s);

		public static IntPtr SecureStringToBSTR (SecureString s)
		{
			if (s == null)
				throw new ArgumentNullException ("s");
			int len = s.Length;
			IntPtr ctm = AllocCoTaskMem ((len+1) * 2 + 4);
			byte [] buffer = null;
			WriteInt32 (ctm, 0, len*2);
			try {
				buffer = s.GetBuffer ();

				for (int i = 0; i < len; i++)
					WriteInt16 (ctm, 4 + (i * 2), (short) ((buffer [(i*2)] << 8) | (buffer [i*2+1])));
				WriteInt16 (ctm, 4 + buffer.Length, 0);
			} finally {
				if (buffer != null)
					for (int i = buffer.Length; i > 0; ){
						i--;
						buffer [i] = 0;
					}
			}
			return (IntPtr) ((long)ctm + 4);
		}

		public static IntPtr SecureStringToCoTaskMemAnsi (SecureString s)
		{
			if (s == null)
				throw new ArgumentNullException ("s");
			int len = s.Length;
			IntPtr ctm = AllocCoTaskMem (len + 1);
			byte [] copy = new byte [len+1];

			try {
				byte [] buffer = s.GetBuffer ();
				int i = 0, j = 0;
				for (; i < len; i++, j += 2){
					copy [i] = buffer [j+1];
					buffer [j] = 0;
					buffer [j+1] = 0;
				}
				copy [i] = 0;
				copy_to_unmanaged (copy, 0, ctm, len+1);
			} finally {
				// Ensure that we clear the buffer.
				for (int i = len; i > 0; ){
					i--;
					copy [i] = 0;
				}
			}
			return ctm;
		}

		public static IntPtr SecureStringToCoTaskMemUnicode (SecureString s)
		{
			if (s == null)
				throw new ArgumentNullException ("s");
			int len = s.Length;
			IntPtr ctm = AllocCoTaskMem (len * 2 + 2);
			byte [] buffer = null;
			try {
				buffer = s.GetBuffer ();
				for (int i = 0; i < len; i++)
					WriteInt16 (ctm, i * 2, (short) ((buffer [(i*2)] << 8) | (buffer [i*2+1])));
				WriteInt16 (ctm, buffer.Length, 0);
			} finally {
				if (buffer != null)
					for (int i = buffer.Length; i > 0; ){
						i--;
						buffer [i] = 0;
					}
			}
			return ctm;
		}

		public static IntPtr SecureStringToGlobalAllocAnsi (SecureString s)
		{
			if (s == null)
				throw new ArgumentNullException ("s");
			return SecureStringToCoTaskMemAnsi (s);
		}

		public static IntPtr SecureStringToGlobalAllocUnicode (SecureString s)
		{
			if (s == null)
				throw new ArgumentNullException ("s");
			return SecureStringToCoTaskMemUnicode (s);
		}

		[ReliabilityContractAttribute (Consistency.WillNotCorruptState, Cer.MayFail)]
		[ComVisible (true)]
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		public extern static void StructureToPtr (object structure, IntPtr ptr, bool fDeleteOld);

#if NET_4_5
		public static void StructureToPtr<T> (T structure, IntPtr ptr, bool fDeleteOld) {
			StructureToPtr ((object)structure, ptr, fDeleteOld);
		}
#endif

		public static void ThrowExceptionForHR (int errorCode) {
			Exception ex = GetExceptionForHR (errorCode);
			if (ex != null)
				throw ex;
		}

		public static void ThrowExceptionForHR (int errorCode, IntPtr errorInfo) {
			Exception ex = GetExceptionForHR (errorCode, errorInfo);
			if (ex != null)
				throw ex;
		}

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		public extern static IntPtr UnsafeAddrOfPinnedArrayElement (Array arr, int index);

#if NET_4_5
		public static IntPtr UnsafeAddrOfPinnedArrayElement<T> (T[] arr, int index) {
			return UnsafeAddrOfPinnedArrayElement ((Array)arr, index);
		}
#endif

		public static void WriteByte (IntPtr ptr, byte val)
		{
			unsafe {
				*(byte*)ptr = val;
			}
		}

		public static void WriteByte (IntPtr ptr, int ofs, byte val) {
			unsafe {
				*(byte*)(IntPtr.Add (ptr, ofs)) = val;
			}
		}

		[MonoTODO]
		[SuppressUnmanagedCodeSecurity]
		public static void WriteByte ([In, Out, MarshalAs (UnmanagedType.AsAny)] object ptr, int ofs, byte val)
		{
			throw new NotImplementedException ();
		}

		public static unsafe void WriteInt16 (IntPtr ptr, short val)
		{
			byte *addr = (byte *) ptr;
			
			if (((uint)addr & 1) == 0)
				*(short*)addr = val;
			else
				String.memcpy (addr, (byte*)&val, 2);
		}

		public static unsafe void WriteInt16 (IntPtr ptr, int ofs, short val)
		{
			byte *addr = ((byte *) ptr) + ofs;

			if (((uint)addr & 1) == 0)
				*(short*)addr = val;
			else {
				String.memcpy (addr, (byte*)&val, 2);
			}
		}

		[MonoTODO]
		[SuppressUnmanagedCodeSecurity]
		public static void WriteInt16 ([In, Out, MarshalAs (UnmanagedType.AsAny)] object ptr, int ofs, short val)
		{
			throw new NotImplementedException ();
		}

		public static void WriteInt16 (IntPtr ptr, char val)
		{
			WriteInt16 (ptr, 0, (short)val);
		}

		public static void WriteInt16 (IntPtr ptr, int ofs, char val)
		{
			WriteInt16 (ptr, ofs, (short)val);
		}

		[MonoTODO]
		public static void WriteInt16([In, Out] object ptr, int ofs, char val)
		{
			throw new NotImplementedException ();
		}

		public static unsafe void WriteInt32 (IntPtr ptr, int val)
		{
			byte *addr = (byte *) ptr;
			
			if (((uint)addr & 3) == 0) 
				*(int*)addr = val;
			else {
				String.memcpy (addr, (byte*)&val, 4);
			}
		}

		public unsafe static void WriteInt32 (IntPtr ptr, int ofs, int val)
		{
			byte *addr = ((byte *) ptr) + ofs;

			if (((uint)addr & 3) == 0) 
				*(int*)addr = val;
			else {
				String.memcpy (addr, (byte*)&val, 4);
			}
		}

		[MonoTODO]
		[SuppressUnmanagedCodeSecurity]
		public static void WriteInt32([In, Out, MarshalAs(UnmanagedType.AsAny)] object ptr, int ofs, int val)
		{
			throw new NotImplementedException ();
		}

		public static unsafe void WriteInt64 (IntPtr ptr, long val)
		{
			byte *addr = (byte *) ptr;
			
			// The real alignment might be 4 on some platforms, but this is just an optimization,
			// so it doesn't matter.
			if (((uint)addr & 7) == 0) 
				*(long*)addr = val;
			else 
				String.memcpy (addr, (byte*)&val, 8);
		}

		public static unsafe void WriteInt64 (IntPtr ptr, int ofs, long val)
		{
			byte *addr = ((byte *) ptr) + ofs;

			// The real alignment might be 4 on some platforms, but this is just an optimization,
			// so it doesn't matter.
			if (((uint)addr & 7) == 0) 
				*(long*)addr = val;
			else 
				String.memcpy (addr, (byte*)&val, 8);
		}

		[MonoTODO]
		[SuppressUnmanagedCodeSecurity]
		public static void WriteInt64 ([In, Out, MarshalAs (UnmanagedType.AsAny)] object ptr, int ofs, long val)
		{
			throw new NotImplementedException ();
		}

		public static void WriteIntPtr (IntPtr ptr, IntPtr val)
		{
			if (IntPtr.Size == 4)
				WriteInt32 (ptr, (int)val);
			else
				WriteInt64 (ptr, (long)val);
		}

		public static void WriteIntPtr (IntPtr ptr, int ofs, IntPtr val)
		{
			if (IntPtr.Size == 4)
				WriteInt32 (ptr, ofs, (int)val);
			else
				WriteInt64 (ptr, ofs, (long)val);
		}

		[MonoTODO]
		public static void WriteIntPtr([In, Out, MarshalAs(UnmanagedType.AsAny)] object ptr, int ofs, IntPtr val)
		{
			throw new NotImplementedException ();
		}

		public static Exception GetExceptionForHR (int errorCode) {
			return GetExceptionForHR (errorCode, IntPtr.Zero);
		}

		public static Exception GetExceptionForHR (int errorCode, IntPtr errorInfo) {

			const int E_OUTOFMEMORY = unchecked ((int)0x8007000EL);
			const int E_INVALIDARG = unchecked ((int)0X80070057);
			
			switch (errorCode)
			{
			case E_OUTOFMEMORY:
				return new OutOfMemoryException ();
			case E_INVALIDARG:
				return new ArgumentException ();
			}
			if (errorCode < 0)
				return new COMException ("", errorCode);
			return null;
		}

#if !FULL_AOT_RUNTIME
		public static int FinalReleaseComObject (object o)
		{
			while (ReleaseComObject (o) != 0);
			return 0;
		}
#endif

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		private static extern Delegate GetDelegateForFunctionPointerInternal (IntPtr ptr, Type t);

		public static Delegate GetDelegateForFunctionPointer (IntPtr ptr, Type t)
		{
			if (t == null)
				throw new ArgumentNullException ("t");
			if (!t.IsSubclassOf (typeof (MulticastDelegate)) || (t == typeof (MulticastDelegate)))
				throw new ArgumentException ("Type is not a delegate", "t");
			if (t.IsGenericType)
				throw new ArgumentException ("The specified Type must not be a generic type definition.");
			if (ptr == IntPtr.Zero)
				throw new ArgumentNullException ("ptr");

			return GetDelegateForFunctionPointerInternal (ptr, t);
		}

#if NET_4_5
		public static Delegate GetDelegateForFunctionPointer<T> (IntPtr ptr) {
			return GetDelegateForFunctionPointer (ptr, typeof (T));
		}
#endif

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetFunctionPointerForDelegateInternal (Delegate d);
		
		public static IntPtr GetFunctionPointerForDelegate (Delegate d)
		{
			if (d == null)
				throw new ArgumentNullException ("d");
			
			return GetFunctionPointerForDelegateInternal (d);
		}

#if NET_4_5
		public static IntPtr GetFunctionPointerForDelegate<TDelegate> (TDelegate d) {
			if (d == null)
				throw new ArgumentNullException ("d");
			
			return GetFunctionPointerForDelegateInternal ((Delegate)(object)d);
		}
#endif
	}
}
