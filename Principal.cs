using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.IO;
using System.Linq;
using Android.Content;
using System.Threading.Tasks;
using Android.Graphics;

namespace ubi_health
{
    [Activity(Label = "Principal")]
    public class Principal : Activity
    {
        Android.App.ProgressDialog progress;
        string elementoimagen;
        ListView listado;
        List<Hospital> ListadodeHospital = new List<Hospital>();
        List<ElementosHospital> Elementos = new List<ElementosHospital>();
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.principal);
            listado = FindViewById<ListView>(Resource.Id.lista);
            progress = new Android.App.ProgressDialog(this);
            progress.Indeterminate = true;
            progress.SetProgressStyle(Android.App.ProgressDialogStyle.Spinner);
            progress.SetMessage("Cargando datos de Azure...");
            progress.SetCancelable(false);
            progress.Show();
            await CargarDatosAzure();
            progress.Hide();
        }
        public async Task CargarDatosAzure()
        {
            try
            {
                var CuentadeAlmacenamiento = CloudStorageAccount.Parse
                    ("DefaultEndpointsProtocol=https;AccountName=programacionparamovilesb;AccountKey=9PZM1I5tcsvetJbY6E6hYVU5dHbUxEhR10kt0MRVNsj42TeTKX6dlWQeSmgCmfKgZFmt+z+JVNApy7zMr3w93Q==;EndpointSuffix=core.windows.net");
                var ClienteBlob = CuentadeAlmacenamiento.CreateCloudBlobClient();
                //apunta a contenedor blob con imagenes
                var Contenedor = ClienteBlob.GetContainerReference("hospital");
                var TablaNoSQL = CuentadeAlmacenamiento.CreateCloudTableClient();
                //apunta a tabla no sql con datos
                var Tabla = TablaNoSQL.GetTableReference("Hospitales");

                var Consulta = new TableQuery<Hospital>(); //select*from pero en tabla no sql
                TableContinuationToken token = null;
                var Datos = await Tabla.ExecuteQuerySegmentedAsync<Hospital>
                    (Consulta, token, null, null); //lanza consulta y trae datos correspondientes. null 1 no hay accion especifica null 2 no hay contexto
                ListadodeHospital.AddRange(Datos.Results);
                //Contador para cada uno de los elementos dentro de la tabla
                
                int iEspecialidad = 0;
                int iImagen = 0;
                int iDireccion = 0;
                int iNombre = 0;
                int iLatitud = 0;
                int iLongitud = 0;

                //recibe lo que tiene la lista de cleintes y lo empata a la clase tabla. 1 en 1
                Elementos = ListadodeHospital.Select(r => new ElementosHospital()
                {
                    Nombre = ListadodeHospital.ElementAt(iNombre++).RowKey,
                    Imagen = ListadodeHospital.ElementAt(iImagen++).Imagen,
                    Direccion = ListadodeHospital.ElementAt(iDireccion++).Direccion,
                    Latitud = ListadodeHospital.ElementAt(iLatitud++).Latitud,
                    Longitud = ListadodeHospital.ElementAt(iLongitud++).Longitud,
                    Especialidad = ListadodeHospital.ElementAt(iEspecialidad++).Especialidad

                }).ToList();

                int contadorImagen = 0;
                while (contadorImagen < ListadodeHospital.Count)
                {
                    //ubica imagen e imagenfondo
                    elementoimagen = ListadodeHospital.ElementAt(contadorImagen).Imagen;
                    //indico que elemento voy a apuntar
                    var ImagenBlob = Contenedor.GetBlockBlobReference(elementoimagen);
                    //preparacion de carpeta
                    var rutaImagen = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                    //preparo archivo
                    var ArchivoImagen = System.IO.Path.Combine(rutaImagen, elementoimagen);
                    //preparo ejecucion de descarga de archivo
                    var StreamImagen = File.OpenWrite(ArchivoImagen);
                    //descarga de imagen
                    await ImagenBlob.DownloadToStreamAsync(StreamImagen);
                    contadorImagen++;
                }
                //Toast.MakeText(this, "Imagenes descargadas", ToastLength.Long).Show();
                listado.Adapter = new DataAdapter(this, Elementos);
                listado.ItemClick += onListItemClick;
            }
            catch (System.Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
        }

        public void onListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var DataSend = Elementos[e.Position];
            var DataIntent = new Intent(this, typeof(DataDetailActivity));
            DataIntent.PutExtra("latitud", DataSend.Latitud.ToString());
            DataIntent.PutExtra("longitud", DataSend.Longitud.ToString());
            StartActivity(DataIntent);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

    }


    //tabla para usar los datos dentro de la app
    public class ElementosHospital
    {
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Imagen { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public string Especialidad { get; set; }
        public double Saldo { get; set; }
    }
    //tabla entendida que se usa para la conexion a azure
    public class Hospital : TableEntity
    {
        public Hospital(string Hospital, string Nombre)
        {
            PartitionKey = Hospital;
            RowKey = Nombre;
        }

        public Hospital() { }
        public string Direccion { get; set; }
        public string Imagen { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public string Especialidad { get; set; }

    }
}