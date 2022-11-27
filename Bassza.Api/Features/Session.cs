namespace Bassza.Api.Features;

public class Session
{
    private LoginDetailsDto? _loginDetails;

    public Session(){}
    
    public static Session Create(LoginDetailsDto loginDetailsDto)
    {
        var obj = new Session
        {
            _loginDetails = loginDetailsDto
        };

        return obj;
    }

    public bool LoginOpen()
    {
        if (_loginDetails == null) return false;


        var loginInit = new RequestDto()
        {
            EndPointDto = Endpoints.Login
        };

        try
        {
            var responseInit = loginInit.RunRequest();

            if (responseInit == null) return false;
        
            //var addCookieHeaders = responseInit?.Headers.Get("Set-Cookie");

            var addCookieHeaders = responseInit.Headers.GetValues("Set-Cookie");

            var cookieHeaders = addCookieHeaders.ToList()[0].Split(";");
            if (!cookieHeaders.Any()) return false;
        
            foreach (var cookie in cookieHeaders)
            {
                OlemsController.AddCookie(cookie);
            }
        }
        catch (Exception e)
        {
            return false;
        }

        var requestExecute = new RequestDto
        {
            EndPointDto = Endpoints.LoginExecute
        };
        
        requestExecute.BodyData.Add("fldUserID", _loginDetails.Username);
        requestExecute.BodyData.Add("fldPassword", _loginDetails.Password);
        requestExecute.BodyData.Add("fldPreferredUserID", _loginDetails.Username);
        requestExecute.BodyData.Add("fldInitialPassword", _loginDetails.Password);
        requestExecute.BodyData.Add("fldConfirmPassword", _loginDetails.Password);
        requestExecute.BodyData.Add("fldEmail", "");
        requestExecute.BodyData.Add("fldEmailConfirm", "");
        requestExecute.BodyData.Add("fldCnID", "1");
        requestExecute.BodyData.Add("fldBID", "");

        try
        {
            var responseExecute = requestExecute.RunRequest();
            if (responseExecute!.Headers.Contains("Location"))
            {
                var locationValue = responseExecute.Headers.GetValues("Location");
                if (locationValue.Any(hd => hd.Contains("Authorise-2FA.asp"))) return true;
            }
        }
        catch (Exception e)
        {
            return false;
        }
        

        return false;
    }
    
    
}