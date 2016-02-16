using System;
namespace PPlatform
{
    /// <summary>
    /// Interface for the network protocol (missing the join function still, not neccessary so far)
    /// </summary>
    interface INetgroup
    {
        void Close();
        void Open(string name, Action<ANetgroup.SignalingMessageType, int, string> lEventHandler);
        //void Join(string name, Action<global::PPlatform.Netgroup.SignalingMessageType, int, string> lEventHandler);
        void SendMessageTo(string message, int userid);
    }
}
