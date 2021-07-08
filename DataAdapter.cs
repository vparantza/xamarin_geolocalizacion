using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Graphics.Drawable;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ubi_health
{
    public class DataAdapter : BaseAdapter<ElementosHospital>
    {
        List<ElementosHospital> items;
        Activity context;

        //constructor base referencia al interior de la clase
        public DataAdapter(Activity context, List<ElementosHospital> items) : base()
        {
            this.context = context;
            this.items = items;
        }

        //ubicacion de donde el usuario pudiera hacer una seleccion
        public override long GetItemId(int position)
        {
            return position;
        }

        public override ElementosHospital this[int position]
        {
            get { return items[position]; }
        }

        public override int Count
        {
            get { return items.Count; }
        }
        //SI REGRESAS AHORITA VUELVO
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];
            View view = convertView;
            view = context.LayoutInflater.Inflate(Resource.Layout.DataRow, null);
            view.FindViewById<TextView>(Resource.Id.txtnombre).Text = item.Nombre;
            view.FindViewById<TextView>(Resource.Id.txtespecialidad).Text = item.Especialidad;
            view.FindViewById<TextView>(Resource.Id.txtdireccion).Text = item.Direccion;
            var path = System.IO.Path.Combine(System.Environment.GetFolderPath
                (System.Environment.SpecialFolder.Personal), item.Imagen);
            var ImagenHospital = BitmapFactory.DecodeFile(path);
            ImagenHospital = ResizeBitmap(ImagenHospital, 100, 100);
            view.FindViewById<ImageView>(Resource.Id.imagenhospital).SetImageDrawable(getRoundedCornerImagen(ImagenHospital, 5));
            return view;
        }

        public static RoundedBitmapDrawable getRoundedCornerImagen(Bitmap image, int cornerRadius)
        {
            var corner = RoundedBitmapDrawableFactory.Create(null, image);
            corner.CornerRadius = cornerRadius;
            return corner;
        }

        private Bitmap ResizeBitmap(Bitmap imagenoriginal, int widthimagenoriginal, int heightimagenoriginal)
        {
            Bitmap resizedImage = Bitmap.CreateBitmap(widthimagenoriginal, heightimagenoriginal, Bitmap.Config.Argb8888);
            float Width = imagenoriginal.Width;
            float Height = imagenoriginal.Height;
            var canvas = new Canvas(resizedImage);
            var scala = widthimagenoriginal / Width;
            var xTranslation = 0.0f;
            var yTranslation = (heightimagenoriginal - Height * scala) / 2.0f;
            var transformacion = new Matrix();
            transformacion.PostTranslate(xTranslation, yTranslation);
            transformacion.PreScale(scala, scala);
            var paint = new Paint();
            paint.FilterBitmap = true;
            canvas.DrawBitmap(imagenoriginal, transformacion, paint);
            return resizedImage;
        }


    }

}