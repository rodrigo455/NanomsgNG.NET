using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace nng.Native.Tls
{
    using static Globals;

#if NETSTANDARD2_0
    [System.Security.SuppressUnmanagedCodeSecurity]
#endif
    public sealed class UnsafeNativeMethods
    {
        [DllImport(NngDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 nng_tls_config_alloc(out nng_tls_config config, nng_tls_mode mode);

        [DllImport(NngDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 nng_tls_config_hold(nng_tls_config config);

        [DllImport(NngDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 nng_tls_config_free(nng_tls_config config);

        [DllImport(NngDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 nng_tls_config_server_name(nng_tls_config config, string name);

        [DllImport(NngDll, CallingConvention = CallingConvention.Cdecl)]
        static extern Int32 nng_tls_config_ca_chain(nng_tls_config config, string chain, IntPtr crl);

        public static Int32 nng_tls_config_ca_chain(nng_tls_config config, string chain, string clr)
        {
            IntPtr clrPtr = Marshal.StringToHGlobalAuto(clr);
            return nng_tls_config_ca_chain(config, chain, clrPtr);
        }

        public static Int32 nng_tls_config_ca_chain(nng_tls_config config, string chain)
        {
            return nng_tls_config_ca_chain(config, chain, IntPtr.Zero);
        }

        [DllImport(NngDll, CallingConvention = CallingConvention.Cdecl)]
        static extern Int32 nng_tls_config_own_cert(nng_tls_config config, string cert, string key, IntPtr passwd);

        public static Int32 nng_tls_config_own_cert(nng_tls_config config, string cert, string key, string passwd)
        {
            IntPtr passwdPtr = Marshal.StringToHGlobalAuto(passwd);
            return nng_tls_config_own_cert(config, cert, key, passwdPtr);
        }

        public static Int32 nng_tls_config_own_cert(nng_tls_config config, string cert, string key)
        {
            return nng_tls_config_own_cert(config, cert, key, IntPtr.Zero);
        }

        [DllImport(NngDll, CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe Int32 nng_tls_config_key(nng_tls_config config, byte* key, UIntPtr size);

        public static Int32 nng_tls_config_key(nng_tls_config config, byte[] key)
        {
            unsafe
            {
                fixed (byte* ptr = &key[0])
                {
                    return nng_tls_config_key(config, ptr, (UIntPtr)key.Length);
                }
            }
        }

        [DllImport(NngDll, CallingConvention = CallingConvention.Cdecl)]
        static extern Int32 nng_tls_config_pass(nng_tls_config config, string pass);

        [DllImport(NngDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 nng_tls_config_auth_mode(nng_tls_config config, nng_tls_auth_mode mode);

        [DllImport(NngDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 nng_tls_config_ca_file(nng_tls_config config, string path);

        [DllImport(NngDll, CallingConvention = CallingConvention.Cdecl)]
        static extern Int32 nng_tls_config_cert_key_file(nng_tls_config config, string path, IntPtr passwd);

        public static Int32 nng_tls_config_cert_key_file(nng_tls_config config, string path, string passwd)
        {
            IntPtr passwdPtr = Marshal.StringToHGlobalAuto(passwd);
            return nng_tls_config_cert_key_file(config, path, passwdPtr);
        }

        public static Int32 nng_tls_config_cert_key_file(nng_tls_config config, string path)
        {
            return nng_tls_config_cert_key_file(config, path, IntPtr.Zero);
        }

        [DllImport(NngDll, CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe Int32 nng_tls_config_psk(nng_tls_config config, string identity, byte* data, UIntPtr size);

        public static Int32 nng_tls_config_psk(nng_tls_config config, string identity, byte[] key)
        {
            unsafe
            {
                fixed (byte* ptr = &key[0])
                {
                    return nng_tls_config_psk(config, identity, ptr, (UIntPtr)key.Length);
                }
            }
        }

        [DllImport(NngDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 nng_tls_config_version(nng_tls_config config, nng_tls_version min_ver, nng_tls_version max_ver);

        [DllImport(NngDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern string nng_tls_engine_name();

        [DllImport(NngDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern string nng_tls_engine_description();

        [DllImport(NngDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool nng_tls_engine_fips_mode();
    }
}