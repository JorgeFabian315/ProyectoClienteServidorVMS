using ProyectoClienteServidorVMS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
            while(true)
            {
                HttpListenerContext? context = server.GetContext();
                if (context != null)
                {
                    var pagina = File.ReadAllText("assets/index.html");
                    byte[] buffer = Encoding.UTF8.GetBytes(pagina);

                    var paginaRegresar = File.ReadAllText("assets/regresar.html");
                    byte[] buffer2 = Encoding.UTF8.GetBytes(paginaRegresar);

                    if (context.Request.Url != null)
                    {
                        if(context.Request.Url.LocalPath == "/vms/")
                        {
                            context.Response.ContentLength64 = buffer.Length;
                            context.Response.OutputStream.Write(buffer,0,buffer.Length);

                            context.Response.StatusCode = (int)HttpStatusCode.OK;
                            context.Response.Close();
                        }

                        else if(context.Request.HttpMethod == "POST" && context.Request.Url.LocalPath == "/vms/crear")
                        {
                            byte[] bufferdatos = new byte[context.Request.ContentLength64];
                            context.Request.InputStream.Read(bufferdatos, 0, bufferdatos.Length);
                            string datos = Encoding.UTF8.GetString(bufferdatos);

                            context.Response.ContentLength64 = buffer2.Length;
                            context.Response.OutputStream.Write(buffer2, 0, buffer2.Length);

                            var diccionario = HttpUtility.ParseQueryString(datos);

                            VMS vms = new()
                            {
                                Mensaje = diccionario["mensaje"] ?? ""
                            };

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                VMSRecibido?.Invoke(this, vms);
                            });

                            context.Response.StatusCode = (int)HttpStatusCode.OK;
                            context.Response.Close();
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

    }
}
