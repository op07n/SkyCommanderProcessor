using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SkyCommanderProcessor.EXLogic {
  public class HttpPostStatus {
    public HttpStatusCode StatusCode { get; set; }
    public String ResponseText { get; set; }    
  }

  public class SMS {
    private String _SMSNumber;
    private String _SMSBody;

    public SMS(String SMSNumber, String SMSBody) {
      _SMSNumber = SMSNumber;
      _SMSBody = SMSBody;
    }

    public async Task<AlertSendStatus> Send() {

      String url = "https://www.smsglobal.com/http-api.php";
      String username = "dna8n7cj";
      String password = "4fzidBpV";
      String Body = WebUtility.UrlEncode(_SMSBody);

      string data =
      "action=sendsms" +
      "&user=" + WebUtility.UrlEncode(username) +
      "&password=" + WebUtility.UrlEncode(password) +
      "&from=EXPONENT" +
      "&to=" + _SMSNumber +
      "&text=" + Body +
      "&maxsplit=3";

      HttpPostStatus PostResult = await Post(url, data);
      var result = new AlertSendStatus {
        IsSend = PostResult.StatusCode == HttpStatusCode.OK,
        ProviderMessage = PostResult.ResponseText
      };

      return result;
    }

    public async Task<HttpPostStatus> Post(string url, string data) {
      HttpPostStatus httpPostStatus = new HttpPostStatus {
        StatusCode = HttpStatusCode.ServiceUnavailable,
        ResponseText = String.Empty
      };

      try {
        //throw new Exception();
        byte[] buffer = Encoding.Default.GetBytes(data);

        HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(url);
        WebReq.Method = "POST";
        WebReq.ContentType = "application/x-www-form-urlencoded";
        WebReq.ContentLength = buffer.Length;

        Stream PostData = await WebReq.GetRequestStreamAsync();
        await PostData.WriteAsync(buffer, 0, buffer.Length);
        PostData.Close();
        HttpWebResponse WebResp = (HttpWebResponse)await WebReq.GetResponseAsync();
        httpPostStatus.StatusCode = WebResp.StatusCode;
        Stream Response = WebResp.GetResponseStream();
        StreamReader _Response = new StreamReader(Response);
        httpPostStatus.ResponseText = await _Response.ReadToEndAsync();
      } catch (Exception ex) {
        httpPostStatus.ResponseText = ex.Message;
      }

      return httpPostStatus;
    }


  }
}
