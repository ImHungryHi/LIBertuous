using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharepointWorkflow.Common
{
    /// <summary>
    /// Use(/adjust) this container class to gain faster access to config items concerning PAF IO.
    /// </summary>
    static class PresetConfig
    {
        // Static PAF search fields corresponding to the ConfigItem objects. Keep these on top, so the ConfigItems list knows it's variables.
        public static string Approach = "Approach";
        public static string Approval = "Approval";
        public static string Attachments = "Attachments";
        public static string Budget = "Budget";
        public static string BillableCosts = "BillableCosts";
        public static string ClientLocationCommunity = "CustomerCommunity";
        public static string ClientLocationCountry = "CustomerCountry";
        public static string ClientLocationStreet = "CustomerStreet";
        public static string ClientName = "ClientName";
        public static string CRMStatus = "CRMStatus";
        public static string CVConsultantNames = "CVConsultantNames";
        public static string CVConsultantRoles = "CVConsultantRoles";
        public static string CVProjectDescription = "CVProjectDescription";
        public static string DefectionReason = "DefectionReason";
        public static string Deliverables = "Deliverables";
        public static string Dependencies = "Dependencies";
        public static string Description = "Description";
        public static string EndDate = "EndDate";
        public static string Entity = "Entity";
        public static string EuroPerKm = "EuroPerKm";
        public static string ExternalEndResponsibleMail = "ExternalEndResponsibleMail";
        public static string ExternalEndResponsibleName = "ExternalEndResponsibleName";
        public static string ExternalProjectResponsibleMail = "ExternalProjectResponsibleMail";
        public static string ExternalProjectResponsibleName = "ExternalProjectResponsibleName";
        public static string GenvalToClient = "GenvalToClient";
        public static string FacturationInFunctionOf = "FacturationInFunctionOf";
        public static string FacturationSchedule = "FacturationSchedule";
        public static string FacturationType = "FacturationType";
        public static string FirstStageFacturationAmount = "FirstStageFacturationAmount";
        public static string FirstStageFacturationDate = "FirstStageFacturationDate";
        public static string FourthStageFacturationAmount = "FourthStageFacturationAmount";
        public static string FourthStageFacturationDate = "FourthStageFacturationDate";
        public static string InternalControlFrequency = "InternalControlFrequency";
        public static string InternalEndResponsibleMail = "InternalEndResponsibleMail";
        public static string InternalEndResponsibleName = "InternalEndResponsibleName";
        public static string InternalMeetings = "InternalMeetings";
        public static string InternalProjectResponsibleMail = "InternalProjectResponsibleMail";
        public static string InternalProjectResponsibleName = "InternalProjectResponsibleName";
        public static string LogoUrl = "LogoUrl";
        public static string Language = "Language";
        public static string OrderNumber = "OrderNumber";
        public static string OtherAvailable = "OtherAvailable";
        public static string OtherCommentary = "OtherCommentary";
        public static string OtherCost = "OtherCost";
        public static string OtherCostPrices = "OtherCostPrices";
        public static string OtherCostTitles = "OtherCostTitles";
        public static string OtherSpecification = "OtherSpecification";
        public static string ParkingAvailable = "ParkingAvailable";
        public static string PaymentConditions = "PaymentConditions";
        public static string PeriodicalFacturationAmount = "PeriodicalFacturationAmount";
        public static string PeriodicalFacturationDate = "PeriodicalFacturationDate";
        public static string PowerpointLogoUsage = "PowerpointLogoUsage";
        public static string ProcedureDefection = "ProcedureDefection";
        public static string ProjectCode = "ProjectCode";
        public static string ProjectConfidentialityStatus = "ProjectConfidentialityStatus";
        public static string ProjectTitle = "ProjectTitle";
        public static string ResourceDayRates = "ResourceDayRates";
        public static string ResourceDays = "ResourceDays";
        public static string ResourceHourRates = "ResourceHourRates";
        public static string ResourceMonthlyDays = "ResourceMonthlyDays";
        public static string ResourceNames = "ResourceNames";
        public static string ResourceRoles = "ResourceRoles";
        public static string Results = "Results";
        public static string SecondStageFacturationAmount = "SecondStageFacturationAmount";
        public static string SecondStageFacturationDate = "SecondStageFacturationDate";
        public static string Sector = "Sector";
        public static string SMLToClient = "SMLToClient";
        public static string StartDate = "StartDate";
        public static string StrategicalMoment = "StrategicalMoment";
        public static string SubcontractorDayRates = "SubContractorDayRates";
        public static string SubcontractorDays = "SubContractorDays";
        public static string SubcontractorHourRates = "SubContractorHourRates";
        public static string SubcontractorMonthlyDays = "SubContractorMonthlyDays";
        public static string SubcontractorNames = "SubContractorNames";
        public static string SubcontractorPresent = "SubContractorPresent";
        public static string SubcontractorRoles = "SubContractorRoles";
        public static string Summary = "Summary";
        public static string TaxNumber = "TaxNumber";
        public static string TaxRate = "TaxRate";
        public static string ThirdStageFacturationAmount = "ThirdStageFacturationAmount";
        public static string ThirdStageFacturationDate = "ThirdStageFacturationDate";
        public static string TimeOfCompletion = "TimeOfCompletion";
        public static string TotalKm = "TotalKm";
        public static string VersionDate = "VersionDate";
        public static string VersionDescription = "VersionDescription";
        public static string VersionInfluence = "VersionInfluence";

        // SharePoint property names.
        public static string SPApproach = "Approach";
        public static string SPApproval = "Approval";
        public static string SPBillableCosts = "BillableCosts";
        public static string SPBudget = "Budget";
        public static string SPClientLocationCommunity = "CustomerCommunity";
        public static string SPClientLocationCountry = "CustomerCountry";
        public static string SPClientLocationStreet = "CustomerStreet";
        public static string SPClientName = "CustomerName0";
        public static string SPCRMStatus = "CRMStatus";
        public static string SPCVConsultantNames = "CVConsultantNames";
        public static string SPCVConsultantRoles = "CVConsultantRoles";
        public static string SPCVProjectDescription = "CVProjectDescription";
        public static string SPDefectionReason = "DefectionReason";
        public static string SPDeliverables = "Deliverables";
        public static string SPDependencies = "Dependencies";
        public static string SPDescription = "Description0";
        public static string SPEndDate = "EndDate";
        public static string SPEntity = "Entity";
        public static string SPEuroPerKm = "EuroPerKm";
        public static string SPExternalEndResponsibleFirstName = "ExternalEndFirstName";
        public static string SPExternalEndResponsibleLastName = "ExternalEndLastName";
        public static string SPExternalEndResponsibleMail = "ExternalEndResponsibleMail";
        public static string SPExternalProjectResponsibleFirstName = "ExternalProjectFirstName";
        public static string SPExternalProjectResponsibleLastName = "ExternalProjectLastName";
        public static string SPExternalProjectResponsiblMail = "ExternalProjectResponsibleMail";
        public static string SPGenvalToClient = "GenvalToClient";
        public static string SPFacturationInFunctionOf = "FacturationInFunctionOf";
        public static string SPFacturationSchedule = "FacturationSchedule";
        public static string SPFacturationType = "FacturationType";
        public static string SPFirstStageFacturationAmount = "FirstStageAmount";
        public static string SPFirstStageFacturationDate = "FirstStageDate";
        public static string SPFourthStageFacturationAmount = "FourthStageAmount";
        public static string SPFourthStageFacturationDate = "FourthStageDate";
        public static string SPInternalControlFrequency = "InternalMeetingFrequency";
        public static string SPInternalEndResponsible = "InternalEndResponsible";
        public static string SPInternalMeetings = "InternalMeetings";
        public static string SPInternalProjectResponsible = "InternalProjectResponsible";
        public static string SPLanguage = "Language";
        public static string SPOrderNumber = "OrderNumber";
        public static string SPOtherAvailable = "OtherAvailable";
        public static string SPOtherCommentary = "OtherCommentary";
        public static string SPOtherCost = "OtherCost";
        public static string SPOtherCostPrices1 = "OtherCosts1";
        public static string SPOtherCostPrices2 = "OtherCosts2";
        public static string SPOtherCostPrices3 = "OtherCosts3";
        public static string SPOtherCostPrices4 = "OtherCosts4";
        public static string SPOtherCostTitles1 = "OtherCostsSpec1";
        public static string SPOtherCostTitles2 = "OtherCostsSpec2";
        public static string SPOtherCostTitles3 = "OtherCostsSpec3";
        public static string SPOtherCostTitles4 = "OtherCostsSpec4";
        public static string SPOtherSpecification = "OtherSpecification";
        public static string SPParkingAvailable = "ParkingAvailable0";
        public static string SPPaymentConditions = "PaymentConditions";
        public static string SPPeriodicalFacturationAmount = "PeriodicalAmount";
        public static string SPPeriodicalFacturationDate = "PeriodicalDate";
        public static string SPPowerpointLogoUsage = "PowerpointLogoUsage";
        public static string SPProcedureDefection = "ProcedureDefection";
        public static string SPProjectCode = "ProjectCode";
        public static string SPProjectConfidentialityStatus = "ProjectConfidentialityStatus";
        public static string SPProjectID = "ID";
        public static string SPProjectTitle = "Title";
        public static string SPRepliconStatus = "RepliconStatus";
        public static string SPResource = "AssignedTo";
        public static string SPResourceRate = "Rate";
        public static string SPResourceRole = "Role";
        public static string SPResourceProfile = "Profile";
        public static string SPResourceProjectId = "Project_x0020_ID";
        public static string SPResourceType = "ResourceType";
        public static string SPResults = "Results";
        public static string SPSecondStageFacturationAmount = "SecondStageAmount";
        public static string SPSecondStageFacturationDate = "SecondStageDate";
        public static string SPSector = "Branch";
        public static string SPSMLToClient = "SMLToClient";
        public static string SPStartDate = "StartDate";
        public static string SPStrategicalMoment = "StrategicalMoment";
        public static string SPSubcontractorPresent = "SubcontractorPresent";
        public static string SPSubcontractor = "AssignedToSubcontractor0";
        public static string SPSummary = "Summary";
        public static string SPTaxNumber = "TaxNumber";
        public static string SPTaxRate = "TaxRate";
        public static string SPThirdStageFacturationAmount = "ThirdStageAmount";
        public static string SPThirdStageFacturationDate = "ThirdStageDate";
        public static string SPTimeOfCompletion = "TimeOfCompletion";
        public static string SPTotalKm = "TotalKm";
        public static string SPVersionDate = "VersionDate";
        public static string SPVersionDescription = "VersionDescription";
        public static string SPVersionInfluence = "VersionInfluence";

        // List of ConfigItem objects containing the location of information that needs to be read.
        public static List<ConfigItem> ConfigItems = new List<ConfigItem>
        {
            new ConfigItem(Language, new List<string> { "1. Identification" }, new List<string> { "C2" }),
            new ConfigItem(ProjectTitle, new List<string> { "1. Identification" }, new List<string> { "C4" }),
            new ConfigItem(LogoUrl, new List<string> { "1. Identification" }, new List<string> { "G2" }),
            new ConfigItem(ExternalEndResponsibleName, new List<string> { "1. Identification" }, new List<string> { "C9" }),
            new ConfigItem(ExternalEndResponsibleMail, new List<string> { "1. Identification" }, new List<string> { "C11" }),
            new ConfigItem(ExternalProjectResponsibleName, new List<string> { "1. Identification" }, new List<string> { "C14" }),
            new ConfigItem(ExternalProjectResponsibleMail, new List<string> { "1. Identification" }, new List<string> { "C15" }),
            new ConfigItem(InternalProjectResponsibleName, new List<string> { "1. Identification" }, new List<string> { "C17" }),
            new ConfigItem(InternalProjectResponsibleMail, new List<string> { "1. Identification" }, new List<string> { "C18" }),
            new ConfigItem(InternalEndResponsibleName, new List<string> { "1. Identification", "3. Information for MÖBIUS" }, new List<string> { "C20", "C6" }),
            new ConfigItem(InternalEndResponsibleMail, new List<string> { "1. Identification" }, new List<string> { "C21" }),
            new ConfigItem(ClientName, new List<string> { "1. Identification" }, new List<string> { "G9" }),
            new ConfigItem(SubcontractorPresent, new List<string> { "1. Identification" }, new List<string> { "H12" }),
            new ConfigItem(ClientLocationStreet, new List<string> { "1. Identification" }, new List<string> { "G14" }),
            new ConfigItem(ClientLocationCommunity, new List<string> { "1. Identification" }, new List<string> { "G14" }),
            new ConfigItem(ClientLocationCountry, new List<string> { "1. Identification" }, new List<string> { "G14" }),
            new ConfigItem(TaxNumber, new List<string> { "1. Identification" }, new List<string> { "H17" }),
            new ConfigItem(Approval, new List<string> { "1. Identification" }, new List<string> { "H18" }),
            new ConfigItem(OrderNumber, new List<string> { "1. Identification" }, new List<string> { "H20" }),
            new ConfigItem(Description, new List<string> { "1. Identification" }, new List<string> { "B26" }),
            new ConfigItem(Summary, new List<string> { "1. Identification" }, new List<string> { "B29" }),
            new ConfigItem(Sector, new List<string> { "1. Identification", "3. Information for MÖBIUS" }, new List<string> { "B32", "F6" }),
            new ConfigItem(Deliverables, new List<string> { "1. Identification" }, new List<string> { "B35" }),
            new ConfigItem(Dependencies, new List<string> { "1. Identification" }, new List<string> { "B38" }),
            new ConfigItem(VersionDescription, new List<string> { "1. Identification" }, new List<string> { "B42:B47" }),
            new ConfigItem(VersionInfluence, new List<string> { "1. Identification" }, new List<string> { "E42:E47" }),
            new ConfigItem(VersionDate, new List<string> { "1. Identification" }, new List<string> { "G42:G47" }),
            new ConfigItem(Attachments, new List<string> { "1. Identification" }, new List<string> { "C200" }),
            new ConfigItem(Entity, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "C2" }),
            new ConfigItem(CRMStatus, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "C4" }),
            new ConfigItem(ProjectCode, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "C10" }),
            new ConfigItem(StartDate, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "C12" }),
            new ConfigItem(EndDate, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "C14" }),
            new ConfigItem(ProcedureDefection, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "C16" }),
            new ConfigItem(DefectionReason, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "C17" }),
            new ConfigItem(Budget, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "G8" }),
            new ConfigItem(TaxRate, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "G10" }),
            new ConfigItem(FacturationType, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "G14" }),
            new ConfigItem(PaymentConditions, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "G15" }),
            new ConfigItem(TimeOfCompletion, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "F34" }),
            new ConfigItem(ResourceNames, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "C25:C31" }),
            new ConfigItem(ResourceRoles, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "B25:B31" }),
            new ConfigItem(ResourceDayRates, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "D25:D31" }),
            new ConfigItem(ResourceHourRates, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "E25:E31" }),
            new ConfigItem(ResourceDays, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "F25:F31" }),
            new ConfigItem(ResourceMonthlyDays, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "I25:AF31" }),
            new ConfigItem(SubcontractorNames, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "C32" }),
            new ConfigItem(SubcontractorRoles, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "B32" }),
            new ConfigItem(SubcontractorDayRates, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "D32" }),
            new ConfigItem(SubcontractorHourRates, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "E32" }),
            new ConfigItem(SubcontractorDays, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "F32" }),
            new ConfigItem(SubcontractorMonthlyDays, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "I32:AF32" }),
            new ConfigItem(OtherCostTitles, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "E41:E44" }),
            new ConfigItem(OtherCostPrices, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "G41:G44" }),
            new ConfigItem(FacturationSchedule, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "C48" }),
            new ConfigItem(FacturationInFunctionOf, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "F48" }),
            new ConfigItem(FirstStageFacturationDate, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "E53" }),
            new ConfigItem(FirstStageFacturationAmount, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "G53" }),
            new ConfigItem(SecondStageFacturationDate, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "E55" }),
            new ConfigItem(SecondStageFacturationAmount, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "G55" }),
            new ConfigItem(ThirdStageFacturationDate, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "E57" }),
            new ConfigItem(ThirdStageFacturationAmount, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "G57" }),
            new ConfigItem(FourthStageFacturationDate, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "E59" }),
            new ConfigItem(FourthStageFacturationAmount, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "G59" }),
            new ConfigItem(PeriodicalFacturationDate, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "E61" }),
            new ConfigItem(PeriodicalFacturationAmount, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "G61" }),
            new ConfigItem(BillableCosts, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "C64" }),
            new ConfigItem(TotalKm, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "D65" }),
            new ConfigItem(EuroPerKm, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "E65" }),
            new ConfigItem(SMLToClient, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "F65" }),
            new ConfigItem(GenvalToClient, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "G65" }),
            new ConfigItem(ParkingAvailable, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "D66" }),
            new ConfigItem(OtherAvailable, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "D67" }),
            new ConfigItem(OtherSpecification, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "F67" }),
            new ConfigItem(OtherCost, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "G67" }),
            new ConfigItem(ProjectConfidentialityStatus, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "D69" }),
            new ConfigItem(PowerpointLogoUsage, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "D70" }),
            new ConfigItem(OtherCommentary, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "C71" }),
            new ConfigItem(InternalMeetings, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "C75" }),
            new ConfigItem(StrategicalMoment, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "C78" }),
            new ConfigItem(Approach, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "C78" }),
            new ConfigItem(Results, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "C78" }),
            new ConfigItem(CVProjectDescription, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "C78" }),
            new ConfigItem(CVConsultantNames, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "B80:B87" }),
            new ConfigItem(CVConsultantRoles, new List<string> { "3. Information for MÖBIUS" }, new List<string> { "C80:C87" }),
            new ConfigItem(InternalControlFrequency, new List<string> { "3. Budget & other information" }, new List<string> { "C49" })
        };

        // Placeholder names.
        public static string ApprovalPlaceholder = "APPROVALPLACEHOLDER";
        public static string BillableCostsPlaceholder = "BILLABLECOSTSPLACEHOLDER";
        public static string BudgetExclPlaceholder = "BUDGETEXCLPLACEHOLDER";
        public static string BudgetInclPlaceholder = "BUDGETINCLPLACEHOLDER";
        public static string ClientEndResponsiblePlaceholder = "CLIENTENDRESPONSIBLEPLACEHOLDER";
        public static string ClientNamePlaceholder = "FULLCLIENTNAMEPLACEHOLDER";
        public static string ClientProjectResponsiblePlaceholder = "CLIENTPROJECTRESPONSIBLEPLACEHOLDER";
        public static string CommunityPlaceholder = "COMMUNITYPLACEHOLDER";
        public static string CountryPlaceholder = "COUNTRYPLACEHOLDER";
        public static string DeliverablesPlaceholder = "DELIVERABLESPLACEHOLDER";
        public static string DependenciesPlaceholder = "DEPENDENCIESPLACEHOLDER";
        public static string DescriptionPlaceholder = "DESCRIPTIONPLACEHOLDER";
        public static string EndDatePlaceholder = "ENDDATEPLACEHOLDER";
        public static string EntityPlaceholder = "ENTITYPLACEHOLDER";
        public static string FacturationSchedulePlaceholder = "FACTURATIONSCHEDULEPLACEHOLDER";
        public static string FacturationTypePlaceholder = "FACTURATIONTYPEPLACEHOLDER";
        public static string FirstPhaseAmountPlaceholder = "FIRSTPHASEAMOUNTPLACEHOLDER";
        public static string FirstPhaseDatePlaceholder = "FIRSTPHASEDATEPLACEHOLDER";
        public static string FourthPhaseAmountPlaceholder = "FOURTHPHASEAMOUNTPLACEHOLDER";
        public static string FourthPhaseDatePlaceholder = "FOURTHPHASEDATEPLACEHOLDER";
        public static string InternalMeetingsPlaceholder = "INTERNALMEETINGSPLACEHOLDER";
        public static string KmCostPlaceholder = "KMCOSTPLACEHOLDER";
        public static string LogoPlaceholder = "LOGOPLACEHOLDER";
        public static string MobiusEndResponsiblePlaceholder = "MOBIUSENDRESPONSIBLEPLACEHOLDER";
        public static string MobiusProjectResponsiblePlaceholder = "MOBIUSPROJECTRESPONSIBLEPLACEHOLDER";
        public static string OrderNumberPlaceholder = "ORDERNUMBERPLACEHOLDER";
        public static string OtherBinaryPlaceholder = "OTHERBINARYPLACEHOLDER";
        public static string OtherPlaceholder = "OTHERPLACEHOLDER";
        public static string ParkingPlaceholder = "PARKINGPLACEHOLDER";
        public static string PaymentConditionsPlaceholder = "PAYMENTCONDITIONSPLACEHOLDER";
        public static string PeriodicalAmountPlaceholder = "PERIODICAMOUNTPLACEHOLDER";
        public static string PeriodicalDatePlaceholder = "PERIODICDATEPLACEHOLDER";
        public static string ReferencePlaceholder = "REFERENCEPLACEHOLDER";
        public static string SecondPhaseAmountPlaceholder = "SECONDPHASEAMOUNTPLACEHOLDER";
        public static string SecondPhaseDatePlaceholder = "SECONDPHASEDATEPLACEHOLDER";
        public static string SectorPlaceholder = "SECTORPLACEHOLDER";
        public static string StartDatePlaceholder = "STARTDATEPLACEHOLDER";
        public static string StreetPlaceholder = "STREETPLACEHOLDER";
        public static string SubcontractorPlaceholder = "SUBCONTRACTORPLACEHOLDER";
        public static string TaxNumberPlaceholder = "TAXNUMBERPLACEHOLDER";
        public static string TaxRatePlaceholder = "TAXRATEPLACEHOLDER";
        public static string ThirdPhaseAmountPlaceholder = "THIRDPHASEAMOUNTPLACEHOLDER";
        public static string ThirdPhaseDatePlaceholder = "THIRDPHASEDATEPLACEHOLDER";
        public static string TitlePlaceholder = "TITLEPLACEHOLDER";
        public static string TotalDaysPlaceholder = "TOTALDAYSPLACEHOLDER";
        public static string TotalKmPlaceholder = "TOTALKMPLACEHOLDER";
        public static string TotalPricePlaceholder = "TOTALPRICEPLACEHOLDER";
        public static string VersionPlaceholder = "VERSIONPLACEHOLDER";

        // Template table names.
        public static string ResourceTable = "ResourceTable";

        // Array of all global project owners.
        public static string[] Owners = {
                                            "SOPDEV",
                                            "JACVER",
                                            "MIEGEL",
                                            "ANNSTE",
                                            "VALVAN",
                                            "CHRDES"
                                        };

        // Code constants.
        //public static string DefaultSiteUrl = "https://192.168.1.94";
        public static string DefaultSiteUrl = "https://portal.mobius.be"; // Live version
        //public static string DefaultProjectUrl = "https://192.168.1.94/Projects";
        public static string DefaultProjectUrl = "https://portal.mobius.be/Projects"; // Live version
        //public static string DefaultISOUrl = "https://192.168.1.94/Projects/ISO";
        public static string DefaultISOUrl = "https://portal.mobius.be/supportteams/Quality"; // Live version
        public static string MasterProjectList = "Master Project List";
        public static string PAFResourceList = "PAF Resources";
        //public static string PAFProjectList = "PAF Project List";
        public static string PAFProjectList = "PAF Projects"; // Live version
        //public static string SiteTemplate = "PROJECTSITE#0";
        public static string SiteTemplate = "{E8E95DE1-D44E-4B0E-9E48-1004436F5236}#20130422projecttemplate";
        //public static string ISOSourceRootFolder = "Shared Documents";
        //public static string ISODestinationFolder = "Shared Documents";
        public static string ISOSourceRootFolder = "PAF Documents";   // Live version
        public static string ISODestinationFolder = "ISO Documents";  // Live version
        public static string SMTPServer = "MOBE-MAIL-01.mobius.be";
        public static string SourceMail = "replicon@mobius.eu";
        //public static string DestinationMail = "chris.desmedt@mobius.eu";
        public static string DestinationMail = "replicon@mobius.eu";
        public static string DebugSourceMail = "replicon@mobius.eu";
        //public static string DebugDestinationMail = "chris.desmedt@mobius.eu";
        public static string DebugDestinationMail = "replicon@mobius.eu";
        public static string DebugSubject = "Debug message";
        public static string EmailSuffix = "@mobius.eu";
        public static string RepliconMailSubject = "Replicon automatisation";
        public static string RepliconMailContent = "A new project has been created in the SharePoint environment.\nPlease upload the .csv file from the attachments to the replicon web resources under Projects > Import Projects";
        public static string ShareLocation = @"\\MOBE-MOSS13-01\Automatisation";
        public static string DefaultCsvColumns = "Project_Name,Task_Name,Task_Start_Date,Task_Finish_Date,Resource_Names,Description,Status,Type,Billing,ProjectLeader,Outline_Level";
        public static string LoReGo = "Local ＆ Regional Government";
        public static string FedGov = "Federal Government";
        public static string BanIns = "Banking ＆ Insurance";
        public static string IndRet = "Industry ＆ retail";
        public static string Health = "Healthcare";
        public static string UtiWas = "Waste ＆ Utilities";
        public static string TeSePu = "Telecom ＆ Services ＆ Public";
        public static string EnglishReportTemplate = "Template_EN.docx";
        public static string DutchReportTemplate = "Template_NL.docx";
        public static string FrenchReportTemplate = "Template_FR.docx";
        public static int EnglishLocale = 1033;
        public static int DefaultOutlineLevel = 1;

        // Months of the year, as used in the Replicon import csv.
        public static string Januari = "Jan";
        public static string Februari = "Feb";
        public static string March = "Mar";
        public static string April = "Apr";
        public static string May = "May";
        public static string June = "Jun";
        public static string July = "Jul";
        public static string August = "Aug";
        public static string September = "Sep";
        public static string October = "Oct";
        public static string November = "Nov";
        public static string December = "Dec";

        // Months of the year, as used in the duration of a SharePoint project.
        public static List<string> Months = new List<string>()
        {
            "January",
            "February",
            "March",
            "April",
            "May",
            "June",
            "July",
            "August",
            "September",
            "October",
            "November",
            "December"
        };

        // PAF Resources month field titles.
        public static List<string> MonthFields = new List<string>()
        {
            "This_x0020_January",
            "This_x0020_February",
            "This_x0020_March",
            "This_x0020_April",
            "This_x0020_May",
            "This_x0020_June",
            "This_x0020_July",
            "This_x0020_August",
            "This_x0020_September",
            "This_x0020_October",
            "This_x0020_November",
            "This_x0020_December",
            "Next_x0020_January",
            "Next_x0020_February",
            "Next_x0020_March",
            "Next_x0020_April",
            "Next_x0020_May",
            "Next_x0020_June",
            "Next_x0020_July",
            "Next_x0020_August",
            "Next_x0020_September",
            "Next_x0020_October",
            "Next_x0020_November",
            "Next_x0020_December"
        };

        // Checkable extensions.
        public static List<string> ImageExtensions = new List<string>()
        {
            ".jpg",
            ".jpeg",
            ".gif",
            ".raw",
            ".tiff",
            ".png",
            ".bmp"
        };

        public static List<string> FirstNameStrings = new List<string>()
        {
            "First name",
            "Voornaam",
            "Prénom"
        };

        public static List<string> LastNameStrings = new List<string>()
        {
            "Surname",
            "Achternaam",
            "Nom"
        };
    }
}
