using PortalGestorias.Infrastructure.Data;
using PortalGestorias.Domain.Models;
using PortalGestorias.Infrastructure.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using PortalGestorias.Domain.Entities;
using System.Net.Mime;
using System.Globalization;
using Serilog;

namespace PortalGestorias.Business.Services
{
    public class MailService : IService
    {

        protected CrmDbContext Db;

        public MailService(CrmDbContext contexto = null)
        {
            Db = contexto ?? new CrmDbContext();
        }
        public bool MailAltaProductos(string file)
        {
            string destinatarios = ConfigurationManager.AppSettings["MailDestinatarios"];

            var plantilla = Db.CorreoPlantillas.Where(e => e.TipoCorreo == "AltaProducto").FirstOrDefault();

            var cuerpo = plantilla.Cuerpo;
  
            string[] adjunto = new string[1];
            adjunto[0] = file;
            
            try {
                SendMail(destinatarios, null, "", plantilla.Asunto, cuerpo, adjunto);
                return true;
            }
            catch (Exception e )
            {
                return false;
            }
        }

     

        public string MonthName(int month)
        {
            DateTimeFormatInfo dtinfo = new CultureInfo("es-ES", false).DateTimeFormat;
            return dtinfo.GetMonthName(month).ToUpper();
        }


        protected virtual void SendMail(string para, string copia, string copiaOculta, string asunto, string cuerpo, string[] documentos = null)
        {
           
            if (bool.Parse(ConfigurationManager.AppSettings["MailActivo"]))
            {
                if (bool.Parse(ConfigurationManager.AppSettings["EsEntornoPruebas"]))
                {
                    para = ConfigurationManager.AppSettings["MailToPruebas"];
                }

                MailMessage mail = new MailMessage(ConfigurationManager.AppSettings["MailFrom"], para);

                MailAddress email;

                if (!bool.Parse(ConfigurationManager.AppSettings["EsEntornoPruebas"]))
                {
                    if (copia != null)
                    {
                        string[] correosEnCopia = copia.Split(';');
                        foreach (string correo in correosEnCopia)
                        {
                            if (correo != "")
                            {
                                email = new MailAddress(correo);
                                mail.CC.Add(email);
                            }

                        }
                    }
                    if (copiaOculta != null)
                    {
                        string[] correosEnCopiaOculta = copiaOculta.Split(';');
                        foreach (string correo in correosEnCopiaOculta)
                        {
                            if (correo != "")
                            {
                                email = new MailAddress(correo);
                                mail.CC.Add(email);
                            }
                        }
                    }
                }
                if (documentos != null && documentos.Length != 0)
                {
                    foreach (string doc in documentos)
                    {
                        if (doc != "")
                        {
                            Attachment data = new Attachment(doc, MediaTypeNames.Application.Octet);
                            mail.Attachments.Add(data);
                        }
                    }
                }

                mail.Subject = asunto;
                mail.IsBodyHtml = true;
                mail.Body = cuerpo;

                Log.Information("Subject:" + mail.Subject);
                Log.Information("Cuerpo:" + mail.Body);

                SmtpClient mailServer = new SmtpClient(ConfigurationManager.AppSettings["MailServer"], Convert.ToInt32(ConfigurationManager.AppSettings["MailPort"]));
                mailServer.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["MailUser"], ConfigurationManager.AppSettings["MailPass"]);

                Log.Information("Para:" + mail.To);
                mailServer.EnableSsl = bool.Parse(ConfigurationManager.AppSettings["MailSSL"].ToString());

                try
                {
                    mailServer.Send(mail);
                }

                catch (Exception e) {
                    Log.Information("Excepción:" + e.ToString());
                }
            }

        }

        /*private string DecryptPassword(string encryptedPass)
        {
            var pass = encryptedPass;
            var certificate = certificateService.Get();
            var privateKey = certificate.PrivateKey as RSACryptoServiceProvider;

            if (privateKey != null)
            {
                pass = Encoding.UTF8.GetString(privateKey.Decrypt(Convert.FromBase64String(encryptedPass), RSAEncryptionPadding.OaepSHA1));
            }

            return pass;
        }*/


    }
}
