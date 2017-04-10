using Newtonsoft.Json;

namespace server_socketchat
{
    public enum SocketActions
    {
        ClientLogin, //Cliente intenta hacer login con un nombre de usuario
        ServerLogin, //Servidor envia respuesta del login
        ClientSendMessage, //Cliente envia mensaje a un chat
        ServerSendMessage, //Servidor envia mensaje a un chat (clientes/broadcast)
        ClientChat, //Cliente se establece en una sala de chat
        ClientGoodbye,
        CommandUsuaris
    }

    public class SocketAction
    {
        public SocketActions SocketActions  { get; set; }
        public string Message { get; set; }
        public bool Valid { get; set; }

        public SocketAction(SocketActions socketActions, string message, bool valid)
        {
            SocketActions = socketActions;
            Message = message;
            Valid = valid;
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static SocketAction Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<SocketAction>(json);
        }
    }
}
