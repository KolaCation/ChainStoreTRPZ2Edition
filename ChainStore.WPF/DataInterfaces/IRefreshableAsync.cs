using ChainStoreTRPZ2Edition.Messages;

namespace ChainStoreTRPZ2Edition.DataInterfaces
{
    /// <summary>
    ///     Async version of interface to refresh VM's data before its rendering using Messenger
    /// </summary>
    public interface IRefreshableAsync
    {
        void RefreshData(RefreshDataMessage refreshDataMessage);
    }
}