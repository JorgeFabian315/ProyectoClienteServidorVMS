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

                try
                {
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

                                RecibirVMS(context, ref vms);

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
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public void EnviarRespuesta(HttpListenerContext context, string contenido, string? estilos = null)
        {
            try
            {
                if (estilos != null)
                    contenido = contenido.Replace("</head>", $"<style>{estilos}</style></head>");

                byte[] buffer = Encoding.UTF8.GetBytes(contenido);
                context.Response.ContentLength64 = buffer.Length;
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void RecibirVMS(HttpListenerContext context, ref VMS vms)
        {
            try
            {
                byte[] bufferdatos = new byte[context.Request.ContentLength64];
                context.Request.InputStream.Read(bufferdatos, 0, bufferdatos.Length);
                string datos = Encoding.UTF8.GetString(bufferdatos);

                var diccionario = HttpUtility.ParseQueryString(datos);

                vms.Linea1 = diccionario["linea1"] ?? "";
                vms.Linea2 = diccionario["linea2"] ?? "";
                vms.Linea3 = diccionario["linea3"] ?? "";
                vms.ColorLinea1 = diccionario["colorlinea1"] == "" ? "#FFF" : diccionario["colorlinea1"] ?? "";
                vms.ColorLinea2 = diccionario["colorlinea2"] == "" ? "#FFF" : diccionario["colorlinea2"] ?? "";
                vms.ColorLinea3 = diccionario["colorlinea3"] == "" ? "#FFF" : diccionario["colorlinea3"] ?? "";
                vms.ImagenLinea1 = diccionario["imagenlinea1"] ?? "";
                vms.ImagenLinea2 = diccionario["imagenlinea2"] ?? "";
                vms.ImagenLinea3 = diccionario["imagenlinea3"] ?? "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void Apagar()
        {
            if (server.IsListening)
            {
                server.Close();
            }
        }

    }
}
