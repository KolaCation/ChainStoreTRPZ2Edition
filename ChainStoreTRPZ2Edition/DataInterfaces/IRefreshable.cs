using ChainStoreTRPZ2Edition.Messages;

namespace ChainStoreTRPZ2Edition.DataInterfaces
{
    /// <summary>
    ///     Interface to refresh VM's data before its rendering using Messenger
    /// </summary>
    public interface IRefreshable
    {
        void RefreshData(RefreshDataMessage refreshDataMessage);
    }
}