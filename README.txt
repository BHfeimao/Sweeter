Sweeter  -- .NET MVC文档生成工具

版本选择：
根据项目中System.Web.Mvc.dll的版本(5.*.*)对应选择SweeterForMVC5.*.*.dll

使用步骤：
1、将Sweeter.dll引用到目标MVC项目中；
2、目标MVC项目web.config中<appSettings>节点下增加配置项：
	<add key="Sweeter_isShow" value="true" />  -- 表示Sweeter生成的接口文档是否可见
	<add key="Sweeter_xmlNames" value="***.XML,***.XML,..."/>  -- 表示用于生成文档时需要加载的XML文件
    <add key="Sweeter_dllName" value="***.dll"/> -- 表示用于生成文档WEB项目程序集名称
	（其中***.XML和***.dll分别代表目标MVC项目输出XML文件名称和生成dll程序集名称)
3、将Sweeter文件夹整体放到目标MVC项目根目录下；
4、<returns>标签填写规范（非必须项，按照规范填写文档方可显示“响应内容”，否则无）
	例如：
	/// <summary>
    /// 接口名称
    /// </summary>
    /// <param name="id">id参数</param>
	/// <returns>
    /// Gdky.Common.dll;
    /// Gdky.Common.ExecutionResult`1[[System.Collections.Generic.List`1[[Gdky.Entities.Bas_Crew, 
    /// Gdky.Entities, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], mscorlib, Version=4.0.0.0, 
    /// Culture=neutral, PublicKeyToken=b77a5c561934e089]];
    /// 返回注释说明
    /// </returns>

	<returns>标签中，
	第一部分（即第一行）需填写返回类型所在程序集文件(Gdky.Common.dll;)冒号结尾，此值可通过 Type.Assembly.ManifestModule.Name 属性获取。
	第二部分（即第二、三、四行）为返回类型的FullName值，此值可通过 Type.FullName 属性获取。
	第三部分（即第五行之后）为返回注释说明文字，无限制，可空。


注意：
a、目标MVC项目中所有Controller类中至少需要含有一项注释（可加载至XML文档中）；
b、目标MVC项目中所有方法(Method)必须含有注释（可加载至XML文档中）；
c、方法名称（MethodName）在同一个Controller类中必须保证唯一；