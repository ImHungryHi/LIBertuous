using SharepointWorkflow.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SharepointWorkflow.Data
{
    class RepliconResourceWriter
    {
        private string path;
        private string projectCode;
        private List<string> consultantNames;
        private List<List<double>> monthlyPercentages;

        /// <summary>
        /// Default constructor which initializes the fields, setting empty values.
        /// </summary>
        public RepliconResourceWriter()
        {
            path = "";
            projectCode = "";
            consultantNames = new List<string>();
            monthlyPercentages = new List<List<double>>();
        }

        /// <summary>
        /// Method to write away all data concerning replicon web resources.
        /// </summary>
        /// <param name="projectCode">Project identification code.</param>
        /// <param name="consultantNames">List of all consultant names.</param>
        /// <param name="profiles">List of consultant profile items, containing both the profile name and start index.</param>
        /// <param name="monthlyPercentages">2-dimensional list of monthly percentages ([column][row]).</param>
        /// <param name="path">Path string to which the csv will be written.</param>
        public void Write(string projectCode, string title, DateTime startDate, DateTime endDate, List<string> consultantNames, List<List<double>> monthlyPercentages, string description, ProjectStatus status, Billing billable, string leader, string path)
        {
            // Set the class fields.
            this.projectCode = projectCode;
            this.consultantNames = consultantNames;
            this.monthlyPercentages = monthlyPercentages;
            this.path = path;

            // Check for nulls and empty values.
            if (consultantNames.Count == 0)
            {
                return;
            }
            else
            {
                bool check = false;

                foreach (string consultantName in consultantNames)
                {
                    if (!consultantName.Equals(""))
                    {
                        check = true;
                    }
                }

                if (!check)
                {
                    return;
                }
            }

            // Get the correct csv syntax from the data.
            List<string> lines = GetCsvSyntax(projectCode, title, startDate, endDate, consultantNames, monthlyPercentages, description, status, billable, leader);

            if (lines.Count() <= 1)
            {
                return;
            }

            // Start writing.
            using (StreamWriter writer = new StreamWriter(path, false))
            {
                // Probably here
                foreach (string line in lines)
                {
                    writer.WriteLine(line);
                }
            }

            // Mail the csv file to the dedicated person.
            using (MailMessage message = new MailMessage(new MailAddress(PresetConfig.SourceMail), new MailAddress(PresetConfig.DestinationMail)))
            {
                message.Attachments.Add(new System.Net.Mail.Attachment(path));
                message.Subject = PresetConfig.RepliconMailSubject + " - " + projectCode;
                message.Body = PresetConfig.RepliconMailContent;
                SmtpClient client = new SmtpClient(PresetConfig.SMTPServer);
                client.Send(message);
            }

            // Finally, delete the temporary file.
            System.IO.File.Delete(path);
        }

        /// <summary>
        /// Reads all data out of the parameters and transforms it into a list of csv lines containing the correct syntax.
        /// </summary>
        /// <param name="projectCode">Project identification code.</param>
        /// <param name="consultantNames">List of all consultant names.</param>
        /// <param name="monthlyPercentages">2-dimensional list of monthly percentages ([column][row]).</param>
        /// <returns>Returns a list of lines containing all replicon resource data with the correct csv syntax.</returns>
        public List<string> GetCsvSyntax(string projectCode, string title, DateTime startDate, DateTime endDate, List<string> consultantNames, List<List<double>> monthlyPercentages, string description, ProjectStatus status, Billing billable, string leader)
        {
            // Set all necessary objects.
            string statusString = "";
            string billableString = "";
            int lineCount = consultantNames.Count;
            List<string> syntaxList = new List<string>();
            syntaxList.Add(PresetConfig.DefaultCsvColumns);

            // Transform enumeration values into string values.
            switch (status)
            {
                case ProjectStatus.InProgress:
                    statusString = "In Progress";
                    break;
                case ProjectStatus.Contracted:
                    statusString = "Contracted";
                    break;
                case ProjectStatus.Ended:
                    statusString = "Ended";
                    break;
                case ProjectStatus.Forecast:
                    statusString = "FORECAST";
                    break;
                default:
                    break;
            }

            switch (billable)
            {
                case Billing.Billable:
                    billableString = "Billable";
                    break;
                case Billing.NonBillable:
                    billableString = "Non-Billable";
                    break;
                case Billing.Both:
                    billableString = "Both";
                    break;
                default:
                    break;
            }

            // This is where the magic happens. Loop through all profiles, setting prerequisite values per profile, then internally loop through every month and every resource.
            // This will create lines which will contain the monthly resources and their procentual input, for each profile available.
            int maxMonths = 0;
            int lineCounter = 1;
            int c = 0;

            for (int x = 0; x < monthlyPercentages.Count; x++)
            {
                c = x;
                int maxCounter = 0;

                foreach (double percentage in monthlyPercentages[x])
                {
                    maxCounter++;
                }

                if (maxCounter > maxMonths)
                {
                    maxMonths = maxCounter;
                }
            }

            c = 0;
            int d = 0;

            for (int x = 0; x < maxMonths; x++)
            {
                c = x;
                string consultantString = "";
                string monthString = "";
                int consultantNumber = 0;
                int year = 0;
                int month = 0;
                int startDay = 1;    // Default value

                if ((x + 1) <= 12)
                {
                    year = DateTime.Now.Year;
                    month = x + 1;
                }
                else
                {
                    year = DateTime.Now.Year + 1;
                    month = x - 11;
                }

                switch (month)
                {
                    case 1:
                        monthString = PresetConfig.Januari;
                        break;
                    case 2:
                        monthString = PresetConfig.Februari;
                        break;
                    case 3:
                        monthString = PresetConfig.March;
                        break;
                    case 4:
                        monthString = PresetConfig.April;
                        break;
                    case 5:
                        monthString = PresetConfig.May;
                        break;
                    case 6:
                        monthString = PresetConfig.June;
                        break;
                    case 7:
                        monthString = PresetConfig.July;
                        break;
                    case 8:
                        monthString = PresetConfig.August;
                        break;
                    case 9:
                        monthString = PresetConfig.September;
                        break;
                    case 10:
                        monthString = PresetConfig.October;
                        break;
                    case 11:
                        monthString = PresetConfig.November;
                        break;
                    case 12:
                        monthString = PresetConfig.December;
                        break;
                    default:
                        break;
                }

                int endDay = DateTime.DaysInMonth(year, month);

                if (year == startDate.Year && month == startDate.Month)
                {
                    startDay = startDate.Day;
                }
                else if (year == endDate.Year && month == endDate.Month)
                {
                    endDay = endDate.Day;
                }

                for (int y = 0; y < monthlyPercentages.Count; y++)
                {
                    d = y;
                    if (x < monthlyPercentages[y].Count)
                    {
                        if (monthlyPercentages[y][x] > 0)
                        {
                            if (consultantNumber >= 1)
                            {
                                consultantString += ",";
                            }

                            consultantString += SharePointHelper.GetUsername(consultantNames[y]) + "[" + monthlyPercentages[y][x].ToString().Replace(',', '.') + "%]";
                            consultantNumber++;
                        }
                    }
                }

                if (consultantNumber > 0)
                {
                    syntaxList.Add(projectCode + " - " + title.Replace(",", ".") + ",Month " + lineCounter + ",\"" + ((startDay.ToString().Length == 1) ? "0" : "") + startDay + " " + monthString + " " + year + "\",\"" + ((endDay.ToString().Length == 1) ? "0" : "") + endDay + " " + monthString + " " + year + "\"," + ((consultantString.Contains(',')) ? "\"" : "") + consultantString + ((consultantString.Contains(',')) ? "\"" : "") + "," + description.Replace(",", "") + "," + statusString + ",," + billableString + "," + ((leader != null) ? leader : "") + "," + PresetConfig.DefaultOutlineLevel);
                    lineCounter++;
                }
            }

            return syntaxList;
        }

        /// <summary>
        /// Returns the path to which the last data was written.
        /// </summary>
        /// <returns>Returns the path to which the last data was written.</returns>
        public string GetPath()
        {
            return path;
        }
    }
}
