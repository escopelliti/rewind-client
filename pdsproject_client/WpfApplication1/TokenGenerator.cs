using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1
{
    public class TokenGenerator
    {
        private List<Guid> tokenList;
        public const ushort TOKEN_DIM = 16;
        public TokenGenerator()
        {
            this.tokenList = new List<Guid>();
        }

        public byte[] GetNewToken()
        {
            byte[] token;
            Guid guid = Guid.NewGuid();           
            while (tokenList.Contains(guid))
            {
                guid = Guid.NewGuid();
            }
            token = guid.ToByteArray();
            return token;
        } 

    }
}
