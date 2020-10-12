using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranspileTest.Parser;

namespace TranspileTest.ExpressionParser
{
    public static class ExpressionSerializer
    {
        public static Expression Read(BinaryReader br)
        {
            int instructionCount = br.ReadInt32();

            var rv = new Expression();
            rv.Instructions = new List<ExpressionToken>();
            var idBuffer = new byte[16];
            for (int i = 0; i < instructionCount; i++)
            {
                var expresionToken = new ExpressionToken();
                
                expresionToken.TokenType = (SemanticTokenType)br.ReadUInt16();
                expresionToken.OperationType = (OperationType)br.ReadByte();

                var id = br.Read(idBuffer, 0, 16);
                var guid = new Guid(idBuffer);
                expresionToken.Data.Guid = guid;

                expresionToken.OperatorAssociativity = (OperatorAssociativity)br.ReadByte();
                expresionToken.Precedence = br.ReadByte();

                rv.Instructions.Add(expresionToken);
            }

            return rv;
        }

        public static void Write(Expression expression, BinaryWriter bw)
        {
            bw.Write(expression.Instructions.Count);
            foreach (var i in expression.Instructions)
            {
                // TODO Need an execution token to cut out some of the meta data
                bw.Write((UInt16)i.TokenType);
                bw.Write((byte)i.OperationType);
                bw.Write(i.Data.Guid.ToByteArray());

                // These two are probably not required for runtime operations
                bw.Write((byte)i.OperatorAssociativity);
                bw.Write((byte)i.Precedence);
            }
        }
    }
}
