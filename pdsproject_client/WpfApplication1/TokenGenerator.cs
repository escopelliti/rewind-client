using System;
using System.Collections.Generic;

namespace Protocol
{
    public class TokenGenerator
    {
        private List<Guid> tokenList;
        public const ushort TOKEN_DIM = 4;
        public TokenGenerator()
        {
            this.tokenList = new List<Guid>();
        }

        public void Reset()
        {
            this.tokenList.Clear();
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
            byte[] resizedToken = new byte[TOKEN_DIM];
            System.Buffer.BlockCopy(token, TOKEN_DIM, resizedToken, 0, TOKEN_DIM);
            return resizedToken;
        }
    }
}
