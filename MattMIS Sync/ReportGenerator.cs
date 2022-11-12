
using BetterConsoleTables;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;


namespace MattMIS_Sync
{
    public static class ReportGenerator
    {
        //STATS FOR MAILER
        public static int studentOnRollCount = 0;
        public static int studentAccountsMatchedCount = 0;
        public static int studentAccountsCreatedCount = 0;
        public static int studentAccountsDeprovisionedCount = 0;
        public static int studentAccountsDeprovisionExcludedCount = 0;

        //Staff Job Role and Student Stats
        public static List<String> staffJobRoles = new List<String>();
        public static List<String> studentFormGroups = new List<String>();

        //NEW ACCOUNT PASSWORDS
        public static Dictionary<DirectoryEntry, string> NewlyCreatedAccounts = new Dictionary<DirectoryEntry, string>();

        //SIMS ADMIN WARNINGS
        public static Dictionary<StaffModel.StaffMISRecord, string> SIMSAdminWarnings = new Dictionary<StaffModel.StaffMISRecord, string>();

        //NEWLY DISABLED ACCOUNTS
        public static List<DirectoryEntry> NewlyDisabledAccounts = new List<DirectoryEntry>();


        //STUDENTS
        public static int staffOnRollCount = 0;
        public static int staffAccountsMatchedCount = 0;
        public static int staffAccountsCreatedCount = 0;
        public static int staffAccountsDeprovisionedCount = 0;
        public static int staffAccountsDeprovisionExcludedCount = 0;


        public static string YearGroupMappingExpiration = "";

        //ACCOUNT ISSUES
        public static string[] StaffMissingEmail;
        public static string[] StaffWithNoJobRole;

        public static void SendReportEmail()
        {

            String SendMailSubject = "MattMIS Sync: Overnight Account Results";
            String SendMailBody =
            $@"Dear Sir/Madam,
            
MattMIS Sync has processed your schools SIMS data overnight with the following results:

Students - {studentOnRollCount} currently enrolled:

	• {studentAccountsMatchedCount} student accounts matched.
        • {studentAccountsCreatedCount} student accounts created/reprovisioned.
	• {studentAccountsDeprovisionedCount} student accounts disabled/deprovisioned.	
	• {studentAccountsDeprovisionExcludedCount} student accounts were excluded from deprovision due to a local rule.


Staff - {staffOnRollCount} currently in active employment: 

	• {staffAccountsMatchedCount} staff accounts matched.
	• {staffAccountsCreatedCount} staff accounts created/reprovisioned.
	• {staffAccountsDeprovisionedCount} staff accounts disabled/deprovisioned.
	• {staffAccountsDeprovisionExcludedCount} staff accounts were excluded from deprovision due to a local rule.

To view further information regarding this synchronisation and resolve any issues, please refer to the synchronisation logs on your domain controller.

NOTE: YOUR SCHOOLS YEARGROUP ALLOCATION FILE WILL EXPIRE ON THE FOLLOWING DATE: {YearGroupMappingExpiration}.
Please set up your schools yeargroup provising file for the next academic year ahead of time to avoid delays provisioning new students.

Mapping file located at: {Config.AppConfig.AccountProvisioning.StudentSettingsPath}

Thank you.

MattMIS Sync Auto Mailer.
";

            //SAVE FILE SYSTEM REPORTS
            File.WriteAllText($"Reports\\Sync Report {DateTime.Now.ToString("dd MMMM yyyy")}.txt", SendMailBody);

            try
            {
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com", 587);
                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                MailMessage email = new MailMessage();
                // START
                email.From = new MailAddress(Config.AppConfig.Reporting.EmailAlerts.SendFromAddress);
                foreach (string s in Config.AppConfig.Reporting.EmailAlerts.SendTo) { email.Bcc.Add(s); }
                email.CC.Add(Config.AppConfig.Reporting.EmailAlerts.SendFromAddress);
                email.Subject = SendMailSubject;
                email.Body = SendMailBody;

                //ADD REPORTS AS ATTACHMENTS AND SAVE LOCALLY IF THEY EXIST
                if (GenerateNewAccountTable() != null)
                {
                    File.WriteAllText($"Reports\\Created Accounts Report {DateTime.Now.ToString("dd MMMM yyyy")}.txt", GenerateNewAccountTable());
                    email.Attachments.Add(new Attachment($"Reports\\Created Accounts Report {DateTime.Now.ToString("dd MMMM yyyy")}.txt"));
                }

                //ADD REPORTS AS ATTACHMENTS AND SAVE LOCALLY IF THEY EXIST
                if (GenerateSIMSAdminWarningsTable() != null)
                {
                    File.WriteAllText($"Reports\\SIMS Admin Report {DateTime.Now.ToString("dd MMMM yyyy")}.txt", GenerateSIMSAdminWarningsTable());
                    email.Attachments.Add(new Attachment($"Reports\\SIMS Admin Report {DateTime.Now.ToString("dd MMMM yyyy")}.txt"));
                }

                //ADD REPORTS AS ATTACHMENTS AND SAVE LOCALLY
                if (GenerateDisabledAccountTable() != null)
                {
                    File.WriteAllText($"Reports\\Disabled Accounts Report {DateTime.Now.ToString("dd MMMM yyyy")}.txt", GenerateDisabledAccountTable());
                    email.Attachments.Add(new Attachment($"Reports\\Disabled Accounts Report {DateTime.Now.ToString("dd MMMM yyyy")}.txt"));
                }

                if (Config.AppConfig.Reporting.EmailAlerts.Enabled)
                {
                    //END
                    SmtpServer.Timeout = 10000;
                    SmtpServer.EnableSsl = true;
                    SmtpServer.UseDefaultCredentials = false;
                    SmtpServer.Credentials = new NetworkCredential(Config.AppConfig.Reporting.EmailAlerts.SendFromAddress, Config.AppConfig.Reporting.EmailAlerts.GmailAppKey);
                    SmtpServer.Send(email);

                    Console.WriteLine("Email Successfully Sent");
                }


            }
            catch (Exception ex)
            {
                File.WriteAllText("Logs//Email Report Error - Latest.txt", $"{ex.Message}\n{ex.StackTrace}\n{ex.InnerException}");
            }

        }

        //NOT USED
        public static void SendSIMSAdminEmail(string[] SendTo, string SendMailFrom)
        {

            String SendMailSubject = "MattMIS Sync: Results for SIMS Administrator";
            String SendMailBody =
            $@"Dear Sir/Madam,
            
MattMIS Sync has processed your schools SIMS data overnight with the following results:

Students - {studentOnRollCount} currently enrolled:

	• {studentAccountsMatchedCount} student accounts matched.
	• {studentAccountsDeprovisionedCount} student accounts disabled/deprovisioned.
	• {studentAccountsCreatedCount} student accounts created/reprovisioned.
	• {studentAccountsDeprovisionExcludedCount} student accounts were excluded from deprovision due to a local rule.


Staff - {staffOnRollCount} currently in active employment: 

	• {staffAccountsMatchedCount} staff accounts matched.
	• {staffAccountsDeprovisionedCount} staff accounts disabled/deprovisioned.
	• {staffAccountsCreatedCount} staff accounts created/reprovisioned.
	• {staffAccountsDeprovisionExcludedCount} staff accounts were excluded from deprovision due to a local rule.

To view further information regarding this synchronisation and resolve any issues, please refer to the synchronisation logs on your domain controller.

NOTE: YOUR SCHOOLS YEARGROUP ALLOCATION FILE WILL EXPIRE ON THE FOLLOWING DATE: {YearGroupMappingExpiration}.
Please set up your schools yeargroup provising file for the next academic year ahead of time to avoid delays provisioning new students.

Mapping file located at: {Config.AppConfig.AccountProvisioning.StudentSettingsPath}

Thank you.

MattMIS Sync Auto Mailer.


";



            File.WriteAllText($"Reports\\SIMS Admin Report {DateTime.Now.ToString("dd MMMM yyyy")}.txt", SendMailBody);

            try
            {
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com", 587);
                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                MailMessage email = new MailMessage();
                // START
                email.From = new MailAddress(SendMailFrom);
                foreach (string s in SendTo) { email.To.Add(s); }
                email.CC.Add(SendMailFrom);
                email.Subject = SendMailSubject;
                email.Body = SendMailBody;
                //END
                SmtpServer.Timeout = 5000;
                SmtpServer.EnableSsl = true;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new NetworkCredential(SendMailFrom, Config.AppConfig.Reporting.EmailAlerts.GmailAppKey);
                //SmtpServer.Send(email);

                Console.WriteLine("Email Successfully Sent");

            }
            catch (Exception ex)
            {
                File.WriteAllText("errors/emailsimserror.txt", $"{ex.Message}\n{ex.StackTrace}\n{ex.InnerException}");
            }



        }

        public static void GenerateStatisticsFiles()
        {
            var duplicates = staffJobRoles.GroupBy(x => x)
                       .Select(y => new
                       {
                           Name = y.Key,
                           Count = y.Count()
                       }).ToList();

            StreamWriter streamWriter = new StreamWriter("Reports//Staff Job Titles.txt");
            foreach (var i in duplicates) { streamWriter.WriteLine(i.ToString()); }              
            streamWriter.Close();

            duplicates = studentFormGroups.GroupBy(x => x)
                       .Select(y => new
                       {
                           Name = y.Key,
                           Count = y.Count()
                       }).ToList();

            streamWriter = new StreamWriter("Reports//Student Form Groups.txt");
            foreach (var i in duplicates) { streamWriter.WriteLine(i.ToString()); }
            streamWriter.Close();
        }


        public static string GenerateNewAccountTable()
        {
            if (NewlyCreatedAccounts.Keys.Count == 0) { return null; }

            Table table = new Table("Full Name", "Role", "Primary Email", "Password");

            foreach (DirectoryEntry de in NewlyCreatedAccounts.Keys)
            {
                table.AddRow(de.Properties["DisplayName"].Value ?? "EMPTY", de.Properties["Department"].Value ?? "EMPTY", de.Properties["userPrincipalName"].Value ?? "EMPTY", NewlyCreatedAccounts[de] ?? "EMPTY");
            }
            return table.ToString();
        }
        private static string GenerateDisabledAccountTable()
        {
            if (NewlyDisabledAccounts.Count == 0) { return null; }
            Table table = new Table("Full Name", "Role", "Primary Email");

            foreach (DirectoryEntry de in NewlyDisabledAccounts)
            {
                table.AddRow(de.Properties["DisplayName"].Value ?? "EMPTY", de.Properties["Department"].Value ?? "EMPTY", de.Properties["userPrincipalName"].Value ?? "EMPTY");

            }
            return table.ToString();
        }
        private static string GenerateSIMSAdminWarningsTable()
        {
            if (SIMSAdminWarnings.Keys.Count == 0) { return null; }
            Table table = new Table("Full Name", "Role", "Primary Email", "Message");
            foreach (StaffModel.StaffMISRecord de in SIMSAdminWarnings.Keys)
            {
                table.AddRow(de.FullName ?? "EMPTY", de.Role ?? "EMPTY", de.WorkEmail ?? "EMPTY", SIMSAdminWarnings[de] ?? "NONE");

            }
            return table.ToString();

        }
    }
}
