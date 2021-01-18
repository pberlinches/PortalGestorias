using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using PortalGestorias.Domain.Entities;
using PortalGestorias.Infrastructure.Data;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;

namespace PortalGestorias.Business.Services
{
    public class iCalServices : IService
    {
        protected CrmDbContext Db;

        public iCalServices(CrmDbContext contexto = null)
        {
            Db = contexto ?? new CrmDbContext();
        }

        //public bool EnviarAltaConvocatoria(Pedido actividad)
        //{
        //    try
        //    {

        //        var calendar = new Ical.Net.Calendar();
        //        calendar.Scale = "GREGORIAN";
        //        calendar.Method = "REQUEST";

        //        CalendarEvent evento = new CalendarEvent();

        //        //string asunto = actividad.Cliente.Nombre + " ::  " + actividad.Descripcion + " -  " + actividad.Propietario.NombreCompleto;
        //        //evento.Summary = asunto;
        //        //evento.Description = asunto;

        //        string cuerpo = ""; /* @"<b>- Cliente:</b> " + actividad.Cliente.Nombre + "<br/>" +
        //                         "<b>- Gerente:</b> " + actividad.Propietario.Nombre + "<br/>" +
        //                         "<b>- Contacto:</b> " + actividad.Contacto.NombreCompleto + "<br/>" +
        //                         "<b>- Fecha Visita:</b> " + actividad.FechaVencimiento.ToString("dd/MM/yyyy") + "<br/>" +
        //                         "<b>- Hora Inicio:</b> " + actividad.HoraInicio + "<br/>" +
        //                         "<b>- Hora Fin:</b> " + actividad.HoraFin + "<br/>" +
        //                         "<b>- Descripción:</b> " + actividad.Descripcion + "<br/>";*/

        //        evento.Status = "CONFIRMED";                

        //        evento = rellenarAtributosComunesEvento(actividad, evento);

        //        calendar.Events.Add(evento);

        //        var serializer = new CalendarSerializer(new SerializationContext());
        //        var serializedCalendar = serializer.SerializeToString(calendar);
        //        var bytesCalendar = Encoding.UTF8.GetBytes(serializedCalendar);
        //        MemoryStream ms = new MemoryStream(bytesCalendar);
        //        System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(ms, "invite.ics", "text/calendar");

        //        System.Net.Mime.ContentType ct = new System.Net.Mime.ContentType("text/calendar");
        //        ct.Parameters.Add("method", "REQUEST");
        //        AlternateView avCal = AlternateView.CreateAlternateViewFromString(serializer.SerializeToString(calendar), ct);

        //        string EmailPersonalDestinario = "";
        //        string EmailCorporativo = "";

        //        //if (actividad.Propietario.NotificarAgendaEmailPersonal && actividad.Propietario.EmailPersonal != null && actividad.Propietario.EmailPersonal != "" && actividad.Propietario.EmailPersonal != "u4bw@sin.correo" )
        //        //{
        //        //    EmailPersonalDestinario = actividad.Propietario.EmailPersonal;
        //        //}

        //        //if (actividad.Propietario.NotificarAgendaEmailNCS)
        //        //{
        //        //    EmailCorporativo = actividad.Propietario.EmailCorporativo;
        //        //}

        //        SendMail(EmailCorporativo, ConfigurationManager.AppSettings["MailEstaticoConvocatoriasCalendario"], EmailPersonalDestinario, "", cuerpo, attachment, avCal);

        //    }
        //    catch (Exception)
        //    {

        //        return false;
        //    }

        //    return true;
        //}


        //public bool EnviarModificacionConvocatoria(Pedido actividad)
        //{
        //    try
        //    {

        //        var calendar = new Ical.Net.Calendar();
        //        calendar.Scale = "GREGORIAN";
        //        calendar.Method = "REQUEST";

        //        CalendarEvent evento = new CalendarEvent();

        //        //string asunto = "Modificada  " + actividad.Cliente.Nombre + " :: " + actividad.Descripcion + " - " + actividad.Propietario.NombreCompleto;
        //        //evento.Summary = asunto; 
        //        //evento.Description = asunto;

        //        evento.Status = "CONFIRMED";                

        //        evento = rellenarAtributosComunesEvento(actividad, evento);

        //        calendar.Events.Add(evento);

        //        var serializer = new CalendarSerializer(new SerializationContext());
        //        var serializedCalendar = serializer.SerializeToString(calendar);
        //        var bytesCalendar = Encoding.UTF8.GetBytes(serializedCalendar);
        //        MemoryStream ms = new MemoryStream(bytesCalendar);
        //        System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(ms, "invite.ics", "text/calendar");

        //        System.Net.Mime.ContentType ct = new System.Net.Mime.ContentType("text/calendar");
        //        ct.Parameters.Add("method", "REQUEST");
        //        AlternateView avCal = AlternateView.CreateAlternateViewFromString(serializer.SerializeToString(calendar), ct);

        //        string EmailPersonalDestinario = "";
        //        string EmailCorporativo = "";

        //        //if (actividad.Propietario.NotificarAgendaEmailPersonal && actividad.Propietario.EmailPersonal != null && actividad.Propietario.EmailPersonal != "" && actividad.Propietario.EmailPersonal != "u4bw@sin.correo")
        //        //{
        //        //    EmailPersonalDestinario = actividad.Propietario.EmailPersonal;
        //        //}

        //        //if (actividad.Propietario.NotificarAgendaEmailNCS)
        //        //{
        //        //    EmailCorporativo = actividad.Propietario.EmailCorporativo;
        //        //}

        //        SendMail(EmailCorporativo, ConfigurationManager.AppSettings["MailEstaticoConvocatoriasCalendario"], EmailPersonalDestinario, "", "", attachment, avCal);

        //    }
        //    catch (Exception)
        //    {

        //        return false;
        //    }

        //    return true;
        //}

        //public bool EnviarCancelacionConvocatoria(Pedido actividad)
        //{
        //    try
        //    {

        //        var calendar = new Ical.Net.Calendar();
        //        calendar.Scale = "GREGORIAN";
        //        calendar.Method = "CANCEL";

        //        CalendarEvent evento = new CalendarEvent();

        //        //string asunto = "Cancelada: " + actividad.Cliente.Nombre + " :: " + actividad.Descripcion + " - " + actividad.Propietario.NombreCompleto;
        //        //evento.Summary = asunto;;
        //        //evento.Description = asunto;

        //        evento.Status = "CANCELLED";

        //        evento = rellenarAtributosComunesEvento(actividad, evento);

        //        calendar.Events.Add(evento);

        //        var serializer = new CalendarSerializer(new SerializationContext());
        //        var serializedCalendar = serializer.SerializeToString(calendar);
        //        var bytesCalendar = Encoding.UTF8.GetBytes(serializedCalendar);
        //        MemoryStream ms = new MemoryStream(bytesCalendar);
        //        System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(ms, "invite.ics", "text/calendar");

        //        System.Net.Mime.ContentType ct = new System.Net.Mime.ContentType("text/calendar");
        //        ct.Parameters.Add("method", "REQUEST");
        //        AlternateView avCal = AlternateView.CreateAlternateViewFromString(serializer.SerializeToString(calendar), ct);

        //        string EmailPersonalDestinario = "";
        //        string EmailCorporativo = "";

        //        //if (actividad.Propietario.NotificarAgendaEmailPersonal && actividad.Propietario.EmailPersonal != null && actividad.Propietario.EmailPersonal != "" && actividad.Propietario.EmailPersonal != "u4bw@sin.correo")
        //        //{
        //        //    EmailPersonalDestinario = actividad.Propietario.EmailPersonal;
        //        //}

        //        //if (actividad.Propietario.NotificarAgendaEmailNCS)
        //        //{
        //        //    EmailCorporativo = actividad.Propietario.EmailCorporativo;
        //        //}

        //        SendMail(EmailCorporativo, ConfigurationManager.AppSettings["MailEstaticoConvocatoriasCalendario"], EmailPersonalDestinario, "", "", attachment, avCal);

        //    }
        //    catch (Exception)
        //    {

        //        return false;
        //    }

        //    return true;
        //}

        //private CalendarEvent rellenarAtributosComunesEvento(Pedido actividad, CalendarEvent evento)
        //{
        //    evento.Class = "PUBLIC";

        //    //evento.Created = new CalDateTime(Convert.ToDateTime(actividad.HoraInicio));
        //    //evento.DtStart = new CalDateTime(Convert.ToDateTime(actividad.HoraInicio));
        //    //evento.DtEnd = new CalDateTime(Convert.ToDateTime(actividad.HoraFin));
        //    //evento.LastModified = new CalDateTime(DateTime.Now);
        //    evento.DtStamp = new CalDateTime(DateTime.Now);
        //    evento.Sequence = 0;
        //    evento.Location = "";
        //    evento.Uid = actividad.Id.ToString();
        //    //evento.Uid = "18b8c5a9-f68b-4d49-9e97-43a611efc125";
        //    evento.Transparency = TransparencyType.Transparent;



        //    evento.Organizer = new Organizer
        //    {
        //        CommonName = "NCS - Robot Correos",
        //        Value = new Uri("mailto:" + ConfigurationManager.AppSettings["MailFrom"].ToString())
        //    };

        //    Attendee attendee = new Attendee
        //    {
        //        CommonName = "Cuenta GMAIL Común",
        //        ParticipationStatus = "REQ-PARTICIPANT",
        //        Rsvp = true,
        //        Value = new Uri("mailto:" + ConfigurationManager.AppSettings["MailEstaticoConvocatoriasCalendario"].ToString())
        //    };
        //    evento.Attendees.Add(attendee);


        //    //if (actividad.Propietario.NotificarAgendaEmailNCS)
        //    //{
        //    //    attendee = new Attendee
        //    //    {
        //    //        CommonName = actividad.Propietario.NombreCompleto,
        //    //        ParticipationStatus = "REQ-PARTICIPANT",
        //    //        Rsvp = true,
        //    //        Value = new Uri("mailto:" + actividad.Propietario.EmailCorporativo)
        //    //    };
        //    //    evento.Attendees.Add(attendee);
        //    //}


        //    //if (actividad.Propietario.NotificarAgendaEmailPersonal && actividad.Propietario.EmailPersonal != null && actividad.Propietario.EmailPersonal != "" && actividad.Propietario.EmailPersonal != "u4bw@sin.correo")                
        //    //{
        //    //    attendee = new Attendee
        //    //    {
        //    //        CommonName = actividad.Propietario.NombreCompleto,
        //    //        ParticipationStatus = "REQ-PARTICIPANT",
        //    //        Rsvp = true,
        //    //        Value = new Uri("mailto:" + actividad.Propietario.EmailPersonal)
        //    //    };
        //    //    evento.Attendees.Add(attendee);
        //    //}

        //    return evento;

        //}

        private static void SendMail(string para, string copia, string copiaOculta, string asunto, string cuerpo, System.Net.Mail.Attachment ics, AlternateView avCal)
        {
            if (bool.Parse(ConfigurationManager.AppSettings["MailActivo"]))
            {
                if (bool.Parse(ConfigurationManager.AppSettings["EsEntornoPruebas"]))
                {
                    para = ConfigurationManager.AppSettings["MailToPruebas"];
                }

                if (para == "")
                {
                    para = copia;
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
                                mail.Bcc.Add(email);
                            }
                        }
                    }
                }

                if (ics != null)
                {
                    mail.Attachments.Add(ics);
                }

                if (avCal != null)
                {
                    mail.AlternateViews.Add(avCal);
                }

                mail.Subject = asunto;
                mail.IsBodyHtml = true;
                mail.Body = cuerpo;

                SmtpClient mailServer = new SmtpClient(ConfigurationManager.AppSettings["MailServer"], Convert.ToInt32(ConfigurationManager.AppSettings["MailPort"]));
                mailServer.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["MailUser"], ConfigurationManager.AppSettings["MailPass"]);
                mailServer.Send(mail);
            }
        }

    }
}

