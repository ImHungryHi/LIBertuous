using Microsoft.Office.Word.Server.Conversions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WorkflowActions;
using SharepointWorkflow.Common;
using SharepointWorkflow.Logic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace SharepointWorkflow
{
    class WorkflowStarter
    {
        /// <summary>
        /// Starter method to initiate the creation of a SharePoint site.
        /// </summary>
        /// <param name="context">Workflow context containing the information needed to create the new site.</param>
        /// <param name="listId">Project list name.</param>
        /// <param name="itemId">Item identification number.</param>
        /// <param name="fileUrl">Url containing the location of the PAF excel file.</param>
        public static bool CreateSite(WorkflowContext context, string listId, int itemId, string fileUrl)
        {
            try
            {
                string[] hierarchy = fileUrl.Split('/');
                List<string> paths = new List<string>();

                // If the file isn't even an Excel file, there is nothing to do. (xlsx also contains xls, so we kill two birds with one stone)
                if (!hierarchy.Last().Contains(".xls"))
                {
                    return false;
                }
                else
                {
                    if (hierarchy[hierarchy.Length - 2].Equals(PresetConfig.ISOSourceRootFolder) || hierarchy[hierarchy.Length - 2].Equals(PresetConfig.ISOSourceRootFolder.Replace(" ", "%20")))
                    {
                        paths.Add(fileUrl);
                    }
                    else
                    {
                        SPFolder projectFolder = context.Web.Folders[PresetConfig.ISOSourceRootFolder].SubFolders[hierarchy[hierarchy.Length - 2].Replace("%20", " ")];

                        foreach (SPFile file in projectFolder.Files)
                        {
                            paths.Add(file.Item[SPBuiltInFieldId.EncodedAbsUrl].ToString());
                        }
                    }
                }

                // Initialize every field.
                ErrorDelegate errorDelegate = new ErrorDelegate(Update);
                LogicControl logic = new LogicControl(errorDelegate);

                // Load in the config to enable reading and writing to the Excel file.
                logic.LoadConfig();
                logic.SortConfigItems();    // Put this in comment to save some resources.

                // Loads all datasheet contents.
                if (fileUrl != null && !fileUrl.Equals(""))
                {
                    // Load the file into the memory.
                    logic.LoadExcel(fileUrl);
                }
                else
                {
                    // Show the user an error, the file wasn't loaded yet.
                    Update("Could not load the excel file.");
                }

                // Create the site and expect a response.
                if (!logic.CreateProject(GetRootPath(fileUrl)))
                {
                    return false;
                }

                foreach (string path in paths)
                {
                    MoveFile(context, listId, itemId, path, logic);
                }

                if (!hierarchy[hierarchy.Length - 2].Equals(PresetConfig.ISOSourceRootFolder) && !hierarchy[hierarchy.Length - 2].Equals(PresetConfig.ISOSourceRootFolder.Replace(" ", "%20")))
                {
                    SPFolder projectFolder = context.Web.Folders[PresetConfig.ISOSourceRootFolder].SubFolders[hierarchy[hierarchy.Length - 2].Replace("%20", " ")];
                    projectFolder.Delete();
                }
            }
            catch (Exception ex)
            {
                Update(ex.Message + Environment.NewLine + ex.TargetSite);
                return false;
            }

            // If the program was able to get this far, all is good.
            return true;
        }

        /// <summary>
        /// Starter method to initiate the creation of a SharePoint site through the use of a SharePoint PAF list item.
        /// </summary>
        /// <param name="context">Workflow context containing the information needed to create the new site.</param>
        /// <param name="listId">Project list name.</param>
        /// <param name="itemId">Item identification number.</param>
        public static bool CreateSiteFromPAFList(WorkflowContext context, string listId, int itemId)
        {
            try
            {
                // Initialize every field.
                LogicControl logic = new LogicControl(new ErrorDelegate(Update));

                // Load in the config to enable reading from the PAF Project List and enable writing to a PAF file.
                logic.LoadConfig();

                // Create the site and expect a response.
                if (!logic.CreateProjectFromList(itemId, new OpenFileDelegate(OpenFile), new SaveDelegate(SaveFile), new CopyDelegate(CopyFile)))
                {
                    return false;
                }

                string wordFile = logic.GetISODocumentUrls().Where(i => i.Contains(".docx")).First();

                if (wordFile != null && !wordFile.Equals(""))
                {
                    ConversionJobSettings jobSettings = new ConversionJobSettings();
                    ConversionJob pdfConversion = new ConversionJob("Word Automation Services", jobSettings);
                    pdfConversion.UserToken = context.Web.CurrentUser.UserToken;
                    string pdfFile = wordFile.Replace(".docx", ".pdf");
                    pdfConversion.AddFile(wordFile, pdfFile);
                    pdfConversion.Start();
                }
            }
            catch (Exception ex)
            {
                Update(ex.Message + Environment.NewLine + ex.TargetSite);
                return false;
            }

            // If the program was able to get this far, all is good.
            return true;
        }

        /// <summary>
        /// Starter method to initiate the linking of a PAF Resource item to a PAF Project item.
        /// </summary>
        /// <param name="context">Workflow context containing the necessary information.</param>
        /// <param name="listId">Project list name.</param>
        /// <param name="itemId">Item identification number.</param>
        public static bool SetCorrectID(WorkflowContext context, string listId, int itemId)
        {
            try
            {
                LogicControl logic = new LogicControl(new ErrorDelegate(Update));
                logic.RegulateResourceLink(itemId);
            }
            catch (Exception ex)
            {
                Update("An error occurred while setting the correct project ID to the resource item." + Environment.NewLine + ex.Message + Environment.NewLine + ex.TargetSite);
                return false;
            }

            // If the program was able to get this far, all is good.
            return true;
        }

        /// <summary>
        /// Starter method to initiate the deletion of a PAF Resource item and it's corresponding PAF Resources.
        /// </summary>
        /// <param name="context">Workflow context containing the necessary information.</param>
        /// <param name="listId">Project list name.</param>
        /// <param name="itemId">Item identification number.</param>
        public static bool DeletePAFProject(WorkflowContext context, string listId, int itemId)
        {
            try
            {
                LogicControl logic = new LogicControl(new ErrorDelegate(Update));
                logic.DeletePAFInformation(itemId);
            }
            catch (Exception ex)
            {
                Update("An error occurred while deleting the PAF project." + Environment.NewLine + ex.Message + Environment.NewLine + ex.TargetSite);
                return false;
            }

            // If the program was able to get this far, all is good.
            return true;
        }

        /// <summary>
        /// Method to debug all methods on the SharePoint environment. Mainly to check functions which might be faulty on the server side, but not on the client side.
        /// </summary>
        /// <param name="context">Workflow context containing the information needed to move the file provided by the path.</param>
        /// <param name="listId">Project list name.</param>
        /// <param name="itemId">Item identification number.</param>
        public static bool Debug(WorkflowContext context, string listId, int itemId)
        {
            // Todo: verander paf lezen naar list lezen
            try
            {
                //LogicControl logic = new LogicControl(new ErrorDelegate(Update));
                //logic.Test(itemId);
                //List<Item> items = logic.GetItems();
                //logic.WriteToDatabase(items);

                // Initialize every field.
                LogicControl logic = new LogicControl(new ErrorDelegate(Update));

                // Load in the config to enable reading from the PAF Project List and enable writing to a PAF file.
                logic.LoadConfig();

                // Create the site and expect a response.
                if (!logic.CreateProjectFromList(itemId, new OpenFileDelegate(OpenFile), new SaveDelegate(SaveFile), new CopyDelegate(CopyFile)))
                {
                    return false;
                }

                string wordFile = logic.GetISODocumentUrls().Where(i => i.Contains(".docx")).First();

                if (wordFile != null && !wordFile.Equals(""))
                {
                    ConversionJobSettings jobSettings = new ConversionJobSettings();
                    ConversionJob pdfConversion = new ConversionJob("Word Automation Services", jobSettings);
                    pdfConversion.UserToken = context.Web.CurrentUser.UserToken;
                    string pdfFile = wordFile.Replace(".docx", ".pdf");
                    pdfConversion.AddFile(wordFile, pdfFile);
                    pdfConversion.Start();
                }
            }
            catch (Exception ex)
            {
                Update(ex.Message + Environment.NewLine + ex.TargetSite);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Starter method to initiate the update of a SharePoint site through the use of a SharePoint PAF list item.
        /// </summary>
        /// <param name="context">Workflow context containing the information needed to create the new site.</param>
        /// <param name="listId">Project list name.</param>
        /// <param name="itemId">Item identification number.</param>
        public static bool PAFProjectUpdateNotification(WorkflowContext context, string listId, int itemId)
        {
            try
            {
                List<Dictionary<string, string>> changedColumns = GetChangedColumns(itemId);

                SPSite site = new SPSite(PresetConfig.DefaultISOUrl);
                SPWeb web = site.OpenWeb();
                SPList list = web.Lists[PresetConfig.PAFProjectList];
                SPListItem item = list.GetItemById(itemId);
                string projectTitle = (item[PresetConfig.SPProjectTitle] != null) ? item[PresetConfig.SPProjectTitle].ToString() : "";

                if (changedColumns.Count > 0)
                {
                    string content = "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"><style type=\"text/css\">a { color: #0072bc; text-decoration: none; }</style><table border=\"0\" cellspacing=\"0\" cellpadding=\"8\" width=\"600\" dir=\"ltr\"><tr><td align=\"left\" valign=\"top\"><div style=\"font-family: 'Segoe UI Light', 'Segoe UI', Verdana, sans-serif; color: #444444;\"><div style=\"margin-bottom: 21px; font-size: 18px;\">A PAF project " + ((!projectTitle.Equals("")) ? "named \"" + projectTitle + "\" " : "") + "has been updated and is ready for approval.\nThe following columns have been updated:<br /><ul>";

                    foreach (Dictionary<string, string> column in changedColumns)
                    {
                        content += "<li>" + column["StaticName"] + "</li>";
                    }

                    content += "</ul></div><div style=\"font-size: 14px;\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" dir=\"ltr\"><tr><td align=\"left\" valign=\"top\"><div style=\"margin-bottom: 8px; font-size: 28px; font-family: 'Segoe UI Light', 'Segoe UI', Verdana, sans-serif; color: #444444;\">Go to <a href=\"" + PresetConfig.DefaultISOUrl + "/Lists/" + PresetConfig.PAFProjectList.Replace(" ", "%20") + "/AllItems.aspx#InplviewHash2ef45b75-d47f-4a63-ba69-75eb3c145f00=SortField%3DProject%255Fx0020%255FID-SortDir%3DDesc-FilterField1%3DID-FilterValue1%3D" + itemId + "%252E0000000000000\">" + ((!projectTitle.Equals("")) ? projectTitle : "this page") + "</a></div></td></tr></table></div></div></td></tr></table>";

                    MailMessage msg = new MailMessage(PresetConfig.SourceMail, PresetConfig.DestinationMail, "PAF Project has been updated" + ((!projectTitle.Equals("")) ? " - " + projectTitle : ""), content);
                    msg.IsBodyHtml = true;
                    SmtpClient client = new SmtpClient(PresetConfig.SMTPServer);
                    client.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

                    try
                    {
                        client.Send(msg);
                    }
                    catch (Exception ex)
                    {
                        // Do nothing, we can't send mail if the sending of mails contains errors.
                    }
                }
            }
            catch (Exception ex)
            {
                Update(ex.Message + Environment.NewLine + ex.TargetSite);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Starter method to initiate the update of a SharePoint site through the use of a SharePoint PAF list item.
        /// </summary>
        /// <param name="context">Workflow context containing the information needed to create the new site.</param>
        /// <param name="listId">Project list name.</param>
        /// <param name="itemId">Item identification number.</param>
        public static bool EditSiteFromPAFList(WorkflowContext context, string listId, int itemId)
        {
            try
            {
                // Initialize the necessary variables.
                LogicControl logic = new LogicControl(new ErrorDelegate(Update));

                // Load in the config to enable reading from the PAF Projects list and enable writing to a PAF file.
                logic.LoadConfig();

                // Create the site and expect a response.
                if (!logic.EditProjectFromList(itemId, new OpenFileDelegate(OpenFile), new SaveDelegate(SaveFile), new CopyDelegate(CopyFile)))
                {
                    return false;
                }

                string wordFile = logic.GetISODocumentUrls().Where(i => i.Contains(".docx")).First();

                if (wordFile != null && !wordFile.Equals(""))
                {
                    ConversionJobSettings jobSettings = new ConversionJobSettings();
                    ConversionJob pdfConversion = new ConversionJob("Word Automation Services", jobSettings);
                    pdfConversion.UserToken = context.Web.CurrentUser.UserToken;
                    string pdfFile = wordFile.Replace(".docx", ".pdf");
                    pdfConversion.AddFile(wordFile, pdfFile);
                    pdfConversion.Start();
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
        /// Starter method to initiate the creation and sending of a report of a SharePoint site through the use of a SharePoint PAF list item
        /// </summary>
        /// <param name="context">Workflow context containing the information needed to create the new site.</param>
        /// <param name="listId">Project list name.</param>
        /// <param name="itemId">Item identification number.</param>
        public static bool SendIntermediaryReport(WorkflowContext context, string listId, int itemId)
        {
            try
            {
                // Initialize the necessary variables.
                LogicControl logic = new LogicControl(new ErrorDelegate(Update));

                // Load the config to enable reading from the PAF Projects list to enable writing to a PAF file.
                logic.LoadConfig();

                // Build and send an intermediary report.
                SPUser initiator = context.InitiatorUser;
                logic.SendIntermediaryReport(itemId, initiator.Email, new OpenFileDelegate(OpenFile));
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
        /// Method used to move a file from the project provided in the context to the project provided in the file.
        /// </summary>
        /// <param name="context">Workflow context containing the information needed to move the file provided by the path.</param>
        /// <param name="listId">Project list name.</param>
        /// <param name="itemId">Item identification number.</param>
        /// <param name="fileUrl">Url containing the location of the PAF excel file.</param>
        public static void MoveFile(WorkflowContext context, string listId, int itemId, string fileUrl, LogicControl logic = null)
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });

            try
            {
                string[] temp = fileUrl.Split('/');
                string fileName = temp.Last();
                string root = GetRootPath(context.CurrentWebUrl);

                WebRequest srcRequest = WebRequest.Create(fileUrl);
                srcRequest.Credentials = CredentialCache.DefaultCredentials;
                WebResponse srcResponse = srcRequest.GetResponse();

                // Transfer the file from the database to the server.
                using (Stream input = srcResponse.GetResponseStream())
                {
                    byte[] data;

                    using (MemoryStream memStream = new MemoryStream())
                    {
                        input.CopyTo(memStream);
                        data = memStream.ToArray();
                    }

                    SPSite site = new SPSite(root + ((root.Last() == '/') ? "" : "/") + logic.GetProjectCode());
                    SPWeb web = site.OpenWeb();
                    SPFolder folder = web.Folders[PresetConfig.ISODestinationFolder];
                    SPFile file = folder.Files.Add(root + "/" + logic.GetProjectCode() + folder.Url.Replace(" ", "%20") + "/" + fileName, data, true);

                    if (temp[temp.Length - 2].Equals(PresetConfig.ISOSourceRootFolder) || temp[temp.Length - 2].Equals(PresetConfig.ISOSourceRootFolder.Replace(" ", "%20")))
                    {
                        SPFile delete = context.Web.Folders[PresetConfig.ISOSourceRootFolder].Files[fileName];
                        delete.Delete();
                    }
                    else
                    {
                        SPFile delete = context.Web.Folders[PresetConfig.ISOSourceRootFolder].SubFolders[temp[temp.Length - 2].Replace("%20", " ")].Files[fileName];
                        delete.Delete();
                    }
                }
            }
            catch (Exception ex)
            {
                Update("An error occurred while moving the files from the source folder to the destination folder." + Environment.NewLine + ex.Message);
            }
        }

        /// <summary>
        /// Method used to copy files from one location to another in the SharePoint environment.
        /// </summary>
        /// <param name="sourceUrl">Full SharePoint source url linking to the file of origin.</param>
        /// <param name="projectUrl">Full SharePoint destination url linking to the project site.</param>
        /// <param name="documentLibrary">Document Library title (destination folder).</param>
        /// <param name="fileName">File name.</param>
        public static void CopyFile(string sourceUrl, string projectUrl, string documentLibrary, string fileName)
        {
            try
            {
                WebRequest request = WebRequest.Create(sourceUrl);
                request.Credentials = CredentialCache.DefaultCredentials;
                request.UseDefaultCredentials = true;
                WebResponse response = request.GetResponse();
                Stream source = response.GetResponseStream();
                MemoryStream memory = new MemoryStream();
                source.CopyTo(memory);

                SPSite site = new SPSite(projectUrl);
                SPWeb web = site.OpenWeb();
                SPFolder folder = web.Folders[documentLibrary.Replace(" ", "%20")];
                SPFile file = folder.Files.Add(fileName, memory.ToArray(), true);
            }
            catch (Exception ex)
            {
                Update(ex.Message + Environment.NewLine + ex.TargetSite);
            }
        }

        /// <summary>
        /// Method used to open a template file from which data needs to be read.
        /// </summary>
        /// <param name="url">Template url.</param>
        /// <returns>Returns the data read from the template file in a MemoryStream object.</returns>
        public static MemoryStream OpenFile(string url)
        {
            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Credentials = CredentialCache.DefaultCredentials;
                request.UseDefaultCredentials = true;
                WebResponse response = request.GetResponse();
                Stream source = response.GetResponseStream();
                MemoryStream memory = new MemoryStream();
                source.CopyTo(memory);
                return memory;
            }
            catch (Exception ex)
            {
                return new MemoryStream();
            }
        }

        /// <summary>
        /// Method used to save a MemoryStream object into a new file in the SharePoint environment.
        /// </summary>
        /// <param name="memory">MemoryStream object containing the file's data.</param>
        /// <param name="projectUrl">Project root site url.</param>
        /// <param name="documentLibrary">Destination document library.</param>
        /// <param name="fileName">File name, including extension.</param>
        public static void SaveFile(MemoryStream memory, string projectUrl, string documentLibrary, string fileName)
        {
            try
            {
                SPSite site = new SPSite(projectUrl);
                SPWeb web = site.OpenWeb();
                SPFolder folder = web.Folders[documentLibrary.Replace(" ", "%20")];
                SPFile file = folder.Files.Add(fileName, memory.ToArray(), true);
            }
            catch (Exception ex)
            {
                // Do nothing yet.
            }
        }

        /// <summary>
        /// Starts the procedure which will give all members full control and revoke their "Contribute" permissions at the same time.
        /// Currently, the method will always return true, but should the need arise, extra checks will be needed to return either true or false.
        /// </summary>
        /// <param name="context">Workflow context containing the necessary information.</param>
        /// <param name="listId">Project list name.</param>
        /// <param name="itemId">Item identification number.</param>
        /// <returns>Returns whether or not the upgrade has been executed.</returns>
        public static bool UpgradeMemberPermissions(WorkflowContext context, string listId, int itemId)
        {
            LogicControl logic = new LogicControl(new ErrorDelegate(Update));
            logic.UpgradeMemberPermissions();

            // If the program got this far, all is good.
            return true;
        }

        /// <summary>
        /// Error/message handling method.
        /// </summary>
        /// <param name="message">Message containing information about errors or events.</param>
        public static void Update(string message)
        {
            MailMessage msg = new MailMessage(PresetConfig.DebugSourceMail, PresetConfig.DebugDestinationMail, PresetConfig.DebugSubject, "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"><style type=\"text/css\">a{color: #0072bc;text-decoration: none;}</style><table border=\"0\" cellspacing=\"0\" cellpadding=\"8\" width=\"600\" dir=\"ltr\"><tr><td align=\"left\" valign=\"top\"><div style=\"font-family: 'Segoe UI Light', 'Segoe UI', Verdana, sans-serif; color: #444444;\"><div style=\"margin-bottom: 21px; font-size: 18px;\">" + message + "</div></td></tr></table>");
            msg.IsBodyHtml = true;
            SmtpClient client = new SmtpClient(PresetConfig.SMTPServer);
            client.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

            try
            {
                client.Send(msg);
            }
            catch (Exception ex)
            {
                // Do nothing, we can't send mail if the sending of mails contains errors.
            }
        }

        /// <summary>
        /// Retrieves a list of all changed columns of a certain PAF Projects item. (Select with "InternalName" or "StaticName".
        /// </summary>
        /// <param name="project">Project identification number.</param>
        /// <returns>Returns a list of all changed columns of a certain PAF Projects item.</returns>
        public static List<Dictionary<string, string>> GetChangedColumns(int project)
        {
            List<Dictionary<string, string>> changedColumns = new List<Dictionary<string, string>>();
            List<string> columns = new List<string>
            {
                PresetConfig.SPApproach,
                PresetConfig.SPApproval,
                PresetConfig.SPBudget,
                PresetConfig.SPClientLocationCommunity,
                PresetConfig.SPClientLocationCountry,
                PresetConfig.SPClientLocationStreet,
                PresetConfig.SPClientName,
                PresetConfig.SPCRMStatus,
                PresetConfig.SPDefectionReason,
                PresetConfig.SPDeliverables,
                PresetConfig.SPDependencies,
                PresetConfig.SPDescription,
                PresetConfig.SPEndDate,
                PresetConfig.SPEntity,
                PresetConfig.SPEuroPerKm,
                PresetConfig.SPExternalEndResponsibleFirstName,
                PresetConfig.SPExternalEndResponsibleLastName,
                PresetConfig.SPExternalEndResponsibleMail,
                PresetConfig.SPExternalProjectResponsibleFirstName,
                PresetConfig.SPExternalProjectResponsibleLastName,
                PresetConfig.SPExternalProjectResponsiblMail,
                PresetConfig.SPGenvalToClient,
                PresetConfig.SPFacturationInFunctionOf,
                PresetConfig.SPFacturationSchedule,
                PresetConfig.SPFacturationType,
                PresetConfig.SPFirstStageFacturationAmount,
                PresetConfig.SPFirstStageFacturationDate,
                PresetConfig.SPFourthStageFacturationAmount,
                PresetConfig.SPFourthStageFacturationDate,
                PresetConfig.SPInternalControlFrequency,
                PresetConfig.SPInternalEndResponsible,
                PresetConfig.SPInternalMeetings,
                PresetConfig.SPInternalProjectResponsible,
                PresetConfig.SPLanguage,
                PresetConfig.SPOrderNumber,
                PresetConfig.SPOtherAvailable,
                PresetConfig.SPOtherCommentary,
                PresetConfig.SPOtherCost,
                PresetConfig.SPOtherCostPrices1,
                PresetConfig.SPOtherCostPrices2,
                PresetConfig.SPOtherCostPrices3,
                PresetConfig.SPOtherCostPrices4,
                PresetConfig.SPOtherCostTitles1,
                PresetConfig.SPOtherCostTitles2,
                PresetConfig.SPOtherCostTitles2,
                PresetConfig.SPOtherCostTitles3,
                PresetConfig.SPOtherCostTitles4,
                PresetConfig.SPOtherSpecification,
                PresetConfig.SPParkingAvailable,
                PresetConfig.SPPaymentConditions,
                PresetConfig.SPPeriodicalFacturationAmount,
                PresetConfig.SPPeriodicalFacturationDate,
                PresetConfig.SPPowerpointLogoUsage,
                PresetConfig.SPProcedureDefection,
                PresetConfig.SPProjectCode,
                PresetConfig.SPProjectConfidentialityStatus,
                PresetConfig.SPProjectID,
                PresetConfig.SPProjectTitle,
                PresetConfig.SPResults,
                PresetConfig.SPSecondStageFacturationAmount,
                PresetConfig.SPSecondStageFacturationDate,
                PresetConfig.SPSector,
                PresetConfig.SPSMLToClient,
                PresetConfig.SPStartDate,
                PresetConfig.SPStrategicalMoment,
                PresetConfig.SPSubcontractorPresent,
                PresetConfig.SPSummary,
                PresetConfig.SPTaxNumber,
                PresetConfig.SPTaxRate,
                PresetConfig.SPThirdStageFacturationAmount,
                PresetConfig.SPThirdStageFacturationDate
            };

            try
            {
                SPSite st = new SPSite(PresetConfig.DefaultISOUrl);
                SPWeb wb = st.OpenWeb();
                SPList lst = wb.Lists[PresetConfig.PAFProjectList];
                SPListItem itm = lst.GetItemById(project);

                if (itm.Versions.Count > 1)
                {
                    SPListItemVersion current = itm.Versions[0];
                    SPListItemVersion previous = itm.Versions[1];

                    if (!current.IsCurrentVersion)
                    {
                        foreach (SPListItemVersion version in itm.Versions)
                        {
                            if (version.IsCurrentVersion)
                            {
                                current = version;
                                break;
                            }
                        }
                    }

                    foreach (string column in columns)
                    {
                        SPField previousField = previous.Fields.GetFieldByInternalName(column);
                        SPField currentField = current.Fields.GetFieldByInternalName(column);

                        if (previous[previousField.StaticName] != null && current[currentField.StaticName] != null && !previous[previousField.StaticName].Equals(current[currentField.StaticName]))
                        {
                            changedColumns.Add(new Dictionary<string, string> { { "InternalName", column }, { "StaticName", currentField.Title } });
                        }
                        else if ((previous[previousField.StaticName] == null && current[currentField.StaticName] != null) || (previous[previousField.StaticName] != null && current[currentField.StaticName] == null))
                        {
                            changedColumns.Add(new Dictionary<string, string> { { "InternalName", column }, { "StaticName", currentField.Title } });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Just return an empty list.
                return new List<Dictionary<string, string>>();
            }

            return changedColumns;
        }

        /// <summary>
        /// Calculates the root path from the fileurl parameter.
        /// </summary>
        /// <returns>The root path containing everything.</returns>
        private static string GetRootPath(string fileUrl)
        {
            try
            {
                string[] split = fileUrl.Split('/');
                string returnValue = "";

                if (split.Length == 0)
                {
                    throw new Exception();
                }

                foreach (string element in split)
                {
                    if (element == null || element.Equals(""))
                    {
                        returnValue += "/";
                    }
                    else
                    {
                        if (element.Equals("Projects"))
                        {
                            // We know that the element contains "Projects", so just add it to the string manually. Also break the loop, we don't need anything further to be added to the return string.
                            returnValue += "/Projects";
                            break;
                        }
                        else
                        {
                            returnValue += ((element.Equals("https:") || element.Equals("http:")) ? "" : "/") + element;
                        }
                    }
                }

                return returnValue;
            }
            catch (Exception ex)
            {
                Update("No url value found to calculate root path from.");
            }

            // If the program got this far, something went wrong, just return an empty string to indicate failure.
            return "";
        }
    }
}
