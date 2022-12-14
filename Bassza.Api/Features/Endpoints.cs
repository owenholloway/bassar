namespace Bassza.Api.Features
{
    public static class Endpoints
    {
        public static string BaseUrl = "registrations.appleislemoot.com.au";
        
        public static EndPointDto Login = new EndPointDto()
        {
            FunctionType = FunctionTypeEnum.GET,
            Function = "/Login.asp"
        };
        
        public static EndPointDto LoginExecute = new EndPointDto()
        {
            FunctionType = FunctionTypeEnum.POST,
            Function = "/Login-Execute.asp",
            UrlData = new Dictionary<string, string>()
            {
                {"NextPage",""},
                {"type", "Login"}
            }
        };
        
        public static EndPointDto Logout = new EndPointDto()
        {
            FunctionType = FunctionTypeEnum.GET,
            Function = "/Logout.asp"
        };

        public static EndPointDto ReportMedical = new EndPointDto()
        {
            FunctionType = FunctionTypeEnum.GET,
            Function = "Report-medical.asp"
        };

        public static EndPointDto BasicDetailsWithEmails = new EndPointDto()
        {
            FunctionType = FunctionTypeEnum.GET,
            Function = "Report-Template.asp",
            UrlData = new Dictionary<string, string>()
            {
                {"RTID","78"}
            }
            
        };
        
        public static EndPointDto ExpeditionAllocation = new EndPointDto()
        {
            FunctionType = FunctionTypeEnum.GET,
            Function = "Report-Template.asp",
            UrlData = new Dictionary<string, string>()
            {
                {"RTID","93"}
            }
            
        };
        
        public static EndPointDto PaymentDetailsExtended = new EndPointDto()
        {
            FunctionType = FunctionTypeEnum.GET,
            Function = "Report-Payments(ExtSummary).asp"

        };

        public static EndPointDto OffSiteActivities = new EndPointDto()
        {
            FunctionType = FunctionTypeEnum.GET,
            Function = "Report-OffSiteActivities.asp"

        };

        public static EndPointDto Authorise2FAGet = new EndPointDto()
        {
            FunctionType = FunctionTypeEnum.GET,
            Function = "Authorise-2FA.asp"
        };
        
        public static EndPointDto Authorise2FAPost = new EndPointDto()
        {
            FunctionType = FunctionTypeEnum.POST,
            Function = "Authorise-2FA.asp"
        };
        
        public static EndPointDto AuthPHP = new EndPointDto()
        {
            FunctionType = FunctionTypeEnum.GET,
            Function = "CORE/2fa/auth.php"
        };


        public static EndPointDto MailingLabels = new EndPointDto()
        {
            FunctionType = FunctionTypeEnum.GET,
            Function = "Report-mailingLabels.asp"
        };

        public static EndPointDto PaymentPayWay = new EndPointDto()
        {
            FunctionType = FunctionTypeEnum.GET,
            Function = "Report-PaymentPayWay.asp"
        };
        
        public static EndPointDto Payments = new EndPointDto()
        {
            FunctionType = FunctionTypeEnum.GET,
            Function = "Report-Payments.asp"
        };

    }
}