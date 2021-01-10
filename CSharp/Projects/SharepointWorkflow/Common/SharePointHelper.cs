using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace SharepointWorkflow.Common
{
    /// <summary>
    /// This class sort of belongs in the Data section, but is placed in the Common section to make harmless data functions and common functions available everywhere.
    /// </summary>
    static class SharePointHelper
    {
        static char[] Numbers = {
                             '0',
                             '1',
                             '2',
                             '3',
                             '4',
                             '5',
                             '6',
                             '7',
                             '8',
                             '9'
                         };

        /// <summary>
        /// Gets the correct syntax for a certain array of worksheets as they should be used in the OleDb classes.
        /// </summary>
        /// <param name="needles">Original worksheet names as shown in the Excel application.</param>
        /// <param name="haystack">Worksheet names as used in the OleDb classes.</param>
        /// <returns>Returns an array of string values containing the correct syntax of available worksheet names, in the order as given in the needles parameter.</returns>
        public static string[] GetCorrectSyntax(string[] needles, string[] haystack)
        {
            string[] syntaxes = new string[needles.Length];

            for (int i = 0; i < syntaxes.Length; i++)
            {
                for (int x = 0; x < haystack.Length; x++)
                {
                    bool check = false;
                    string temp = needles[i];

                    for (int y = 0; y < temp.Length; y++)
                    {
                        if (!check && !SharePointHelper.Numbers.Contains(temp[y]))
                        {
                            check = false;
                            StringBuilder sb = new StringBuilder(temp);
                            sb[y] = '#';
                            temp = sb.ToString() + "$";
                            break;
                        }
                    }
                        
                    if (haystack[x][0] == '\'' && haystack[x].Contains("$\'"))
                    {
                        if (("'" + temp + "'").Equals(haystack[x]))
                        {
                            syntaxes[i] = temp;
                            break;
                        }
                        else
                        {
                            syntaxes[i] = "";
                        }
                    }
                    else
                    {
                        if (temp.Equals(haystack[x]))
                        {
                            syntaxes[i] = temp;
                            break;
                        }
                        else
                        {
                            syntaxes[i] = "";
                        }
                    }
                }
            }

            return syntaxes;
        }

        /// <summary>
        /// Calls the CalculatePercentage method for each item in the 2-dimensional days list.
        /// </summary>
        /// <param name="days">2-dimensional list containing the amount of days spent on a function per month per resource.</param>
        /// <returns>Returns a 2-dimensional list containing the percentage of days spent on a function per month per resource.</returns>
        public static List<List<double>> CalculatePercentages(List<List<double>> days)
        {
            List<List<double>> percentages = new List<List<double>>();

            for (int x = 0; x < days.Count; x++)
            {
                List<double> line = new List<double>();

                for (int y = 0; y < days[x].Count; y++)
                {
                    line.Add(CalculatePercentage(days[x][y]));
                }

                percentages.Add(line);
            }

            return percentages;
        }

        /// <summary>
        /// Calculates the percentage of days spent on a function, relative to the maximum amount of days in a month.
        /// </summary>
        /// <param name="days">Amount of days spent on a function.</param>
        /// <returns>Returns the percentage of days spent on a function, relative to the maximum amount of days in a month.</returns>
        public static double CalculatePercentage(double days)
        {
            return days * 5;    // days / 20 * 100
        }

        /// <summary>
        /// Calculates the percentage of days spent on a function, relative to the maximum amount of days in a month, the complicated way.
        /// </summary>
        /// <param name="days">Amount of days spent on a function.</param>
        /// <param name="month">Month in which the days were spent</param>
        /// <param name="year">Year in which the days were spent.</param>
        /// <returns>Returns the percentage of days spent on a function, relative to the maximum amount of days in a month.</returns>
        public static double ComplicatedCalculatePercentage(double days, int month, int year)
        {
            int counter = 0;

            DayOfWeek[] weekends = new DayOfWeek[] {
                DayOfWeek.Saturday,
                DayOfWeek.Sunday
            };

            for (int x = 1; x <= DateTime.DaysInMonth(year, month); x++)
            {
                if (!weekends.Contains(new DateTime(year, month, x).DayOfWeek))
                {
                    counter++;
                }
            }

            return ((days / counter) * 100);
        }

        /// <summary>
        /// Queries the SharePoint environment for a user by a combination of the user's first and last name in order.
        /// </summary>
        /// <param name="fullname">User's full name, most commonly a "last name, first name" combination.</param>
        /// <returns>Returns a user's login name if it occurs in the database. If it doesn't, the returnvalue will be an empty string.</returns>
        public static string GetUsername(string fullname)
        {
            if (fullname == null || fullname.Equals(""))
            {
                return "";
            }

            string username = "";
            string temp = fullname; // Default value, will be overwritten if the fullname parameter contains a comma.
            string[] split = fullname.Split(',');

            for (int x = 0; x < split.Length; x++)
            {
                if (split[x][0] == ' ')
                {
                    split[x] = split[x].Remove(0, 1);
                }
            }

            if (split.Length > 1)
            {
                temp = split[1] + " " + split[0];
            }

            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });

            try
            {
                ClientContext context = new ClientContext(PresetConfig.DefaultSiteUrl);
                context.Load(context.Web);
                context.ExecuteQuery();
                User user = context.Web.EnsureUser(temp);
                context.Load(user);
                context.ExecuteQuery();

                if (user != null)
                {
                    username = user.LoginName.Split('\\').Last();
                }
            }
            catch (Exception ex)
            {
                // Do nothing, the user will simply be ignored.
            }

            return username;
        }

        /// <summary>
        /// Deletes the html syntax from an input string and returns the result.
        /// </summary>
        /// <param name="input">Input string to filter the html syntax from.</param>
        /// <returns>Returns the result of the html filtering.</returns>
        public static string DeleteHtml(string input)
        {
            // Harbor all information inside one big paragraph instead of multiple smaller paragraphs.
            input = input.Replace("</p> <p>", "\n").Replace("</p><p>", "\n").Replace("</p>\n <p>", "\n").Replace("</p> \n<p>", "\n").Replace("</p>\n<p>", "\n").Replace("</p>\n  <p>", "\n").Replace("​", "");
            
            // Decide the index of the first paragraph tag and the last one.
            int start = input.IndexOf("<p>") + 3;
            int end = input.IndexOf("</p>");

            // Return the input minus the html tags.
            return input.Substring(start, (end - start));
        }

        /// <summary>
        /// Loops through each extension and checks whether or not the url contains it.
        /// </summary>
        /// <param name="haystack">File url in which the search for extensions will occur.</param>
        /// <param name="needles">Extension list.</param>
        /// <returns>Returns whether or not any of the extensions were found in the url.</returns>
        public static bool ContainsExtension(string haystack, List<string> needles)
        {
            return ((needles.Where(i => haystack.ToLower().Contains(i.ToLower())).Count() != 0) ? true : false);
        }
        
        /// <summary>
        /// Compares the contents of the 2 parameters
        /// </summary>
        /// <param name="search">Search name.</param>
        /// <param name="target">Target name.</param>
        /// <returns>Returns whether or not both targets are equal in name.</returns>
        public static bool CompareNames(string search, string target)
        {
            // First, handle the replacing of unnecessary characters, then handle the deletion of redundant spaces.
            search = search.Replace(",", "").Replace("  ", " ");
            target = target.Replace(",", "").Replace("  ", " ");
            search = ((search.Last() == ' ') ? ((search.First() == ' ') ? search.Remove(search.LastIndexOf(' ')).Remove(0, 1) : search.Remove(search.LastIndexOf(' '))) : ((search.First() == ' ') ? search.Remove(0, 1) : search));
            target = ((target.Last() == ' ') ? ((target.First() == ' ') ? target.Remove(target.LastIndexOf(' ')).Remove(0, 1) : target.Remove(target.LastIndexOf(' '))) : ((target.First() == ' ') ? target.Remove(0, 1) : target));

            int count = 0;  // This should end up being as long as the split search string array.
            string[] splitSearch = search.Split(' ');
            string[] splitTarget = target.Split(' ');

            foreach (string needle in splitSearch)
            {
                foreach (string haystack in splitTarget)
                {
                    if (haystack.Contains(needle))
                    {
                        count++;
                    }
                }
            }

            if (count == splitSearch.Length)
            {
                return false;
            }

            return true;
        }
    }
}
