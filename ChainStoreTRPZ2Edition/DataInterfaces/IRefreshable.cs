using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ChainStoreTRPZ2Edition.Messages;

namespace ChainStoreTRPZ2Edition.DataInterfaces
{
    /// <summary>
    /// Interface to refresh VM's data before its rendering using Messenger
    /// </summary>
    public interface IRefreshable
    {
        void RefreshData(RefreshDataMessage refreshDataMessage);
    }
}
