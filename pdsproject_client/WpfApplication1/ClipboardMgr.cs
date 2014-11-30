﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
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
        public String Text { get; set; }
        public JObject receivedJson { get; set; }

        public ChannelManager ChannelMgr { get; set; }
        
        private String currentContent;

        public ClipboardMgr()
        {            
            currentContent = String.Empty;
            filesToReceive = new List<ProtocolUtils.FileStruct>();
        }        

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
                if (bufferData == null)
                {
                    ResetClassValues();
                    currentContent = "NONE";
                    return;
                }

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

        private void CreateClipboardContent(JObject contentJson, string dir)
        {
            string path = dir;
            List<ProtocolUtils.FileStruct> files = new List<ProtocolUtils.FileStruct>();
            try {
                files = JsonConvert.DeserializeObject<List<ProtocolUtils.FileStruct>>(contentJson[ProtocolUtils.FILE].ToString());
                filesToReceive.AddRange(files);
            }
            catch (NullReferenceException)
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
            } catch (NullReferenceException)
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
                receivedJson = JObject.Parse(Encoding.Unicode.GetString(buffer));
                string type = receivedJson[ProtocolUtils.TYPE].ToString();
                switch (type)
                {
                    case ProtocolUtils.SET_CLIPBOARD_FILES:
                        NewClipboardFileToPaste((JObject)receivedJson[ProtocolUtils.CONTENT]);
                        currentContent = ProtocolUtils.SET_CLIPBOARD_FILES;
                        MoveByteToFiles();                        
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
                    case ProtocolUtils.SET_CLIPBOARD_AUDIO:
                        this.Data = new byte[(int)receivedJson[ProtocolUtils.CONTENT]];
                        currentContent = ProtocolUtils.SET_CLIPBOARD_AUDIO;
                        NewClipboardDataToPaste();
                        break;
                    default:
                        break;
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
                byte[] bytesFileToSend = new byte[4096];
                if (File.Exists(currenFilePath))
                {
                    byte[] bytesFile = new byte[4096 - TokenGenerator.TOKEN_DIM];
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
            this.currentContent = "NONE";
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
