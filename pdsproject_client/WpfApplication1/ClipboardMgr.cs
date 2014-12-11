using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;

using Protocol;
using ConnectionModule;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Specialized;

namespace Clipboard
{
    public class ClipboardMgr
    {
        public byte[] Data { get; set; }
        public Stream audio { get; set; }
        public List<ProtocolUtils.FileStruct> filesToReceive { get; set; }
        public List<ProtocolUtils.FileStruct> filesToSend { get; set; }
        public String Text { get; set; }
        public JObject receivedJson { get; set; }

        public ChannelManager ChannelMgr { get; set; }
        
        private String currentContent;

        public ClipboardMgr()
        {            
            currentContent = String.Empty;
            filesToReceive = new List<ProtocolUtils.FileStruct>();
            filesToSend = new List<ProtocolUtils.FileStruct>();
        }        

        private void MoveByteToFiles()
        {
            int currentFileNum = 0;
            this.ChannelMgr.SendRequest(ProtocolUtils.GET_CLIPBOARD_FILES, String.Empty);
            long dim = 0;
            ProtocolUtils.FileStruct currentFile;
            try
            {
                currentFile = filesToReceive.ElementAt(currentFileNum);
            }
            catch (Exception)
            {
                this.ChannelMgr.DeleteServer(this.ChannelMgr.GetCurrentServer(), System.Net.Sockets.SocketShutdown.Both);
                MessageBox.Show("C'è stato un problema! Prova a riavviare l'applicazione appena possibile.", "Ops...", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            while (currentFileNum < filesToReceive.Count)
            {
                if (currentFile.size == 0)
                {
                    File.Create(ProtocolUtils.TMP_DIR + currentFile.dir + currentFile.name);
                    currentFileNum++;
                    dim = 0;
                    if (currentFileNum == filesToReceive.Count)
                    {
                        this.ChannelMgr.SendRequest(ProtocolUtils.GET_CLIPBOARD_FILES, String.Empty);
                        break;
                    }
                    this.ChannelMgr.ReceiveAck();
                    this.ChannelMgr.SendRequest(ProtocolUtils.GET_CLIPBOARD_FILES, String.Empty);
                    currentFile = filesToReceive.ElementAt(currentFileNum);
                    continue;
                }
                byte[] bufferData = this.ChannelMgr.ReceiveData();
                if (bufferData == null)
                {
                    ResetClassValues();
                    currentContent = "NONE";
                    return;
                }
                try
                {
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
                catch (Exception)
                {
                    this.ChannelMgr.DeleteServer(this.ChannelMgr.GetCurrentServer(), System.Net.Sockets.SocketShutdown.Both);
                    MessageBox.Show("C'è stato un problema! Prova a riavviare l'applicazione appena possibile.", "Ops...", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
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

        private void CreateClipboardContent(JObject contentJson, string dir)
        {
            string path = dir;
            List<ProtocolUtils.FileStruct> files = new List<ProtocolUtils.FileStruct>();
            try
            {
                files = JsonConvert.DeserializeObject<List<ProtocolUtils.FileStruct>>(contentJson[ProtocolUtils.FILE].ToString());
                filesToReceive.AddRange(files);
            }
            catch (NullReferenceException)
            {
                //nothing to do
            }
            catch (Exception)
            {
                //nothing to do
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

        private void NewClipboardFileToPaste(JObject contentJson)
        {
            //leggere il JSON e preparare il lavoro per i file e le cartelle che arrivano
            DeleteFileDirContent(ProtocolUtils.TMP_DIR);

            List<ProtocolUtils.FileStruct> files = new List<ProtocolUtils.FileStruct>();
            try
            {
                files = JsonConvert.DeserializeObject<List<ProtocolUtils.FileStruct>>(contentJson[ProtocolUtils.FILE].ToString());
                filesToReceive.AddRange(files);
            } 
            catch (NullReferenceException)
            {
                //nothing to do
            }
            catch (Exception)
            {
                //nothing to do
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
                string type = null;
                try
                {
                    receivedJson = JObject.Parse(Encoding.Unicode.GetString(buffer));
                    type = receivedJson[ProtocolUtils.TYPE].ToString();
                    switch (type)
                    {
                        case ProtocolUtils.SET_CLIPBOARD_FILES:
                            NewClipboardFileToPaste((JObject)receivedJson[ProtocolUtils.CONTENT]);
                            currentContent = ProtocolUtils.SET_CLIPBOARD_FILES;
                            MoveByteToFiles();                        
                            break;
                        case ProtocolUtils.SET_CLIPBOARD_TEXT:
                            this.Text = receivedJson[ProtocolUtils.CONTENT].ToString();
                            currentContent = ProtocolUtils.SET_CLIPBOARD_TEXT;
                            break;
                        case ProtocolUtils.SET_CLIPBOARD_IMAGE:
                            this.Data = new byte[(int)receivedJson[ProtocolUtils.CONTENT]];
                            currentContent = ProtocolUtils.SET_CLIPBOARD_IMAGE;
                            NewClipboardDataToPaste();
                            break;
                        case ProtocolUtils.SET_CLIPBOARD_AUDIO:
                            this.Data = new byte[(int)receivedJson[ProtocolUtils.CONTENT]];
                            currentContent = ProtocolUtils.SET_CLIPBOARD_AUDIO;
                            NewClipboardDataToPaste();
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                } 
            }
        }

        private void NewClipboardDataToPaste()
        {           
            int offset = 0;
            while (offset < Data.Length)
            {                    
                this.ChannelMgr.SendRequest(ProtocolUtils.GET_CLIPBOARD_DATA, String.Empty);
                byte[] bufferData = this.ChannelMgr.ReceiveData();
                if (bufferData == null)
                {
                    currentContent = "NONE";
                    return;
                }
                try
                {
                    System.Buffer.BlockCopy(bufferData, 0, Data, offset, bufferData.Length);
                }
                catch (Exception)
                {
                    this.ChannelMgr.DeleteServer(this.ChannelMgr.GetCurrentServer(), System.Net.Sockets.SocketShutdown.Both);
                    MessageBox.Show("C'è stato un problema! Prova a riavviare l'applicazione appena possibile.", "Ops...", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }                
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
                    SendClipboardData(Protocol.ProtocolUtils.SET_CLIPBOARD_IMAGE);
                    break;
                case ProtocolUtils.SET_CLIPBOARD_AUDIO:
                    SendClipboardData(Protocol.ProtocolUtils.SET_CLIPBOARD_AUDIO);
                    break;
                default:
                    break;
            }
            
        }

        private void SendClipboardData(String requestType)
        {
            this.ChannelMgr.AssignNewToken();
            StandardRequest sr = new StandardRequest();
            sr.type = requestType;
            sr.content = Data.Length;
            string toSend = JSON.JSONFactory.CreateJSONStandardRequest(sr);
            if (toSend == null)
            {
                this.ChannelMgr.DeleteServer(this.ChannelMgr.GetCurrentServer(), System.Net.Sockets.SocketShutdown.Both);
                MessageBox.Show("C'è stato un problema! Prova a riavviare l'applicazione appena possibile.", "Ops...", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
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
                try
                {
                    System.Buffer.BlockCopy(Data, offset, chunk, 0, dim);
                }
                catch (Exception)
                {
                    this.ChannelMgr.DeleteServer(this.ChannelMgr.GetCurrentServer(), System.Net.Sockets.SocketShutdown.Both);
                    MessageBox.Show("C'è stato un problema! Prova a riavviare l'applicazione appena possibile.", "Ops...", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
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
            if (toSend == null)
            {
                this.ChannelMgr.DeleteServer(this.ChannelMgr.GetCurrentServer(), System.Net.Sockets.SocketShutdown.Both);
                MessageBox.Show("C'è stato un problema! Prova a riavviare l'applicazione appena possibile.", "Ops...", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            this.ChannelMgr.SendBytes(Encoding.Unicode.GetBytes(toSend));
            this.ChannelMgr.ReceiveAck();
        }

        private void SendClipboardFiles()
        {
            String[] fileDropListArray = RetrieveFileDropListArray();
            if (fileDropListArray == null)
            {
                this.ChannelMgr.DeleteServer(this.ChannelMgr.GetCurrentServer(), System.Net.Sockets.SocketShutdown.Both);
                MessageBox.Show("C'è stato un problema! Prova a riavviare l'applicazione appena possibile.", "Ops...", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;            
            }
            this.ChannelMgr.AssignNewToken();
            string toSend = JSON.JSONFactory.CreateFileTransferJSONRequest(Protocol.ProtocolUtils.SET_CLIPBOARD_FILES, fileDropListArray);
            if (toSend == null)
            {
                this.ChannelMgr.DeleteServer(this.ChannelMgr.GetCurrentServer(), System.Net.Sockets.SocketShutdown.Both);
                MessageBox.Show("C'è stato un problema! Prova a riavviare l'applicazione appena possibile.", "Ops...", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            this.ChannelMgr.SendBytes(Encoding.Unicode.GetBytes(toSend));
            this.ChannelMgr.ReceiveAck();
            byte[] token = this.ChannelMgr.CurrentToken;
            foreach (ProtocolUtils.FileStruct fileStruct in filesToSend)
            {
                string currenFilePath = ProtocolUtils.TMP_DIR + fileStruct.dir + "\\" + fileStruct.name;
                byte[] bytesFileToSend = new byte[1024];
                if (File.Exists(currenFilePath))
                {
                    if (new FileInfo(currenFilePath).Length == 0) 
                    {
                        this.ChannelMgr.SendBytes(new byte[0]);
                        this.ChannelMgr.ReceiveAck();
                        continue;
                    }

                    byte[] bytesFile = new byte[1024 - TokenGenerator.TOKEN_DIM];
                    try
                    {
                        using (var stream = new FileStream(currenFilePath, FileMode.Open))
                        {
                            int bytesRead;
                            while ((bytesRead = stream.Read(bytesFile, 0, bytesFile.Length)) > 0)
                            {
                                byte[] byteToSend = new byte[bytesRead];
                                System.Buffer.BlockCopy(bytesFile, 0, byteToSend, 0, bytesRead);
                                this.ChannelMgr.SendBytes(byteToSend);
                                this.ChannelMgr.ReceiveAck();
                            }
                        }
                    }
                    catch (Exception)
                    {
                        this.ChannelMgr.DeleteServer(this.ChannelMgr.GetCurrentServer(), System.Net.Sockets.SocketShutdown.Both);
                        MessageBox.Show("C'è stato un problema! Prova a riavviare l'applicazione appena possibile.", "Ops...", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;                           
                    }
                }
            }           
        }

        public void ResetClassValues()
        {            
            this.receivedJson = null;
            this.Data = null;
            this.filesToReceive.Clear();
            this.filesToSend.Clear();
            this.audio = null;
        }

        private string[] RetrieveFileDropListArray()
        {
            StringCollection strColl = new StringCollection();
            String[] fileDropList;
            try
            {
                foreach (String fullNameFiles in Directory.GetFiles(ProtocolUtils.TMP_DIR))
                {
                    strColl.Add(fullNameFiles);
                    ProtocolUtils.FileStruct fileStruct = this.filesToReceive.Find(x => fullNameFiles.Contains(x.name));
                    this.filesToSend.Add(fileStruct);
                }

                string[] dirToSend = Directory.GetDirectories(ProtocolUtils.TMP_DIR);
                if(dirToSend.Length > 0)
                {
                    this.filesToSend = this.filesToSend.Union(filesToReceive).ToList();
                }
                foreach (String fullNameDir in dirToSend)
                {
                    strColl.Add(fullNameDir);
                }
                fileDropList = new String[strColl.Count];
                strColl.CopyTo(fileDropList, 0);
            }
            catch (Exception)
            {
                return null;
            }
            return fileDropList;
        }
    }
}
