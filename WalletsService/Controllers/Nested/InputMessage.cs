using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace WalletsService.Controllers.Nested
{
    public enum InputMessageError
    {
        [Description("None")] None,
        [Description("X-UserId error")] UserId,
        [Description("X-Digest error")] Digest,
        [Description("Json error")] Json,
        [Description("Wallet not found")] WalletNotFound
    }

    public class InputMessage
    {
        public Guid UserId { get; set; }

        public string Digest { get; set; } = string.Empty;

        public byte[]? Data { get; set; }

        public InputMessageError Error { get; set;} = InputMessageError.None;

        private static Guid GetUserGuid(WalletsController controller)
        {
            var headers = controller.Request.Headers;
            var userId = headers["X-UserId"];
            if (userId.Count <= 0) return Guid.Empty;
            return Guid.TryParse(userId[0], out var userGuid) ? userGuid : Guid.Empty;
        }

        private static string GetDigest(WalletsController controller)
        {
            var headers = controller.Request.Headers;
            var digest = headers["X-Digest"];
            return digest.Count <= 0 ? string.Empty : digest[0] ?? string.Empty;
        }

        public static InputMessage ReadUserId(WalletsController controller)
        {
            var id = GetUserGuid(controller);
            var mes = new InputMessage { UserId = id };
            if (id == Guid.Empty) mes.Error = InputMessageError.UserId;
            return mes;
        }

        private static bool CompareHash(byte[] rawData, string testHash)
        {
            using var sha1 = new SHA1Managed();
            var hash = sha1.ComputeHash(rawData);
            var sb = new StringBuilder(hash.Length * 2);

            foreach (var b in hash)
            {
                sb.Append(b.ToString("X2"));
            }

            var h1 = sb.ToString();
            return testHash == h1;
        }

        private static async Task<byte[]?> GetMessage(WalletsController controller)
        {
            byte[]? rawData;

            using (var ms = new MemoryStream())
            {
                await controller.Request.Body.CopyToAsync(ms);
                ms.Position = 0;
                rawData = ms.ToArray();
            }

            var digest = GetDigest(controller);
            var compareHash = CompareHash(rawData, digest);

            return !compareHash ? null : rawData;
        }

        public static InputMessage ReadMessaged(WalletsController controller)
        {
            var id = GetUserGuid(controller);
            var mes = new InputMessage { UserId = id };
            if (id == Guid.Empty) mes.Error = InputMessageError.UserId;
            else
            {
                mes.Data = GetMessage(controller).Result;
                if (mes.Data == null) mes.Error = InputMessageError.Digest;
            }
            return mes;
        }

        public T? ParseJson<T>() => Data != null ? JsonNode.Parse(Data).Deserialize<T>() : default;
    }
}