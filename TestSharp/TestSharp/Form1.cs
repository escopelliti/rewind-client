//using System;
//using System.Drawing;
//using System.Windows.Forms;
//using System.Net;
//using System.Net.Sockets;
//using System.Threading;
//using System.IO;
//using System.Runtime.Serialization.Formatters.Binary;
//using System.Runtime.Serialization;
//using TestSharp;
//using System.Windows;
//using CommunicationLibrary;
//using System.Collections.Generic;
//using System.Text;
//using System.Linq;
//using Protocol;
//using System.Runtime.InteropServices;

//namespace MouseEvent
//{
//    public class Form1 : System.Windows.Forms.Form
//    {
//        private System.Windows.Forms.Panel panel1;
//        private System.Windows.Forms.Label label1;
//        private System.Windows.Forms.Label label2;
//        private System.Windows.Forms.Label label3;
//        private System.Windows.Forms.Label label4;
//        private System.Windows.Forms.Label label5;
//        private System.Windows.Forms.Label label6;
//        private System.Windows.Forms.Label label7;
//        private System.Windows.Forms.Label label8;
//        private System.Windows.Forms.Label label9;
//        private System.Windows.Forms.Button clearButton;
//        private System.Drawing.Drawing2D.GraphicsPath mousePath;
//        private System.Windows.Forms.GroupBox groupBox1;

//        private int fontSize = 20;

//        private static Socket socket;
//        private static ClientCommunicationManager communicationManager;


//        [STAThread]
//        static void Main()
//        {

//            communicationManager = new ClientCommunicationManager();
//            socket = communicationManager.CreateSocket(ProtocolType.Tcp);
//            //IPHostEntry ipHostInfo = Dns.GetHostEntry("Luigi");
//            //IPAddress ipAddress = ipHostInfo.AddressList[0];
//            //IPEndPoint remoteEP = new IPEndPoint(ipAddress, 15000);
//            //client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//            //client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
//            //connectDone.WaitOne();
//            communicationManager.Connect(Dns.GetHostName(), 12000, socket);
//            Application.Run(new Form1());
//        }

//        //private static void ConnectCallback(IAsyncResult ar)
//        //{
//        //    try
//        //    {
//        //         //Retrieve the socket from the state object.
//        //        Socket client = (Socket)ar.AsyncState;

//        //         //Complete the connection.
//        //        client.EndConnect(ar);

//        //        Console.WriteLine("Socket connected to {0}",
//        //            client.RemoteEndPoint.ToString());

//        //         //Signal that the connection has been made.
//        //        connectDone.Set();
//        //    }
//        //    catch (Exception e)
//        //    {
//        //        Console.WriteLine(e.ToString());
//        //    }
//        //}


//        public Form1()
//        {
//            mousePath = new System.Drawing.Drawing2D.GraphicsPath();

//            this.panel1 = new System.Windows.Forms.Panel();
//            this.label1 = new System.Windows.Forms.Label();
//            this.clearButton = new System.Windows.Forms.Button();
//            this.label2 = new System.Windows.Forms.Label();
//            this.label3 = new System.Windows.Forms.Label();
//            this.label4 = new System.Windows.Forms.Label();
//            this.label5 = new System.Windows.Forms.Label();
//            this.label6 = new System.Windows.Forms.Label();
//            this.label7 = new System.Windows.Forms.Label();
//            this.label8 = new System.Windows.Forms.Label();
//            this.label9 = new System.Windows.Forms.Label();
//            this.groupBox1 = new System.Windows.Forms.GroupBox();

//            //Mouse Events Label
//            this.label1.Location = new System.Drawing.Point(24, 504);
//            this.label1.Size = new System.Drawing.Size(392, 23);
//            //DoubleClickSize Label
//            this.label2.AutoSize = true;
//            this.label2.Location = new System.Drawing.Point(24, 48);
//            this.label2.Size = new System.Drawing.Size(35, 13);
//            //DoubleClickTime Label
//            this.label3.AutoSize = true;
//            this.label3.Location = new System.Drawing.Point(24, 72);
//            this.label3.Size = new System.Drawing.Size(35, 13);
//            //MousePresent Label
//            this.label4.AutoSize = true;
//            this.label4.Location = new System.Drawing.Point(24, 96);
//            this.label4.Size = new System.Drawing.Size(35, 13);
//            //MouseButtons Label
//            this.label5.AutoSize = true;
//            this.label5.Location = new System.Drawing.Point(24, 120);
//            this.label5.Size = new System.Drawing.Size(35, 13);
//            //MouseButtonsSwapped Label
//            this.label6.AutoSize = true;
//            this.label6.Location = new System.Drawing.Point(320, 48);
//            this.label6.Size = new System.Drawing.Size(35, 13);
//            //MouseWheelPresent Label
//            this.label7.AutoSize = true;
//            this.label7.Location = new System.Drawing.Point(320, 72);
//            this.label7.Size = new System.Drawing.Size(35, 13);
//            //MouseWheelScrollLines Label
//            this.label8.AutoSize = true;
//            this.label8.Location = new System.Drawing.Point(320, 96);
//            this.label8.Size = new System.Drawing.Size(35, 13);
//            //NativeMouseWheelSupport Label
//            this.label9.AutoSize = true;
//            this.label9.Location = new System.Drawing.Point(320, 120);
//            this.label9.Size = new System.Drawing.Size(35, 13);

//            //Mouse Panel
//            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
//                | System.Windows.Forms.AnchorStyles.Right);
//            this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
//            this.panel1.Location = new System.Drawing.Point(16, 160);
//            this.panel1.Size = new System.Drawing.Size(664, 320);
//            this.panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseUp);
//            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
//            this.panel1.MouseEnter += new System.EventHandler(this.panel1_MouseEnter);
//            this.panel1.MouseHover += new System.EventHandler(this.panel1_MouseHover);
//            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);
//            this.panel1.MouseLeave += new System.EventHandler(this.panel1_MouseLeave);
//            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
//            this.panel1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseWheel);

//            //Clear Button
//            this.clearButton.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
//            this.clearButton.Location = new System.Drawing.Point(592, 504);
//            this.clearButton.TabIndex = 1;
//            this.clearButton.Text = "Clear";
//            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);

//            //GroupBox
//            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
//                | System.Windows.Forms.AnchorStyles.Right);
//            this.groupBox1.Location = new System.Drawing.Point(16, 24);
//            this.groupBox1.Size = new System.Drawing.Size(664, 128);
//            this.groupBox1.Text = "System.Windows.Forms.SystemInformation";

//            //Set up how the form should be displayed and add the controls to the form.
//            this.ClientSize = new System.Drawing.Size(696, 534);
//            this.Controls.AddRange(new System.Windows.Forms.Control[] {
//                                        this.label9,this.label8,this.label7,this.label6,
//                                        this.label5,this.label4,this.label3,this.label2,
//                                        this.clearButton,this.panel1,this.label1,this.groupBox1});
//            this.Text = "Mouse Event Example";

//            //Displays information about the system mouse.
//            label2.Text = "SystemInformation.DoubleClickSize: " + SystemInformation.DoubleClickSize.ToString();
//            label3.Text = "SystemInformation.DoubleClickTime: " + SystemInformation.DoubleClickTime.ToString();
//            label4.Text = "SystemInformation.MousePresent: " + SystemInformation.MousePresent.ToString();
//            label5.Text = "SystemInformation.MouseButtons: " + SystemInformation.MouseButtons.ToString();
//            label6.Text = "SystemInformation.MouseButtonsSwapped: " + SystemInformation.MouseButtonsSwapped.ToString();
//            label7.Text = "SystemInformation.MouseWheelPresent: " + SystemInformation.MouseWheelPresent.ToString();
//            label8.Text = "SystemInformation.MouseWheelScrollLines: " + SystemInformation.MouseWheelScrollLines.ToString();
//            label9.Text = "SystemInformation.NativeMouseWheelSupport: " + SystemInformation.NativeMouseWheelSupport.ToString();

//        }

//        private void panel1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
//        {
//            //Update the mouse path with the mouse information
//            //Point mouseDownLocation = new Point(e.X, e.Y);

//            string eventString = null;
//            switch (e.Button)
//            {
//                case MouseButtons.Left:
//                    eventString = "L";
//                    break;
//                case MouseButtons.Right:
//                    eventString = "R";
//                    break;
//                case MouseButtons.Middle:
//                    eventString = "M";
//                    break;
//                case MouseButtons.XButton1:
//                    eventString = "X1";
//                    break;
//                case MouseButtons.XButton2:
//                    eventString = "X2";
//                    break;
//                case MouseButtons.None:
//                default:
//                    break;
//            }

//            if (eventString != null)
//            {
//                //mousePath.AddString(eventString, FontFamily.GenericSerif, (int)FontStyle.Bold, fontSize, mouseDownLocation, StringFormat.GenericDefault);
//            }
//            else
//            {
//                //mousePath.AddLine(mouseDownLocation, mouseDownLocation);
//            }
//            panel1.Focus();
//            panel1.Invalidate();
//        }

//        private void panel1_MouseEnter(object sender, System.EventArgs e)
//        {
//            //Update the mouse event label to indicate the MouseEnter event occurred.
//            label1.Text = sender.GetType().ToString() + ": MouseEnter";
//        }

//        private void panel1_MouseHover(object sender, System.EventArgs e)
//        {
//            //Update the mouse event label to indicate the MouseHover event occurred.
//            label1.Text = sender.GetType().ToString() + ": MouseHover";
//        }

//        private void panel1_MouseLeave(object sender, System.EventArgs e)
//        {
//            //Update the mouse event label to indicate the MouseLeave event occurred.
//            label1.Text = sender.GetType().ToString() + ": MouseLeave";
//        }

//        private void panel1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
//        {
//            //Update the mouse path that is drawn onto the Panel.
//            int mouseX = e.X;
//            int mouseY = e.Y;

//            mousePath.AddLine(mouseX, mouseY, mouseX, mouseY);
//            Console.WriteLine("MOUSEX: " + mouseX + " MOUSEY: " + mouseY);

//            byte[] x = BitConverter.GetBytes(mouseX);
//            byte[] y = BitConverter.GetBytes(mouseY);
//            byte[] toSend = new byte[x.Length + y.Length];
//            System.Buffer.BlockCopy(x, 0, toSend, 0, x.Length);
//            System.Buffer.BlockCopy(y, 0, toSend, x.Length, y.Length);

//            if (BitConverter.IsLittleEndian)
//                Array.Reverse(toSend);
//            byte[] result = toSend;

//            //MouseCoordinates mouseCoord = new MouseCoordinates(mouseX, mouseY);
//            //MemoryStream ms = new MemoryStream();
//            //try
//            //{
//            //    BinaryFormatter binaryFormatter = new BinaryFormatter();
//            //    binaryFormatter.Serialize(ms, mouseCoord);
//            //}
//            //catch (SerializationException se)
//            //{
//            //    Console.WriteLine(se.Message);
//            //}

//            // Begin sending the data to the remote device.
//            //client.BeginSend(ms.ToArray(), 0, (int) ms.Length, 0, new AsyncCallback(SendCallback), client);
//            //client.BeginSend(toSend, 0, toSend.Length, 0, new AsyncCallback(SendCallback), client);
//            //communicationManager.Send(toSend);           
//        }

//        //private static void SendCallback(IAsyncResult ar)
//        //{
//        //    try
//        //    {
//        //         //Retrieve the socket from the state object.
//        //        Socket client = (Socket)ar.AsyncState;

//        //         //Complete sending the data to the remote device.
//        //        int bytesSent = client.EndSend(ar);
//        //        Console.WriteLine("Sent {0} bytes to server.", bytesSent);

//        //         //Signal that all bytes have been sent.             
//        //    }
//        //    catch (Exception e)
//        //    {
//        //        Console.WriteLine(e.ToString());
//        //    }
//        //}

//        private void panel1_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
//        {
//            //Update the drawing based upon the mouse wheel scrolling.

//            int numberOfTextLinesToMove = e.Delta * SystemInformation.MouseWheelScrollLines / 120;
//            int numberOfPixelsToMove = numberOfTextLinesToMove * fontSize;

//            if (numberOfPixelsToMove != 0)
//            {
//                System.Drawing.Drawing2D.Matrix translateMatrix = new System.Drawing.Drawing2D.Matrix();
//                translateMatrix.Translate(0, numberOfPixelsToMove);
//                mousePath.Transform(translateMatrix);
//            }
//            panel1.Invalidate();
//        }



//        private void panel1_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
//        {
//            //Point mouseUpLocation = new System.Drawing.Point(e.X, e.Y);

//            //Show the number of clicks in the path graphic.
//            int numberOfClicks = e.Clicks;
//            //mousePath.AddString("    " + numberOfClicks.ToString(),
//            //            FontFamily.GenericSerif, (int)FontStyle.Bold,
//            //            fontSize, mouseUpLocation, StringFormat.GenericDefault);

//            panel1.Invalidate();
//            byte[] byteToSend = null;
//            //List<ProtocolUtils.FileStruct> fileStructList = null;
//            string[] array = null;
//            if (System.Windows.Forms.Clipboard.ContainsText())
//            {
//                string clipboardText = System.Windows.Forms.Clipboard.GetText();
//                string toSend = JSONFactory.CreateStandardJSONRequest(ProtocolUtils.SET_CLIPBOARD_TEXT, clipboardText);
//                byteToSend = Encoding.Unicode.GetBytes(toSend);
//                //XDocument doc = XMLFactory.CreateXMLDocument(ProtocolUtils.SET_CLIPBOARD_TEXT, clipboardText);
//                //byteToSend = ProtocolUtils.convertXDocumentToByteArray(doc);
//            }
//            if (System.Windows.Forms.Clipboard.ContainsImage())
//            {
//                Image image = System.Windows.Forms.Clipboard.GetImage();
//                byte[] imageBytes;
//                using (var ms = new MemoryStream())
//                {
//                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
//                    imageBytes = ms.ToArray();
//                }
//                string toSend = JSONFactory.CreateStandardJSONRequest(ProtocolUtils.SET_CLIPBOARD_IMAGE, imageBytes.Length);
//                byteToSend = Encoding.Unicode.GetBytes(toSend);
//                byte[] arrayToSend = new byte[byteToSend.Length + 16];
//                byte[] token = Guid.NewGuid().ToByteArray();
//                System.Buffer.BlockCopy(token, 0, arrayToSend, 0, 16);
//                System.Buffer.BlockCopy(byteToSend, 0, arrayToSend, 16, byteToSend.Length);
//                communicationManager.Send(arrayToSend, socket);

//                communicationManager.Receive(new byte[1024], socket);

//                byte[] chunk = new byte[1024];
//                System.Buffer.BlockCopy(token, 0, chunk, 0, 16);
//                int bytesToRead = 0;
//                int fixedDataSize = 1024 - 16;
//                while (bytesToRead + fixedDataSize < imageBytes.Length)
//                {
//                    System.Buffer.BlockCopy(imageBytes, bytesToRead, chunk, 16, fixedDataSize);
//                    bytesToRead += (fixedDataSize);
//                    communicationManager.Send(chunk, socket);
//                    communicationManager.Receive(new byte[1024], socket);
//                }
//                int rest = imageBytes.Length - bytesToRead;
//                if (rest > 0)
//                {
//                    System.Buffer.BlockCopy(imageBytes, (imageBytes.Length - rest), chunk, 16, rest);
//                    communicationManager.Send(chunk, socket);
//                    communicationManager.Receive(new byte[1024], socket);
//                }
//                //NON INVIARE TUTTO IMAGEBYTES IN UNA VOLTA MA A PEZZI COME NEI FILE

//                //arrayToSend = new byte[byteToSend.Length + 16];
//                //System.Buffer.BlockCopy(token, 0, arrayToSend, 0, 16);
//                //System.Buffer.BlockCopy(byteToSend, 0, arrayToSend, 16, byteToSend.Length);

//                //communicationManager.Send(arrayToSend, socket);
//                //communicationManager.Send(byteToSend, socket);
//                byteToSend = null;
//            }
//            if (System.Windows.Forms.Clipboard.ContainsFileDropList())
//            {
//                System.Collections.Specialized.StringCollection test = System.Windows.Forms.Clipboard.GetFileDropList();
//                array = new string[test.Count];
//                test.CopyTo(array, 0);
//                string toSend = JSONFactory.CreateFileTransferJSONRequest(ProtocolUtils.SET_CLIPBOARD_FILES, array);
//                byteToSend = Encoding.Unicode.GetBytes(toSend);
//                Console.WriteLine(toSend);
//                //XDocument doc = XMLFactory.CreateXMLDocument(ProtocolUtils.SET_CLIPBOARD_FILES, fileStructList);
//                //byteToSend = ProtocolUtils.convertXDocumentToByteArray(doc);



//            }
//            if (!(byteToSend == null))
//            {
//                byte[] toSend = new byte[byteToSend.Length + 16];
//                System.Buffer.BlockCopy(Guid.NewGuid().ToByteArray(), 0, toSend, 0, 16);
//                System.Buffer.BlockCopy(byteToSend, 0, toSend, 16, byteToSend.Length);
//                communicationManager.Send(toSend, socket);

//                byte[] token = new byte[16];
//                System.Buffer.BlockCopy(toSend, 0, token, 0, 16);

//                byte[] bytesFileToSend = new byte[1024];
//                System.Buffer.BlockCopy(token, 0, bytesFileToSend, 0, 16);
//                foreach (string file in array)
//                {
//                    if (File.Exists(file))
//                    {
//                        byte[] bytesFile = new byte[1024 - 16];
//                        using (var stream = new FileStream(file, FileMode.Open))
//                        {
//                            int bytesRead;
//                            while ((bytesRead = stream.Read(bytesFile, 0, bytesFile.Length)) > 0)
//                            {
//                                System.Buffer.BlockCopy(bytesFile, 0, bytesFileToSend, 16, bytesFile.Length);
//                                byte[] final = new byte[bytesRead + 16];
//                                System.Buffer.BlockCopy(bytesFileToSend, 0, final, 0, bytesRead + 16);
//                                communicationManager.Send(final, socket);
//                                byte[] received = new byte[16];
//                                communicationManager.Receive(received, socket);
//                                if (!(received.SequenceEqual(token)))
//                                {
//                                    break;
//                                }
//                            }

//                        }
//                    }
//                }
//                //}

//                //byte[] toRead = new byte[512];
//                //int bytesReadNum = communicationManager.Receive(toRead, socket);
//                //byte[] actualData = new byte[bytesReadNum];
//                //System.Buffer.BlockCopy(toRead, 0, actualData, 0, bytesReadNum);
//                //MemoryStream ms = new MemoryStream(actualData);
//                //XDocument request = XDocument.Load(ms);
//                //string type = request.Descendants(ProtocolUtils.TYPE).ElementAt(0).Value;
//                //if (type == ProtocolUtils.TRANSFER_FILES)
//                //{
//                //foreach (string file in array)
//                //{
//                //    communicationManager.SendFiles(file, socket);
//                //    bytesReadNum = communicationManager.Receive(toRead, socket);
//                //    actualData = new byte[bytesReadNum];
//                //    System.Buffer.BlockCopy(toRead, 0, actualData, 0, bytesReadNum);
//                //    ms = new MemoryStream(actualData);
//                //    request = XDocument.Load(ms);
//                //    type = request.Descendants(ProtocolUtils.TYPE).ElementAt(0).Value;
//                //    if (!(type == ProtocolUtils.TRANSFER_FILES)) break;
//                //}

//                //}

//                //}
//                //XML FACTORY FILE CON BYTE
//                //byte[] allBytes = File.ReadAllBytes(file.name);
//                //int copiedBytes = 1024;
//                //while (copiedBytes < allBytes.Length)
//                //{
//                //    XElement chunk = new XElement(ProtocolUtils.CHUNK);
//                //    chunk.SetAttributeValue(ProtocolUtils.SIZE, 1024);
//                //    byte[] toCopy = new byte[1024];
//                //    System.Buffer.BlockCopy(allBytes, copiedBytes - 1024, toCopy, 0, 1024);
//                //    chunk.Value = Convert.ToBase64String(toCopy);
//                //    copiedBytes += 1024;
//                //    fileElement.Add(chunk);
//                //}
//                //copiedBytes -= 1024;
//                //int remainingBytes = allBytes.Length - copiedBytes;
//                //if (remainingBytes > 0)
//                //{
//                //    XElement chunk = new XElement(ProtocolUtils.CHUNK);
//                //    chunk.SetAttributeValue(ProtocolUtils.SIZE, remainingBytes);
//                //    byte[] toCopy = new byte[remainingBytes];
//                //    System.Buffer.BlockCopy(allBytes, copiedBytes, toCopy, 0, remainingBytes);
//                //    chunk.Value = Convert.ToBase64String(toCopy);
//                //    fileElement.Add(chunk);
//                //}



//                //RICEZIONE XML CON BYTE
//                //List<string> fileList = new List<string>();
//                //fileList.Add("C:\\Users\\erics_000\\Desktop\\Link Utili PDS.txt");
//                //communicationManager.SendFiles(fileList);

//                //IEnumerable<XElement> fileElement = ((XDocument)param).Descendants(ProtocolUtils.FILE);
//                //foreach (XElement element in fileElement)
//                //{

//                //    String size = element.Descendants(ProtocolUtils.SIZE).ElementAt(0).Value;
//                //    byte[] bytesFile = new byte[Convert.ToInt64(size)];
//                //    IEnumerable<XElement> chunkElements = element.Descendants(ProtocolUtils.CHUNK);
//                //    int copiedBytes = 0;
//                //    foreach (XElement chunk in chunkElements)
//                //    {
//                //        byte[] chunkBytes = new byte[Convert.ToInt64(chunk.Attribute(ProtocolUtils.SIZE).Value)];
//                //        chunkBytes = Convert.FromBase64String(chunk.Value);
//                //        System.Buffer.BlockCopy(chunkBytes, 0, bytesFile, copiedBytes, chunkBytes.Length);
//                //        copiedBytes += chunkBytes.Length;
//                //    }
//                //    File.WriteAllBytes("C:\\Users\\erics_000\\Documents\\" + element.Attribute(ProtocolUtils.NAME).Value, bytesFile);
//                //}
//            }
//        }

//        private void panel1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
//        {
//            //Perform the painting of the Panel.
//            e.Graphics.DrawPath(System.Drawing.Pens.DarkRed, mousePath);
//        }

//        private void clearButton_Click(object sender, System.EventArgs e)
//        {
//            //Clear the Panel display.
//            mousePath.Dispose();
//            mousePath = new System.Drawing.Drawing2D.GraphicsPath();
//            panel1.Invalidate();
//        }
//    }
//}

