using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Xml;
using Newtonsoft.Json;

namespace MenuTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var html = new XmlDocument();

            var menu = Menu.GetMenu(html);

            Console.WriteLine(html.OuterXml);

            Console.WriteLine(JsonConvert.SerializeObject(menu));

        }
    }

    class Menu
    {
        public int MenuId { get; set; }
        public int? ParentMenuId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public List<Menu> SubMenus { get; set; }
        public bool Visible { get; set; }

        public Menu()
        {
            this.SubMenus = new List<Menu>();
        }
        public Menu(DataRow dataRow)
        {
            this.SubMenus = new List<Menu>();
            this.MenuId = Convert.ToInt32(dataRow["MENU_ID"].ToString());
            this.Description = dataRow["DESCRIPTION"].ToString();
            this.Title = dataRow["TITLE"].ToString();
            this.Url = dataRow["URL"].ToString();
            this.Visible = Convert.ToBoolean(dataRow["VISIBLE"].ToString());

            var parentMenuId = dataRow["PARENT_MENU_ID"].ToString();
            if (String.IsNullOrEmpty(parentMenuId)) { this.ParentMenuId = null; }
            else { this.ParentMenuId = Int32.Parse(parentMenuId); }
        }

        public static List<Menu> GetMenu(XmlDocument html = null)
        {
            var menus = new List<Menu>();

            var dataTable = GetMenuItems();

            XmlElement ul = null;
            if (html != null) ul = html.CreateElement("ul");
            
            foreach (DataRow row in dataTable.Rows)
            {
                var menu = new Menu(row);

                XmlElement li = null;

                if (menu.ParentMenuId == null)
                {
                    if (ul != null)
                    {
                        li = html.CreateElement("li");
                        if (String.IsNullOrEmpty(menu.Url)) { li.InnerText = menu.Title; }
                        else { ApplyAnchor(menu, li, html); }
                    }

                    GetSubMenus(menu, dataTable, li, html);

                    menus.Add(menu);
                }

                if (li != null) ul.AppendChild(li);
                                
            }

            if (ul != null) html.AppendChild(ul);

            return menus;
        }

        public static void GetSubMenus(Menu menu, DataTable dataTable, XmlElement li = null, XmlDocument html = null)
        {

            var subMenus = dataTable
                           .AsEnumerable()
                           .Where(r => r.Field<int?>("PARENT_MENU_ID") != null &&
                                       r.Field<int?>("PARENT_MENU_ID") == menu.MenuId);
            

            XmlElement ul = null;
            if (li != null && html != null) ul = html.CreateElement("ul");

            foreach (DataRow row in subMenus)
            {
                var subMenu = new Menu(row);
                
                XmlElement subLi = null;
                if (ul != null)
                {
                    subLi = html.CreateElement("li");
                    if (String.IsNullOrEmpty(subMenu.Url))
                    {
                        subLi.InnerText = subMenu.Title;
                        subLi.SetAttribute("class", "subheading");
                    }
                    else
                    {
                        ApplyAnchor(subMenu, subLi, html);
                    }
                }

                GetSubMenus(subMenu, dataTable, subLi, html);

                menu.SubMenus.Add(subMenu);

                if (subLi != null)
                {

                    ul.AppendChild(subLi);
                    li.AppendChild(ul);
                }
            }

        }
        public static void ApplyAnchor(Menu menu, XmlElement li, XmlDocument html, bool hasChildren = false)
        {
            var anchor = html.CreateElement("a");

            anchor.SetAttribute("href", menu.Url);
           
            anchor.InnerText = menu.Title;
 
            li.AppendChild(anchor);
        }
        public static DataTable GetMenuItems()
        {
            var dataTable = new DataTable();

            using (var conn = new SqlConnection("Server=(local);Integrated Security=SSPI;Initial Catalog=Generic;"))
            {
                
                var cmd = new SqlCommand("GET_MENU", conn);

                conn.Open();

                var sda = new SqlDataAdapter(cmd);

                sda.Fill(dataTable);

                conn.Close();

                sda.Dispose();

            }

            return dataTable;

        }
    }
}
