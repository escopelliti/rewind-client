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

namespace WpfApplication1
{
    public class ClipboardMgr
    {
        public Image image { get; set; }
        public Stream audio { get; set; }
        public List<ProtocolUtils.FileStruct> filesToReceive { get; set; }
        public JObject receivedJson { get; set; }
        
        public ChannelManager ChannelMgr {get; set;}

        private int currentFileNum;

        public ClipboardMgr()
        {            
            currentFileNum = 0;
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
            byte[] bufferData = this.ChannelMgr.ReceiveData();
            while (bufferData != null)
            {
                if (currentFileNum < filesToReceive.Count)
                {
                    ProtocolUtils.FileStruct currentFile = filesToReceive.ElementAt(currentFileNum);

                    using (var stream = new FileStream(ProtocolUtils.TMP_DIR + currentFile.dir + currentFile.name, FileMode.Append))
                    {
                        stream.Write(bufferData, 0, bufferData.Length);
                        stream.Close();
                        //ACK
                        this.ChannelMgr.SendRequest(ProtocolUtils.GET_CLIPBOARD_FILES, String.Empty);
                    }
                    if (new FileInfo(ProtocolUtils.TMP_DIR + currentFile.dir + currentFile.name).Length == currentFile.size)
                    {
                        currentFileNum++;
                        if (currentFileNum == filesToReceive.Count)
                        {
                            break;
                        }
                    }
                }
            }
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

        private void CreateClipboardContent(JObject contentJson, string dir)
        {
            List<ProtocolUtils.FileStruct> files = new List<ProtocolUtils.FileStruct>();
            files = JsonConvert.DeserializeObject<List<ProtocolUtils.FileStruct>>(contentJson[ProtocolUtils.FILE].ToString());
            filesToReceive.Concat(files);
            foreach (var prop in contentJson)
            {
                if (prop.Key != ProtocolUtils.FILE)
                {
                    Directory.CreateDirectory(ProtocolUtils.TMP_DIR + dir + "\\" + prop.Key);
                    CreateClipboardContent((JObject)contentJson[prop.Key], prop.Key);
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
            files = JsonConvert.DeserializeObject<List<ProtocolUtils.FileStruct>>(contentJson[ProtocolUtils.FILE].ToString());
            filesToReceive.AddRange(files);            

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
                        NewClipboardFileToPaste((JObject) receivedJson[ProtocolUtils.CONTENT]);
                        MoveByteToFiles();
                        break;
                }

            } 
        }

        public bool GetClipboardDimensionOverFlow()
        {
            this.ChannelMgr.SendRequest(ProtocolUtils.GET_CLIPBOARD_DIMENSION, String.Empty);
            long dimension = this.ChannelMgr.GetClipboardDimension();
            if (dimension >= ProtocolUtils.CLIBPOARD_DIM_THRESHOLD)
            {
                //apri finestra
                return false;
            }
            else
            {
                return true;
            }
        }
    }
    //struct RequestState
    //{
    //    public Client client;
    //    public byte[] data;
    //    public JObject stdRequest;
    //    public string type;
    //    public string token;
    //}
}
