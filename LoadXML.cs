using Sweeter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Sweeter
{
    public class LoadXMLHelper
    {
        private static string path = System.AppDomain.CurrentDomain.RelativeSearchPath + "/";

        public static List<Member> LoadXML()
        {
            List<Member> result = new List<Member>();
            string[] xmlNames = System.Configuration.ConfigurationSettings.AppSettings["Sweeter_xmlNames"].ToString().Split(',');
            foreach (string xmlName in xmlNames)
            {
                result.AddRange(load(path + xmlName));
            }
            return result;
        }

        private static List<Member> load(string xmlUrl)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlUrl);
            XmlElement root = null;
            root = doc.DocumentElement;
            string msg = string.Empty;
            XmlNodeList listNodes = null;
            listNodes = root.SelectNodes("//member");
            List<Member> meberList = new List<Member>();
            foreach (XmlNode node in listNodes)
            {
                Member member = new Member();
                member.Name = node.Attributes["name"].Value;
                Summary summary = new Summary();
                XmlNode node_s = node.SelectSingleNode("summary");
                if(node_s!=null)
                    summary.Value = node_s.InnerText.Trim();
                member.Summary = summary;
                XmlNodeList node_ps = node.SelectNodes("param");
                List<Param> paramList = new List<Param>();
                foreach (XmlNode node_p in node_ps)
                {
                    Param param = new Param();
                    param.Name = node_p.Attributes["name"].Value;
                    param.Value = node_p.InnerText.Trim();
                    paramList.Add(param);
                }
                member.ParamList = paramList;
                XmlNode node_r = node.SelectSingleNode("returns");
                Returns returns = new Returns();
                if (node_r != null)
                {
                    returns.Value = node_r.InnerText.Trim();
                }
                member.Returns = returns;
                meberList.Add(member);
            }
            int count = meberList.Count;
            return meberList;
        }
    }
}
