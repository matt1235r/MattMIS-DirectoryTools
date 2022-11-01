using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace MattMIS_Sync
{

    internal class Program
    {
        private static readonly log4net.ILog staffLog = log4net.LogManager.GetLogger("StaffRollingLogFileAppender");
        private static readonly log4net.ILog studentLog = log4net.LogManager.GetLogger("StudentRollingLogFileAppender");
        public static Dictionary<int, YearGroupModel.YearGroupRecord> CurrentYearGroups;
        public static Dictionary<string, StaffRoleModel.StaffRoleRecord> StaffRoleGroups;
        public static StaffRoleModel.StaffProvisoningSettings StaffProvisoningSettings;
        public static YearGroupModel.StudentProvisoningSettings StudentProvisoningSettings;
        public static List<String> KnownStaffUPNList = new List<String>();
        public static List<String> KnownStudentUPNList = new List<String>();
        public static string[] KeepEnabledADAccounts = new string[] { };
        public static string[] KeepEnabledADAccountEndings = new string[] { };
        static string[] SIMSIDDoNotMakeADAccount = new string[] { };
        


        //STATS for Mailer

        static void Main(string[] args)
        {            
            try
            {
                SharedMethods.CreateRequiredFolders(); // Creates required folders for logging.
                log4net.Config.XmlConfigurator.Configure(); //Sets up logging enviroment
                Config.LoadConfig("Configs/config.xml"); //Loads Application Central COnfig
                
                //See if any of these override files have been specified
                if (File.Exists("Exclusions//Retain AD Accounts With UPN.txt")) { KeepEnabledADAccounts = File.ReadAllLines("Exclusions//Retain AD Accounts With UPN.txt"); }
                if (File.Exists("Exclusions//SIMS ID's to ignore.txt")) { SIMSIDDoNotMakeADAccount = File.ReadAllLines("Exclusions//SIMS ID's to ignore.txt"); }
                if (File.Exists("Exclusions//Retain AD Accounts Ending With Value.txt")) { KeepEnabledADAccountEndings = File.ReadAllLines("Exclusions//Retain AD Accounts Ending With Value.txt"); }

                //Gather SIMS Reports - External BAT File
                SharedMethods.RunSIMSReports();

                //========== Process Staff =====================
                ReadStaffRoleFile();
                ReadStaffMISExport();
                SharedMethods.CheckSurplusADAccounts("staff");
                
                //Delete Staff Export After Processing if specified
                if (Config.AppConfig.MISExports.DeleteReportsAfterProcessing) { File.Delete(Config.AppConfig.MISExports.StaffReportPath); }               

                //========================= Process Students ====================
                //LOAD NEW ACCOUNT PROVISIONING
                ReadYearGroupFile();
                ReadStudentMISExport();
                SharedMethods.CheckSurplusADAccounts("student");

                //Delete Student Export After Processing if specified
                if (Config.AppConfig.MISExports.DeleteReportsAfterProcessing) { File.Delete(Config.AppConfig.MISExports.StudentReportPath); }

                ReportGenerator.GenerateStatisticsFiles();
                ReportGenerator.SendReportEmail();
            }
            catch (Exception ex)
            {
                File.WriteAllText("Logs//Fatal Crash - Latest.txt", $"\n======== MattMIS Sync Crash Report ========\n\n{ex.Message}\n{ex.InnerException}\n{ex.StackTrace}");
            }
            
        }


        private static int CreateStaffAccount(StaffModel.StaffMISRecord simsRecord)
        {
            //CHECK IF SIMS ID IS EXCLUDED FROM HAVING AN AD ACCOUNT.
            if (SIMSIDDoNotMakeADAccount.Contains(simsRecord.ID.ToString())) { staffLog.Info($"SIMS ID {simsRecord.ID} has been excluded from AD account creation. Will be skipped."); return 3; }

            //CHECK WORK EMAIL IS ENTERED. SKIP IF NOT.
            if ((Convert.ToString(simsRecord.WorkEmail) ?? "").Length < 3) { ReportGenerator.SIMSAdminWarnings.Add(simsRecord, "Work Email Missing"); staffLog.Error($"SIMS ID {simsRecord.ID} DOES NOT HAVE A WORK EMAIL SPECIFIED SO CANNOT CREATE ACCOUNT. PLEASE CORRECT THIS IN SIMS."); return 3; }
            
            //CHECK WORK EMAIL IS ENTERED. SKIP IF NOT.
            if (!simsRecord.WorkEmail.EndsWith(StaffProvisoningSettings.ValidateWorkEmailEndsWith)) { ReportGenerator.SIMSAdminWarnings.Add(simsRecord, "Work Email Invalid"); staffLog.Error($"SIMS ID {simsRecord.ID} DOES NOT HAVE A .ORG SCHOOL EMAIL ADDRESS SO CANNOT CREATE ACCOUNT. PLEASE CORRECT THIS IN SIMS."); return 3; }

            try
            {
                //GET STAFF ROLE DETAILS
                
                string role = "Staff";
                if ((simsRecord.Role ?? "").Length > 0) { role = simsRecord.Role.Split('(')[0].Trim(); } 

                StaffRoleModel.StaffRoleRecord staffRoleDetails;
                
                if (StaffRoleGroups.ContainsKey(role))
                {
                    staffRoleDetails = Program.StaffRoleGroups[role];
                }
                else
                {
                    ReportGenerator.SIMSAdminWarnings.Add(simsRecord, "ROLE MISSING FROM ALLOCATON FILE");
                    staffLog.Fatal($"JOB ROLE: {role} is missing from staff allocation file. USER CAN NOT BE CREATED!");
                    return 2;
                    
                }
                

                //EXTRACT USERNAME FROM WORK EMAIL             
                string SAMname = StaffUsernameFromEmail(simsRecord.WorkEmail);
                string userUPN = SAMname + staffRoleDetails.UPN;

                staffLog.Info($"Creating AD account for user: {userUPN}");
                if (userUPN.Length > 0)
                {
                    DirectoryEntry parentOU = new DirectoryEntry("LDAP://" + staffRoleDetails.OU);
                    DirectoryEntry StaffADObject = parentOU.Children.Add($"CN={simsRecord.PreferredForename} {simsRecord.PreferredSurname}", "user");

                    //SET BASIC AD ACCOUNT PROPERTIES
                    StaffADObject.Properties["samAccountName"].Value = SAMname;
                    StaffADObject.Properties["userPrincipalName"].Value = userUPN;
                    StaffADObject.Properties["mail"].Value = userUPN;
                    StaffADObject.Properties["physicalDeliveryOfficeName"].Value = Convert.ToString(simsRecord.ID);
                    StaffADObject.Properties["title"].Value = "Staff";

                    //SETUP Profile Home
                    StaffADObject.Properties["homeDrive"].Value = "S:";
                    StaffADObject.Properties["homeDirectory"].Value = $"{staffRoleDetails.ProfilePath}{SAMname}";

                    //USER INITIAL SAVE
                    StaffADObject.CommitChanges();
                    parentOU.CommitChanges();
                   
                    staffLog.Info($"Saved AD account for user: {userUPN}....");

                    string passwordToSet = $"{StaffProvisoningSettings.DefaultPassword}";
                    StaffADObject.Invoke("SetPassword", new object[] { passwordToSet });
                    StaffADObject.Properties["pwdLastSet"][0] = 0;

                    StaffADObject.CommitChanges();

                    //ADD USER TO GROUPS
                    //WAIT HERE AND LET ACTIVE DIRECTORY CATCH UP!
                    Console.WriteLine("Waiting for active directory to catch up....");
                    Thread.Sleep(5000);
                    foreach (string groupName in staffRoleDetails.groups) { staffLog.Info($"Now adding user to group: {groupName}"); SharedMethods.AddUserToGroup(StaffADObject.Properties["userPrincipalName"].Value.ToString(), groupName, "staff"); }
                    

                    staffLog.Info($"Account created for user.");
                    //then update normal checkable fields
                    CheckForStaffUpdates(simsRecord, StaffADObject);

                    //BETTER ADD ACCOUNT TO THE KNOWN ACCOUNT LIST SO IT DOESN'T GET DISABLED
                    KnownStaffUPNList.Add(StaffADObject.Properties["userPrincipalName"].Value.ToString());

                    //After first update I will set the account back to "created". So we can sort 365 things out later...
                    StaffADObject.Properties["comment"].Value = $"CREATED### by MattMIS  -  ###{DateTime.Now.ToString("dd/MM/yy HH:mm:ss")}";
                    SharedMethods.EnableADAccount(StaffADObject);


                    ReportGenerator.staffAccountsCreatedCount++;
                    ReportGenerator.NewlyCreatedAccounts.Add(StaffADObject, passwordToSet);

                    StaffADObject.CommitChanges();

                    
                }
                else
                {
                    staffLog.Error($"ERROR! ACCOUNT CAN NOT BE CREATED AS THE WORK EMAIL IS MISSING. PLEASE CORRECT THIS IN SIMS.");
                }


            }
            catch (Exception ex)
            {

                if (ex.Message.Contains("The object already exists.\r\n") || ex.Message.Contains("A constraint violation occurred.\r\n"))
                {
                    staffLog.Error($"THE USER ACCOUNT ALREADY EXISTS. THE SIMS MAPPING MAY BE DAMAGED - OR THERE IS A USERNAME CONFLICT.\n");
                }
                else
                {
                    staffLog.Error($"ERROR OCCURED WHILE CREATING AD ACCOUNT. ERROR: {ex.Message}");
                    staffLog.Error($"ERROR OCCURED WHILE CREATING AD ACCOUNT. ERROR: {ex.InnerException}");
                }

            }

            return 1;
        }

        //NOT YET IMPLEMENTED

        private static string StudentUsernameGenerator(string FirstName, string LastName, YearGroupModel.YearGroupRecord yearGroupRecord)
        {
            //See if special characters need stripping
            Regex alphaNumeric = new Regex("[^a-zA-Z0-9-]");
            if (StudentProvisoningSettings.StripSpecialCharacters) { LastName = alphaNumeric.Replace(LastName, ""); }
            if (StudentProvisoningSettings.StripSpecialCharacters) { FirstName = alphaNumeric.Replace(FirstName, ""); }
            
            int surnamelength = LastName.Length;
            if (surnamelength > 5) { surnamelength = 5; }
            LastName = LastName.Substring(0, surnamelength);
            
            return Regex.Replace(FirstName, "[^a-zA-Z]+", "", RegexOptions.Compiled)[0] + LastName + yearGroupRecord.ShortStartYearCode;
        }

        private static string StaffUsernameFromEmail(string email)
        {
            //See if special characters need stripping
            Regex alphaNumeric = new Regex("[^a-zA-Z0-9-]");

            string emailAlias = email.Split('@')[0];

            if (StaffProvisoningSettings.StripSpecialCharacters) { emailAlias = alphaNumeric.Replace(emailAlias, ""); }

            return emailAlias;
        }

        private static int CreateStudentAccount(StudentModel.StudentMISRecord simsRecord)
        {

            //CHECK IF SIMS ID IS EXCLUDED FROM HAVING AN AD ACCOUNT.
            if (SIMSIDDoNotMakeADAccount.Contains(simsRecord.ID.ToString())) { studentLog.Info($"SIMS ID {simsRecord.ID} has been excluded from AD account creation. Will be skipped."); return 3; }

            try
            {               
                //GET STUDENT YEAR DETAILS
                YearGroupModel.YearGroupRecord studentYearDetails = Program.CurrentYearGroups[simsRecord.YeartaughtinCode];

                string SAMname = StudentUsernameGenerator(simsRecord.Forename, simsRecord.Surname, studentYearDetails);
                string userUPN = SAMname + studentYearDetails.UPN;

                studentLog.Info($"Creating AD account for user: {userUPN}");
                if (userUPN.Length > 0)
                {
                    DirectoryEntry parentOU = new DirectoryEntry("LDAP://" + studentYearDetails.OU);
                    DirectoryEntry StaffADObject = parentOU.Children.Add($"CN={simsRecord.Forename} {simsRecord.Surname}", "user");

                    //SET BASIC AD ACCOUNT PROPERTIES
                    StaffADObject.Properties["samAccountName"].Value = SAMname;
                    StaffADObject.Properties["userPrincipalName"].Value = userUPN;
                    StaffADObject.Properties["mail"].Value = userUPN;
                    StaffADObject.Properties["physicalDeliveryOfficeName"].Value = Convert.ToString(simsRecord.ID);
                    StaffADObject.Properties["title"].Value = "STUDENT";

                    //SETUP Profile Home
                    StaffADObject.Properties["homeDrive"].Value = "S:";
                    StaffADObject.Properties["homeDirectory"].Value = $"{studentYearDetails.ProfilePath}{SAMname}";

                    //USER INITIAL SAVE
                    StaffADObject.CommitChanges();
                    parentOU.CommitChanges();
                    studentLog.Info($"Creating AD account for user: {userUPN}");


                    //ADD USER TO GROUPS
                    //WAIT HERE AND LET ACTIVE DIRECTORY CATCH UP!
                    Console.WriteLine("Waiting for active directory to catch up....");
                    Thread.Sleep(5000);
                    foreach (string groupName in studentYearDetails.groups) { staffLog.Info($"Now adding user to group: {groupName}"); SharedMethods.AddUserToGroup(StaffADObject.Properties["userPrincipalName"].Value.ToString(), groupName, "student"); }

                    string passwordToSet = StudentProvisoningSettings.DefaultPassword;
                    StaffADObject.Invoke("SetPassword", new object[] { StudentProvisoningSettings.DefaultPassword });
                    StaffADObject.Properties["pwdLastSet"][0] = 0;

                    StaffADObject.CommitChanges();


                    studentLog.Info($"Account created for user.");
                    //then update normal checkable fields
                    CheckForStudentUpdates(simsRecord, StaffADObject);

                    //BETTER ADD ACCOUNT TO THE KNOWN ACCOUNT LIST SO IT DOESN'T GET DISABLED
                    KnownStudentUPNList.Add(StaffADObject.Properties["userPrincipalName"].Value.ToString());

                    //After first update I will set the account back to "created". So we can sort 365 things out later...
                    StaffADObject.Properties["comment"].Value = $"CREATED### by MattMIS  -  ###{DateTime.Now.ToString("dd/MM/yy HH:mm:ss")}";
                    SharedMethods.EnableADAccount(StaffADObject);
                    StaffADObject.CommitChanges();

                    ReportGenerator.studentAccountsCreatedCount++;
                    ReportGenerator.NewlyCreatedAccounts.Add(StaffADObject, passwordToSet);
                }
                else
                {
                    studentLog.Error($"ERROR! ACCOUNT CAN NOT BE CREATED AS THE WORK EMAIL IS MISSING. PLEASE CORRECT THIS IN SIMS.");
                    //ReportGenerator.SIMSAdminWarnings.Add(simsRecord, "EMAIL MISSING");
                }


            }
            catch (Exception ex)
            {

                if (ex.Message.Contains("The object already exists.\r\n") || ex.Message.Contains("A constraint violation occurred.\r\n"))
                {
                    studentLog.Error($"THE USER ACCOUNT ALREADY EXISTS. THE SIMS MAPPING MAY BE DAMAGED - OR THERE IS A USERNAME CONFLICT.\n");
                }
                else
                {
                    studentLog.Error($"ERROR OCCURED WHILE CREATING AD ACCOUNT. ERROR: {ex.Message}");
                    studentLog.Error($"ERROR OCCURED WHILE CREATING AD ACCOUNT. ERROR: {ex.InnerException}");
                }

            }

            return 1;
        }

        private static void ReadYearGroupFile()
        {
            studentLog.Info($"Loading student yeargroup file: {Config.AppConfig.AccountProvisioning.StudentSettingsPath}");
            XmlDocument doc = new XmlDocument();
            doc.Load(Config.AppConfig.AccountProvisioning.StudentSettingsPath);

            XmlSerializer serializer2 = new XmlSerializer(typeof(YearGroupModel.YearGroupProvisoning));
            YearGroupModel.YearGroupProvisoning reportInfo = null;
            using (StringReader reader = new StringReader(doc.InnerXml))
            {
                reportInfo = (YearGroupModel.YearGroupProvisoning)serializer2.Deserialize(reader);
            }

            //FIRST LETS CHECK THE EXPIRATION DATE!
            DateTime expirationDate = DateTime.Parse(reportInfo.ExpiresOn);
            ReportGenerator.YearGroupMappingExpiration = reportInfo.ExpiresOn;
            

            //expiration date is less than current date.. AKA file has expired
            if (expirationDate < DateTime.Now)
            {
               
                SharedMethods.CriticalError("STUDENTALLOCATIONEXPIRED");

            }

            //Iterate through all students in this report
            CurrentYearGroups = new Dictionary<int, YearGroupModel.YearGroupRecord>();
            CurrentYearGroups = reportInfo.years.ToDictionary(i => i.code, i => i);
            

            //load staff default settings
            StudentProvisoningSettings = reportInfo.Settings;

            studentLog.Info($"Student yeargroup file read: {Config.AppConfig.AccountProvisioning.StudentSettingsPath}");
        }
        private static void ReadStaffRoleFile()
        {
            staffLog.Info($"Loading staff role file: {Config.AppConfig.AccountProvisioning.StaffSettingsPath}");
            XmlDocument doc = new XmlDocument();
            doc.Load(Config.AppConfig.AccountProvisioning.StaffSettingsPath);

            XmlSerializer serializer2 = new XmlSerializer(typeof(StaffRoleModel.StaffProvisoning));
            StaffRoleModel.StaffProvisoning reportInfo = null;
            using (StringReader reader = new StringReader(doc.InnerXml))
            {
                reportInfo = (StaffRoleModel.StaffProvisoning)serializer2.Deserialize(reader);
            }
            //Iterate through all staff in this report
            StaffRoleGroups = new Dictionary<string, StaffRoleModel.StaffRoleRecord>();
            StaffRoleGroups = reportInfo.Roles.ToDictionary(i => i.name, i => i) ;

            //load staff default settings
            StaffProvisoningSettings = reportInfo.Settings;

            staffLog.Info($"Staff role file read: {Config.AppConfig.AccountProvisioning.StaffSettingsPath}");

            
        }

        private static bool ReadStudentMISExport()
        {

            studentLog.Info($"Loading student export file: {Config.AppConfig.MISExports.StudentReportPath}");
            XmlDocument doc = new XmlDocument();
            doc.Load(Config.AppConfig.MISExports.StudentReportPath);

            XmlSerializer serializer2 = new XmlSerializer(typeof(StudentModel.SuperStarReport));
            StudentModel.SuperStarReport reportInfo = null;
            using (StringReader reader = new StringReader(doc.InnerXml))
            {
                reportInfo = (StudentModel.SuperStarReport)serializer2.Deserialize(reader);
            }
            //Iterate through all staff in this report
            Dictionary<string, StudentModel.StudentMISRecord> Today = new Dictionary<string, StudentModel.StudentMISRecord>();
            studentLog.Info($"\n\n====================== PROCESSING {reportInfo.Record.Length} STUDENTS ======================\n");

            ReportGenerator.studentOnRollCount = reportInfo.Record.Length;


            foreach (StudentModel.StudentMISRecord r in reportInfo.Record)
            {
                studentLog.Info($"======== Processing {r.Forename} {r.Surname} (ID {r.ID}) =======");
                Console.WriteLine($"\n====== {r.Forename} {r.Surname} (ID {r.ID}) ======");

                ReportGenerator.studentFormGroups.Add(r.Reg);
                ProcessStudentMember(r);


            }

            Console.WriteLine($"\n\n========= FINISHED PROCESSING {reportInfo.Record.Length} STUDENTS =========");
            studentLog.Info($"\n\n========= FINISHED PROCESSING {reportInfo.Record.Length} STUDENTS =========");

            return false;
        }

        private static bool ReadStaffMISExport()
        {

            staffLog.Info($"Loading staff export file: {Config.AppConfig.MISExports.StaffReportPath}");
            XmlDocument doc = new XmlDocument();
            doc.Load(Config.AppConfig.MISExports.StaffReportPath);

            XmlSerializer serializer2 = new XmlSerializer(typeof(StaffModel.SuperStarReport));
            StaffModel.SuperStarReport reportInfo = null;
            using (StringReader reader = new StringReader(doc.InnerXml))
            {
                reportInfo = (StaffModel.SuperStarReport)serializer2.Deserialize(reader);
            }
            //Iterate through all staff in this report
            //remeber current staff member 
            int currentStaffID = 0;
            Dictionary<string, StaffModel.StaffMISRecord> Today = new Dictionary<string, StaffModel.StaffMISRecord>();
            staffLog.Info($"\n\n====================== PROCESSING {reportInfo.Record.Length} STAFF ======================\n");
            ReportGenerator.staffOnRollCount = reportInfo.Record.Length;

            foreach (StaffModel.StaffMISRecord r in reportInfo.Record)
            {
                if (currentStaffID != r.ID)
                {
                    currentStaffID = r.ID;
                    staffLog.Info($"======== Processing {r.PreferredForename} {r.PreferredSurname} (ID {r.ID}) =======");
                    Console.WriteLine($"\n====== {r.PreferredForename} {r.PreferredSurname} {r.ID}======");

                    if (r.Role != null) { ReportGenerator.staffJobRoles.Add(r.Role.Split('(')[0].Trim()); }
                    else { ReportGenerator.staffJobRoles.Add("EMPTY"); }

                    ProcessStaffMember(r);
                }
                else
                {
                    staffLog.Info($"======== Duplicate detected  {r.PreferredForename} {r.PreferredSurname} (ID {r.ID}) =======");
                }

            }

            Console.WriteLine($"\n\n========= FINISHED PROCESSING {reportInfo.Record.Length} STAFF =========");
            staffLog.Info($"\n\n========= FINISHED PROCESSING {reportInfo.Record.Length} STAFF =========");           

            return false;
        }

        private static void ProcessStaffMember(StaffModel.StaffMISRecord simsRecord)
        {
            DirectoryEntry StaffADObject = ResolveADAccount(simsRecord.ID, "staff");

            //if not null must be found.
            if (StaffADObject.Properties["comment"].Value.ToString() == "Matched to Single User")
            {
                //All good. User Account was found. Now it's time to check if any values need changing for them in AD.
                CheckForStaffUpdates(simsRecord, StaffADObject);

            }
            else if (StaffADObject.Properties["comment"].Value.ToString() == "Not Found in AD")
            {
                CreateStaffAccount(simsRecord);
            }
            else if (StaffADObject.Properties["comment"].Value.ToString() == "Multple Users Found")
            {
                //Multiple Accounts for the user found in AD. Let's log this - I'm not touching that with a 10 foot barge pole.

            }
            else
            {
               staffLog.Error($"Unexpected Outcome with user: {simsRecord.PreferredForename} {simsRecord.PreferredSurname}. Please correct this to resume MIS/AD Sync.");
            }

            
        }

        private static void ProcessStudentMember(StudentModel.StudentMISRecord simsRecord)
        {
            DirectoryEntry StaffADObject = ResolveADAccount(simsRecord.ID, "student");
            //if not null must be found.
            if (StaffADObject.Properties["comment"].Value.ToString() == "Matched to Single User")
            {
                //All good. User Account was found. Now it's time to check if any values need changing for them in AD.
                CheckForStudentUpdates(simsRecord, StaffADObject);
            }
            else if (StaffADObject.Properties["comment"].Value.ToString() == "Not Found in AD")
            {
                CreateStudentAccount(simsRecord);

            }
            else if (StaffADObject.Properties["comment"].Value.ToString() == "Multple Users Found")
            {
                //Multiple Accounts for the user found in AD. Let's log this - I'm not touching that with a 10 foot barge pole.

            }
            else
            {
                studentLog.Error($"Unexpected Outcome with user: {simsRecord.Forename} {simsRecord.Surname}. Please correct this to resume MIS/AD Sync.");
            }
        }

        private static void CheckForStaffUpdates(StaffModel.StaffMISRecord simsRecord, DirectoryEntry StaffADObject)
        {
            try
            {
                //Check if account has been excluded. Return if so.
                if ((Convert.ToString(StaffADObject.Properties["facsimileTelephoneNumber"].Value) ?? "") == "SKIP") { StaffADObject.Properties["comment"].Value = $"SKIPPED### by MattMIS  -  ###{DateTime.Now.ToString("dd/MM/yy HH:mm:ss")}"; StaffADObject.CommitChanges(); return; }

                //Start checking properties are set correctly from MIS
                PropertyCollection properties = StaffADObject.Properties;

                //Check surname
                if ((Convert.ToString(StaffADObject.Properties["sn"].Value) ?? "") != simsRecord.PreferredSurname)
                {
                    staffLog.Warn($"Found Surname Mismatch.. {Convert.ToString(StaffADObject.Properties["sn"].Value) ?? ""} should be {simsRecord.PreferredSurname}");
                    StaffADObject.Properties["sn"].Value = simsRecord.PreferredSurname;
                }

                //Check Display Name Printable
                if ((Convert.ToString(properties["displayNamePrintable"].Value) ?? "") != $"{simsRecord.PreferredForename} {simsRecord.PreferredSurname}")
                {
                    staffLog.Warn($"Found DisplayName Printable Mismatch.. {Convert.ToString(StaffADObject.Properties["displayNamePrintable"].Value) ?? ""} should be {simsRecord.PreferredForename} {simsRecord.PreferredSurname}");
                    StaffADObject.Properties["displayNamePrintable"].Value = $"{simsRecord.PreferredForename} {simsRecord.PreferredSurname}";
                }

                //Check first name
                //SHOULD I JUST USE STAFF INITIAL
                if (StaffProvisoningSettings.UseFirstNameInitial)
                {
                    if ((Convert.ToString(StaffADObject.Properties["givenName"].Value) ?? "") != $"{simsRecord.PreferredForename[0]}")
                    {
                        staffLog.Warn($"Found Firstname Mismatch.. {(StaffADObject.Properties["givenName"].Value) ?? ""[0]} should be {simsRecord.PreferredForename[0]}");
                        StaffADObject.Properties["givenName"].Value = $"{simsRecord.PreferredForename[0]}";
                    }
                }
                else //DO NOT USE ONLY INITIAL
                {
                    if ((Convert.ToString(StaffADObject.Properties["givenName"].Value) ?? "") != simsRecord.PreferredForename)
                    {
                        staffLog.Warn($"Found Firstname Mismatch.. {Convert.ToString(StaffADObject.Properties["givenName"].Value) ?? ""} should be {simsRecord.PreferredForename}");
                        StaffADObject.Properties["givenName"].Value = simsRecord.PreferredForename;
                    }
                }


                //Check Display Name
                if (StaffProvisoningSettings.UseFirstNameInitial)
                {
                    if ((Convert.ToString(properties["DisplayName"].Value) ?? "") != $"{simsRecord.PreferredForename[0]} {simsRecord.PreferredSurname}")
                    {
                        staffLog.Warn($"Found DisplayName Mismatch.. {(StaffADObject.Properties["DisplayName"].Value) ?? ""} should be {simsRecord.PreferredForename[0]} {simsRecord.PreferredSurname}");
                        StaffADObject.Properties["DisplayName"].Value = $"{simsRecord.PreferredForename[0]} {simsRecord.PreferredSurname}";             
                    }
                }
                else // USE FULL NAME
                {
                    if ((Convert.ToString(properties["DisplayName"].Value) ?? "") != $"{simsRecord.PreferredForename} {simsRecord.PreferredSurname}")
                    {
                        staffLog.Warn($"Found DisplayName Mismatch.. {Convert.ToString(StaffADObject.Properties["DisplayName"].Value) ?? ""} should be {simsRecord.PreferredForename} {simsRecord.PreferredSurname}");
                        StaffADObject.Properties["DisplayName"].Value = $"{simsRecord.PreferredForename} {simsRecord.PreferredSurname}";                     
                    }
                }
                    

                //Check Department
                string roleName = "Staff";
                string startDate = simsRecord.EmploymentStartDate.ToString("dd/MM/yy") ?? "Unknown";
                
                if ((simsRecord.Role ?? "").Length > 0) 
                { 
                    roleName = simsRecord.Role.Split('(')[0].Trim(); 
                } 
                else
                {
                    if (ReportGenerator.SIMSAdminWarnings.ContainsKey(simsRecord))
                    {
                        ReportGenerator.SIMSAdminWarnings[simsRecord] += (" & Job Role Missing");
                    }
                    else
                    {
                        ReportGenerator.SIMSAdminWarnings.Add(simsRecord, "Job Role Missing");
                    }
                }
                
                if ((Convert.ToString(properties["department"].Value) ?? "") != roleName)
                {
                    staffLog.Warn($"Found Department Mismatch.. {Convert.ToString(StaffADObject.Properties["Department"].Value) ?? ""} should be {roleName}");
                    StaffADObject.Properties["department"].Value = roleName;
                }

                //Check Department - USES ROLENAME AND STARTDATE
                if (((Convert.ToString(StaffADObject.Properties["description"].Value) ?? "") != $"{simsRecord.FullName} - {roleName} (Joined: {startDate.Split(' ')[0]})"))
                {
                    staffLog.Warn($"Found Description Mismatch.. {Convert.ToString(StaffADObject.Properties["description"].Value) ?? ""} should be {simsRecord.FullName} - {roleName} (Joined: {startDate.Split(' ')[0]})");
                    StaffADObject.Properties["description"].Value = $"{simsRecord.FullName} - {roleName} (Joined: {startDate.Split(' ')[0]})";
                }

                //Check Staff Initials Code
                if (((Convert.ToString(StaffADObject.Properties["initials"].Value) ?? "") != simsRecord.StaffCode) && (simsRecord.StaffCode != null))
                {
                    staffLog.Warn($"Found Staff Title Mismatch.. {Convert.ToString(StaffADObject.Properties["initials"].Value) ?? ""} should be {simsRecord.StaffCode}");
                    StaffADObject.Properties["initials"].Value = simsRecord.StaffCode;
                }

                //Check Proxy Email Attributes

                if (simsRecord.WorkEmail != null)
                {
                    bool simsAliasRegistered = false;
                    foreach (String address in (properties["proxyaddresses"]))
                    {
                        if (address.ToLower() == "smtp:" + simsRecord.WorkEmail.ToLower()) { simsAliasRegistered = true; break; }
                        
                    }
                    //IF IT IS THE UPN THEN WE WILL SKIP
                    if (Convert.ToString(StaffADObject.Properties["userPrincipleName"].Value ?? "") == simsRecord.WorkEmail ) { simsAliasRegistered = true; };
                    
                    if (!simsAliasRegistered)
                    {
                        staffLog.Warn($"SIMS EMAIL NOT FOUND AS ALIAS. {(Convert.ToString(properties["proxyaddresses"].Value) ?? "").ToLower()} should contain {"smtp:" + simsRecord.WorkEmail}");
                        StaffADObject.Properties["proxyaddresses"].Value = new object[] { "smtp:" + simsRecord.WorkEmail };
                    }
                }
                else
                {
                    staffLog.Warn($"USER MISSING WORK EMAIL ADDRESS. PLEASE ENTER THIS IN SIMS. WILL BE SKIPPED");
                    if (ReportGenerator.SIMSAdminWarnings.ContainsKey(simsRecord))
                    {
                        ReportGenerator.SIMSAdminWarnings[simsRecord] += (" & Work Email Missing");
                    }
                    else
                    {
                        ReportGenerator.SIMSAdminWarnings.Add(simsRecord, "Work Email Missing");
                    }
                    
                }

                //CHECK IF IN ADDRESS BOOK
                if(Convert.ToBoolean(StaffADObject.Properties["msExchHideFromAddressLists"].Value) == true)
                {
                    StaffADObject.Properties["msExchHideFromAddressLists"].Value = null;
                    staffLog.Warn($"User is hidden from address book! Will be reshown");
                }

                //Set Warning of Processing
                StaffADObject.Properties["comment"].Value = $"CHECKED### by MattMIS  -  ###{DateTime.Now.ToString("dd/MM/yy HH:mm:ss")}";

                StaffADObject.CommitChanges();
                //log.Info($"Saved Sucessfully"); NO NEED TO LOG THIS ANYMORE.
                staffLog.Info($"======== Finished Processing {simsRecord.PreferredForename} {simsRecord.PreferredSurname} (ID {simsRecord.ID}) =======");

                

            }
            catch (Exception ex)
            {

                staffLog.Fatal("ERROR OCCURED: " + ex.Message);
                //staffLog.Info($"======== FAILED TO PROCESS {simsRecord.PreferredForename} {simsRecord.PreferredSurname} (ID {simsRecord.ID}) =======");

            }

        }

        private static void CheckForStudentUpdates(StudentModel.StudentMISRecord simsRecord, DirectoryEntry StaffADObject)
        {

            try
            {
                //Check if account has been excluded. Return if so.
                if ((Convert.ToString(StaffADObject.Properties["facsimileTelephoneNumber"].Value) ?? "") == "SKIP") { StaffADObject.Properties["comment"].Value = $"SKIPPED### by MattMIS  -  ###{DateTime.Now.ToString("dd/MM/yy HH:mm:ss")}"; StaffADObject.CommitChanges(); return; }

                //Start checking properties are set correctly from MIS
                PropertyCollection properties = StaffADObject.Properties;

                //Check surname
                if ((Convert.ToString(StaffADObject.Properties["sn"].Value) ?? "") != simsRecord.Surname)
                {
                    studentLog.Warn($"Found Surname Mismatch.. {Convert.ToString(StaffADObject.Properties["sn"].Value) ?? ""} should be {simsRecord.Surname}");
                    StaffADObject.Properties["sn"].Value = simsRecord.Surname;
                }

                //Check first name
                //SHOULD I JUST USE STUDENT INITIAL
                if (StudentProvisoningSettings.UseFirstNameInitial)
                {
                    if ((Convert.ToString(StaffADObject.Properties["givenName"].Value) ?? "") != $"{simsRecord.Forename[0]}")
                    {
                        studentLog.Warn($"Found Firstname Mismatch.. {Convert.ToString(StaffADObject.Properties["givenName"].Value) ?? ""} should be {simsRecord.Forename}");
                        StaffADObject.Properties["givenName"].Value = $"{simsRecord.Forename[0]}";
                    }
                }
                else //DO NOT USE ONLY INITIAL
                {
                    if ((Convert.ToString(StaffADObject.Properties["givenName"].Value) ?? "") != simsRecord.Forename)
                    {
                        studentLog.Warn($"Found Firstname Mismatch.. {Convert.ToString(StaffADObject.Properties["givenName"].Value) ?? ""} should be {simsRecord.Forename}");
                        StaffADObject.Properties["givenName"].Value = simsRecord.Forename;
                    }
                }

                //Check Display Name
                if (StudentProvisoningSettings.UseFirstNameInitial)
                {
                    if ((Convert.ToString(properties["DisplayName"].Value) ?? "") != $"{simsRecord.Forename[0]} {simsRecord.Surname}")
                    {
                        studentLog.Warn($"Found DisplayName Mismatch.. {Convert.ToString(StaffADObject.Properties["DisplayName"].Value) ?? ""} should be {simsRecord.Forename} {simsRecord.Surname}");
                        StaffADObject.Properties["DisplayName"].Value = $"{simsRecord.Forename[0]} {simsRecord.Surname}";
                    }
                }
                else // USE FULL NAME
                {
                    //Check Display Name
                    if ((Convert.ToString(properties["DisplayName"].Value) ?? "") != $"{simsRecord.Forename} {simsRecord.Surname}")
                    {
                        studentLog.Warn($"Found DisplayName Mismatch.. {Convert.ToString(StaffADObject.Properties["DisplayName"].Value) ?? ""} should be {simsRecord.Forename} {simsRecord.Surname}");
                        StaffADObject.Properties["DisplayName"].Value = $"{simsRecord.Forename} {simsRecord.Surname}";
                    }
                }                

                //Check Display Name Printable
                if ((Convert.ToString(properties["displayNamePrintable"].Value) ?? "") != $"{simsRecord.Forename} {simsRecord.Surname}")
                {
                    studentLog.Warn($"Found DisplayName Printable Mismatch.. {Convert.ToString(StaffADObject.Properties["displayNamePrintable"].Value) ?? ""} should be {simsRecord.Forename} {simsRecord.Surname}");
                    StaffADObject.Properties["displayNamePrintable"].Value = $"{simsRecord.Forename} {simsRecord.Surname}";
                }


                //Check Description
                string roleName = "Student";
                if ((simsRecord.Reg ?? "").Length > 0) { roleName = simsRecord.Reg; }
                string startDate = simsRecord.Startdate.ToString("dd/MM/yy") ?? "Unknown";

                //Check Staff Initials Code
                if (((Convert.ToString(StaffADObject.Properties["description"].Value) ?? "") != $"{simsRecord.FullName} - {roleName} (Joined: {startDate.Split(' ')[0]})"))
                {
                    studentLog.Warn($"Found Description Mismatch.. {Convert.ToString(StaffADObject.Properties["description"].Value) ?? ""} should be {simsRecord.FullName} - {roleName} (Joined: {startDate.Split(' ')[0]})");
                    StaffADObject.Properties["description"].Value = $"{simsRecord.FullName} - {roleName} (Joined: {startDate.Split(' ')[0]})";
                }

                //Check Reg Group
                if ((Convert.ToString(properties["department"].Value) ?? "") != $"{simsRecord.Reg}")
                {
                    studentLog.Warn($"Found Registration Group Mismatch.. {Convert.ToString(StaffADObject.Properties["department"].Value) ?? ""} should be {simsRecord.Reg}");
                    StaffADObject.Properties["department"].Value = $"{simsRecord.Reg}";
                }

                //CHECK IF IN ADDRESS BOOK
                if (Convert.ToBoolean(StaffADObject.Properties["msExchHideFromAddressLists"].Value) == true)
                {
                    StaffADObject.Properties["msExchHideFromAddressLists"].Value = null;
                    studentLog.Warn($"User is hidden from address book! Will be reshown");
                }

                //Set Warning of Processing
                StaffADObject.Properties["comment"].Value = $"CHECKED### by MattMIS  -  ###{DateTime.Now.ToString("dd/MM/yy HH:mm:ss")}";

                StaffADObject.CommitChanges();
                //log.Info($"Saved Sucessfully"); NO NEED TO LOG THIS ANYMORE.
                studentLog.Info($"======== Finished Processing {simsRecord.Forename} {simsRecord.Surname} (ID {simsRecord.ID}) =======");


            }
            catch (Exception ex)
            {

                studentLog.Fatal("ERROR OCCURED: " + ex.Message);

            }

        }

        private static DirectoryEntry ResolveADAccount(int ID, string role)
        {
            DirectoryEntry StaffADEntry = new DirectoryEntry();

            DirectoryEntry staffOUObject = new DirectoryEntry();
            if (role.ToLower() == "staff")
            {
                staffOUObject.Path = "LDAP://" + Config.AppConfig.ActiveDirectory.StaffRootOU;
            }
            else
            {
                staffOUObject.Path = "LDAP://" + Config.AppConfig.ActiveDirectory.StudentRootOU;
            }

            DirectorySearcher deSearch = new DirectorySearcher(staffOUObject);

            deSearch.SearchScope = SearchScope.Subtree;
            deSearch.Filter = $"(&(objectCategory=person)(objectClass=User)(physicalDeliveryOfficeName={ID}))";
            SearchResultCollection userResults = deSearch.FindAll();

            //Firsty - Register any matched AD account's as known accounts!
            // I will add them to the list of matched staff/students too - dont want them deleted!
            if (role.ToLower() == "staff")
            {
                foreach (SearchResult matchedADAccount in userResults) { KnownStaffUPNList.Add(matchedADAccount.GetDirectoryEntry().Properties["userPrincipalName"].Value.ToString()); staffLog.Info($"Sucessfully matched to: {matchedADAccount.GetDirectoryEntry().Properties["userPrincipalName"].Value.ToString()}"); ReportGenerator.staffAccountsMatchedCount++; }
            }
            else
            {
                foreach (SearchResult matchedADAccount in userResults) { KnownStudentUPNList.Add(matchedADAccount.GetDirectoryEntry().Properties["userPrincipalName"].Value.ToString()); studentLog.Info($"Sucessfully matched to: {matchedADAccount.GetDirectoryEntry().Properties["userPrincipalName"].Value.ToString()}"); ReportGenerator.studentAccountsMatchedCount++; }
            }

            if (userResults.Count == 1) //Just one  Member Exists with this ID -- All good. I will check if any updates are needed and if they have been disabled.                
            {
                StaffADEntry = userResults[0].GetDirectoryEntry();

                //CHECK IF USER ACCOUNT HAS BEEN DISABLED AND WHY
                if (SharedMethods.IsADAccountDisabled(StaffADEntry))
                {

                    //WAS IT DISABLED BY US?
                    if ((Convert.ToString(StaffADEntry.Properties["comment"].Value) ?? "").StartsWith("DISABLED### by MattMIS  -  ###") || (Convert.ToString(StaffADEntry.Properties["description"].Value) ?? "").Contains("(Disabled By MattMIS"))
                    {
                        try
                        {
                            //ACCOUNT WAS DISABLED BY MATTMIS. LETS ENABLE IT AGAIN
                            SharedMethods.EnableADAccount(StaffADEntry);
                            if (StaffADEntry.Properties["description"].Value != null) { StaffADEntry.Properties["description"].Value = StaffADEntry.Properties["description"].Value.ToString().Split('|')[0]; }


                            if (role == "student") { ReportGenerator.studentAccountsCreatedCount++; studentLog.Error($"USER ACCOUNT WAS DISABLED BY MATT MIS IN PAST. ACCOUNT HAS BEEN RE-PROVISONED SUCESSFULLY"); }
                            else if (role == "staff") { ReportGenerator.staffAccountsCreatedCount++; staffLog.Error($"USER ACCOUNT WAS DISABLED BY MATT MIS IN PAST. ACCOUNT HAS BEEN RE-PROVISONED SUCESSFULLY"); }
                            StaffADEntry.Properties["comment"].Value = "Matched to Single User";

                        }
                        catch (Exception E)
                        {

                            if (role == "student") { studentLog.Fatal($"USER ACCOUNT WAS DISABLED BY MATT MIS IN PAST. TRIED TO REPROVISON BUT FAILED. \n{E.Message}"); }
                            else if (role == "staff") { staffLog.Fatal($"USER ACCOUNT WAS DISABLED BY MATT MIS IN PAST. TRIED TO REPROVISON BUT FAILED. \n{E.Message}"); }
                            StaffADEntry.Properties["comment"].Value = "SKIP";
                        }

                    }
                    //MATTMIS HAS NOT ADDED A COMMENT SAYING WE DISABLED IT. SO ASSUME IT WAS DONE INTENTIONALLY AND LEAVE.
                    else
                    {
                        if (role == "student") { studentLog.Error($"USER ACCOUNT DISABLED MANUALLY! WILL NOT BE RE-PROVISONED"); }
                        else if (role == "staff") { staffLog.Error($"USER ACCOUNT DISABLED MANUALLY! WILL NOT BE RE-PROVISONED"); }
                        StaffADEntry.Properties["comment"].Value = "SKIP";
                    }

                }
                //ACCOUNT MATCHED AND ALL LOOKS GOOD
                else
                {
                    StaffADEntry.Properties["comment"].Value = "Matched to Single User";
                }

            }
            else if (userResults.Count == 0) //Staff Member does not exist with this ID. Lets see if I should make them
            {
                StaffADEntry.Properties["comment"].Value = "Not Found in AD";
                if (role == "student") { studentLog.Error($"ERROR: COULD NOT FIND USERS AD ACCOUNT"); }
                else if (role == "staff") { staffLog.Error($"ERROR: COULD NOT FIND USERS AD ACCOUNT"); }
            }
            else //ERM this is strange - multiple users found with this name.
            {
                StaffADEntry.Properties["comment"].Value = "Multple Users Found";
                if (role == "student") { studentLog.Error($"ERROR: SIMS ID HAS MATCHED MULTPLE AD ACCOUNTS. PLEASE CORRECT THIS."); }
                else if (role == "staff") { staffLog.Error($"ERROR: SIMS ID HAS MATCHED MULTPLE AD ACCOUNTS. PLEASE CORRECT THIS."); }
            }

            return StaffADEntry;
        }



    }


}
