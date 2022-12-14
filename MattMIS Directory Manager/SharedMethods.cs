using BrightIdeasSoftware;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MattMIS_Directory_Manager
{
    public static class SharedMethods
    {

        public static void EnableADAccount(DirectoryEntry user)
        {
            int val = (int)user.Properties["userAccountControl"].Value;
            user.Properties["userAccountControl"].Value = val & ~0x2;
            user.Properties["msExchHideFromAddressLists"].Value = null;
            //ADS_UF_NORMAL_ACCOUNT;
            user.CommitChanges();
        }

        public static PrincipalContext PC;
        public static void ChangePassword(DirectoryEntry de, string passwordToSet, bool changeAtLogin = false)
        {
            try
            {
                UserPrincipal pe = UserPrincipal.FindByIdentity(PC,Convert.ToString(de.Properties["distinguishedName"].Value ?? ""));
                pe.SetPassword(passwordToSet);
                
                if (changeAtLogin) { pe.ExpirePasswordNow(); }
                pe.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to change password: \n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void CreateRequiredFolders()
        {
            Directory.CreateDirectory("Reports");
            Directory.CreateDirectory("Configs");
            Directory.CreateDirectory("Exports");
        }

        //public static DirectoryEntry ResolveADGroup(string name)
        //{
        //    DirectoryEntry RootADObject = new DirectoryEntry();
        //    RootADObject.Path = "LDAP://" + Config.AppConfig.ActiveDirectory.UserGroupsRootOU;

        //    DirectorySearcher deSearch = new DirectorySearcher(RootADObject);

        //    deSearch.SearchScope = SearchScope.Subtree;
        //    deSearch.Filter = $"(&(objectCategory=group)(name={name}))";
        //    SearchResultCollection userResults = deSearch.FindAll();

        //    if(userResults.Count == 1) { return userResults[0].GetDirectoryEntry(); }
        //    else { return null; }
        //}

        //public static void CriticalError(string message)
        //{
        //    //FIRST TRY TO LOOKUP ERROR
        //    if(message == "STUDENTALLOCATIONEXPIRED")
        //    {
        //        studentLog.Fatal($"MattMIS is unable to continue as your student allocation file has expired. This is a failsafe to ensure new students are created correctly at the start of new academic years. Please update this file now - and modify the expiration date to resolve this error.\n\nFile location: {Config.AppConfig.AccountProvisioning.StudentSettingsPath}\nExpiration date:{ReportGenerator.YearGroupMappingExpiration}\n\n === APPLICATION WILL NOW EXIT ===");
        //        staffLog.Fatal($"MattMIS is unable to continue as your student allocation file has expired. This is a failsafe to ensure new students are created correctly at the start of new academic years. Please update this file now - and modify the expiration date to resolve this error.\n\nFile location: {Config.AppConfig.AccountProvisioning.StudentSettingsPath}\nExpiration date:{ReportGenerator.YearGroupMappingExpiration}\n\n === APPLICATION WILL NOW EXIT ===");
        //    }
        //}

        //public static void AddItemToGroup(string UPN, string groupName, string role)
        //{
        //    try
        //    {
        //        if (role == "staff") { staffLog.Info($"Group Adding {groupName}"); } else{ studentLog.Info($"Group Adding {groupName}"); }

        //        using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, Config.AppConfig.ActiveDirectory.ShortDomainName))
        //        {

        //            if (role == "staff") { staffLog.Info($"done pc"); } else { studentLog.Info($"done pc"); }
        //            GroupPrincipal group = GroupPrincipal.FindByIdentity(pc, groupName);
        //            if (role == "staff") { staffLog.Info($"found group"); } else { studentLog.Info($"found group"); }
        //            group.Members.Add(pc, IdentityType.UserPrincipalName, UPN);
        //            if (role == "staff") { staffLog.Info($"added user"); } else { studentLog.Info($"added user"); }
        //            group.Save();
        //            if (role == "staff") { staffLog.Info($"Finished Adding Group"); } else { studentLog.Info($"Finished Adding Group"); }

        //        }
        //    }
        //    catch (Exception ex)
        //    { 
        //        if (role == "staff") { staffLog.Info($"ERROR ADDING GROUP: {groupName}. ERROR IS: {ex.Message}"); } else { studentLog.Info($"ERROR ADDING GROUP: {groupName}. ERROR IS: {ex.Message}"); }
        //    }
        //}

        public static void DisableADAccount(DirectoryEntry user)
        {
            int val = (int)user.Properties["userAccountControl"].Value;
            user.Properties["userAccountControl"].Value = val | 0x2;
            user.Properties["msExchHideFromAddressLists"].Value = true;

            //ADS_UF_ACCOUNTDISABLE;
            user.CommitChanges();

        }

        public static class ColorModes
        {
            public static bool darkMode = false;
            public static Color foreColor = SystemColors.WindowText;
            public static Color backColor = SystemColors.Control;
            public static Color controlColor = SystemColors.Window;
            public static Color itemHoverColor = Color.FromKnownColor(KnownColor.GradientInactiveCaption);
            public static Color itemSelectedColor = Color.FromKnownColor(KnownColor.GradientActiveCaption);
            public static FlatStyle themeFlatStyle = FlatStyle.Flat;
            public static HeaderFormatStyle headerFormatStyle = null;
        }


        public static bool IsADAccountDisabled(DirectoryEntry de)
        {
            const int UF_ACCOUNTDISABLE = 0x0002;

            int flags = (int)de.Properties["userAccountControl"][0];

            if (Convert.ToBoolean(flags & UF_ACCOUNTDISABLE))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
       
        public static string StripSpecialChars(string input)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            return rgx.Replace(input, "");
        }
    }
}

