using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace nng.Native.Ctx
{
    using static Globals;

#if NETSTANDARD2_0
    [System.Security.SuppressUnmanagedCodeSecurity]
#endif
    public sealed class UnsafeNativeMethods
    {
        [DllImport(NngDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 nng_ctx_open(out nng_ctx ctx, nng_socket socket);

        [DllImport(NngDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 nng_ctx_close(nng_ctx ctx);

        [DllImport(NngDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 nng_ctx_id(nng_ctx ctx);

        [DllImport(NngDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void nng_ctx_recv(nng_ctx ctx, nng_aio aio);

        [DllImport(NngDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void nng_ctx_send(nng_ctx ctx, nng_aio aio);

        [DllImport(NngDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int nng_ctx_get(nng_ctx ctx, string name, byte[] data, UIntPtr size);

        [DllImport(NngDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int nng_ctx_get_bool(nng_ctx ctx, string name, out bool data);

        [DllImport(NngDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int nng_ctx_get_int(nng_ctx ctx, string name, out Int32 data);

        [DllImport(NngDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int nng_ctx_get_ms(nng_ctx ctx, string name, out nng_duration data);

        [DllImport(NngDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int nng_ctx_get_ptr(nng_ctx ctx, string name, out IntPtr data);

        [DllImport(NngDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int nng_ctx_get_size(nng_ctx ctx, string name, out UIntPtr data);

        [DllImport(NngDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int nng_ctx_get_string(nng_ctx ctx, string name, out IntPtr data);

        [DllImport(NngDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int nng_ctx_get_uint64(nng_ctx ctx, string name, out UInt64 data);


        [DllImport(NngDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int nng_ctx_set(nng_ctx ctx, string name, byte[] data, UIntPtr size);

        public static int nng_ctx_set(nng_ctx ctx, string name, byte[] data)
        {
            return nng_ctx_set(ctx, name, data, (UIntPtr)data.Length);
        }

        [DllImport(NngDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int nng_ctx_set_bool(nng_ctx ctx, string name, bool value);

        [DllImport(NngDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int nng_ctx_set_int(nng_ctx ctx, string name, Int32 value);

        [DllImport(NngDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int nng_ctx_set_ms(nng_ctx ctx, string name, nng_duration value);

        [DllImport(NngDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int nng_ctx_set_ptr(nng_ctx ctx, string name, IntPtr value);

        [DllImport(NngDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int nng_ctx_set_size(nng_ctx ctx, string name, UIntPtr value);

        [DllImport(NngDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int nng_ctx_set_string(nng_ctx ctx, string name, string value);

        [DllImport(NngDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int nng_ctx_set_uint64(nng_ctx ctx, string name, UInt64 value);
    }
}