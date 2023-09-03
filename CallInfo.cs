
using AsterNET.Manager.Event;

namespace CheckBalance
{
    public class CallInfo
    {
        public string ChannelId { get; set; }
        public string CalledId { get; set; }
        public string CalledNumber{ get; set; }
        
        public NewChannelEvent InviteInfo { get; set; }
        public NewStateEvent AnswerInfo { get; set; }

        public CallInfo(string calledId, string channelId)
        {
            CalledId = calledId;
            ChannelId = channelId;
        }
    }
}