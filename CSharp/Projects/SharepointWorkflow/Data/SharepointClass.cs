using Microsoft.SharePoint;
using Microsoft.SharePoint.Client;
using Novacode;
using SharepointWorkflow.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace SharepointWorkflow.Data
{
    class SharepointClass
    {
        private ErrorDelegate errorDelegate;
        private ClientContext clientContext;
        private CamlQuery query;
        private List list;
        private ListItemCollection listItems;
        private List<Item> configData;
        private User user;
        private string url;
        private string listName;

        /// <summary>
        /// Default constructor method, used to construct all basic properties.
        /// </summary>
        public SharepointClass(ErrorDelegate errorDelegate)
        {
            // Set the fields.
            this.errorDelegate = errorDelegate;
            url = PresetConfig.DefaultProjectUrl;
            this.configData = new List<Item>();

            // Bypass the ssl exception to enable reading.
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });

            // Create a new ClientContext object, referring to a standard url.
            clientContext = new ClientContext(url);

            // Initialize extra necessary objects.
            CamlQuery query = new CamlQuery();
            query.ViewXml = "<View/>";

            // Get the domain user.
            user = clientContext.Web.EnsureUser("administrator");
            clientContext.Load(user);
            clientContext.ExecuteQuery();
        }

        /// <summary>
        /// Non-default constructor method, used to construct all basic properties, using specified parameters to allow for dynamic SharePoint actions.
        /// </summary>
        /// <param name="url">SharePoint root url.</param>
        public SharepointClass(string url, ErrorDelegate errorDelegate)
        {
            // Set the fields.
            this.url = url;
            this.errorDelegate = errorDelegate;
            this.configData = new List<Item>();

            // Bypass the ssl exception to enable reading.
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });

            // Create a new ClientContext object, referring to the url from the parameter.
            clientContext = new ClientContext(url);

            // Initialize extra necessary objects.
            query = new CamlQuery();
            query.ViewXml = "<View/>";

            // Get the domain user.
            user = clientContext.Web.EnsureUser("administrator");
            clientContext.Load(user);
            clientContext.ExecuteQuery();
        }

        /// <summary>
        /// Loads a specified list of objects into the predefined class fields.
        /// </summary>
        /// <param name="listName"></param>
        public void LoadList(string listName)
        {
            // Load the parameters into the class fields.
            this.listName = listName;

            // Load the specified list into the appropriate List object.
            list = clientContext.Web.Lists.GetByTitle(listName);
            clientContext.Load(list);
            clientContext.ExecuteQuery();

            // Load the list contents.
            listItems = list.GetItems(query);
            clientContext.Load(listItems);
            clientContext.ExecuteQuery();

            if (list == null || listItems == null)
            {
                throw new Exception("An error occurred while querying the list of projects.");
            }
        }

        /// <summary>
        /// Manages the creation of a new site, the correspondent project and the upload of all ISO documents.
        /// </summary>
        /// <param name="listName">Name of the list used to create the new project in.</param>
        /// <param name="title">Project title.</param>
        /// <param name="projectCode">Project code.</param>
        /// <param name="client">Client name.</param>
        /// <param name="entity">Entity code.</param>
        /// <param name="resources">Array of resource names.</param>
        /// <param name="owners">Array of owner names (in this case a conversion of the first 3 letters from both the first and last name).</param>
        /// <param name="duration">String containing the period the project will be executed in.</param>
        /// <param name="startDate">The date of start of the project.</param>
        /// <param name="endDate">The end date of the project.</param>
        /// <param name="description">Project description.</param>
        /// <param name="budget">Budget reserved for the project.</param>
        public bool AddProject(string listName, string title, string projectCode, string client, string entity, string sector, string[] resources, string[] owners, string duration, DateTime startDate, DateTime endDate, string description, double budget)
        {
            try
            {
                if (SiteExists(projectCode) || ProjectExists(projectCode) >= 0 || PermissionGroupsExist(projectCode))
                {
                    errorDelegate("<h1>Error at project " + projectCode + "</h1><p>A project or remnants of a project containing this project code already exists.</p><p>Please change the project code and try again.</p>");
                    return false;
                }
            }
            catch(Exception ex)
            {
                errorDelegate("<h1>Error at project " + projectCode + "</h1><p>There was an error while checking for existing objects.</p>");
            }

            try
            {
                // Initialize the necessary variables.
                this.listName = listName;
                string solution = "";   // Final branch variable.
                string tempBranch = "";   // We'll use this to check for branch syntaxes.
                List<string> branches = new List<string>();
                List<FieldUserValue> lstResources = new List<FieldUserValue>();
                List<User> ownerList = new List<User>();
                List<User> memberList = new List<User>();
                User mobiusmembers = clientContext.Web.EnsureUser("mobiusmembers");
                FieldUserValue[] resourceArray;
                FieldUrlValue url = new FieldUrlValue();
                url.Description = projectCode;
                url.Url = this.url + "/" + projectCode;

                // Query all branch names.
                foreach (ListItem iterated in listItems)
                {
                    if (iterated.FieldValues["Business_x0020_Development_x00200"] != null)
                    {
                        branches.Add(iterated.FieldValues["Business_x0020_Development_x00200"].ToString());
                    }
                }

                // Parse the project code into the correct syntax.
                switch (sector.ToLower())
                {
                    case "lorego":
                        tempBranch = PresetConfig.LoReGo;
                        break;
                    case "fedgov":
                        tempBranch = PresetConfig.FedGov;
                        break;
                    case "banins":
                        tempBranch = PresetConfig.BanIns;
                        break;
                    case "indret":
                        tempBranch = PresetConfig.IndRet;
                        break;
                    case "health":
                        tempBranch = PresetConfig.Health;
                        break;
                    case "utiwas":
                        tempBranch = PresetConfig.UtiWas;
                        break;
                    case "tesepu":
                        tempBranch = PresetConfig.TeSePu;
                        break;
                    default:
                        tempBranch = "";
                        break;
                }

                clientContext.Load(mobiusmembers);
                clientContext.ExecuteQuery();

                foreach (string s in owners)
                {
                    User owner = clientContext.Web.EnsureUser(s);
                    clientContext.Load(owner);
                    clientContext.ExecuteQuery();

                    ownerList.Add(owner);
                }

                // Reload the resources to get the right container.
                foreach (string s in resources)
                {
                    // Set the temporary string to be the resource string by default, this could be overwritten in case of a comma seperated value (foreach defined values don't support overwriting).
                    string tempString = s;

                    // Reformat the string to eliminate comma sepereted values and make them into a first name - last name value
                    if (s.Contains(","))
                    {
                        string[] split = s.Split(',');

                        if (split[1][0].Equals(' '))
                        {
                            split[1] = split[1].Remove(0, 1);
                        }

                        tempString = split[1] + " " + split[0];
                    }

                    User resource = clientContext.Web.EnsureUser(tempString);
                    clientContext.Load(resource);
                    clientContext.ExecuteQuery();

                    memberList.Add(resource);

                    FieldUserValue userValue = new FieldUserValue();
                    userValue.LookupId = resource.Id;

                    lstResources.Add(userValue);
                }

                // Create a subsite.
                Web site = clientContext.Web;
                clientContext.Load(site);
                clientContext.ExecuteQuery();
                WebCollection webs = site.Webs;
                clientContext.Load(webs);
                clientContext.ExecuteQuery();

                WebCreationInformation webCreateInfo = new WebCreationInformation();
                webCreateInfo.Description = description;
                webCreateInfo.Language = PresetConfig.EnglishLocale;
                webCreateInfo.Title = projectCode;
                webCreateInfo.Url = projectCode;
                webCreateInfo.WebTemplate = PresetConfig.SiteTemplate;
                webCreateInfo.UseSamePermissionsAsParentSite = false;

                Web newSite = webs.Add(webCreateInfo);
                clientContext.Load(newSite, website => website.ServerRelativeUrl, website => website.Created);
                clientContext.ExecuteQuery();
                
                newSite.Navigation.UseShared = true;
                clientContext.Load(newSite.SiteGroups);
                clientContext.ExecuteQuery();

                // Load the context permission roles so we can read them..
                clientContext.Load(clientContext.Web.RoleDefinitions);
                clientContext.ExecuteQuery();

                newSite.BreakRoleInheritance(false, true);

                // Create the owner group, setting themselves as the actual owners.
                GroupCreationInformation ownerGroupInfo = new GroupCreationInformation();
                ownerGroupInfo.Title = projectCode + " Owners";
                ownerGroupInfo.Description = "Use this group to grant people full control permissions to the SharePoint site: <a href=\"/Projects/" + projectCode + "\">" + projectCode + "</a>";
                Group ownerGroup = newSite.SiteGroups.Add(ownerGroupInfo);

                foreach (User owner in ownerList)
                {
                    ownerGroup.Users.AddUser(owner);
                }

                // Create the member permission group and assign the owner function to the owner group.
                GroupCreationInformation memberGroupInfo = new GroupCreationInformation();
                memberGroupInfo.Title = projectCode + " Members";
                memberGroupInfo.Description = "Use this group to grant people full control permissions to the SharePoint site: <a href=\"/Projects/" + projectCode + "\">" + projectCode + "</a>";
                Group memberGroup = newSite.SiteGroups.Add(memberGroupInfo);

                foreach (User member in memberList)
                {
                    memberGroup.Users.AddUser(member);
                }

                // Create the visitor permission group and assign the owner function to the owner group.
                GroupCreationInformation visitorGroupInfo = new GroupCreationInformation();
                visitorGroupInfo.Title = projectCode + " Visitors";
                visitorGroupInfo.Description = "Use this group to grant people read permissions to the SharePoint site: <a href=\"/Projects/" + projectCode + "\">" + projectCode + "</a>";
                Group visitorGroup = newSite.SiteGroups.Add(visitorGroupInfo);
                visitorGroup.Users.AddUser(mobiusmembers);

                // Parse the resource list to an array of FieldUserValue objects.
                resourceArray = lstResources.ToArray<FieldUserValue>();

                // Get the necessary role definitions from the SharePoint environment.
                RoleDefinition fullRole = clientContext.Web.RoleDefinitions.GetByType(RoleType.Administrator);
                RoleDefinition readRole = clientContext.Web.RoleDefinitions.GetByType(RoleType.Reader);

                // Assign each role to their respective role definition binding collection.
                RoleDefinitionBindingCollection memberBindings = new RoleDefinitionBindingCollection(clientContext);
                RoleDefinitionBindingCollection ownerBindings = new RoleDefinitionBindingCollection(clientContext);
                RoleDefinitionBindingCollection visitorBindings = new RoleDefinitionBindingCollection(clientContext);
                memberBindings.Add(fullRole);
                ownerBindings.Add(fullRole);
                visitorBindings.Add(readRole);

                // Assign the previously built groups to the site object and load the values into SharePoint.
                RoleAssignment memberAssignment = newSite.RoleAssignments.Add(memberGroup, memberBindings);
                RoleAssignment ownerAssignment = newSite.RoleAssignments.Add(ownerGroup, ownerBindings);
                RoleAssignment visitorAssignment = newSite.RoleAssignments.Add(visitorGroup, visitorBindings);

                // Load the roles into the context.
                clientContext.Load(fullRole, role => role.Name);
                clientContext.Load(readRole, role => role.Name);
                clientContext.ExecuteQuery();

                // Assiciate the new groups to the site and save changes.
                newSite.AssociatedMemberGroup = memberGroup;
                newSite.AssociatedOwnerGroup = ownerGroup;
                newSite.AssociatedVisitorGroup = visitorGroup;
                newSite.Update();
                clientContext.ExecuteQuery();

                // Assign the right group owners to the groups and allow everyone to view the group memberships.
                newSite.AssociatedOwnerGroup.OnlyAllowMembersViewMembership = false;
                newSite.AssociatedMemberGroup.OnlyAllowMembersViewMembership = false;
                newSite.AssociatedVisitorGroup.OnlyAllowMembersViewMembership = false;
                newSite.AssociatedOwnerGroup.Owner = ownerGroup;
                newSite.AssociatedMemberGroup.Owner = ownerGroup;
                newSite.AssociatedVisitorGroup.Owner = ownerGroup;
                newSite.AssociatedOwnerGroup.Update();
                newSite.AssociatedMemberGroup.Update();
                newSite.AssociatedVisitorGroup.Update();
                newSite.Update();
                clientContext.ExecuteQuery();

                // Load the specified list into the appropriate List object.
                list = clientContext.Web.Lists.GetByTitle(listName);
                clientContext.Load(list);
                clientContext.ExecuteQuery();

                // Start by creating a new item.
                ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
                ListItem item = list.AddItem(itemCreateInfo);
                itemCreateInfo.FolderUrl = this.url + "/Lists/" + listName.Replace(" ", "%20"); // Master%20Project%20List
                item.Update();
                clientContext.ExecuteQuery();

                try
                {
                    // Fill the new item with the appropriate information.
                    item["Title"] = title;
                    item["Project_x0020_Code"] = projectCode;
                    item["Customer"] = client;
                    item["M_x00d6_BIUS_x0020_entity"] = entity;
                    item["Resources"] = resourceArray;
                    item["SP_x0020_ProjectSite"] = url;
                    item["Duration"] = duration;
                    item["KpiDescription"] = description;
                    item["Budget"] = budget;
                    item.Update();
                    clientContext.ExecuteQuery();

                    try
                    {
                        item["Start_x0020_Date"] = startDate;
                        item.Update();
                        clientContext.ExecuteQuery();
                    }
                    catch (Exception ex)
                    {
                        //errorDelegate("Start has an error");
                    }

                    try
                    {
                        item["End_x0020_Date"] = endDate;
                        item.Update();
                        clientContext.ExecuteQuery();
                    }
                    catch (Exception ex)
                    {
                        //errorDelegate("End has an error");
                    }

                    // Do the actual checking for the branch syntax.
                    if (!tempBranch.Equals(""))
                    {
                        branches = branches.Distinct().ToList<string>();

                        foreach (string branchItem in branches)
                        {
                            string[] temp = branchItem.Split('|');

                            if (temp[0].Equals(tempBranch))
                            {
                                solution = branchItem;
                                break;
                            }
                            else
                            {
                                if (temp[0].Contains(tempBranch))
                                {
                                    solution = branchItem;
                                    break;
                                }
                            }
                        }

                        // This will either be empty or filled with the correct syntax. In any case, if the syntax is incorrect, the value will not show in the SharePoint environment.
                        item["Business_x0020_Development_x00200"] = solution;
                    }

                    // Mark the item to make it ready for update and execute the update.
                    item.Update();
                    clientContext.ExecuteQuery();
                }
                catch (Exception ex)
                {
                    // This should be handled seperately, the import of data should not interfere with the creation of a project.
                }

                try
                {
                    User administrator = clientContext.Web.EnsureUser("administrator@mobius.eu");
                    clientContext.Load(administrator);
                    clientContext.ExecuteQuery();

                    foreach (User recipient in memberGroup.Users)
                    {
                        clientContext.Load(recipient);
                        clientContext.ExecuteQuery();

                        System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage(administrator.Email, recipient.Email);
                        msg.IsBodyHtml = true;
                        msg.Subject = administrator.Title + " has invited you to '" + projectCode + "'";
                        msg.Body = "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"><style type=\"text/css\">a{color: #0072bc;text-decoration: none;}</style><table border=\"0\" cellspacing=\"0\" cellpadding=\"8\" width=\"600\" dir=\"ltr\"><tr><td align=\"left\" valign=\"top\"><div style=\"font-family: 'Segoe UI Light', 'Segoe UI', Verdana, sans-serif; color: #444444;\"><div style=\"margin-bottom: 21px; font-size: 18px;\">Here's the site that " + administrator.Title + " shared with you.</div><div style=\"font-size: 14px;\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" dir=\"ltr\"><tr><td align=\"left\" valign=\"top\"><div style=\"margin-bottom: 8px; font-size: 28px; font-family: 'Segoe UI Light', 'Segoe UI', Verdana, sans-serif; color: #444444;\">Go to <a href=\"" + PresetConfig.DefaultProjectUrl + "/" + projectCode + "\">" + projectCode + "</a></div><div style=\"font-size: 14px; font-family: 'Segoe UI', Verdana, sans-serif; color: #444444;\"><a href=\"" + PresetConfig.DefaultProjectUrl + "/" + projectCode + "?FollowSite=1&amp;SiteName=" + projectCode + "\">Follow</a> this site to get updates in your newsfeed.</div></td></tr></table></div></div></td></tr></table>";
                        System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient(PresetConfig.SMTPServer);

                        try
                        {
                            smtpClient.Send(msg);
                        }
                        catch (Exception ex)
                        {
                            // Do nothing, errors sending individual mails may not interfere with sending mails to all other recipients.
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Do nothing, errors sending mails may not interfere with the creation of a project.
                }

                // If the program was able to get this far, all is good.
                return true;
            }
            catch (Exception ex)
            {
                // If any error has occurred, delete all remnants of what has been created and return failure.
                DeleteRemnants(projectCode);
                errorDelegate("<h1>Error at project " + projectCode + "</h1><p>" + ex.Message + "\n" + ex.TargetSite + "</p>");
                return false;
            }
        }

        /// <summary>
        /// Manages the creation of a new site, the correspondent project and the upload of all ISO documents.
        /// </summary>
        /// <param name="listName">Name of the list used to create the new project in.</param>
        /// <param name="title">Project title.</param>
        /// <param name="projectCode">Project code.</param>
        /// <param name="client">Client name.</param>
        /// <param name="entity">Entity code.</param>
        /// <param name="resources">Array of resource names.</param>
        /// <param name="owners">Array of owner names (in this case a conversion of the first 3 letters from both the first and last name).</param>
        /// <param name="duration">String containing the period the project will be executed in.</param>
        /// <param name="startDate">The date of start of the project.</param>
        /// <param name="endDate">The end date of the project.</param>
        /// <param name="description">Project description.</param>
        /// <param name="budget">Budget reserved for the project.</param>
        public bool UpdateProject(string listName, string title, string projectCode, string client, string entity, string sector, string[] resources, string[] owners, string duration, DateTime startDate, DateTime endDate, string description, double budget)
        {
            try
            {
                if (!SiteExists(projectCode) || ProjectExists(projectCode) <= 0 || !PermissionGroupsExist(projectCode))
                {
                    errorDelegate("<h1>Error at project " + projectCode + "</h1><p>Some of the necessary project objects are missing.</p>");
                    return false;
                }
            }
            catch(Exception ex)
            {
                errorDelegate("<h1>Error at project " + projectCode + "</h1><p>There was an error while checking for missing project objects.</p>");
            }

            try
            {
                // Initialize the necessary variables.
                this.listName = listName;
                string solution = "";   // Final branch variable.
                string tempBranch = "";   // We'll use this to check for branch syntaxes.
                List<string> branches = new List<string>();
                List<FieldUserValue> lstResources = new List<FieldUserValue>();
                List<User> memberList = new List<User>();
                ClientContext subContext = new ClientContext(PresetConfig.DefaultProjectUrl + "/" + projectCode);
                subContext.Load(subContext.Web);
                subContext.ExecuteQuery();
                FieldUserValue[] resourceArray;
                FieldUrlValue url = new FieldUrlValue();
                url.Description = projectCode;
                url.Url = this.url + "/" + projectCode;
                User administrator = clientContext.Web.EnsureUser("administrator@mobius.eu");
                clientContext.Load(administrator);
                clientContext.Load(clientContext.Web);
                clientContext.Load(clientContext.Web.SiteGroups);
                clientContext.ExecuteQuery();

                // Query all branch names.
                foreach (ListItem iterated in listItems)
                {
                    if (iterated.FieldValues["Business_x0020_Development_x00200"] != null)
                    {
                        branches.Add(iterated.FieldValues["Business_x0020_Development_x00200"].ToString());
                    }
                }

                // Parse the project code into the correct syntax.
                switch (sector.ToLower())
                {
                    case "lorego":
                        tempBranch = PresetConfig.LoReGo;
                        break;
                    case "fedgov":
                        tempBranch = PresetConfig.FedGov;
                        break;
                    case "banins":
                        tempBranch = PresetConfig.BanIns;
                        break;
                    case "indret":
                        tempBranch = PresetConfig.IndRet;
                        break;
                    case "health":
                        tempBranch = PresetConfig.Health;
                        break;
                    case "utiwas":
                        tempBranch = PresetConfig.UtiWas;
                        break;
                    case "tesepu":
                        tempBranch = PresetConfig.TeSePu;
                        break;
                    default:
                        tempBranch = "";
                        break;
                }

                // Reload the resources to get the right container.
                foreach (string s in resources)
                {
                    // Set the temporary string to be the resource string by default, this could be overwritten in case of a comma seperated value (foreach defined values don't support overwriting).
                    string tempString = s;

                    // Reformat the string to eliminate comma sepereted values and make them into a first name - last name value
                    if (s.Contains(","))
                    {
                        string[] split = s.Split(',');

                        if (split[1][0].Equals(' '))
                        {
                            split[1] = split[1].Remove(0, 1);
                        }

                        tempString = split[1] + " " + split[0];
                    }

                    User resource = clientContext.Web.EnsureUser(tempString);
                    clientContext.Load(resource);
                    clientContext.ExecuteQuery();

                    memberList.Add(resource);

                    FieldUserValue userValue = new FieldUserValue();
                    userValue.LookupId = resource.Id;

                    lstResources.Add(userValue);
                }

                // Load the subsite.
                Web site = subContext.Web;
                subContext.Load(site);
                subContext.Load(site.SiteGroups);
                subContext.Load(site.AssociatedMemberGroup);
                subContext.ExecuteQuery();

                // Update the member group users and add the new ones to a seperate array for mailing.
                Group memberGroup = clientContext.Web.SiteGroups.GetByName(site.AssociatedMemberGroup.Title);
                clientContext.Load(memberGroup);
                clientContext.Load(memberGroup.Users);
                clientContext.ExecuteQuery();

                List<int> memberGroupIdList = new List<int>();

                foreach (User groupUser in memberGroup.Users)
                {
                    clientContext.Load(groupUser);
                    clientContext.ExecuteQuery();
                    memberGroupIdList.Add(groupUser.Id);
                }

                foreach (User member in memberList)
                {
                    clientContext.Load(member);
                    clientContext.ExecuteQuery();
                    bool groupContainsUser = false;

                    if (memberGroupIdList.Contains(member.Id))
                    {
                        groupContainsUser = true;
                    }

                    if (!groupContainsUser)
                    {
                        memberGroup.Users.AddUser(member);

                        try
                        {
                            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage(administrator.Email, member.Email);
                            msg.IsBodyHtml = true;
                            msg.Subject = administrator.Title + " has invited you to '" + projectCode + "'";
                            msg.Body = "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"><style type=\"text/css\">a{color: #0072bc;text-decoration: none;}</style><table border=\"0\" cellspacing=\"0\" cellpadding=\"8\" width=\"600\" dir=\"ltr\"><tr><td align=\"left\" valign=\"top\"><div style=\"font-family: 'Segoe UI Light', 'Segoe UI', Verdana, sans-serif; color: #444444;\"><div style=\"margin-bottom: 21px; font-size: 18px;\">Here's the site that " + administrator.Title + " shared with you.</div><div style=\"font-size: 14px;\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" dir=\"ltr\"><tr><td align=\"left\" valign=\"top\"><div style=\"margin-bottom: 8px; font-size: 28px; font-family: 'Segoe UI Light', 'Segoe UI', Verdana, sans-serif; color: #444444;\">Go to <a href=\"" + PresetConfig.DefaultProjectUrl + "/" + projectCode + "\">" + projectCode + "</a></div><div style=\"font-size: 14px; font-family: 'Segoe UI', Verdana, sans-serif; color: #444444;\"><a href=\"" + PresetConfig.DefaultProjectUrl + "/" + projectCode + "?FollowSite=1&amp;SiteName=" + projectCode + "\">Follow</a> this site to get updates in your newsfeed.</div></td></tr></table></div></div></td></tr></table>";
                            System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient(PresetConfig.SMTPServer);

                            try
                            {
                                smtpClient.Send(msg);
                            }
                            catch (Exception ex)
                            {
                                // Do nothing, errors sending individual mails may not interfere with sending mails to all other recipients.
                            }
                        }
                        catch (Exception ex)
                        {
                            // The sending of mails should not interfere with the rest of the code.
                        }
                    }
                }

                foreach (User user in memberGroup.Users)
                {
                    clientContext.Load(user);
                    clientContext.ExecuteQuery();

                    if (!memberList.Any(u => u.Id == user.Id))
                    {
                        memberGroup.Users.Remove(user);
                    }
                }

                // Parse the resource list to an array of FieldUserValue objects.
                resourceArray = lstResources.ToArray<FieldUserValue>();

                // Load the specified list into the appropriate List object.
                list = clientContext.Web.Lists.GetByTitle(listName);
                clientContext.Load(list);
                clientContext.ExecuteQuery();

                CamlQuery query = new CamlQuery();
                query.ViewXml = "<View><Query><Where><Eq><FieldRef Name='Project_x0020_Code' /><Value Type='Text'>" + projectCode + "</Value></Eq></Where></Query><RowLimit>1</RowLimit></View>";

                ListItemCollection items = list.GetItems(query);
                clientContext.Load(items);
                clientContext.ExecuteQuery();

                // Load the project item.
                ListItem item = items.First();

                if (items.Count == 1)
                {
                    item = items.First();
                    clientContext.Load(item);
                    clientContext.ExecuteQuery();
                }
                else if (items.Count > 1)
                {
                    foreach (ListItem target in items)
                    {
                        clientContext.Load(target);
                        clientContext.ExecuteQuery();

                        if (target["Project_x0020_Code"].ToString().Equals(projectCode))
                        {
                            item = target;
                        }
                    }
                }

                try
                {
                    // Fill the new item with the appropriate information.
                    item["Title"] = title;
                    item["Customer"] = client;
                    item["M_x00d6_BIUS_x0020_entity"] = entity;
                    item["Resources"] = resourceArray;
                    item["SP_x0020_ProjectSite"] = url;
                    item["Duration"] = duration;
                    item["KpiDescription"] = description;
                    item["Budget"] = budget;
                    item.Update();
                    clientContext.ExecuteQuery();

                    try
                    {
                        item["Start_x0020_Date"] = startDate;
                        item.Update();
                        clientContext.ExecuteQuery();
                    }
                    catch (Exception ex)
                    {
                        //errorDelegate("Start has an error");
                    }

                    try
                    {
                        item["End_x0020_Date"] = endDate;
                        item.Update();
                        clientContext.ExecuteQuery();
                    }
                    catch (Exception ex)
                    {
                        //errorDelegate("End has an error");
                    }

                    // Do the actual checking for the branch syntax.
                    if (!tempBranch.Equals(""))
                    {
                        branches = branches.Distinct().ToList<string>();

                        foreach (string branchItem in branches)
                        {
                            string[] temp = branchItem.Split('|');

                            if (temp[0].Equals(tempBranch))
                            {
                                solution = branchItem;
                                break;
                            }
                            else
                            {
                                if (temp[0].Contains(tempBranch))
                                {
                                    solution = branchItem;
                                    break;
                                }
                            }
                        }

                        // This will either be empty or filled with the correct syntax. In any case, if the syntax is incorrect, the value will not show in the SharePoint environment.
                        item["Business_x0020_Development_x00200"] = solution;
                    }

                    // Mark the item to make it ready for update and execute the update.
                    item.Update();
                    clientContext.ExecuteQuery();
                }
                catch (Exception ex)
                {
                    // This should be handled seperately, the import of data should not interfere with the creation of a project.
                }

                // If the program was able to get this far, all is good.
                return true;
            }
            catch (Exception ex)
            {
                // If any error has occurred, return failure.
                errorDelegate("<h1>Error at project " + projectCode + "</h1><p>" + ex.Message + "\n" + ex.TargetSite + "</p>");
                return false;
            }
        }

        /// <summary>
        /// Writes the necessary ISO files and stores the url into a list to be returned.
        /// </summary>
        /// <param name="projectCode">Project identification code.</param>
        /// <param name="itemList">List of project data.</param>
        /// <param name="projectUrl">Project destination url.</param>
        /// <param name="documentLibrary">Destination document library name.</param>
        /// <param name="fileName">Output file name.</param>
        /// <param name="memory">Stream object containing all file data.</param>
        /// <param name="saveDelegate">Delegated method to save all file contents.</param>
        /// <returns>Returns a list of ISO document url's.</returns>
        public List<string> WriteISOFiles(string projectCode, List<Item> itemList, string projectUrl, string documentLibrary, string fileName, MemoryStream memory, SaveDelegate saveDelegate)
        {
            // Get the versioning first.
            fileName = fileName.Replace(".docx", "").Replace(".doc", "");
            int version = 1;
            
            // Load a new context in which the code needs to take care of the versioning.
            ClientContext versioningCtx = new ClientContext(projectUrl);
            versioningCtx.Load(versioningCtx.Web);
            versioningCtx.Load(versioningCtx.Web.Folders);
            versioningCtx.ExecuteQuery();

            Folder isoDocuments = versioningCtx.Web.Folders.GetByUrl(projectUrl + "/" + documentLibrary.Replace(" ", "%20"));
            versioningCtx.Load(isoDocuments);
            versioningCtx.Load(isoDocuments.Files);
            versioningCtx.ExecuteQuery();

            foreach (Microsoft.SharePoint.Client.File isoFile in isoDocuments.Files)
            {
                versioningCtx.Load(isoFile);
                versioningCtx.ExecuteQuery();

                if ((isoFile.Name.Contains(".docx") || isoFile.Name.Contains(".doc")) && isoFile.Name.Contains(fileName))
                {
                    version++;
                }
            }

            bool exists = false;

            do
            {
                // Reset the check value.
                exists = false;

                foreach (Microsoft.SharePoint.Client.File isoFile in isoDocuments.Files)
                {
                    versioningCtx.Load(isoFile);
                    versioningCtx.ExecuteQuery();

                    if (isoFile.Name.Contains(fileName + "_v" + version + ".docx"))
                    {
                        exists = true;
                        break;
                    }
                }

                if (exists)
                {
                    version++;
                }
            } while (exists);

            fileName += "_v" + version + ".docx";

            // Set the default values and pick up the correct PAF template.
            List<string> returnList = new List<string>();
            string destinationUrl = projectUrl + "/" + documentLibrary.Replace(" ", "%20") + "/" + fileName;

            try
            {
                //using (DocX document = DocX.Load(destinationUrl))
                using (DocX document = DocX.Load(memory))
                {
                    // Replace as many placeholders that can be replaced with single statements.
                    document.ReplaceText(PresetConfig.TitlePlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ProjectTitle)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.VersionPlaceholder, version.ToString());
                    document.ReplaceText(PresetConfig.ReferencePlaceholder, "");
                    document.ReplaceText(PresetConfig.EntityPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.Entity)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.ClientEndResponsiblePlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ExternalEndResponsibleName)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.ClientProjectResponsiblePlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ExternalProjectResponsibleName)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.MobiusProjectResponsiblePlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.InternalProjectResponsibleName)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.MobiusEndResponsiblePlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.InternalEndResponsibleName)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.ClientNamePlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ClientName)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.SubcontractorPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.SubcontractorPresent)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.StreetPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ClientLocationStreet)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.CommunityPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ClientLocationCommunity)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.CountryPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ClientLocationCountry)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.TaxNumberPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.TaxNumber)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.ApprovalPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.Approval)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.OrderNumberPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.OrderNumber)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.DescriptionPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.Description)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.SectorPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.Sector)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.DeliverablesPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.Deliverables)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.DependenciesPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.Dependencies)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.BudgetExclPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.Budget)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.BudgetInclPlaceholder, (Convert.ToDouble((itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.Budget)).First().Values.First().First() != null && !itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.Budget)).First().Values.First().First().Equals("")) ? itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.Budget)).First().Values.First().First() : "0") * (Convert.ToDouble((itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.TaxRate)).First().Values.First().First() != null && !itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.TaxRate)).First().Values.First().First().Equals("")) ? itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.TaxRate)).First().Values.First().First() : "0") + 100) / 100).ToString());
                    document.ReplaceText(PresetConfig.TaxRatePlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.TaxRate)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.FacturationTypePlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.FacturationType)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.PaymentConditionsPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.PaymentConditions)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.FacturationSchedulePlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.FacturationSchedule)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.FirstPhaseDatePlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.FirstStageFacturationDate)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.FirstPhaseAmountPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.FirstStageFacturationAmount)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.SecondPhaseDatePlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.SecondStageFacturationDate)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.SecondPhaseAmountPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.SecondStageFacturationAmount)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.ThirdPhaseDatePlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ThirdStageFacturationDate)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.ThirdPhaseAmountPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ThirdStageFacturationAmount)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.FourthPhaseDatePlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.FourthStageFacturationDate)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.FourthPhaseAmountPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.FourthStageFacturationAmount)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.PeriodicalDatePlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.PeriodicalFacturationDate)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.PeriodicalAmountPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.PeriodicalFacturationAmount)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.BillableCostsPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.BillableCosts)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.BillableCostsPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.BillableCosts)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.TotalKmPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.TotalKm)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.KmCostPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.EuroPerKm)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.ParkingPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ParkingAvailable)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.OtherBinaryPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.OtherAvailable)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.OtherPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.OtherSpecification)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.InternalMeetingsPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.InternalMeetings)).First().Values.First().First());

                    // Parse what needs to be parsed.
                    string startString = itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.StartDate)).First().Values.First().First();
                    string endString = itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.EndDate)).First().Values.First().First();

                    try
                    {
                        if (startString != null && !startString.Equals(""))
                        {
                            DateTime start = DateTime.Parse(startString);

                            if (start.TimeOfDay.Hours >= 20)
                            {
                                start = start.AddDays(1);
                            }

                            startString = start.ToShortDateString();
                        }
                    }
                    catch (Exception ex)
                    {
                        startString = "";
                    }

                    try
                    {
                        if (endString != null && !endString.Equals(""))
                        {
                            DateTime end = DateTime.Parse(endString);

                            if (end.TimeOfDay.Hours >= 20)
                            {
                                end = end.AddDays(1);
                            }

                            endString = end.ToShortDateString();
                        }
                    }
                    catch (Exception ex)
                    {
                        endString = "";
                    }

                    // Replace the logo placeholder with the logo.
                    if (itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.LogoUrl)).First().Values.First().First() != null && !itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.LogoUrl)).First().Values.First().First().Equals(""))
                    {
                        WebRequest req = WebRequest.Create(itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.LogoUrl)).First().Values.First().First());
                        req.Credentials = CredentialCache.DefaultCredentials;
                        req.UseDefaultCredentials = true;
                        WebResponse resp = req.GetResponse();
                        Stream stream = resp.GetResponseStream();
                        MemoryStream lifestream = new MemoryStream();
                        stream.CopyTo(lifestream);
                        Image image = document.AddImage(lifestream);

                        Picture pic = image.CreatePicture();
                        double tempWidth = pic.Width;
                        double tempHeight = pic.Height;

                        if (tempWidth > 240)
                        {
                            tempHeight = (tempHeight / tempWidth) * 240;
                            tempWidth = 240;
                        }

                        if (tempHeight > 150)
                        {
                            tempWidth = (tempWidth / tempHeight) * 150;
                            tempHeight = 150;
                        }

                        pic.Width = Convert.ToInt32(tempWidth);
                        pic.Height = Convert.ToInt32(tempHeight);

                        foreach (Paragraph paragraph in document.Paragraphs)
                        {
                            List<int> valueIndices = paragraph.FindAll(PresetConfig.LogoPlaceholder);

                            foreach (int index in valueIndices)
                            {
                                paragraph.RemoveText(index, PresetConfig.LogoPlaceholder.Length);
                                paragraph.InsertPicture(pic, index);
                            }
                        }
                    }
                    else
                    {
                        document.ReplaceText(PresetConfig.LogoPlaceholder, "");
                    }

                    // Replace the resource table with all resource and subcontractor information.
                    List<List<string>> resourceNameList = itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ResourceNames)).First().Values;
                    List<List<string>> resourceRoleList = itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ResourceRoles)).First().Values;
                    List<List<string>> resourceDayRateList = itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ResourceDayRates)).First().Values;
                    List<List<string>> resourceHourRateList = itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ResourceHourRates)).First().Values;
                    List<List<string>> resourceDaysList = itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ResourceDays)).First().Values;
                    List<List<string>> subcontractorNameList = itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.SubcontractorNames)).First().Values;
                    List<List<string>> subcontractorRoleList = itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.SubcontractorRoles)).First().Values;
                    List<List<string>> subcontractorDayRateList = itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.SubcontractorDayRates)).First().Values;
                    List<List<string>> subcontractorHourRateList = itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.SubcontractorHourRates)).First().Values;
                    List<List<string>> subcontractorDaysList = itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.SubcontractorDays)).First().Values;

                    // Loop through the tables to search for the one containing resource and subcontractor information and fill the correct table.
                    foreach (Table table in document.Tables)
                    {
                        double totalDays = 0;
                        double totalPrice = 0;
                        bool check = false;

                        if (table.Xml.ToString().Contains(PresetConfig.ResourceTable))
                        {
                            for (int x = 0; x < resourceNameList.Count; x++)
                            {
                                for (int y = 0; y < resourceNameList[x].Count; y++)
                                {
                                    if ((resourceNameList[x][y] != null && !resourceNameList[x][y].Equals("")) || (resourceRoleList[x][y] != null && !resourceRoleList[x][y].Equals("")) || (resourceDayRateList[x][y] != null && !resourceDayRateList[x][y].Equals("")) || (resourceHourRateList[x][y] != null && !resourceHourRateList[x][y].Equals("")) || (resourceDaysList[x][y] != null && !resourceDaysList[x][y].Equals("")))
                                    {
                                        double days = ((resourceDaysList[x][y] != null && !resourceDaysList[x][y].Equals("")) ? Convert.ToDouble(resourceDaysList[x][y]) : 0);
                                        double price = ((resourceDayRateList[x][y] != null && !resourceDayRateList[x][y].Equals("")) ? Convert.ToDouble(resourceDayRateList[x][y]) : 0);
                                        totalDays += days;
                                        totalPrice += (price * days);

                                        table.InsertRow();
                                        table.Rows.Last().Cells[0].Paragraphs.First().InsertText(resourceRoleList[x][y]);
                                        table.Rows.Last().Cells[1].Paragraphs.First().InsertText(resourceNameList[x][y]);
                                        table.Rows.Last().Cells[2].Paragraphs.First().InsertText(resourceDayRateList[x][y]);
                                        table.Rows.Last().Cells[3].Paragraphs.First().InsertText(resourceHourRateList[x][y]);
                                        table.Rows.Last().Cells[4].Paragraphs.First().InsertText(resourceDaysList[x][y]);
                                        table.Rows.Last().Cells[5].Paragraphs.First().InsertText((days * price).ToString());

                                        table.Rows.Last().Cells[0].Width = table.Rows.First().Cells[0].Width;
                                        table.Rows.Last().Cells[1].Width = table.Rows.First().Cells[1].Width;
                                        table.Rows.Last().Cells[2].Width = table.Rows.First().Cells[2].Width;
                                        table.Rows.Last().Cells[3].Width = table.Rows.First().Cells[3].Width;
                                        table.Rows.Last().Cells[4].Width = table.Rows.First().Cells[4].Width;
                                        table.Rows.Last().Cells[5].Width = table.Rows.First().Cells[5].Width;
                                    }
                                }
                            }

                            for (int x = 0; x < subcontractorNameList.Count; x++)
                            {
                                for (int y = 0; y < subcontractorNameList[x].Count; y++)
                                {
                                    if ((subcontractorNameList[x][y] != null && !subcontractorNameList[x][y].Equals("")) || (subcontractorRoleList[x][y] != null && !subcontractorRoleList[x][y].Equals("")) || (subcontractorDayRateList[x][y] != null && !subcontractorDayRateList[x][y].Equals("")) || (subcontractorHourRateList[x][y] != null && !subcontractorHourRateList[x][y].Equals("")) || (subcontractorDaysList[x][y] != null && !subcontractorDaysList[x][y].Equals("")))
                                    {
                                        double days = ((subcontractorDaysList[x][y] != null && !subcontractorDaysList[x][y].Equals("")) ? Convert.ToDouble(subcontractorDaysList[x][y]) : 0);
                                        double price = ((subcontractorDayRateList[x][y] != null && !subcontractorDayRateList[x][y].Equals("")) ? Convert.ToDouble(subcontractorDayRateList[x][y]) : 0);
                                        totalDays += days;
                                        totalPrice += (price * days);

                                        table.InsertRow();
                                        table.Rows.Last().Cells[0].Paragraphs.First().InsertText(subcontractorRoleList[x][y]);
                                        table.Rows.Last().Cells[1].Paragraphs.First().InsertText(subcontractorNameList[x][y]);
                                        table.Rows.Last().Cells[2].Paragraphs.First().InsertText(subcontractorDayRateList[x][y]);
                                        table.Rows.Last().Cells[3].Paragraphs.First().InsertText(subcontractorHourRateList[x][y]);
                                        table.Rows.Last().Cells[4].Paragraphs.First().InsertText(subcontractorDaysList[x][y]);
                                        table.Rows.Last().Cells[5].Paragraphs.First().InsertText(((subcontractorDaysList[x][y] != null && !subcontractorDaysList[x][y].Equals("") && subcontractorDayRateList[x][y] != null && !subcontractorDayRateList[x][y].Equals("")) ? (Convert.ToDouble(subcontractorDaysList[x][y]) * Convert.ToDouble(subcontractorDayRateList[x][y])).ToString() : "0"));

                                        table.Rows.Last().Cells[0].Width = table.Rows.First().Cells[0].Width;
                                        table.Rows.Last().Cells[1].Width = table.Rows.First().Cells[1].Width;
                                        table.Rows.Last().Cells[2].Width = table.Rows.First().Cells[2].Width;
                                        table.Rows.Last().Cells[3].Width = table.Rows.First().Cells[3].Width;
                                        table.Rows.Last().Cells[4].Width = table.Rows.First().Cells[4].Width;
                                        table.Rows.Last().Cells[5].Width = table.Rows.First().Cells[5].Width;
                                    }
                                }
                            }

                            table.SetBorder(TableBorderType.Left, new Border(BorderStyle.Tcbs_single, BorderSize.five, 0, System.Drawing.Color.Black));
                            table.SetBorder(TableBorderType.Right, new Border(BorderStyle.Tcbs_single, BorderSize.five, 0, System.Drawing.Color.Black));
                            table.SetBorder(TableBorderType.Top, new Border(BorderStyle.Tcbs_single, BorderSize.five, 0, System.Drawing.Color.Black));
                            table.SetBorder(TableBorderType.Bottom, new Border(BorderStyle.Tcbs_single, BorderSize.five, 0, System.Drawing.Color.Black));
                            check = true;
                        }

                        if (check)
                        {
                            document.ReplaceText(PresetConfig.TotalDaysPlaceholder, totalDays.ToString());
                            document.ReplaceText(PresetConfig.TotalPricePlaceholder, totalPrice.ToString());
                        }
                    }

                    // Finally replace the parsed values and save the document. Don't forget to also save the output document's url.
                    document.ReplaceText(PresetConfig.StartDatePlaceholder, startString);
                    document.ReplaceText(PresetConfig.EndDatePlaceholder, endString);

                    document.Save();
                    returnList.Add(destinationUrl);

                    saveDelegate(memory, projectUrl, documentLibrary, fileName);
                }
            }
            catch (Exception ex)
            {
                errorDelegate("<h1>Error at project " + projectCode + "</h1><p>" + ex.Message + Environment.NewLine + ex.TargetSite + "</p>");
            }

            // Return a list of all processed ISO documents.
            return returnList;
        }

        /// <summary>
        /// Writes the necessary ISO files and stores the url into a list to be returned.
        /// </summary>
        /// <param name="itemList">List of project data.</param>
        /// <param name="fileName">Output file name.</param>
        /// <param name="memory">Stream object containing all template file data.</param>
        /// <returns>Returns a MemoryStream object containing all file data.</returns>
        public MemoryStream WriteIntermediaryReport(List<Item> itemList, MemoryStream memory)
        {
            // Set the default values and pick up the correct PAF template.
            string title = itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ProjectTitle)).First().Values.First().First();
            if (title == null || title.Equals(""))
            {
                title = "New project";
            }

            try
            {
                //using (DocX document = DocX.Load(destinationUrl))
                using (DocX document = DocX.Load(memory))
                {
                    // Replace as many placeholders that can be replaced with single statements.
                    document.ReplaceText(PresetConfig.TitlePlaceholder, title);
                    document.ReplaceText(PresetConfig.VersionPlaceholder, "1");
                    document.ReplaceText(PresetConfig.ReferencePlaceholder, "");
                    document.ReplaceText(PresetConfig.EntityPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.Entity)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.ClientEndResponsiblePlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ExternalEndResponsibleName)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.ClientProjectResponsiblePlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ExternalProjectResponsibleName)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.MobiusProjectResponsiblePlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.InternalProjectResponsibleName)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.MobiusEndResponsiblePlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.InternalEndResponsibleName)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.ClientNamePlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ClientName)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.SubcontractorPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.SubcontractorPresent)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.StreetPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ClientLocationStreet)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.CommunityPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ClientLocationCommunity)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.CountryPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ClientLocationCountry)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.TaxNumberPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.TaxNumber)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.ApprovalPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.Approval)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.OrderNumberPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.OrderNumber)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.DescriptionPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.Description)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.SectorPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.Sector)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.DeliverablesPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.Deliverables)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.DependenciesPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.Dependencies)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.BudgetExclPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.Budget)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.BudgetInclPlaceholder, (Convert.ToDouble((itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.Budget)).First().Values.First().First() != null && !itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.Budget)).First().Values.First().First().Equals("")) ? itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.Budget)).First().Values.First().First() : "0") * (Convert.ToDouble((itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.TaxRate)).First().Values.First().First() != null && !itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.TaxRate)).First().Values.First().First().Equals("")) ? itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.TaxRate)).First().Values.First().First() : "0") + 100) / 100).ToString());
                    document.ReplaceText(PresetConfig.TaxRatePlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.TaxRate)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.FacturationTypePlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.FacturationType)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.PaymentConditionsPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.PaymentConditions)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.FacturationSchedulePlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.FacturationSchedule)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.FirstPhaseDatePlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.FirstStageFacturationDate)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.FirstPhaseAmountPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.FirstStageFacturationAmount)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.SecondPhaseDatePlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.SecondStageFacturationDate)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.SecondPhaseAmountPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.SecondStageFacturationAmount)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.ThirdPhaseDatePlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ThirdStageFacturationDate)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.ThirdPhaseAmountPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ThirdStageFacturationAmount)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.FourthPhaseDatePlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.FourthStageFacturationDate)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.FourthPhaseAmountPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.FourthStageFacturationAmount)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.PeriodicalDatePlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.PeriodicalFacturationDate)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.PeriodicalAmountPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.PeriodicalFacturationAmount)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.BillableCostsPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.BillableCosts)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.BillableCostsPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.BillableCosts)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.TotalKmPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.TotalKm)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.KmCostPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.EuroPerKm)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.ParkingPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ParkingAvailable)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.OtherBinaryPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.OtherAvailable)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.OtherPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.OtherSpecification)).First().Values.First().First());
                    document.ReplaceText(PresetConfig.InternalMeetingsPlaceholder, itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.InternalMeetings)).First().Values.First().First());

                    // Parse what needs to be parsed.
                    string startString = itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.StartDate)).First().Values.First().First();
                    string endString = itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.EndDate)).First().Values.First().First();

                    try
                    {
                        if (startString != null && !startString.Equals(""))
                        {
                            DateTime start = DateTime.Parse(startString);

                            if (start.TimeOfDay.Hours >= 20)
                            {
                                start = start.AddDays(1);
                            }

                            startString = start.ToShortDateString();
                        }
                    }
                    catch (Exception ex)
                    {
                        startString = "";
                    }

                    try
                    {
                        if (endString != null && !endString.Equals(""))
                        {
                            DateTime end = DateTime.Parse(endString);

                            if (end.TimeOfDay.Hours >= 20)
                            {
                                end = end.AddDays(1);
                            }

                            endString = end.ToShortDateString();
                        }
                    }
                    catch (Exception ex)
                    {
                        endString = "";
                    }

                    // Replace the logo placeholder with the logo.
                    if (itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.LogoUrl)).First().Values.First().First() != null && !itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.LogoUrl)).First().Values.First().First().Equals(""))
                    {
                        WebRequest req = WebRequest.Create(itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.LogoUrl)).First().Values.First().First());
                        req.Credentials = CredentialCache.DefaultCredentials;
                        req.UseDefaultCredentials = true;
                        WebResponse resp = req.GetResponse();
                        Stream stream = resp.GetResponseStream();
                        MemoryStream lifestream = new MemoryStream();
                        stream.CopyTo(lifestream);
                        Image image = document.AddImage(lifestream);

                        Picture pic = image.CreatePicture();
                        double tempWidth = pic.Width;
                        double tempHeight = pic.Height;

                        if (tempWidth > 240)
                        {
                            tempHeight = (tempHeight / tempWidth) * 240;
                            tempWidth = 240;
                        }

                        if (tempHeight > 150)
                        {
                            tempWidth = (tempWidth / tempHeight) * 150;
                            tempHeight = 150;
                        }

                        pic.Width = Convert.ToInt32(tempWidth);
                        pic.Height = Convert.ToInt32(tempHeight);

                        foreach (Paragraph paragraph in document.Paragraphs)
                        {
                            List<int> valueIndices = paragraph.FindAll(PresetConfig.LogoPlaceholder);

                            foreach (int index in valueIndices)
                            {
                                paragraph.RemoveText(index, PresetConfig.LogoPlaceholder.Length);
                                paragraph.InsertPicture(pic, index);
                            }
                        }
                    }
                    else
                    {
                        document.ReplaceText(PresetConfig.LogoPlaceholder, "");
                    }

                    // Replace the resource table with all resource and subcontractor information.
                    List<List<string>> resourceNameList = itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ResourceNames)).First().Values;
                    List<List<string>> resourceRoleList = itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ResourceRoles)).First().Values;
                    List<List<string>> resourceDayRateList = itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ResourceDayRates)).First().Values;
                    List<List<string>> resourceHourRateList = itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ResourceHourRates)).First().Values;
                    List<List<string>> resourceDaysList = itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.ResourceDays)).First().Values;
                    List<List<string>> subcontractorNameList = itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.SubcontractorNames)).First().Values;
                    List<List<string>> subcontractorRoleList = itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.SubcontractorRoles)).First().Values;
                    List<List<string>> subcontractorDayRateList = itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.SubcontractorDayRates)).First().Values;
                    List<List<string>> subcontractorHourRateList = itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.SubcontractorHourRates)).First().Values;
                    List<List<string>> subcontractorDaysList = itemList.Where(i => i.ConfigItem.Variable.Equals(PresetConfig.SubcontractorDays)).First().Values;

                    // Loop through the tables to search for the one containing resource and subcontractor information and fill the correct table.
                    foreach (Table table in document.Tables)
                    {
                        double totalDays = 0;
                        double totalPrice = 0;
                        bool check = false;

                        if (table.Xml.ToString().Contains(PresetConfig.ResourceTable))
                        {
                            for (int x = 0; x < resourceNameList.Count; x++)
                            {
                                for (int y = 0; y < resourceNameList[x].Count; y++)
                                {
                                    if ((resourceNameList[x][y] != null && !resourceNameList[x][y].Equals("")) || (resourceRoleList[x][y] != null && !resourceRoleList[x][y].Equals("")) || (resourceDayRateList[x][y] != null && !resourceDayRateList[x][y].Equals("")) || (resourceHourRateList[x][y] != null && !resourceHourRateList[x][y].Equals("")) || (resourceDaysList[x][y] != null && !resourceDaysList[x][y].Equals("")))
                                    {
                                        double days = ((resourceDaysList[x][y] != null && !resourceDaysList[x][y].Equals("")) ? Convert.ToDouble(resourceDaysList[x][y]) : 0);
                                        double price = ((resourceDayRateList[x][y] != null && !resourceDayRateList[x][y].Equals("")) ? Convert.ToDouble(resourceDayRateList[x][y]) : 0);
                                        totalDays += days;
                                        totalPrice += (price * days);

                                        table.InsertRow();
                                        table.Rows.Last().Cells[0].Paragraphs.First().InsertText(resourceRoleList[x][y]);
                                        table.Rows.Last().Cells[1].Paragraphs.First().InsertText(resourceNameList[x][y]);
                                        table.Rows.Last().Cells[2].Paragraphs.First().InsertText(resourceDayRateList[x][y]);
                                        table.Rows.Last().Cells[3].Paragraphs.First().InsertText(resourceHourRateList[x][y]);
                                        table.Rows.Last().Cells[4].Paragraphs.First().InsertText(resourceDaysList[x][y]);
                                        table.Rows.Last().Cells[5].Paragraphs.First().InsertText((days * price).ToString());

                                        table.Rows.Last().Cells[0].Width = table.Rows.First().Cells[0].Width;
                                        table.Rows.Last().Cells[1].Width = table.Rows.First().Cells[1].Width;
                                        table.Rows.Last().Cells[2].Width = table.Rows.First().Cells[2].Width;
                                        table.Rows.Last().Cells[3].Width = table.Rows.First().Cells[3].Width;
                                        table.Rows.Last().Cells[4].Width = table.Rows.First().Cells[4].Width;
                                        table.Rows.Last().Cells[5].Width = table.Rows.First().Cells[5].Width;
                                    }
                                }
                            }

                            for (int x = 0; x < subcontractorNameList.Count; x++)
                            {
                                for (int y = 0; y < subcontractorNameList[x].Count; y++)
                                {
                                    if ((subcontractorNameList[x][y] != null && !subcontractorNameList[x][y].Equals("")) || (subcontractorRoleList[x][y] != null && !subcontractorRoleList[x][y].Equals("")) || (subcontractorDayRateList[x][y] != null && !subcontractorDayRateList[x][y].Equals("")) || (subcontractorHourRateList[x][y] != null && !subcontractorHourRateList[x][y].Equals("")) || (subcontractorDaysList[x][y] != null && !subcontractorDaysList[x][y].Equals("")))
                                    {
                                        double days = ((subcontractorDaysList[x][y] != null && !subcontractorDaysList[x][y].Equals("")) ? Convert.ToDouble(subcontractorDaysList[x][y]) : 0);
                                        double price = ((subcontractorDayRateList[x][y] != null && !subcontractorDayRateList[x][y].Equals("")) ? Convert.ToDouble(subcontractorDayRateList[x][y]) : 0);
                                        totalDays += days;
                                        totalPrice += (price * days);

                                        table.InsertRow();
                                        table.Rows.Last().Cells[0].Paragraphs.First().InsertText(subcontractorRoleList[x][y]);
                                        table.Rows.Last().Cells[1].Paragraphs.First().InsertText(subcontractorNameList[x][y]);
                                        table.Rows.Last().Cells[2].Paragraphs.First().InsertText(subcontractorDayRateList[x][y]);
                                        table.Rows.Last().Cells[3].Paragraphs.First().InsertText(subcontractorHourRateList[x][y]);
                                        table.Rows.Last().Cells[4].Paragraphs.First().InsertText(subcontractorDaysList[x][y]);
                                        table.Rows.Last().Cells[5].Paragraphs.First().InsertText(((subcontractorDaysList[x][y] != null && !subcontractorDaysList[x][y].Equals("") && subcontractorDayRateList[x][y] != null && !subcontractorDayRateList[x][y].Equals("")) ? (Convert.ToDouble(subcontractorDaysList[x][y]) * Convert.ToDouble(subcontractorDayRateList[x][y])).ToString() : "0"));

                                        table.Rows.Last().Cells[0].Width = table.Rows.First().Cells[0].Width;
                                        table.Rows.Last().Cells[1].Width = table.Rows.First().Cells[1].Width;
                                        table.Rows.Last().Cells[2].Width = table.Rows.First().Cells[2].Width;
                                        table.Rows.Last().Cells[3].Width = table.Rows.First().Cells[3].Width;
                                        table.Rows.Last().Cells[4].Width = table.Rows.First().Cells[4].Width;
                                        table.Rows.Last().Cells[5].Width = table.Rows.First().Cells[5].Width;
                                    }
                                }
                            }

                            table.SetBorder(TableBorderType.Left, new Border(BorderStyle.Tcbs_single, BorderSize.five, 0, System.Drawing.Color.Black));
                            table.SetBorder(TableBorderType.Right, new Border(BorderStyle.Tcbs_single, BorderSize.five, 0, System.Drawing.Color.Black));
                            table.SetBorder(TableBorderType.Top, new Border(BorderStyle.Tcbs_single, BorderSize.five, 0, System.Drawing.Color.Black));
                            table.SetBorder(TableBorderType.Bottom, new Border(BorderStyle.Tcbs_single, BorderSize.five, 0, System.Drawing.Color.Black));
                            check = true;
                        }

                        if (check)
                        {
                            document.ReplaceText(PresetConfig.TotalDaysPlaceholder, totalDays.ToString());
                            document.ReplaceText(PresetConfig.TotalPricePlaceholder, totalPrice.ToString());
                        }
                    }

                    // Finally replace the parsed values and save the document. Don't forget to also save the output document's url.
                    document.ReplaceText(PresetConfig.StartDatePlaceholder, startString);
                    document.ReplaceText(PresetConfig.EndDatePlaceholder, endString);

                    document.Save();
                }
            }
            catch (Exception ex)
            {
                errorDelegate("<h1>Error at project " + title + "</h1><p>" + ex.Message + Environment.NewLine + ex.TargetSite + "</p>");
                return null;
            }

            // Return a list of all processed ISO documents.
            return memory;
        }

        /// <summary>
        /// Upgrades all member permission groups from having contribute rights to full control.
        /// </summary>
        public void UpgradeMemberPermissions()
        {
            try
            {
                ClientContext ctx = new ClientContext(PresetConfig.DefaultProjectUrl);
                ctx.Load(ctx.Web);
                ctx.Load(ctx.Web.Webs);
                ctx.Load(ctx.Web.Lists);
                ctx.ExecuteQuery();

                List<string> errors = new List<string>();
                string msg = "";
                string current = "";

                foreach (Web site in ctx.Web.Webs)
                {
                    try
                    {
                        ctx.Load(site);
                        ctx.ExecuteQuery();

                        current = site.Title;

                        Group members = site.AssociatedMemberGroup;
                        ctx.Load(members);
                        ctx.ExecuteQuery();

                        RoleAssignment assignment = site.RoleAssignments.GetByPrincipal(members);
                        ctx.Load(assignment);
                        ctx.Load(assignment.RoleDefinitionBindings);
                        ctx.ExecuteQuery();

                        RoleDefinition definition = ctx.Web.RoleDefinitions.GetByType(RoleType.Administrator);
                        ctx.Load(definition);
                        ctx.ExecuteQuery();

                        try
                        {
                            assignment.RoleDefinitionBindings.Add(definition);
                            assignment.RoleDefinitionBindings.Remove(site.RoleDefinitions.GetByType(RoleType.Contributor));
                            assignment.Update();
                            ctx.ExecuteQuery();
                        }
                        catch (Exception ex)
                        {
                            // Getting an error here means the role definition was already added to the group.
                        }
                    }
                    catch (Exception ex)
                    {
                        // We might get errors with individual items, but don't let it bother the other updates.
                        errors.Add("<h1>Error at project " + current + "</h1><p>Project " + current + " - An error occurred while changing the member group permissions: " + ex.Message + "</p>");
                    }
                }

                if (errors.Count != 0)
                {
                    foreach (string error in errors)
                    {
                        msg += "<p>" + error + "</p>";
                    }

                    errorDelegate(msg);
                }
            }
            catch (Exception ex)
            {
                errorDelegate("<p>An error occurred</p><p>" + ex.Message + "<p></p>" + ex.TargetSite + "</p>");
            }
        }

        /// <summary>
        /// Reads all information concerning a project from the PAF Project List and PAF Resources lists.
        /// </summary>
        public bool ReadPAFInfo(int pafProjectId, bool ignoreCode = false)
        {
            configData = new List<Item>();
            bool found = false;

            try
            {
                query = new CamlQuery();
                query.ViewXml = "<View />";

                ClientContext context = new ClientContext(PresetConfig.DefaultISOUrl);
                context.Load(context.Web);
                context.ExecuteQuery();

                List pafProjects = context.Web.Lists.GetByTitle(PresetConfig.PAFProjectList);
                context.Load(pafProjects);
                context.ExecuteQuery();

                ListItemCollection projectItems = pafProjects.GetItems(query);
                context.Load(projectItems);
                context.ExecuteQuery();

                List pafResources = context.Web.Lists.GetByTitle(PresetConfig.PAFResourceList);
                context.Load(pafResources);
                context.ExecuteQuery();

                ListItemCollection resourceItems = pafResources.GetItems(query);
                context.Load(resourceItems);
                context.ExecuteQuery();

                ListItem project = projectItems.First();
                List<ListItem> linkedResources = new List<ListItem>();

                // Loop through the PAF Project List items and select the correct one by id.
                foreach (ListItem projectItem in projectItems)
                {
                    if (int.Parse(projectItem[PresetConfig.SPProjectID].ToString()) == pafProjectId)
                    {
                        project = projectItem;
                        found = true;
                        break;
                    }
                }

                // Loop through each and every resource in the PAF Resources list and stock the ones related to the project in our personal list.
                foreach (ListItem resourceItem in resourceItems)
                {
                    if (int.Parse(resourceItem[PresetConfig.SPResourceProjectId].ToString()) == pafProjectId)
                    {
                        linkedResources.Add(resourceItem);
                    }
                }

                // A project code has to be filled in.
                if ((project[PresetConfig.SPProjectCode] == null || project[PresetConfig.SPProjectCode].Equals("")) && !ignoreCode)
                {
                    errorDelegate("<h1>Error at project " + pafProjectId + "</h1><p>Can't create a project without a project code.\nPlease fill in a project code and try again.</p>");
                    return false;
                }

                if (projectItems != null && projectItems.Count != 0 && project != null && found)
                {
                    context.Load(project.AttachmentFiles);
                    context.ExecuteQuery();

                    // Fill the list of Item objects with as much inline information as possible.
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.Language)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPLanguage] != null && (project[PresetConfig.SPLanguage].ToString().Equals("Nederlands") || project[PresetConfig.SPLanguage].ToString().Equals("Français") || project[PresetConfig.SPLanguage].ToString().Equals("English"))) ? project[PresetConfig.SPLanguage].ToString() : "English") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.ProjectTitle)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPProjectTitle] != null) ? project[PresetConfig.SPProjectTitle].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.ClientName)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPClientName] != null) ? project[PresetConfig.SPClientName].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.ClientLocationStreet)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPClientLocationStreet] != null) ? project[PresetConfig.SPClientLocationStreet].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.ClientLocationCommunity)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPClientLocationCommunity] != null) ? project[PresetConfig.SPClientLocationCommunity].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.ClientLocationCountry)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPClientLocationCountry] != null) ? project[PresetConfig.SPClientLocationCountry].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.SubcontractorPresent)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPSubcontractorPresent] != null) ? project[PresetConfig.SPSubcontractorPresent].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.Approval)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPApproval] != null) ? project[PresetConfig.SPApproval].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.InternalControlFrequency)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPInternalControlFrequency] != null) ? project[PresetConfig.SPInternalControlFrequency].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.CRMStatus)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPCRMStatus] != null) ? project[PresetConfig.SPCRMStatus].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.FacturationSchedule)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPFacturationSchedule] != null) ? project[PresetConfig.SPFacturationSchedule].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.OtherCommentary)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPOtherCommentary] != null) ? project[PresetConfig.SPOtherCommentary].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.InternalMeetings)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPInternalMeetings] != null) ? project[PresetConfig.SPInternalMeetings].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.TotalKm)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SMLToClient] != null) ? project[PresetConfig.SMLToClient].ToString() : ((project[PresetConfig.GenvalToClient] != null) ? project[PresetConfig.GenvalToClient].ToString() : "")) } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.ParkingAvailable)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPParkingAvailable] != null) ? project[PresetConfig.SPParkingAvailable].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.OtherAvailable)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPOtherAvailable] != null) ? project[PresetConfig.SPOtherAvailable].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.ProjectConfidentialityStatus)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPProjectConfidentialityStatus] != null) ? project[PresetConfig.SPProjectConfidentialityStatus].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.PowerpointLogoUsage)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPPowerpointLogoUsage] != null) ? project[PresetConfig.SPPowerpointLogoUsage].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.FirstStageFacturationDate)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPFirstStageFacturationDate] != null) ? project[PresetConfig.SPFirstStageFacturationDate].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.FirstStageFacturationAmount)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPFirstStageFacturationAmount] != null) ? project[PresetConfig.SPFirstStageFacturationAmount].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.SecondStageFacturationDate)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPSecondStageFacturationDate] != null) ? project[PresetConfig.SPSecondStageFacturationDate].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.SecondStageFacturationAmount)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPSecondStageFacturationAmount] != null) ? project[PresetConfig.SPSecondStageFacturationAmount].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.ThirdStageFacturationDate)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPThirdStageFacturationDate] != null) ? project[PresetConfig.SPThirdStageFacturationDate].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.ThirdStageFacturationAmount)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPThirdStageFacturationAmount] != null) ? project[PresetConfig.SPThirdStageFacturationAmount].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.FourthStageFacturationDate)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPFourthStageFacturationDate] != null) ? project[PresetConfig.SPFourthStageFacturationDate].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.FourthStageFacturationAmount)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPFourthStageFacturationAmount] != null) ? project[PresetConfig.SPFourthStageFacturationAmount].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.PeriodicalFacturationDate)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPPeriodicalFacturationDate] != null) ? project[PresetConfig.SPPeriodicalFacturationDate].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.PeriodicalFacturationAmount)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPPeriodicalFacturationAmount] != null) ? project[PresetConfig.SPPeriodicalFacturationAmount].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.GenvalToClient)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPGenvalToClient] != null) ? project[PresetConfig.SPGenvalToClient].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.SMLToClient)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPSMLToClient] != null) ? project[PresetConfig.SPSMLToClient].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.OtherSpecification)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPOtherSpecification] != null) ? project[PresetConfig.SPOtherSpecification].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.OtherCost)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPOtherCost] != null) ? project[PresetConfig.SPOtherCost].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.FacturationInFunctionOf)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPFacturationInFunctionOf] != null) ? project[PresetConfig.SPFacturationInFunctionOf].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.EuroPerKm)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPEuroPerKm] != null) ? project[PresetConfig.SPEuroPerKm].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.Entity)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPEntity] != null) ? project[PresetConfig.SPEntity].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.Description)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPDescription] != null) ? project[PresetConfig.SPDescription].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.Summary)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPSummary] != null) ? project[PresetConfig.SPSummary].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.Deliverables)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPDeliverables] != null) ? project[PresetConfig.SPDeliverables].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.Dependencies)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPDependencies] != null) ? project[PresetConfig.SPDependencies].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.Budget)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPBudget] != null) ? project[PresetConfig.SPBudget].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.Sector)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPSector] != null) ? project[PresetConfig.SPSector].ToString() : ""), ((project["Branch"] != null) ? project["Branch"].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.StartDate)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPStartDate] != null) ? project[PresetConfig.SPStartDate].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.EndDate)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPEndDate] != null) ? project[PresetConfig.SPEndDate].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.TaxNumber)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPTaxNumber] != null) ? project[PresetConfig.SPTaxNumber].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.ProjectCode)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPProjectCode] != null) ? project[PresetConfig.SPProjectCode].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.OrderNumber)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPOrderNumber] != null) ? project[PresetConfig.SPOrderNumber].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.ExternalProjectResponsibleName)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPExternalProjectResponsibleFirstName] != null && project[PresetConfig.SPExternalProjectResponsibleLastName] != null && !PresetConfig.FirstNameStrings.Contains(project[PresetConfig.SPExternalProjectResponsibleFirstName].ToString()) && !PresetConfig.LastNameStrings.Contains(project[PresetConfig.SPExternalProjectResponsibleLastName].ToString())) ? project[PresetConfig.SPExternalProjectResponsibleLastName].ToString() + ", " + project[PresetConfig.SPExternalProjectResponsibleFirstName].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.ExternalProjectResponsibleMail)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPExternalProjectResponsiblMail] != null) ? project[PresetConfig.SPExternalProjectResponsiblMail].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.ExternalEndResponsibleName)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPExternalEndResponsibleFirstName] != null && project[PresetConfig.SPExternalEndResponsibleLastName] != null && !PresetConfig.FirstNameStrings.Contains(project[PresetConfig.SPExternalEndResponsibleFirstName].ToString()) && !PresetConfig.LastNameStrings.Contains(project[PresetConfig.SPExternalEndResponsibleLastName].ToString())) ? project[PresetConfig.SPExternalEndResponsibleLastName].ToString() + ", " + project[PresetConfig.SPExternalEndResponsibleFirstName].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.ExternalEndResponsibleMail)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPExternalEndResponsibleMail] != null) ? project[PresetConfig.SPExternalEndResponsibleMail].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.FacturationType)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPFacturationType] != null) ? project[PresetConfig.SPFacturationType].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.PaymentConditions)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPPaymentConditions] != null) ? project[PresetConfig.SPPaymentConditions].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.ProcedureDefection)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPProcedureDefection] != null) ? project[PresetConfig.SPProcedureDefection].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.DefectionReason)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPDefectionReason] != null) ? project[PresetConfig.SPDefectionReason].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.VersionDescription)).First(), new List<List<string>> { new List<string> { "", "", "", "", "", "" } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.VersionInfluence)).First(), new List<List<string>> { new List<string> { "", "", "", "", "", "" } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.VersionDate)).First(), new List<List<string>> { new List<string> { "", "", "", "", "", "" } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.OtherCostTitles)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPOtherCostTitles1] != null) ? project[PresetConfig.SPOtherCostTitles1].ToString() : ""), ((project[PresetConfig.SPOtherCostTitles2] != null) ? project[PresetConfig.SPOtherCostTitles2].ToString() : ""), ((project[PresetConfig.SPOtherCostTitles3] != null) ? project[PresetConfig.SPOtherCostTitles3].ToString() : ""), ((project[PresetConfig.SPOtherCostTitles4] != null) ? project[PresetConfig.SPOtherCostTitles4].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.OtherCostPrices)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPOtherCostPrices1] != null) ? project[PresetConfig.SPOtherCostPrices1].ToString() : ""), ((project[PresetConfig.SPOtherCostPrices2] != null) ? project[PresetConfig.SPOtherCostPrices2].ToString() : ""), ((project[PresetConfig.SPOtherCostPrices3] != null) ? project[PresetConfig.SPOtherCostPrices3].ToString() : ""), ((project[PresetConfig.SPOtherCostPrices4] != null) ? project[PresetConfig.SPOtherCostPrices4].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.StrategicalMoment)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPStrategicalMoment] != null) ? project[PresetConfig.SPStrategicalMoment].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.Approach)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPApproach] != null) ? project[PresetConfig.SPApproach].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.Results)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPResults] != null) ? project[PresetConfig.SPResults].ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.LogoUrl)).First(), new List<List<string>> { new List<string> { ((project.AttachmentFiles.Count > 0 && project.AttachmentFiles.Where(i => SharePointHelper.ContainsExtension(i.ServerRelativeUrl, PresetConfig.ImageExtensions)).Count() != 0) ? PresetConfig.DefaultSiteUrl + project.AttachmentFiles.Where(i => SharePointHelper.ContainsExtension(i.ServerRelativeUrl, PresetConfig.ImageExtensions)).First().ServerRelativeUrl.ToString() : "") } }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.TaxRate)).First(), new List<List<string>> { new List<string> { ((project[PresetConfig.SPTaxRate] != null) ? project[PresetConfig.SPTaxRate].ToString() : "0") } }));

                    // Add all attachments as a string to the Item list.
                    if (project.AttachmentFiles.Count > 0)
                    {
                        List<List<string>> attachmentUrls = new List<List<string>>();

                        foreach (Attachment attachment in project.AttachmentFiles)
                        {
                            attachmentUrls.Add(new List<string> { PresetConfig.DefaultSiteUrl + attachment.ServerRelativeUrl.ToString() });
                        }

                        configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.Attachments)).First(), attachmentUrls));
                    }

                    // See whether or not other costs have been filled in and set the choice of BillableCosts to either Yes or No.
                    if (((project[PresetConfig.SPEuroPerKm] != null && !project[PresetConfig.SPEuroPerKm].ToString().Equals("") && !project[PresetConfig.SPEuroPerKm].ToString().Equals("0")) || (project[PresetConfig.SPSMLToClient] != null && !project[PresetConfig.SPSMLToClient].ToString().Equals("") && !project[PresetConfig.SPSMLToClient].ToString().Equals("0")) || (project[PresetConfig.SPGenvalToClient] != null && !project[PresetConfig.SPGenvalToClient].ToString().Equals("") && !project[PresetConfig.SPGenvalToClient].ToString().Equals("0")) || (project[PresetConfig.SPParkingAvailable] != null && !project[PresetConfig.SPParkingAvailable].Equals("") && !project[PresetConfig.SPParkingAvailable].Equals("No") && !project[PresetConfig.SPParkingAvailable].Equals("Non") && !project[PresetConfig.SPParkingAvailable].Equals("Nee")) || (project[PresetConfig.SPOtherAvailable] != null && !project[PresetConfig.SPOtherAvailable].Equals("") && !project[PresetConfig.SPOtherAvailable].Equals("No") && !project[PresetConfig.SPOtherAvailable].Equals("Non") && !project[PresetConfig.SPOtherAvailable].Equals("Nee")) || (project[PresetConfig.SPOtherSpecification] != null && !project[PresetConfig.SPOtherSpecification].Equals("Please Specify") && !project[PresetConfig.SPOtherSpecification].Equals("Specifieer Aub") && !project[PresetConfig.SPOtherSpecification].Equals("Veuillez Préciser")) || (project[PresetConfig.SPOtherCost] != null && !project[PresetConfig.SPOtherCost].ToString().Equals("") && !project[PresetConfig.SPOtherCost].ToString().Equals("0") && !project[PresetConfig.SPOtherCost].ToString().Equals("€"))))
                    {
                        configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.BillableCosts)).First(), new List<List<string>> { new List<string> { "Yes" } }));
                    }
                    else
                    {
                        configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.BillableCosts)).First(), new List<List<string>> { new List<string> { "No" } }));
                    }

                    // Retrieve the necessary internal responsible information.
                    if (project[PresetConfig.SPInternalEndResponsible] != null)
                    {
                        User internalEndResponsible = context.Web.EnsureUser(((FieldUserValue)project[PresetConfig.SPInternalEndResponsible]).LookupValue);
                        context.Load(internalEndResponsible);
                        context.ExecuteQuery();

                        configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.InternalEndResponsibleName)).First(), new List<List<string>> { new List<string> { internalEndResponsible.Title, internalEndResponsible.Title } }));
                        configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.InternalEndResponsibleMail)).First(), new List<List<string>> { new List<string> { internalEndResponsible.Email } }));
                    }
                    else
                    {
                        configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.InternalEndResponsibleName)).First(), new List<List<string>> { new List<string> { "" } }));
                        configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.InternalEndResponsibleMail)).First(), new List<List<string>> { new List<string> { "" } }));
                    }

                    if (project[PresetConfig.SPInternalProjectResponsible] != null)
                    {
                        User internalProjectResponsible = context.Web.EnsureUser(((FieldUserValue)project[PresetConfig.SPInternalProjectResponsible]).LookupValue);
                        context.Load(internalProjectResponsible);
                        context.ExecuteQuery();

                        configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.InternalProjectResponsibleName)).First(), new List<List<string>> { new List<string> { internalProjectResponsible.Title } }));
                        configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.InternalProjectResponsibleMail)).First(), new List<List<string>> { new List<string> { internalProjectResponsible.Email } }));
                    }
                    else
                    {
                        configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.InternalProjectResponsibleName)).First(), new List<List<string>> { new List<string> { "" } }));
                        configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.InternalProjectResponsibleMail)).First(), new List<List<string>> { new List<string> { "" } }));
                    }

                    // Start loading the resources (and format them as they would be read from an Excel file).
                    List<string> resourceNames = new List<string>();
                    List<string> resourceRoles = new List<string>();
                    List<string> resourceHourRates = new List<string>();
                    List<string> resourceDayRates = new List<string>();
                    List<string> resourceDays = new List<string>();
                    List<List<string>> resourceMonthlyDays = new List<List<string>>();
                    List<string> subcontractorNames = new List<string>();
                    List<string> subcontractorRoles = new List<string>();
                    List<string> subcontractorHourRates = new List<string>();
                    List<string> subcontractorDayRates = new List<string>();
                    List<string> subcontractorDays = new List<string>();
                    List<List<string>> subcontractorMonthlyDays = new List<List<string>>();
                    double timeOfCompletion = 0;
                    string previousResourceRole = "";
                    string previousSubcontractorRole = "";

                    // Parse the resource and subcontractor data into List objects.
                    foreach (ListItem resource in linkedResources)
                    {
                        string resourceType = ((resource[PresetConfig.SPResourceType] != null) ? resource[PresetConfig.SPResourceType].ToString() : "Employee");

                        if (resourceType.Equals("Employee") || resourceType.Equals("Employé") || resourceType.Equals("Werknemer"))
                        {
                            if (resource[PresetConfig.SPResource] == null)
                            {
                                continue;
                            }

                            User assignedUser = context.Web.EnsureUser(((FieldUserValue)resource[PresetConfig.SPResource]).LookupValue);
                            context.Load(assignedUser);
                            context.ExecuteQuery();

                            string resourceRole = ((resource[PresetConfig.SPResourceProfile] != null) ? resource[PresetConfig.SPResourceProfile].ToString() : "");
                            string roleValue = "";

                            if (!resourceRole.Equals(previousResourceRole) && !resourceRole.Equals(""))
                            {
                                roleValue = resourceRole;
                                previousResourceRole = resourceRole;
                            }

                            resourceNames.Add(((resource[PresetConfig.SPResource] != null) ? assignedUser.Title : ""));
                            resourceRoles.Add(roleValue);
                            resourceHourRates.Add(((resource[PresetConfig.SPResourceRate] != null) ? (Convert.ToDouble(resource[PresetConfig.SPResourceRate].ToString()) / 8).ToString() : "0"));
                            resourceDayRates.Add(((resource[PresetConfig.SPResourceRate] != null) ? resource[PresetConfig.SPResourceRate].ToString() : "0"));

                            List<string> subList = new List<string>();
                            double days = 0;

                            foreach (string s in PresetConfig.MonthFields)
                            {
                                subList.Add(((resource[s] != null) ? resource[s].ToString() : ""));
                                days += ((resource[s] != null) ? Convert.ToDouble(resource[s].ToString()) : 0);
                                timeOfCompletion += ((resource[s] != null) ? Convert.ToDouble(resource[s].ToString()) : 0);
                            }

                            resourceDays.Add(days.ToString());
                            resourceMonthlyDays.Add(subList);
                        }
                        else
                        {
                            if (resource[PresetConfig.SPSubcontractor] == null || resource[PresetConfig.SPSubcontractor].Equals(""))
                            {
                                continue;
                            }

                            string resourceRole = ((resource[PresetConfig.SPResourceProfile] != null) ? resource[PresetConfig.SPResourceProfile].ToString() : "Subcontractor");
                            string roleValue = "";

                            if (!resourceRole.Equals(previousSubcontractorRole) && !resourceRole.Equals(""))
                            {
                                roleValue = resourceRole;
                                previousSubcontractorRole = resourceRole;
                            }

                            subcontractorNames.Add(((resource[PresetConfig.SPSubcontractor] != null) ? resource[PresetConfig.SPSubcontractor].ToString() : ""));
                            subcontractorRoles.Add(roleValue);
                            subcontractorHourRates.Add(((resource[PresetConfig.SPResourceRate] != null) ? (Convert.ToDouble(resource[PresetConfig.SPResourceRate].ToString()) / 8).ToString() : "0"));
                            subcontractorDayRates.Add(((resource[PresetConfig.SPResourceRate] != null) ? resource[PresetConfig.SPResourceRate].ToString() : "0"));

                            List<string> subList = new List<string>();
                            double days = 0;

                            foreach (string s in PresetConfig.MonthFields)
                            {
                                subList.Add(((resource[s] != null) ? resource[s].ToString() : ""));
                                days += ((resource[s] != null) ? Convert.ToDouble(resource[s].ToString()) : 0);
                                timeOfCompletion += ((resource[s] != null) ? Convert.ToDouble(resource[s].ToString()) : 0);
                            }

                            subcontractorDays.Add(days.ToString());
                            subcontractorMonthlyDays.Add(subList);
                        }
                    }

                    // First, inverse the resources (this sounds silly, but we'll need to do this in order to recreate an Excel design).
                    List<List<string>> inversedResources = new List<List<string>>();

                    if (resourceMonthlyDays.Count > 0)
                    {
                        for (int x = 0; x < resourceMonthlyDays.First().Count; x++)
                        {
                            List<string> personalPercentages = new List<string>();

                            for (int y = 0; y < resourceMonthlyDays.Count; y++)
                            {
                                if (resourceMonthlyDays[y][x] != null && !resourceMonthlyDays[y][x].Equals(""))
                                {
                                    personalPercentages.Add(resourceMonthlyDays[y][x].ToString());
                                }
                                else
                                {
                                    personalPercentages.Add("0");
                                }
                            }

                            inversedResources.Add(personalPercentages);
                        }
                    }
                    else
                    {
                        // This is to avoid errors about objects containing no values.
                        inversedResources.Add(new List<string> { "" });
                    }

                    // Next up, the subcontractors.
                    List<List<string>> inversedSubcontractors = new List<List<string>>();

                    if (subcontractorMonthlyDays.Count > 0)
                    {
                        for (int x = 0; x < subcontractorMonthlyDays.First().Count; x++)
                        {
                            List<string> personalPercentages = new List<string>();

                            for (int y = 0; y < subcontractorMonthlyDays.Count; y++)
                            {
                                if (subcontractorMonthlyDays[y][x] != null && !subcontractorMonthlyDays[y][x].Equals(""))
                                {
                                    personalPercentages.Add(subcontractorMonthlyDays[y][x].ToString());
                                }
                                else
                                {
                                    personalPercentages.Add("0");
                                }
                            }

                            inversedSubcontractors.Add(personalPercentages);
                        }
                    }
                    else
                    {
                        // This is to avoid errors about objects containing no values.
                        inversedSubcontractors.Add(new List<string> { "" });
                    }

                    // This is to avoid errors about objects containing no values.
                    if (resourceNames.Count == 0) resourceNames.Add("");
                    if (resourceRoles.Count == 0) resourceRoles.Add("");
                    if (resourceHourRates.Count == 0) resourceHourRates.Add("");
                    if (resourceDayRates.Count == 0) resourceDayRates.Add("");
                    if (resourceDays.Count == 0) resourceDays.Add("");
                    if (subcontractorNames.Count == 0) subcontractorNames.Add("");
                    if (subcontractorRoles.Count == 0) subcontractorRoles.Add("");
                    if (subcontractorHourRates.Count == 0) subcontractorHourRates.Add("");
                    if (subcontractorDayRates.Count == 0) subcontractorDayRates.Add("");
                    if (subcontractorDays.Count == 0) subcontractorDays.Add("");

                    // Add all resource data to the Item list.
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.ResourceNames)).First(), new List<List<string>>() { resourceNames }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.ResourceRoles)).First(), new List<List<string>>() { resourceRoles }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.ResourceHourRates)).First(), new List<List<string>>() { resourceHourRates }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.ResourceDayRates)).First(), new List<List<string>>() { resourceDayRates }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.ResourceDays)).First(), new List<List<string>>() { resourceDays }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.ResourceMonthlyDays)).First(), inversedResources));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.SubcontractorNames)).First(), new List<List<string>>() { subcontractorNames }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.SubcontractorRoles)).First(), new List<List<string>>() { subcontractorRoles }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.SubcontractorHourRates)).First(), new List<List<string>>() { subcontractorHourRates }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.SubcontractorDayRates)).First(), new List<List<string>>() { subcontractorDayRates }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.SubcontractorDays)).First(), new List<List<string>>() { subcontractorDays }));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.SubcontractorMonthlyDays)).First(), inversedSubcontractors));
                    configData.Add(new Item(PresetConfig.ConfigItems.Where(i => i.Variable.Equals(PresetConfig.TimeOfCompletion)).First(), new List<List<string>> { new List<string> { timeOfCompletion.ToString() } }));
                }
                else
                {
                    errorDelegate("<h1>Error at project " + pafProjectId + "</h1><p>No items were found, canceling project creation.</p>");
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Send an error and return failure.
                errorDelegate("<h1>Error at project " + pafProjectId + "</h1><p>An error occurred while reading the data from the PAF Project and PAF Resources lists.\n" + ex.Message + "</p><p>" + ex.TargetSite + "</p>");
                return false;
            }

            // If the program was able to get this far, all is good.
            return true;
        }

        /// <summary>
        /// Deletes all remnants of a project's creation process. Comes in handy in case of errors.
        /// </summary>
        /// <param name="projectCode">Project identification code.</param>
        public void DeleteRemnants(string projectCode)
        {
            try
            {
                // Initialize the necessary variables.
                int projectId = ProjectExists(projectCode);

                if (projectId >= 0)
                {
                    listItems[projectId].DeleteObject();
                    clientContext.ExecuteQuery();
                }

                try
                {
                    SPSite site = new SPSite(PresetConfig.DefaultProjectUrl);
                    SPWeb web = site.OpenWeb();
                    SPWeb target = web.Webs.Where(i => i.Title.Equals(projectCode)).First();

                    if (target != null)
                    {
                        target.Delete();
                    }
                }
                catch (Exception ex)
                {
                    // Do nothing
                }

                // Load the site groups first, before searching through them.
                GroupCollection siteGroups = clientContext.Web.SiteGroups;
                clientContext.Load(siteGroups);
                clientContext.ExecuteQuery();

                // Look for the owner, member and visitor groups of the specified project.
                for (int x = 0; x < siteGroups.Count; x++)
                {
                    if (siteGroups[x].Title.Equals(projectCode + " Owners") || siteGroups[x].Title.Equals(projectCode + " Members") || siteGroups[x].Title.Equals(projectCode + " Visitors"))
                    {
                        siteGroups.Remove(siteGroups[x]);
                        clientContext.ExecuteQuery();
                        x--;
                    }
                }
            }
            catch (Exception ex)
            {
                errorDelegate("<h1>Error at project " + projectCode + "</h1><p>Error while cleaning up after creation errors.</p><p>" + ex.Message + "</p><br /><p>" + ex.Source + "</p>");
            }
        }

        /// <summary>
        /// Deletes all information concerning a given PAF project.
        /// </summary>
        /// <param name="project">PAF Project identification number.</param>
        /// <returns>Returns whether or not the project information was deleted.</returns>
        public bool DeletePAFInformation(int project)
        {
            try
            {
                // Load the PAF project list and PAF resource list.
                ClientContext context = new ClientContext(PresetConfig.DefaultISOUrl);
                context.Load(context.Web);
                context.ExecuteQuery();

                CamlQuery query = new CamlQuery();
                query.ViewXml = "<View />";

                List pafProjects = context.Web.Lists.GetByTitle(PresetConfig.PAFProjectList);
                context.Load(pafProjects);
                context.ExecuteQuery();

                ListItemCollection projectItems = pafProjects.GetItems(query);
                context.Load(projectItems);
                context.ExecuteQuery();

                List pafResources = context.Web.Lists.GetByTitle(PresetConfig.PAFResourceList);
                context.Load(pafResources);
                context.ExecuteQuery();

                ListItemCollection resourceItems = pafResources.GetItems(query);
                context.Load(resourceItems);
                context.ExecuteQuery();

                // Start the actual deleting.
                foreach (ListItem projectItem in projectItems)
                {
                    if (int.Parse(projectItem[PresetConfig.SPProjectID].ToString()) == project)
                    {
                        projectItem.DeleteObject();
                        context.ExecuteQuery();
                    }
                }

                foreach (ListItem resource in resourceItems)
                {
                    if (int.Parse(resource[PresetConfig.SPResourceProjectId].ToString()) == project)
                    {
                        resource.DeleteObject();
                        context.ExecuteQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorDelegate("<h1>Error at project " + project + "</h1><p>An error occurred while deleting the PAF Project and PAF Resource items.</p><p>" + ex.Message + "</p><p>" + ex.TargetSite + "</p>");
                return false;
            }

            // If the program was able to get this far, all is good.
            return true;
        }

        /// <summary>
        /// Checks whether or not the permission groups contain any existing groups related to the project.
        /// </summary>
        /// <param name="projectCode">Project identification code.</param>
        /// <returns>Returns whether or not the permission groups contain any existing groups related to the project.</returns>
        public bool PermissionGroupsExist(string projectCode)
        {
            // Load the site groups first, before searching through them.
            GroupCollection siteGroups = clientContext.Web.SiteGroups;
            clientContext.Load(siteGroups);
            clientContext.ExecuteQuery();

            // Loop through the site groups and return true when a match has been found.
            for (int x = 0; x < siteGroups.Count; x++)
            {
                if (siteGroups[x].Title.Equals(projectCode + " Owners") || siteGroups[x].Title.Equals(projectCode + " Members") || siteGroups[x].Title.Equals(projectCode + " Visitors"))
                {
                    return true;
                }
            }

            // If the code was able to get this far, not a single permission group was found.
            return false;
        }

        /// <summary>
        /// Sets the list name class field.
        /// </summary>
        /// <param name="listName">List name.</param>
        public void SetListName(string listName)
        {
            this.listName = listName;
        }

        /// <summary>
        /// Sets the project id for the given resource.
        /// </summary>
        /// <param name="resource">Resource identification number.</param>
        /// <returns>Returns whether or not the link was made.</returns>
        public bool SetResourceLink(int resource)
        {
            string projectTitle = "";

            try
            {
                // Load the PAF project list and PAF resource list.
                ClientContext context = new ClientContext(PresetConfig.DefaultISOUrl);
                context.Load(context.Web);
                context.ExecuteQuery();

                CamlQuery query = new CamlQuery();
                query.ViewXml = "<View />";

                List pafProjects = context.Web.Lists.GetByTitle(PresetConfig.PAFProjectList);
                context.Load(pafProjects);
                context.ExecuteQuery();

                ListItemCollection projects = pafProjects.GetItems(query);
                context.Load(projects);
                context.ExecuteQuery();

                List pafResources = context.Web.Lists.GetByTitle(PresetConfig.PAFResourceList);
                context.Load(pafResources);
                context.ExecuteQuery();

                ListItemCollection resourceItems = pafResources.GetItems(query);
                context.Load(resourceItems);
                context.ExecuteQuery();

                if (projects.Count > 0)
                {
                    // Loop through the resources, checking the user that performed the latest modifications agains the user that performed the latest modifications to the projects.
                    // Also check the time of editing to link the project to the resources nearest in time.
                    foreach (ListItem resourceItem in resourceItems)
                    {
                        if (int.Parse(resourceItem["ID"].ToString()) == resource)
                        {
                            DateTime createTime = (DateTime)resourceItem["Created"];
                            TimeSpan minimumSpan = DateTime.MaxValue.Subtract(DateTime.MinValue);
                            ListItem latestAlteredProject = projects.First();

                            foreach (ListItem projectItem in projects)
                            {
                                TimeSpan currentSpan = createTime.Subtract((DateTime)projectItem["Modified"]);

                                FieldUserValue resourceCreator = (FieldUserValue)resourceItem["Author"];
                                FieldUserValue projectEditor = (FieldUserValue)projectItem["Editor"];
                                User resourceUser = context.Web.EnsureUser(resourceCreator.LookupValue);
                                User projectUser = context.Web.EnsureUser(projectEditor.LookupValue);
                                context.Load(resourceUser);
                                context.Load(projectUser);
                                context.ExecuteQuery();

                                int difference = currentSpan.CompareTo(minimumSpan);

                                if (resourceUser.Email.Equals(projectUser.Email) && currentSpan.CompareTo(minimumSpan) < 0)
                                {
                                    minimumSpan = currentSpan;
                                    latestAlteredProject = projectItem;
                                }
                            }

                            if (latestAlteredProject != null)
                            {
                                try
                                {
                                    projectTitle = latestAlteredProject[PresetConfig.SPProjectTitle].ToString();
                                }
                                catch (Exception ex)
                                {
                                    // Do nothing.
                                }
                                resourceItem[PresetConfig.SPResourceProjectId] = latestAlteredProject["ID"];
                                resourceItem.Update();
                                context.ExecuteQuery();
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // An error occurred, time to let the logic class know.
                errorDelegate("<h1>Error at project " + projectTitle + "</h1><p>An error occurred while linking the PAF Project and PAF Resource items.</p><p>" + ex.Message + "</p>" + ex.TargetSite + "</p>");
                return false;
            }

            // If the program got this far, all is good.
            return true;
        }

        /// <summary>
        /// Returns the highest existing PAF Project Item ID.
        /// </summary>
        /// <returns>Returns the highest existing PAF Project Item ID. Will return 0 when nothing was found.</returns>
        public int GetHighestPAFProjectID()
        {
            int id = 0;    // Default ID value, if this doesn't change, no PAF Project items were found.

            try
            {
                ClientContext context = new ClientContext(PresetConfig.DefaultISOUrl);
                context.Load(context.Web);
                context.ExecuteQuery();

                // Load the list of PAF Projects.
                List pafList = context.Web.Lists.GetByTitle(PresetConfig.PAFProjectList);
                context.Load(pafList);
                context.ExecuteQuery();

                CamlQuery query = new CamlQuery();
                query.ViewXml = "<View />";

                // Query the project list to receive all PAF Project items.
                ListItemCollection items = pafList.GetItems(query);
                context.Load(items);
                context.ExecuteQuery();

                // Loop through each project item, comparing all identification numbers and selecting the highest one into the id-variable.
                foreach (ListItem item in items)
                {
                    if (int.Parse(item["ID"].ToString()) >= id)
                    {
                        id = int.Parse(item["ID"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                // Do nothing here, if an error occurs, the method will return a 0, indicating a sort of "not found".
            }

            // Return either the highest available identification number or a "not found" notification.
            return id;
        }

        /// <summary>
        /// Checks whether or not the project exists.
        /// </summary>
        /// <param name="projectCode">Project identification code.</param>
        /// <returns>Returns the index number of the project. If no project was found, the method returns -1.</returns>
        public int ProjectExists(string projectCode)
        {
            try
            {
                // No list name was specified, so no project could be found.
                if (listName == null || listName.Equals(""))
                {
                    return -1;
                }

                if (list == null || listItems == null)
                {
                    LoadList(listName);
                }

                // The list item will most likely be in the end, so start the loop reversed to save resources.
                for (int x = listItems.Count - 1; x > 0; x--)
                {
                    if (listItems[x]["Project_x0020_Code"].Equals(projectCode))
                    {
                        return x;
                    }
                }

                // If the program was able to get this far, the project wasn't found.
                return -1;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        /// <summary>
        /// Checks whether or not the project site exists.
        /// </summary>
        /// <param name="projectCode">Project identification code.</param>
        /// <returns>Returns whether or not the project site exists.</returns>
        public bool SiteExists(string projectCode)
        {
            try
            {
                SPSite site = new SPSite(PresetConfig.DefaultProjectUrl);
                SPWeb web = site.OpenWeb();
                SPWeb target = web.Webs.Where(i => i.Title.Equals(projectCode)).First();

                if (target != null && target.Title.Equals(projectCode))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            // The code is not supposed to get this far, return failure.
            return false;
        }

        /// <summary>
        /// Returns a list of all data read from the PAF Project List and PAF Resources lists.
        /// </summary>
        /// <returns>Returns a list of all data read from the PAF Project List and PAF Resources lists.</returns>
        public List<Item> GetPAFData()
        {
            return configData;
        }

        /// <summary>
        /// Queries the SharePoint environment for a user and returns the first name.
        /// </summary>
        /// <param name="internalFullName">Internal member full name.</param>
        /// <returns>Returns the first name of an internal member.</returns>
        public string GetInternalFirstName(string internalFullName)
        {
            ClientContext c = new ClientContext(PresetConfig.DefaultSiteUrl);
            c.Load(c.Web);
            c.Load(c.Web.SiteUsers);
            c.ExecuteQuery();

            User user = c.Web.EnsureUser(internalFullName);
            c.Load(user);
            c.ExecuteQuery();

            return user.Email.Split('@').First().Split('.').First();
        }

        /// <summary>
        /// Queries the SharePoint environment for a user and returns the last name.
        /// </summary>
        /// <param name="internalFullName">Internal member full name.</param>
        /// <returns>Returns the last name of an internal member.</returns>
        public string GetInternalLastName(string internalFullName)
        {
            ClientContext c = new ClientContext(PresetConfig.DefaultSiteUrl);
            c.Load(c.Web);
            c.Load(c.Web.SiteUsers);
            c.ExecuteQuery();

            User user = c.Web.EnsureUser(internalFullName);
            c.Load(user);
            c.ExecuteQuery();

            return user.Email.Split('@').First().Split('.').Last();
        }

        /// <summary>
        /// Queries the SharePoint environment for a user and returns the mail address.
        /// </summary>
        /// <param name="internalFullName">Internal member full name.</param>
        /// <returns>Returns the mail address of an internal member.</returns>
        public string GetInternalMail(string internalFullName)
        {
            ClientContext c = new ClientContext(PresetConfig.DefaultSiteUrl);
            c.Load(c.Web);
            c.Load(c.Web.SiteUsers);
            c.ExecuteQuery();

            User user = c.Web.EnsureUser(internalFullName);
            c.Load(user);
            c.ExecuteQuery();

            return user.Email;
        }
    }
}
