using Android.App;
using Android.Content;
using Android.Icu.Text;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ubi_health
{
    [Activity(Label = "Agenda")]
    public class Agenda : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            string latusuario, lonusuario;
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Agenda);

            var btnAceptar = FindViewById<Button>(Resource.Id.btnaceptar);
            var txtNombre = FindViewById<EditText>(Resource.Id.txtname);
            var txtEdad = FindViewById<EditText>(Resource.Id.txtage);
            var txtTelefono = FindViewById<EditText>(Resource.Id.txtphone);
            var txtCorreo = FindViewById<EditText>(Resource.Id.txtemail);
            var txtDia = FindViewById<EditText>(Resource.Id.txtday);
            var txtMes = FindViewById<EditText>(Resource.Id.txtmonth);
            var txtAnio = FindViewById<EditText>(Resource.Id.txtyear);
            latusuario = Intent.GetStringExtra("Latitud");
            lonusuario = Intent.GetStringExtra("Longitud");

            btnAceptar.Click += async delegate
            {
                var StorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=programacionparamovilesb;AccountKey=9PZM1I5tcsvetJbY6E6hYVU5dHbUxEhR10kt0MRVNsj42TeTKX6dlWQeSmgCmfKgZFmt+z+JVNApy7zMr3w93Q==;EndpointSuffix=core.windows.net");
                var TableNoSQL = StorageAccount.CreateCloudTableClient();
                var Table = TableNoSQL.GetTableReference("AgendaCitas");
                await Table.CreateIfNotExistsAsync();

                var cita = new Cita("Cita", txtNombre.Text);
                cita.Edad = int.Parse(txtEdad.Text);
                cita.Correo = txtCorreo.Text;
                cita.Telefono = txtTelefono.Text;
                cita.Fecha = txtDia.Text + "/" + txtMes.Text + "/" + txtAnio.Text;
                //cita.LatitudOrigen = latusuario.ToString();
                //cita.LongitudOrigen = lonusuario.ToString();

                var Store = TableOperation.Insert(cita);
                await Table.ExecuteAsync(Store);
                Toast.MakeText(this.ApplicationContext, "Su cita ha sido agendada correctamente", ToastLength.Long).Show();
            };
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }

    public class Cita : TableEntity
    {
        public Cita(string Category, string Name)
        {
            PartitionKey = Category;
            RowKey = Name;
        }
        public int Edad { get; set; }
        public string Fecha { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string LatitudOrigen { get; set; }
        public string LongitudOrigen { get; set; }
    }
}