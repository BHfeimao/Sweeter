using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sweeter.Models
{
    /// <summary>
    /// XML成员
    /// </summary>
    public class Member
    {
        /// <summary>
        /// name属性值
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 按照‘.’切割成list
        /// </summary>
        private List<string> names
        {
            get
            {
                return Name.Split('.').ToList();
            }
        }
        /// <summary>
        /// 首字符
        /// </summary>
        public char PreWord 
        {
            get
            {
                return Name.First();
            }
        }
        /// <summary>
        /// 控制器名称
        /// </summary>
        public string ControllerName
        {
            get
            {
                try
                {
                    int i = names.IndexOf("Controllers");
                    if (i >= 0)
                        return names[i + 1];
                    return string.Empty;
                }
                catch { return string.Empty; }
            }
        }
        /// <summary>
        /// 控制器命名空间
        /// </summary>
        public string ControllerNamespace
        {
            get
            {
                try
                {
                    return Name.Substring(Name.IndexOf(':') + 1, Name.IndexOf(ControllerName) - Name.IndexOf(':') - 1) + ControllerName;
                }
                catch { return string.Empty; }
            }
        }
        /// <summary>
        /// 方法名称
        /// </summary>
        public string MethodName
        {
            get
            {
                try
                {
                    if (PreWord != 'M') return string.Empty;
                    string methodName = names[names.IndexOf("Controllers") + 2];
                    if (methodName.Contains('(')) methodName = methodName.Substring(0, methodName.IndexOf('('));
                    return methodName;
                }
                catch { return string.Empty; }
            }
        }
        /// <summary>
        /// 方法
        /// </summary>
        public string Method
        {
            get
            {
                string method = MethodName;
                if (Name.Contains('(')) { return method + Name.Substring(Name.IndexOf('(')); }
                return method;
            }
        }

        /// <summary>
        /// 注释
        /// </summary>
        public Summary Summary { get; set; }
        /// <summary>
        /// 参数列表
        /// </summary>
        public List<Param> ParamList { get; set; }
        /// <summary>
        /// 返回结果
        /// </summary>
        public Returns Returns { get; set; }
    }

    /// <summary>
    /// 注释类
    /// </summary>
    public class Summary
    {
        /// <summary>
        /// 注释内容
        /// </summary>
        public string Value { get; set; }
    }

    /// <summary>
    /// 参数类
    /// </summary>
    public class Param
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 参数注释
        /// </summary>
        public string Value { get; set; }
    }

    /// <summary>
    /// 返回类
    /// </summary>
    public class Returns
    {
        /// <summary>
        /// 返回注释内容 [Assembly]FullNamespace.ClassName; 或者 [Assembly].AfterTheNamespace.ClassName;
        /// </summary>
        public string Value { get; set; }
        private string[] values
        { 
            get 
            {
                return this.Value.Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace(" ","").Split(';');
            } 
        }
        /// <summary>
        /// 返回类所在程序集名称
        /// </summary>
        public string AssemblyName
        {
            get
            {
                try
                {
                    return values[0];
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
        /// <summary>
        /// 返回类型TypeFullName
        /// </summary>
        public string TypeFullName
        {
            get
            {
                try
                {
                    return values[1];
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark
        {
            get
            {
                try
                {
                    List<string> list = values.ToList();
                    list.RemoveAt(0); list.RemoveAt(0);
                    return string.Join(";", list);
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
    }
}
