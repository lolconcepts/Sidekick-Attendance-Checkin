using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using ZXing;

namespace Sidekick_Attendance_Checkin
{
    public partial class Form1 : Form
    {
        FilterInfoCollection filterInfoCollection;
        VideoCaptureDevice captureDevice;
        WebClient web = new WebClient();

        public Form1()
        {
            InitializeComponent();
            
            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo filterInfo in filterInfoCollection)
                cmbCam.Items.Add(filterInfo.Name);
            cmbCam.SelectedIndex = 0;
            lblMessage.Text = "Please Scan Card To Checkin..";

        }
        

        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            captureDevice = new VideoCaptureDevice(filterInfoCollection[cmbCam.SelectedIndex].MonikerString);
            captureDevice.NewFrame += CaptureDevice_NewFrame;
            captureDevice.Start();
            timer1.Start();

           
           

        }
        private void CaptureDevice_NewFrame(object sender, NewFrameEventArgs e)
        {
            pictureBox2.Image = (Bitmap)e.Frame.Clone();
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(captureDevice.IsRunning)
            {
                captureDevice.Stop();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblMessage.Text = "Please Scan Card To Checkin..";
            if (pictureBox2.Image != null)
            {
                BarcodeReader barcodeReader = new BarcodeReader();
                Result result = barcodeReader.Decode((Bitmap)pictureBox2.Image);
                if(result != null)
                {
                    txtRaw.Text = result.ToString(); // show the decoded QR to the Textbox

                    System.IO.Stream stream = web.OpenRead(result.ToString()); // Should go to the URL and check in the student, returning the page data
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                    {
                        String text = reader.ReadToEnd();
                        lblMessage.Text = text;
                    }

                    //System.Diagnostics.Process.Start(result.ToString());
                    //timer1.Stop();
                    //if(captureDevice.IsRunning)
                    //{
                    //    captureDevice.Stop();
                    //}
                }
            }
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {

        }
    }
}
