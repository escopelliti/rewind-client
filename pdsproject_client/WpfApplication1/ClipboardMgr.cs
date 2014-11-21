using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using Protocol;
using CommunicationLibrary;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Specialized;

namespace WpfApplication1
{
    public class ClipboardMgr
    {
        public byte[] Data { get; set; }
        public Stream audio { get; set; }
        public List<ProtocolUtils.FileStruct> filesToReceive { get; set; }
        public String Text { get; set; }
        public JObject receivedJson { get; set; }

        public ChannelManager ChannelMgr { get; set; }
        
        private String currentContent;

        public ClipboardMgr()
        {            
            currentContent = String.Empty;
            filesToReceive = new List<ProtocolUtils.FileStruct>();
        }

        //private void ReceiveDataForClipboard(Object source, Object param)
        //{
        //    RequestState requestState = (RequestState)param;
        //    string filename = ProtocolUtils.protocolDictionary[requestState.type];
        //    using (var stream = new FileStream(filename, FileMode.Append))
        //    {
        //        stream.Write(requestState.data, 0, requestState.data.Length);
        //        stream.Close();
        //        //chi chiama questa funziona chiamera questa send
        //        //server.Send(Convert.FromBase64String(requestState.token), requestState.client.GetSocket());
        //    }
        //    if (new FileInfo(filename).Length >= Convert.ToInt64(requestState.stdRequest[ProtocolUtils.CONTENT].ToString()))
        //    {

        //        //CHI CHIAMA QUESTO METODO DEVE FARE QUESTA COSA
        //        //RequestState value = new RequestState();
        //        //if (!requestDictionary.TryRemove(requestState.token, out value))
        //        //{//custom exception would be better than this
        //        //    throw new Exception("Request not present in the dictionary");
        //        //}

        //        //TODO : AVOID CONDITIONAL TEST
        //        if (requestState.type == ProtocolUtils.TRANSFER_IMAGE)
        //        {
        //            CreateImageForClipboard(filename);
        //        }
        //    }
        //}

        //private void CreateImageForClipboard(string filename)
        //{
        //    Image image = null;
        //    using (var ms = new MemoryStream(File.ReadAllBytes(filename)))
        //    {
        //        image = Image.FromStream(ms);
        //    }
        //    File.Delete(filename);
        //    this.image = image;
        //    //MainForm.mainForm.Invoke(MainForm.clipboardImageDelegate, image);
        //}

        private void MoveByteToFiles()
        {
            int currentFileNum = 0;
            this.ChannelMgr.SendRequest(ProtocolUtils.GET_CLIPBOARD_FILES, String.Empty);
            long dim = 0;
            ProtocolUtils.FileStruct currentFile = filesToReceive.ElementAt(currentFileNum);
            //stream = new FileStream(ProtocolUtils.TMP_DIR + currentFile.dir + currentFile.name, FileMode.Append);
            while (currentFileNum < filesToReceive.Count)
            {

                byte[] bufferData = this.ChannelMgr.ReceiveData();
                using (var stream = new FileStream(ProtocolUtils.TMP_DIR + currentFile.dir + currentFile.name, FileMode.Append))
                {
                    stream.Write(bufferData, 0, bufferData.Length);
                    dim += bufferData.Length;
                    //ACK
                    this.ChannelMgr.SendRequest(ProtocolUtils.GET_CLIPBOARD_FILES, String.Empty);

                    if (dim == currentFile.size)
                    {
                        currentFileNum++;
                        dim = 0;
                        stream.Close();
                        if (currentFileNum == filesToReceive.Count)
                        {
                            break;
                        }
                        currentFile = filesToReceive.ElementAt(currentFileNum);
                        

                    }

                }    
            }
            this.ChannelMgr.ReceiveAck();
        }


        private static void DeleteFileDirContent(string toRemove)
        {

            foreach (string dir in Directory.GetDirectories(toRemove))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(dir);
                dirInfo.Delete(true);
            }
            foreach (string file in Directory.GetFiles(toRemove))
            {
                FileInfo fileInfo = new FileInfo(file);
                fileInfo.Delete();
            }

        }


        //private void NewClipboardFileToPaste(Object source, Object param)
        //{
        //    //leggere il JSON e preparare il lavoro per i file e le cartelle che arrivano
        //    DeleteFileDirContent(ProtocolUtils.TMP_DIR);
        //    RequestState requestState = (RequestState)param;

        //    JObject contentJson = (JObject)requestState.stdRequest[ProtocolUtils.CONTENT];

        //    List<ProtocolUtils.FileStruct> files = new List<ProtocolUtils.FileStruct>();
        //    files = JsonConvert.DeserializeObject<List<ProtocolUtils.FileStruct>>(contentJson[ProtocolUtils.FILE].ToString());
        //    filesToReceive.AddRange(files);
        //    foreach (ProtocolUtils.FileStruct fileStruct in files)
        //    {
        //        fileDropList.Add(ProtocolUtils.TMP_DIR + fileStruct.name);
        //    }

        //    foreach (var prop in contentJson)
        //    {
        //        if (prop.Key != ProtocolUtils.FILE)
        //        {
        //            Directory.CreateDirectory(ProtocolUtils.TMP_DIR + prop.Key);
        //            CreateClipboardContent((JObject)contentJson[prop.Key], prop.Key);
        //            fileDropList.Add(ProtocolUtils.TMP_DIR + prop.Key);
        //        }
        //    }


        //}

        //
        private void CreateClipboardContent(JObject contentJson, string dir)
        {
            string path = dir;
            List<ProtocolUtils.FileStruct> files = new List<ProtocolUtils.FileStruct>();
            try {
                files = JsonConvert.DeserializeObject<List<ProtocolUtils.FileStruct>>(contentJson[ProtocolUtils.FILE].ToString());
                filesToReceive.AddRange(files);
            }
            catch (NullReferenceException ex)
            {
                ;
            }
            foreach (var prop in contentJson)
            {
                if (prop.Key != ProtocolUtils.FILE)
                {
                    Directory.CreateDirectory(ProtocolUtils.TMP_DIR + path + "\\" + prop.Key);
                    CreateClipboardContent((JObject)contentJson[prop.Key], path + "\\" + prop.Key);
                }
            }
        }


        //private void NewClipboardDataToPaste(Object source, Object param)
        //{
        //    //XDocument xRequest = ((Request)param).xRequest;
        //    JObject stdRequest = ((RequestState)param).stdRequest;
        //    MainForm.mainForm.Invoke(MainForm.clipboardTextDelegate, stdRequest[ProtocolUtils.CONTENT].ToString());
        //    RequestState value = new RequestState();
        //    if (!requestDictionary.TryRemove(((RequestState)param).token, out value))
        //    {//custom exception would be better than this
        //        throw new Exception("Request not present in the dictionary");
        //    }
        //}

        private void NewClipboardFileToPaste(JObject contentJson)
        {
            //leggere il JSON e preparare il lavoro per i file e le cartelle che arrivano
            DeleteFileDirContent(ProtocolUtils.TMP_DIR);

            List<ProtocolUtils.FileStruct> files = new List<ProtocolUtils.FileStruct>();
            try
            {
                files = JsonConvert.DeserializeObject<List<ProtocolUtils.FileStruct>>(contentJson[ProtocolUtils.FILE].ToString());
                filesToReceive.AddRange(files);
            } catch (NullReferenceException ex)
            {
                ;
            }

            foreach (var prop in contentJson)
            {
                if (prop.Key != ProtocolUtils.FILE)
                {
                    Directory.CreateDirectory(ProtocolUtils.TMP_DIR + prop.Key);
                    CreateClipboardContent((JObject)contentJson[prop.Key], prop.Key);
                }
            }
        }

        public void ReceiveClipboard()
        {
            this.ChannelMgr.SendRequest(ProtocolUtils.GET_CLIPBOARD_CONTENT, String.Empty);
            byte[] buffer = this.ChannelMgr.ReceiveData();
            if (buffer != null)
            {
                receivedJson = JObject.Parse(Encoding.Unicode.GetString(buffer));
                string type = receivedJson[ProtocolUtils.TYPE].ToString();
                switch (type)
                {
                    case ProtocolUtils.SET_CLIPBOARD_FILES:
                        NewClipboardFileToPaste((JObject)receivedJson[ProtocolUtils.CONTENT]);
                        MoveByteToFiles();
                        currentContent = ProtocolUtils.SET_CLIPBOARD_FILES;
                        break;
                    case ProtocolUtils.SET_CLIPBOARD_TEXT:
                        this.Text = (String) receivedJson[ProtocolUtils.CONTENT];
                        currentContent = ProtocolUtils.SET_CLIPBOARD_TEXT;
                        break;
                    case ProtocolUtils.SET_CLIPBOARD_IMAGE:
                        this.Data = new byte[(int)receivedJson[ProtocolUtils.CONTENT]];
                        currentContent = ProtocolUtils.SET_CLIPBOARD_IMAGE;
                        NewClipboardDataToPaste();
                        break;
                }

            }
        }

        private void NewClipboardDataToPaste()
        {           
            string filename = String.Empty;
            if (currentContent == ProtocolUtils.SET_CLIPBOARD_IMAGE)
            {
                filename = ProtocolUtils.TMP_IMAGE_FILE;
                //chiedo di mandarmi l'immagine e ricevo i dati;
            }
            int offset = 0;
            while (offset < Data.Length)
            {                    
                    this.ChannelMgr.SendRequest(ProtocolUtils.GET_CLIPBOARD_IMG, String.Empty);
                    byte[] bufferData = this.ChannelMgr.ReceiveData();
                    System.Buffer.BlockCopy(bufferData, 0, Data, offset, bufferData.Length);                    
                    offset += bufferData.Length;   
            }                                    
        }

        public bool GetClipboardDimensionOverFlow()
        {
            this.ChannelMgr.SendRequest(ProtocolUtils.GET_CLIPBOARD_DIMENSION, String.Empty);
            long dimension = this.ChannelMgr.GetClipboardDimension();
            if (dimension >= ProtocolUtils.CLIBPOARD_DIM_THRESHOLD)
            {
                return true;
            }
            else
            {
                if (dimension == 0)
                {
                    throw new Exception("No clipboard content");
                }
                return false;
            }
        }

        public void SendClipboard()
        {
            switch (currentContent)
            {
                case ProtocolUtils.SET_CLIPBOARD_FILES:
                    SendClipboardFiles();
                    break;
                case ProtocolUtils.SET_CLIPBOARD_TEXT:
                    SendClipboardText();
                    break;
                case ProtocolUtils.SET_CLIPBOARD_IMAGE:
                    SendClipboardImg();
                    break;
            }
            
        }

        private void SendClipboardImg()
        {
            this.ChannelMgr.AssignNewToken();
            StandardRequest sr = new StandardRequest();
            sr.type = Protocol.ProtocolUtils.SET_CLIPBOARD_IMAGE;
            sr.content = Data.Length;
            string toSend = JSON.JSONFactory.CreateJSONStandardRequest(sr);
            this.ChannelMgr.SendBytes(Encoding.Unicode.GetBytes(toSend));
            this.ChannelMgr.ReceiveAck();
            int offset = 0;
            while (offset < Data.Length)
            {
                int dim = 1024;
                if (Data.Length - offset < dim)
                {
                    dim = Data.Length - offset;
                }
                byte[] chunk = new byte[dim];                
                System.Buffer.BlockCopy(Data, offset, chunk, 0, dim);
                offset += dim;
                this.ChannelMgr.SendBytes(chunk);
                this.ChannelMgr.ReceiveAck();
            }
        }       

        private void SendClipboardText()
        {
            this.ChannelMgr.AssignNewToken();
            StandardRequest sr = new StandardRequest();
            sr.content = this.Text;
            sr.type = ProtocolUtils.SET_CLIPBOARD_TEXT;
            string toSend = JSON.JSONFactory.CreateJSONStandardRequest(sr);
            this.ChannelMgr.SendBytes(Encoding.Unicode.GetBytes(toSend));
            this.ChannelMgr.ReceiveAck();
        }

        private void SendClipboardFiles()
        {
            String[] fileDropListArray = RetrieveFileDropListArray();
            this.ChannelMgr.AssignNewToken();
            string toSend = JSON.JSONFactory.CreateFileTransferJSONRequest(Protocol.ProtocolUtils.SET_CLIPBOARD_FILES, fileDropListArray);
            this.ChannelMgr.SendBytes(Encoding.Unicode.GetBytes(toSend));
            this.ChannelMgr.ReceiveAck();
            byte[] token = this.ChannelMgr.CurrentToken;
            foreach (ProtocolUtils.FileStruct fileStruct in filesToReceive)
            {
                string currenFilePath = ProtocolUtils.TMP_DIR + fileStruct.dir + "\\" + fileStruct.name;
                byte[] bytesFileToSend = new byte[1024];
                if (File.Exists(currenFilePath))
                {
                    byte[] bytesFile = new byte[1024 - TokenGenerator.TOKEN_DIM];
                    using (var stream = new FileStream(currenFilePath, FileMode.Open))
                    {
                        int bytesRead;
                        while ((bytesRead = stream.Read(bytesFile, 0, bytesFile.Length)) > 0)
                        {
                            byte[] byteToSend = new byte[bytesRead + TokenGenerator.TOKEN_DIM];
                            System.Buffer.BlockCopy(token, 0, byteToSend, 0, TokenGenerator.TOKEN_DIM);
                            System.Buffer.BlockCopy(bytesFile, 0, byteToSend, TokenGenerator.TOKEN_DIM, bytesRead);

                            this.ChannelMgr.SendBytes(byteToSend);
                            this.ChannelMgr.ReceiveAck();
                        }

                    }
                }
            }           
        }

        public void ResetClassValues()
        {
            this.currentContent = null;
            this.receivedJson = null;
            this.Data = null;
            this.filesToReceive.Clear();
            this.audio = null;
        }

        private string[] RetrieveFileDropListArray()
        {
            StringCollection strColl = new StringCollection();            
            foreach (String fullNameFiles in Directory.GetFiles(ProtocolUtils.TMP_DIR))
            {
                //FileInfo fileInfo = new FileInfo(fullNameFiles);
                strColl.Add(fullNameFiles);
            }
            foreach (String fullNameDir in Directory.GetDirectories(ProtocolUtils.TMP_DIR)) 
            {
                //DirectoryInfo dir = new DirectoryInfo(fullNameDir);
                strColl.Add(fullNameDir);
            }
            String[] fileDropList = new String[strColl.Count];
            strColl.CopyTo(fileDropList, 0);
            return fileDropList;
        }
    }
}
