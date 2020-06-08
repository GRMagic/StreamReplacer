using System;
using System.IO;
using System.Text;
using Xunit;

namespace StreamReplaced.Test
{
    public class UnitTest1
    {
        [Theory]
        [InlineData("Traga-me um café!", "um café", "uma cerveja", "Traga-me uma cerveja!")]
        [InlineData("Qual é seu nome?", "nome", "email", "Qual é seu email?")]
        [InlineData("Trocar o começo", "Trocar", "Mudar", "Mudar o começo")]
        [InlineData("Trocar o fim", "fim", "final", "Trocar o final")]
        [InlineData("Eu gosto de coca", "coca", "coco", "Eu gosto de coco")]
        [InlineData("Trocar as letras a da frase", "a", "X", "TrocXr Xs letrXs X dX frXse")]
        [InlineData("Não encontrar", "abc", "XXX", "Não encontrar")]
        [InlineData("AABBCC", "B", "X", "AAXXCC")]
        [InlineData("AABBCC", "B", "XXX", "AAXXXXXXCC")]
        [InlineData("AABBCC", "B", "", "AACC")]
        [InlineData("ABC", "C", "+", "AB+")]
        [InlineData("ABC", "A", "X", "ABC", 1)]
        [InlineData("ABCA", "A", "X", "ABCX", 1)]
        [InlineData("ABCA", "A", "X", "XBCA", 0, 2)]
        [InlineData("ABCA", "A", "X", "ABCA", 1, 2)]
        [InlineData("ABCA", "A", "X", "ABCX", 1, 2000)]
        public void StreamReplaced_ByteReplace_SubstituirComSucesso(string content, string from, string to, string expected, int start = 0, int? end = null)
        {
            // Arrange
            var bContent = Encoding.UTF8.GetBytes(content);
            var bFrom = Encoding.UTF8.GetBytes(from);
            var bTo = Encoding.UTF8.GetBytes(to);

            // Act
            var result = StreamReplacer.ByteReplace(bContent, bFrom, bTo, start, end ?? bContent.Length);

            //Assert
            var sResult = Encoding.UTF8.GetString(result);
            Assert.Equal(expected, sResult );

        }

        [Theory]
        [InlineData("Traga-me um café!", "café", "chá", "Traga-me um chá!")]
        [InlineData("Traga-me um café!", "um café", "uma cerveja", "Traga-me uma cerveja!")]
        [InlineData("Qual é seu nome?", "nome", "email", "Qual é seu email?")]
        [InlineData("Trocar o começo", "Trocar", "Mudar", "Mudar o começo")]
        [InlineData("Trocar o fim", "fim", "final", "Trocar o final")]
        [InlineData("Eu gosto de coca", "coca", "coco", "Eu gosto de coco")]
        [InlineData("Trocar as letras a da frase", "a", "X", "TrocXr Xs letrXs X dX frXse")]
        [InlineData("Não encontrar", "abc", "XXX", "Não encontrar")]
        [InlineData("AABBCC", "B", "X", "AAXXCC")]
        [InlineData("AABBCC", "B", "XXX", "AAXXXXXXCC")]
        [InlineData("AABBCC", "B", "", "AACC")]
        [InlineData("ABC", "C", "+", "AB+")]
        [InlineData("Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
            "Lorem", "???",
            "??? Ipsum is simply dummy text of the printing and typesetting industry. ??? Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing ??? Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of ??? Ipsum.")]
        [InlineData("Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
            ".", ";",
            "Lorem Ipsum is simply dummy text of the printing and typesetting industry; Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book; It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged; It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum;")]
        public void StreamReplaced_Read_SubstituirComSucesso(string content, string from, string to, string expected)
        {
            // Arrange
            var stream = GenerateStreamFromString(content);
            var sReplaced = new StreamReplacer(stream);

            var bFrom = Encoding.UTF8.GetBytes(from);
            var bTo = Encoding.UTF8.GetBytes(to);

            sReplaced.Replace(bFrom, bTo);

            var sr = new StreamReader(sReplaced, Encoding.UTF8, false, 128);

            // Act
            var result = sr.ReadToEnd();
            sr.Dispose();

            // Assert
            Assert.Equal(expected, result);
        }

        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

    }
}
