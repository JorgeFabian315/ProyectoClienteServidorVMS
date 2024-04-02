using ProyectoClienteServidorVMS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;

namespace ProyectoClienteServidorVMS.Services
{
    public class VMSServices
    {
        private int _puerto = 6001;
        HttpListener server = new();
        public event EventHandler<VMS>? VMSRecibido;
        public VMSServices()
        {
            server.Prefixes.Add($"http://*:{_puerto}/vms/");
        }
        public void Iniciar()
        {
            if (!server.IsListening)
            {
                server.Start();
                new Thread(Escuchar) { IsBackground = true }.Start();
            }

        }

        private void Escuchar()
        {
            while (true)
            {
                HttpListenerContext? context = server.GetContext();
                if (context != null)
                {
                    string indexHtml = File.ReadAllText("assets/index.html");
                    string estilos = File.ReadAllText("assets/style.css");
                    string paginaRegresar = File.ReadAllText("assets/regresar.html");

                    if (context.Request.Url != null)
                    {
                        if (context.Request.Url.LocalPath == "/vms/")
                        {
                            EnviarRespuesta(context, indexHtml, estilos);
                        }

                        else if (context.Request.HttpMethod == "POST" && context.Request.Url.LocalPath == "/vms/crear")
                        {
                            VMS vms = new();

                            RecibirVMS(context,ref vms);

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                VMSRecibido?.Invoke(this, vms);
                            });

                            EnviarRespuesta(context, paginaRegresar, estilos);
                        }
                        else
                        {
                            context.Response.StatusCode = 400;
                            context.Response.Close();
                        }
                    }
                }
            }
        }



        public void EnviarRespuesta(HttpListenerContext context, string contenido, string estilos)
        {
            contenido = contenido.Replace("</head>", $"<style>{estilos}</style></head>");
            byte[] buffer = Encoding.UTF8.GetBytes(contenido);
            context.Response.ContentLength64 = buffer.Length;
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.Close();
        }

        public void RecibirVMS(HttpListenerContext context,ref VMS vms)
        {
            byte[] bufferdatos = new byte[context.Request.ContentLength64];
            context.Request.InputStream.Read(bufferdatos, 0, bufferdatos.Length);
            string datos = Encoding.UTF8.GetString(bufferdatos);

            var diccionario = HttpUtility.ParseQueryString(datos);

            vms.Mensaje = diccionario["mensaje"] ?? "";
        }


    }
}
