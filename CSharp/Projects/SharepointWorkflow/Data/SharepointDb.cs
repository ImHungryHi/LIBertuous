using System;
using System.Collections.Generic;
using System.Linq;

namespace SharepointWorkflow.Data
{
    partial class SharepointDbDataContext
    {
        private DateTime DATENULL = new DateTime(2000, 1, 1);
        private char[] charNumbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        #region Selects
        /// <summary>
        /// Calculates the lowest available id out of a list of id's.
        /// </summary>
        /// <param name="idList">List of identification numbers.</param>
        /// <returns>The lowest available identification number.</returns>
        public int GetLowestAvailableId(List<int> idList)
        {
            idList.Sort();

            for (int x = 0; x < idList.Count; x++)
            {
                // If we encounter a mismatch, where the number is not equal to the index incremented by 1, we encounter an unused id.
                if (idList[x] != x + 1)
                {
                    return (x + 1);
                }
            }

            // If the loop has not found any mismatches, return the last id incremented by one.
            return (idList[idList.Count - 1] + 1);
        }

        /// <summary>
        /// Selects a list of all clients.
        /// </summary>
        /// <returns>A list of all clients.</returns>
        public List<Client> GetClients()
        {
            try
            {
                return (from c in Clients
                        select c).ToList<Client>();
            }
            catch (Exception ex)
            {
                return new List<Client>();
            }
        }

        /// <summary>
        /// Selects a client by it's identification number.
        /// </summary>
        /// <param name="id">Client identification number.</param>
        /// <returns>The client matching the identification number.</returns>
        public Client GetClientById(int id)
        {
            try
            {
                return (from c in Clients
                        where c.Id == id
                        select c).First<Client>();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Selects a client id by it's name.
        /// </summary>
        /// <param name="name">Client name.</param>
        /// <returns>The id matching the client name.</returns>
        public int GetClientIdByName(string name)
        {
            try
            {
                return (from c in Clients
                        where c.Name.ToLower().Equals(name.ToLower())
                        select c.Id).First<int>();
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        /// <summary>
        /// Selects a list of all client identification numbers.
        /// </summary>
        /// <returns>A list of all client identification numbers.</returns>
        public List<int> GetClientIdList()
        {
            try
            {
                return (from c in Clients
                        select c.Id).ToList<int>();
            }
            catch (Exception ex)
            {
                return new List<int>();
            }
        }

        /// <summary>
        /// Selects the lowest available role id.
        /// </summary>
        /// <returns>Lowest available role id.</returns>
        public int GetMinimumAvailableClientId()
        {
            return GetLowestAvailableId(GetClientIdList());
        }

        /// <summary>
        /// Selects a list of all available entities.
        /// </summary>
        /// <returns>A list of all available entities.</returns>
        public List<Entity> GetEntities()
        {
            try
            {
                return (from e in Entities
                        select e).ToList<Entity>();
            }
            catch (Exception ex)
            {
                return new List<Entity>();
            }
        }

        /// <summary>
        /// Selects an entity by it's identification number.
        /// </summary>
        /// <param name="code">Entity identification code.</param>
        /// <returns>The entity matching the identification code.</returns>
        public Entity GetEntityByCode(string code)
        {
            try
            {
                return (from e in Entities
                        where e.Code.ToLower().Equals(code.ToLower())
                        select e).First<Entity>();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Selects a list of all external contacts.
        /// </summary>
        /// <returns>A list of all external contacts.</returns>
        public List<ExternalContact> GetExternalContacts()
        {
            try
            {
                return (from e in ExternalContacts
                        select e).ToList<ExternalContact>();
            }
            catch (Exception ex)
            {
                return new List<ExternalContact>();
            }
        }

        /// <summary>
        /// Selects an external contact by it's identification number.
        /// </summary>
        /// <param name="id">External contact's identification number.</param>
        /// <returns>The external contact matching the identification code.</returns>
        public ExternalContact GetExternalContactById(int id)
        {
            try
            {
                return (from e in ExternalContacts
                        where e.Id == id
                        select e).First<ExternalContact>();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Selects an external contact's identification number by it's information.
        /// </summary>
        /// <param name="name">External contact's last name.</param>
        /// <param name="firstName">External contact's first name.</param>
        /// <param name="client">Client's name.</param>
        /// <returns>The external contact's identification number matching the parameter information.</returns>
        public int GetExternalContactIdByNames(string name, string firstName, string client)
        {
            int clientId = GetClientIdByName(client);

            try
            {
                return (from e in ExternalContacts
                        where e.ClientId == clientId && e.Name.ToLower().Equals(name.ToLower()) && e.FirstName.ToLower().Equals(firstName.ToLower())
                        select e.Id).First<int>();
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        /// <summary>
        /// Selects a list of all external contact identification numbers.
        /// </summary>
        /// <returns>A list of all external contact identification numbers.</returns>
        public List<int> GetExternalContactIdList()
        {
            try
            {
                return (from e in ExternalContacts
                        select e.Id).ToList<int>();
            }
            catch (Exception ex)
            {
                return new List<int>();
            }
        }

        /// <summary>
        /// Selects the lowest available external contact id.
        /// </summary>
        /// <returns>The lowest available external contact id.</returns>
        public int GetMinimumAvailableExternalContactId()
        {
            return GetLowestAvailableId(GetExternalContactIdList());
        }

        /// <summary>
        /// Selects a list of all internal contacts.
        /// </summary>
        /// <returns>A list of all internal contacts.</returns>
        public List<InternalContact> GetInternalContacts()
        {
            try
            {
                return (from i in InternalContacts
                        select i).ToList<InternalContact>();
            }
            catch (Exception ex)
            {
                return new List<InternalContact>();
            }
        }

        /// <summary>
        /// Selects an internal contact by it's identification number.
        /// </summary>
        /// <param name="id">Internal contact's identification number.</param>
        /// <returns>The internal contact matching the identification number.</returns>
        public InternalContact GetInternalContactById(int id)
        {
            try
            {
                return (from i in InternalContacts
                        where i.Id == id
                        select i).First<InternalContact>();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Selects an internal contact's identification number by it's member identification number and/or role id.
        /// </summary>
        /// <param name="member">Member identification number.</param>
        /// <param name="role">Role identification number.</param>
        /// <returns>The internal contact's identification number mathing the parameter information.</returns>
        public int GetInternalContactIdByInfo(int member, int role = 0)
        {
            try
            {
                if (role < 1)
                {
                    return (from i in InternalContacts
                            where i.Member == member && i.Role == null
                            select i.Id).First<int>();
                }

                return (from i in InternalContacts
                        where i.Member == member && i.Role == role
                        select i.Id).First<int>();
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        /// <summary>
        /// Selects a list of all internal contact identification numbers.
        /// </summary>
        /// <returns>A list of all internal contact identification numbers.</returns>
        public List<int> GetInternalContactIdList()
        {
            try
            {
                return (from i in InternalContacts
                        select i.Id).ToList<int>();
            }
            catch (Exception ex)
            {
                return new List<int>();
            }
        }

        /// <summary>
        /// Selects the lowest available internal contact id.
        /// </summary>
        /// <returns>The lowest available internal contact id.</returns>
        public int GetMinimumAvailableInternalContactId()
        {
            return GetLowestAvailableId(GetInternalContactIdList());
        }

        /// <summary>
        /// Selects a list of all members.
        /// </summary>
        /// <returns>A list of all members.</returns>
        public List<Member> GetMembers()
        {
            try
            {
                return (from m in Members
                        select m).ToList<Member>();
            }
            catch (Exception ex)
            {
                return new List<Member>();
            }
        }

        /// <summary>
        /// Selects a member by it's identification number.
        /// </summary>
        /// <param name="id">Member identification number.</param>
        /// <returns>The member matching the identification number.</returns>
        public Member GetMemberById(int id)
        {
            try
            {
                return (from m in Members
                        where m.Id == id
                        select m).First<Member>();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Selects the identification number of a member based on all of his/her information.
        /// </summary>
        /// <param name="name">Last name.</param>
        /// <param name="firstName">First name.</param>
        /// <param name="email">E-mail address.</param>
        /// <returns>The member id matching all information.</returns>
        public int GetMemberIdByInfo(string name, string firstName, string email)
        {
            try
            {
                return (from m in Members
                        where m.Name.ToLower().Equals(name.ToLower()) && m.FirstName.ToLower().Equals(firstName.ToLower()) && m.Email.ToLower().Equals(email.ToLower())
                        select m.Id).First<int>();
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        /// <summary>
        /// Selects a list of all member identification numbers.
        /// </summary>
        /// <returns>A list of all member identification numbers.</returns>
        public List<int> GetMemberIdList()
        {
            try
            {
                return (from m in Members
                        select m.Id).ToList<int>();
            }
            catch (Exception ex)
            {
                return new List<int>();
            }
        }

        /// <summary>
        /// Selects the lowest available member id.
        /// </summary>
        /// <returns>The lowest available member id.</returns>
        public int GetMinimumAvailableMemberId()
        {
            return GetLowestAvailableId(GetMemberIdList());
        }

        /// <summary>
        /// Selects a project by it's identification code.
        /// </summary>
        /// <param name="code">Project identification code.</param>
        /// <returns>The project matching the identification code.</returns>
        public Project GetProjectByCode(string code)
        {
            try
            {
                return (from p in Projects
                        where p.Code.ToLower().Equals(code.ToLower())
                        select p).First<Project>();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Selects a list of all approved projects in the database.
        /// </summary>
        /// <returns>A list of approved projects.</returns>
        public List<Project> GetProjects()
        {
            try
            {
                return (from p in Projects
                        select p).ToList<Project>();
            }
            catch (Exception ex)
            {
                return new List<Project>();
            }
        }

        /// <summary>
        /// Selects a list of all approved projects containing a specified string in the title.
        /// </summary>
        /// <param name="content">String to filter the title by.</param>
        /// <returns>A list of approved projects containing a specified string in the title.</returns>
        public List<Project> FilterProjectsByTitle(string content)
        {
            try
            {
                return (from p in Projects
                        where p.Title.ToLower().Contains(content.ToLower())
                        select p).ToList<Project>();
            }
            catch (Exception ex)
            {
                return new List<Project>();
            }
        }

        /// <summary>
        /// Selects a list of approved project codes.
        /// </summary>
        /// <returns>A list of approved project codes.</returns>
        public List<string> GetProjectCodeList()
        {
            try
            {
                return (from p in Projects
                        select p.Code).ToList<string>();
            }
            catch (Exception ex)
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// Selects a list of all approved project codes containing a specified string.
        /// </summary>
        /// <param name="content">String to filter the code by.</param>
        /// <returns>A list of approved project codes containing a specified string.</returns>
        public List<string> FilterProjectCodes(string content)
        {
            try
            {
                return (from p in Projects
                        where p.Code.ToLower().Contains(content.ToLower())
                        select p.Code).ToList<string>();
            }
            catch (Exception ex)
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// Select the last index occurrence of a certain code from the approved projects.
        /// </summary>
        /// <param name="code">Code to search by (with or without index).</param>
        /// <returns>The last index occurrence of a certain project code.</returns>
        public int GetLastProjectCodeOccurrence(string code)
        {
            // Initialize 2 seperate containers to store the string-part and int-part of the code.
            string stringValue = "";
            string intValue = "";
            int value = 0;
            bool check = false;

            for (int x = 0; x < code.Length; x++)
            {
                if (!check)
                {
                    if (!charNumbers.Contains(code[x]))
                    {
                        stringValue += code[x];
                    }
                    else
                    {
                        intValue += code[x];
                        check = true;
                    }
                }
                else
                {
                    if (!charNumbers.Contains(code[x]))
                    {
                        stringValue += intValue + code[x];
                        intValue = "";
                        check = false;
                    }
                    else
                    {
                        intValue += code[x];
                    }
                }
            }

            try
            {
                // Retrieve the code list to search for the latest code similar to the parameter.
                List<string> codeList = FilterProjectCodes(stringValue);

                foreach (string s in codeList)
                {
                    string temp = s.Replace(stringValue, "");
                    int parsed = 0;

                    // Try to parse the temporary value that's supposed to be an integer.
                    if (int.TryParse(temp, out parsed))
                    {
                        // If the integer value is larger than the previous maximum, set the value to be the new maximum.
                        if (value < parsed || (value == 0 && parsed >= 0))
                        {
                            value = parsed;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Do nothing and let the method return the default value.
            }

            return value;
        }

        /// <summary>
        /// Selects a list of all project resources.
        /// </summary>
        /// <returns>A list of all project resources.</returns>
        public List<ProjectResource> GetProjectResources()
        {
            try
            {
                return (from r in ProjectResources
                        select r).ToList<ProjectResource>();
            }
            catch (Exception ex)
            {
                return new List<ProjectResource>();
            }
        }

        /// <summary>
        /// Selects a list of project resources linked to a certain project.
        /// </summary>
        /// <param name="code">Project's identification code.</param>
        /// <returns>A list of project resources linked to a certain project.</returns>
        public List<ProjectResource> GetProjectResourcesByProjectCode(string code)
        {
            try
            {
                return (from r in ProjectResources
                        where r.ProjectCode.ToLower().Equals(code.ToLower())
                        select r).ToList<ProjectResource>();
            }
            catch (Exception ex)
            {
                return new List<ProjectResource>();
            }
        }

        /// <summary>
        /// Selects a list of all available roles.
        /// </summary>
        /// <returns>A list of all available roles.</returns>
        public List<Role> GetRoles()
        {
            try
            {
                return (from r in Roles
                        select r).ToList<Role>();
            }
            catch (Exception ex)
            {
                return new List<Role>();
            }
        }

        /// <summary>
        /// Selects a role by it's identification number.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The role matching the identification number.</returns>
        public Role GetRoleById(int id)
        {
            try
            {
                return (from r in Roles
                        where r.Id == id
                        select r).First<Role>();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Select a role's identification number by it's name and rate per hour.
        /// </summary>
        /// <param name="name">Role name.</param>
        /// <param name="rate">Role's rate per hour.</param>
        /// <returns>The role's identification number matching the parameter information.</returns>
        public int GetRoleIdByNameAndRate(string name, double rate)
        {
            try
            {
                return (from r in Roles
                        where r.Name.ToLower().Equals(name.ToLower()) && r.HourRate == Convert.ToDecimal(rate)
                        select r.Id).First<int>();
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        /// <summary>
        /// Selects a list of all role id's.
        /// </summary>
        /// <returns>A list of all role id's.</returns>
        public List<int> GetRoleIdList()
        {
            try
            {
                return (from r in Roles
                        select r.Id).ToList<int>();
            }
            catch (Exception ex)
            {
                return new List<int>();
            }
        }

        /// <summary>
        /// Selects the lowest available role id.
        /// </summary>
        /// <returns>Lowest available role id.</returns>
        public int GetMinimumAvailableRoleId()
        {
            return GetLowestAvailableId(GetRoleIdList());
        }

        /// <summary>
        /// Selects a list of all sectors.
        /// </summary>
        /// <returns>A list of all sectors.</returns>
        public List<Sector> GetSectors()
        {
            try
            {
                return (from s in Sectors
                        select s).ToList<Sector>();
            }
            catch (Exception ex)
            {
                return new List<Sector>();
            }
        }

        /// <summary>
        /// Selects a sector by it's sector name.
        /// </summary>
        /// <param name="name">Sector name.</param>
        /// <returns>The sector matching the sector name.</returns>
        public Sector GetSectorByName(string name)
        {
            try
            {
                return (from s in Sectors
                        where s.Name.ToLower().Equals(name.ToLower())
                        select s).First<Sector>();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion Selects

        #region Checks
        /// <summary>
        /// Checks whether or not a client has been added to the database.
        /// </summary>
        /// <param name="id">Client identification number.</param>
        /// <returns>Returns true when the database contains the specified client.</returns>
        public bool InClients(int id)
        {
            try
            {
                return (from c in Clients
                        where c.Id == id
                        select c).Count<Client>() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks whether or not a client has been added to the database.
        /// </summary>
        /// <param name="name">Client name.</param>
        /// <returns>Returns true when the database contains the specified client.</returns>
        public bool InClients(string name)
        {
            try
            {
                return (from c in Clients
                        where c.Name.ToLower().Equals(name.ToLower())
                        select c).Count<Client>() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks whether or not a certain entity occurs in the database.
        /// </summary>
        /// <param name="code">Entity identification code.</param>
        /// <returns>Returns true when the database contains the specified entity code.</returns>
        public bool InEntities(string code)
        {
            try
            {
                return (from e in Entities
                        where e.Code.ToLower().Equals(code.ToLower())
                        select e).Count<Entity>() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks whether or not a certain external contact occurs in the database.
        /// </summary>
        /// <param name="id">External contact identification number.</param>
        /// <returns>Returns true when the database contains the specified external contact.</returns>
        public bool InExternalContacts(int id)
        {
            try
            {
                return (from e in ExternalContacts
                        where e.Id == id
                        select e).Count<ExternalContact>() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks whether or not a certain external contact occurs in the database.
        /// </summary>
        /// <param name="name">External contact's last name.</param>
        /// <param name="firstName">External contact's first name.</param>
        /// <param name="email">External contact's email address.</param>
        /// <returns>Returns true when the database contains the specified external contact.</returns>
        public bool InExternalContacts(string name, string firstName, string email = null)
        {
            try
            {
                return (from e in ExternalContacts
                        where e.Name.ToLower().Equals(name.ToLower()) && e.FirstName.ToLower().Equals(firstName.ToLower()) && e.Email.ToLower().Equals(email.ToLower())
                        select e).Count<ExternalContact>() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks whether or not a certain internal contact occurs in the database.
        /// </summary>
        /// <param name="id">Internal contact identification number.</param>
        /// <returns>Returns true when the database contains the specified internal contact.</returns>
        public bool InInternalContacts(int id)
        {
            try
            {
                return (from i in InternalContacts
                        where i.Id == id
                        select i).Count<InternalContact>() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks whether or not a certain internal contact occurs in the database.
        /// </summary>
        /// <param name="memberId">Member identification number.</param>
        /// <param name="roleId">Role identification number.</param>
        /// <returns>Returns true when the database contains the specified internal contact.</returns>
        public bool InInternalContacts(int memberId, int roleId = 0)
        {
            try
            {
                if (roleId == 0)
                {
                    return (from i in InternalContacts
                            where i.Member == memberId && i.Role == null
                            select i).Count<InternalContact>() > 0;
                }

                return (from i in InternalContacts
                        where i.Role == roleId && i.Member == memberId
                        select i).Count<InternalContact>() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks whether or not a certain member occurs in the database.
        /// </summary>
        /// <param name="id">Member identification number.</param>
        /// <returns>Returns true when the database contains the specified member.</returns>
        public bool InMembers(int id)
        {
            try
            {
                return (from m in Members
                        where m.Id == id
                        select m).Count<Member>() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks whether or not a certain member occurs in the database.
        /// </summary>
        /// <param name="name">Member's last name.</param>
        /// <param name="firstName">Member's first name.</param>
        /// <param name="email">Member's email address.</param>
        /// <returns>Returns true when the database contains the specified member.</returns>
        public bool InMembers(string name, string firstName, string email = null)
        {
            try
            {
                if (email == null || email.Equals(""))
                {
                    return (from m in Members
                            where m.Name.ToLower().Equals(name.ToLower()) && m.FirstName.ToLower().Equals(firstName.ToLower())
                            select m).Count<Member>() > 0;
                }

                return (from m in Members
                        where m.Name.ToLower().Equals(name.ToLower()) && m.FirstName.ToLower().Equals(firstName.ToLower()) && m.Email.ToLower().Equals(email.ToLower())
                        select m).Count<Member>() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks whether or not a project has been added to the database, using it's project code.
        /// </summary>
        /// <param name="code">Project identification code.</param>
        /// <returns>Returns true when the code has occurred in the project list.</returns>
        public bool InProjects(string code)
        {
            try
            {
                return (from p in Projects
                        where p.Code.ToLower().Equals(code.ToLower())
                        select p).Count<Project>() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks whether or not the project has been linked to any internal contacts.
        /// </summary>
        /// <param name="code">Project identification code.</param>
        /// <returns>Returns true when the project has been linked to any resources.</returns>
        public bool ProjectHasResources(string code)
        {
            try
            {
                return (from r in ProjectResources
                        where r.ProjectCode.ToLower().Equals(code.ToLower())
                        select r).Count<ProjectResource>() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks whether or not the project has been linked to a certain internal contact.
        /// </summary>
        /// <param name="code">Project identification code.</param>
        /// <param name="internalContactId">Internal contact identification number.</param>
        /// <returns>Returns true when the project has been linked to the specified internal contact.</returns>
        public bool ProjectHasResource(string code, int internalContactId)
        {
            try
            {
                return (from r in ProjectResources
                        where r.ProjectCode.ToLower().Equals(code.ToLower()) && r.InternalContactId == internalContactId
                        select r).Count<ProjectResource>() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks whether or not the role exists in the database.
        /// </summary>
        /// <param name="id">Role identification number.</param>
        /// <returns>Returns true when the role has been found in the database.</returns>
        public bool InRoles(int id)
        {
            try
            {
                return (from r in Roles
                        where r.Id == id
                        select r).Count<Role>() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks whether or not the role exists in the database.
        /// </summary>
        /// <param name="name">Role name.</param>
        /// <param name="hourRate">Rate per hour.</param>
        /// <returns>Returns true when the role has been found in the database.</returns>
        public bool InRoles(string name, double hourRate = 0)
        {
            try
            {
                if (hourRate != 0)
                {
                    return (from r in Roles
                            where r.Name.ToLower().Equals(name.ToLower()) && r.HourRate == Convert.ToDecimal(hourRate)
                            select r).Count<Role>() > 0;
                }

                return (from r in Roles
                        where r.Name.ToLower().Equals(name.ToLower())
                        select r).Count<Role>() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks whether or not the sector already exists in the database.
        /// </summary>
        /// <param name="name">Sector name.</param>
        /// <returns>Returns true when the sector has been found in the database.</returns>
        public bool InSectors(string name)
        {
            try
            {
                return (from s in Sectors
                        where s.Name.ToLower().Equals(name.ToLower())
                        select s).Count<Sector>() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion Checks

        #region Updates
        #endregion Updates

        #region Deletes
        /// <summary>
        /// Deletes a client from the database.
        /// </summary>
        /// <param name="id">Client identification number.</param>
        /// <returns>Returns whether or not the client has been deleted from the database.</returns>
        public bool DeleteClient(int id)
        {
            // There was nothing to delete, so all is good. Just make sure to leave the method.
            if (!InClients(id))
            {
                return true;
            }

            // Delete the client from the Clients table and submit all changes.
            Clients.DeleteOnSubmit(GetClientById(id));
            SubmitChanges();

            // If the client is still in the database, something went wrong.
            if (InClients(id))
            {
                return false;
            }

            // If the program was able to get this far, all is good.
            return true;
        }

        /// <summary>
        /// Deletes an entity from the database.
        /// </summary>
        /// <param name="code">Entity identification code.</param>
        /// <returns>Returns whether or not the entity has been deleted from the database.</returns>
        public bool DeleteEntity(string code)
        {
            // There was nothing to delete so all is good. Just make sure to leave the method.
            if (!InEntities(code))
            {
                return true;
            }

            // Delete the entity from the Entities table and submit all changes.
            Entities.DeleteOnSubmit(GetEntityByCode(code));
            SubmitChanges();

            // If the entity is still in the database, something went wrong.
            if (InEntities(code))
            {
                return false;
            }

            // If the program got this far, all is good.
            return true;
        }

        /// <summary>
        /// Deletes an external contact from the database.
        /// </summary>
        /// <param name="id">External contact's identification number.</param>
        /// <returns>Returns whether or not the external contact has been deleted from the database.</returns>
        public bool DeleteExternalContact(int id)
        {
            // There was nothing to delete so all is good. Just make sure to leave the method.
            if (!InExternalContacts(id))
            {
                return true;
            }

            // Delete the external contact from the ExternalContacts table and submit all changes.
            ExternalContacts.DeleteOnSubmit(GetExternalContactById(id));
            SubmitChanges();

            // If the external contact is still in the database, something went wrong.
            if (InExternalContacts(id))
            {
                return false;
            }

            // If the program got this far, all is good.
            return true;
        }

        /// <summary>
        /// Deletes an internal contact from the database.
        /// </summary>
        /// <param name="id">Internal contact's identification number.</param>
        /// <returns>Returns whether or not the internal contact has been deleted from the database.</returns>
        public bool DeleteInternalContact(int id)
        {
            // There was nothing to delete so all is good. Just make sure to leave the method.
            if (!InInternalContacts(id))
            {
                return true;
            }

            // Delete the internal contact from the InternalContacts table and submit all changes.
            InternalContacts.DeleteOnSubmit(GetInternalContactById(id));
            SubmitChanges();

            // If the internal contact is still in the database, something went wrong.
            if (InInternalContacts(id))
            {
                return false;
            }

            // If the program got this far, all is good.
            return true;
        }

        /// <summary>
        /// Deletes a member from the database.
        /// </summary>
        /// <param name="id">Member identification number.</param>
        /// <returns>Returns whether or not the member has been deleted from the database.</returns>
        public bool DeleteMember(int id)
        {
            // There was nothing to delete so all is good. Just make sure to leave the method.
            if (!InMembers(id))
            {
                return true;
            }

            // Delete the member from the Members table and submit all changes.
            Members.DeleteOnSubmit(GetMemberById(id));
            SubmitChanges();

            // If the member is still in the database, something went wrong.
            if (InMembers(id))
            {
                return false;
            }

            // If the program got this far, all is good.
            return true;
        }

        /// <summary>
        /// Deletes a project from the database.
        /// </summary>
        /// <param name="code">Project identification code.</param>
        /// <returns>Returns whether or not the project has been deleted from the database.</returns>
        public bool DeleteProject(string code)
        {
            // There was nothing to delete so all is good. Just make sure to leave the method.
            if (!InProjects(code))
            {
                return true;
            }

            // A project which has been deleted can't have links to any resources, delete these first.
            DeleteProjectResources(code);

            // Delete the project from the Projects table and submit all changes.
            Projects.DeleteOnSubmit(GetProjectByCode(code));
            SubmitChanges();

            // If the project is still in the database, something went wrong.
            if (InProjects(code))
            {
                return false;
            }

            // If the program got this far, all is good.
            return true;
        }

        /// <summary>
        /// Deletes all resources containing the specified project code.
        /// </summary>
        /// <param name="code">Project identification code.</param>
        public void DeleteProjectResources(string code)
        {
            // Loop through every ProjectResources record and prepare it for deleting.
            foreach (ProjectResource projectResource in GetProjectResourcesByProjectCode(code))
            {
                ProjectResources.DeleteOnSubmit(projectResource);
            }

            SubmitChanges();
        }

        /// <summary>
        /// Deletes a role from the database.
        /// </summary>
        /// <param name="id">Role identification number.</param>
        /// <returns>Returns whether or not the role has been deleted from the database.</returns>
        public bool DeleteRole(int id)
        {
            // There was nothing to delete so all is good. Just make sure to leave the method.
            if (!InRoles(id))
            {
                return true;
            }

            // Delete the role from the Roles table and submit all changes.
            Roles.DeleteOnSubmit(GetRoleById(id));
            SubmitChanges();

            // If the role is still in the database, something went wrong.
            if (InRoles(id))
            {
                return false;
            }

            // If the program got this far, all is good.
            return true;
        }

        /// <summary>
        /// Deletes a sector from the database.
        /// </summary>
        /// <param name="name">Sector's name.</param>
        /// <returns>Returns whether or not the sector has been deleted from the database.</returns>
        public bool DeleteSector(string name)
        {
            // There was nothing to delete so all is good. Just make sure to leave the method.
            if (!InSectors(name))
            {
                return true;
            }

            // Delete the sector from the Sectors table and submit all changes.
            Sectors.DeleteOnSubmit(GetSectorByName(name));
            SubmitChanges();

            // If the sector is still in the database, something went wrong.
            if (InSectors(name))
            {
                return false;
            }

            // If the program got this far, all is good.
            return true;
        }
        #endregion Deletes

        #region Adds
        /// <summary>
        /// Adds a client to the database.
        /// </summary>
        /// <param name="name">Client's name.</param>
        /// <param name="sector">Client's sector.</param>
        /// <param name="addSector">Permission to create new sector in case it doesn't exist.</param>
        /// <returns>Returns whether or not the record has been added.</returns>
        public bool AddClient(string name, string sector, bool addSector = false)
        {
            // If the client has already been found in the database, all is good. Just make sure to leave the method.
            if (InClients(name))
            {
                return true;
            }

            if (sector != null && !sector.Equals(""))
            {
                // Check if the specified sector exists.
                if (!InSectors(sector))
                {
                    if (addSector)
                    {
                        // Add the sector to the database.
                        AddSector(sector);
                    }
                    else
                    {
                        // The client could not be added because the sector doesn't exist in the database.
                        return false;
                    }
                }
            }

            int id = GetMinimumAvailableClientId();

            // Create a new Client container and add all necessary data.
            Client client = new Client();
            client.Id = id;
            client.Name = name;
            client.Sector = ((sector == null || sector.Equals("")) ? null : sector);

            // Insert the new client and submit all changes();
            Clients.InsertOnSubmit(client);
            SubmitChanges();

            // If the client hasn't been added to the database, something went wrong.
            if (!InClients(id))
            {
                return false;
            }

            // If the program got this far, all is good.
            return true;
        }

        /// <summary>
        /// Add an entity to the database.
        /// </summary>
        /// <param name="code">Entity code.</param>
        /// <returns>Returns whether or not the record has been added.</returns>
        public bool AddEntity(string code)
        {
            // If the entity has already been found in the database, all is good. Just make sure to leave the method.
            if (InEntities(code))
            {
                return true;
            }

            // Create a new Entities record and assign the code from the parameters to it.
            Entity entity = new Entity();
            entity.Code = code;

            // Insert the new entity and submit all changes.
            Entities.InsertOnSubmit(entity);
            SubmitChanges();

            // If the entity has not been added to the database, something went wrong.
            if (!InEntities(code))
            {
                return false;
            }

            // If the program got this far, all is good.
            return true;
        }

        /// <summary>
        /// Adds an external contact to the database.
        /// </summary>
        /// <param name="name">Last name of the external contact.</param>
        /// <param name="firstName">First name of the external contact.</param>
        /// <param name="email">E-mail address of the external contact.</param>
        /// <param name="clientId">Client identification number.</param>
        /// <returns>Returns whether or not the record has been added.</returns>
        public bool AddExternalContact(string name, string firstName, string email, int clientId)
        {
            // The external contact could not be added, no appropriate client has been found.
            if (!InClients(clientId))
            {
                return false;
            }

            // If the external contact has already been found in the database, all is good. Just make sure to leave the method.
            if (InExternalContacts(name, firstName, email))
            {
                return true;
            }

            // Get the lowest available external contact id.
            int id = GetMinimumAvailableExternalContactId();

            // Create a new ExternalContacts record and assign the necessary information to it.
            ExternalContact external = new ExternalContact();
            external.Id = id;
            external.Name = name;
            external.FirstName = firstName;
            external.Email = email;
            external.ClientId = clientId;

            // Insert the new external contact and submit all changes.
            ExternalContacts.InsertOnSubmit(external);
            SubmitChanges();

            // If the external contact has not been added to the database, something went wrong.
            if (!InExternalContacts(id))
            {
                return false;
            }

            // If the program got this far, all is good.
            return true;
        }

        /// <summary>
        /// Adds a new internal contact to the database.
        /// </summary>
        /// <param name="roleId">Role identification number.</param>
        /// <param name="name">Internal contact's last name.</param>
        /// <param name="firstName">Internal contact's first name.</param>
        /// <param name="email">Internal contact's email address.</param>
        /// <returns>Returns whether or not the record has been added.</returns>
        public bool AddInternalContact(string name, string firstName, string email, int roleId = 0)
        {
            // Add a new member to the database and check whether or not it was successful.
            if (!AddMember(name, firstName, email))
            {
                return false;
            }

            // Add a new internal contact and check whether or not it was successful.
            if (!AddInternalContact(GetMemberIdByInfo(name, firstName, email), roleId))
            {
                return false;
            }

            // If the program got this far, all is good.
            return true;
        }

        /// <summary>
        /// Adds a new internal contact to the database.
        /// </summary>
        /// <param name="roleId">Role identification number.</param>
        /// <param name="memberId">Member identification number.</param>
        /// <returns>Returns whether or not the record has been added.</returns>
        public bool AddInternalContact(int memberId, int roleId = 0)
        {
            // If the internal contact has already been found in the database, all is good. Just make sure to leave the method.
            if (InInternalContacts(memberId, roleId))
            {
                return true;
            }

            // Could not create a new internal contact because the parameters contained wrong data.
            if ((roleId != 0 && !InRoles(roleId)) || !InMembers(memberId))
            {
                return false;
            }

            // Get the lowest available internal contact id.
            int id = GetMinimumAvailableInternalContactId();

            // Create a new InternalContacts record and assign the necessary information to it.
            InternalContact internalContact = new InternalContact();
            internalContact.Id = id;
            internalContact.Member = memberId;

            if (roleId == 0)
            {
                internalContact.Role = null;
            }
            else
            {
                internalContact.Role = roleId;
            }

            // Insert the new internal contact and submit all changes.
            InternalContacts.InsertOnSubmit(internalContact);
            SubmitChanges();

            // If the internal contact has not been added to the database, something went wrong.
            if (!InInternalContacts(id))
            {
                return false;
            }

            // If the program got this far, all is good.
            return true;
        }

        /// <summary>
        /// Adds a new member to the database.
        /// </summary>
        /// <param name="name">Member's last name.</param>
        /// <param name="firstName">Member's first name.</param>
        /// <param name="email">Member's email address.</param>
        /// <returns>Returns whether or not the record has been added to the database.</returns>
        public bool AddMember(string name, string firstName, string email)
        {
            // If the member has already been found in the database, all is good. Just make sure to leave the method.
            if (InMembers(name, firstName, email))
            {
                return true;
            }

            // Get the lowest available member id.
            int id = GetMinimumAvailableMemberId();

            // Create a new Members record and assign the necessary information to it.
            Member member = new Member();
            member.Id = id;
            member.Name = name;
            member.FirstName = firstName;
            member.Email = email;

            // Insert the new member and submit all changes.
            Members.InsertOnSubmit(member);
            SubmitChanges();

            // If the member has not been added to the database, something went wrong.
            if (!InMembers(id))
            {
                return false;
            }

            // If the program got this far, all is good.
            return true;
        }

        /// <summary>
        /// Adds a new project to the database.
        /// </summary>
        /// <param name="code">Project code.</param>
        /// <param name="title">Project title.</param>
        /// <param name="startDate">Project's start date.</param>
        /// <param name="endDate">Project's end date.</param>
        /// <param name="budget">Budget reserved for the project.</param>
        /// <param name="timeOfCompletion">The amount of days it takes to complete the project.</param>
        /// <param name="entity">Project's entity.</param>
        /// <param name="internalProjectResponsible">Project responsible within Möbius.</param>
        /// <param name="internalEndResponsible">End responsible within Möbius.</param>
        /// <param name="clientId">Client identification number.</param>
        /// <param name="description">Project description.</param>
        /// <param name="summary">Project summary.</param>
        /// <param name="externalProjectResponsible">Project responsible within the client organisation.</param>
        /// <param name="externalEndResponsible">End responsible within the client organisation.</param>
        /// <returns>Returns whether or not the record has been added to the database.</returns>
        public bool AddProject(string code, string title, DateTime startDate, DateTime endDate, double budget, double timeOfCompletion, string entity, int internalProjectResponsible, int internalEndResponsible, int clientId, string description = null, string summary = null, int externalProjectResponsible = 0, int externalEndResponsible = 0)
        {
            // If the project has already been found in the database. Notify the admin of failure.
            if (InProjects(code))
            {
                return false;
            }

            Project project = new Project();
            project.Code = code;
            project.Title = title;
            project.Description = description;
            project.Summary = summary;
            project.Budget = Convert.ToDecimal(budget);
            project.TimeOfCompletion = Convert.ToDecimal(timeOfCompletion);
            project.Entity = entity;
            project.InternalEndResponsible = internalEndResponsible;
            project.InternalProjectResponsible = internalEndResponsible;
            project.ClientId = clientId;

            if (startDate <= new DateTime(2000, 1, 1))
            {
                project.StartDate = null;
            }
            else
            {
                project.StartDate = startDate;
            }

            if (endDate <= new DateTime(2000, 1, 1))
            {
                project.EndDate = null;
            }
            else
            {
                project.EndDate = endDate;
            }

            if (externalEndResponsible > 0)
            {
                project.ExternalEndResponsible = externalEndResponsible;
            }
            else
            {
                project.ExternalEndResponsible = null;
            }

            if (externalProjectResponsible > 0)
            {
                project.ExternalProjectResponsible = externalProjectResponsible;
            }
            else
            {
                project.ExternalProjectResponsible = null;
            }

            // Link the necessary data to the project.
            if (InEntities(entity))
            {
                project.Entity1 = GetEntityByCode(entity);
            }
            else
            {
                AddEntity(entity);
                project.Entity1 = GetEntityByCode(entity);
            }

            // Insert the new project and submit all changes.
            Projects.InsertOnSubmit(project);
            SubmitChanges();

            // If the project has not been added to the database, something went wrong.
            if (!InProjects(code))
            {
                return false;
            }

            // If the program got this far, all is good.
            return true;
        }

        /// <summary>
        /// Adds a link between a certain approved project and an internal contact, while indicating the amount of days the contact will need for the project.
        /// </summary>
        /// <param name="code">Project identification code.</param>
        /// <param name="internalContactId">Internal contact identification number.</param>
        /// <param name="days">Amount of days the internal contact needs for the project.</param>
        /// <returns>Returns whether or not the record has been added.</returns>
        public bool AddResource(string code, int internalContactId, double days)
        {
            // The project already has a link to the specified internal contact, so all is good. Just make sure to leave the method.
            if (ProjectHasResource(code, internalContactId))
            {
                return true;
            }

            if (!InInternalContacts(internalContactId) || !InProjects(code))
            {
                return false;
            }

            // Create a new ProjectResource container and assign all necessary data.
            ProjectResource resource = new ProjectResource();
            resource.ProjectCode = code;
            resource.InternalContactId = internalContactId;
            resource.Days = Convert.ToDecimal(days);

            // Insert the record and submit all changes.
            ProjectResources.InsertOnSubmit(resource);
            SubmitChanges();

            // If the link between the project and the internal contact has not been made, something went wrong.
            if (!ProjectHasResource(code, internalContactId))
            {
                return false;
            }

            // If the program got this far, all is good.
            return true;
        }

        /// <summary>
        /// Adds a role to the database.
        /// </summary>
        /// <param name="id">Role identification number.</param>
        /// <param name="name">Role name.</param>
        /// <param name="hourRate">Rate per hour assigned to the role.</param>
        /// <param name="enabled">Is the role allowed to be active?</param>
        /// <returns>Returns whether or not the record has been added.</returns>
        public bool AddRole(string name, double hourRate, bool enabled)
        {
            // The role already exists in the database, so all is good. Just make sure to leave the method.
            if (InRoles(name, hourRate))
            {
                return true;
            }

            // Query the database for the lowest available role id.
            int id = GetMinimumAvailableRoleId();

            // Create a new Role container and fill it with the necessary data.
            Role role = new Role();
            role.Id = id;
            role.HourRate = Convert.ToDecimal(hourRate);

            if (enabled)
            {
                role.Enabled = 'Y';
            }
            else
            {
                role.Enabled = 'N';
            }

            // Insert the role into the database and submit all changes.
            Roles.InsertOnSubmit(role);
            SubmitChanges();

            // If the role hasn't been added to the database, something went wrong.
            if (!InRoles(id))
            {
                return false;
            }

            // If the program got this far, all is good.
            return true;
        }

        /// <summary>
        /// Adds a sector to the database.
        /// </summary>
        /// <param name="name">Sector name.</param>
        /// <returns>Returns whether or not the record has been added.</returns>
        public bool AddSector(string name)
        {
            // The sector already exists in the database, so all is good. Just make sure to leave the method.
            if (InSectors(name))
            {
                return true;
            }

            // Create a new Sector container and fill it with the necessary data.
            Sector sector = new Sector();
            sector.Name = name;

            // Insert the sector into the database and submit all changes.
            Sectors.InsertOnSubmit(sector);
            SubmitChanges();

            // If the sector hasn't been added to the database, something went wrong.
            if (!InSectors(name))
            {
                return false;
            }

            // If the program got this far, all is good.
            return true;
        }
        #endregion Adds
    }
}
