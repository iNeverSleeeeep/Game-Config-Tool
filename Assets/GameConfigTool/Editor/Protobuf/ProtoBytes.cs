using LuaInterface;
using NPOI.SS.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace GCT
{
    internal class ProtoBytes
    {
        public static void Generate(IEnumerable<GCTExcel> excels)
        {
            ProtoHeader.Generate();
            var sw = new Stopwatch();
            sw.Start();
            Parallel.ForEach(excels, PCall.Call<GCTExcel>(GenerateThread));
            sw.Stop();
            Debugger.Log(string.Format("全部ProtoBytes生成完成，耗时{0:N2}秒", (float)sw.ElapsedMilliseconds / 1000));
        }

        private static void GenerateThread(GCTExcel excel)
        {
            var sw = new Stopwatch();
            sw.Start();
            var L = LuaDLL.luaL_newstate();
            LuaDLL.luaL_openlibs(L);
            LuaDLL.tolua_openlibs(L);
            LuaDLL.luaopen_cpb(L);
            LuaPushFunction(L, Print);
            LuaDLL.lua_setglobal(L, "print");

            int oldTop = LuaDLL.lua_gettop(L);

            var databytes = File.ReadAllBytes(GCTSettings.Instance.OutputPath + "/protolua/" + excel.name + ".lua");
            var md5 = new StringBuilder();
            foreach (var hash in MD5.Create().ComputeHash(databytes))
                md5.Append(hash.ToString("X2"));
            excel.md5 = md5.ToString();
            var datalist = File.ReadAllText(GCTSettings.Instance.OutputPath + "/protolua/" + excel.name + ".lua");
            var table = string.Format("{{tests = {0}, md5 = \"{1}\"}}", datalist, excel.md5);

            var proto = File.ReadAllBytes(GCTSettings.Instance.OutputPath + "/protolua/" + excel.name + ".proto.bytes");


            var code = new StringBuilder();
            code.AppendFormat(
@"
function encode(proto)
    pb.load(proto)
    return pb.encode('{0}', {1})
end", "Config." + excel.name + "list", table);

            var bytes = Encoding.UTF8.GetBytes(code.ToString());
            if (LuaDLL.luaL_loadbuffer(L, bytes, bytes.Length, excel.name) == 0)
            {
                LuaDLL.lua_pcall(L, 0, 0, 0);
                LuaDLL.lua_getglobal(L, "encode");
                LuaDLL.lua_pushlstring(L, proto, proto.Length);
                if (LuaDLL.lua_pcall(L, 1, 1, 0) == 0)
                {
                    int len = 0;
                    var source = LuaDLL.lua_tolstring(L, -1, out len);
                    byte[] buffer = new byte[len];
                    Marshal.Copy(source, buffer, 0, len);
                    FileHelper.WriteAllBytes(GCTSettings.Instance.OutputPath + "/protobin/" + excel.name + ".bin", buffer);
                }
                else
                {
                    string error = LuaDLL.lua_tostring(L, -1);
                    Debugger.LogError(error);
                }
            }
            else
            {
                string error = LuaDLL.lua_tostring(L, -1);
                Debugger.LogError(error);
            }

            LuaDLL.lua_settop(L, oldTop - 1);
            LuaDLL.lua_close(L);
            sw.Stop();
            Debugger.Log(string.Format("生成{0}.bin完成，耗时{1:N2}秒", excel.name, (float)sw.ElapsedMilliseconds / 1000));
        }

        public static void LuaPushFunction(IntPtr L, LuaCSFunction func)
        {
            IntPtr fn = Marshal.GetFunctionPointerForDelegate(func);
            LuaDLL.lua_pushcclosure(L, fn, 0);
        }

        private static int Print(IntPtr L)
        {
            int n = LuaDLL.lua_gettop(L);
            var sb = new StringBuilder();
            for (int i = 1; i <= n; ++i)
            {
                if (i > 1)
                {
                    sb.Append("    ");
                }

                var text = PrintVariable(L, i, 0);
                sb.Append(text);
            }

            Debugger.Log(sb.ToString());
            return 0;
        }

        private static string PrintVariable(IntPtr l, int i, int depth)
        {
            if (LuaDLL.lua_isstring(l, i) == 1)
            {
                return LuaDLL.lua_tostring(l, i);
            }
            else if (LuaDLL.lua_isnil(l, i))
            {
                return "nil";
            }
            else if (LuaDLL.lua_isboolean(l, i))
            {
                return LuaDLL.lua_toboolean(l, i) ? "true" : "false";
            }
            else if (LuaDLL.tolua_isint64(l, i))
            {
                return LuaDLL.tolua_toint64(l, i).ToString();
            }
            else if (LuaDLL.tolua_isuint64(l, i))
            {
                return LuaDLL.tolua_touint64(l, i).ToString();
            }
            else if (LuaDLL.lua_istable(l, i))
            {
                if (depth > 3)
                {
                    return "{...}";
                }

                var sb = new StringBuilder();
                sb.Append("{");

                int count = 0;
                LuaDLL.lua_pushvalue(l, i);
                LuaDLL.lua_pushnil(l);
                while (LuaDLL.lua_next(l, i) != 0)
                {
                    LuaDLL.lua_pushvalue(l, -2);
                    var key = PrintVariable(l, -1, depth + 1);
                    var value = PrintVariable(l, -2, depth + 1);
                    if (count == 0)
                    {
                        sb.AppendFormat("{0} = {1}", key, value);
                    }
                    else
                    {
                        sb.AppendFormat(", {0} = {1}", key, value);
                    }

                    LuaDLL.lua_pop(l, 2);
                    ++count;
                }

                LuaDLL.lua_pop(l, 1);
                sb.Append("}");
                return sb.ToString();
            }
            else if (LuaDLL.lua_isuserdata(l, i) != 0)
            {
                return "userdata:null";
            }
            else
            {
                var p = LuaDLL.lua_topointer(l, i);
                if (p == IntPtr.Zero)
                {
                    return "nil";
                }
                else
                {
                    return string.Format(
                        "{0}:0x{1}",
                        LuaDLL.luaL_typename(l, i),
                        p.ToString("X"));
                }
            }
        }
    }
}

