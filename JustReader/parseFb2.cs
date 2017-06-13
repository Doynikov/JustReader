using System.Text;
using System.Windows;
using System.Xml;

namespace JustReader
{
    class parseFb2
    {
        XmlElement xRoot;
        XmlDocument xDoc;
        XmlNodeList nodeList;
        XmlNode node;
        XmlNode nodetemp;
        XmlNode nodeInfo;
        XmlAttribute attr;
        XmlNamespaceManager ns;

        public string sTitle = "Без названия";
        public string sAuthor = "Автор неизвестен";
        public bool validFile = false;
        
        public parseFb2(string fn, int zip, string sxml)
        {
            NameTable nt = new NameTable();
            ns = new XmlNamespaceManager(nt);
            ns.AddNamespace("fb", "http://www.gribuser.ru/xml/fictionbook/2.0");
            ns.AddNamespace("l", "http://www.w3.org/1999/xlink");
            ns.AddNamespace("NS1", "http://www.w3.org/1999/xlink");
            ns.AddNamespace("xlink", "http://www.w3.org/1999/xlink");
            xDoc = new XmlDocument();

            try
            {
                if (zip == 0)
                {
                    xDoc.Load(fn);
                }
                else
                {
                    xDoc.LoadXml(sxml);
                }
            }
            catch
            {
                MessageBox.Show("Не получается открыть fb файл. Возможно не валидный формат");
                return;
            }
            validFile = true;
            xRoot = xDoc.DocumentElement;
            nodeInfo = xRoot.SelectSingleNode("//fb:description/fb:title-info", ns);
        }
        public string getTitle()
        {
            string s = "";
            node = nodeInfo.SelectSingleNode("fb:book-title", ns);
            
            if (node != null)
            {
                sTitle = node.InnerText;
                s = book_title_start + sTitle + book_title_end;
            }
            return s;
        }
        public string getSequence()
        {
            string s = "";
            node = nodeInfo.SelectSingleNode("fb:sequence", ns);
            if (node != null)
            {
                attr = node.Attributes["name"];
                if (attr != null) s = attr.Value;
                attr = node.Attributes["number"];
                if (attr != null) s += " № " + attr.Value;
            }
            return s;
        }
        public string getDate()
        {
            string s = "";
            node = nodeInfo.SelectSingleNode("fb:date", ns);
            if (node != null)
            {
                s = node.InnerText;
            }
            return s;
        }
        public string getSequenceLine()
        {
            string s = "";
            s = getSequence() + " " + getDate();
            if (s != " ") s=book_sequence_line_start + s + book_sequence_line_end;
            return s;
        }
        public string getAuthor()
        {
            string s = "";
            nodeList = nodeInfo.SelectNodes("fb:author", ns);
            foreach (XmlNode nod in nodeList)
            {
                if (s != "") s += ", ";
                nodetemp = nod.SelectSingleNode("fb:first-name", ns);
                if (nodetemp != null)
                {
                    s += nodetemp.InnerText + " ";
                }
                nodetemp = nod.SelectSingleNode("fb:last-name", ns);
                if (nodetemp != null)
                {
                    s += nodetemp.InnerText;
                }
            }
            if (s != "")
            {
                sAuthor = s;
                s=book_author_start + s + book_author_end;
            }
            return s;
        }
        public string getTitleLine()
        { 
            if(sAuthor!="") sTitle += " - " + sAuthor;
            return sTitle;
        }
        public string getAnnotation()
        {
            string s = "";
            node = nodeInfo.SelectSingleNode("fb:annotation", ns);
            if (node != null)
            {
                s=book_annotation_start + node.InnerText + book_annotation_end;
            }
            return s;
        }
        public string getCover()
        {
            string s = "";
            string simg = "";
            node = nodeInfo.SelectSingleNode("fb:coverpage/fb:image", ns);
            if (node != null)
            {
                foreach (XmlAttribute at in node.Attributes)
                {
                    if (at.Name.IndexOf("href") > 0)
                    {
                        s = at.Value;
                        break;
                    }
                }
                if (s != "")
                {
                    s = s.Replace("#", "");
                    nodetemp = xRoot.SelectSingleNode("//fb:binary[@id='" + s + "']", ns);
                    if (nodetemp != null)
                    {
                        simg = book_cover_start + nodetemp.InnerText + book_cover_end;
                    }
                }
            }
            return simg;
        }

        public string getBody()
        {
            string s = "";
            StringBuilder sb = new StringBuilder();
            node = xRoot.SelectSingleNode("//fb:body", ns);
            if (node != null)
            {
                nodeList = node.ChildNodes;
                foreach (XmlNode nod in nodeList)
                {
                    switch (nod.Name)
                    {
                        case "section":
                            sidToHTML(xRoot, nod, ns, sb);
                            break;
                        case "title":
                            sb.Append(section_title_start + nod.InnerXml + section_title_end);
                            break;
                        case "epigraph":
                            sb.Append(section_epigraph_start + nod.InnerText + section_epigraph_end);
                            break;
                        case "image":
                            s = "";
                            foreach (XmlAttribute at in nod.Attributes)
                            {
                                if (at.Name.IndexOf("href") > 0)
                                {
                                    s = at.Value;
                                    break;
                                }
                            }
                            if (s != "")
                            {
                                s = s.Replace("#", "");
                                nodetemp = xRoot.SelectSingleNode("//fb:binary[@id='" + s + "']", ns);
                                if (nodetemp != null)
                                {
                                    sb.Append(section_image_start + nodetemp.InnerText + section_image_end);
                                }
                            }
                            break;

                    }
                }
            }


            return sb.ToString().Replace("http://www.gribuser.ru/xml/fictionbook/2.0", "").Replace("emphasis>", "i>");
        }

        private void sidToHTML(XmlElement xRoot, XmlNode node, XmlNamespaceManager ns, StringBuilder sb)
        {
            XmlNodeList nodeList = node.ChildNodes;
            XmlNode nodetemp;
            string s = "";

            int n = 0;
            foreach (XmlNode nod in nodeList)
            {
                if (nod.NodeType.ToString() == "Element")
                {
                    int i = nod.ChildNodes.Count;
                    switch (nod.Name)
                    {
                        case "section":
                            sidToHTML(xRoot, nod, ns, sb);
                            break;
                        case "p":
                            if (i > 1)
                            {
                                sb.Append("<p>" + nod.InnerXml + "</p>");
                            }
                            else
                            {
                                sb.Append("<p>" + nod.InnerText + "</p>");
                            }
                            break;
                        case "image":
                            s = "";
                            foreach (XmlAttribute at in nod.Attributes)
                            {
                                if (at.Name.IndexOf("href") > 0)
                                {
                                    s = at.Value;
                                    break;
                                }
                            }
                            if (s != "")
                            {
                                s = s.Replace("#", "");
                                nodetemp = xRoot.SelectSingleNode("//fb:binary[@id='" + s + "']", ns);
                                if (nodetemp != null)
                                {
                                    sb.Append(section_image_start + nodetemp.InnerText + section_image_end);
                                }
                            }
                            break;
                        case "title":
                            if (i > 1)
                            {
                                sb.Append(section_subtitle_start + nod.InnerXml + section_subtitle_end);
                            }
                            else
                            {
                                sb.Append(section_subtitle_start + nod.InnerText + section_subtitle_end);
                            }
                            break;
                        case "poem":
                            sb.Append(poem_start + nod.InnerXml.Replace("v>", "p>") + poem_end);
                            break;
                        case "subtitle":
                        case "epigraph":
                        case "annotation":
                            sb.Append(section_annotation_start + nod.InnerText + section_annotation_end);
                            break;
                        case "table":
                            sb.Append(node.OuterXml);
                            break;
                        case "empty - line":
                            sb.Append("<br />");
                            break;
                    }
                }
            }
        }

        #region Private Const

        string book_title_start = "<p fontsize='48.0' style='text-align:center;'>";
        string book_title_end = "</p>";
        string book_sequence_line_start = "<p fontsize='19.0' style='text-align:center' align='center'>";
        string book_sequence_line_end = "</p>";
        string book_author_start = "<p fontsize='36.0' style='text-align:center;'>";
        string book_author_end = "</p>";
        string book_annotation_start = "<p fontsize = '21.0' style='font-style:italic;'>";
        string book_annotation_end = "</p>";
        string book_cover_start = "<p style='text-align:center'><img bin='";
        string book_cover_end = "' /></p>";

        string section_title_start = "<div fontsize='36.0' style='text-align:center'>";
        string section_title_end = "</div>";
        string section_epigraph_start = "<div fontsize = '19.0' style='text-align:right'>";
        string section_epigraph_end = "</div>";
        string section_subtitle_start = "<div fontsize='36.0' style='text-align:center'>";
        string section_subtitle_end = "</div>";
        string section_annotation_start = "<p fontsize='19.0' style='text-align:center'>";
        string section_annotation_end = "</p>";
        string section_image_start = "< p style='text-align:center'><img bin = '";
        string section_image_end = "' /></p>";
        string poem_start = "<p fontsize='24.0' style='text-align:left'>";
        string poem_end = "</p>";
        

        #endregion Private Const
    }
}
