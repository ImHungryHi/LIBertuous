using SharepointWorkflow.Common;
using SharepointWorkflow.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace SharepointWorkflow.Logic
{
    class LogicControl
    {
        private List<ConfigItem> configItems;
        private List<Item> items;
        private ErrorDelegate errorDelegate;
        private SharepointDbDataContext data;
        private string projectUrl;
        private List<string> isoDocumentUrls;

        // No standard constructor needed.

        /// <summary>
        /// Standard constructor
        /// Initializes all fields in the class.
        /// </summary>
        /// <param name="errorDelegate">Delegated update method</param>
        public LogicControl(ErrorDelegate errorDelegate)
        {
            // Initialize all fields
            this.errorDelegate = errorDelegate;
            projectUrl = "";
            isoDocumentUrls = new List<string>();
            //this.data = new SharepointDbDataContext();
        }

        /// <summary>
        /// Creates a new project and site and copies all documents into the new document library.
        /// </summary>
        /// <param name="url">Link to the sharepoint site.</param>
        /// <returns>Whether or not the site was created.</returns>
        public bool CreateProject(string url)
        {
            // Check whether or not the Excel file has been loaded.
            if (items == null || items.Count == 0)
            {
                return false;
            }

            //SharepointClass sharepoint = new SharepointClass();
            SharepointClass sharepoint = new SharepointClass(url, errorDelegate);
            sharepoint.LoadList(PresetConfig.MasterProjectList);
            List<string> resources = new List<string>();    // We'll parse this to an array afterwards.
            List<List<double>> monthlyPercentages = new List<List<double>>();

            // Query the items list containing all necessary info an store the information in the appropriate variable.
            string title = (from i in items
                            where i.ConfigItem.Variable.Equals(PresetConfig.ProjectTitle)
                            select i.Values.First().First()).First<string>();
            string code = GetProjectCode();
            string client = (from i in items
                             where i.ConfigItem.Variable.Equals(PresetConfig.ClientName)
                             select i.Values.First().First()).First<string>();
            string responsible = (from i in items
                                  where i.ConfigItem.Variable.Equals(PresetConfig.InternalEndResponsibleName)
                                  select i.Values.First().First()).First<string>();
            string entity = (from i in items
                             where i.ConfigItem.Variable.Equals(PresetConfig.Entity)
                             select i.Values.First().First()).First<string>();
            string description = (from i in items
                                  where i.ConfigItem.Variable.Equals(PresetConfig.Description)
                                  select i.Values.First().First()).First<string>();
            string budget = (from i in items
                             where i.ConfigItem.Variable.Equals(PresetConfig.Budget)
                             select i.Values.First().First()).First<string>();
            List<List<string>> resourceList = (from i in items
                                               where i.ConfigItem.Variable.Equals(PresetConfig.ResourceNames)
                                               select i.Values).First<List<List<string>>>();
            string sector = (from i in items
                             where i.ConfigItem.Variable.Equals(PresetConfig.Sector)
                             select i.Values.First().First()).First<string>();
            DateTime startDate = Convert.ToDateTime((from i in items
                                                     where i.ConfigItem.Variable.Equals(PresetConfig.StartDate)
                                                     select i.Values.First().First()).First<string>());
            DateTime endDate = Convert.ToDateTime((from i in items
                                                   where i.ConfigItem.Variable.Equals(PresetConfig.EndDate)
                                                   select i.Values.First().First()).First<string>());
            List<List<string>> percentageStrings = (from i in items
                                                    where i.ConfigItem.Variable.Equals(PresetConfig.ResourceMonthlyDays)
                                                    select i.Values).First<List<List<string>>>();

            // Parse the resource list into an array, each list inside the list contains only 1 value because there's only 1 column
            foreach (List<string> sublist in resourceList)
            {
                foreach (string resource in sublist)
                {
                    // Filter out the empty ones.
                    if (!resource.Equals(""))
                    {
                        resources.Add(resource);
                    }
                }
            }

            var numberFormat = (System.Globalization.NumberFormatInfo)System.Globalization.CultureInfo.InstalledUICulture.NumberFormat.Clone();
            numberFormat.NumberDecimalSeparator = ".";

            // All rows and columns are of equal length in their own dimensions.
            for (int x = 0; x < percentageStrings.First().Count; x++)
            {
                List<double> personalPercentages = new List<double>();

                for (int y = 0; y < percentageStrings.Count; y++)
                {
                    if (percentageStrings[y][x] != null && !percentageStrings[y][x].Equals(""))
                    {
                        personalPercentages.Add(Convert.ToDouble(percentageStrings[y][x], numberFormat));
                    }
                    else
                    {
                        personalPercentages.Add(0);
                    }
                }

                monthlyPercentages.Add(personalPercentages);
            }

            monthlyPercentages = SharePointHelper.CalculatePercentages(monthlyPercentages);

            // Check for minimum requirements.
            if (code == null || code.Equals(""))
            {
                return false;
            }

            if (!sharepoint.AddProject(PresetConfig.MasterProjectList, title, code, client, entity, sector, resources.ToArray<string>(), PresetConfig.Owners, (PresetConfig.Months[startDate.Month - 1] + " " + startDate.Year + " - " + PresetConfig.Months[endDate.Month - 1] + " " + endDate.Year), startDate, endDate, description, ((budget == null || budget.Equals("")) ? 0 : double.Parse(budget))))
            {
                return false;
            }

            RepliconResourceWriter writer = new RepliconResourceWriter();
            writer.Write(code, title, startDate, endDate, resources, monthlyPercentages, description, ProjectStatus.InProgress, Billing.Billable, SharePointHelper.GetUsername(responsible), Path.Combine(PresetConfig.ShareLocation, code + ".csv"));

            // If the program was able to get this far, all is good.
            return true;
        }

        public bool Test(int pafProjectId)
        {
            SharepointClass sharepoint = new SharepointClass(errorDelegate);

            if (!sharepoint.ReadPAFInfo(pafProjectId))
            {
                return false;
            }

            items = sharepoint.GetPAFData();
            List<string> resources = new List<string>();    // We'll parse this to an array afterwards.
            List<List<double>> monthlyPercentages = new List<List<double>>();

            if (items.Count == 0)
            {
                return false;
            }

            // Query the items list containing all necessary info an store the information in the appropriate variable.
            string title = (from i in items
                            where i.ConfigItem.Variable.Equals(PresetConfig.ProjectTitle)
                            select i.Values.First().First()).First<string>();
            string code = GetProjectCode();
            string client = (from i in items
                             where i.ConfigItem.Variable.Equals(PresetConfig.ClientName)
                             select i.Values.First().First()).First<string>();
            string responsible = (from i in items
                                  where i.ConfigItem.Variable.Equals(PresetConfig.InternalEndResponsibleName)
                                  select i.Values.First().First()).First<string>();
            string entity = (from i in items
                             where i.ConfigItem.Variable.Equals(PresetConfig.Entity)
                             select i.Values.First().First()).First<string>();
            string description = (from i in items
                                  where i.ConfigItem.Variable.Equals(PresetConfig.Description)
                                  select i.Values.First().First()).First<string>();
            string budget = (from i in items
                             where i.ConfigItem.Variable.Equals(PresetConfig.Budget)
                             select i.Values.First().First()).First<string>();
            List<List<string>> resourceList = (from i in items
                                               where i.ConfigItem.Variable.Equals(PresetConfig.ResourceNames)
                                               select i.Values).First<List<List<string>>>();
            string sector = (from i in items
                             where i.ConfigItem.Variable.Equals(PresetConfig.Sector)
                             select i.Values.First().First()).First<string>();
            DateTime startDate = Convert.ToDateTime((from i in items
                                                     where i.ConfigItem.Variable.Equals(PresetConfig.StartDate)
                                                     select i.Values.First().First()).First<string>());
            DateTime endDate = Convert.ToDateTime((from i in items
                                                   where i.ConfigItem.Variable.Equals(PresetConfig.EndDate)
                                                   select i.Values.First().First()).First<string>());
            List<List<string>> percentageStrings = (from i in items
                                                    where i.ConfigItem.Variable.Equals(PresetConfig.ResourceMonthlyDays)
                                                    select i.Values).First<List<List<string>>>();

            // Parse the resource list into an array, each list inside the list contains only 1 value because there's only 1 column
            foreach (List<string> sublist in resourceList)
            {
                foreach (string resource in sublist)
                {
                    // Filter out the empty ones.
                    if (!resource.Equals(""))
                    {
                        resources.Add(resource);
                    }
                }
            }

            var numberFormat = (System.Globalization.NumberFormatInfo)System.Globalization.CultureInfo.InstalledUICulture.NumberFormat.Clone();
            numberFormat.NumberDecimalSeparator = ".";

            // All rows and columns are of equal length in their own dimensions.
            for (int x = 0; x < percentageStrings.First().Count; x++)
            {
                List<double> personalPercentages = new List<double>();

                for (int y = 0; y < percentageStrings.Count; y++)
                {
                    if (percentageStrings[y][x] != null && !percentageStrings[y][x].Equals(""))
                    {
                        personalPercentages.Add(Convert.ToDouble(percentageStrings[y][x], numberFormat));
                    }
                    else
                    {
                        personalPercentages.Add(0);
                    }
                }

                monthlyPercentages.Add(personalPercentages);
            }

            monthlyPercentages = SharePointHelper.CalculatePercentages(monthlyPercentages);

            // Check for minimum requirements.
            if (code == null || code.Equals(""))
            {
                errorDelegate("Can't create a project without a project code.\nPlease fill in a project code and try again.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Creates a new project and site based on a PAF Project List item and its linked PAF Resources.
        /// </summary>
        /// <param name="pafProjectId">PAF Project identification number.</param>
        /// <returns>Whether or not the site was created.</returns>
        public bool CreateProjectFromList(int pafProjectId, OpenFileDelegate openFileDelegate, SaveDelegate saveDelegate, CopyDelegate copyDelegate)
        {
            SharepointClass sharepoint = new SharepointClass(errorDelegate);
            DateTime startDate, endDate;

            if (!sharepoint.ReadPAFInfo(pafProjectId))
            {
                return false;
            }

            items = sharepoint.GetPAFData();
            List<string> resources = new List<string>();    // We'll parse this to an array afterwards.
            List<List<double>> monthlyPercentages = new List<List<double>>();

            if (items.Count == 0)
            {
                return false;
            }

            // Query the items list containing all necessary info an store the information in the appropriate variable.
            string title = (from i in items
                            where i.ConfigItem.Variable.Equals(PresetConfig.ProjectTitle)
                            select i.Values.First().First()).First<string>();
            string code = GetProjectCode();
            string client = (from i in items
                             where i.ConfigItem.Variable.Equals(PresetConfig.ClientName)
                             select i.Values.First().First()).First<string>();
            string responsible = (from i in items
                                  where i.ConfigItem.Variable.Equals(PresetConfig.InternalEndResponsibleName)
                                  select i.Values.First().First()).First<string>();
            string entity = (from i in items
                             where i.ConfigItem.Variable.Equals(PresetConfig.Entity)
                             select i.Values.First().First()).First<string>();
            string description = (from i in items
                                  where i.ConfigItem.Variable.Equals(PresetConfig.Description)
                                  select i.Values.First().First()).First<string>();
            string budget = (from i in items
                             where i.ConfigItem.Variable.Equals(PresetConfig.Budget)
                             select i.Values.First().First()).First<string>();
            List<List<string>> resourceList = (from i in items
                                               where i.ConfigItem.Variable.Equals(PresetConfig.ResourceNames)
                                               select i.Values).First<List<List<string>>>();
            string sector = (from i in items
                             where i.ConfigItem.Variable.Equals(PresetConfig.Sector)
                             select i.Values.First().First()).First<string>();

            // Parse the dates into a correct syntax.
            try
            {
                string tempDate = items.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.StartDate)).First().Values.First().First();
                startDate = ((tempDate != null && !tempDate.Equals("")) ? Convert.ToDateTime(tempDate) : DateTime.MinValue);

                if (startDate.Hour >= 20)
                {
                    startDate = new DateTime(startDate.Year, startDate.Month, (startDate.Day + 1), 0, 0, 0);
                }
            }
            catch (Exception ex)
            {
                startDate = DateTime.MinValue;
            }

            try
            {
                string tempDate = items.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.EndDate)).First().Values.First().First();
                endDate = ((tempDate != null && !tempDate.Equals("")) ? Convert.ToDateTime(tempDate) : DateTime.MinValue);

                if (endDate.Hour >= 20)
                {
                    endDate = new DateTime(endDate.Year, endDate.Month, (endDate.Day + 1), 0, 0, 0);
                }
            }
            catch (Exception ex)
            {
                endDate = DateTime.MinValue;
            }

            List<List<string>> percentageStrings = (from i in items
                                                    where i.ConfigItem.Variable.Equals(PresetConfig.ResourceMonthlyDays)
                                                    select i.Values).First<List<List<string>>>();

            // Parse the resource list into an array, each list inside the list contains only 1 value because there's only 1 column
            foreach (List<string> sublist in resourceList)
            {
                foreach (string resource in sublist)
                {
                    // Filter out the empty ones.
                    if (!resource.Equals(""))
                    {
                        resources.Add(resource);
                    }
                }
            }

            var numberFormat = (System.Globalization.NumberFormatInfo)System.Globalization.CultureInfo.InstalledUICulture.NumberFormat.Clone();
            numberFormat.NumberDecimalSeparator = ".";

            // All rows and columns are of equal length in their own dimensions.
            for (int x = 0; x < percentageStrings.First().Count; x++)
            {
                List<double> personalPercentages = new List<double>();

                for (int y = 0; y < percentageStrings.Count; y++)
                {
                    if (percentageStrings[y][x] != null && !percentageStrings[y][x].Equals(""))
                    {
                        personalPercentages.Add(Convert.ToDouble(percentageStrings[y][x], numberFormat));
                    }
                    else
                    {
                        personalPercentages.Add(0);
                    }
                }

                monthlyPercentages.Add(personalPercentages);
            }

            monthlyPercentages = SharePointHelper.CalculatePercentages(monthlyPercentages);

            // Check for minimum requirements.
            if (code == null || code.Equals(""))
            {
                errorDelegate("Can't create a project without a project code.\nPlease fill in a project code and try again.");
                return false;
            }

            // Load the master project into the sharepoint class.
            sharepoint.LoadList(PresetConfig.MasterProjectList);

            if (!sharepoint.AddProject(PresetConfig.MasterProjectList, title, code, client, entity, sector, resources.ToArray<string>(), PresetConfig.Owners, (PresetConfig.Months[startDate.Month - 1] + " " + startDate.Year + " - " + PresetConfig.Months[endDate.Month - 1] + " " + endDate.Year), startDate, endDate, description, ((budget == null || budget.Equals("")) ? 0 : Convert.ToDouble(budget))))
            {
                return false;
            }

            projectUrl = PresetConfig.DefaultProjectUrl + "/" + code;
            string srcTemplate = PresetConfig.DefaultISOUrl + "/" + PresetConfig.ISOSourceRootFolder.Replace(" ", "%20") + "/";

            try
            {
                switch (items.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.Language)).First().Values.First().First())
                {
                    case "Nederlands":
                        srcTemplate += PresetConfig.DutchReportTemplate;
                        break;
                    case "Français":
                        srcTemplate += PresetConfig.FrenchReportTemplate;
                        break;
                    default:
                        srcTemplate += PresetConfig.EnglishReportTemplate;
                        break;
                }
            }
            catch (Exception ex)
            {
                srcTemplate += PresetConfig.EnglishReportTemplate;
            }

            isoDocumentUrls = sharepoint.WriteISOFiles(code, items, projectUrl, PresetConfig.ISODestinationFolder, code + ".docx", openFileDelegate(srcTemplate), saveDelegate);

            // Try copying all attachments to the ISO destination folder.
            try
            {
                if (items.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.Attachments)).First() != null)
                {
                    foreach (List<string> sublist in items.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.Attachments)).First().Values)
                    {
                        foreach (string attachmentUrl in sublist)
                        {
                            try
                            {
                                copyDelegate(attachmentUrl, PresetConfig.DefaultProjectUrl + "/" + code, PresetConfig.ISODestinationFolder, attachmentUrl.Split('/').Last());
                            }
                            catch (Exception ex)
                            {
                                // Do nothing.
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Do nothing.
            }

            RepliconResourceWriter writer = new RepliconResourceWriter();
            writer.Write(code, title, startDate, endDate, resources, monthlyPercentages, description, ProjectStatus.InProgress, Billing.Billable, SharePointHelper.GetUsername(responsible), Path.Combine(PresetConfig.ShareLocation, code + ".csv"));

            // If the program was able to get this far, all is good.
            return true;
        }

        /// <summary>
        /// Updates a project and site based on a PAF Project List item and its linked PAF Resources.
        /// </summary>
        /// <param name="pafProjectId">PAF Project identification number.</param>
        /// <returns>Whether or not the site was updated.</returns>
        public bool EditProjectFromList(int pafProjectId, OpenFileDelegate openFileDelegate, SaveDelegate saveDelegate, CopyDelegate copyDelegate)
        {
            SharepointClass sharepoint = new SharepointClass(errorDelegate);
            DateTime startDate, endDate;

            if (!sharepoint.ReadPAFInfo(pafProjectId))
            {
                return false;
            }

            items = sharepoint.GetPAFData();
            List<string> resources = new List<string>();    // We'll parse this to an array afterwards.
            List<List<double>> monthlyPercentages = new List<List<double>>();

            if (items.Count == 0)
            {
                return false;
            }

            // Query the items list containing all necessary info an store the information in the appropriate variable.
            string title = (from i in items
                            where i.ConfigItem.Variable.Equals(PresetConfig.ProjectTitle)
                            select i.Values.First().First()).First<string>();
            string code = GetProjectCode();
            string client = (from i in items
                             where i.ConfigItem.Variable.Equals(PresetConfig.ClientName)
                             select i.Values.First().First()).First<string>();
            string responsible = (from i in items
                                  where i.ConfigItem.Variable.Equals(PresetConfig.InternalEndResponsibleName)
                                  select i.Values.First().First()).First<string>();
            string entity = (from i in items
                             where i.ConfigItem.Variable.Equals(PresetConfig.Entity)
                             select i.Values.First().First()).First<string>();
            string description = (from i in items
                                  where i.ConfigItem.Variable.Equals(PresetConfig.Description)
                                  select i.Values.First().First()).First<string>();
            string budget = (from i in items
                             where i.ConfigItem.Variable.Equals(PresetConfig.Budget)
                             select i.Values.First().First()).First<string>();
            List<List<string>> resourceList = (from i in items
                                               where i.ConfigItem.Variable.Equals(PresetConfig.ResourceNames)
                                               select i.Values).First<List<List<string>>>();
            string sector = (from i in items
                             where i.ConfigItem.Variable.Equals(PresetConfig.Sector)
                             select i.Values.First().First()).First<string>();

            // Parse the dates into a correct syntax.
            try
            {
                string tempDate = items.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.StartDate)).First().Values.First().First();
                startDate = ((tempDate != null && !tempDate.Equals("")) ? Convert.ToDateTime(tempDate) : DateTime.MinValue);

                if (startDate.Hour >= 20)
                {
                    startDate = new DateTime(startDate.Year, startDate.Month, (startDate.Day + 1), 0, 0, 0);
                }
            }
            catch (Exception ex)
            {
                startDate = DateTime.MinValue;
            }

            try
            {
                string tempDate = items.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.EndDate)).First().Values.First().First();
                endDate = ((tempDate != null && !tempDate.Equals("")) ? Convert.ToDateTime(tempDate) : DateTime.MinValue);

                if (endDate.Hour >= 20)
                {
                    endDate = new DateTime(endDate.Year, endDate.Month, (endDate.Day + 1), 0, 0, 0);
                }
            }
            catch (Exception ex)
            {
                endDate = DateTime.MinValue;
            }

            List<List<string>> percentageStrings = (from i in items
                                                    where i.ConfigItem.Variable.Equals(PresetConfig.ResourceMonthlyDays)
                                                    select i.Values).First<List<List<string>>>();

            // Parse the resource list into an array, each list inside the list contains only 1 value because there's only 1 column
            foreach (List<string> sublist in resourceList)
            {
                foreach (string resource in sublist)
                {
                    // Filter out the empty ones.
                    if (!resource.Equals(""))
                    {
                        resources.Add(resource);
                    }
                }
            }

            var numberFormat = (System.Globalization.NumberFormatInfo)System.Globalization.CultureInfo.InstalledUICulture.NumberFormat.Clone();
            numberFormat.NumberDecimalSeparator = ".";

            // All rows and columns are of equal length in their own dimensions.
            for (int x = 0; x < percentageStrings.First().Count; x++)
            {
                List<double> personalPercentages = new List<double>();

                for (int y = 0; y < percentageStrings.Count; y++)
                {
                    if (percentageStrings[y][x] != null && !percentageStrings[y][x].Equals(""))
                    {
                        personalPercentages.Add(Convert.ToDouble(percentageStrings[y][x], numberFormat));
                    }
                    else
                    {
                        personalPercentages.Add(0);
                    }
                }

                monthlyPercentages.Add(personalPercentages);
            }

            monthlyPercentages = SharePointHelper.CalculatePercentages(monthlyPercentages);

            // Check for minimum requirements.
            if (code == null || code.Equals(""))
            {
                errorDelegate("Can't update a project without a project code.\nPlease fill in a project code and try again.");
                return false;
            }

            // Load the master project into the sharepoint class.
            sharepoint.LoadList(PresetConfig.MasterProjectList);

            if (!sharepoint.UpdateProject(PresetConfig.MasterProjectList, title, code, client, entity, sector, resources.ToArray<string>(), PresetConfig.Owners, "", startDate, endDate, description, ((budget == null || budget.Equals("")) ? 0 : Convert.ToDouble(budget))))
            {
                return false;
            }

            // Load the correct report template.
            projectUrl = PresetConfig.DefaultProjectUrl + "/" + code;
            string srcTemplate = PresetConfig.DefaultISOUrl + "/" + PresetConfig.ISOSourceRootFolder.Replace(" ", "%20") + "/";

            try
            {
                switch (items.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.Language)).First().Values.First().First())
                {
                    case "Nederlands":
                        srcTemplate += PresetConfig.DutchReportTemplate;
                        break;
                    case "Français":
                        srcTemplate += PresetConfig.FrenchReportTemplate;
                        break;
                    default:
                        srcTemplate += PresetConfig.EnglishReportTemplate;
                        break;
                }
            }
            catch (Exception ex)
            {
                srcTemplate += PresetConfig.EnglishReportTemplate;
            }

            isoDocumentUrls = sharepoint.WriteISOFiles(code, items, projectUrl, PresetConfig.ISODestinationFolder, code + ".docx", openFileDelegate(srcTemplate), saveDelegate);

            // Try copying all attachments to the ISO destination folder.
            try
            {
                if (items.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.Attachments)).First() != null)
                {
                    foreach (List<string> sublist in items.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.Attachments)).First().Values)
                    {
                        foreach (string attachmentUrl in sublist)
                        {
                            try
                            {
                                copyDelegate(attachmentUrl, PresetConfig.DefaultProjectUrl + "/" + code, PresetConfig.ISODestinationFolder, attachmentUrl.Split('/').Last());
                            }
                            catch (Exception ex)
                            {
                                // Do nothing.
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Do nothing.
            }

            RepliconResourceWriter writer = new RepliconResourceWriter();
            writer.Write(code, title, startDate, endDate, resources, monthlyPercentages, description, ProjectStatus.InProgress, Billing.Billable, SharePointHelper.GetUsername(responsible), Path.Combine(PresetConfig.ShareLocation, code + ".csv"));

            // If the program was able to get this far, all is good.
            return true;
        }

        /// <summary>
        /// Upgrades the member permissions group from having contribute rights to full control.
        /// </summary>
        public void UpgradeMemberPermissions()
        {
            SharepointClass sharepoint = new SharepointClass(errorDelegate);
            sharepoint.UpgradeMemberPermissions();
        }

        /// <summary>
        /// Load all data from a specified Excel file.
        /// </summary>
        /// <param name="path">Path to the Excel file</param>
        public void LoadExcel(string path)
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });

            try
            {
                // Make preparations to create a temporary file on the server.
                string share = PresetConfig.ShareLocation;
                string tempPath = Path.Combine(share, Path.GetFileName(path));

                // Make authorized requests to be able to read the file from the database.
                WebRequest request = WebRequest.Create(path);
                request.Credentials = CredentialCache.DefaultCredentials;
                WebResponse response = request.GetResponse();

                // Transfer the file from the database to the server.
                using (Stream stream = response.GetResponseStream())
                using (FileStream fsWorkbook = System.IO.File.Open(tempPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    byte[] buffer = new byte[1024];
                    int length = stream.Read(buffer, 0, buffer.Length);

                    while (length > 0)
                    {
                        fsWorkbook.Write(buffer, 0, length);
                        length = stream.Read(buffer, 0, buffer.Length);
                    }
                }

                ExcelReader reader = new ExcelReader(tempPath, errorDelegate);  // Create an ExcelReader object using the path parameter.
                reader.Read(configItems);                                       // Read all data from the Excel file.
                items = reader.GetItems();

                File.Delete(tempPath);                                          // Delete the temporary file from the server.
            }
            catch (Exception ex)
            {
                errorDelegate("An error has occurred while reading the Excel file." + Environment.NewLine + ex.Message + Environment.NewLine + ex.TargetSite);
            }
        }

        /// <summary>
        /// Writes all data to the database, or at least calls the correct methods to do so.
        /// </summary>
        public void WriteToDatabase()
        {
            // If the items have not been filled, there is nothing to write to the database.
            if (items == null || items.Count == 0)
            {
                return;
            }

            // Initialize new objects.
            List<string> resources = new List<string>();
            List<string> roles = new List<string>();
            int counter = 0;

            // Loops through all config items and collects the data into the right containers.
            string title = (from i in items
                            where i.ConfigItem.Variable.Equals(PresetConfig.ProjectTitle)
                            select i.Values.First().First()).First<string>();
            string code = GetProjectCode();
            string client = (from i in items
                             where i.ConfigItem.Variable.Equals(PresetConfig.ClientName)
                             select i.Values.First().First()).First<string>();
            string entity = (from i in items
                             where i.ConfigItem.Variable.Equals(PresetConfig.Entity)
                             select i.Values.First().First()).First<string>();
            string description = (from i in items
                                  where i.ConfigItem.Variable.Equals(PresetConfig.Description)
                                  select i.Values.First().First()).First<string>();
            string summary = (from i in items
                              where i.ConfigItem.Variable.Equals(PresetConfig.Summary)
                              select i.Values.First().First()).First<string>();
            double budget = double.Parse((from i in items
                                          where i.ConfigItem.Variable.Equals(PresetConfig.Budget)
                                          select i.Values.First().First()).First<string>());
            double timeOfCompletion = double.Parse((from i in items
                                                    where i.ConfigItem.Variable.Equals(PresetConfig.TimeOfCompletion)
                                                    select i.Values.First().First()).First<string>());
            List<List<string>> resourceList = (from i in items
                                               where i.ConfigItem.Variable.Equals(PresetConfig.ResourceNames)
                                               select i.Values).First<List<List<string>>>();
            List<List<string>> roleList = (from i in items
                                           where i.ConfigItem.Variable.Equals(PresetConfig.ResourceRoles)
                                           select i.Values).First<List<List<string>>>();
            List<List<string>> rates = (from i in items
                                        where i.ConfigItem.Variable.Equals(PresetConfig.ResourceHourRates)
                                        select i.Values).First<List<List<string>>>();
            List<List<string>> dayList = (from i in items
                                          where i.ConfigItem.Variable.Equals(PresetConfig.ResourceDays)
                                          select i.Values).First<List<List<string>>>();
            string sector = (from i in items
                             where i.ConfigItem.Variable.Equals(PresetConfig.Sector)
                             select i.Values.First().First()).First<string>();
            string externalProjectName = (from i in items
                                          where i.ConfigItem.Equals(PresetConfig.ExternalProjectResponsibleName)
                                          select i.Values.First().First()).First<string>();
            string externalProjectMail = (from i in items
                                          where i.ConfigItem.Equals(PresetConfig.ExternalProjectResponsibleMail)
                                          select i.Values.First().First()).First<string>();
            string externalEndName = (from i in items
                                      where i.ConfigItem.Equals(PresetConfig.ExternalEndResponsibleName)
                                      select i.Values.First().First()).First<string>();
            string externalEndMail = (from i in items
                                      where i.ConfigItem.Equals(PresetConfig.ExternalEndResponsibleMail)
                                      select i.Values.First().First()).First<string>();
            string internalProjectName = (from i in items
                                          where i.ConfigItem.Equals(PresetConfig.InternalProjectResponsibleName)
                                          select i.Values.First().First()).First<string>();
            string internalProjectMail = (from i in items
                                          where i.ConfigItem.Equals(PresetConfig.InternalProjectResponsibleMail)
                                          select i.Values.First().First()).First<string>();
            string internalEndName = (from i in items
                                      where i.ConfigItem.Equals(PresetConfig.InternalEndResponsibleName)
                                      select i.Values.First().First()).First<string>();
            string internalEndMail = (from i in items
                                      where i.ConfigItem.Equals(PresetConfig.InternalEndResponsibleMail)
                                      select i.Values.First().First()).First<string>();

            // Select the dates, parse them into integers and subtract 36525 from the number. (this is the number of days starting from 01-01-2000 instead of 31-12-1899)
            int startDays = int.Parse((from i in items
                                       where i.ConfigItem.Variable.Equals(PresetConfig.StartDate)
                                       select i.Values.First().First()).First<string>()) - 36526;
            int endDays = int.Parse((from i in items
                                     where i.ConfigItem.Variable.Equals(PresetConfig.EndDate)
                                     select i.Values.First().First()).First<string>()) - 36526;

            // Extra check in case the dates go under 01/01/2000.
            if (startDays < 1)
            {
                startDays = 0;
            }

            if (endDays < 1)
            {
                endDays = 0;
            }

            // Convert the integers to DateTime objects.
            DateTime startDate = new DateTime(2000, 1, 1).AddDays(startDays);
            DateTime endDate = new DateTime(2000, 1, 1).AddDays(endDays);

            // Parse the resource list into an array, each list inside the list contains only 1 value because there's only 1 column.
            foreach (List<string> temp in resourceList)
            {
                // Filter out the empty ones.
                if (!temp[0].Equals(""))
                {
                    resources.Add(temp[0]);

                    // Put the checks/adds in here to save a loopthrough of the resources list.
                    if (temp[0].Contains(", "))
                    {
                        string name = temp[0].Split(',')[0];
                        string firstName = temp[0].Split(',')[0].Remove(0, 1);

                        if (!data.InMembers(name, firstName))
                        {
                            data.AddMember(name, firstName, firstName + "." + name + PresetConfig.EmailSuffix);
                        }
                    }
                }
            }

            // Parse the role list into an array, each list inside the list contains only 1 value because there's only 1 column.
            foreach (List<string> temp in roleList)
            {
                // filter out the empty ones.
                if (!temp[0].Equals(""))
                {
                    roles.Add(temp[0]);

                    // Put the checks/adds in here to save a loopthrough of the roles list.
                    if (!data.InRoles(temp[0], double.Parse(rates[counter].ToString())))
                    {
                        data.AddRole(temp[0], double.Parse(rates[counter].ToString()), true);
                    }
                }

                counter++;
            }

            // Perform the rest of the checks/adds.
            if (!data.InSectors(sector))
            {
                data.AddSector(sector);
            }

            if (!data.InClients(client))
            {
                data.AddClient(client, sector);
            }

            if (!data.InEntities(entity))
            {
                data.AddEntity(entity);
            }

            #region ExternalEndResponsible
            // Create new containers for the first and last name.
            string externalEndFirst = "";
            string externalEndLast = "";

            // Execute the format actions.
            if (externalEndName.Contains(", "))
            {
                externalEndLast = externalEndName.Split(',')[0];
                externalEndFirst = externalEndName.Split(',')[1].Remove(0, 1);
            }
            else
            {
                string[] externalEndSplit = externalEndName.Split(' ');

                externalEndFirst = externalEndSplit[0];

                for (int x = 1; x < externalEndSplit.Length; x++)
                {
                    externalEndLast += externalEndSplit[x] + ((x == (externalEndSplit.Length - 1)) ? "" : " ");
                }
            }

            // Do the check the previous commands prepared for.
            if (!data.InExternalContacts(externalEndLast, externalEndFirst))
            {
                if (externalEndMail == null || externalEndMail.Equals(""))
                {
                    externalEndMail = "nomail";
                }

                data.AddExternalContact(externalEndLast, externalEndFirst, externalEndMail, data.GetClientIdByName(client));
            }
            #endregion ExternalEndResponsible

            #region ExternalProjectResponsible
            // Create new containers for the first and last name.
            string externalProjectFirst = "";
            string externalProjectLast = "";

            // Execute the format actions.
            if (externalProjectName.Contains(", "))
            {
                externalProjectLast = externalProjectName.Split(',')[0];
                externalProjectFirst = externalProjectName.Split(',')[1].Remove(0, 1);
            }
            else
            {
                string[] externalProjectSplit = externalProjectName.Split(' ');

                externalProjectFirst = externalProjectSplit[0];

                for (int x = 1; x < externalProjectSplit.Length; x++)
                {
                    externalProjectLast += externalProjectSplit[x] + ((x == (externalProjectSplit.Length - 1)) ? "" : " ");
                }
            }

            // Do the check the previous commands prepared for.
            if (!data.InExternalContacts(externalProjectLast, externalProjectFirst))
            {
                if (externalEndName.Equals(externalProjectName))
                {
                    externalProjectMail = externalEndMail;
                }

                if (externalProjectMail == null || externalProjectMail.Equals(""))
                {
                    externalProjectMail = "nomail";
                }

                data.AddExternalContact(externalProjectLast, externalProjectFirst, externalProjectMail, data.GetClientIdByName(client));
            }
            #endregion ExternalProjectResponsible

            #region InternalEndResponsible
            // Create new containers for the first and last name.
            string internalEndLast = "";
            string internalEndFirst = "";

            // Execute the format actions.
            if (internalEndName.Contains(", "))
            {
                internalEndLast = internalEndName.Split(',')[0];
                internalEndFirst = internalEndName.Split(',')[1].Remove(0, 1);
            }
            else
            {
                string[] internalEndSplit = internalEndName.Split(' ');

                internalEndFirst = internalEndSplit[0];

                for (int x = 1; x < internalEndSplit.Length; x++)
                {
                    internalEndLast += internalEndSplit[x] + ((x == (internalEndSplit.Length - 1)) ? "" : " ");
                }
            }

            // Do the check the previous commands prepared for.
            if (!data.InInternalContacts(data.GetMemberIdByInfo(internalEndLast, internalEndFirst, internalEndMail)))
            {
                data.AddInternalContact(internalEndLast, internalEndFirst, internalEndMail);
            }
            #endregion InternalEndResponsible

            #region InternalProjectResponsible
            // Create new containers for the first and last name.
            string internalProjectLast = "";
            string internalProjectFirst = "";

            // Execute the format actions.
            if (internalProjectName.Contains(", "))
            {
                internalProjectLast = internalProjectName.Split(',')[0];
                internalProjectFirst = internalProjectName.Split(',')[1].Remove(0, 1);
            }
            else
            {
                string[] internalProjectSplit = internalProjectName.Split(' ');

                internalProjectFirst = internalProjectSplit[0];

                for (int x = 1; x < internalProjectSplit.Length; x++)
                {
                    internalEndLast += internalProjectSplit[x] + ((x == (internalProjectSplit.Length - 1)) ? "" : " ");
                }
            }

            // Do the check the previous commands prepared for.
            if (!data.InInternalContacts(data.GetMemberIdByInfo(internalProjectLast, internalProjectFirst, internalProjectMail)))
            {
                data.AddInternalContact(internalProjectLast, internalProjectFirst, internalProjectMail);
            }
            #endregion InternalEndResponsible

            // Start adding the project and it's resources.
            if (!data.InProjects(code))
            {
                data.AddProject(code, title, startDate, endDate, budget, timeOfCompletion, entity, data.GetInternalContactIdByInfo(data.GetMemberIdByInfo(internalProjectLast, internalProjectFirst, internalProjectMail)), data.GetInternalContactIdByInfo(data.GetMemberIdByInfo(internalEndLast, internalEndFirst, internalEndMail)), data.GetClientIdByName(client), description, summary, data.GetExternalContactIdByNames(externalProjectLast, externalProjectFirst, client), data.GetExternalContactIdByNames(externalEndLast, externalEndFirst, client));
            }

            string role = "";

            for (int x = 0; x < resourceList.Count; x++)
            {
                if (!role.ToLower().Equals(roleList[x][0].ToLower()))
                {
                    role = roleList[x][0];
                }

                string firstName = "";
                string lastName = "";

                if (resourceList[x][0].Contains(", "))
                {
                    firstName = resourceList[x][0].Split(',')[1].Remove(0, 1);
                    lastName = resourceList[x][0].Split(',')[0];
                }
                else
                {
                    string[] split = resourceList[x][0].Split(' ');
                    firstName = split[0];

                    for (int y = 1; y < split.Length; y++)
                    {
                        lastName += split[y] + ((y == (split.Length - 1)) ? "" : " ");
                    }
                }

                int id = data.GetInternalContactIdByInfo(data.GetMemberIdByInfo(lastName, firstName, lastName + "." + firstName + PresetConfig.EmailSuffix), data.GetRoleIdByNameAndRate(role, double.Parse(rates[x][0].ToString())));

                if (!data.ProjectHasResource(code, id))
                {
                    data.AddResource(code, id, double.Parse(dayList[x][0].ToString()));
                }
            }
        }

        /// <summary>
        /// Writes all data to the database, or at least calls the correct methods to do so.
        /// </summary>
        /// <param name="configItems">List of items containing all PAF information.</param>
        public void WriteToDatabase(List<Item> configItems)
        {
            // Initialize new objects.
            SharepointClass sharepoint = new SharepointClass(errorDelegate);
            SharepointDbDataContext data = new SharepointDbDataContext();
            List<string> resources = new List<string>();
            List<string> roles = new List<string>();
            int counter = 0;
            int clientId = 0;
            int externalEndId = 0;
            int externalProjectId = 0;
            int internalEndId = 0;
            int internalEndMemberId = 0;
            int internalProjectId = 0;
            int internalProjectMemberId = 0;

            // This will help the code convert strings to double values.
            var numberFormat = (System.Globalization.NumberFormatInfo)System.Globalization.CultureInfo.InstalledUICulture.NumberFormat.Clone();
            numberFormat.NumberDecimalSeparator = ".";

            // Retrieve the necessary information.
            string client = configItems.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ClientName)).First().Values.First().First();
            string sector = configItems.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.Sector)).First().Values.First().First();
            string entity = configItems.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.Entity)).First().Values.First().First();
            string code = configItems.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ProjectCode)).First().Values.First().First();
            string title = configItems.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ProjectTitle)).First().Values.First().First();
            string description = configItems.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.Description)).First().Values.First().First();
            string summary = configItems.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.Summary)).First().Values.First().First();
            double budget = Convert.ToDouble(configItems.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.Budget)).First().Values.First().First());
            double timeOfCompletion = Convert.ToDouble(configItems.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.TimeOfCompletion)).First().Values.First().First());
            DateTime startDate = DateTime.Parse(configItems.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.StartDate)).First().Values.First().First());
            DateTime endDate = DateTime.Parse(configItems.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.EndDate)).First().Values.First().First());
            List<List<string>> resourceList = configItems.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ResourceNames)).First().Values;
            List<List<string>> roleList = configItems.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ResourceRoles)).First().Values;
            List<List<string>> rates = configItems.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ResourceHourRates)).First().Values;
            List<List<string>> dayList = configItems.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ResourceDays)).First().Values;

            // Parse the resource list into an array, each list inside the list contains only 1 value because there's only 1 column.
            foreach (List<string> temp in resourceList)
            {
                // Filter out the empty ones.
                if (!temp[0].Equals(""))
                {
                    string resourceName = temp[0];

                    if (resourceName.Contains(", "))
                    {
                        resourceName = resourceName.Split(',')[1].Remove(0, 1) + " " + resourceName.Split(',')[0];
                    }

                    string mail = sharepoint.GetInternalMail(resourceName);

                    if (!data.InMembers(mail.Split('@').First().Split('.').Last(), mail.Split('@').First().Split('.').First()))
                    {
                        data.AddMember(mail.Split('@').First().Split('.').Last(), mail.Split('@').First().Split('.').First(), mail);
                    }

                    resources.Add(resourceName);
                }
            }

            // Parse the role list into an array, each list inside the list contains only 1 value because there's only 1 column.
            foreach (List<string> temp in roleList)
            {
                // filter out the empty ones.
                if (!temp[0].Equals(""))
                {
                    roles.Add(temp[0]);

                    // Put the checks/adds in here to save a loopthrough of the roles list.
                    if (!data.InRoles(temp[0], Convert.ToDouble(rates[counter][0].ToString(), numberFormat)))
                    {
                        data.AddRole(temp[0], Convert.ToDouble(rates[counter][0].ToString(), numberFormat), true);
                    }
                }

                counter++;
            }

            // Perform the rest of the checks/adds.
            if (!data.InSectors(sector))
            {
                data.AddSector(sector);
            }

            if (!data.InClients(client))
            {
                data.AddClient(client, sector);
            }

            if (!data.InEntities(entity))
            {
                data.AddEntity(entity);
            }

            if (data.GetClientIdByName(client) != 0)
            {
                clientId = data.GetClientIdByName(client);
            }
            else
            {
                data.AddClient(client, sector, true);
                clientId = data.GetClientIdByName(client);
            }

            #region responsibleRegion
            bool externalEndFound = false;
            bool externalProjectFound = false;
            bool internalEndFound = false;
            bool internalProjectFound = false;
            string externalEndName = ((items.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ExternalEndResponsibleName)).First().Values.First().First() != null) ? items.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ExternalEndResponsibleName)).First().Values.First().First() : "").Replace(",", "").Replace("  ", " ");

            if (externalEndName != null && !externalEndName.Equals(""))
            {
                externalEndName = ((externalEndName[externalEndName.Length - 1] == ' ') ? ((externalEndName[0] == ' ') ? externalEndName.Remove(externalEndName.LastIndexOf(' ')).Remove(0, 1) : externalEndName.Remove(externalEndName.LastIndexOf(' '))) : ((externalEndName[0] == ' ') ? externalEndName.Remove(0, 1) : externalEndName));
            }
            else
            {
                // In case it's null.
                externalEndName = "";
            }

            string externalProjectName = ((items.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ExternalProjectResponsibleName)).First().Values.First().First() != null) ? items.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ExternalProjectResponsibleName)).First().Values.First().First() : "").Replace(",", "").Replace("  ", " ");

            if (externalProjectName != null && !externalProjectName.Equals(""))
            {
                externalProjectName = ((externalProjectName[externalProjectName.Length - 1] == ' ') ? ((externalProjectName[0] == ' ') ? externalProjectName.Remove(externalProjectName.LastIndexOf(' ')).Remove(0, 1) : externalProjectName.Remove(externalProjectName.LastIndexOf(' '))) : ((externalProjectName[0] == ' ') ? externalProjectName.Remove(0, 1) : externalProjectName));
            }
            else
            {
                // In case it's null.
                externalProjectName = "";
            }

            string internalEndName = ((items.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.InternalEndResponsibleName)).First().Values.First().First() != null) ? items.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.InternalEndResponsibleName)).First().Values.First().First() : "").Replace(",", "").Replace("  ", " ");

            if (internalEndName != null && !internalEndName.Equals(""))
            {
                internalEndName = ((internalEndName[internalEndName.Length - 1] == ' ') ? ((internalEndName[0] == ' ') ? internalEndName.Remove(internalEndName.LastIndexOf(' ')).Remove(0, 1) : internalEndName.Remove(internalEndName.LastIndexOf(' '))) : ((internalEndName[0] == ' ') ? internalEndName.Remove(0, 1) : internalEndName));
            }
            else
            {
                // In case it's null.
                internalEndName = "";
            }

            string internalProjectName = ((items.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.InternalProjectResponsibleName)).First().Values.First().First() != null) ? items.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.InternalProjectResponsibleName)).First().Values.First().First() : "").Replace(",", "").Replace("  ", " ");

            if (internalProjectName != null && !internalProjectName.Equals(""))
            {
                internalProjectName = ((internalProjectName[internalProjectName.Length - 1] == ' ') ? ((internalProjectName[0] == ' ') ? internalProjectName.Remove(internalProjectName.LastIndexOf(' ')).Remove(0, 1) : internalProjectName.Remove(internalProjectName.LastIndexOf(' '))) : ((internalProjectName[0] == ' ') ? internalProjectName.Remove(0, 1) : internalProjectName));
            }
            else
            {
                // In case it's null.
                internalProjectName = "";
            }

            // Retrieve data.
            List<ExternalContact> externalContacts = data.GetExternalContacts();
            List<Member> members = data.GetMembers();

            if (externalContacts.Count != 0)
            {
                foreach (ExternalContact contact in externalContacts)
                {
                    if (externalEndName != null && !externalEndName.Equals("") && SharePointHelper.CompareNames(externalEndName, contact.FirstName + " " + contact.Name))
                    {
                        externalEndFound = true;
                    }

                    if (externalProjectName != null && !externalProjectName.Equals("") && SharePointHelper.CompareNames(externalProjectName, contact.FirstName + " " + contact.Name))
                    {
                        externalProjectFound = true;
                    }
                }
            }

            if (members.Count != 0)
            {
                foreach (Member member in members)
                {
                    if (internalEndName != null && !internalEndName.Equals("") && SharePointHelper.CompareNames(internalEndName, member.FirstName + " " + member.Name))
                    {
                        internalEndFound = true;
                    }

                    if (internalProjectFound != null && !internalProjectFound.Equals("") && SharePointHelper.CompareNames(internalProjectName, member.FirstName + " " + member.Name))
                    {
                        internalProjectFound = true;
                    }
                }
            }

            if (client != null && !client.Equals(""))
            {
                if (externalEndFound && externalEndName != null && !externalEndName.Equals(""))
                {
                    string[] split = items.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ExternalEndResponsibleName)).First().Values.First().First().Split(',');

                    if (data.GetExternalContactIdByNames(split[0], split[1], client) == 0)
                    {
                        data.AddExternalContact(split[0], split[1], items.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ExternalEndResponsibleMail)).First().Values.First().First(), clientId);
                    }

                    externalEndId = data.GetExternalContactIdByNames(split[0], split[1], client);
                }
                else if (externalEndName != null && !externalEndName.Equals(""))
                {
                    string[] split = items.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ExternalEndResponsibleName)).First().Values.First().First().Split(',');
                    data.AddExternalContact(split[0], split[1], ((items.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ExternalEndResponsibleMail)).First().Values.First().First() != null) ? items.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ExternalEndResponsibleMail)).First().Values.First().First() : ""), clientId);
                    externalEndId = data.GetExternalContactIdByNames(split[0], split[1], client);
                }

                if (externalProjectFound && externalProjectName != null && !externalProjectName.Equals(""))
                {
                    string[] split = items.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ExternalProjectResponsibleName)).First().Values.First().First().Split(',');

                    if (data.GetExternalContactIdByNames(split[0], split[1], client) == 0)
                    {
                        data.AddExternalContact(split[0], split[1], items.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ExternalProjectResponsibleMail)).First().Values.First().First(), clientId);
                    }

                    externalProjectId = data.GetExternalContactIdByNames(split[0], split[1], client);
                }
                else if (externalProjectName != null && !externalProjectName.Equals(""))
                {
                    string[] split = items.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ExternalProjectResponsibleName)).First().Values.First().First().Split(',');
                    data.AddExternalContact(split[0], split[1], ((items.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ExternalProjectResponsibleMail)).First().Values.First().First() != null) ? items.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ExternalProjectResponsibleMail)).First().Values.First().First() : ""), clientId);
                    externalProjectId = data.GetExternalContactIdByNames(split[0], split[1], client);
                }
            }

            if (internalEndFound && internalEndName != null && !internalEndName.Equals(""))
            {
                string mail = sharepoint.GetInternalMail(internalEndName);

                if (data.GetMemberIdByInfo(mail.Split('@').First().Split('.').Last(), mail.Split('@').First().Split('.').First(), mail) == 0)
                {
                    data.AddMember(mail.Split('@').First().Split('.').Last(), mail.Split('@').First().Split('.').First(), mail);
                }

                internalEndMemberId = data.GetMemberIdByInfo(mail.Split('@').First().Split('.').Last(), mail.Split('@').First().Split('.').First(), mail);
            }
            else if (internalEndName != null && !internalEndName.Equals(""))
            {
                string mail = sharepoint.GetInternalMail(internalEndName);
                data.AddMember(mail.Split('@').First().Split('.').Last(), mail.Split('@').First().Split('.').First(), mail);
                internalEndMemberId = data.GetMemberIdByInfo(mail.Split('@').First().Split('.').Last(), mail.Split('@').First().Split('.').First(), mail);
            }

            if (internalProjectFound && internalProjectName != null && !internalProjectName.Equals(""))
            {
                string mail = sharepoint.GetInternalMail(internalProjectName);

                if (data.GetMemberIdByInfo(mail.Split('@').First().Split('.').Last(), mail.Split('@').First().Split('.').First(), mail) == 0)
                {
                    data.AddMember(mail.Split('@').First().Split('.').Last(), mail.Split('@').First().Split('.').First(), mail);
                }

                internalProjectMemberId = data.GetMemberIdByInfo(mail.Split('@').First().Split('.').Last(), mail.Split('@').First().Split('.').First(), mail);
            }
            else if (internalProjectName != null && !internalProjectName.Equals(""))
            {
                string mail = sharepoint.GetInternalMail(internalProjectName);
                data.AddMember(mail.Split('@').First().Split('.').Last(), mail.Split('@').First().Split('.').First(), mail);
                internalProjectMemberId = data.GetMemberIdByInfo(mail.Split('@').First().Split('.').Last(), mail.Split('@').First().Split('.').First(), mail);
            }

            // InternalContacts checking and/or adding.
            if (data.InInternalContacts(internalEndMemberId))
            {
                internalEndId = data.GetInternalContactIdByInfo(internalEndMemberId);
            }
            else
            {
                data.AddInternalContact(internalEndMemberId);
                internalEndId = data.GetInternalContactIdByInfo(internalEndMemberId);
            }

            if (data.InInternalContacts(internalProjectMemberId))
            {
                internalProjectId = data.GetInternalContactIdByInfo(internalProjectMemberId);
            }
            else
            {
                data.AddInternalContact(internalProjectMemberId);
                internalProjectId = data.GetInternalContactIdByInfo(internalProjectMemberId);
            }
            #endregion responsibleRegion

            errorDelegate(configItems.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.Description)).First().Values.First().First() + Environment.NewLine + configItems.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.Summary)).First().Values.First().First());

            // Start adding the project and it's resources.
            if (!data.InProjects(code))
            {
                string[] endSplit = items.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ExternalEndResponsibleName)).First().Values.First().First().Split(',');
                string[] projectSplit = items.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ExternalProjectResponsibleName)).First().Values.First().First().Split(',');
                errorDelegate(configItems.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.Description)).First().Values.First().First() + "\t" + description + Environment.NewLine + configItems.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.Summary)).First().Values.First().First() + "\t" + summary);
                data.AddProject(code, title, startDate, endDate, budget, timeOfCompletion, entity, internalProjectId, internalEndId, clientId, description, summary, externalProjectId, externalEndId);
            }

            string role = "";

            for (int x = 0; x < resourceList.Count; x++)
            {
                string name = ((resourceList[x][0].Contains(',')) ? ((resourceList[x][0].Split(',').Last().First() == ' ') ? resourceList[x][0].Split(',').Last().Remove(0, 1) : resourceList[x][0].Split(',').Last()) + " " + resourceList[x][0].Split(',').First() : resourceList[x][0]);
                string mail = sharepoint.GetInternalMail(name);
                
                if (!role.ToLower().Equals(roleList[x][0].ToLower()))
                {
                    role = roleList[x][0];
                }

                int id = data.GetInternalContactIdByInfo(data.GetMemberIdByInfo(mail.Split('@').First().Split(',').Last(), mail.Split('@').First().Split(',').First(), mail), data.GetRoleIdByNameAndRate(role, Convert.ToDouble(rates[x][0].ToString())));

                if (id == 0)
                {
                    if (data.GetMemberIdByInfo(mail.Split('@').First().Split(',').Last(), mail.Split('@').First().Split(',').First(), mail) == 0)
                    {
                        data.AddMember(mail.Split('@').First().Split(',').Last(), mail.Split('@').First().Split(',').First(), mail);
                    }

                    if (data.GetRoleIdByNameAndRate(role, Convert.ToDouble(rates[x][0].ToString())) == 0)
                    {
                        data.AddRole(role, Convert.ToDouble(rates[x][0].ToString()), true);
                    }

                    if (data.GetInternalContactIdByInfo(data.GetMemberIdByInfo(mail.Split('@').First().Split(',').Last(), mail.Split('@').First().Split(',').First(), mail), data.GetRoleIdByNameAndRate(role, Convert.ToDouble(rates[x][0].ToString()))) == 0)
                    {
                        data.AddInternalContact(data.GetMemberIdByInfo(mail.Split('@').First().Split(',').Last(), mail.Split('@').First().Split(',').First(), mail), data.GetRoleIdByNameAndRate(role, Convert.ToDouble(rates[x][0].ToString())));
                    }

                    id = data.GetInternalContactIdByInfo(data.GetMemberIdByInfo(mail.Split('@').First().Split(',').Last(), mail.Split('@').First().Split(',').First(), mail), data.GetRoleIdByNameAndRate(role, Convert.ToDouble(rates[x][0].ToString())));
                }

                if (!data.ProjectHasResource(code, id))
                {
                    data.AddResource(code, id, double.Parse(dayList[x][0].ToString()));
                }
            }
        }

        /// <summary>
        /// Reads the PAF information, creates an intermediary report and sends it to the destined person by mail.
        /// </summary>
        /// <param name="projectId">PAF project identification number.</param>
        /// <returns>Returns whether or not the action was succesful.</returns>
        public bool SendIntermediaryReport(int projectId, string destinationAddress, OpenFileDelegate openFileDelegate)
        {
            try
            {
                SharepointClass sharepoint = new SharepointClass(errorDelegate);

                // If no data can be read, no data can be written.
                if (!sharepoint.ReadPAFInfo(projectId, true))
                {
                    return false;
                }

                items = sharepoint.GetPAFData();
                string title = items.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ProjectTitle)).First().Values.First().First();

                // Load the correct report template.
                projectUrl = PresetConfig.DefaultProjectUrl + "/" + title;
                string srcTemplate = PresetConfig.DefaultISOUrl + "/" + PresetConfig.ISOSourceRootFolder.Replace(" ", "%20") + "/";

                try
                {
                    switch (items.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.Language)).First().Values.First().First())
                    {
                        case "Nederlands":
                            srcTemplate += PresetConfig.DutchReportTemplate;
                            break;
                        case "Français":
                            srcTemplate += PresetConfig.FrenchReportTemplate;
                            break;
                        default:
                            srcTemplate += PresetConfig.EnglishReportTemplate;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    srcTemplate += PresetConfig.EnglishReportTemplate;
                }

                MemoryStream document = sharepoint.WriteIntermediaryReport(items, openFileDelegate(srcTemplate));

                if (document != null)
                {
                    try
                    {
                        document.Seek(0, SeekOrigin.Begin);

                        MailMessage mail = new MailMessage(PresetConfig.SourceMail, destinationAddress, ("Intermediary PAF project report" + ((title != null && !title.Equals("")) ? " - " + title : "")), "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"><style type=\"text/css\">a{color: #0072bc;text-decoration: none;}</style><table border=\"0\" cellspacing=\"0\" cellpadding=\"8\" width=\"600\" dir=\"ltr\"><tr><td align=\"left\" valign=\"top\"><div style=\"font-family: 'Segoe UI Light', 'Segoe UI', Verdana, sans-serif; color: #444444;\"><div style=\"margin-bottom: 21px; font-size: 18px;\">Here's the project report that you requested.</div><div style=\"font-size: 14px;\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" dir=\"ltr\"><tr><td align=\"left\" valign=\"top\"><div style=\"margin-bottom: 8px; font-size: 28px; font-family: 'Segoe UI Light', 'Segoe UI', Verdana, sans-serif; color: #444444;\">Project title: " + title + "</div></td></tr></table></div></div></td></tr></table>");
                        mail.IsBodyHtml = true;
                        Attachment attachment = new Attachment(document, (((title != null && !title.Equals("")) ? title : "New project") + ".docx"));
                        mail.Attachments.Add(attachment);
                        SmtpClient client = new SmtpClient(PresetConfig.SMTPServer);
                        client.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                        client.Send(mail);
                    }
                    catch (Exception ex)
                    {
                        // If the mail could not be sent, then this method missed it's purpose.
                        return false;
                    }
                }
                else
                {
                    // If the document contains no data, something went wrong.
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Return failure.
                return false;
            }

            // If the code was able to get this far, all is good.
            return true;
        }

        /// <summary>
        /// Sets the identification number linking the PAF Resource item to the PAF Project item to the correct value. (Default = 0)
        /// </summary>
        /// <param name="resource">Resource identification number.</param>
        /// <returns>Returns whether or not the resource link was regulated.</returns>
        public bool RegulateResourceLink(int resource)
        {
            SharepointClass sharepoint = new SharepointClass(errorDelegate);
            int project = sharepoint.GetHighestPAFProjectID();
            return sharepoint.SetResourceLink(resource);
        }

        /// <summary>
        /// Deletes all information concerning a given PAF project.
        /// </summary>
        /// <param name="project">PAF Project identification number.</param>
        /// <returns>Returns whether or not the project information was deleted.</returns>
        public bool DeletePAFInformation(int project)
        {
            SharepointClass sharepoint = new SharepointClass(errorDelegate);
            return sharepoint.DeletePAFInformation(project);
        }

        /// <summary>
        /// Load all config items into a list using the PresetConfig class.
        /// </summary>
        public void LoadConfig()
        {
            configItems = PresetConfig.ConfigItems;
        }

        /// <summary>
        /// Sort all config items in the order of sheets (nested), ranges (nested) and variable names.
        /// </summary>
        public void SortConfigItems()
        {
            // Loop through each ConfigItem to sort the list fields.
            foreach (ConfigItem item in configItems)
            {
                item.Sheets.Sort();
                item.Ranges.Sort();
            }

            // Do the actual sorting.
            configItems = (from c in configItems
                           orderby c.Sheets.First<string>(), c.Ranges.First<string>(), c.Variable
                           select c).ToList<ConfigItem>();
        }

        /// <summary>
        /// Setter method to initialize the error handling delegate.
        /// </summary>
        /// <param name="errorDelegate">Delegated update method</param>
        public void SetDelegate(ErrorDelegate errorDelegate)
        {
            this.errorDelegate = errorDelegate;
        }

        /// <summary>
        /// Returns the project code from the config items list.
        /// </summary>
        /// <returns>Returns the value loaded in the ProjectCode config item.</returns>
        public string GetProjectCode()
        {
            return (from i in items
                    where i.ConfigItem.Variable.Equals(PresetConfig.ProjectCode)
                    select i.Values.First().First()).First<string>();
        }

        /// <summary>
        /// Returns the list of Item objects containing the contents of the Excel file, combined with their location.
        /// </summary>
        /// <returns>A list of items with their location.</returns>
        public List<Item> GetItems()
        {
            return items;
        }

        /// <summary>
        /// Returns the latest project url if this class has initiated the creation of a new project. Otherwise, this method will return an empty string.
        /// </summary>
        /// <returns>Returns the project string if the program has created one.</returns>
        public string GetProjectUrl()
        {
            return projectUrl;
        }

        /// <summary>
        /// Returns the urls of all ISO documents the program has created.
        /// </summary>
        /// <returns>Returns the urls of all ISO documents the program has created.</returns>
        public List<string> GetISODocumentUrls()
        {
            return isoDocumentUrls;
        }

        /// <summary>
        /// Checks to see if the config items have been loaded and contain values. Requires both LoadConfig and LoadExcel to be executed.
        /// </summary>
        /// <returns>Returns whether or not the config items have been loaded.</returns>
        public bool IsConfigLoaded()
        {
            return (items != null && configItems != null);
        }
    }
}
