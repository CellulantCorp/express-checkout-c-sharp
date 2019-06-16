using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
class Checkout
{
    static void Main(string[] args)
    {
        Dictionary<string, string> requestBody = new Dictionary<string, string>();
        requestBody.Add("merchantTransactionID", "<unique_transaction_value>");
        requestBody.Add("customerFirstName", "John");
        requestBody.Add("customerLastName", "Doe");
        requestBody.Add("MSISDN", "2547XXXXXXXX");
        requestBody.Add("customerEmail", "john.doe@example.com");
        requestBody.Add("requestAmount", "100");
        requestBody.Add("currencyCode", "KES");
        requestBody.Add("accountNumber", "<account_reference>");
        requestBody.Add("serviceCode", "MULADEMOX");
        requestBody.Add("dueDate", "2019-06-01 23:59:59");
        requestBody.Add("requestDescription", "Dummy merchant transaction");
        requestBody.Add("countryCode", "<ISO_country_code>");
        requestBody.Add("languageCode", "en");
        requestBody.Add("successRedirectUrl", "<YOUR_SUCCESS_REDIRECT_URL>");
        requestBody.Add("failRedirectUrl", "<YOUR_FAIL_REDIRECT_URL>");
        requestBody.Add("paymentWebhookUrl", "<PAYMENT_WEBHOOK_URL>");

        string payload = JsonConvert.SerializeObject(requestBody);
        payload = Regex.Replace(payload, "/", "\\/");

        string IV = "";
        string key = "";

        Console.WriteLine(Encrypt(payload, key, IV));
    }

    public static string Encrypt(string payload, string key, string IV) {
        byte[] result;

        using (var algo = Aes.Create()) 
        {
            algo.KeySize = 256;
            algo.BlockSize = 128;
            algo.Mode = CipherMode.CBC;
            algo.Padding = PaddingMode.PKCS7;
            algo.IV = Encoding.UTF8.GetBytes(HashString(IV).Substring(0, 16));
            algo.Key = Encoding.UTF8.GetBytes(HashString(key).Substring(0, 32));

            var cipher = algo.CreateEncryptor();

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, cipher, CryptoStreamMode.Write))
                    {
                        using (var streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(payload);
                        }

                        result = memoryStream.ToArray();
                    }
            }
        }

        return Convert.ToBase64String(Encoding.UTF8.GetBytes(Convert.ToBase64String(result)));
    }

    private static string HashString(string str)
    {
        var stringBuilder = new StringBuilder();

        using (var hash = SHA256.Create())
        {
            var result = hash.ComputeHash(Encoding.UTF8.GetBytes(str));
            foreach (var x in result)
            {
                stringBuilder.Append(x.ToString("x2"));
            }
        }

        return stringBuilder.ToString();
    }
}
