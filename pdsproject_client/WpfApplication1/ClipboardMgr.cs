//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using System.IO;
//using System.Drawing;
//using Protocol;

//namespace WpfApplication1
//{
//    class ClipboardMgr
//    {
//        public Image image { get; set; }
//        public Stream audio { get; set; }
//        public List<ProtocolUtils.FileStruct> filesToReceive { get; set; }
//        public String contentToReceive { get; set; }

//        public ClipboardMgr(String contentType)
//        {
//            this.contentToReceive = contentType;
//        }

//        private void ReceiveDataForClipboard(Object source, Object param)
//        {
//            RequestState requestState = (RequestState)param;
//            string filename = ProtocolUtils.protocolDictionary[requestState.type];
//            using (var stream = new FileStream(filename, FileMode.Append))
//            {
//                stream.Write(requestState.data, 0, requestState.data.Length);
//                stream.Close();
//                //chi chiama questa funziona chiamera questa send
//                //server.Send(Convert.FromBase64String(requestState.token), requestState.client.GetSocket());
//            }
//            if (new FileInfo(filename).Length >= Convert.ToInt64(requestState.stdRequest[ProtocolUtils.CONTENT].ToString()))
//            {

//                //CHI CHIAMA QUESTO METODO DEVE FARE QUESTA COSA
//                //RequestState value = new RequestState();
//                //if (!requestDictionary.TryRemove(requestState.token, out value))
//                //{//custom exception would be better than this
//                //    throw new Exception("Request not present in the dictionary");
//                //}

//                //TODO : AVOID CONDITIONAL TEST
//                if (requestState.type == ProtocolUtils.TRANSFER_IMAGE)
//                {
//                    CreateImageForClipboard(filename);
//                }
//            }
//        }

//        private void CreateImageForClipboard(string filename)
//        {
//            Image image = null;
//            using (var ms = new MemoryStream(File.ReadAllBytes(filename)))
//            {
//                image = Image.FromStream(ms);
//            }
//            File.Delete(filename);
//            this.image = image;
//            //MainForm.mainForm.Invoke(MainForm.clipboardImageDelegate, image);
//        }

//        private void MoveByteToFiles(Object source, Object param)
//        {
//            RequestState request = (RequestState)param;
//            if (currentFileNum < filesToReceive.Count)
//            {
//                ProtocolUtils.FileStruct currentFile = filesToReceive.ElementAt(currentFileNum);
//                using (var stream = new FileStream(ProtocolUtils.TMP_DIR + currentFile.dir + currentFile.name, FileMode.Append))
//                {
//                    stream.Write(request.data, 0, request.data.Length);
//                    stream.Close();
//                    server.Send(Convert.FromBase64String(request.token), request.client.GetSocket());
//                }
//                if (new FileInfo(ProtocolUtils.TMP_DIR + currentFile.dir + currentFile.name).Length == currentFile.size)
//                {
//                    currentFileNum++;
//                    if (currentFileNum == filesToReceive.Count)
//                    {
//                        RequestState value = new RequestState();
//                        if (!requestDictionary.TryRemove(request.token, out value))
//                        {//custom exception would be better than this
//                            throw new Exception("Request not present in the dictionary");
//                        }
//                        MainForm.mainForm.Invoke(MainForm.clipboardFilesDelegate, fileDropList);
//                    }
//                }
//            }
//        }

//        private static void DeleteFileDirContent(string toRemove)
//        {

//            foreach (string dir in Directory.GetDirectories(toRemove))
//            {
//                DirectoryInfo dirInfo = new DirectoryInfo(dir);
//                dirInfo.Delete(true);
//            }
//            foreach (string file in Directory.GetFiles(toRemove))
//            {
//                FileInfo fileInfo = new FileInfo(file);
//                fileInfo.Delete();
//            }

//        }


//        private void NewClipboardFileToPaste(Object source, Object param)
//        {
//            //leggere il JSON e preparare il lavoro per i file e le cartelle che arrivano
//            DeleteFileDirContent(ProtocolUtils.TMP_DIR);
//            RequestState requestState = (RequestState)param;

//            JObject contentJson = (JObject)requestState.stdRequest[ProtocolUtils.CONTENT];

//            List<ProtocolUtils.FileStruct> files = new List<ProtocolUtils.FileStruct>();
//            files = JsonConvert.DeserializeObject<List<ProtocolUtils.FileStruct>>(contentJson[ProtocolUtils.FILE].ToString());
//            filesToReceive.AddRange(files);
//            foreach (ProtocolUtils.FileStruct fileStruct in files)
//            {
//                fileDropList.Add(ProtocolUtils.TMP_DIR + fileStruct.name);
//            }

//            foreach (var prop in contentJson)
//            {
//                if (prop.Key != ProtocolUtils.FILE)
//                {
//                    Directory.CreateDirectory(ProtocolUtils.TMP_DIR + prop.Key);
//                    CreateClipboardContent((JObject)contentJson[prop.Key], prop.Key);
//                    fileDropList.Add(ProtocolUtils.TMP_DIR + prop.Key);
//                }
//            }
            

//        }

//        private void CreateClipboardContent(JObject contentJson, string dir)
//        {
//            List<ProtocolUtils.FileStruct> files = new List<ProtocolUtils.FileStruct>();
//            files = JsonConvert.DeserializeObject<List<ProtocolUtils.FileStruct>>(contentJson[ProtocolUtils.FILE].ToString());
//            filesToReceive.Concat(files);
//            foreach (var prop in contentJson)
//            {
//                if (prop.Key != ProtocolUtils.FILE)
//                {
//                    Directory.CreateDirectory(ProtocolUtils.TMP_DIR + dir + "\\" + prop.Key);
//                    CreateClipboardContent((JObject)contentJson[prop.Key], prop.Key);
//                }
//            }
//        }


//        private void NewClipboardDataToPaste(Object source, Object param)
//        {
//            //XDocument xRequest = ((Request)param).xRequest;
//            JObject stdRequest = ((RequestState)param).stdRequest;
//            MainForm.mainForm.Invoke(MainForm.clipboardTextDelegate, stdRequest[ProtocolUtils.CONTENT].ToString());
//            RequestState value = new RequestState();
//            if (!requestDictionary.TryRemove(((RequestState)param).token, out value))
//            {//custom exception would be better than this
//                throw new Exception("Request not present in the dictionary");
//            }
//        }

//    }
//    struct RequestState
//    {
//        public Client client;
//        public byte[] data;
//        public JObject stdRequest;
//        public string type;
//        public string token;
//    }
//}
