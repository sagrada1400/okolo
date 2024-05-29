using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.WindowsForms;
using System.Data.SqlClient;
//using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace okolo
{
    

    public partial class Form1 : Form
    {
        DataBase dataBase = new DataBase();

        int selected_id;

        GMap.NET.WindowsForms.GMapControl gmap;
        public Form1()
        {
            InitializeComponent();
            gmap = new GMap.NET.WindowsForms.GMapControl();
            gmap.MapProvider = GMap.NET.MapProviders.GMapProviders.OpenStreetMap;
            gmap.Dock = DockStyle.Fill;
            //gmap.MapProvider = GMap.NET.MapProviders.BingMapProvider.Instance;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache;
            gmap.ShowCenter = false;
            gmap.MinZoom = 15; 
            gmap.MaxZoom = 20;
            panel1.Controls.Add(gmap);

           

            gmap.ShowTileGridLines = false;

            

            gmap.Position = new PointLatLng(52.7576684669826,32.2482204437256);
            

            gmap.OnMarkerClick += Gmap_OnMarkerClick;

            gmap.OnMapDoubleClick += Gmap_OnMapDoubleClick;
            gmap.DragButton = MouseButtons.Left;
            gmap.OnMapClick += Gmap_OnMapClick;

            var cmd = $"select * from markers";
            dataBase.openConnection();
            SqlCommand command = new SqlCommand(cmd, dataBase.getConnection());
            SqlDataReader reader = command.ExecuteReader();

            

            while (reader.Read())
            {
                markers.List.Add(new markers(reader.GetInt32(0),reader.GetDouble(1),reader.GetDouble(2), reader.GetString(3)));
            }

            dataBase.closeConnection();

            foreach (var marker in markers.List)
            {
                var markerOverlay = new GMapOverlay($"{marker.id_marker}");

                var mar = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(new PointLatLng(marker.lat,marker.lng),
                    GMap.NET.WindowsForms.Markers.GMarkerGoogleType.red_small);

                markerOverlay.Markers.Add(mar);
                gmap.Overlays.Add(markerOverlay);

            }
            gmap.Zoom = 5;

        }

        private void Gmap_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            var g = markers.List.FirstOrDefault(x => x.id_marker.ToString() == item.Overlay.Id);

            if (g == null)
            {
                return;
            }

            textBox3.Text = ($"{g.name}");


            selected_id = Convert.ToInt32(item.Overlay.Id);
            textBox4.Text = item.Position.Lat.ToString();
            textBox1.Text = item.Position.Lat.ToString();
            textBox5.Text = item.Position.Lng.ToString();
            textBox2.Text = item.Position.Lng.ToString();


        }

        private void Gmap_OnMapDoubleClick(PointLatLng pointClick, MouseEventArgs e)
        {
            //gmap.Update();
            //gmap.Refresh();
            //var markerOverlay = new GMapOverlay("marker1");

            //var marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(new PointLatLng(Convert.ToDouble(textBox1.Text), Convert.ToDouble(textBox2.Text)),
            //    GMap.NET.WindowsForms.Markers.GMarkerGoogleType.red_small);

            //markerOverlay.Markers.Add(marker);
            //gmap.Overlays.Add(markerOverlay);
            
        }

        private void Gmap_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            

        }

        private void Gmap_OnMapClick(PointLatLng pointClick, MouseEventArgs e)
        {

                textBox3.Clear();
                textBox4.Clear();
                textBox5.Clear();

                var a = pointClick.Lat.ToString();
                var b = pointClick.Lng.ToString();
                textBox1.Text = a;
                textBox2.Text = b;
            
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
         
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {

            
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            markers newmarker = new markers(markers.List.Count+1, Convert.ToDouble(textBox1.Text), Convert.ToDouble(textBox2.Text), (string.IsNullOrWhiteSpace(textBox3.Text) ? "NewMarker" : textBox3.Text));

            var markerOverlay = new GMapOverlay($"{newmarker.id_marker}");

            var marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(new PointLatLng(newmarker.lat, newmarker.lng),
                GMap.NET.WindowsForms.Markers.GMarkerGoogleType.red_small);

            markers.List.Add(newmarker);

            var querystring = $"insert into markers(id_marker, lat, lng, name) " +
                $"values ('{newmarker.id_marker}','{(newmarker.lat).ToString().Replace(',','.')}','{newmarker.lng.ToString().Replace(',','.')}','{newmarker.name}')";
            dataBase.openConnection();
            SqlCommand sqlCommand = new SqlCommand(querystring, dataBase.getConnection());
            sqlCommand.ExecuteNonQuery();

            selected_id = newmarker.id_marker;

            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();

            markerOverlay.Markers.Add(marker);
            gmap.Overlays.Add(markerOverlay);

            gmap.Zoom += 0.000001;
            gmap.Zoom -= 0.000001;

            MessageBox.Show("Успешно!");

            dataBase.closeConnection();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var marker = markers.List.FirstOrDefault(x => x.id_marker == selected_id);
            if (marker == null)
            {
                MessageBox.Show("Не выбран маркер!");
                return;
            }

            var querysting = $"delete from markers where id_marker = '{marker.id_marker}'";
            dataBase.openConnection();
            SqlCommand sql = new SqlCommand(querysting, dataBase.getConnection());
            sql.ExecuteNonQuery();

            dataBase.closeConnection();

            var zxc = gmap.Overlays.FirstOrDefault(x => x.Id == selected_id.ToString());
            if (zxc != null)
            {
                gmap.Overlays.Remove(zxc);
            }

            markers.List.Remove(marker);
            MessageBox.Show("Успешно!");

            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();

            Refresh();

            selected_id = -1;


        }

        private void button3_Click(object sender, EventArgs e)
        {
            var marker = markers.List.FirstOrDefault(x => x.id_marker == selected_id);

            if (marker == null) 
            {
                MessageBox.Show("Не выбран маркер!");
                return;
            }

            marker.name = textBox3.Text;

            var querystring = $"update markers set name = '{marker.name}' where id_marker = '{marker.id_marker}'";
            dataBase.openConnection();
            SqlCommand sql = new SqlCommand(querystring, dataBase.getConnection());
            sql.ExecuteNonQuery();

            

            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();

            MessageBox.Show("Успешно!");
            dataBase.closeConnection();

            Refresh();
            selected_id = -1;
        }
    }
}
