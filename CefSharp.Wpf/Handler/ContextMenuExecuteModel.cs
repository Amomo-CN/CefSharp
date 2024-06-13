//版权所有 © 2021 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System.Collections.Generic;

namespace CefSharp.Wpf.Handler
{
    /// <summary>
    /// 上下文菜单执行模型
    /// </summary>
    public class ContextMenuExecuteModel
    {
        /// <summary>
        /// 菜单命令
        /// </summary>
        public CefMenuCommand MenuCommand { get; private set; }
        /// <summary>
        /// 词典建议
        /// </summary>
        public IList<string> DictionarySuggestions { get; private set; }
        /// <summary>
        /// X坐标
        /// </summary>
        public int XCoord { get; private set; }
        /// <summary>
        /// Y坐标
        /// </summary>
        public int YCoord { get; private set; }
        /// <summary>
        /// 选择文本
        /// </summary>
        public string SelectionText { get; private set; }
        /// <summary>
        /// 拼写错误的单词
        /// </summary>
        public string MisspelledWord { get; private set; }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        /// <param name="menuCommand">菜单命令</param>
        ///<param name="dictionarySuggestions">词典建议</param>
        ///<param name="xCoord">x 坐标</param>
        ///<param name="yCoord">y 坐标</param>
        ///<param name="selectionText">选择文本</param>
        ///<param name="misspelledWord">拼写错误的单词</param>
        public ContextMenuExecuteModel(CefMenuCommand menuCommand, IList<string> dictionarySuggestions, int xCoord, int yCoord, string selectionText, string misspelledWord)
        {
            MenuCommand = menuCommand;
            DictionarySuggestions = dictionarySuggestions;
            XCoord = xCoord;
            YCoord = yCoord;
            SelectionText = selectionText;
            MisspelledWord = misspelledWord;
        }
    }
}
