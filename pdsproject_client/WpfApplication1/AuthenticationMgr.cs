using System;
using System.Text;

using ConnectionModule;
using Protocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MainApp;

namespace Authentication
{
    public class AuthenticationMgr
    {
        private ChannelManager channelMgr;
        private MainWindow mainWin;

        public AuthenticationMgr(ChannelManager channelMgr, MainWindow mainWin)
        {
            this.channelMgr = channelMgr;
            this.mainWin = mainWin;           
        }

        public bool Authenticate(Server toAuthenticate, String pswDigest)
        {
            this.channelMgr.SendRequest(ProtocolUtils.TRY_AUTHENTICATE, pswDigest, toAuthenticate.GetChannel().GetCmdSocket());
            byte[] data = new byte[1024];
            int bytesRead = this.channelMgr.ccm.Receive(data, toAuthenticate.GetChannel().GetCmdSocket());
            if (bytesRead > 0)
            {
                byte[] actualData = new byte[bytesRead];
                System.Buffer.BlockCopy(data, 0, actualData, 0, bytesRead);
                data = null;
                try
                {
                    StandardRequest sr = JsonConvert.DeserializeObject<StandardRequest>(Encoding.Unicode.GetString(actualData));                                           
                    if (sr.type.Equals(ProtocolUtils.TRY_AUTHENTICATE))
                    {
                        if ((bool)sr.content)
                        {
                            toAuthenticate.Authenticated = true;
                            mainWin.Permitted(toAuthenticate);
                            //autnticato --> evento
                            return true;
                        }
                        else
                        {
                            toAuthenticate.Authenticated = false;                            
                            //non autenticato --> evento
                            mainWin.Forbidden(toAuthenticate);
                            return false;
                        }                        
                    }
                }
                catch (JsonException ex)
                {
                    toAuthenticate.Authenticated = false;
                    mainWin.Forbidden(toAuthenticate);
                    throw ex;   
                }
                catch (Exception ex)
                {
                    toAuthenticate.Authenticated = false;
                    mainWin.Forbidden(toAuthenticate);
                    throw ex;
                }
                
            }
            throw new Exception("problem on socket");
        }

    }
}
