using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sweeter.Models
{
    /// <summary>
    /// 控制器模型
    /// </summary>
    public class ControllerModel
    {
        /// <summary>
        /// 控制器名称
        /// </summary>
        public string ControllerName { get; set; }
        /// <summary>
        /// 控制器注释
        /// </summary>
        public string ControllerSummary { get; set; }
        /// <summary>
        /// 方法列表
        /// </summary>
        public List<MethodModel> MethodList = new List<MethodModel>();
    }

     /// <summary>
    /// 方法模型
    /// </summary>
    public class MethodModel
    {
        public MethodModel() { }

        public MethodModel(Member member)
        {
            this.Method = member.Method;
            this.MethodName = member.MethodName;
            this.MethodSummary = member.Summary.Value;
            this.ControllerName = member.ControllerName;
            this.ReturnRemark = member.Returns.Remark;
        }
        /// <summary>
        /// 方法
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// 方法名
        /// </summary>
        public string MethodName { get; set; }
        /// <summary>
        /// 方法注释
        /// </summary>
        public string MethodSummary { get; set; }
        /// <summary>
        /// 方法请求类型
        /// </summary>
        public string MethodType = "Get";
        /// <summary>
        /// 控制器名称
        /// </summary>
        public string ControllerName { get; set; }
        /// <summary>
        /// 方法路径
        /// </summary>
        public string MethodUrl { 
            get {
                return string.Format("/{0}/{1}", this.ControllerName.Replace("Controller", ""), this.MethodName);
            } 
        }
        /// <summary>
        /// 参数列表
        /// </summary>
        public List<JsonModle> ParamList = new List<JsonModle>();
        /// <summary>
        /// 返回类说明列表
        /// </summary>
        public List<JsonModle> ReturnList = new List<JsonModle>();
        /// <summary>
        /// 返回备注
        /// </summary>
        public string ReturnRemark { get; set; }
    }

    

    /// <summary>
    /// Json模型
    /// </summary>
    public class JsonModle
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public JsonModle() { }
        /// <summary>
        /// 字段名称
        /// </summary>
        public string ColumName { get; set; }
        /// <summary>
        /// 字段说明
        /// </summary>
        public string ColumSummary { get; set; }
        /// <summary>
        /// 字段类型
        /// </summary>
        public string ColumType { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue { get; set; }
        /// <summary>
        /// 子字段列表
        /// </summary>
        public List<JsonModle> ChildColumList = new List<JsonModle>();
    }
}
