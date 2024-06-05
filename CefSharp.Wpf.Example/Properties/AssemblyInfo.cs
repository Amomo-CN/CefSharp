// 引用系统基础命名空间，包含基本类型和编程模型支持
using System;

// 引入反射相关的命名空间，用于处理程序集和类型的元数据
using System.Reflection;

// 引入平台调用服务，允许托管代码调用非托管代码（如Windows API）
using System.Runtime.InteropServices;

// 引入CefSharp命名空间，包含CefSharp库中WPF应用使用的类和接口
using CefSharp;

// 设置程序集的标题，此信息通常在Windows资源管理器中查看文件属性时可见
[assembly: AssemblyTitle("CefSharp.Wpf.Example")]

// 指定公司名称，此属性描述程序集的制造商
[assembly: AssemblyCompany(AssemblyInfo.AssemblyCompany)]

// 定义产品名称，标识程序集属于哪个产品
[assembly: AssemblyProduct(AssemblyInfo.AssemblyProduct)]

// 设置版权信息，表明程序集的版权归属
[assembly: AssemblyCopyright("Copyright © 2023 YourCompanyName. All rights reserved.")]

// 控制程序集的COM可见性，决定程序集是否可以被COM客户端访问
[assembly: ComVisible(false)]

// 设定程序集的版本号，分为四部分：主版本.次版本.内部版本.修订版本
[assembly: AssemblyVersion("1.0.0.0")]

// 设置程序集的文件版本，通常与产品发布和更新管理相关联
[assembly: AssemblyFileVersion("1.0.0.0")]

// 指示程序集是否遵循CLS（公共语言规范），确保跨语言兼容性
[assembly: CLSCompliant(true)]