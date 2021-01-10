using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
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

namespace SharepointWorkflow.Data.Unused
{
    class SPSharepointClass
    {
        private const int ENGLISHLOCALE = 1033;
        private SPWeb rootWeb;
        private SPSite rootSite;
        private SPQuery query;
        private SPList list;
        private SPListItemCollection listItems;
        private SPUser user;
        private string url;
        private string listName;

        /// <summary>
        /// Default constructor method, used to construct all basic properties.
        /// </summary>
        public SPSharepointClass()
        {
            // Set the url field.
            url = PresetConfig.DefaultProjectUrl;

            // Bypass the ssl exception to enable reading.
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });

            // Create a new SPSite object, referring to a standard url.
            rootSite = new SPSite(url);
            rootWeb = rootSite.RootWeb;

            // Initialize extra necessary objects.
            query = new SPQuery();
            query.ViewXml = "<View/>";

            // Get the domain user.
            user = rootWeb.EnsureUser("administrator");
        }

        /// <summary>
        /// Non-default constructor method, used to construct all basic properties, using specified parameters to allow for dynamic SharePoint actions.
        /// </summary>
        /// <param name="url">SharePoint root url.</param>
        public SPSharepointClass(string url)
        {
            // Set the url field to be the same as the parameter.
            this.url = url;

            // Bypass the ssl exception to enable reading.
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });

            // Create a new SPSite object, referring to a custom url.
            rootSite = new SPSite(url);
            rootWeb = rootSite.RootWeb;

            // Initialize extra necessary objects.
            query = new SPQuery();
            query.ViewXml = "<View/>";

            // Get the domain user.
            user = rootWeb.EnsureUser("administrator");
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
            list = rootWeb.Lists[listName];

            // Load the list contents.
            listItems = list.GetItems(query);
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
        /// <param name="files">Array of files to be included into the new project.</param>
        public void AddProject(string listName, string title, string projectCode, string client, string entity, string[] resources, string[] owners, string duration, DateTime startDate, DateTime endDate, string description, double budget, string[] files)
        {
            SPUser defaultOwner = rootWeb.EnsureUser("SOPDEV");

            // Load the necessary permission roles, setting the default to be the first possible role.
            SPRoleDefinition contributeRole = rootWeb.RoleDefinitions["Contribute"];
            SPRoleDefinition fullRole = rootWeb.RoleDefinitions["Full Control"];
            SPRoleDefinition readRole = rootWeb.RoleDefinitions["Read"];

            // Create a subsite.
            rootWeb.AllowUnsafeUpdates = true;
            SPWebTemplate template = rootWeb.Site.GetWebTemplates(ENGLISHLOCALE)["PROJECTSITE#0"];
            SPWeb newSite = rootWeb.Webs.Add(projectCode, projectCode, description, ENGLISHLOCALE, template, true, false);
            newSite.AllowUnsafeUpdates = true;

            // Define automatically created values.
            List<SPFieldUserValue> lstResources = new List<SPFieldUserValue>();
            SPFieldUserValue[] resourceArray;

            newSite.BreakRoleInheritance(false, true);

            rootWeb.SiteGroups.Add(projectCode + " Owners", defaultOwner, defaultOwner, "Use this group to grant people full control permissions to the SharePoint site: <a href=\"" + this.url + projectCode + "\">" + projectCode + "</a>");
            SPGroup ownerGroup = rootWeb.SiteGroups[projectCode + " Owners"];
            SPRoleAssignment ownerAssignment = new SPRoleAssignment(ownerGroup);
            ownerAssignment.RoleDefinitionBindings.Add(fullRole);

            rootWeb.SiteGroups.Add(projectCode + " Members", defaultOwner, defaultOwner, "Use this group to grant people contribute permissions to the SharePoint site: <a href=\"" + this.url + projectCode + "\">" + projectCode + "</a>");
            SPGroup memberGroup = rootWeb.SiteGroups[projectCode + " Member"];
            SPRoleAssignment memberAssignment = new SPRoleAssignment(memberGroup);
            memberAssignment.RoleDefinitionBindings.Add(contributeRole);

            rootWeb.SiteGroups.Add(projectCode + " Visitors", defaultOwner, defaultOwner, "Use this group to grant people read permissions to the SharePoint site: <a href=\"" + this.url + projectCode + "\">" + projectCode + "</a>");
            SPGroup visitorGroup = rootWeb.SiteGroups[projectCode + " Visitors"];
            SPRoleAssignment visitorAssignment = new SPRoleAssignment(visitorGroup);
            visitorAssignment.RoleDefinitionBindings.Add(readRole);

            //SPGroup ownerGroup = new SPGroup();
            //ownerGroup.Name = projectCode + " Owners";
            //ownerGroup.Description = "Use this group to grant people full control permissions to the SharePoint site: <a href=\"" + this.url + projectCode + "\">" + projectCode + "</a>";
            //SPRoleAssignment ownerAssignment = new SPRoleAssignment(ownerGroup);
            //ownerAssignment.RoleDefinitionBindings.Add(fullRole);

            //SPGroup memberGroup = new SPGroup();
            //memberGroup.Name = projectCode + " Members";
            //memberGroup.Description = "Use this group to grant people contribute permissions to the SharePoint site: <a href=\"" + this.url + projectCode + "\">" + projectCode + "</a>";
            //SPRoleAssignment memberAssignment = new SPRoleAssignment(memberGroup);
            //memberAssignment.RoleDefinitionBindings.Add(contributeRole);

            //SPGroup visitorGroup = new SPGroup();
            //visitorGroup.Name = projectCode + " Visitors";
            //visitorGroup.Description = "Use this group to grant people read permissions to the SharePoint site: <a href=\"" + this.url + projectCode + "\">" + projectCode + "</a>";
            //SPRoleAssignment visitorAssignment = new SPRoleAssignment(visitorGroup);
            //visitorAssignment.RoleDefinitionBindings.Add(readRole);

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

                SPUser resource = rootWeb.EnsureUser(tempString);
                memberGroup.AddUser(resource);
                ownerGroup.AddUser(resource);

                SPFieldUserValue userValue = new SPFieldUserValue();
                userValue.LookupId = resource.ID;

                lstResources.Add(userValue);
            }

            resourceArray = lstResources.ToArray<SPFieldUserValue>();

            foreach (string s in owners)
            {
                SPUser owner = rootWeb.EnsureUser(s);
                ownerGroup.AddUser(owner);
            }

            // Assign the previously built groups to the site object and load the values into SharePoint.
            newSite.AssociatedMemberGroup = memberGroup;
            newSite.AssociatedOwnerGroup = ownerGroup;
            newSite.AssociatedVisitorGroup = visitorGroup;

            // Load the specified list into the appropriate List object.
            list = rootWeb.Lists[listName];

            // Start by creating a new item.
            SPListItem item = list.AddItem();

            // Fill the new item with the appropriate information.
            item["Title"] = title;
            item["Project_x0020_Code"] = projectCode;
            item["Customer"] = client;
            item["M_x00d6_BIUS_x0020_entity"] = entity;
            item["Resources"] = resourceArray;
            item["SP_x0020_ProjectSite"] = url;
            item["Duration"] = duration;
            item["Start_x0020_Date"] = startDate;
            item["End_x0020_Date"] = endDate;
            item["KpiDescription"] = description;
            item["Budget"] = budget;

            // Mark the item to make it ready for update and execute the update.
            item.Update();
        }
    }
}
