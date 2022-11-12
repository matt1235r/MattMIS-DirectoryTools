using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Drawing;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MattMIS_Directory_Manager
{
    public static class Handler
    {

        private static List<UserModel> Objects = new List<UserModel>();

        private static List<TreeModel> TreeObjects = new List<TreeModel>();


        public class UserModel
        {
            public string ID { get; set; }
            public string FullName { get; set; }

            public string Department { get; set; }

            public string Status { get; set; }

            public string Username { get; set; }

            public string ImageKey { get; set; }

            public string ObjectType { get; set; }
            public string Extras { get; set; }

            public DirectoryEntry directoryEntry { get; set; }

            public UserModel(string ID, string FullName, string Department, string Username, string Status, string ImageKey, string objectType, DirectoryEntry directoryEntry, string extras = "")
            {
                this.ID = ID;
                this.FullName = FullName;
                this.Department = Department;
                this.Status = Status;
                this.Username = Username;
                this.ImageKey = ImageKey;
                this.directoryEntry = directoryEntry;
                ObjectType = objectType;
                Extras = extras;
            }
        }



        private static bool IsActive(DirectoryEntry de)
        {
            if (de.NativeGuid == null) return false;

            int flags = (int)de.Properties["userAccountControl"].Value;

            return !Convert.ToBoolean(flags & 0x0002);
        }

        public static void AddItem(DirectoryEntry dirObject)
        {
            string objectComment = Convert.ToString(dirObject.Properties["comment"].Value ?? "");
            string imageKey = "";

            string objectType = dirObject.SchemaEntry.Name;

            if (objectType == "user")
            {
                bool isActive = IsActive(dirObject);
                if (objectComment.StartsWith("CHECKED### by MattMIS") && isActive)
                {
                    objectComment = objectComment.Split(new[] { "###" }, StringSplitOptions.None)[2];
                    objectComment = $"Healthy: ({objectComment.Split(' ')[0]})";
                    imageKey = "user_enabled.ico";
                }
                else if (objectComment.StartsWith("DISABLED### by MattMIS") && !isActive)
                {
                    objectComment = objectComment.Split(new[] { "###" }, StringSplitOptions.None)[2];
                    objectComment = "Deprovisioned";
                    // betterListViewItem.ForeColor = Color.Red;
                    imageKey = "user_disabled.ico";


                }
                else if (!isActive)
                {
                    objectComment = $"Manually Disabled";
                    // betterListViewItem.ForeColor = Color.Red;
                    imageKey = "user_disabled.ico";
                }
                else
                {
                    objectComment = $"Not Matched";
                    // betterListViewItem.ForeColor = Color.Blue;
                    imageKey = "user_normal.ico";
                }
            }
            else if (objectType == "computer")
            {
                bool isActive = IsActive(dirObject);
                bool isServer = Convert.ToString(dirObject.Properties["operatingSystem"].Value ?? "").ToLower().Contains("server");
                if (isActive && !isServer)
                {
                    objectComment = $"Domain Computer";
                    // betterListViewItem.ForeColor = Color.Blue;
                    imageKey = "computer_enabled.png";
                }
                else if (isActive && isServer)
                {
                    objectComment = $"Domain Server";
                    // betterListViewItem.ForeColor = Color.Blue;
                    imageKey = "server.ico";
                }
                else
                {
                    objectComment = $"Domain Computer (Disabled)";
                    // betterListViewItem.ForeColor = Color.Blue;
                    imageKey = "computer_disabled.png";
                }

            }


            if (objectType == "user") Objects.Add(new UserModel($"{dirObject.Properties["physicalDeliveryOfficeName"].Value ?? ""}", $"{dirObject.Properties["cn"].Value ?? ""}", $"{dirObject.Properties["department"].Value ?? ""}", $"{dirObject.Properties["userPrincipalName"].Value}", objectComment, imageKey, $"{objectType}", dirObject));
            else if (objectType == "computer") { Objects.Add(new UserModel("", $"{dirObject.Properties["cn"].Value ?? ""}", $"{dirObject.Properties["operatingSystem"].Value ?? ""}", $"{dirObject.Properties["dNSHostName"].Value ?? ""}", objectComment, imageKey, $"{objectType}", dirObject)); }
            else if (objectType == "organizationalUnit") { Objects.Add(new UserModel("", $"{dirObject.Properties["name"].Value ?? ""}", "Organizational Unit", "", "", "OU.png", $"{objectType}", dirObject, dirObject.Path)); }
            else if (objectType == "group") { Objects.Add(new UserModel("", $"{dirObject.Properties["name"].Value ?? ""}", "Security Group", "", "", "group.ico", $"{objectType}", dirObject, dirObject.Path)); }
            else if (objectType == "volume") { Objects.Add(new UserModel("", $"{dirObject.Properties["name"].Value ?? ""}", "Shared Folder", "", "", "netShare.ico", $"{objectType}", dirObject, dirObject.Path)); }
            else if (objectType == "printQueue") { Objects.Add(new UserModel("", $"{dirObject.Properties["name"].Value ?? ""}", "Network Printer", "", "", "printer.ico", $"{objectType}", dirObject, dirObject.Path)); }
            else { System.Windows.Forms.MessageBox.Show(objectType); }
        }

        public static List<UserModel> GetObjects()
        {
            return Objects;
        }

        public static void Clear()
        {
            Objects.Clear();
        }

        public static object ObjImageGetter(object rowObject)
        {
            return ((UserModel)rowObject).ImageKey ?? "";
        }

        public static object TreeImageGetter(object rowObject)
        {
            return ((TreeModel)rowObject).ImageKey;

        }

        public class TreeModel
        {
            public List<TreeModel> Children { get; set; }
            public string Name { get; set; }

            public string Command { get; set; }

            public string Argument { get; set; }

            public string ImageKey { get; set; }

            public int ChildCount { get; }

            public bool shouldExpand { get; set; }

            public DirectoryEntry directoryEntry { get; set; }

            public TreeModel()
            {
                this.Children = new List<TreeModel>();
            }
        }

        public static List<TreeModel> GetTreeObjects()
        {
            return TreeObjects;
        }

        public static void ClearTree()
        {
            TreeObjects.Clear();
        }

        public static void AddTree(TreeModel t)
        {
            TreeObjects.Add(t);
        }
    }
}

