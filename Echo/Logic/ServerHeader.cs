
namespace Echo.Logic
{
    public abstract class ServerHeader
    {
        public const int KEY = 5;
	    public const int TOKEN = 10;
	    public const int INCOMINGCALL = 15;
	    public const int REGISTERSUCCESS = 20;
	    public const int ERROR = 25;
	    public const int REMOTEHANGUP = 30;
	    public const int RINGING = 35;
	    public const int CALLEEPICKUP = 40;
        public const int ANALYSING = 45;
        public const int VOICEPORT = 50;
        public const int TEXT = 55;
        public const int MORETEXT = 56;
    }
}
