using System;
using System.Text;
using System.Runtime.InteropServices;

namespace Squirrel
{
    using SQInteger = Int32;
    using HSQUIRRELVM = IntPtr;
    using SQRESULT = Int32;
    using SQBool = UInt32;
    using SQUnsignedInteger = UInt32;

#pragma warning disable 414
    public class MonoPInvokeCallbackAttribute : System.Attribute
    {
        private Type type;
        public MonoPInvokeCallbackAttribute(Type t)
        {
            type = t;
        }
    }
#pragma warning restore 414

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate SQInteger SQFUNCTION(HSQUIRRELVM v);
#else
	public delegate SQInteger SQFUNCTION(HSQUIRRELVM v);
#endif

    public class SquirrelDLL
    {
        public const SQBool SQFalse = 0;
        public const SQBool SQTrue = 1;

#if UNITY_IPHONE && !UNITY_EDITOR
		const string LUADLL = "__Internal";
#else
        const string LUADLL = "squirrel";
#endif
        public static bool SQ_FAILED(SQRESULT res) { return res < 0; }
        public static bool SQ_SUCCEEDED(SQRESULT res) { return res >= 0; }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern HSQUIRRELVM sq_open(SQInteger initialstacksize);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void sq_close(HSQUIRRELVM v);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void sq_pushroottable(HSQUIRRELVM v);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void sq_push(HSQUIRRELVM v, SQInteger idx);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void sq_pop(HSQUIRRELVM v, SQInteger nelemstopop);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void sq_poptop(HSQUIRRELVM v);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern SQInteger sq_gettop(HSQUIRRELVM v);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void sq_remove(HSQUIRRELVM v, SQInteger idx);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern SQRESULT sq_compilebuffer(HSQUIRRELVM v, byte[] s, SQInteger size, string sourcename, SQBool raiseerror);

        public static SQRESULT sq_compilebuffer(HSQUIRRELVM v, string s, string sourcename, SQBool raiseerror)
        {
            var bytes = Encoding.UTF8.GetBytes(s);
            return sq_compilebuffer(v, bytes, bytes.Length, sourcename, raiseerror);
        }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern SQRESULT sq_call(HSQUIRRELVM v, SQInteger args, SQBool retval, SQBool raiseerror);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void sq_pushstring(HSQUIRRELVM v, byte[] s, SQInteger len);

        public static void sq_pushstring(HSQUIRRELVM v, string s)
        {
            var bytes = Encoding.UTF8.GetBytes(s);
            sq_pushstring(v, bytes, bytes.Length);
        }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void sq_newclosure(HSQUIRRELVM v, IntPtr func, SQUnsignedInteger nfreevars);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern SQRESULT sq_newslot(HSQUIRRELVM v, SQInteger idx, SQBool bstatic);

        public static void sq_newclosure(HSQUIRRELVM v, SQFUNCTION function, SQUnsignedInteger nfreevars)
        {
            IntPtr fn = Marshal.GetFunctionPointerForDelegate(function);
            sq_newclosure(v, fn, nfreevars);
        }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern SQRESULT sq_getstring(HSQUIRRELVM v, SQInteger idx, out IntPtr c);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern SQRESULT sq_getinteger(HSQUIRRELVM v, SQInteger idx, out SQInteger i);

        public static SQRESULT sq_getstring(HSQUIRRELVM v, SQInteger idx, out string c)
        {
            IntPtr ptr;
            SQRESULT result;
            if (SQ_SUCCEEDED(result = sq_getstring(v, idx, out ptr)))
            {
                c = Marshal.PtrToStringAnsi(ptr);
            }
            else
            {
                c = null;
            }
            return result;
        }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern SQRESULT sq_tostring(HSQUIRRELVM v, SQInteger idx);
    }
}
