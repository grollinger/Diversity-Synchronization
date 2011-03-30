using System.Windows;
using System.Windows.Controls;
using System.Security.Permissions;
using System.IO;
using System;
using MVVMDiversity.ViewModel;
using MVVMDiversity.Interface;
using MVVMDiversity.Model;
using System.Windows.Input;

namespace MVVMDiversity.View.Pages
{
    /// <summary>
    /// Description for MapView.
    /// </summary>
    public partial class MapView : UserControl , IMapView
    {
        MapViewModel VM { get { return DataContext as MapViewModel; } }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        [System.Runtime.InteropServices.ComVisibleAttribute(true)]
        public class MapsCOMInterface
        {
            MapView owner;
            public MapsCOMInterface(MapView owner)
            {
                this.owner = owner;
            }
        }


        MapsCOMInterface _comInterface;
        /// <summary>
        /// Initializes a new instance of the MapView class.
        /// </summary>
        public MapView()
        {
            InitializeComponent();            

            

            Uri uri = new Uri(@"pack://application:,,,/Maps/GoogleMap.html");
            Stream source = Application.GetContentStream(uri).Stream;
            webbrowserMap.NavigateToStream(source);
            
            webbrowserMap.ObjectForScripting = new MapsCOMInterface(this);
            
        }



        #region Save Map

        private void showOverlay()
        {
            
            Object[] obj = new Object[2];
           // obj[0] = (Object)DeviceWidth;
           // obj[1] = (Object)DeviceHeight;

            webbrowserMap.InvokeScript("showOverlay", obj);
            
        }





        // Validate all dependency objects in a window
        bool IsValid(DependencyObject node)
        {
            // Check if dependency object was passed
            if (node != null)
            {
                // Check if dependency object is valid.
                // NOTE: Validation.GetHasError works for controls that have validation rules attached 
                bool isValid = !Validation.GetHasError(node);
                if (!isValid)
                {
                    // If the dependency object is invalid, and it can receive the focus,
                    // set the focus
                    if (node is IInputElement) Keyboard.Focus((IInputElement)node);
                    return false;
                }
            }

            // If this dependency object is valid, check all child dependency objects
            foreach (object subnode in LogicalTreeHelper.GetChildren(node))
            {
                if (subnode is DependencyObject)
                {
                    // If a child dependency object is invalid, return false immediately,
                    // otherwise keep checking
                    if (IsValid((DependencyObject)subnode) == false) return false;
                }
            }

            // All dependency objects are valid
            return true;
        }       

        private void saveMapToLocalMachine()
        {
            /*if (imageMap.Source == null)
            {
                throw new Exception("No map loaded to be saved.");
            }

            StringBuilder builder = new StringBuilder();
            builder.Append(Environment.CurrentDirectory + @"\Maps");
            builder.Append(@"\");
            builder.Append(this.textBoxFile.Text.Trim());
            builder.Append(".png");

            String localFileName = builder.ToString();
            if (!Directory.Exists(Environment.CurrentDirectory + @"\Maps"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + @"\Maps");
            }

            ImageOptions iOpt = new ImageOptions();
            iOpt.Name = this.textBoxFile.Text.Trim();
            iOpt.Description = this.textBoxFileDescription.Text.Trim();

            if (OptionsAccess.MapSaveOptions.UseDeviceDimensions)
            {
                //Cut and Save Image
                Bitmap img = new Bitmap(deviceWidth, deviceHeight);
                Graphics g = Graphics.FromImage(img);
                g.DrawImageUnscaledAndClipped(pictureBoxMap.Image, new Rectangle((-1) * overlayX, (-1) * overlayY, deviceWidth + overlayX, deviceHeight + overlayY));
                g.Save();
                img.Save(localFileName, ImageFormat.Png);

                try
                {
                    CultureInfo info = null;
                    if (textBoxNELong.Text.Contains(","))
                        info = System.Globalization.CultureInfo.CreateSpecificCulture("de-de");
                    else if (textBoxNELong.Text.Contains("."))
                        info = System.Globalization.CultureInfo.CreateSpecificCulture("en-us");

                     //for now: maps are assumed northbound
                     //otherwise quotients must be differently calculated
                    float deltaLongitude = float.Parse(textBoxNELong.Text.Trim(), info) - float.Parse(textBoxSWLong.Text.Trim(), info);
                    float ratioLongitude = deltaLongitude / pictureBoxMap.Image.Width;

                    float deltaLatitude = float.Parse(textBoxNELat.Text.Trim(), info) - float.Parse(textBoxSWLat.Text.Trim(), info);
                    float ratioLatitude = deltaLatitude / pictureBoxMap.Image.Height;

                    iOpt.NELong = float.Parse(textBoxNELong.Text.Trim(), info) - ((pictureBoxMap.Image.Width - overlayX - deviceWidth) * ratioLongitude);
                    iOpt.NELat = float.Parse(textBoxNELat.Text.Trim(), info) - (overlayY * ratioLatitude);

                    iOpt.SWLong = float.Parse(textBoxSWLong.Text.Trim(), info) + (overlayX * ratioLongitude);
                    iOpt.SWLat = float.Parse(textBoxSWLat.Text.Trim(), info) + ((pictureBoxMap.Image.Height - overlayY - deviceHeight) * ratioLatitude);

                    if (checkBoxNotNorthbound.Checked)
                    {
                        //ToDo: calculate SE Corner
                    }
                    else
                    {
                    iOpt.SELong = iOpt.NELong;
                    iOpt.SELat = iOpt.SWLat;
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("GPS Data couldn`t be calculated. XML File couldn't be saved!");
                    throw e;
                }
            }
            else
            {
                imageMap..Save(localFileName, ImageFormat.Png);

                iOpt.NELong = float.Parse(textBoxNELong.Text.Trim());
                iOpt.NELat = float.Parse(textBoxNELat.Text.Trim());

                iOpt.SWLong = float.Parse(textBoxSWLong.Text.Trim());
                iOpt.SWLat = float.Parse(textBoxSWLat.Text.Trim());

                if (checkBoxNotNorthbound.Checked)
                {
                    iOpt.SELong = float.Parse(textBoxSELong.Text.Trim());
                    iOpt.SELat = float.Parse(textBoxSELat.Text.Trim());
                }
                else
                {
                    iOpt.SELong = iOpt.NELong;
                    iOpt.SELat = iOpt.SWLat;
                }
            }

            builder = new StringBuilder();
            builder.Append(Environment.CurrentDirectory + @"\Maps");
            builder.Append(@"\");
            builder.Append(this.textBoxFile.Text.Trim());
            builder.Append(".xml");

            this.writeSettingsToXML(builder.ToString(), iOpt);

            this.textBoxFileDescription.Clear();
            this.textBoxFile.Clear();
            */
        }

        

        #endregion




        #region Helper Methods        

        private void getCurrentPosition()
        {
            //Current centered Latitude and Longitude for GPS WGS84 
            if (webbrowserMap.Document != null)
            {
                Object temp;
                temp = webbrowserMap.InvokeScript("getLatitude");
                this.textBoxLatitude.Text = temp.ToString();


                temp = webbrowserMap.InvokeScript("getLongitude");
                this.textBoxLongitude.Text = temp.ToString();

            }
        }

        

        //private Bitmap zoomImage(Bitmap image)
        //{
        //    if (image != null)
        //    {
        //        int imgWidth = image.Size.Width;
        //        int imgHeight = image.Size.Height;

        //        double imgRate = imgWidth / imgHeight;

        //        if (imgWidth > this.pictureBoxMap.Width || imgHeight > this.pictureBoxMap.Height)
        //        {
        //            if (imgWidth > imgHeight)
        //            {
        //                imgWidth = this.pictureBoxMap.Width;
        //                imgHeight = (int)(imgWidth / imgRate);
        //            }
        //            else
        //            {
        //                imgHeight = this.pictureBoxMap.Height;
        //                imgWidth = (int)(imgHeight * imgRate);
        //            }
        //        }
        //        return new Bitmap(image, new Size(imgWidth, imgHeight));
        //    }
        //    return null;
        //}


        
           

        #endregion

        public Model.MapInfo getMapInfo()
        {
            MapInfo result = null;
            if (webbrowserMap.Document != null)
            {
                result = new MapInfo();
                Object temp;
                temp = webbrowserMap.InvokeScript("getSWLatitude");
                result.SWLat = float.Parse(temp.ToString());
                result.SELat = float.Parse(temp.ToString());

                temp = webbrowserMap.InvokeScript("getSWLongitude");
                result.SWLong = float.Parse(temp.ToString());

                temp = webbrowserMap.InvokeScript("getNELatitude");
                result.NELat = float.Parse(temp.ToString());

                temp = webbrowserMap.InvokeScript("getNELongitude");
                result.NELong = float.Parse(temp.ToString());
                result.SELong = float.Parse(temp.ToString());

                temp = webbrowserMap.InvokeScript("getZoomLevel");
                result.ZoomLevel = int.Parse(temp.ToString());
            }
            return result;
        }

        public string getMapURL(int width, int height)
        {
            Object[] obj = new Object[2];
            obj[0] = (Object)width;
            obj[1] = (Object)height;

            return webbrowserMap.InvokeScript("loadImageWithParam", obj).ToString();
        }


        public string getMapURL()
        {
            throw new NotImplementedException();
        }
    }
}