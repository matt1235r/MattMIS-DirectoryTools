using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MattMIS_Sync
{
    public static class SharedMethods
    {
        private static readonly log4net.ILog staffLog = log4net.LogManager.GetLogger("StaffRollingLogFileAppender");
        private static readonly log4net.ILog studentLog = log4net.LogManager.GetLogger("StudentRollingLogFileAppender");

        public static void EnableADAccount(DirectoryEntry user)
        {
            int val = (int)user.Properties["userAccountControl"].Value;
            user.Properties["userAccountControl"].Value = val & ~0x2;
            user.Properties["msExchHideFromAddressLists"].Value = null;
            //ADS_UF_NORMAL_ACCOUNT;
            user.CommitChanges();
        }

        public static void CreateRequiredFolders()
        {
            Directory.CreateDirectory("Reports");
            Directory.CreateDirectory("Configs");
            Directory.CreateDirectory("Exports");
        }

        public static DirectoryEntry ResolveADGroup(string name)
        {
            DirectoryEntry RootADObject = new DirectoryEntry();
            RootADObject.Path = "LDAP://" + Config.AppConfig.ActiveDirectory.UserGroupsRootOU;

            DirectorySearcher deSearch = new DirectorySearcher(RootADObject);

            deSearch.SearchScope = SearchScope.Subtree;
            deSearch.Filter = $"(&(objectCategory=group)(name={name}))";
            SearchResultCollection userResults = deSearch.FindAll();

            if(userResults.Count == 1) { return userResults[0].GetDirectoryEntry(); }
            else { return null; }
        }

        public static void CriticalError(string message)
        {
            //FIRST TRY TO LOOKUP ERROR
            if(message == "STUDENTALLOCATIONEXPIRED")
            {
                studentLog.Fatal($"MattMIS is unable to continue as your student allocation file has expired. This is a failsafe to ensure new students are created correctly at the start of new academic years. Please update this file now - and modify the expiration date to resolve this error.\n\nFile location: {Config.AppConfig.AccountProvisioning.StudentSettingsPath}\nExpiration date:{ReportGenerator.YearGroupMappingExpiration}\n\n === APPLICATION WILL NOW EXIT ===");
                staffLog.Fatal($"MattMIS is unable to continue as your student allocation file has expired. This is a failsafe to ensure new students are created correctly at the start of new academic years. Please update this file now - and modify the expiration date to resolve this error.\n\nFile location: {Config.AppConfig.AccountProvisioning.StudentSettingsPath}\nExpiration date:{ReportGenerator.YearGroupMappingExpiration}\n\n === APPLICATION WILL NOW EXIT ===");
            }
        }

        public static void AddUserToGroup(string UPN, string groupName, string role)
        {
            try
            {
                if (role == "staff") { staffLog.Info($"Group Adding {groupName}"); } else{ studentLog.Info($"Group Adding {groupName}"); }

                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, Config.AppConfig.ActiveDirectory.ShortDomainName))
                {

                    if (role == "staff") { staffLog.Info($"done pc"); } else { studentLog.Info($"done pc"); }
                    GroupPrincipal group = GroupPrincipal.FindByIdentity(pc, groupName);
                    if (role == "staff") { staffLog.Info($"found group"); } else { studentLog.Info($"found group"); }
                    group.Members.Add(pc, IdentityType.UserPrincipalName, UPN);
                    if (role == "staff") { staffLog.Info($"added user"); } else { studentLog.Info($"added user"); }
                    group.Save();
                    if (role == "staff") { staffLog.Info($"Finished Adding Group"); } else { studentLog.Info($"Finished Adding Group"); }

                }
            }
            catch (Exception ex)
            { 
                if (role == "staff") { staffLog.Info($"ERROR ADDING GROUP: {groupName}. ERROR IS: {ex.Message}"); } else { studentLog.Info($"ERROR ADDING GROUP: {groupName}. ERROR IS: {ex.Message}"); }
            }
        }

        public static void DisableADAccount(DirectoryEntry user)
        {
            int val = (int)user.Properties["userAccountControl"].Value;
            user.Properties["userAccountControl"].Value = val | 0x2;
            user.Properties["msExchHideFromAddressLists"].Value = true;

            //ADS_UF_ACCOUNTDISABLE;
            user.CommitChanges();

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

        public static void CheckSurplusADAccounts(string role)
        {

            //START LOOPING THROUGH STAFF
            DirectoryEntry staffOUObject = new DirectoryEntry();
            if (role.ToLower() == "staff")
            {
                staffOUObject.Path = $"LDAP://{Config.AppConfig.ActiveDirectory.StaffRootOU}";
                //TIME TO SEE WHICH AD ACCOUNTS CAN BE DISABLED
                staffLog.Info($"\n\n====================== CHECKING STAFF ACCOUNTS ELIGBLE TO BE DISABLED ======================\n");
            }
            else
            {
                staffOUObject.Path = $"LDAP://{Config.AppConfig.ActiveDirectory.StudentRootOU}";
                //TIME TO SEE WHICH AD ACCOUNTS CAN BE DISABLED
                studentLog.Info($"\n\n====================== CHECKING STUDENT ACCOUNTS ELIGBLE TO BE DISABLED ======================\n");
            }


            //SETUP AD SEARCHER TO FIND STAFF ELIGBLE FOR DISABLING
            DirectorySearcher deSearch = new DirectorySearcher(staffOUObject);
            deSearch.PageSize = 1000;
            deSearch.SearchScope = SearchScope.Subtree;
            deSearch.Filter = $"(&(objectCategory=person)(objectClass=User)(physicalDeliveryOfficeName=*)(userAccountControl=*)(!(userAccountControl:1.2.840.113556.1.4.803:=2)))";
            if (role == "student") { studentLog.Info($"COLLECTING ALL ACCOUNTS......."); } else if (role == "staff") { staffLog.Info($"COLLECTING ALL ACCOUNTS......."); }

            SearchResultCollection allStaffAccounts = deSearch.FindAll();
            if (role == "student") { studentLog.Info($"FOUND {allStaffAccounts.Count} ACCOUNTS\n----------------------------------------------------------------------------------------------------------"); } else if (role == "staff") { staffLog.Info($"FOUND {allStaffAccounts.Count} ACCOUNTS\n----------------------------------------------------------------------------------------------------------"); }

            //GO THROUGH ALL STAFF RESULTS
            foreach (SearchResult result in allStaffAccounts)
            {
                DirectoryEntry StaffAccount = result.GetDirectoryEntry();                

                if (role == "staff")
                {
                    //IF LDAP ACCOUNT ISNT IN KNOWN STAFF UPN LIST AND 
                    if (!Program.KnownStaffUPNList.Contains(StaffAccount.Properties["userPrincipalName"].Value))
                    {
                        string lastLogin = "UNKNOWN";
                        try { lastLogin = DateTime.FromFileTime((long)result.Properties["lastLogonTimestamp"][0]).ToString("dd/MM/yyy HH:mm:ss"); } catch (Exception) { }

                        staffLog.Info($"{StaffAccount.Properties["userPrincipalName"].Value} is no longer matched to an individual and will be evaluated for deprovison. Last Login Time: {lastLogin}");
                        CheckAccountDeprovision(StaffAccount, role);
                    }
                }
                else if (role == "student")
                {
                    //IF LDAP ACCOUNT ISNT IN KNOWN STUDENT UPN LIST
                    //BUT ALSO CHECK ACCOUNTS HAVENT BEEN MADE IN ADVANCE FOR NEW YEAR 7 OR NEW YEAR 12! SO CHECK CURRENT YEAR AND YEAR 12 - CURRENT YEAR - 5
                    if (!Program.KnownStudentUPNList.Contains(StaffAccount.Properties["userPrincipalName"].Value))
                    {
                        string lastLogin = "UNKNOWN";
                        try { lastLogin = DateTime.FromFileTime((long)result.Properties["lastLogonTimestamp"][0]).ToString("dd/MM/yyy HH:mm:ss"); } catch (Exception) { }

                        studentLog.Info($"{StaffAccount.Properties["userPrincipalName"].Value} is no longer matched to an individual and will be evaluated for deprovison. Last Login Time: {lastLogin}");
                        CheckAccountDeprovision(StaffAccount, role);
                    }
                }
            }
        }

        public static void CheckAccountDeprovision(DirectoryEntry StaffADObject, string role)
        {
            //check whether account has been set to be ignored or set to keep
            if (((Convert.ToString(StaffADObject.Properties["facsimileTelephoneNumber"].Value) ?? "") == "SKIP") || (Convert.ToString(StaffADObject.Properties["facsimileTelephoneNumber"].Value) ?? "") == "KEEP" || Program.KeepEnabledADAccounts.Contains(StaffADObject.Properties["userPrincipalName"].Value.ToString().ToLower()))
            {

                if (role == "student") {  ReportGenerator.studentAccountsDeprovisionExcludedCount++; studentLog.Info($"{StaffADObject.Properties["userPrincipalName"].Value} is not eligble for deprovision as the UPN has been whitelisted."); } else if (role == "staff") {  ReportGenerator.staffAccountsDeprovisionExcludedCount++; staffLog.Info($"{StaffADObject.Properties["userPrincipalName"].Value} is not eligble for deprovision as the UPN has been whitelisted."); }

            }
            //CHECK USERAME DOES NOT END WITH EXLUSION
            else if (Program.KeepEnabledADAccountEndings.Any(x => StaffADObject.Properties["sAMAccountName"].Value.ToString().EndsWith(x)))
            {
                if (role == "student") { ReportGenerator.studentAccountsDeprovisionExcludedCount++; studentLog.Info($"{StaffADObject.Properties["userPrincipalName"].Value} is not eligble for deprovision as the username ends with a whitelisted value"); } else if (role == "staff") { ReportGenerator.staffAccountsDeprovisionExcludedCount++; staffLog.Info($"{StaffADObject.Properties["userPrincipalName"].Value} is not eligble for deprovision as the username ends with a whitelisted value"); }

            }
            //check if account has been reenabled after deprovison
            else if (Convert.ToString(StaffADObject.Properties["comment"].Value ?? "").Contains("DISABLED### by MattMIS")) { 
                if (role == "student") { ReportGenerator.studentAccountsDeprovisionExcludedCount++; studentLog.Info($"{StaffADObject.Properties["userPrincipalName"].Value} is not eligble for deprovision as it has been manually renabled."); } else if (role == "staff") { ReportGenerator.staffAccountsDeprovisionExcludedCount++; staffLog.Info($"{StaffADObject.Properties["userPrincipalName"].Value} is not eligble for deprovision as it has been manually renabled."); } 
            }
            
            else //IS NOT PROTECTED - SO CAN TRY TO BE DISABLED
            {
                try
                {
                    if (role == "student") { studentLog.Info($"======== Evaluating {StaffADObject.Properties["userPrincipalName"].Value} ======="); } else if (role == "staff") { staffLog.Info($"======== Evaluating {StaffADObject.Properties["userPrincipalName"].Value} ======="); }                   
                    StaffADObject.Properties["comment"].Value = $"DISABLED### by MattMIS  -  ###{DateTime.Now.ToString("dd/MM/yy HH:mm:ss")}";
                    StaffADObject.Properties["description"].Value = StaffADObject.Properties["description"].Value + $" |(Disabled By MattMIS {DateTime.Now.ToString("dd/MM/yy")})";
                    SharedMethods.DisableADAccount(StaffADObject);
                    if (role == "student") {  ReportGenerator.studentAccountsDeprovisionedCount++; studentLog.Info($"======== ACCOUNT {StaffADObject.Properties["userPrincipalName"].Value} AS BEEN DEPROVISONED BY THE SYSTEM ======="); } else if (role == "staff") {  ReportGenerator.staffAccountsDeprovisionedCount++; staffLog.Info($"======== ACCOUNT {StaffADObject.Properties["userPrincipalName"].Value} AS BEEN DEPROVISONED BY THE SYSTEM ======="); }
                    ReportGenerator.NewlyDisabledAccounts.Add(StaffADObject);

                }
                catch (Exception e)
                {
                    if (role == "student") { studentLog.Fatal($"{StaffADObject.Properties["userPrincipalName"].Value} HAS FAILED TO DEPROVISON. Error: {e.Message}"); } else if (role == "staff") { staffLog.Fatal($"{StaffADObject.Properties["userPrincipalName"].Value} HAS FAILED TO DEPROVISON. Error: {e.Message}"); }
                }
            }
        }

        public static void RunSIMSReports()
        {
            Console.WriteLine("======================================= Running SIMS Reports =====================================\n\nPlease wait...");
            ProcessStartInfo ProcessInfo;

            //RUN SPECIFIED BAT FILE - REDIRECT OUTPUT TO LOG FILE. WILL WAIT FOR ALL SIMS REPORTS TO FINISH BEFORE CONTINUEING.
            Process Process = new Process();
            ProcessInfo = new ProcessStartInfo("cmd.exe", "/c " + $"{Config.AppConfig.MISExports.ReportRunnerPath} > \"Logs\\SIMS Export Log - Latest.txt\"");
            ProcessInfo.UseShellExecute = false;

            Process.EnableRaisingEvents = true;
            Process.StartInfo = ProcessInfo;

            Process.Start();
            Process.WaitForExit();

            Process.Close();

            Console.WriteLine("======================================= Finished SIMS Reports =====================================\n\nStarting MattMIS...");

        }
    }
}

