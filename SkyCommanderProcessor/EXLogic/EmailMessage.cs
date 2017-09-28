using EASendMail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SkyCommanderProcessor.Models;

namespace SkyCommanderProcessor.EXLogic {
  public class EmailMessage {
    PortalAlertEmail emailMessage;
    public EmailMessage(PortalAlertEmail portalAlertEmail) {
      emailMessage = portalAlertEmail;
    }


    public AlertSendStatus Send() {
      AlertSendStatus alertSendStatus = new AlertSendStatus {
        IsSend = false,
        ProviderMessage = "Waiting"
      };
      try {
        CDO.Message oMsg = new CDO.Message();
        CDO.IConfiguration iConfg;

        iConfg = oMsg.Configuration;

        ADODB.Fields oFields;
        oFields = iConfg.Fields;

        // Set configuration.
        oFields["http://schemas.microsoft.com/cdo/configuration/sendusing"].Value = 2;
        oFields["http://schemas.microsoft.com/cdo/configuration/smtpserver"].Value = "smtp.gmail.com";
        oFields["http://schemas.microsoft.com/cdo/configuration/smptserverport"].Value = 587;
        oFields["http://schemas.microsoft.com/cdo/configuration/smtpauthenticate"].Value = 1;
        oFields["http://schemas.microsoft.com/cdo/configuration/smtpusessl"].Value = true;
        oFields["http://schemas.microsoft.com/cdo/configuration/smtpconnectiontimeout"].Value = 60;
        oFields["http://schemas.microsoft.com/cdo/configuration/sendusername"].Value = "portal@exponent-ts.com";
        oFields["http://schemas.microsoft.com/cdo/configuration/sendpassword"].Value = "drone123";

        oFields.Update();

        oMsg.CreateMHTMLBody(emailMessage.EmailURL);
        oMsg.Subject = emailMessage.EmailSubject;

        //TODO: Change the To and From address to reflect your information.                       
        oMsg.From = "Exponent <portal@exponent-ts.com>";
        oMsg.To = emailMessage.ToAddress;
        
        //ADD attachment.
        //TODO: Change the path to the file that you want to attach.
        if(!String.IsNullOrEmpty(emailMessage.Attachments)) {
          foreach(String FileWithPath in emailMessage.Attachments.Split(',')) {
            oMsg.AddAttachment(FileWithPath);
          }
        }

        oMsg.Send();
        alertSendStatus.IsSend = true;
        alertSendStatus.ProviderMessage = "Send Successfully";
      } catch (Exception e) {
        alertSendStatus.ProviderMessage = e.Message;
      }

      return alertSendStatus;

    }
  }
}
