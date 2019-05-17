using Sweeter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Configuration;
using NPOI.XWPF.UserModel;
using NPOI.OpenXmlFormats.Wordprocessing;

namespace Sweeter
{
    /// <summary>
    /// Sweeter控制器
    /// </summary>
    public class SweeterController : Controller
    {
        /// <summary>
        /// 目标主程序集路径
        /// </summary>
        private string assemlyUrl
        {
            get { return System.AppDomain.CurrentDomain.RelativeSearchPath + "/" + ConfigurationSettings.AppSettings["Sweeter_dllName"].ToString(); }
        }
        /// <summary>
        /// 是否显示接口文档
        /// </summary>
        private bool isShow
        {
            get { try { return Convert.ToBoolean(ConfigurationSettings.AppSettings["Sweeter_isShow"]); } catch { return false; } }
        }

        /// <summary>
        /// Sweeter主页
        /// </summary>
        /// <returns>View</returns>
        public ActionResult Index()
        {
            if (isShow)
                return Redirect("/Sweeter/Sweeter.html");
            return Content("Sweeter@power by 2019");
        }

        /// <summary>
        /// 获取所有控制器
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetControllers()
        {
            try
            {
                if (isShow)
                {
                    List<ControllerModel> controllerList = GetControllerModelList();
                    return Json(controllerList);
                }
                return Json("Sweeter@power by 2019");
            }
            catch (Exception ex)
            {
                return Json(string.Format("Error:{0}    --Sweeter@power by 2019", ex.Message));
            }
        }

        /// <summary>
        /// 获取控制器下所有方法
        /// </summary>
        /// <param name="ControllerName">控制器名称</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetMethods(string ControllerName)
        {
            try
            {
                if (isShow)
                {
                    List<MethodModel> methodList = GetMethodModelList(ControllerName);
                    return Json(methodList);
                }
                return Json("Sweeter@power by 2019");
            }
            catch (Exception ex)
            {
                return Json(string.Format("Error:{0}    --Sweeter@power by 2019", ex.Message));
            }
        }

        /// <summary>
        /// 获取方法详细通过方法名称
        /// </summary>
        /// <param name="ControllerName">控制器名称</param>
        /// <param name="MethodName">方法名称</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetMethodByName(string ControllerName, string MethodName)
        {
            try
            {
                if (isShow)
                {
                    MethodModel methodModel = GetMethodModelByName(ControllerName, MethodName);
                    return Json(methodModel);
                }
                return Json("Sweeter@power by 2019", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(string.Format("Error:{0}    --Sweeter@power by 2019", ex.Message));
            }
        }

        /// <summary>
        /// 导出API文档
        /// </summary>
        /// <param name="ControllerNames">控制器名称数组</param>
        /// <param name="IsAll">是否导出全部 默认否</param>
        public ActionResult ExportApiWord(string ControllerNames, bool IsAll = false)
        {
            List<ControllerModel> controllers = GetControllerModelList();
            if (!IsAll) controllers = controllers.Where(t => ControllerNames.Split(';').Contains(t.ControllerName)).ToList();

            #region 将数据转化为word格式

            var itemSettingList = new List<Sweeter.NpoiWordHelper.ItemSetting>();
            foreach (ControllerModel controller in controllers)
            {
                //todo:根据控制器获取方法并导出Word文档
                var methodList = GetMethodModelList(controller.ControllerName);

                var mainContentSettingList = new List<NpoiWordHelper.ContentItemSetting>();
                foreach (var method in methodList)
                {
                    var methodModel = this.GetMethodModelByName(controller.ControllerName, method.MethodName);
                    var _fontSize = 17;
                    mainContentSettingList.Add(new NpoiWordHelper.ContentItemSetting()
                    {
                        MainContent = "请求URL：" + method.MethodUrl,
                        FontSize = _fontSize,
                        HasBold = true
                    });
                    mainContentSettingList.Add(new NpoiWordHelper.ContentItemSetting()
                    {
                        MainContent = "方法名称：" + method.MethodName,
                        FontSize = _fontSize
                    });
                    mainContentSettingList.Add(new NpoiWordHelper.ContentItemSetting()
                    {
                        MainContent = "请求类型：" + method.MethodType,
                        FontSize = _fontSize
                    });
                    mainContentSettingList.Add(new NpoiWordHelper.ContentItemSetting()
                    {
                        MainContent = "接口说明：" + method.MethodSummary,
                        FontSize = _fontSize
                    });
                    mainContentSettingList.Add(new NpoiWordHelper.ContentItemSetting()
                    {
                        MainContent = "返回注释：" + method.ReturnRemark,
                        FontSize = _fontSize
                    });

                    mainContentSettingList.Add(new NpoiWordHelper.ContentItemSetting()
                    {
                        MainContent = "请求参数：",
                        FontSize = _fontSize,
                        HasBold = true
                    });
                    //循环获取请求参数
                    string[][] paramTableArray = new string[methodModel.ParamList.Count + 1][];
                    paramTableArray[0] = new string[] { "名称", "类型", "默认值", "说明" };

                    string[][] paramChildTableArray = null;
                    for (int i = 0; i < methodModel.ParamList.Count; i++)
                    {
                        var param = methodModel.ParamList[i];
                        paramTableArray[i + 1] = new string[] { param.ColumName, param.ColumType, param.DefaultValue, param.ColumSummary };
                        //如果是实体类，则获取实体类属性
                        if (param.ChildColumList != null && param.ChildColumList.Count > 0)
                        {
                            paramChildTableArray = new string[param.ChildColumList.Count+1][];
                            paramChildTableArray[0] = new string[] { "子名称", "类型", "默认值", "说明" };
                            for (int j = 0; j < param.ChildColumList.Count; j++)
                            {
                                var childParam = param.ChildColumList[j];
                                paramChildTableArray[j + 1] = new string[] { childParam.ColumName, childParam.ColumType, childParam.DefaultValue, childParam.ColumSummary };
                            }
                        }

                    }
                    //添加表格-字段参数
                    mainContentSettingList.Add(new NpoiWordHelper.ContentItemSetting()
                    {
                        MainContent = @"",
                        FontSize = _fontSize,
                        TableArray = paramTableArray
                    });
                    //添加表格-解析实体类参数
                    if (paramChildTableArray != null)
                    {
                        mainContentSettingList.Add(new NpoiWordHelper.ContentItemSetting()
                        {
                            MainContent = "\r\n",
                            FontSize = _fontSize,
                            TableArray = paramChildTableArray
                        });
                    }
                    mainContentSettingList.Add(new NpoiWordHelper.ContentItemSetting()
                      {
                          MainContent = "响应内容：",
                          FontSize = _fontSize,
                          HasBold = true
                      });
                    //添加表格-循环获取响应内容
                    string[][] returnTableArray = new string[methodModel.ReturnList.Count + 1][];
                    returnTableArray[0] = new string[] { "名称", "类型", "默认值", "说明" };
                    for (int i = 0; i < methodModel.ReturnList.Count; i++)
                    {
                        var param = methodModel.ReturnList[i];
                        returnTableArray[i + 1] = new string[] { param.ColumName, param.ColumType, param.DefaultValue, param.ColumSummary };
                    }

                    mainContentSettingList.Add(new NpoiWordHelper.ContentItemSetting()
                    {
                        MainContent = @"",
                        FontSize = _fontSize,
                        TableArray = returnTableArray
                    });
                    //添加回车
                    mainContentSettingList.Add(new NpoiWordHelper.ContentItemSetting()
                    {
                        MainContent = "\r\n",
                        FontSize = _fontSize
                    });

                }

                itemSettingList.Add(new NpoiWordHelper.ItemSetting()
                {
                    TitleSetting = new NpoiWordHelper.ContentItemSetting()
                    {
                        Title = controller.ControllerName+"（"+controller.ControllerSummary+"）"
                    },
                    MainContentSettingList = mainContentSettingList
                });
            }

            #endregion

            var documentSetting = new Sweeter.NpoiWordHelper.DocumentSetting()
            {
                ItemSettingList = itemSettingList
            };

            //将文件输出成二进制格式，并返回客户端
            var bytes = NpoiWordHelper.ExportDocument(documentSetting);
            return File(bytes, "application/vnd.ms-word", DateTime.Now.ToString("yyyyMMddHHmmss") + ".docx");
        }

        #region 私有方法
        /// <summary>
        /// 获取控制器模型列表
        /// </summary>
        /// <returns></returns>
        private List<ControllerModel> GetControllerModelList()
        {
            List<Member> memberList = LoadXMLHelper.LoadXML().Where(t => t.ControllerName != "" && (t.PreWord == 'M' || t.PreWord == 'T')).ToList();
            List<IGrouping<string, Member>> groupList = memberList.GroupBy(t => t.ControllerName).ToList();
            List<ControllerModel> controllerList = new List<ControllerModel>();
            foreach (IGrouping<string, Member> groups in groupList)
            {
                ControllerModel c_model = new ControllerModel();
                c_model.ControllerName = groups.FirstOrDefault().ControllerName;
                Member g_summary = memberList.Where(t => t.ControllerName == c_model.ControllerName && t.PreWord == 'T').FirstOrDefault();
                if (g_summary != null) c_model.ControllerSummary = g_summary.Summary.Value; else c_model.ControllerSummary = string.Empty;
                controllerList.Add(c_model);
            }
            return controllerList.OrderBy(t => t.ControllerName).ToList();
        }
        /// <summary>
        /// 获取方法模型列表 根据控制器名称
        /// </summary>
        /// <param name="ControllerName">控制器名称</param>
        /// <returns></returns>
        private List<MethodModel> GetMethodModelList(string ControllerName)
        {
            Assembly asmb = Assembly.LoadFrom(assemlyUrl);
            List<Member> memberList = LoadXMLHelper.LoadXML().Where(t => t.PreWord == 'M' && t.ControllerName == ControllerName).ToList();
            List<MethodModel> methodList = new List<MethodModel>();
            foreach (Member member in memberList)
            {
                MethodModel m_model = new MethodModel(member);
                //反射
                Type type = asmb.GetType(member.ControllerNamespace);
                MethodInfo methodInfo = null;
                try { methodInfo = type.GetMethod(member.MethodName); }
                catch { }
                if (methodInfo == null) continue;
                methodInfo.CustomAttributes.ToList().ForEach(ca =>
                {
                    switch (ca.AttributeType.Name)
                    {
                        case "HttpGetAttribute":
                            m_model.MethodType = "Get";
                            break;
                        case "HttpPostAttribute":
                            m_model.MethodType = "Post";
                            break;
                        case "HttpHeadAttribute":
                            m_model.MethodType = "Head";
                            break;
                        case "HttpOptionsAttribute":
                            m_model.MethodType = "Options";
                            break;
                        case "HttpPutAttribute":
                            m_model.MethodType = "Put";
                            break;
                        case "HttpDeleteAttribute":
                            m_model.MethodType = "Delete";
                            break;
                        case "HttpPatchAttribute":
                            m_model.MethodType = "Patch";
                            break;
                        default:
                            if (string.IsNullOrEmpty(m_model.MethodType))
                                m_model.MethodType = "Get";
                            break;
                    }
                });
                methodList.Add(m_model);
            }
            return methodList;
        }
        /// <summary>
        /// 获取详细方法模型 根据控制器名称和方法名称
        /// </summary>
        /// <param name="ControllerName">控制器名称</param>
        /// <param name="MethodName">方法名称</param>
        /// <returns></returns>
        private MethodModel GetMethodModelByName(string ControllerName, string MethodName)
        {
            Assembly asmb = Assembly.LoadFrom(assemlyUrl);
            List<Member> propertyMembers = LoadXMLHelper.LoadXML().Where(t => t.PreWord == 'P' || t.PreWord == 'F').ToList();
            Member member = LoadXMLHelper.LoadXML().Where(t => t.PreWord == 'M' && t.ControllerName == ControllerName && t.MethodName == MethodName).FirstOrDefault();
            MethodModel methodModel = new MethodModel();
            if (member == null) return methodModel;
            methodModel = new MethodModel(member);
            Type type = asmb.GetType(member.ControllerNamespace);
            MethodInfo methodInfo = type.GetMethod(member.MethodName);
            ParameterInfo[] paramInfoArr = methodInfo.GetParameters();
            foreach (ParameterInfo paramInfo in paramInfoArr)
            {
                JsonModle paramModel = new JsonModle();
                paramModel.ColumName = paramInfo.Name;
                Param param = member.ParamList.Where(t => t.Name == paramInfo.Name).FirstOrDefault();
                if (param != null) paramModel.ColumSummary = param.Value; else paramModel.ColumSummary = string.Empty;
                paramModel.ColumType = GetTypeName(paramInfo.ParameterType);
                paramModel.ChildColumList = GetChildColumList(paramInfo.ParameterType, propertyMembers, 1);
                paramModel.DefaultValue = paramInfo.DefaultValue.ToString();
                methodModel.ParamList.Add(paramModel);
            }
            //响应内容
            if (!string.IsNullOrWhiteSpace(member.Returns.AssemblyName) && !string.IsNullOrWhiteSpace(member.Returns.TypeFullName))
            {
                try
                {
                    string return_assemlyUrl = System.AppDomain.CurrentDomain.RelativeSearchPath + "/" + member.Returns.AssemblyName;
                    Assembly return_asmb = Assembly.LoadFrom(return_assemlyUrl);
                    Type return_type = return_asmb.GetType(member.Returns.TypeFullName);
                    methodModel.ReturnList = GetChildColumList(return_type, propertyMembers, 0);
                }
                catch { }
            }
            return methodModel;
        }
        /// <summary>
        /// 获取字段类型名称
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        private string GetTypeName(Type type)
        {
            string name = string.Empty;
            if (type.IsGenericType)
            {
                Type[] genericTypes = type.GetGenericArguments();
                foreach (Type t in genericTypes)
                {
                    name += "," + GetTypeName(t);
                }
                if (name.Length > 0) name = name.Remove(0, 1);
                name = string.Format("{0}<{1}>", type.Name.Substring(0, type.Name.IndexOf('`')), name);
            }
            else
            {
                name = type.Name;
            }
            return name;
        }
        /// <summary>
        /// 获取子属性列表
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="propertyMembers">类的(P:开头的)Xml文档内容</param>
        /// <param name="depth">模型深度</param>
        /// <returns></returns>
        private List<JsonModle> GetChildColumList(Type type, List<Member> propertyMembers, int depth)
        {
            List<JsonModle> result = new List<JsonModle>();
            if (depth > 5) return result;
            if (type.Namespace != "System")
            {
                if (type.IsArray)
                {
                    Type arrType = type.Assembly.GetType(type.FullName.Replace("[]", ""));
                    result = GetChildColumList(arrType, propertyMembers, depth++);
                }
                else if (type.IsGenericType && type.Namespace == "System.Collections.Generic")
                {
                    Type[] genericTypes = type.GetGenericArguments();
                    if (genericTypes.Length == 1)
                        result = GetChildColumList(genericTypes[0], propertyMembers, depth++);
                }
                else
                {
                    GetJsonModle(type, propertyMembers, depth, ref result);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取JsonModle
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="propertyMembers">属性注释内容</param>
        /// <param name="depth">模型深度</param>
        /// <returns></returns>
        private void GetJsonModle(Type type, List<Member> propertyMembers, int depth, ref List<JsonModle> list)
        {
            List<FieldInfo> fields = type.GetFields().ToList();
            List<PropertyInfo> properties = type.GetProperties().ToList();
            string memberName_P = string.Format("P:{0}.{1}.", type.Namespace, type.Name);
            string memberName_F = string.Format("F:{0}.{1}.", type.Namespace, type.Name);
            foreach (FieldInfo field in fields)
            {
                JsonModle oldModel = list.Where(t => t.ColumName == field.Name).FirstOrDefault();
                if (oldModel != null)
                {
                    list.Remove(oldModel);
                }
                JsonModle jsonModel = new JsonModle();
                jsonModel.ColumName = field.Name;
                Member member = propertyMembers.Where(t => t.Name == (memberName_F + field.Name)).FirstOrDefault();
                if (member != null) jsonModel.ColumSummary = member.Summary.Value; else jsonModel.ColumSummary = string.Empty;
                jsonModel.ColumType = GetTypeName(field.FieldType);
                jsonModel.ChildColumList = GetChildColumList(field.FieldType, propertyMembers, depth + 1);
                jsonModel.DefaultValue = string.Empty;
                list.Add(jsonModel);
            }
            foreach (PropertyInfo property in properties)
            {
                JsonModle oldModel = list.Where(t => t.ColumName == property.Name).FirstOrDefault();
                if (oldModel != null)
                {
                    list.Remove(oldModel);
                }
                JsonModle jsonModel = new JsonModle();
                jsonModel.ColumName = property.Name;
                Member member = propertyMembers.Where(t => t.Name == (memberName_P + property.Name)).FirstOrDefault();
                if (member != null) jsonModel.ColumSummary = member.Summary.Value; else jsonModel.ColumSummary = string.Empty;
                jsonModel.ColumType = GetTypeName(property.PropertyType);
                jsonModel.ChildColumList = GetChildColumList(property.PropertyType, propertyMembers, depth + 1);
                jsonModel.DefaultValue = string.Empty;
                list.Add(jsonModel);
            }
            if (type.BaseType.Namespace != "System")
            {
                GetJsonModle(type.BaseType, propertyMembers, depth, ref list);
            }
        }

        #endregion
    }
}
