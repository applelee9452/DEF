using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace DEF.UCenter;

public sealed class RSAPKCS1SHA256SignatureDescription : SignatureDescription
{
    public RSAPKCS1SHA256SignatureDescription()
    {
        //base.KeyAlgorithm = typeof(RSACryptoServiceProvider).FullName;
        //base.DigestAlgorithm = typeof(SHA256Managed).FullName;
        //base.FormatterAlgorithm = typeof(RSAPKCS1SignatureFormatter).FullName;
        //base.DeformatterAlgorithm = typeof(RSAPKCS1SignatureDeformatter).FullName;
    }

    public override AsymmetricSignatureDeformatter CreateDeformatter(AsymmetricAlgorithm key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        RSAPKCS1SignatureDeformatter deformatter = new(key);
        deformatter.SetHashAlgorithm("SHA256");
        return deformatter;
    }

    public override AsymmetricSignatureFormatter CreateFormatter(AsymmetricAlgorithm key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        RSAPKCS1SignatureFormatter formatter = new(key);
        formatter.SetHashAlgorithm("SHA256");
        return formatter;
    }
}

public class ContainerStatelessChannelWindowsStoreIAP : ContainerStateless, IContainerStatelessChannelWindowsStoreIAP
{
    IHttpClientFactory HttpClientFactory { get; set; }

    public override Task OnCreate()
    {
        HttpClientFactory = UCenterContext.Instance.HttpClientFactory;

        return Task.CompletedTask;
    }

    public override Task OnDestroy()
    {
        return Task.CompletedTask;
    }

    static bool ValidateXml(XmlDocument receipt, X509Certificate2 certificate)
    {
        // Create the signed XML object.
        SignedXml sxml = new(receipt);

        // Get the XML Signature node and load it into the signed XML object.
        XmlNode dsig = receipt.GetElementsByTagName("Signature", SignedXml.XmlDsigNamespaceUrl)[0];
        if (dsig == null)
        {
            // If signature is not found return false
            //System.Console.WriteLine("Signature not found.");
            return false;
        }

        sxml.LoadXml((XmlElement)dsig);

        // Check the signature
        bool isValid = sxml.CheckSignature(certificate, true);

        return isValid;
    }

    public async Task<X509Certificate2> RetrieveCertificate(string certificateId)
    {
        const int MaxCertificateSize = 10000;
        byte[] response_buffer = new byte[MaxCertificateSize];

        // Retrieve the certificate URL.
        string certificate_url = $"https://go.microsoft.com/fwlink/?LinkId=246509&cid={certificateId}";

        using (HttpClient hc = HttpClientFactory.CreateClient())
        {
            using var req = new HttpRequestMessage(HttpMethod.Get, certificate_url);

            using var res = await hc.SendAsync(req);
            if (res.IsSuccessStatusCode)
            {
                Stream resStream = res.Content.ReadAsStream();
                int bytesRead = ReadResponseBytes(response_buffer, resStream);

                if (bytesRead < 1)
                {
                    //TODO: Handle error here
                }
            }
            else
            {
                // Log Error
            }
        }

        return X509CertificateLoader.LoadCertificate(response_buffer);
    }

    // Utility function to read the bytes from an HTTP response
    static int ReadResponseBytes(byte[] responseBuffer, Stream resStream)
    {
        int count;
        int num_bytes_read = 0;
        int num_bytes_to_read = responseBuffer.Length;

        do
        {
            count = resStream.Read(responseBuffer, num_bytes_read, num_bytes_to_read);
            num_bytes_read += count;
            num_bytes_to_read -= count;
        } while (count > 0);

        return num_bytes_read;
    }

    async Task<WindowsStoreVerifyResponse> IContainerStatelessChannelWindowsStoreIAP.VerifyReceipt(PayVerifyChargeRequest request, string transaction, string iap_productid)
    {
        var response = new WindowsStoreVerifyResponse()
        {
            PayErrorCode = PayErrorCode.Error,
            Transaction = string.Empty,
            IAPProductId = string.Empty,
            IsSandbox = false,
        };

        // so register this algorithm for verification.
        CryptoConfig.AddAlgorithm(typeof(RSAPKCS1SHA256SignatureDescription), "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256");

        // Load the receipt that needs to be verified as an XML document
        XmlDocument xmlDoc = new();
        xmlDoc.LoadXml(request.Receipt);
        xmlDoc.Load("..\\..\\receipt.xml");

        // The certificateId attribute is present in the document root, retrieve it
        XmlNode node = xmlDoc.DocumentElement;
        string certificateId = node.Attributes["CertificateId"].Value;

        // Retrieve the certificate from the official site.
        // NOTE: For sake of performance, you would want to cache this certificate locally.
        //       Otherwise, every single call will incur the delay of certificate retrieval.
        X509Certificate2 verificationCertificate = await RetrieveCertificate(certificateId);

        try
        {
            // Validate the receipt with the certificate retrieved earlier
            bool isValid = ValidateXml(xmlDoc, verificationCertificate);
            //System.Console.WriteLine("Certificate valid: " + isValid);
            if (isValid)
            {
                response.Transaction = request.Transaction;
                response.IAPProductId = request.IAPProductId;
            }
            else
            {
                Logger.LogError("订单校验错误，没有需要校验的订单");
                response.PayErrorCode = PayErrorCode.PayInvalidReceipt;
                goto End;
            }
        }
        catch (Exception)
        {
            response.PayErrorCode = PayErrorCode.PayNetError;
            //System.Console.WriteLine(ex.ToString());
        }
        response.PayErrorCode = PayErrorCode.NoError;

    End:

        return response;
    }
}