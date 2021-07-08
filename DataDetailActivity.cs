using Android;
using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using AndroidX.Core.Graphics.Drawable;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System;
using Xamarin.Essentials;

namespace ubi_health
{
    [Activity(Label = "DataDetailActivity")]
    public class DataDetailActivity : Activity, IOnMapReadyCallback
    {
        GoogleMap googleMap;
        double lat, lon, latorigen, lonorigen;
        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.DataDetail);

            var ImagenI = FindViewById<ImageView>(Resource.Id.imagenicono);
            ImagenI.SetImageResource(Resource.Drawable.imagenicono);
            var btnAgendar = FindViewById<Button>(Resource.Id.btnagendar);
            var txtDistancia = FindViewById<TextView>(Resource.Id.txtdistancia);

            ImagenI.Click += async delegate
            {
                try {
                    var location = await Geolocation.GetLastKnownLocationAsync();
                    if (location == null)
                    {
                        location = await Geolocation.GetLocationAsync(new GeolocationRequest
                        {
                            DesiredAccuracy = GeolocationAccuracy.Low,
                            Timeout = TimeSpan.FromSeconds(30)
                        });
                    }
                    latorigen = location.Latitude;
                    lonorigen = location.Longitude;

                    Location puntoa = new Location(latorigen, lonorigen);
                    Location puntob = new Location(double.Parse(Intent.GetStringExtra("latitud")), double.Parse(Intent.GetStringExtra("longitud")));
                    double km = Location.CalculateDistance(puntoa, puntob, DistanceUnits.Kilometers);
                    txtDistancia.Text = (Math.Round(km,2)).ToString() + " km";

                    //Toast.MakeText(this, "Se ha obtenido la ubicacion actual", ToastLength.Long).Show();

                }
                catch (System.Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                }

            };

            try
            {
                lat = double.Parse(Intent.GetStringExtra("latitud"));
                lon = double.Parse(Intent.GetStringExtra("longitud"));

                var mapview = FindViewById<MapView>(Resource.Id.mapView1);
                mapview.OnCreate(savedInstanceState);
                mapview.GetMapAsync(this);
                MapsInitializer.Initialize(this);

            }
            catch (System.Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }

            btnAgendar.Click += delegate
            {
                var VistaAgenda = new Intent(this, typeof(Agenda));
                VistaAgenda.PutExtra("Latitud", latorigen);
                VistaAgenda.PutExtra("Longitud", lonorigen);
                StartActivity(VistaAgenda);
            };
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            this.googleMap = googleMap;
            var builder = CameraPosition.InvokeBuilder();
            builder.Target(new LatLng(lat, lon));
            builder.Zoom(17);
            var cameraPosition = builder.Build();
            var cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
            this.googleMap.AnimateCamera(cameraUpdate);
        }
        public static RoundedBitmapDrawable getRoundedCornerImage(Bitmap image, int cornerRadius)
        {
            var corner = RoundedBitmapDrawableFactory.Create(null, image);
            corner.CornerRadius = cornerRadius;
            return corner;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}