using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProyectoClienteServidorVMS.Models;
using ProyectoClienteServidorVMS.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ProyectoClienteServidorVMS.ViewModels
{
    public partial class VMSViewModel : ObservableObject
    {
        private VMSServices _server = new();
        public ObservableCollection<VMS> ListaVMS { get; set; } = new();

        [ObservableProperty]
        public VMS vms = new();

        [ObservableProperty]
        public int indicevms;
        public VMSViewModel()
        {
            _server.Iniciar();
            _server.VMSRecibido += Server_VMSRecibido;
            CargarDatos();
            Vms = ListaVMS.FirstOrDefault() ?? new();

        }

        private void Server_VMSRecibido(object? sender, VMS e)
        {
            if (e != null)
            {
                ListaVMS.Add(e);
                Indicevms = ListaVMS.IndexOf(e);
                GuardarArchivo();
            }
        }


        [RelayCommand]
        private void Eliminar(VMS vmsp)
        {
            if (vmsp != null)
            {
                ListaVMS.Remove(vmsp);
                Vms = ListaVMS.FirstOrDefault() ?? new();
                GuardarArchivo();
            }
        }


        private string _nombreJson = "vms.json";
        public void GuardarArchivo()
        {
            var json = JsonSerializer.Serialize(ListaVMS);
            File.WriteAllText(_nombreJson, json);
        }

        public void CargarDatos()
        {
            if (File.Exists(_nombreJson))
            {
                var json = File.ReadAllText(_nombreJson);
                var datos = JsonSerializer.Deserialize<ObservableCollection<VMS>?>(json);
                if (datos != null)
                    ListaVMS = datos;
                else
                    ListaVMS = new ObservableCollection<VMS>();
            }
        }


    }
}
