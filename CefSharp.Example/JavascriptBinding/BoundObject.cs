//版权所有 © 2010 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System;
using System.Linq;
using System.Threading.Tasks;

namespace CefSharp.Example.JavascriptBinding
{
    public class BoundObject
    {
        public int MyProperty { get; set; }

        public string MyReadOnlyProperty { get; internal set; }
        public Type MyUnconvertibleProperty { get; set; }
        public SubBoundObject SubObject { get; set; }
        public ExceptionTestBoundObject ExceptionTestObject { get; set; }

        public int this[int i]
        {
            get { return i; }
            set { }
        }

        public uint[] MyUintArray
        {
            get { return new uint[] { 7, 8 }; }
        }

        public int[] MyIntArray
        {
            get { return new[] { 1, 2, 3, 4, 5, 6, 7, 8 }; }
        }

        public Array MyArray
        {
            get { return new short[] { 1, 2, 3 }; }
        }

        public byte[] MyBytes
        {
            get { return new byte[] { 3, 4, 5 }; }
        }

        public BoundObject()
        {
            MyProperty = 42;
            MyReadOnlyProperty = "I'm immutable!";
            IgnoredProperty = "I am an Ignored Property";
            MyUnconvertibleProperty = GetType();
            SubObject = new SubBoundObject();
            ExceptionTestObject = new ExceptionTestBoundObject();
        }

        public void TestCallbackWithDateTime(IJavascriptCallback javascriptCallback)
        {
            Task.Run(async () =>
            {
                using (javascriptCallback)
                {
                    if (javascriptCallback.CanExecute)
                    {
                        var dateTime = new DateTime(2019, 01, 01, 12, 00, 00);
                        await javascriptCallback.ExecuteAsync(dateTime, new[] { dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second });
                    }
                }
            });
        }

        public void TestCallbackWithDateTime1900(IJavascriptCallback javascriptCallback)
        {
            Task.Run(async () =>
            {
                using (javascriptCallback)
                {
                    if (javascriptCallback.CanExecute)
                    {
                        var dateTime = new DateTime(1900, 01, 01, 12, 00, 00);
                        await javascriptCallback.ExecuteAsync(dateTime, new[] { dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second });
                    }
                }
            });
        }

        public void TestCallbackWithDateTime1970(IJavascriptCallback javascriptCallback)
        {
            Task.Run(async () =>
            {
                using (javascriptCallback)
                {
                    if (javascriptCallback.CanExecute)
                    {
                        var dateTime = new DateTime(1970, 01, 01, 12, 00, 00);
                        await javascriptCallback.ExecuteAsync(dateTime, new[] { dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second });
                    }
                }
            });
        }

        public void TestCallbackWithDateTime1985(IJavascriptCallback javascriptCallback)
        {
            Task.Run(async () =>
            {
                using (javascriptCallback)
                {
                    if (javascriptCallback.CanExecute)
                    {
                        var dateTime = new DateTime(1985, 01, 01, 12, 00, 00);
                        await javascriptCallback.ExecuteAsync(dateTime, new[] { dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second });
                    }
                }
            });
        }

        public void TestCallback(IJavascriptCallback javascriptCallback)
        {
            const int taskDelay = 1500;

            Task.Run(async () =>
            {
                await Task.Delay(taskDelay);

                using (javascriptCallback)
                {
                    if (javascriptCallback.CanExecute)
                    {
                        //NOTE: 不支持类，仅支持简单结构
                        var response = new CallbackResponseStruct("来自 C# 的回调被延迟 " + taskDelay + "ms");
                        await javascriptCallback.ExecuteAsync(response);
                    }
                }
            });
        }

        public string TestCallbackFromObject(SimpleClass simpleClass)
        {
            if (simpleClass == null)
            {
                return "TestCallbackFromObject 字典参数为 null";
            }

            IJavascriptCallback javascriptCallback = simpleClass.Callback;

            if (javascriptCallback == null)
            {
                return "未找到回调属性或属性不是函数";
            }

            const int taskDelay = 1500;

            Task.Run(async () =>
            {
                await Task.Delay(taskDelay);

                if (javascriptCallback != null)
                {
                    using (javascriptCallback)
                    {
                        if (javascriptCallback.CanExecute)
                        {
                            await javascriptCallback.ExecuteAsync("来自 C# 的消息 " + simpleClass.TestString + " - " + simpleClass.SubClasses[0].PropertyOne);
                        }
                    }
                }
            });

            return "等待回调执行...";
        }

        public int EchoMyProperty()
        {
            return MyProperty;
        }

        public string Repeat(string str, int n)
        {
            string result = String.Empty;
            for (int i = 0; i < n; i++)
            {
                result += str;
            }
            return result;
        }

        public string EchoParamOrDefault(string param = "这是默认值")
        {
            return param;
        }

        public void EchoVoid()
        {
        }

        public Boolean EchoBoolean(Boolean arg0)
        {
            return arg0;
        }

        public Boolean? EchoNullableBoolean(Boolean? arg0)
        {
            return arg0;
        }

        public SByte EchoSByte(SByte arg0)
        {
            return arg0;
        }

        public SByte? EchoNullableSByte(SByte? arg0)
        {
            return arg0;
        }

        public Int16 EchoInt16(Int16 arg0)
        {
            return arg0;
        }

        public Int16? EchoNullableInt16(Int16? arg0)
        {
            return arg0;
        }

        public Int32 EchoInt32(Int32 arg0)
        {
            return arg0;
        }

        public Int32? EchoNullableInt32(Int32? arg0)
        {
            return arg0;
        }

        public Int64 EchoInt64(Int64 arg0)
        {
            return arg0;
        }

        public Int64? EchoNullableInt64(Int64? arg0)
        {
            return arg0;
        }

        public Byte EchoByte(Byte arg0)
        {
            return arg0;
        }

        public Byte? EchoNullableByte(Byte? arg0)
        {
            return arg0;
        }

        public UInt16 EchoUInt16(UInt16 arg0)
        {
            return arg0;
        }

        public UInt16? EchoNullableUInt16(UInt16? arg0)
        {
            return arg0;
        }

        public UInt32 EchoUInt32(UInt32 arg0)
        {
            return arg0;
        }

        public UInt32? EchoNullableUInt32(UInt32? arg0)
        {
            return arg0;
        }

        public UInt64 EchoUInt64(UInt64 arg0)
        {
            return arg0;
        }

        public UInt64? EchoNullableUInt64(UInt64? arg0)
        {
            return arg0;
        }

        public Single EchoSingle(Single arg0)
        {
            return arg0;
        }

        public Single? EchoNullableSingle(Single? arg0)
        {
            return arg0;
        }

        public Double EchoDouble(Double arg0)
        {
            return arg0;
        }

        public Double? EchoNullableDouble(Double? arg0)
        {
            return arg0;
        }

        public Char EchoChar(Char arg0)
        {
            return arg0;
        }

        public Char? EchoNullableChar(Char? arg0)
        {
            return arg0;
        }

        public DateTime EchoDateTime(DateTime arg0)
        {
            return arg0;
        }

        public DateTime? EchoNullableDateTime(DateTime? arg0)
        {
            return arg0;
        }

        public Decimal EchoDecimal(Decimal arg0)
        {
            return arg0;
        }

        public Decimal? EchoNullableDecimal(Decimal? arg0)
        {
            return arg0;
        }

        public String EchoString(String arg0)
        {
            return arg0;
        }

        // TODO: 目前这不起作用，因为它会导致与 EchoString() 方法发生冲突。我想我们需要找到一种解决方法。
        //public String echoString(String arg)
        //{
        //    return "Lowercase echo: " + arg;
        //}

        public String lowercaseMethod()
        {
            return "lowercase";
        }

        public string ReturnJsonEmployeeList()
        {
            return "{\"employees\":[{\"firstName\":\"John\", \"lastName\":\"Doe\"},{\"firstName\":\"Anna\", \"lastName\":\"Smith\"},{\"firstName\":\"Peter\", \"lastName\":\"Jones\"}]}";
        }

        [JavascriptIgnore]
        public string IgnoredProperty { get; set; }

        [JavascriptIgnore]
        public string IgnoredMethod()
        {
            return "I am an Ignored Method";
        }

        public string ComplexParamObject(object param)
        {
            if (param == null)
            {
                return "param is null";
            }
            return "The param type is:" + param.GetType();
        }

        public SubBoundObject GetSubObject()
        {
            return SubObject;
        }

        /// <summary>
        /// 演示如何使用 params 作为绑定对象中的参数
        ///</摘要>
        ///<param name="name">虚拟参数</param>
        ///<param name="args">参数参数</param>
        public string MethodWithParams(string name, params object[] args)
        {
            return "Name:" + name + ";Args:" + string.Join(", ", args.ToArray());
        }

        public string MethodWithoutParams(string name, string arg2)
        {
            return string.Format("{0}, {1}", name, arg2);
        }

        public string MethodWithoutAnything()
        {
            return "没有任何调用并成功返回的方法.";
        }

        public string MethodWithThreeParamsOneOptionalOneArray(string name, string optionalParam = null, params object[] args)
        {
            return "MethodWithThreeParamsOneOptionalOneArray:" + (name ?? "未指定名称") + " - " + (optionalParam ?? "未指定可选参数") + ";Args:" + string.Join(", ", args.ToArray());
        }
    }
}
