using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.XWPF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sweeter
{
    public class NpoiWordHelper
    {
        /// <summary>
        /// 创建文档
        /// </summary>
        /// <param name="setting"></param>
        public static byte[] ExportDocument(DocumentSetting setting)
        {
            XWPFDocument docx = new XWPFDocument();
            MemoryStream ms = new MemoryStream();

            //设置文档
            docx.Document.body.sectPr = new CT_SectPr();
            CT_SectPr setPr = docx.Document.body.sectPr;
            //获取页面大小
            Tuple<int, int> size = GetPaperSize(setting.PaperType);
            setPr.pgSz.w = (ulong)size.Item1;
            setPr.pgSz.h = (ulong)size.Item2;
            //创建一个段落
            CT_P p = docx.Document.body.AddNewP();
            //段落水平居中
            p.AddNewPPr().AddNewJc().val = ST_Jc.center;
            XWPFParagraph gp = new XWPFParagraph(p, docx);

            XWPFRun gr = gp.CreateRun();

            foreach (var itemSetting in setting.ItemSettingList)
            {
                var titleSetting = itemSetting.TitleSetting;
                var mainContentSettingList = itemSetting.MainContentSettingList;

                //创建标题
                if (titleSetting != null && !string.IsNullOrEmpty(titleSetting.Title))
                {
                    gr.GetCTR().AddNewRPr().AddNewRFonts().ascii = titleSetting.FontName;
                    gr.GetCTR().AddNewRPr().AddNewRFonts().eastAsia = titleSetting.FontName;
                    gr.GetCTR().AddNewRPr().AddNewRFonts().hint = ST_Hint.eastAsia;
                    gr.GetCTR().AddNewRPr().AddNewSz().val = (ulong)titleSetting.FontSize;//2号字体
                    gr.GetCTR().AddNewRPr().AddNewSzCs().val = (ulong)titleSetting.FontSize;
                    gr.GetCTR().AddNewRPr().AddNewB().val = titleSetting.HasBold; //加粗
                    gr.GetCTR().AddNewRPr().AddNewColor().val = "black";//字体颜色
                    p.AddNewPPr().AddNewJc().val = ST_Jc.center;
                    gr.SetText(titleSetting.Title);
                }
                foreach (var mainContentSetting in mainContentSettingList)
                {
                    //创建文档主要内容
                    if (!string.IsNullOrEmpty(mainContentSetting.MainContent))
                    {
                        p = docx.Document.body.AddNewP();
                        p.AddNewPPr().AddNewJc().val = ST_Jc.both;
                        gp = new XWPFParagraph(p, docx)
                        {
                            IndentationFirstLine = 2
                        };

                        //单倍为默认值（240）不需设置，1.5倍=240X1.5=360，2倍=240X2=480
                        p.AddNewPPr().AddNewSpacing().line = "300";
                        p.AddNewPPr().AddNewSpacing().lineRule = ST_LineSpacingRule.exact;

                        gr = gp.CreateRun();
                        CT_RPr rpr = gr.GetCTR().AddNewRPr();
                        CT_Fonts rfonts = rpr.AddNewRFonts();
                        rfonts.ascii = mainContentSetting.FontName;
                        rfonts.eastAsia = mainContentSetting.FontName;
                        rpr.AddNewSz().val = (ulong)mainContentSetting.FontSize;//5号字体-21
                        rpr.AddNewSzCs().val = (ulong)mainContentSetting.FontSize;
                        rpr.AddNewB().val = mainContentSetting.HasBold;

                        gr.SetText(mainContentSetting.MainContent);
                    }
                    //创建文档表格
                    if (mainContentSetting.TableArray != null && mainContentSetting.TableArray.Length > 0)
                    {
                        var ary = mainContentSetting.TableArray;
                        //创建表格-提前创建好表格后填数
                        XWPFTable tableContent = docx.CreateTable(ary.Length, ary[0].Length);//行列
                        tableContent.Width = 1000 * ary[0].Length;
                        for (int i = 0; i < ary.Length; i++)
                        {
                            tableContent.SetColumnWidth(i, 1000);
                        }
                        for (var i = 0; i < ary.Length; i++)
                        {
                            var ls = ary[i];
                            for (var j = 0; j < ls.Length; j++)
                            {
                                tableContent.GetRow(i).GetCell(j).SetParagraph(SetCellText(docx, tableContent, ls[j].ToString()));
                            }
                        }
                    }
                }
            }


            //开始写入
            docx.Write(ms);
            //using (FileStream fs = new FileStream(setting.SavePath, FileMode.Create, FileAccess.Write))
            //{
            //    byte[] data = ms.ToArray();
            //    fs.Write(data, 0, data.Length);
            //    fs.Flush();
            //}
            //ms.Close();

            return ms.ToArray();
        }

        /// <summary>
        /// 设置文档
        /// </summary>
        public class DocumentSetting
        {
            private PaperType _paperType = PaperType.A4_V;
            /// <summary>
            /// 文档类型，默认为A4纵向
            /// </summary>
            public PaperType PaperType
            {
                get
                {
                    return this._paperType;
                }
                set
                {
                    this._paperType = value;
                }
            }
            /// <summary>
            /// 保存地址，包含文件名
            /// </summary>
            public string SavePath { get; set; }
            /// <summary>
            /// 文档标题内容list
            /// </summary>
            public List<ItemSetting> ItemSettingList { get; set; }

        }

        public class ItemSetting
        {
            /// <summary>
            /// 文档标题相关
            /// </summary>
            public ContentItemSetting TitleSetting { get; set; }
            /// <summary>
            /// 文档主要内容
            /// </summary>
            public List<ContentItemSetting> MainContentSettingList { get; set; }
        }


        /// <summary>
        /// 文档内容相关
        /// </summary>
        public class ContentItemSetting
        {
            /// <summary>
            /// 标题
            /// </summary>
            public string Title { get; set; }
            /// <summary>
            /// 主要内容
            /// </summary>
            public string MainContent { get; set; }
            public string[][] TableArray { get; set; }

            private string _fontName = "仿宋";
            /// <summary>
            /// 使用字体
            /// </summary>
            public string FontName
            {
                get
                {
                    return this._fontName;
                }
                set
                {
                    this._fontName = value;
                }
            }
            private int _fontSize = 34;
            /// <summary>
            /// 字体大小，默认2号字体
            /// </summary>
            public int FontSize
            {
                get
                {
                    return this._fontSize;
                }
                set
                {
                    this._fontSize = value;
                }
            }
            private bool _hasBold = false;
            /// <summary>
            /// 是否加粗，默认不加粗
            /// </summary>
            public bool HasBold
            {
                get
                {
                    return this._hasBold;
                }
                set
                {
                    this._hasBold = value;
                }
            }
        }

        /// <summary>
        /// 纸张类型
        /// </summary>
        public enum PaperType
        {
            /// <summary>
            /// A4纵向
            /// </summary>
            A4_V,
            /// <summary>
            /// A4横向
            /// </summary>
            A4_H,

            /// <summary>
            /// A5纵向
            /// </summary>
            A5_V,
            /// <summary>
            /// A5横向
            /// </summary>
            A5_H,

            /// <summary>
            /// A6纵向
            /// </summary>
            A6_V,
            /// <summary>
            /// A6横向
            /// </summary>
            A6_H
        }

        #region 私有方法
        /// <summary>
        /// 获取纸张大小，单位：Twip
        /// <para>换算关系：1英寸=1440缇  1厘米=567缇  1磅=20缇  1像素=15缇</para>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static Tuple<int, int> GetPaperSize(PaperType type)
        {
            Tuple<int, int> res = null;
            switch (type)
            {
                case PaperType.A4_V:
                    res = new Tuple<int, int>(11906, 16838);
                    break;
                case PaperType.A4_H:
                    res = new Tuple<int, int>(16838, 11906);
                    break;

                case PaperType.A5_V:
                    res = new Tuple<int, int>(8390, 11906);
                    break;
                case PaperType.A5_H:
                    res = new Tuple<int, int>(11906, 8390);
                    break;

                case PaperType.A6_V:
                    res = new Tuple<int, int>(5953, 8390);
                    break;
                case PaperType.A6_H:
                    res = new Tuple<int, int>(8390, 5953);
                    break;
            }
            return res;
        }

        //设置字体样式
        public static XWPFParagraph SetCellText(XWPFDocument doc, XWPFTable table, string setText)
        {
            //table中的文字格式设置  
            CT_P para = new CT_P();
            XWPFParagraph pCell = new XWPFParagraph(para, table.Body);
            pCell.Alignment = ParagraphAlignment.CENTER;//字体居中  
            pCell.VerticalAlignment = TextAlignment.CENTER;//字体居中  

            XWPFRun r1c1 = pCell.CreateRun();
            r1c1.SetText(setText);
            r1c1.FontSize = 10;
            r1c1.FontFamily = "华文楷体";
            //r1c1.SetTextPosition(20);//设置高度  
            return pCell;
        }
        #endregion
    }
}
