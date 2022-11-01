using ComponentOwl.BetterListView;
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
    public static class UserHandler
    {
        public class UserModel
        {
            public string ID { get; set; }
            public string FullName { get; set; }

            public string Department { get; set; }

            public string Status { get; set; }

            public string Username { get; set; }

            public string SID { get; set; }
            public string ImageKey { get; set; }

            public UserModel(string ID, string FullName, string Department, string Username, string Status , string SID, string ImageKey)
            {
                this.ID = ID;
                this.FullName = FullName;
                this.Department = Department;
                this.Status = Status;
                this.Username = Username;
                this.SID = SID;
                this.ImageKey = ImageKey;
            }
        }

        private static List<UserModel> Users = new List<UserModel>();

        private static bool IsActive(DirectoryEntry de)
        {
            if (de.NativeGuid == null) return false;

            int flags = (int)de.Properties["userAccountControl"].Value;

            return !Convert.ToBoolean(flags & 0x0002);
        }

        public static void AddUser(DirectoryEntry userObject)
        {
            var sidInBytes = (byte[])userObject.Properties["objectSid"].Value;
            var sid = new SecurityIdentifier(sidInBytes, 0);
            string userComment = Convert.ToString(userObject.Properties["comment"].Value ?? "");
            string imageKey = "";
            bool userActive = IsActive(userObject);
            
            if (userComment.StartsWith("CHECKED### by MattMIS") && userActive)
            {
                userComment = userComment.Split(new[] { "###" }, StringSplitOptions.None)[2];
                userComment = $"Healthy: ({userComment.Split(' ')[0]})";
                imageKey = "enabled.png";
            }
            else if (userComment.StartsWith("DISABLED### by MattMIS") && !userActive)
            {
                userComment = userComment.Split(new[] { "###" }, StringSplitOptions.None)[2];
                userComment = "Deprovisioned";
                // betterListViewItem.ForeColor = Color.Red;
                imageKey = "disabled.png";

            }
            else if (userActive)
            {
                userComment = $"Not Matched";
                // betterListViewItem.ForeColor = Color.Blue;
                imageKey = "unlinked.png";
            }
            else if (!userActive)
            {
                userComment = $"Manually Disabled";
                // betterListViewItem.ForeColor = Color.Red;
                imageKey = "disabled.png";
            }

            Users.Add(new UserModel($"{userObject.Properties["physicalDeliveryOfficeName"].Value ?? ""}", $"{userObject.Properties["cn"].Value ?? ""}", $"{userObject.Properties["department"].Value ?? ""}", $"{userObject.Properties["userPrincipalName"].Value}", userComment, sid.ToString(), imageKey));

        }

        public static List<UserModel> GetUsers()
        {      
            return Users;
        }

        public static void Clear()
        {
            Users.Clear();
        }

        public static object UserImageGetter(object rowObject)
        {
             return ((UserModel)rowObject).ImageKey;
            
        }
    }

}

