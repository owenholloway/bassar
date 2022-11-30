using System.Net;
using Serilog;

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

    public bool LoginOpen(string mfaCode = "")
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
                if (locationValue.Any(hd => hd.Contains("Authorise-2FA.asp")) && CheckForMFA(mfaCode)) return true;
            }
        }
        catch (Exception e)
        {
            Log.Error($"Could not login {e.Message}");
            return false;
        }
        

        return false;
    }

    private bool CheckForMFA(string mfaCode)
    {
        var request = new RequestDto()
        {
            EndPointDto = Endpoints.Authorise2FAGet,
        };
        
        var responseExecute = request.RunRequest();

        if (responseExecute.StatusCode != HttpStatusCode.Redirect)
        {
            //We have to do a TOTP Check
            var requestTotp = new RequestDto()
            {
                EndPointDto = Endpoints.Authorise2FAPost,
            };
            
            requestTotp.BodyData.Add("fldGoogleCode", mfaCode);

            var responseTotp = requestTotp.RunRequest();

            if (responseTotp.StatusCode != HttpStatusCode.Redirect) return false;
            var locationValue = responseTotp.Headers.GetValues("Location");

            var s1 = locationValue.First().Split("?")[1].Split("=")[1];
            
            var phpCodeRequest = new RequestDto()
            {
                EndPointDto = Endpoints.AuthPHP
            };

            phpCodeRequest.UrlData["s1"] = s1;

            var phpCodeResponse = phpCodeRequest.RunRequest();
            if (phpCodeResponse.StatusCode != HttpStatusCode.Redirect) return false;
            var phpCodeLocationValue = phpCodeResponse.Headers.GetValues("Location");

            var urlCodes = phpCodeLocationValue.First().Split("?")[1].Split("&");
            
            var authRequest = new RequestDto()
            {
                EndPointDto = Endpoints.Authorise2FAGet
            };
            
            foreach (var urlCode in urlCodes)
            {
                var varPair = urlCode.Split("=");
                authRequest.UrlData[varPair[0]] = varPair[1];

            }

            var authResponseMessage = authRequest.RunRequest();
            
            if (authResponseMessage.StatusCode != HttpStatusCode.Redirect) return false;

        };

        return true;
    }
    
    
}