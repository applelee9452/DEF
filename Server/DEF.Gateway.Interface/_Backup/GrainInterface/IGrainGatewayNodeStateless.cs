//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace DEF.Gateway;

//public interface IGrainGatewayNodeStateless : IGrainWithIntegerKey
//{
//    Task<string> RequestResponse(ushort method_id, byte[] data, string client_ip);

//    Task<string> RequestResponse2(string method_id, Dictionary<string, string> map_data, string client_ip);

//    Task<GatewayAuthResponse> RequestAuth(string acc_id, string token);
//}