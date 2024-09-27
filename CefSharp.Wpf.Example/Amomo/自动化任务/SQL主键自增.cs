using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using System.Runtime.InteropServices;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Amomo
{

    class SQL主键自增
    {

        // 加载配置数据

        private static DateTime? 缓存创建时间 = null;
        private static DateTime? 缓存修改时间 = null;
        private static int 检测次数 = 0; // 仅用于调试，避免无限死循环

        public static async Task<(bool 成功, Dictionary<string, 文件时间信息> 处理后的配置)> 主键自增(string SQL_Excel文件路径)
        {
            // var 文件名 = Path.GetFileName(文件路径); // 添加这一行来获取文件名

            // 确定配置文件路径，假设与Excel文件在同一目录下
            var SQL配置文件路径 = Path.Combine(Path.GetDirectoryName(SQL_Excel文件路径), "SQL配置文件.json");



            // 加载配置数据
            var 配置数据 = 配置管理器.从文件加载配置(SQL配置文件路径, SQL_Excel文件路径);

            Debug.WriteLine("配置数据: " + 配置数据);

            var SQL_Excel文件名无扩展 = Path.GetFileNameWithoutExtension(SQL_Excel文件路径); // 获取不含扩展名的文件名

            if (File.Exists(SQL_Excel文件路径))
            {
                var 创建时间 = File.GetCreationTime(SQL_Excel文件路径);
                var 修改时间 = File.GetLastWriteTime(SQL_Excel文件路径);


                Debug.WriteLine($"检测到文件: {SQL_Excel文件名无扩展} 创建时间={创建时间}, 修改时间={修改时间}");



               Amomo.高精度计时器.获取并重置();

                if (配置数据.Count > 0 && 配置数据.TryGetValue(Path.GetFileNameWithoutExtension(SQL_Excel文件路径), out 文件时间信息 当前文件配置))
                {
                    if (配置数据.TryGetValue(Path.GetFileNameWithoutExtension(SQL_Excel文件路径), out 文件时间信息 文件记录))
                    {
                        缓存创建时间 = 文件记录.创建时间;
                        缓存修改时间 = 文件记录.修改时间;
                        Debug.WriteLine($"成功从配置中初始化缓存时间：创建时间={缓存创建时间}, 修改时间={缓存修改时间}，耗时: {Amomo.高精度计时器.获取并重置()}");

                    }
                    else
                    {
                        Debug.WriteLine($"未能在配置中找到 {Path.GetFileNameWithoutExtension(SQL_Excel文件路径)} 的记录，无法初始化缓存时间。");
                    }
                    // 根据是否格式化及文件时间戳决定是否处理文件
                    

                    if (缓存创建时间 != 创建时间 || 缓存修改时间 != 修改时间 || 当前文件配置.是否格式化 != true)
                    {
                        Debug.WriteLine($"{SQL_Excel文件名无扩展} 文件发生变化，开始处理...");

                        try
                        {
                            using (var 包 = new ExcelPackage(new FileInfo(SQL_Excel文件路径)))
                            {
                                Amomo.高精度计时器.获取并重置();

                                Debug.WriteLine($"检查工作簿中是否有工作表: " + Amomo.高精度计时器.获取并重置());

                                var 工作表 = 包.Workbook.Worksheets[0];

                                Debug.WriteLine($"工作表 = 包.Workbook.Worksheets[0];: " + Amomo.高精度计时器.获取并重置());
                                // 获取工作表的当前名称
                                var 当前工作表名称 = 工作表.Name;
                                Debug.WriteLine($"获取工作表的当前名称: " + Amomo.高精度计时器.获取并重置());
                                // 如果工作表名称与文件名（不含扩展名）不一致，则修改工作表名称
                                if (!当前工作表名称.Equals(SQL_Excel文件名无扩展, StringComparison.OrdinalIgnoreCase))
                                {
                                    工作表.Name = SQL_Excel文件名无扩展;

                                    Debug.WriteLine($"更新 {SQL_Excel文件名无扩展} 的工作表名称: {工作表.Name}  耗时: {Amomo.高精度计时器.获取并重置()}");

                                }


                                bool 首行存在合并单元格或全为空 = 工作表.Cells[1, 1, 1, 工作表.Dimension.End.Column].Any(单元格 => 单元格.Merge) ||
                                                                    工作表.Cells[1, 1, 1, 工作表.Dimension.End.Column].All(单元格 => string.IsNullOrWhiteSpace(单元格.Text));




                                if (首行存在合并单元格或全为空)
                                {
                                    工作表.DeleteRow(1, 1);
                                    Debug.WriteLine($"{SQL_Excel文件名无扩展} 正在删除首行 存在合并单元格或全为空... 耗时: {Amomo.高精度计时器.获取并重置()}");
                                    配置数据[SQL_Excel文件名无扩展].是否格式化 = true;
                                }


                                bool 首列存在合并单元格或全为空 = 工作表.Cells[1, 1, 工作表.Dimension.End.Row, 1].Any(单元格 => 单元格.Merge) ||
                                                                   工作表.Cells[1, 1, 工作表.Dimension.End.Row, 1].All(单元格 => string.IsNullOrWhiteSpace(单元格.Text));

                                if (首列存在合并单元格或全为空)
                                {
                                    工作表.DeleteColumn(1);
                                    Debug.WriteLine($"{SQL_Excel文件名无扩展} 正在删除首列 存在合并单元格或全为空... 耗时: {Amomo.高精度计时器.获取并重置()}");
                                    配置数据[SQL_Excel文件名无扩展].是否格式化 = true;
                                }



                                // 检测第一列的第一行是否名为ID
                                if (工作表.Cells[1, 1].Text != "主键ID")
                                {
                                    // 在第一列前插入一列
                                    工作表.InsertColumn(1, 1);
                                    工作表.Cells[1, 1].Value = "主键ID";

                                    // 复制原第二列样式到新插入的列，并设置固定格式
                                    for (int 行号 = 1; 行号 <= 工作表.Dimension.End.Row; 行号++)
                                    {
                                        var 原单元格 = 工作表.Cells[行号, 2]; // 原第二列现在是第三列
                                        var 新单元格 = 工作表.Cells[行号, 1];

                                        // 复制样式
                                        新单元格.StyleID = 原单元格.StyleID;

                                        // 设置单元格格式为常规，字体居中
                                        var 单元格样式 = 新单元格.Style as ExcelStyle;
                                        单元格样式.Numberformat.Format = "@";
                                        单元格样式.Font.Bold = false;
                                        单元格样式.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    }

                                    // 自增数值的初始值
                                    int 自增值 = 1;

                                    // 从第二行开始循环到第二列的列尾
                                    int 行号自增 = 2;
                                    while (工作表.Cells[行号自增, 2].Text != string.Empty)
                                    {
                                        工作表.Cells[行号自增, 1].Value = 自增值;
                                        自增值++;
                                        行号自增++;
                                    }
                                    Debug.WriteLine($"主键ID自增耗时: " + Amomo.高精度计时器.获取并重置());

                                    // 假设ID列是第一列，查找ID列最后一个有数值的行号
                                    int 最后有数值的行号 = 1;
                                    Debug.WriteLine($"{SQL_Excel文件名无扩展} 正在查找ID列最后一个有数值的行号...");
                                    while (工作表.Cells[最后有数值的行号 + 1, 1].Value != null)
                                    {
                                        最后有数值的行号++;
                                    }
                                    Debug.WriteLine($"{SQL_Excel文件名无扩展} 找到主键ID列最后一个有数值的行号：{最后有数值的行号}" + Amomo.高精度计时器.获取并重置());

                                    // 删除ID列最后一行之后的所有行
                                    if (最后有数值的行号 < 工作表.Dimension.End.Row)
                                    {
                                        Debug.WriteLine($"准备删除主键ID列第 {最后有数值的行号 + 1} 行之后的所有行...");
                                        // 直接删除最后有数值的行号之后的所有行
                                        工作表.DeleteRow(最后有数值的行号 + 1, 工作表.Dimension.End.Row - 最后有数值的行号);
                                        Debug.WriteLine($"{SQL_Excel文件名无扩展} 删除了 ID 后面的所有行。" + "耗时: " + Amomo.高精度计时器.获取并重置());
                                    }
                                    else
                                    {
                                        Debug.WriteLine($"{SQL_Excel文件名无扩展} 主键ID列已经是最后一行，没有更多行可删除。" + "耗时: " + Amomo.高精度计时器.获取并重置());
                                    }


                                    Debug.WriteLine($"{SQL_Excel文件名无扩展} 已检测到文件并进行了修改。" + "耗时: " + Amomo.高精度计时器.获取并重置());

                                    // 更新缓存时间戳
                                    缓存创建时间 = File.GetCreationTime(SQL_Excel文件路径);
                                    缓存修改时间 = File.GetLastWriteTime(SQL_Excel文件路径);
                                    当前文件配置.创建时间 = 缓存创建时间 ?? default(DateTime);
                                    当前文件配置.修改时间 = 缓存修改时间 ?? default(DateTime);
                                    当前文件配置.是否格式化 = true;

                                    // 输出调试信息
                                    Debug.WriteLine($"{SQL_Excel文件名无扩展} 文件检查完成: 创建时间={缓存创建时间}, 修改时间={缓存修改时间}");

                                }
                                else
                                {
                                    Debug.WriteLine($"{SQL_Excel文件名无扩展} 第一列的第一行已名为 '主键ID' ，无需修改。" + "耗时: " + Amomo.高精度计时器.获取并重置());
                                }


                                // 保存修改
                                包.Save();


                            }
                        }
                        catch (IOException ex)
                        {
                            Debug.WriteLine($"{SQL_Excel文件名无扩展} 文件操作失败: {ex.Message}");
                        }
                        catch (UnauthorizedAccessException ex)
                        {
                            Debug.WriteLine($"{SQL_Excel文件名无扩展} 文件访问被拒绝: {ex.Message}");
                        }



                        // 处理完成后，更新时间戳 回写配置文件
                        当前文件配置.创建时间 = File.GetCreationTime(SQL_Excel文件路径);
                        当前文件配置.修改时间 = File.GetLastWriteTime(SQL_Excel文件路径);
                        当前文件配置.是否格式化 = true;

                        配置管理器.将配置保存到文件(SQL配置文件路径, 配置数据);
                    }
                    else
                    {
                        Debug.WriteLine($"{SQL_Excel文件名无扩展} 文件未发生变化且已格式化，无需处理。");
                    }
                }
                else
                {
                    Debug.WriteLine($"无法在配置中找到 {SQL_Excel文件名无扩展} 的记录。");
                }


            }
            else
            {
                Debug.WriteLine($"{SQL_Excel文件名无扩展} 文件不存在。");
            }

            Debug.WriteLine($"{SQL_Excel文件名无扩展}  任务完成,返回了配置数据。");
            //返回处理后的配置结果

            foreach (var kvp in 配置数据)
            {
                var 文件时间记录 = kvp.Value;

                Debug.WriteLine($"文件名: {kvp.Key}"); // 使用 kvp.Key 直接代替了之前的 "文件名无扩展"
                Debug.WriteLine($"创建时间: {文件时间记录.创建时间}");
                Debug.WriteLine($"修改时间: {文件时间记录.修改时间}");
                Debug.WriteLine($"是否格式化: {文件时间记录.是否格式化}");
                Debug.WriteLine("--------------------"); // 分隔不同记录
            }

            //Debug.WriteLine($"配置数据总条目数: {配置数据.Count}");

            return (true, 配置数据);

        }


        public static class 配置管理器
        {

            public static void 确保配置文件存在(string 文件路径)
            {
                if (!File.Exists(文件路径))
                {
                    File.Create(文件路径).Dispose(); // 创建一个空文件并立即关闭
                    Debug.WriteLine($"配置文件不存在，已自动创建于路径: {文件路径}");
                }
            }
            public static void 将配置保存到文件(string 文件路径, Dictionary<string, 文件时间信息> 配置数据)
            {
                string json文本 = JsonConvert.SerializeObject(配置数据, Formatting.Indented);
                File.WriteAllText(文件路径, json文本);
            }


            public static Dictionary<string, 文件时间信息> 从文件加载配置(string SQL配置文件路径, string SQL_Excel文件路径)
            {
                if (File.Exists(SQL配置文件路径))
                {
                    string json文本 = File.ReadAllText(SQL配置文件路径);
                    var 配置数据 = JsonConvert.DeserializeObject<Dictionary<string, 文件时间信息>>(json文本) ?? new Dictionary<string, 文件时间信息>();

                    // 只处理与当前SQL_Excel文件路径相关的记录
                    获取或添加文件记录(配置数据, SQL_Excel文件路径);

                    // 返回的配置数据现在应该只包含与当前文件相关的项，但为了明确，
                    // 我们实际只需要返回这个特定的记录，而不是整个配置字典。
                    // 因此，我们找到这个特定记录并以它为结果返回。
                    string 文件名无扩展 = Path.GetFileNameWithoutExtension(SQL_Excel文件路径);
                    if (配置数据.TryGetValue(文件名无扩展, out 文件时间信息 特定文件记录))
                    {
                        return new Dictionary<string, 文件时间信息> { { 文件名无扩展, 特定文件记录 } };
                    }
                    else
                    {
                        // 这个分支理论上不会执行，因为我们之前已经确保了记录的存在。
                        // 但出于完整性考虑，这里提供一个默认返回。
                        return new Dictionary<string, 文件时间信息>();
                    }
                }
                return new Dictionary<string, 文件时间信息>();
            }

            public static 文件时间信息 获取或添加文件记录(Dictionary<string, 文件时间信息> 配置数据, string SQL_Excel文件路径)
            {
                string 文件名无扩展 = Path.GetFileNameWithoutExtension(SQL_Excel文件路径);
                if (!配置数据.TryGetValue(文件名无扩展, out 文件时间信息 文件记录))
                {
                    文件记录 = new 文件时间信息
                    {
                        创建时间 = File.GetCreationTime(SQL_Excel文件路径),
                        修改时间 = File.GetLastWriteTime(SQL_Excel文件路径),
                        是否格式化 = false
                    };
                    配置数据.Add(文件名无扩展, 文件记录);
                }
                return 文件记录;
            }


        }
        public class 文件时间信息
        {
            public DateTime 创建时间 { get; set; }
            public DateTime 修改时间 { get; set; }
            public bool 是否格式化 { get; set; } = false; // 初始化为false
        }

        public static async Task 开始检测(string 文件路径)
        {
            while (true)
            {
                检测次数++; // 增加检测次数
                Debug.WriteLine($"开始第 {检测次数} 次检测...");

                await 主键自增(文件路径);

                // 调试：设置一个退出条件，仅用于调试，避免无限循环
                if (检测次数 >= 2)
                    break;

                // 每隔1秒进行检测
                await Task.Delay(1000);
            }
        }
    }

}
