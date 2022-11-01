using System;
using System.Collections.Generic;
using System.Linq;
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

            public UserModel(string ID, string FullName, string Department, string Status, string Username, string SID)
            {
                this.ID = ID;
                this.FullName = FullName;
                this.Department = Department;
                this.Status = Status;
                this.Username = Username;
                this.SID = SID;
            }
        }

        public static List<UserModel> Users = new List<UserModel>();
        
        public static List<UserModel> GetUsers()
        {
            
            return Users;
        }
    }

}

